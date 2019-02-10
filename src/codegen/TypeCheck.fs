module Codegen.TypeCheck
open Parser.AST
open Codegen.Tables

// a, b if a automatically casts to b (casts can use multiple edges)
let edges = [
  Int, Long
  Char, Long
  Char, Int
  Float, Double
  Int, Float
  Long, Double
 ]
let cast_graph =
  List.fold (fun acc_map (a, b) ->
    if Map.containsKey a acc_map then Map.add a (b::acc_map.[a]) acc_map else Map.add a [b] acc_map
   ) Map.empty edges
let check_cast_one_way t1 t2 =  // can t1 be casted to t2?
  let visited = System.Collections.Generic.Dictionary()
  let rec find_cast_path dest n =
    if not (visited.ContainsKey n) then
      visited.[n] <-
        if dest = n then true
        elif Map.containsKey n cast_graph then List.exists (find_cast_path dest) cast_graph.[n]
        else false
    visited.[n]
  match t1, t2 with
  |_ when find_cast_path t2 t1 -> true
  |Pointer _, Pointer _ -> true
  |Datatype.Function(args1, t), Datatype.Function(args2, rtype) when args1 = args2 && t.is_superset_of rtype -> true
  |_ -> false
let assert_cast_one_way t1 t2 = if not (check_cast_one_way t1 t2) then failwithf "cannot cast %A to %A" t1 t2
let assert_cast t1 t2 =
  try assert_cast_one_way t1 t2
  with ex ->
    try assert_cast_one_way t2 t1
    with _ -> raise ex

let rec cast dtype ast = Apply(Value(Var("\cast", dtype)), [ast])

let rec check_type (symtbl: SymbolTable) = function
  |Value(Var(name, typ)) ->
    ignore <| symtbl.check_var name typ
    let t = symtbl.var_to_type.[name]
    Value(Var(name, t)), t
  |Value(Lit(_, t)) as ast -> ast, t
  |Value Unit as ast -> ast, Void
  |Declare _ -> failwith "should never be reached"
  |Apply(f, args) ->
    match check_type symtbl f with
    |Value(Var(fname, _)), Unknown ts when List.forall (function Datatype.Function(_, _) -> true | _ -> false) ts ->
      let args', discovered_arg_types = List.unzip (List.map (check_type symtbl) args)
      match
        List.choose (function
          |Datatype.Function(arg_types, ret) as ftype when List.forall2 check_cast_one_way discovered_arg_types arg_types ->
            let args'', casted =
              List.map3 (fun arg expected_t discovered_t ->
                match expected_t, discovered_t with
                |_ when expected_t = discovered_t -> arg, false
                |Pointer(t', None), Pointer(t'', _) when t' = t'' -> arg, false
                |_ -> cast expected_t arg, true
               ) args' arg_types discovered_arg_types
               |> List.unzip
            Some (Apply(Value(Var(fname, ftype)), args''), ret, List.exists id casted)
          |_ -> None
         ) ts
        with
      |(hda, hdb, _)::_ as ll ->
        match List.tryFind (function _, _, any_casted -> not any_casted) ll with
        |Some(a, b, _) -> a, b
        |None -> hda, hdb
      |[] -> failwithf "none of the possible function types for %A matched %A" fname discovered_arg_types
    |f', Datatype.Function(arg_types, ret) ->
      let args', discovered_arg_types = List.unzip (List.map (check_type symtbl) args)
      let args'' =
        List.map3 (fun arg expected_type discovered_type ->
          if expected_type = discovered_type then arg
          else
            assert_cast expected_type discovered_type
            cast expected_type arg
         ) args' arg_types discovered_arg_types
      Apply(f', args''), ret
    |_ -> failwithf "%A is not a function" f
  |Assign(l, r) ->
    match check_type symtbl l with
    |l', (Datatype.Function(_, ret_type) as tl) ->
      let r', tr = check_type {symtbl with return_type = Some ret_type} r
      assert_cast tr tl
      Assign(l', r'), tl
    |l', tl ->
      let r', tr = check_type symtbl r
      assert_cast tr tl
      let r'' = if tr = tl then r' else cast tl r'
      Assign(l', r''), tl
  |Index(a, i) ->
    let i', ti = check_type symtbl i
    assert_cast ti Int
    let i'' = if ti = Int then i' else cast Int i'
    match check_type symtbl a with
    |a', Pointer(t, _) -> Index(a', i''), t
    |_ -> failwith "not indexable"
  |DeclareHelper _ -> failwith "should never be reached, handled in block"
  |Return ast ->
    let ast', t = check_type symtbl ast
    match symtbl.return_type with
    |Some rt when t <> rt -> Return (cast rt ast'), rt
    |_ -> Return ast', t
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
    Block xprs', Void
  |If(cond, thn, els) ->
    let cond', condt = check_type symtbl cond
    assert_cast condt Char
    let cond'' = if condt = Char then cond' else cast Char cond'
    let (thn', thnt), (els', elst) = check_type symtbl thn, check_type symtbl els
    if thnt = elst
     then If(cond'', thn', els'), thnt
     else
      try
        assert_cast_one_way thnt elst
        If(cond'', cast elst thn', els'), elst
      with _ ->
        assert_cast_one_way elst thnt
        If(cond'', thn', cast thnt els'), thnt
  |While(cond, body) ->
    let (cond', condt), (body', _) = check_type symtbl cond, check_type symtbl body
    assert_cast condt Char
    let cond'' = if condt = Char then cond' else cast Char cond'
    While(cond'', body'), Void
  |Function(args, body) ->
    let arg_types = List.map snd args
    let body', _ = check_type (List.fold (fun tbl (n, t) -> tbl.register_var n t) symtbl args) body
    Function(args, body'), Datatype.Function(arg_types, Unknown [])
  |GlobalParse xprs ->
    match check_type symtbl (Block xprs) with
    |Block xprs', Void -> GlobalParse xprs', Void
    |_ -> failwith "should never be reached"
    
