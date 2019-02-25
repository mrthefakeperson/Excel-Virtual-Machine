module TestInterpreter
open Fuchu
open Parser.AST
open Codegen.TypeCheck
open Codegen.Interpreter
open Parser.Main
open Codegen.Tables

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
  let x = Value(Var("x", Datatype.Int))
  let body_xpr = Apply(Value(Var("+", t_any)), [x; Apply(Value(Var("*", t_any)), [x; x])])
  let func_type = Datatype.Function([Datatype.Int], Datatype.Int)
  let decl_func_xpr = Declare("f", func_type)
  let assign_func_xpr = Assign(Value(Var("f", func_type)), Function(["x", Datatype.Int], Return body_xpr))
  let xpr = Block [decl_func_xpr; assign_func_xpr; Return(Apply(Value(Var("f", t_any)), [Value(Lit("5", Datatype.Int))]))]
  test_xpr "function call 1" xpr expected

  let expected = Int 20, ""
  let xpr = parse_string_to_ast_with code_body "{ int e; while ('a') { e = e + 1; if (e == 20) return e; } }"
  test_xpr "accumulator" xpr expected

  let xpr =
    GlobalParse [
      DeclareHelper
     [Declare ("main",Datatype.Function ([Unknown []],Datatype.Int));
      Assign
        (Value (Var ("main",Datatype.Function ([Unknown []],Datatype.Int))),
         Function
           ([(".", Unknown [])],
            Block
              [Apply
                 (Value (Var ("printf",Unknown [])),
                  [Value (Lit ("\"Hello, World! \n\"",Pointer (Char,Some 19)))]);
               Return (Value (Lit ("0",Datatype.Int)))]))]]
  test_xpr "hello world" xpr (Int 0, "Hello, World! \n (fmt [])")