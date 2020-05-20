module Interpreter.Main
open Utils
open CompilerDatatypes.AST
open CompilerDatatypes.Semantics.InterpreterValue

let interpret_semantics_ast: SemanticAST.AST -> Boxed * string =
  InterpretAST.interpret_ast

let interpret_syntax_ast: SyntaxAST.AST -> Boxed * string =
  Transformers.Main.transform >> InterpretAST.interpret_ast

let interpret_string: string -> Boxed * string =
  Transformers.Main.transform_from_string >> InterpretAST.interpret_ast

[<EntryPoint>]
let main argv =
  let flags = CLI.get_cli_flags argv
  let inline interact repl = CLI.interact "Interpreter REPL (parser/transformations)" repl
  interact interpret_string
  0