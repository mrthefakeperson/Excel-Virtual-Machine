module Parser.AST

type Datatype =
  |Int | Char | Long | Void | Float | Double
  |Pointer of Datatype
  |Function of Datatype list * Datatype
  |Unknown of possible_types: Datatype list  // empty list <=> could be any type

type Value =
  |Unit
  |Literal of string * Datatype
  |Variable of string * Datatype
  with
    override x.ToString() =
      match x with
      |Unit -> "()"
      |Literal(s, _) -> s
      |Variable(s, t) -> sprintf "%s: %A" s t

type AST =
  |Apply of AST * AST list  // f(a, b, ...)
  |Assign of AST * AST // a = b
  |Index of AST * AST  // a[b]
  |Declare of AST list  // [int] a [= 1], b, ...
  |Return of AST  // return a
  |Block of AST list  // {x; y; ...}
  |If of AST * AST * AST  // if (a) b; else c
  |While of AST * AST  // while (a) b
  |Function of Value list * AST  // [int] f(a, b, ...) x     global
  |Value of Value  // a
  |GlobalParse of AST list  // a; b; ...     global
   //with
   // member x.symbolic_print() =
   //   match x with
   //   |Apply(f, args) -> sprintf "