module Sim.Main
open Utils
open CompilerDatatypes.AST
open CompilerDatatypes.Semantics.InterpreterValue
open CompilerDatatypes.PseudoASM

let interpret_semantics_ast: SemanticAST.AST -> Boxed * string =
  Interpreter.InterpretAST.interpret_ast

let interpret_syntax_ast: SyntaxAST.AST -> Boxed * string =
  Transformers.Main.transform >> Interpreter.InterpretAST.interpret_ast

let interpret_string: string -> Boxed * string =
  Transformers.Main.transform_from_string >> Interpreter.InterpretAST.interpret_ast

let simulate_pasm: Flat.Asm list -> Boxed * string =
  Simple.convert_from_flat >> PAsmMachine.eval >> fun e -> (e.regs.[R 0], e.stdout)

[<EntryPoint>]
let main argv =
  let flags = CLI.get_cli_flags argv
  let inline interact repl = CLI.interact "Sim Tools REPL" repl
  match () with
  |_ when flags.ContainsKey "-ast" -> interact interpret_string
  |_ when flags.ContainsKey "-pasm" -> failwith "not implemented yet"
  |_ -> failwith "run with -ast or -pasm"
  0