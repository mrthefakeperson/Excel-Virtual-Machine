module Codegen.Main
open Parser.AST
open Codegen.Tables
open Codegen.PAsm
open Codegen.TypeCheck
open Codegen.Interpreter
open Codegen.BuiltinFunctions

let next_lblname = ref 1
let get_lblname() =
  try !next_lblname
  finally incr next_lblname

let get_highest_register: Boxed Asm list -> int =
  List.fold (fun acc_max -> function
    |MovRR(r1, r2) | MovRM(r1, Indirect r2) | MovMR(Indirect r1, r2) ->
      let acc_max' = match r1 with R n -> max n acc_max | _ -> acc_max
      match r2 with R n -> max n acc_max' | _ -> acc_max'
    |Push r | Pop r | MovRM(r, _) | MovMR(_, r) ->
      match r with R n -> max n acc_max | _ -> acc_max
    |_ -> acc_max
   ) 0

let addr_of (symtbl: SymbolTable) = function
  |Value(Var(name, _)) when symtbl.check_global_var name -> Choice1Of3 (Lbl name)
  |Value(Var(name, t)) ->
    let r, _ = symtbl.check_var name t
    Choice2Of3 (R r)
  |Apply(Value(Var("*prefix", _)), [addr]) -> Choice3Of3 addr
  |Index(a, i) -> Choice3Of3 (Apply(Value(Var("+", tf_arith_infix)), [a; i]))
  |unexpected -> failwithf "cannot deference %A" unexpected

// after each expression, the result should be in R0 (if there is one)
let rec generate (symtbl: SymbolTable) = function
  |Apply(f, xs) ->
    let get_args_r0 = List.map (generate symtbl) xs
    let push_args = List.collect push_r0 get_args_r0
    match f with
    // hack - adding the 0 value of a type different to what's in R0 will cast R0 to a different type
    |Value(Var("\cast", t)) -> List.exactlyOne get_args_r0 @ [AddC(R 0, Boxed.default_value t)]
    |Value(Var(name, _)) when symtbl.check_global_var name ->
      [PushRealRs] @ push_args @ [MovRR(BP, SP); Call name; PopRealRs]
    |Value(Var(f, _)) when List.exists ((=) f) (List.map fst Tables.builtins) -> BuiltinFunctions.generate f get_args_r0
    |unexpected -> failwithf "not a function: %A" unexpected
  |Assign(a, b) ->
    match addr_of symtbl a with
    |Choice1Of3 mem -> generate symtbl b @ [MovMR(mem, R 0)]
    |Choice2Of3 reg -> to_rn_from_r0 reg (generate symtbl b)
    |Choice3Of3 ast -> generate symtbl b @ [Push (R 0)] @ generate symtbl ast @ [MovMR(Indirect(R 0), SP); Pop (R 0)]
  |Index(a, i) ->
    generate symtbl (Apply(Value(Var("*prefix", t_any)), [Apply(Value(Var("+", tf_arith_infix)), [a; i])]))
  |Declare _ | DeclareHelper _ -> failwith "should never be reached; handled in block"
  |Return ast -> generate symtbl ast @ [Ret]
  |Block xprs ->
    let rec gen_block (symtbl: SymbolTable) = function
      |DeclareHelper decls::rest -> gen_block symtbl (decls @ rest)
      |Declare(name, (Pointer(_, Some sz) as t))::rest ->
        let symtbl' = symtbl.register_var name t
        [Alloc sz; MovRR(R (fst (symtbl'.check_var name t)), R 0)] @ gen_block symtbl' rest
      |Declare(name, t)::rest -> gen_block (symtbl.register_var name t) rest
      |xpr::rest -> generate symtbl xpr @ gen_block symtbl rest
      |[] -> []
    gen_block symtbl xprs
  |If(cond, thn, els) ->
    let els_label, cont_label = sprintf "else_%i" (get_lblname()), sprintf "cont_%i" (get_lblname())
    match thn, els with
    |_, (Value Unit | Block []) -> generate symtbl cond @ [Br0 els_label] @ generate symtbl thn @ [Label els_label]
    |(Value Unit | Block []), _ -> generate symtbl cond @ [BrT cont_label] @ generate symtbl els @ [Label cont_label]
    |_ -> generate symtbl cond @ [Br0 els_label] @ generate symtbl thn @ [Br cont_label; Label els_label] @ generate symtbl els @ [Label cont_label]
  |While(cond, body) ->
    let enter_loop_label, loop_label = sprintf "enter_loop_%i" (get_lblname()), sprintf "loop_%i" (get_lblname())
    match body with
    |Value Unit | Block [] -> [Label loop_label] @ generate symtbl cond @ [BrT loop_label]
    |_ -> [Br enter_loop_label; Label loop_label] @ generate symtbl body @ [Label enter_loop_label] @ generate symtbl cond @ [BrT loop_label]
  |Function _ -> failwith "should never be reached; handled in global"
  |Value(Var(name, typ)) ->
    let reg, sz = symtbl.check_var name typ
    [MovRR(R 0, R reg)]
  |Value(Lit _) as v ->
    let x = eval_ast (default_memory()) v
    [MovRC(R 0, x)]
  |Value Unit -> []
  |GlobalParse xprs ->
    let rec gen_global (symtbl: SymbolTable) = function
      |Declare(name, t)::Assign(Value(Var(n2, t2)), Function(args, (Block xprs as body)))::rest when name = n2 && t = t2 ->
        let symtbl' = symtbl.register_global_var name t  // should only have global variables
        let symtbl' =
          List.fold2 (fun symtbl i (name, t) ->
            {symtbl with var_to_type = symtbl.var_to_type.Add(name, t); var_to_register = (name, i)::symtbl.var_to_register}
           ) symtbl' [-List.length args .. -1] args
        let body_instrs = generate symtbl' body
        match get_highest_register body_instrs with
        |0 -> [Label name] @ body_instrs @ gen_global symtbl' rest @ [Ret]
        |x -> [Label name; AddC(SP, Int x)] @ body_instrs @ gen_global symtbl' rest @ [Ret]
      |Declare _::(Assign(_, Function _) as f)::_ -> failwithf "invalid global function definition: %A" f
      |Declare(name, t)::(Assign _::_ as rest) ->
        let grouped_assign = function
          |Assign(Value(Var(n2, t2)), _) | Assign(Index(Value(Var(n2, t2)), _), _) when name = n2 && t = t2 -> true
          |Assign _ as x -> failwithf "invalid global value initialization: %A" x
          |_ -> false
        let assigns, rest = Array.takeWhile grouped_assign (Array.ofList rest), List.skipWhile grouped_assign rest
        let data =
          Array.map (function
            |Assign(_, xpr) -> eval_ast (default_memory()) xpr  // TODO: don't use default memory, import definitions
            |_ -> failwith "should never be reached"
           ) assigns
        [Label name] @ [Data data] @ gen_global (symtbl.register_global_var name t) rest
      |Declare(name, t)::rest ->
        [Label name] @ [Data [|Boxed.default_value t|]] @ gen_global (symtbl.register_global_var name t) rest
      |[] -> []
      |unexpected -> failwithf "unexpected global: %A" unexpected
    [Br "main"] @ gen_global symtbl xprs


let generate_from_ast' = check_type empty_symbol_table >> fst >> generate empty_symbol_table
let generate_from_ast x =
  next_lblname := 1
  generate_from_ast' x