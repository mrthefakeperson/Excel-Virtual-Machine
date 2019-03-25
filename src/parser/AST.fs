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