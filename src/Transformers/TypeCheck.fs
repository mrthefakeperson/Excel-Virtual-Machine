module Transformers.TypeCheck
open System.Collections.Generic
open Utils
open CompilerDatatypes.Token
open CompilerDatatypes.Semantics.RegisterAlloc
open CompilerDatatypes.Semantics.InterpreterValue
open CompilerDatatypes.DT
open CompilerDatatypes.AST
open CompilerDatatypes.AST.Hooks
open CompilerDatatypes.SymbolTable

let private resolve_type_aliases ast : SyntaxAST.AST =
  let aliases = Dictionary()
  let resolve = function
    |TypeDef (Alias _ | Struct _ | Union _ as td) -> aliases.[td]
    |t -> t
  let hook: SyntaxAST.MappingASTHook = fun (|P|) (|Q|) -> function
    |Declare(vname, TypeDef (DeclAlias(tname, dt))) ->
      aliases.[Alias tname] <- resolve dt
      Some (Declare(vname, aliases.[Alias tname]))
    |Declare(vname, (TypeDef (DeclStruct(tname, _)) as dt)) ->
      aliases.[Struct tname] <- dt
      Some (Declare(vname, dt))
    |Declare(vname, (TypeDef (DeclUnion(tname, _)) as dt)) ->
      aliases.[Union tname] <- dt
      Some (Declare(vname, dt))
    |Declare(vname, dt) -> Some (Declare(vname, resolve dt))
    |V (Var(vname, dt)) -> Some (V (Var(vname, resolve dt)))
    |Function(ret_t, args, P body) ->
      let resolved_args = List.map (fun (arg, arg_t) -> (arg, resolve arg_t)) args
      Some (Function(resolve ret_t, resolved_args, body))
    |_ -> None
  apply_mapping_hook hook ast

let private cast_if_necessary expected_type expr_type expr =
  if expected_type = expr_type
   then expr
   else SemanticAST.BuiltinASTs.cast expected_type expr  // TODO: check if cast is valid

let rec private infer_types (symtbl: SymbolTable) : SyntaxAST.AST -> SemanticAST.AST * DT = function
  |V (Var(vname, dt)) ->
    let var_info = symtbl.get_var vname
    let Local(_, dt) | Global dt = var_info
    // ignore (DT.infer_type dt' dt)  // builtin functions cause crashes
    (V (SemanticAST.Var(vname, var_info)), dt)
  |V (Lit(lit_expr, dt) as lit) ->
    match Boxed.from_string dt lit_expr with
    |Some x -> (V (SemanticAST.Lit x), dt)
    |None -> failwithf "no semantic translation for %A" lit
  |Declare _ -> failwith "should never be reached"
  |Apply(V (Var(".", _)), [xpr; (V (Var(field, _)) | Strict field)]) ->
    let (_, txpr) = infer_types symtbl xpr
    match txpr with
    |TypeDef (DeclStruct(_, struct_fields) | DeclUnion(_, struct_fields)) ->
      List.tryPick (function 
        |StructField(fld_name, fld_dt, start_bit, sz_bytes) when field = fld_name ->
          Some(fld_dt, start_bit, sz_bytes)
        |_ -> None
       ) struct_fields
       |> Option.defaultWith (fun () -> failwithf "type %A has no property %s" txpr field)
       |> fun (fld_dt, start_bit, _) ->
            let byte_index = V (Lit(string (start_bit / 8), Ptr Byte))  // TODO: get proper bit alignment
            let (access, _) = infer_types symtbl (SyntaxAST.BuiltinASTs.index xpr byte_index)
            (cast_if_necessary fld_dt Byte access, fld_dt)
    |_ -> failwithf "type %A has no property %s" txpr field
  |Apply(f, args) ->
    let (args, inferred_given_arg_types) = List.unzip (List.map (infer_types symtbl) args)
    match infer_types symtbl f with
    |V (SemanticAST.Var(fname, finfo)), (DT.Function(_, ret_type) as ftype) ->
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
          cast_if_necessary sign_type inferred_type arg
         ) args inferred_given_arg_types sign_arg_types
      let finfo =
        match finfo with
        |Global _ -> Global ftype
        |Local(reg, _) -> Local(reg, ftype)
      (Apply(V (SemanticAST.Var(fname, finfo)), args), inferred_ret_type)
    |f, Function2 ret_type -> (Apply(f, args), ret_type)
    |_ -> failwithf "%A is not a function" f
  |Assign(l, r) ->
    let (l, tl) = infer_types symtbl l
    let (r, tr) = infer_types symtbl r
    let r =
      match (tl, tr) with
      |(Function2 _, DT.Function _) -> r
      |_ -> cast_if_necessary tl tr r
    (Assign(l, r), DT.infer_type tl tr)
  |Return ast ->
    let ast, dt = infer_types symtbl ast
    let (Some ret_dt | Strict ret_dt) = symtbl.return_type
    (Return (cast_if_necessary ret_dt dt ast), dt)
  |Block xprs ->
    let (xprs, _) =
      List.mapFold (fun (acc_tbl: SymbolTable) -> function
        |Declare(vname, dt) ->
          let with_new_var = acc_tbl.register_local_var vname dt
          (Declare(vname, dt), with_new_var)
        |xpr ->
          let (inferred_xpr, _) = infer_types acc_tbl xpr
          (inferred_xpr, acc_tbl)
       ) symtbl xprs
    (Block xprs, Void)
  |If(cond, thn, els) ->
    let (cond, condt) = infer_types symtbl cond
    ignore (DT.infer_type Byte condt)
    let cond = cast_if_necessary Byte condt cond
    let (thn, thnt) = infer_types symtbl thn
    let (els, elst) = infer_types symtbl els
    if thnt = elst
     then (If(cond, thn, els), thnt)
     else
      try
        ignore (DT.infer_type' elst thnt)
        (If(cond, SemanticAST.BuiltinASTs.cast elst thn, els), elst)
      with _ ->
        ignore (DT.infer_type' thnt elst)
        (If(cond, thn, SemanticAST.BuiltinASTs.cast thnt els), thnt)
  |While(cond, body) ->
    let (cond, condt) = infer_types symtbl cond
    let (body, _) = infer_types symtbl body 
    ignore (DT.infer_type Byte condt)
    let cond = cast_if_necessary Byte condt cond
    (While(cond, body), Void)
  |Function(ret_type, args, body) ->
    let n_args = List.length args
    let arg_types = List.map snd args
    let symtbl =
      List.fold2 (fun (acc_tbl: SymbolTable) reg (argname, dt) ->
        { acc_tbl with
            var_lookup = Map.add argname (Local(reg, dt)) acc_tbl.var_lookup }
       )
       {symtbl with return_type = Some ret_type}
       [-1 .. -1 .. -n_args]  // negative regs => negative stack offsets since args are pushed before call
       args
    let (body, _) = infer_types symtbl body
    (Function(ret_type, args, body), DT.Function(arg_types, ret_type))
  |GlobalParse xprs ->
    let (xprs, _) =
      List.mapFold (fun (acc_tbl: SymbolTable) -> function
        |Declare(vname, dt) ->
          let with_new_var = acc_tbl.register_global_var vname dt
          (Declare(vname, dt), with_new_var)
        |xpr ->
          let (inferred_xpr, _) = infer_types acc_tbl xpr
          (inferred_xpr, acc_tbl)
       ) symtbl xprs
    (GlobalParse xprs, Void)

let resolve_types (ast: SyntaxAST.AST) : SemanticAST.AST =
  resolve_type_aliases ast
   |> infer_types (empty_symbol_table())
   |> fst