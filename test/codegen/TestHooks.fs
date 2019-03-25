module TestCodegen.TestHooks
open Fuchu
open Parser.Datatype
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
      Apply(Value(Var("sizeof", TypeClasses.any)), [Value(Var("int", TypeClasses.any))])
      Value.unit
      Assign(Value(Var("a", TypeClasses.any)), Apply(Value(Var("sizeof", TypeClasses.any)), [Value(Var("char", TypeClasses.any))]))
      Return(Apply(Value(Var("sizeof", TypeClasses.any)), [Value(Var("long", TypeClasses.any))]))
     ]
  let expected =
    Block [
      Value(Lit("4", Int)); Value.unit
      Assign(Value(Var("a", TypeClasses.any)), Value(Lit("1", Int))); Return(Value(Lit("8", Int)))
     ]
  test_mapping_hook "mapping hook" transform_sizeof_hook xpr expected

  let xpr =
    GlobalParse [
      Declare("a", Ptr Byte); Assign(Value(Var("a", TypeClasses.any)), Value(Lit("\"xy\"", Ptr Byte)))
      Value(Var("x", TypeClasses.any))
      Value(Lit("\"\"", Ptr Byte))
     ]
  let expected result = List.ofSeq result = ["$78_79", "xy"; "$", ""]
  test_hook "find strings hook" find_strings_hook xpr expected

  let expected =
    GlobalParse [
      Declare("$", Ptr Byte)
      Assign(Value(Var("$", Ptr Byte)), Apply(Value(Var("\stack_alloc", DT.Function([Int], Ptr Byte))), [Value(Lit("1", Int))]))
      Assign(Index(Value(Var("$", TypeClasses.any)), Value(Lit("0", Int))), Value(Lit("'\\0'", Byte)))
      Declare("$78_79", Ptr Byte)
      Assign(Value(Var("$78_79", Ptr Byte)), Apply(Value(Var("\stack_alloc", DT.Function([Int], Ptr Byte))), [Value(Lit("3", Int))]))
      Assign(Index(Value(Var("$78_79", TypeClasses.any)), Value(Lit("0", Int))), Value(Lit("'x'", Byte)))
      Assign(Index(Value(Var("$78_79", TypeClasses.any)), Value(Lit("1", Int))), Value(Lit("'y'", Byte)))
      Assign(Index(Value(Var("$78_79", TypeClasses.any)), Value(Lit("2", Int))), Value(Lit("'\\0'", Byte)))
      Declare("a", Ptr Byte); Assign(Value(Var("a", TypeClasses.any)), Value(Var("$78_79", TypeClasses.any)))
      Value(Var("x", TypeClasses.any))
      Value(Var("$", TypeClasses.any))
     ]
  test_mapping_hook "extract strings hook" extract_strings_to_global_hook xpr expected

  let xpr =
    GlobalParse [
      Value(Lit("\"\"", Ptr Byte))
      Value(Lit("\"\"", Ptr Byte))
     ]
  let expected =
    GlobalParse [
      Declare("$", Ptr Byte)
      Assign(Value(Var("$", Ptr Byte)), Apply(Value(Var("\stack_alloc", DT.Function([Int], Ptr Byte))), [Value(Lit("1", Int))]))
      Assign(Index(Value(Var("$", TypeClasses.any)), Value(Lit("0", Int))), Value(Lit("'\\0'", Byte)))
      Value(Var("$", TypeClasses.any))
      Value(Var("$", TypeClasses.any))
     ]
  test_mapping_hook "extract strings hook 2" extract_strings_to_global_hook xpr expected