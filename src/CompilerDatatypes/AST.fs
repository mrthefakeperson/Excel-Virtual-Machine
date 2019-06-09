module CompilerDatatypes.AST
open CompilerDatatypes.DT
open CompilerDatatypes.Token

type AST =
  |Apply of AST * AST list  // f(a, b, ...)
  |Assign of AST * AST  // a = b
  |Declare of string * DT  // [int|...] a;
  |Return of AST  // return a
  |Block of AST list  // {x; y; ...}
  |If of AST * AST * AST  // if (a) b; else c
  |While of AST * AST  // while (a) b
  |Function of ret: DT * args: (string * DT) list * AST  // [int] f(a, b, ...) x     global
  |V of Value  // a
  |GlobalParse of AST list  // a; b; ...     global

module Value =
  let unit = V (Lit("", DT.Void))
  let (|Unit|_|) = function V (Lit("", DT.Void)) -> Some Unit | _ -> None
 
type Apply' =
  static member fn(name, ?dt) = fun x ->
    Apply(V(Var(name, Option.defaultValue TypeClasses.any dt)), [x])
  static member fn2(name, ?dt) = fun x y ->
    Apply(V(Var(name, Option.defaultValue TypeClasses.any dt)), [x; y])

module BuiltinASTs =
  let stack_alloc dt xs =
    let dim = List.length xs
    let fnsig = DT.Function(List.replicate dim Int, TypeDef (Array(dim, dt)))
    Apply(V (Var("\stack_alloc", fnsig)), xs)
  let alloc dt x = Apply'.fn("\alloc", DT.Function([Int], Ptr dt)) x
  let cast dt x = Apply'.fn("\cast", dt) x
  let index a i =
    Apply'.fn("*prefix", TypeClasses.f_unary TypeClasses.ptr TypeClasses.any)
     (Apply'.fn2("+", TypeClasses.f_arith_infix) a i)

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
      |V _ | Declare _ -> ast
      |Apply(f, args) -> Apply(apply_mapping_hook hook f, List.map (apply_mapping_hook hook) args)
      |Assign(l, r) -> Assign(apply_mapping_hook hook l, apply_mapping_hook hook r)
    //   |Index(a, i) -> Index(apply_mapping_hook hook a, apply_mapping_hook hook i)
    //   |DeclareHelper decls -> DeclareHelper(List.map (apply_mapping_hook hook) decls)
      |Return x -> Return(apply_mapping_hook hook x)
      |Block xprs -> Block(List.map (apply_mapping_hook hook) xprs)
      |If(cond, thn, els) -> If(apply_mapping_hook hook cond, apply_mapping_hook hook thn, apply_mapping_hook hook els)
      |While(cond, body) -> While(apply_mapping_hook hook cond, apply_mapping_hook hook body)
      |Function(ret, args, body) -> Function(ret, args, apply_mapping_hook hook body)
      |GlobalParse xprs -> GlobalParse(List.map (apply_mapping_hook hook) xprs)

  let private indent (s: string) = String.concat "\n" (Seq.map ((+) "  ") (s.Split('\n')))
  let unparse_hook : string ASTHook = fun (|P|) (|Q|) ->
    let (|PBlock|) = function Block xs -> (|P|) (Block xs) | x -> (|P|) (Block [x])
    function
    |V value -> value.ToString()
    |Apply(P f, Q args) -> sprintf "%s(%s)" f (String.concat ", " args)
    |Assign(P l, P r) -> sprintf "%s = %s" l r
    |Declare(s, dt) -> sprintf "%A %s" dt s
    |Return (P x) -> sprintf "return %s" x
    |Block (Q xprs) -> sprintf "{\n%s;\n}" (indent (String.concat ";\n" xprs))
    |If(P cond, PBlock thn, PBlock els) -> sprintf "if (%s) %s\nelse %s" cond thn els
    |While(P cond, PBlock body) -> sprintf "while (%s) %s" cond body
    |Function(ret, args, PBlock body) ->
      let (Q args') = List.map Declare args
      sprintf "%A fn(%s) %s" ret (String.concat ", " args') body
    |GlobalParse (Q xprs) -> String.concat "\n" xprs

let unparse : AST -> string = Hooks.apply_hook Hooks.unparse_hook