module TestInterpreter
open Fuchu
open Parser.AST
open Codegen.Interpreter

let t_any = Unknown []

let test_xpr message ast expected_output =
  testCase message <| fun () ->
        Assert.Equal(
          message, expected_output,
          try eval_ast (default_memory()) ast
          with RaiseReturn x -> x
         )
   |> run
   |> ignore

let test_memory message ast expected_pairs =
  testCase message <| fun () ->
        let mem = default_memory()
        try ignore (eval_ast mem ast) with RaiseReturn _ -> ()
        List.iter (fun (i, v) -> Assert.Equal(message, v, mem.mem.[i])) expected_pairs
   |> run
   |> ignore

let test_declare() =
  let expected = Int64 0L
  test_xpr "simple declare" (Block [Declare("a", Long); Return(Value(Var("a", t_any)))]) expected
  
  let expected = Int64 1L
  test_xpr "simple declare & assign"
   (Block [Declare("a", Long); Assign(Value(Var("a", Long)), Value(Lit("1", Long))); Return(Value(Var("a", t_any)))]) expected

  let expected = Int 37
  let xpr = Apply(Value(Var("+", t_any)), [Value(Lit("2", Datatype.Int)); Apply(Value(Var("*", t_any)), [Value(Lit("5", Datatype.Int)); Value(Lit("7", Datatype.Int))])])
  test_xpr "builtins 1" xpr expected

  let expected = Int 30
  let x = Value(Var("x", Datatype.Int))
  let body_xpr = Apply(Value(Var("+", t_any)), [x; Apply(Value(Var("*", t_any)), [x; x])])
  let func_type = Datatype.Function([Datatype.Int], Datatype.Int)
  let decl_func_xpr = Declare("f", func_type)
  //test_memory "function call 1: declare function" decl_func_xpr [1, Ptr(100000, Datatype.Void)]
  let assign_func_xpr = Assign(Value(Var("f", func_type)), Function(["x", Datatype.Int], Return body_xpr))
  let xpr = Block [decl_func_xpr; assign_func_xpr; Return(Apply(Value(Var("f", t_any)), [Value(Lit("5", Datatype.Int))]))]
  test_xpr "function call 1" xpr expected

  let expected = Int 20
  let xpr =
    Block [
      DeclareHelper [Declare("e", Datatype.Int)]
      While(
        Value(Lit("'a'", Datatype.Char)),
        Block [
          Assign(Value(Var("e", Datatype.Int)), Apply(Value(Var("+", t_any)), [Value(Var("e", Datatype.Int)); Value(Lit("1", Datatype.Int))]))
          If(
            Apply(Value(Var("==", t_any)), [Value(Var("e", Datatype.Int)); Value(Lit("20", Datatype.Int))]),
            Return(Value(Var("e", Datatype.Int))),
            Value Unit
           )
         ]
       )
     ]
  test_xpr "accumulator" xpr expected