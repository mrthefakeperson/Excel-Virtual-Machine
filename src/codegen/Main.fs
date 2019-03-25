module Codegen.Main
open Parser.Datatype
open Parser.AST
open Codegen.Tables
open Codegen.PAsm
open Codegen.PAsm.Flat
open Codegen.Hooks
open Codegen.TypeCheck
open Codegen.Interpreter
open Codegen.BuiltinFunctions

let get_highest_register: Asm list -> int =
  List.fold (fun acc_max -> function
    |MovRR(r1, r2) | MovRM(r1, Indirect r2) | MovMR(Indirect r1, r2) ->
      let acc_max' = match r1 with R n -> max n acc_max | _ -> acc_max
      match r2 with R n -> max n acc_max' | _ -> acc_max'
    |Push r | Pop r | MovRM(r, _) | MovMR(_, r) ->
      match r with R n -> max n acc_max | _ -> acc_max
    |_ -> acc_max
   ) 0

// placeholder for the amount of space to revert the stack upon returning from a function (local vars use the stack)
[<Literal>]
let RETURN_STACKRESET_DEFAULT = -7777777

let rec addr_of (symtbl: SymbolTable) = function
  |Value(Var(name, t)) ->
    match symtbl.check_var name t with
    |Local(reg, sz) -> [MovRHandle(R 0, HandleReg reg)]
    |Global sz -> [MovRHandle(R 0, HandleLbl name)]
  |Apply(Value(Var("*prefix", _)), [addr]) -> generate symtbl addr
  |Apply(Value(Var("\cast", t)), [xpr]) -> addr_of symtbl xpr @ [AddC(R 0, Ptr(0, DT.Ptr t))]
  |Index(a, i) -> generate symtbl (Apply(Value(Var("+", TypeClasses.f_arith_infix)), [a; i]))  // probably doesn't work
  |unexpected -> failwithf "cannot deference %A" unexpected

