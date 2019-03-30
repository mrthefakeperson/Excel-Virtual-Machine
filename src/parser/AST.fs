module Parser.AST
open Parser.Datatype
open System

type Value =
  |Lit of string * DT
  |Var of string * DT
  with
    override x.ToString() =
      match x with
      |Lit(s, _) -> s
      |Var(s, t) -> sprintf "%s: %A" s t

type AST =
  |Apply of AST * AST list  // f(a, b, ...)
  |Assign of AST * AST  // a = b
  |Index of AST * AST  // a[b]
  |Declare of string * DT  // [int|...] a;
  |DeclareHelper of AST list  // like block, but no inner scope (declares are moved to the outside)
  |Return of AST  // return a
  |Block of AST list  // {x; y; ...}
  |If of AST * AST * AST  // if (a) b; else c
  |While of AST * AST  // while (a) b
  |Function of (string * DT) list * AST  // [int] f(a, b, ...) x     global
  |Value of Value  // a
  |GlobalParse of AST list  // a; b; ...     global
  
module Value =
  let unit = Value (Lit("", DT.Void))
  let (|Unit|_|) = function Value (Lit("", DT.Void)) -> Some Unit | _ -> None

type Apply' =
  static member fn(name, ?dt) = fun x -> Apply(Value(Var(name, Option.defaultValue TypeClasses.any dt)), [x])
  static member fn2(name, ?dt) = fun x y -> Apply(Value(Var(name, Option.defaultValue TypeClasses.any dt)), [x; y])

module BuiltinASTs =
  let stack_alloc dt x = Apply'.fn("\stack_alloc", DT.Function([Int], Ptr dt)) x
  let alloc dt x = Apply'.fn("\alloc", DT.Function([Int], Ptr dt)) x
  let cast dt x = Apply'.fn("\cast", dt) x

module Hooks =
  // module for transforming hooks

  // active pattern mapping:
  // takes (|P|): AST -> 'state such that, when P x is matched, x is the state returned by recursive application of the hook (Q x is for lists)
  type 'state ASTHook = (AST -> 'state) -> (AST list -> 'state list) -> AST -> 'state
  // eg. apply_hook (fun (|P|) (|Q|) -> function Return (P x) -> x | Block (Q xs) -> List.sum xs | _ -> 1)
  //   (the hook in this case produces an int state out of an AST when applied)
  let rec apply_hook (hook: 'state ASTHook) (ast: AST) : 'state =
    hook (apply_hook hook) (List.map (apply_hook hook)) ast

  // extension of above - function which optionally returns AST ('state = AST Option) and uses default behavior for None
  type MappingASTHook = (AST -> AST) -> (AST list -> AST list) -> AST -> AST Option
  let rec apply_mapping_hook (hook: MappingASTHook) : AST -> AST = fun ast ->
    match hook (apply_mapping_hook hook) (List.map (apply_mapping_hook hook)) ast with
    |Some x -> x
    |None ->
      match ast with
      |Value _ | Declare _ -> ast
      |Apply(f, args) -> Apply(apply_mapping_hook hook f, List.map (apply_mapping_hook hook) args)
      |Assign(l, r) -> Assign(apply_mapping_hook hook l, apply_mapping_hook hook r)
      |Index(a, i) -> Index(apply_mapping_hook hook a, apply_mapping_hook hook i)
      |DeclareHelper decls -> DeclareHelper(List.map (apply_mapping_hook hook) decls)
      |Return x -> Return(apply_mapping_hook hook x)
      |Block xprs -> Block(List.map (apply_mapping_hook hook) xprs)
      |If(cond, thn, els) -> If(apply_mapping_hook hook cond, apply_mapping_hook hook thn, apply_mapping_hook hook els)
      |While(cond, body) -> While(apply_mapping_hook hook cond, apply_mapping_hook hook body)
      |Function(args, body) -> Function(args, apply_mapping_hook hook body)
      |GlobalParse xprs -> GlobalParse(List.map (apply_mapping_hook hook) xprs)