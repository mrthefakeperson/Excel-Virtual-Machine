module Codegen.Main
open Parser.Datatype
open Parser.AST
open Parser.AST.Hooks
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
    |Push r | Pop r | MovRM(r, _) | MovMR(_, r) | MovRC(r, _) | MovRHandle(r, _) ->
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
    |Global sz -> [MovRHandle(R 0, HandleLbl(name, t))]
  |Apply(Value(Var("*prefix", _)), [addr]) -> generate symtbl addr
  |Apply(Value(Var("\cast", t)), [xpr]) -> addr_of symtbl xpr @ [Cast(DT.Ptr t, R 0)]
  |Index _ -> failwith "should never be reached"
  |unexpected -> failwithf "cannot deference %A" unexpected

// after each expression, the result should be in R0 (if there is one)
and generate (symtbl: SymbolTable) = function
  |Apply(f, xs) ->
    let get_args_r0 = List.map (generate symtbl) xs
    let push_args = List.collect push_r0 get_args_r0
    let pop_args = SubC(4, SP, Ptr(List.length xs, DT.Byte))
    match f with
    |Value(Var("\cast", t)) -> List.exactlyOne get_args_r0 @ [Cast(t, R 0)]
    |Value(Var("&prefix", _)) -> addr_of symtbl (List.exactlyOne xs)
    // TEMP: does not currently alloc onto stack; TODO: scan for these calls and budget stack appropriately
    // TODO 2: allow alloc of a non-constant size
    |Value(Var("\stack_alloc", DT.Function([DT.Int], dt))) ->
      let Int sz | Strict sz = eval_ast (default_memory()) (List.exactlyOne xs)
      [Alloc(sz, dt)]
    |Value(Var(name, _)) when symtbl.check_global_var name ->
      [PushRealRs] @ push_args @ [MovRR(BP, SP); AddC(4, BP, Ptr(1, DT.Byte)); Call name; pop_args; PopRealRs]  // sp is full, so shift BP by one more
    |Value(Var(f, dt)) when List.exists ((=) f) (List.map fst Tables.builtins) ->
      BuiltinFunctions.generate f dt get_args_r0
    |unexpected -> failwithf "not a function: %A" unexpected
  |Assign(a, b) ->
    match addr_of symtbl a with
    |[MovRHandle(R 0, HandleLbl(lbl, dt))] -> generate symtbl b @ [MovMR(Lbl(lbl, dt), R 0)]
    |[MovRHandle(R 0, HandleReg reg)] -> to_rn_from_r0 (R reg) (generate symtbl b)
    |instrs -> generate symtbl b @ [Push (R 0)] @ instrs @ [Pop RX; MovMR(Indirect(R 0), RX); MovRR(R 0, RX)]
  |Index _ -> failwith "should never be reached"
  |Declare _ | DeclareHelper _ -> failwith "should never be reached; handled in block"
  |Return ast ->
    match ast with
    |Apply(Value(Var(name, _)), args) when symtbl.check_global_var name ->  // tailcall (partial optimization, experimental)
      let push_args = List.collect push_r0 (List.map (generate symtbl) args)
      let num_args = List.length args
      push_args @ [MovRR(BP, SP); AddC(4, BP, Ptr(1, DT.Byte))] @ [Call name] @ [SubC(4, SP, Ptr(num_args, DT.Byte))]
       @ [SubC(4, SP, Ptr(RETURN_STACKRESET_DEFAULT, DT.Byte)); Ret]
    |_ -> generate symtbl ast @ [SubC(4, SP, Ptr(RETURN_STACKRESET_DEFAULT, DT.Byte)); Ret]
  |Block xprs ->
    let rec gen_block (symtbl: SymbolTable) = function
      |DeclareHelper decls::rest -> gen_block symtbl (decls @ rest)
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
    match symtbl.check_var name typ, typ with
    |Local(reg, sz), _ -> [MovRR(R 0, R reg)]
    |Global sz, DT.Ptr _ -> [MovRHandle(R 0, HandleLbl(name, typ))]  // unsure if this branch on typ works
    |Global sz, _ -> [MovRM(R 0, Lbl(name, typ))]
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
            { symtbl with
                var_to_type = symtbl.var_to_type.Add(name, t)
                var_to_register = (name, i)::symtbl.var_to_register }
           ) symtbl' [-num_args .. -1] args
        let body_instrs = generate symtbl'' body
        let body_with_setup_teardown =
          let stack_reset = get_highest_register body_instrs
          let setup = if stack_reset = 0 then [] else [AddC(4, SP, Ptr(stack_reset, DT.Byte))]
          let teardown = if stack_reset = 0 then [Ret] else [SubC(4, SP, Ptr(stack_reset, DT.Byte)); Ret]
          let body_stack_reset =
            List.choose (function
              |SubC(4, SP, Ptr(RETURN_STACKRESET_DEFAULT, DT.Byte)) when stack_reset = 0 -> None
              |SubC(4, SP, Ptr(RETURN_STACKRESET_DEFAULT, DT.Byte)) -> Some <| SubC(4, SP, Ptr(stack_reset, DT.Byte))
              |ShiftStackDown _ & Strict x -> x
              |c -> Some c
             ) body_instrs
          setup @ body_stack_reset @ teardown
        let rest_instrs = gen_global symtbl'' rest
        let is_prototype = List.exists ((=) (Label name)) rest_instrs
        if is_prototype
         then rest_instrs
         else [Label name] @ body_with_setup_teardown @ gen_global symtbl'' rest
      |Declare _::(Assign(_, Function _) as f)::_ -> failwithf "invalid global function definition: %A" f
      |Declare(name, t)::(Assign _::_ as rest) ->
        let grouped_assign = function
          |Assign(Value(Var(n2, t2)), _)
          |Assign(Apply(Value(Var("*prefix", _)), [Apply(Value(Var("+", _)), [Value(Var(n2, t2)); _])]), _) when name = n2 && t = t2 -> true
          |Assign _ as x -> failwithf "invalid global value initialization: %A" x
          |_ -> false
        let assigns, rest = List.takeWhile grouped_assign rest, List.skipWhile grouped_assign rest
        let data =
          match assigns with
          |[Assign(_, Apply(Value(Var("\stack_alloc", DT.Function([DT.Int], dt'))), [sz_ast]))] ->
            let Int sz | Strict sz = eval_ast (default_memory()) sz_ast
            Array.create (sz * dt'.sizeof) (Boxed.default_value dt')
          |assigns ->  // TODO: don't use default memory, import definitions
            Array.map (function Assign(_, xpr) | Strict xpr -> eval_ast (default_memory()) xpr) (Array.ofList assigns)
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
   |> apply_mapping_hook convert_index_hook
   |> check_type (empty_symbol_table()) |> fst
   |> apply_mapping_hook scale_ptr_offsets_hook
   |> generate (empty_symbol_table())