// after each expression, the result should be in R0 (if there is one)
and generate (symtbl: SymbolTable) = function
  |Apply(f, xs) ->
    let get_args_r0 = List.map (generate symtbl) xs
    let push_args = List.collect push_r0 get_args_r0
    match f with
    // hack - adding the 0 value of a type different to what's in R0 will cast R0 to a different type
    |Value(Var("\cast", t)) -> List.exactlyOne get_args_r0 @ [AddC(R 0, Boxed.default_value t)]
    |Value(Var("&prefix", _)) -> addr_of symtbl (List.exactlyOne xs)
    |Value(Var(name, _)) when symtbl.check_global_var name ->
      [PushRealRs] @ push_args @ [MovRR(BP, SP); Call name; PopRealRs]
    |Value(Var(f, _)) when List.exists ((=) f) (List.map fst Tables.builtins) ->
      BuiltinFunctions.generate f get_args_r0
    |unexpected -> failwithf "not a function: %A" unexpected
  |Assign(a, b) ->
    match addr_of symtbl a with
    |[MovRHandle(R 0, HandleLbl lbl)] -> generate symtbl b @ [MovMR(Lbl lbl, R 0)]
    |[MovRHandle(R 0, HandleReg reg)] -> to_rn_from_r0 (R reg) (generate symtbl b)
    |instrs -> generate symtbl b @ [Push (R 0)] @ instrs @ [Pop RX; MovMR(Indirect(R 0), RX); MovRR(R 0, RX)]
  |Index(a, i) ->
    generate symtbl (Apply(Value(Var("*prefix", TypeClasses.any)), [Apply(Value(Var("+", TypeClasses.any)), [a; i])]))  // probably doesn't work
  |Declare _ | DeclareHelper _ -> failwith "should never be reached; handled in block"
  |Return ast ->
    match ast with
    |Apply(Value(Var(name, _)), args) when symtbl.check_global_var name ->  // tailcall
      let push_args = List.collect push_r0 (List.map (generate symtbl) args)
      let num_args = List.length args
      push_args @ [SubC(SP, Int RETURN_STACKRESET_DEFAULT)]  // restore stack to original position + pushed args
       @ [ShiftStackDown(RETURN_STACKRESET_DEFAULT - num_args, 0); MovRR(BP, SP); Br name]
    |_ -> generate symtbl ast @ [SubC(SP, Int RETURN_STACKRESET_DEFAULT); Ret]
  |Block xprs ->
    let rec gen_block (symtbl: SymbolTable) = function
      |DeclareHelper decls::rest -> gen_block symtbl (decls @ rest)
      //|Declare(name, (Ptr(_, Some sz) as t))::rest ->  // TODO: dynamic alloc
      //  let symtbl' = symtbl.register_var name t
      //  match symtbl'.check_var name t with
      //  |Local(reg, _) -> [Alloc sz; MovRR(R reg, R 0)] @ gen_block symtbl' rest
      //  |_ -> failwith "should never be reached"
      |Declare(name, t)::rest -> gen_block (symtbl.register_var name t) rest
      |xpr::rest -> drop_r0 (generate symtbl xpr) @ gen_block symtbl rest
      |[] -> []
    gen_block symtbl xprs
  |If(cond, thn, els) ->
    let els_label, cont_label = symtbl.get_label "else", symtbl.get_label "cont"
    match thn, els with
    |_, (Value(Lit(_, DT.Void)) | Block []) ->
      generate symtbl cond @ [CmpC(R 0, Byte 0uy); Br0 els_label] @ generate symtbl thn @ [Label els_label]
    |(Value(Lit(_, DT.Void)) | Block []), _ ->
      generate symtbl cond @ [CmpC(R 0, Byte 0uy); BrT cont_label] @ generate symtbl els @ [Label cont_label]
    |_ ->
      generate symtbl cond @ [CmpC(R 0, Byte 0uy); Br0 els_label]
       @ generate symtbl thn @ [Br cont_label; Label els_label]
       @ generate symtbl els @ [Label cont_label]
  |While(cond, body) ->
    let enter_loop_label, loop_label = symtbl.get_label "enter_loop", symtbl.get_label "loop"
    match body with
    |Value(Lit(_, DT.Void)) | Block [] -> [Label loop_label] @ generate symtbl cond @ [CmpC(R 0, Byte 0uy); BrT loop_label]
    |_ ->
      [Br enter_loop_label; Label loop_label] @ generate symtbl body
       @ [Label enter_loop_label] @ generate symtbl cond @ [CmpC(R 0, Byte 0uy); BrT loop_label]
  |Function _ -> failwith "should never be reached; handled in global"
  |Value(Var(name, typ)) ->
    match symtbl.check_var name typ with
    |Local(reg, sz) -> [MovRR(R 0, R reg)]
    |Global sz -> [MovRM(R 0, Lbl name)]
  |Value(Lit(_, DT.Void)) -> []
  |Value(Lit _) as v ->
    let x = eval_ast (default_memory()) v
    [MovRC(R 0, x)]
  |GlobalParse xprs ->
    let rec gen_global (symtbl: SymbolTable) = function
      |Declare(name, t)::Assign(Value(Var(n2, t2)), Function(args, (Block _ as body)))::rest when name = n2 && t = t2 ->
        let num_args = List.length args
        let symtbl' = symtbl.register_global_var name t  // should only have global variables
        let symtbl'' =
          List.fold2 (fun symtbl i (name, t) ->
            {symtbl with var_to_type = symtbl.var_to_type.Add(name, t); var_to_register = (name, i)::symtbl.var_to_register}
           ) symtbl' [-num_args .. -1] args
        let body_instrs = generate symtbl'' body
        let body_with_setup_teardown =
          match get_highest_register body_instrs with
          |0 ->
            List.filter
             (function SubC(SP, Int RETURN_STACKRESET_DEFAULT) | ShiftStackDown(_, 0) -> false | _ -> true)
             body_instrs
             @ [Ret]
          |x ->
            [AddC(SP, Int x)]
             @ List.map (function
                 |SubC(SP, Int RETURN_STACKRESET_DEFAULT) -> SubC(SP, Int x)
                 |ShiftStackDown(offset, 0) -> ShiftStackDown(offset - RETURN_STACKRESET_DEFAULT + x, num_args)
                 |c -> c
                ) body_instrs
             @ [SubC(SP, Int x); Ret]
        let rest_instrs = gen_global symtbl'' rest
        let is_prototype = List.exists ((=) (Label name)) rest_instrs
        if is_prototype
         then rest_instrs
         else [Label name] @ body_with_setup_teardown @ gen_global symtbl'' rest
      |Declare _::(Assign(_, Function _) as f)::_ -> failwithf "invalid global function definition: %A" f
      |Declare(name, t)::(Assign _::_ as rest) ->
        let grouped_assign = function
          |Assign(Value(Var(n2, t2)), _) | Assign(Index(Value(Var(n2, t2)), _), _) when name = n2 && t = t2 -> true
          |Assign _ as x -> failwithf "invalid global value initialization: %A" x
          |_ -> false
        let assigns, rest = List.takeWhile grouped_assign rest, List.skipWhile grouped_assign rest
        match assigns with
        |Assign(_, Apply(Value(Var("\stack_alloc", _)), _))::assigns | assigns ->
          let data =
            Array.map (function
              |Assign(_, xpr) -> eval_ast (default_memory()) xpr  // TODO: don't use default memory, import definitions
              |_ -> failwith "should never be reached"
             ) (Array.ofList assigns)
          [Label name] @ [Data data] @ gen_global (symtbl.register_global_var name t) rest
      |Declare(name, t)::rest ->
        [Label name] @ [Data [|Boxed.default_value t|]] @ gen_global (symtbl.register_global_var name t) rest
      |[] -> []
      |unexpected -> failwithf "unexpected global: %A" unexpected
    [Br "main"] @ gen_global symtbl xprs


let generate_from_ast'() = check_type (empty_symbol_table()) >> fst >> generate (empty_symbol_table())
let generate_from_ast x =
  x
   |> apply_mapping_hook transform_sizeof_hook
   |> apply_mapping_hook extract_strings_to_global_hook
   |> apply_mapping_hook convert_logic_hook
   // no prototype hook here - just assume it uses the last label
   |> check_type (empty_symbol_table()) |> fst
   |> generate (empty_symbol_table())