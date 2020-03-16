module CompilerDatatypes.AST
open CompilerDatatypes.DT
open Utils

type 'v AST =
  |Apply of 'v AST * 'v AST list  // f(a, b, ...)
  |Assign of 'v AST * 'v AST  // a = b
  |Declare of string * DT  // [int|...] a;
  |Return of 'v AST  // return a
  |Block of 'v AST list  // {x; y; ...}
  |If of 'v AST * 'v AST * 'v AST  // if (a) b; else c
  |While of 'v AST * 'v AST  // while (a) b
  |Function of ret: DT * args: (string * DT) list * 'v AST  // [int] f(a, b, ...) x     global
  |V of 'v  // a
  |GlobalParse of 'v AST list  // a; b; ...     global

module Hooks =
  // submodule for traversing ASTs with minimal boilerplate
  // useful for simplifying / optimizing substructures

  // active pattern mapping:
  // takes (|P|): AST -> 'state such that, when P x is matched, x is the state returned by recursive application of the hook (Q x is for lists)
  type ASTHook<'v, 'state> = ('v AST -> 'state) -> ('v AST list -> 'state list) -> 'v AST -> 'state
  // eg. apply_hook (fun (|P|) (|Q|) -> function Return (P x) -> x | Block (Q xs) -> List.sum xs | _ -> 1)
  //   (the hook in this case produces an int state out of an AST when applied)
  let rec apply_hook (hook: ASTHook<'v, 'state>) (ast: 'v AST) : 'state =
    hook (apply_hook hook) (List.map (apply_hook hook)) ast

  // extension of above - function which optionally returns AST ('state = AST Option) and uses default behavior for None
  type 'v MappingASTHook = ('v AST -> 'v AST) -> ('v AST list -> 'v AST list) -> 'v AST -> 'v AST Option
  let rec apply_mapping_hook (hook: 'v MappingASTHook) : 'v AST -> 'v AST = fun ast ->
    let p = apply_mapping_hook hook  // recurse
    let q = List.map (apply_mapping_hook hook)  // recurse list
    match hook p q ast with
    |Some x -> x
    |None ->
      match ast with
      |V _ | Declare _ -> ast
      |Apply(f, args) -> Apply(p f, q args)
      |Assign(l, r) -> Assign(p l, p r)
      |Return x -> Return (p x)
      |Block xprs -> Block (q xprs)
      |If(cond, thn, els) -> If(p cond, p thn, p els)
      |While(cond, body) -> While(p cond, p body)
      |Function(ret, args, body) -> Function(ret, args, p body)
      |GlobalParse xprs -> GlobalParse (q xprs)

open Hooks

let private indent (s: string) =
  String.concat "\n" (Seq.map ((+) "  ") (s.Split('\n')))

let pprint_ast_structure : 'v AST -> string =
  let pprint_ast_structure_hook : ASTHook<'v, string> = fun (|P|) (|Q|) ->
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
  fun ast -> apply_hook pprint_ast_structure_hook ast

let pprint_c_program : 'v AST -> string =
  let pprint_c_program_hook : ASTHook<'v, string> = fun (|P|) (|Q|) ->
    let (|PBlock|) = function Block xs -> (|P|) (Block xs) | x -> (|P|) (Block [x])
    function
    |V value -> value.ToString()
    |Apply(P f as f_ast, Q args) ->
      match f, args with
      |("." | "->") as accessor, [arg1; arg2] -> arg1 + accessor + arg2
      |Regex "\\+|\\-|\\*|\\/|%|\\|\\||\\&\\&|==|>=|<=|<|>|!=" _ as infix, [arg1; arg2] ->
        sprintf "(%s) %s (%s)" arg1 infix arg2
      |Regex "\\+\\+|\\-\\-|!|.prefix" _ as prefix, [arg1] ->
        sprintf "%s(%s)" (prefix.Replace("prefix", "")) arg1
      |Regex "..suffix" _ as suffix, [arg1] ->
        sprintf "(%s)%s" arg1 (suffix.Replace("suffix", ""))
      |_ ->
        match f_ast with
        |V _ -> sprintf "%s(%s)" f (String.concat "," args)
        |_ -> sprintf "(%s)(%s)" f (String.concat "," args)
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
  fun ast -> apply_hook pprint_c_program_hook ast

module SyntaxAST =
  type AST = Token.Value AST
  type 'state ASTHook = Hooks.ASTHook<Token.Value, 'state>
  type MappingASTHook = Token.Value Hooks.MappingASTHook

  module Value =
    let unit = V (Token.Lit("", DT.Void))
    let (|Unit|_|) = function V (Token.Lit("", DT.Void)) -> Some Unit | _ -> None
   
  [<AutoOpen>]
  module Apply' =
    type Apply' =
      static member fn(name, ?dt) = fun x ->
        Apply(V(Token.Var(name, Option.defaultValue TypeClasses.any dt)), [x])
      static member fn2(name, ?dt) = fun x y ->
        Apply(V(Token.Var(name, Option.defaultValue TypeClasses.any dt)), [x; y])

  module BuiltinASTs =
    open CompilerDatatypes.Token

    let stack_alloc dt xs =
      let dim = List.length xs
      let fnsig = DT.Function(List.replicate dim Int, TypeDef (Array(dim, dt)))
      Apply(V (Var("\stack_alloc", fnsig)), xs)
    let (|StackAlloc|_|) = function
      |Apply(V (Var("\stack_alloc", DT.Function(_, dt))), dims) -> Some(dt, dims)
      |_ -> None

    let alloc dt x = Apply'.fn("\alloc", DT.Function([Int], Ptr dt)) x
    let cast dt x = Apply'.fn("\cast", dt) x

    let index a i =
      Apply'.fn("*prefix", TypeClasses.f_unary TypeClasses.ptr TypeClasses.any)
       (Apply'.fn2("+", TypeClasses.f_arith_infix) a i)
    let (|Index|) = function
      |Apply(V (Var("*prefix", dt)), [Apply(V (Var("+", dt2)), [a; i])])
        when
          dt = TypeClasses.f_unary TypeClasses.ptr TypeClasses.any
           && dt2 = TypeClasses.f_arith_infix ->
        Some(a, i)
      |_ -> None

    let get_static x init = Apply'.fn2 "\get_static" x (Block init)
    let (|GetStatic|_|) = function
      |Apply(V (Var("\get_static", _)), [x; Block init]) -> Some(x, init)
      |_ -> None

module SemanticAST =
  open Semantics.RegisterAlloc
  open Semantics.InterpreterValue

  type Value = Var of VarName * VarInfo | Lit of Boxed
    with
      override x.ToString() =
        match x with
        |Var(name, (Local(_, dt) | Global dt)) -> Token.Value.Var(name, dt).ToString()
        |Lit value -> value.ToString()
  type AST = Value AST
  type 'state ASTHook = Hooks.ASTHook<Value, 'state>
  type MappingASTHook = Value Hooks.MappingASTHook

  module BuiltinASTs =
    let cast dt x = Apply(V (Var("\cast", Global dt)), [x])
    let (|StackAlloc|_|) = function
      |Apply(V (Var("\stack_alloc", Global (DT.Function(_, dt)))), dims) -> Some(dt, dims)
      |_ -> None

  let rec from_syntax_ast: SyntaxAST.AST -> AST =
    Hooks.apply_hook (fun p q -> function
      |V (Token.Value.Lit(s, dt)) -> V (Lit (Option.get (Boxed.from_t dt s)))
      |V (Token.Value.Var(name, dt)) -> V (Var(name, Global dt))
      |Declare(s, dt) -> Declare(s, dt)
      |Apply(f, args) -> Apply(p f, q args)
      |Assign(l, r) -> Assign(p l, p r)
      |Return x -> Return (p x)
      |Block xprs -> Block (q xprs)
      |If(cond, thn, els) -> If(p cond, p thn, p els)
      |While(cond, body) -> While(p cond, p body)
      |Function(ret, args, body) -> Function(ret, args, p body)
      |GlobalParse xprs -> GlobalParse (q xprs)
     )