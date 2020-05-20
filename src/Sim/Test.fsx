#if !TEST
#load "../.fake/build.fsx/intellisense.fsx"
#endif
#r "../build/Utils.dll"
#r "../build/CompilerDatatypes.dll"
#r "../build/Parser.dll"
#r "../build/Transformers.dll"
#r "../build/Codegen.dll"
#r "../build/Sim.dll"

open Fuchu
open CompilerDatatypes
open CompilerDatatypes.Semantics.InterpreterValue
open Parser.Main

let parse_string_to_pasm =
  parse_string_to_ast
   >> Transformers.Main.transform
   >> Codegen.Main.generate_pasm
   >> PseudoASM.Simple.convert_from_flat

let test_interpreter() =
  let mutable input = []
  let mutable expected = Boxed.Void
  let mutable expected_stdout = ""

  let test output_checker message =
    testCase message (fun () ->
      let result = Sim.PAsmMachine.eval input
      output_checker message result
     )
     |> run
     |> ignore

  let check_val msg { Sim.PAsmMachine.State.regs = regs } =
    let return_value = regs.[PseudoASM.Register.R 0]
    Assert.Equal(msg, return_value, expected)

  let check_stdout msg { Sim.PAsmMachine.State.stdout = stdout } =
    Assert.Equal(msg, stdout, expected_stdout)

  let (test, test_stdout) = (test check_val, test check_stdout)
  
  input <- parse_string_to_pasm """
    int main() {
      long a = 1L;
      return a;
     }"""
  expected <- Int 1
  test "simple declare & assign"

  input <- parse_string_to_pasm """
    int main() {
      return 2 + 5 * 7;
     }"""
  expected <- Int 37
  test "arithmetic"

  input <- parse_string_to_pasm """
    int f(int x) {
      return x + x * x;
     }
    int main() {
      return f(5);
     }"""
  expected <- Int 30
  test "function call"

  input <- parse_string_to_pasm """
    int main() {
      int e = 0;
      while (1) {
        e = e + 1;
        if (e == 20) return e;
       }
     }"""
  expected <- Int 20
  test "accumulator"

  input <- parse_string_to_pasm """
    int main() {
      printf("Hello, World! \n");
      return 0;
     }"""
  expected_stdout <- "Hello, World! \n"
  test_stdout "hello world"

test_interpreter()