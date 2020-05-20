#if !TEST
#load "../.fake/build.fsx/intellisense.fsx"
#endif
#r "../build/Utils.dll"
#r "../build/CompilerDatatypes.dll"
#r "../build/Parser.dll"
#r "../build/Transformers.dll"
#r "../build/Interpreter.dll"

open Fuchu
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SemanticAST
open CompilerDatatypes.Semantics.InterpreterValue
open Parser.Main

let parse_string_to_expr_sem = parse_string_to_expr >> SemanticAST.from_syntax_ast
let parse_string_to_control_sem = parse_string_to_control >> SemanticAST.from_syntax_ast
let parse_string_to_ast_sem = parse_string_to_ast >> Transformers.Main.transform

let test_interpreter() =
  let mutable input = V (Lit Boxed.Void)
  let mutable expected = Boxed.Void
  let mutable expected_stdout = ""

  let test output_checker message =
    testCase message (fun () ->
      let result = Interpreter.InterpretAST.interpret_ast input
      output_checker message result
     )
     |> run
     |> ignore
  let check_val msg (result, _) = Assert.Equal(msg, result, expected)
  let check_stdout msg (_, result) = Assert.Equal(msg, result, expected_stdout)
  let (test, test_stdout) = (test check_val, test check_stdout)

  input <- parse_string_to_control_sem "{ long a; return a; }"
  expected <- Int64 0L
  test "Interpreterple declare"
  
  input <- parse_string_to_control_sem "{ long a = 1L; return a; }"
  expected <- Int64 1L
  test "Interpreterple declare & assign"

  input <- parse_string_to_expr_sem "2 + 5 * 7"
  expected <- Int 37
  test "arithmetic"

  input <- parse_string_to_ast_sem "int f(int x) { return x + x * x; } int main() { return f(5); }"
  expected <- Int 30
  test "function call"

  input <- parse_string_to_ast_sem "int main() { int e; while ('a') { e = e + 1; if (e == 20) return e; } }"
  expected <- Int 20
  test "accumulator"

  input <- parse_string_to_ast_sem "int main() { printf(\"Hello, World! \\n\"); return 0; }"
  expected_stdout <- "Hello, World! \n (fmt [])\n"
  test_stdout "hello world"

test_interpreter()