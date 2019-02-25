module TestCodegen.TestHooks
open Fuchu
open Parser.AST
open Codegen.Hooks

let test_hook message hook input_ast expected =
  testCase message <|
    fun () -> Assert.Equal(message, true, expected (apply_hook hook input_ast))
   |> run
   |> ignore

let test_mapping_hook message hook input_ast expected_ast =
  testCase message <|
    fun () -> Assert.Equal(message, expected_ast, apply_mapping_hook hook input_ast)
   |> run
   |> ignore

let run_all() =
  let xpr =
    Block [
      Apply(Value(Var("sizeof", t_any)), [Value(Var("int", t_any))])
      Value Unit
      Assign(Value(Var("a", t_any)), Apply(Value(Var("sizeof", t_any)), [Value(Var("char", t_any))]))
      Return(Apply(Value(Var("sizeof", t_any)), [Value(Var("long", t_any))]))
     ]
  let expected =
    Block [
      Value(Lit("4", Int)); Value Unit
      Assign(Value(Var("a", t_any)), Value(Lit("1", Int))); Return(Value(Lit("8", Int)))
     ]
  test_mapping_hook "mapping hook" transform_sizeof_hook xpr expected

  let xpr =
    GlobalParse [
      Declare("a", Pointer(Char, Some 3)); Assign(Value(Var("a", t_any)), Value(Lit("\"xy\"", Pointer(Char, Some 3))))
      Value(Var("x", t_any))
      Value(Lit("\"\"", Pointer(Char, Some 1)))
     ]
  let expected result = List.ofSeq result = ["$78_79", "xy"; "$", ""]
  test_hook "find strings hook" find_strings_hook xpr expected

  let expected =
    GlobalParse [
      Declare("$", Pointer(Char, Some 1))
      Assign(Index(Value(Var("$", t_any)), Value(Lit("0", Int))), Value(Lit("'\\0'", Char)))
      Declare("$78_79", Pointer(Char, Some 3))
      Assign(Index(Value(Var("$78_79", t_any)), Value(Lit("0", Int))), Value(Lit("'x'", Char)))
      Assign(Index(Value(Var("$78_79", t_any)), Value(Lit("1", Int))), Value(Lit("'y'", Char)))
      Assign(Index(Value(Var("$78_79", t_any)), Value(Lit("2", Int))), Value(Lit("'\\0'", Char)))
      Declare("a", Pointer(Char, Some 3)); Assign(Value(Var("a", t_any)), Value(Var("$78_79", t_any)))
      Value(Var("x", t_any))
      Value(Var("$", t_any))
     ]
  test_mapping_hook "extract strings hook" extract_strings_to_global_hook xpr expected

  let xpr =
    GlobalParse [
      Value(Lit("\"\"", Pointer(Char, Some 1)))
      Value(Lit("\"\"", Pointer(Char, Some 1)))
     ]
  let expected =
    GlobalParse [
      Declare("$", Pointer(Char, Some 1))
      Assign(Index(Value(Var("$", t_any)), Value(Lit("0", Int))), Value(Lit("'\\0'", Char)))
      Value(Var("$", t_any))
      Value(Var("$", t_any))
     ]
  test_mapping_hook "extract strings hook" extract_strings_to_global_hook xpr expected