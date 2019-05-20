module CompilerDatatypes.Token
open CompilerDatatypes.DT

type Value = Var of name: string * DT | Lit of quote: string * DT
  with
    override x.ToString() =
      match x with
      |Lit(s, _) -> s
      |Var(s, t) -> sprintf "%s: %A" s t

type SourceFile = Builtin of string | Local of string
type Token = {
  value : Value
  line : int
  col : int
  file : SourceFile
 }

let deftkn = { value = Var("", DT.Void); line = 0; col = 0; file = Builtin "none" }