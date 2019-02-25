module Parser.AST

open System

type Datatype =
  |Int | Char | Long | Void | Float | Double
  |Pointer of Datatype * deref_size_bytes: int Option
  |Function of Datatype list * Datatype
  |Unknown of possible_types: Datatype list  // empty list <=> could be any type
  |Struct of name: string * properties: (string * int) list
   with
    // memory size in bytes
    member x.sizeof =
      match x with
      |Unknown types -> failwithf "can't get size of ambiguous type %A" types
      |Int -> 4
      |Char -> 1
      |Long -> 8  // depends
      |Void -> 0
      |Float -> 4
      |Double -> 8  // depends
      |Pointer _ -> 4
      |Function _ -> 4
      |Struct(_, props) -> List.sumBy snd props
    
    // if x could describe y
    member x.is_superset_of y =
      match x, y with
      |Unknown [], _ -> true
      |Unknown xtypes, Unknown ytypes ->
        List.forall (fun yt -> List.exists (fun (xt: Datatype) -> xt.is_superset_of yt) xtypes) ytypes
      |Unknown xtypes, y -> List.exists (fun (xt: Datatype) -> xt.is_superset_of y) xtypes
      |x, Unknown ytypes -> List.forall x.is_superset_of ytypes
      |Function(args1, ret1), Function(args2, ret2) ->
        List.length args1 = List.length args2 && List.forall2 (fun (at: Datatype) -> at.is_superset_of) args1 args2
         && ret1.is_superset_of ret2
      |Pointer(t1, s1), Pointer(t2, s2) ->
        match s1, s2 with
        |Some a, Some b when a <> b -> false
        |_ -> t1.is_superset_of t2
      |_ -> x = y

module TypeClasses =
  let real = [Char; Int; Long; Float; Double]
  let integral = [Char; Int; Long]
  // temp
  let ptr_compatible =
    let basics = [Void; Char; Int; Long; Float; Double]
    basics @ List.map (fun e -> Pointer (e, None)) basics

let t_any = Unknown []
let t_numeric = Unknown TypeClasses.real
let tf_unary typeclass1 typeclass2 =
  match Set.toList (Set.ofList [for t1 in typeclass1 do for t2 in typeclass2 -> Datatype.Function([t1], t2)]) with
  |[] -> failwith "type is nothing"
  |[x] -> x
  |types -> Unknown types
let tf_arith_prefix = Unknown [for t in TypeClasses.real -> Datatype.Function([t], t)]
let tf_logic_prefix = Unknown [for t in TypeClasses.real -> Datatype.Function([t], Char)]
let tf_arith_infix = Unknown [for t in TypeClasses.real -> Datatype.Function([t; t], t)]
let tf_logic_infix = Unknown [for t in TypeClasses.real -> Datatype.Function([t; t], Char)]
      

type Value =
  |Unit
  |Lit of string * Datatype
  |Var of string * Datatype
  with
    override x.ToString() =
      match x with
      |Unit -> "()"
      |Lit(s, _) -> s
      |Var(s, t) -> sprintf "%s: %A" s t

type AST =
  |Apply of AST * AST list  // f(a, b, ...)
  |Assign of AST * AST  // a = b
  |Index of AST * AST  // a[b]
  |Declare of string * Datatype  // [int|...] a;
  |DeclareHelper of AST list  // like block, but no inner scope (declares are moved to the outside)
  |Return of AST  // return a
  |Block of AST list  // {x; y; ...}
  |If of AST * AST * AST  // if (a) b; else c
  |While of AST * AST  // while (a) b
  |Function of (string * Datatype) list * AST  // [int] f(a, b, ...) x     global
  |Value of Value  // a
  |GlobalParse of AST list  // a; b; ...     global