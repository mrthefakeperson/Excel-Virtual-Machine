module TestCodegen.TestTypeCheck
open Fuchu
open Lexer.Main
open Parser.AST
open Parser.Combinators
open Parser.Main
open Codegen.Tables
open Codegen.TypeCheck

let test_check_type message ast expected_xpr expected_t =
  testCase message <|
    fun () -> Assert.Equal(message, (expected_xpr, expected_t), check_type empty_symbol_table ast)
   |> run
   |> ignore

let test_basics() =
  let cast dtype ast = Apply(Value(Var("\cast", dtype)), [ast])

  let expected = Value(Lit("1.f", Float))
  test_check_type "value - identity" expected expected Float

  let xpr = Block [Declare("x", Float); Value(Var("x", t_any))]
  let expected = Block [Declare("x", Float); Value(Var("x", Float))]
  test_check_type "declare" xpr expected Void

  let xpr =
    Block [
      Declare("f", Datatype.Function([Char], Double))
      Apply(Value(Var("f", t_any)), [Value(Lit("1", Int))])
     ]
  let expected =
    Block [
      Declare("f", Datatype.Function([Char], Double))
      Apply(Value(Var("f", Datatype.Function([Char], Double))), [cast Char (Value(Lit("1", Int)))])
     ]
  test_check_type "cast when applying function (known signature)" xpr expected Void

  let xpr =
    Block [
      DeclareHelper [
        Declare("x", Pointer(Int, Some 4))
        Assign(Index(Value(Var("x", t_any)), Value(Lit("0L", Long))), Value(Lit("'c'", Char)))
        Declare("y", Char)
       ]
      If(
        Value(Lit("1", Int)),
        Value(Lit("2", Int)),
        Value(Lit("3L", Long))
       )
      While(
        Value(Lit("1", Int)),
        Block [
          Declare("y", Int)
          Value(Var("y", t_any))
         ]
       )
      Return (Value(Var("y", t_any)))
     ]
  let expected =
    Block [
      Declare("x", Pointer(Int, Some 4))
      Assign(Index(Value(Var("x", Pointer(Int, Some 4))), cast Int (Value(Lit("0L", Long)))), cast Int (Value(Lit("'c'", Char))))
      Declare("y", Char)
      If(
        cast Char (Value(Lit("1", Int))),
        cast Long (Value(Lit("2", Int))),
        Value(Lit("3L", Long))
       )
      While(
        cast Char (Value(Lit("1", Int))),
        Block [
          Declare("y", Int)
          Value(Var("y", Int))
         ]
       )
      Return (Value(Var("y", Char)))
     ]
  test_check_type "general casting and scoping" xpr expected Void

  let xpr =
    GlobalParse [
      Declare("abc", Datatype.Int)
      Declare("f", Datatype.Function([], Datatype.Void))
      Assign(Value(Var("f", Datatype.Function([], Datatype.Void))),
        Function([],
          Block [Value(Var("abc", t_any))]
         )
       )
     ]
  let expected =
    GlobalParse [
      Declare("abc", Datatype.Int)
      Declare("f", Datatype.Function([], Datatype.Void))
      Assign(Value(Var("f", Datatype.Function([], Datatype.Void))),
        Function([],
          Block [Value(Var("abc", Datatype.Int))]
         )
       )
     ]
  test_check_type "infer global type" xpr expected Void

  let xpr =
    GlobalParse [
      Declare("f", Datatype.Function([Datatype.Float], Datatype.Void))
      Assign(Value(Var("f", Datatype.Function([Datatype.Float], Datatype.Void))),
        Function(["_", Datatype.Float],
          Block [Apply(Value(Var("f", t_any)), [Value(Lit("1", Datatype.Int))])]
         )
       )
     ]
  let expected =
    GlobalParse [
      Declare("f", Datatype.Function([Datatype.Float], Datatype.Void))
      Assign(Value(Var("f", Datatype.Function([Datatype.Float], Datatype.Void))),
        Function(["_", Datatype.Float],
          Block [
            Apply(Value(Var("f", Datatype.Function([Datatype.Float], Datatype.Void))),
              [cast Datatype.Float (Value(Lit("1", Datatype.Int)))]
             )
           ]
         )
       )
     ]
  test_check_type "recursion" xpr expected Void

  let xpr =
    Apply(
      Value(Var("*prefix", t_any)),
      [Value(Lit("0xab", Datatype.Pointer(Datatype.Long, Some 16)))]
     )
  let expected =
    Apply(
      Value(Var("*prefix", Datatype.Function([Datatype.Pointer(Datatype.Long, None)], Datatype.Long))),
      [Value(Lit("0xab", Datatype.Pointer(Datatype.Long, Some 16)))]
     )
  test_check_type "deref" xpr expected Datatype.Long

