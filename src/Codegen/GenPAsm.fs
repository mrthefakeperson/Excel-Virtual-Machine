module Codegen.GenPAsm
open Utils
open CompilerDatatypes
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SemanticAST
open CompilerDatatypes.Semantics.RegisterAlloc
open CompilerDatatypes.Semantics.InterpreterValue
open CompilerDatatypes.PseudoASM
open CompilerDatatypes.PseudoASM.Flat
open Codegen.BuiltinFunctions

open Hooks

let generate_pasm: AST -> Asm list =
  let mutable next_label = 0
  let get_label s =
    try sprintf "%s_%i" s next_label
    finally next_label <- next_label + 1

  let get_highest_register (ast: AST) : int =
    let mutable acc = 0
    apply_mapping_hook (fun (|P|) (|Q|) -> function
      |V (Var(_, Local(reg, _))) ->
        acc <- max acc reg
        None
      |_ -> None
     ) ast
     |> ignore
    acc

  let rec addr_of: AST -> Asm list = function
    |V (Var(_, Local(reg, _))) -> [MovRHandle(R 0, HandleReg reg)]
    |V (Var(name, Global dt)) -> [MovRHandle(R 0, HandleLbl(name, dt))]
    |Apply(V (Var("*prefix", _)), [addr]) -> generate addr
    |Apply(V (Var("\cast", (Global dt | Strict dt))), [xpr]) -> addr_of xpr @ [Cast(DT.Ptr dt, R 0)]
    |unexpected -> failwithf "cannot deference %A" unexpected

  and generate: AST -> Asm list = function
    |Declare _ -> failwith "should never be reached"
    |Function _ -> failwith "should never be reached; handled in global"
    |V (Var(name, var_info)) ->
      match var_info with
      |Local(reg, _) -> [MovRR(R 0, R reg)]
      |Global (DT.Ptr _ as dt) -> [MovRHandle(R 0, HandleLbl(name, dt))]
      |Global dt -> [MovRM(R 0, Lbl(name, dt))]
    |V (Lit Boxed.Void) -> []
    |V (Lit value) -> [MovRC(R 0, value)]
    |If(cond, thn, els) ->
      let els_label = get_label "else"
      let cont_label = get_label "cont"
      match thn, els with
      |_, (V (Lit Boxed.Void) | Block []) ->
        generate cond @ [CmpC(R 0, Byte 0uy); Br0 els_label] @ generate thn @ [Label els_label]
      |(V (Lit Boxed.Void) | Block []), _ ->
        generate cond @ [CmpC(R 0, Byte 0uy); BrT cont_label] @ generate els @ [Label cont_label]
      |_ ->
        generate cond @ [CmpC(R 0, Byte 0uy); Br0 els_label]
         @ generate thn @ [Br cont_label; Label els_label]
         @ generate els @ [Label cont_label]
    |While(cond, body) ->
      let enter_loop_label = get_label "enter_loop"
      let loop_label = get_label "loop"
      match body with
      |V (Lit Boxed.Void) | Block [] ->
        [Label loop_label] @ generate cond @ [CmpC(R 0, Byte 0uy); BrT loop_label]
      |_ ->
        [Br enter_loop_label; Label loop_label] @ generate body
         @ [Label enter_loop_label] @ generate cond @ [CmpC(R 0, Byte 0uy); BrT loop_label]
    |Assign(a, b) ->
      match addr_of a with
      |[MovRHandle(R 0, HandleReg reg)] -> to_rn_from_r0 (R reg) (generate b)
      |[MovRHandle(R 0, HandleLbl(lbl, dt))] -> generate b @ [MovMR(Lbl(lbl, dt), R 0)]
      |instrs ->
        generate b @ [Push (R 0)] @ instrs @ [Pop RX; MovMR(Indirect (R 0), RX); MovRR(R 0, RX)]
    |Block xprs ->
      List.collect (function
        |Declare _ -> []
        |xpr -> drop_r0 (generate xpr)
       ) xprs
    |Return ast -> generate ast @ [MovRR(SP, BP); Ret]
    |Apply(f, args) ->
      let n_args = List.length args
      let push_args = List.rev args |> List.map generate |> List.collect push_r0
      let pop_args = [SubC(4, SP, Ptr(n_args, DT.Byte))]
      match f with
      |V (Var("\cast", (Global dt | Strict dt))) -> generate (List.exactlyOne args) @ [Cast(dt, R 0)]
      |V (Var("&prefix", _)) -> addr_of (List.exactlyOne args)
      // TEMP: does not currently alloc onto stack; TODO: scan for these calls and budget stack appropriately
      // TODO 2: allow alloc of a non-constant size
      |V (Var("\stack_alloc", (Global (DT.Function([DT.Int], dt)) | Strict dt))) ->
        let extract_int (Int sz | Strict sz) = sz
        let sz =
          List.map (Sim.InterpretAST.interpret_ast >> fst >> extract_int) args
           |> List.fold (*) 1
        [Alloc(sz, dt)]
      |V (Var(f, Global dt)) when List.exists (fst >> (=) f) SymbolTable.builtins ->
        BuiltinFunctions.generate f dt (List.map generate args)
      |V (Var(name, Global _)) ->
        [PushRealRs]
         @ push_args  // SP + regs + n_args
         @ [MovRR(BP, SP); AddC(4, BP, Ptr(1, DT.Byte))]  // copy into BP, add one for return addr
         @ [Call name]  // SP + regs + n_args + 1 before call, SP + regs + n_args after ret
         @ pop_args  // SP + regs
         @ [PopRealRs]  // back to original SP
      |unexpected -> failwithf "not a function: %A" unexpected
    |GlobalParse (Declare(_, (DT.Function _ | DT.Function2 _))::rest) -> generate (GlobalParse rest)
    |GlobalParse (Assign(V (Var(name, (Global (DT.Function _ | DT.Function2 _) | Strict))),
                         Function(_, _, body)
                  )::rest) ->
      let (Block _ | Strict) = body
      let body_instrs = generate body
      let body_with_setup_teardown =
        let stack_reset = get_highest_register body
        let setup = if stack_reset = 0 then [] else [AddC(4, SP, Ptr(stack_reset, DT.Byte))]
        let teardown = if stack_reset = 0 then [Ret] else [MovRR(SP, BP); Ret]
        setup @ body_instrs @ teardown
      [Label name] @ body_with_setup_teardown @ generate (GlobalParse rest)
    |GlobalParse (Declare(name, dt)::rest) ->
      let grouped_assign = function
        |Assign(V (Var(n2, (Global t2 | Strict t2))), _)
        |Assign(Apply(V (Var("*prefix", _)),
                 [Apply(V (Var("+", _)), [V (Var(n2, (Global t2 | Strict t2))); _])]), _)
          when name = n2 && dt = t2 -> true
        |Assign _ as x -> failwithf "invalid global value initialization: %A" x
        |_ -> false
      let assigns = List.takeWhile grouped_assign rest
      let rest = List.skipWhile grouped_assign rest
      let init_data =
        let extract_assign (Assign(_, xpr) | Strict xpr) = xpr
        match assigns with
        |[] -> [|Boxed.default_value dt|]
        |Assign(_, BuiltinASTs.StackAlloc(dt', [sz_ast]))::init_values ->
          let empty =
            let Int sz | Strict sz = fst (Sim.InterpretAST.interpret_ast sz_ast)
            Array.create sz (Boxed.default_value dt')
          let data =
            Array.ofList init_values
             |> Array.map (extract_assign >> Sim.InterpretAST.interpret_ast >> fst)
          Array.append data (Array.skip data.Length empty)
        |init_values ->
          Array.ofList init_values
           |> Array.map (extract_assign >> Sim.InterpretAST.interpret_ast >> fst)
      [Label name] @ [Data init_data] @ generate (GlobalParse rest)
    |GlobalParse [] -> []
    |GlobalParse (hd::_) -> failwithf "invalid global expression: %A" hd

  (@) [Br "main"] << generate