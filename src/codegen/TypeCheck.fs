module Codegen.TypeCheck
open Parser.Datatype
open Parser.AST
open Codegen.Tables
open System.Collections.Generic

let cached = Dictionary()
let ret (yld, dt) =
  cached.[yld] <- dt
  (yld, dt)

let rec check_type (symtbl: SymbolTable) = function
  |Value(Var(name, typ)) ->
    ignore <| symtbl.check_var name typ
    let t = symtbl.var_to_type.[name]
    ret (Value(Var(name, t)), t)
  |Value(Lit(_, t)) as ast -> ast, t
  |Declare _ -> failwith "should never be reached"
  |Apply(f, args) ->
    let args', discovered_arg_types = List.unzip (List.map (check_type symtbl) args)
    match check_type symtbl f with
    |Value(Var(fname, _)), (DT.Function(_, ret_type) as ftype) ->
      let ftype' =
        match f with
        |Value(Var(_, (DT.Function _ | Function2 _ as hint))) -> DT.infer_type hint (DT.Function(discovered_arg_types, ret_type))
        |_ -> DT.Function(discovered_arg_types, ret_type)
      let ftype = DT.infer_type ftype ftype'
      let (DT.Function(inferred_arg_types, inferred_ret_type) | Strict(inferred_arg_types, inferred_ret_type)) = ftype
      let args'' = 
        List.map3 (fun arg expected_type discovered_type ->
          if expected_type = discovered_type then arg
          else BuiltinASTs.cast (DT.infer_type expected_type discovered_type) arg
         ) args' inferred_arg_types discovered_arg_types
      ret (Apply(Value(Var(fname, ftype)), args''), inferred_ret_type)
    |f', DT.Function2 ret_type -> ret (Apply(f', args'), ret_type)
    |_ -> failwithf "%A is not a function" f
  |Assign(l, r) ->
    match check_type symtbl l with
    |l', (DT.Function(_, ret_type) | Function2 ret_type as tl) ->
      let r', tr = check_type {symtbl with return_type = Some ret_type} r
      ret (Assign(l', r'), DT.infer_type tl tr)
    |l', tl ->
      let r', tr = check_type symtbl r
      let r'' = if tr = tl then r' else BuiltinASTs.cast tl r'
      ret (Assign(l', r''), DT.infer_type tl tr)
  |Index(a, i) ->
    let i', ti = check_type symtbl i
    ignore (DT.infer_type Int ti)
    let i'' = if ti = Int then i' else BuiltinASTs.cast Int i'
    match check_type symtbl a with
    |a', Ptr t -> ret (Index(a', i''), t)
    //  Apply(Value(Var("*prefix", DT.Function([Ptr t], t))), [Apply()])
    |_ -> failwith "not indexable"
  |DeclareHelper _ -> failwith "should never be reached, handled in block"
  |Return ast ->
    let ast', t = check_type symtbl ast
    match symtbl.return_type with
    |Some rt when t <> rt -> ret (Return (BuiltinASTs.cast rt ast'), rt)
    |_ -> ret (Return ast', t)
  |Block xprs ->
    let xprs' =
      List.collect (function DeclareHelper xprs -> xprs | xpr -> [xpr]) xprs
       |> List.fold (fun (acc_tbl: SymbolTable, acc_xprs) -> function
            |Declare(name, t) as xpr -> acc_tbl.register_var name t, xpr::acc_xprs
            |xpr ->
              let xpr', _ = check_type acc_tbl xpr
              acc_tbl, xpr'::acc_xprs
           ) (symtbl, [])
       |> snd
       |> List.rev
    ret (Block xprs', Void)
  |If(cond, thn, els) ->
    let cond', condt = check_type symtbl cond
    ignore (DT.infer_type Byte condt)
    let cond'' = if condt = Byte then cond' else BuiltinASTs.cast Byte cond'
    let (thn', thnt), (els', elst) = check_type symtbl thn, check_type symtbl els
    if thnt = elst
     then If(cond'', thn', els'), thnt
     else
      try
        ignore (DT.infer_type' elst thnt)
        ret (If(cond'', BuiltinASTs.cast elst thn', els'), elst)
      with _ ->
        ignore (DT.infer_type' thnt elst)
        ret (If(cond'', thn', BuiltinASTs.cast thnt els'), thnt)
  |While(cond, body) ->
    let (cond', condt), (body', _) = check_type symtbl cond, check_type symtbl body
    ignore (DT.infer_type Byte condt)
    let cond'' = if condt = Byte then cond' else BuiltinASTs.cast Byte cond'
    ret (While(cond'', body'), Void)
  |Function(args, body) ->
    let arg_types = List.map snd args
    let body', _ = check_type (List.fold (fun tbl (n, t) -> tbl.register_var n t) symtbl args) body
    ret (Function(args, body'), DT.Function(arg_types, TypeClasses.any))
  |GlobalParse xprs ->
    let (Block xprs', Void | Strict xprs') = check_type symtbl (Block xprs)
    ret (GlobalParse xprs', Void)