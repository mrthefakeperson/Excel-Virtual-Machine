module Lexer.Token

type Token = {
  value: string
  line: int
  column: int
 }
  with
    override x.ToString() =
      sprintf "%s\t\t\t: %i, %i" x.value x.line x.column

let (|S|) (t: Token) = S t.value
