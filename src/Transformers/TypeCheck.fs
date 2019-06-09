module Transformers.TypeCheck
open System.Collections.Generic
open Utils
open CompilerDatatypes.Token
open CompilerDatatypes.DT
open CompilerDatatypes.AST
open CompilerDatatypes.AST.Hooks
open Transformers.Tables

let resolve_type_aliases ast : AST =
  let aliases = Dictionary()
  let resolve = function
    |TypeDef (Alias _ | Struct _ | Union _ as td) -> aliases.[td]
    |t -> t
  let hook: MappingASTHook = fun (|P|) (|Q|) -> function
    |Declare(vname, TypeDef (DeclAlias(tname, dt))) ->
      aliases.Add(Alias tname, resolve dt)
      Some (Declare(vname, aliases.[Alias tname]))
    |Declare(vname, (TypeDef (DeclStruct(tname, _) | DeclUnion(tname, _)) as dt)) ->
      aliases.Add(Struct tname, dt)
      Some (Declare(vname, dt))
    |Declare(vname, dt) -> Some (Declare(vname, resolve dt))
    |V (Var(vname, dt)) -> Some (V (Var(vname, resolve dt)))
    |Function(ret_t, args, P body) ->
      let resolved_args = List.map (fun (arg, arg_t) -> (arg, resolve arg_t)) args
      Some (Function(resolve ret_t, resolved_args, body))
    |_ -> None
  apply_mapping_hook hook ast

let rec infer_types (symtbl: SymbolTable) : AST -> AST * DT = function
  |V (Var(vname, dt)) ->
    match symtbl.get_var vname with
    |Local(_, dt') | Global dt' ->
      ignore (DT.infer_type dt dt')
      (V (Var(vname, dt')), dt')
  |V (Lit(_, dt)) as ast -> (ast, dt)
  |Declare _ -> failwith "should never be reached"
  |Apply(V (Var(".", _)), [xpr; (V (Var(field, _)) | Strict field)]) ->
    let (xpr, txpr) = infer_types symtbl xpr
    match txpr with
    |TypeDef (DeclStruct(_, struct_fields) | DeclUnion(_, struct_fields)) ->
      List.tryPick (function 
        |StructField(fld_name, fld_dt, start_bit, sz_bytes) when field = fld_name ->
          Some(fld_dt, start_bit, sz_bytes)
        |_ -> None
       ) struct_fields
       |> Option.defaultWith (fun () -> failwithf "type %A has no property %s" txpr field)
       |> fun (fld_dt, start_bit, _) ->
            let byte_index = V (Lit(string (start_bit / 8), DT.Int))  // TODO: get proper bit alignment
            (BuiltinASTs.index xpr byte_index, fld_dt)
    |_ -> failwithf "type %A has no property %s" txpr field
  |Apply(f, args) ->
    let (args, inferred_given_arg_types) = List.unzip (List.map (infer_types symtbl) args)
    match infer_types symtbl f with
    |V (Var(fname, _)), (DT.Function(_, ret_type) as ftype) ->
      let inferred_ftype = DT.Function(inferred_given_arg_types, ret_type)
      let ftype =
        match f with
        |V (Var(_, (DT.Function _ | Function2 _ as hint))) ->
          DT.infer_type hint inferred_ftype
           |> DT.infer_type ftype
        |_ -> DT.infer_type ftype inferred_ftype
      let (DT.Function(sign_arg_types, inferred_ret_type)
          |Strict(sign_arg_types, inferred_ret_type)) = ftype
      let args =
        List.map3 (fun arg inferred_type sign_type ->
          if inferred_type = sign_type
           then arg
           else BuiltinASTs.cast (DT.infer_type inferred_type sign_type) arg
         ) args inferred_given_arg_types sign_arg_types
      (Apply(V (Var(fname, ftype)), args), inferred_ret_type)
    |f, Function2 ret_type -> (Apply(f, args), ret_type)
    |_ -> failwithf "%A is not a function" f
  |Assign(l, r) ->
    let (l, tl) = infer_types symtbl l
    let (r, tr) = infer_types symtbl r
    let r = if tr = tl then r else BuiltinASTs.cast tl r
    (Assign(l, r), DT.infer_type tl tr)
  |Return ast ->
    let ast, t = infer_types symtbl ast
    let (Some rt | Strict rt) = symtbl.return_type
    if t = rt
     then (Return ast, t)
     else (Return (BuiltinASTs.cast rt ast), rt)
  |Block xprs ->
    let (xprs, _) =
      List.mapFold (fun (acc_tbl: SymbolTable) -> function
        |Declare(vname, dt) as xpr ->
          let with_new_var = acc_tbl.register_local_var vname dt
          (xpr, with_new_var)
        |xpr ->
          let (inferred_xpr, _) = infer_types acc_tbl xpr
          (inferred_xpr, acc_tbl)
       ) symtbl xprs
    (Block xprs, Void)
  |If(cond, thn, els) ->
    let (cond, condt) = infer_types symtbl cond
    ignore (DT.infer_type Byte condt)
    let cond = if condt = Byte then cond else BuiltinASTs.cast Byte cond
    let (thn, thnt) = infer_types symtbl thn
    let (els, elst) = infer_types symtbl els
    if thnt = elst
     then If(cond, thn, els), thnt
     else
      try
        ignore (DT.infer_type' elst thnt)
        (If(cond, BuiltinASTs.cast elst thn, els), elst)
      with _ ->
        ignore (DT.infer_type' thnt elst)
        (If(cond, thn, BuiltinASTs.cast thnt els), thnt)
  |While(cond, body) ->
    let (cond, condt) = infer_types symtbl cond
    let (body, _) = infer_types symtbl body 
    ignore (DT.infer_type Byte condt)
    let cond = if condt = Byte then cond else BuiltinASTs.cast Byte cond
    (While(cond, body), Void)
  |Function(ret_type, args, body) ->
    let arg_types = List.map snd args
    let symtbl =
      List.fold (fun (acc_tbl: SymbolTable) (name, dt) ->
        acc_tbl.register_local_var name dt
       ) {symtbl with return_type = Some ret_type} args
    let (body, _) = infer_types symtbl body
    (Function(ret_type, args, body), DT.Function(arg_types, ret_type))
  |GlobalParse xprs ->
    let ((Block xprs', Void) | Strict xprs') = infer_types symtbl (Block xprs)
    (GlobalParse xprs', Void)

let resolve_types ast : AST =
  fst <| infer_types (empty_symbol_table()) (resolve_type_aliases ast)