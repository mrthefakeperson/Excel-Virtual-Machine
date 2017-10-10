#r "lex.dll"
#load "framework.fs"
open Framework

let text = """
(L x . (L x . x) x) x x
"""

let tokens = Lexer.tokenize text |> List.ofSeq

let var = lazy Rule().isOneOf("x", "y", "z")
let rec lambda = lazy Rule("lambda").isSequenceOf(lazy Rule().is "L", var, lazy Rule().is ".", value)
and brackets = lazy Rule("brackets").isSequenceOf(lazy Rule().is "(", value, lazy Rule().is ")")
and apply = lazy Rule("apply").isManyOf(lazy Rule().isOneOf(var, brackets))
and value = lazy Rule().isOneOf(lambda, apply, brackets, var)

value.Force() tokens