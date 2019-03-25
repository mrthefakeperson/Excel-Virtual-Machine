module TestCodegen.TestInterpreter
open Fuchu
open Parser.Datatype
open Parser.AST
open Parser.Main
open Codegen.PAsm
open Codegen.Interpreter

let test_xpr message ast expected_output =
  testCase message <| fun () ->
        Assert.Equal(message, expected_output, preprocess_eval_ast ast)
   |> run
   |> ignore

let test_declare() =
  let expected = Int64 0L, ""
  test_xpr "simple declare" (parse_string_to_ast_with code_body "{ long a; return a; }") expected
  
  let expected = Int64 1L, ""
  test_xpr "simple declare & assign" (parse_string_to_ast_with code_body "{ long a = 1L; return a; }") expected

  let expected = Int 37, ""
  let xpr = parse_string_to_ast_with value "2 + 5 * 7"
  test_xpr "builtins 1" xpr expected

  let expected = Int 30, ""
  let x = Value(Var("x", DT.Int))
  let body_xpr = Apply(Value(Var("+", TypeClasses.any)), [x; Apply(Value(Var("*", TypeClasses.any)), [x; x])])
  let func_type = DT.Function([DT.Int], DT.Int)
  let decl_func_xpr = Declare("f", func_type)
  let assign_func_xpr = Assign(Value(Var("f", func_type)), Function(["x", DT.Int], Return body_xpr))
  let xpr = Block [decl_func_xpr; assign_func_xpr; Return(Apply(Value(Var("f", TypeClasses.any)), [Value(Lit("5", DT.Int))]))]
  test_xpr "function call 1" xpr expected

  let expected = Int 20, ""
  let xpr = parse_string_to_ast_with code_body "{ int e; while ('a') { e = e + 1; if (e == 20) return e; } }"
  test_xpr "accumulator" xpr expected

  let xpr =
    GlobalParse [
      DeclareHelper [
        Declare("main", DT.Function2 DT.Int)
        Assign(Value (Var("main", DT.Function2 DT.Int)),
          Function([],
            Block [
              Apply(Value (Var("printf", TypeClasses.any)),
                [Value (Lit("\"Hello, World! \n\"", DT.Ptr DT.Byte))]
               )
              Return (Value(Lit("0", DT.Int)))
             ]
           )
         )
       ]
     ]
  test_xpr "hello world" xpr (Int 0, "Hello, World! \n (fmt [])")