let test_parse_check message str expected_xpr =
  testCase message <|
      fun () ->
        //printfn "%A" (parse_global_scope () (tokenize_text str))
        match parse_global_scope () (tokenize_text str) with
        |Yes(ast, []) -> Assert.Equal(message, (expected_xpr, Void), check_type empty_symbol_table ast)
        |_ -> failwith "parse error"
   |> run
   |> ignore

let test_check_parser_output() =
  let str = "char a = 'a'; int f() { return a; }"
  let expected =
    GlobalParse [
      Declare ("a", Char)
      Assign (Value (Var ("a", Char)), Value (Lit ("'a'", Char)))
      Declare ("f", Datatype.Function ([],Int))
      Assign (
        Value (Var ("f", Datatype.Function ([],Int))),
        Function ([],
          Block [
            Return (cast Int (Value (Var ("a",Char))))
           ]
         )
       )
     ]
  test_parse_check "cast return value" str expected

  let str = "int f() { for (int e = 0; e < 5; e++) { if (e == 0) e = e + 1; } }"
  let expected =
    GlobalParse [
      Declare("f", Datatype.Function([], Int))
      Assign(
        Value(Var("f", Datatype.Function([], Int))),
        Function([],
          Block [
            Block [
              Declare ("e",Int)
              Assign (Value (Var ("e",Int)),Value (Lit ("0",Int)))
              While(
                Apply(Value(Var("<", Datatype.Function([Int; Int], Char))), [Value (Var ("e",Int)); Value (Lit ("5",Int))]),
                Block [
                  If(
                    Apply(Value(Var("==", Datatype.Function([Int; Int], Char))), [Value(Var("e", Int)); Value(Lit("0", Int))]),
                    Block [
                      Assign(
                        Value(Var("e", Int)),
                        Apply(Value(Var("+", Datatype.Function([Int; Int], Int))), [Value(Var("e", Int)); Value(Lit("1", Int))])
                       )
                     ],
                    Block []
                   )
                  Apply(Value(Var("++suffix", Datatype.Function([Int], Int))), [Value(Var("e", Int))])
                 ]
               )
             ]
           ]
         )
       )
     ]
  test_parse_check "conditional & loop" str expected

  let str = "int f(long a, char *b) { int ***c, d[10]; a; b; c; d; }"
  let expected =
    GlobalParse [
      Declare("f", Datatype.Function([Long; Pointer(Char, Some 1)], Int))
      Assign(
        Value(Var("f", Datatype.Function([Long; Pointer(Char, Some 1)], Int))),
        Function([("a", Long); ("b", Pointer(Char, Some 1))],
          Block [
            Declare("c", Pointer(Pointer(Pointer(Int, Some 4), Some 4), Some 4))
            Declare("d", Pointer(Int, Some 40))
            Value(Var("a", Long)); Value(Var("b", Pointer(Char, Some 1)))
            Value(Var("c", Pointer(Pointer(Pointer(Int, Some 4), Some 4), Some 4))); Value(Var("d", Pointer(Int, Some 40)))
           ]
         )
       )
     ]
  test_parse_check "various declarations" str expected

  let str = "int f() {} \n void g() { f() + 3L; }"
  let expected =
    GlobalParse [
      Declare("f", Datatype.Function([], Int))
      Assign(Value(Var("f", Datatype.Function([], Int))), Function([], Block []))
      Declare("g", Datatype.Function([], Void))
      Assign(
        Value(Var("g", Datatype.Function([], Void))),
        Function([],
          Block [
            Apply(
              Value(Var("+", Datatype.Function([Long; Long], Long))),
              [cast Long (Apply(Value(Var("f", Datatype.Function([], Int))), [])); Value(Lit("3L", Long))]
             )
           ]
         )
       )
     ]
  test_parse_check "function declaration" str expected

let run_all() =
  test_basics()
  test_check_parser_output()