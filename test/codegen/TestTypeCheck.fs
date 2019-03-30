module TestCodegen.TestTypeCheck
open Fuchu
open Lexer.Main
open Parser.Datatype
open Parser.AST
open Parser.Combinators
open Parser.Main
open Codegen.Tables
open Codegen.TypeCheck

let test_check_type message ast expected_xpr expected_t =
  testCase message <|
    fun () -> Assert.Equal(message, (expected_xpr, expected_t), check_type (empty_symbol_table()) ast)
   |> run
   |> ignore

let test_basics() =
  let expected = Value(Lit("1.f", Float))
  test_check_type "value - identity" expected expected Float

  let xpr = Block [Declare("x", Float); Value(Var("x", TypeClasses.any))]
  let expected = Block [Declare("x", Float); Value(Var("x", Float))]
  test_check_type "declare" xpr expected Void

  let xpr =
    Block [
      Declare("f", DT.Function([Byte], Double))
      Apply(Value(Var("f", TypeClasses.any)), [Value(Lit("1", Int))])
     ]
  let expected =
    Block [
      Declare("f", DT.Function([Byte], Double))
      Apply(Value(Var("f", DT.Function([Byte], Double))), [BuiltinASTs.cast Byte (Value(Lit("1", Int)))])
     ]
  test_check_type "cast when applying function (known signature)" xpr expected Void

  let xpr =
    Block [
      DeclareHelper [
        Declare("x", Ptr Int)
        Assign(Index(Value(Var("x", TypeClasses.any)), Value(Lit("0L", Int64))), Value(Lit("'c'", Byte)))
        Declare("y", Byte)
       ]
      If(
        Value(Lit("1", Int)),
        Value(Lit("2", Int)),
        Value(Lit("3L", Int64))
       )
      While(
        Value(Lit("1", Int)),
        Block [
          Declare("y", Int)
          Value(Var("y", TypeClasses.any))
         ]
       )
      Return (Value(Var("y", TypeClasses.any)))
     ]
  let expected =
    Block [
      Declare("x", Ptr Int)
      Assign(Index(Value(Var("x", Ptr Int)), BuiltinASTs.cast Int (Value(Lit("0L", Int64)))), BuiltinASTs.cast Int (Value(Lit("'c'", Byte))))
      Declare("y", Byte)
      If(
        BuiltinASTs.cast Byte (Value(Lit("1", Int))),
        BuiltinASTs.cast Int64 (Value(Lit("2", Int))),
        Value(Lit("3L", Int64))
       )
      While(
        BuiltinASTs.cast Byte (Value(Lit("1", Int))),
        Block [
          Declare("y", Int)
          Value(Var("y", Int))
         ]
       )
      Return (Value(Var("y", Byte)))
     ]
  test_check_type "general casting and scoping" xpr expected Void

  let xpr =
    GlobalParse [
      Declare("abc", DT.Int)
      Declare("f", DT.Function([], DT.Void))
      Assign(Value(Var("f", DT.Function([], DT.Void))),
        Function([],
          Block [Value(Var("abc", TypeClasses.any))]
         )
       )
     ]
  let expected =
    GlobalParse [
      Declare("abc", DT.Int)
      Declare("f", DT.Function([], DT.Void))
      Assign(Value(Var("f", DT.Function([], DT.Void))),
        Function([],
          Block [Value(Var("abc", DT.Int))]
         )
       )
     ]
  test_check_type "infer global type" xpr expected Void

  let xpr =
    GlobalParse [
      Declare("f", DT.Function([DT.Float], DT.Void))
      Assign(Value(Var("f", DT.Function([DT.Float], DT.Void))),
        Function(["_", DT.Float],
          Block [Apply(Value(Var("f", TypeClasses.any)), [Value(Lit("1", DT.Int))])]
         )
       )
     ]
  let expected =
    GlobalParse [
      Declare("f", DT.Function([DT.Float], DT.Void))
      Assign(Value(Var("f", DT.Function([DT.Float], DT.Void))),
        Function(["_", DT.Float],
          Block [
            Apply(Value(Var("f", DT.Function([DT.Float], DT.Void))),
              [BuiltinASTs.cast DT.Float (Value(Lit("1", DT.Int)))]
             )
           ]
         )
       )
     ]
  test_check_type "recursion" xpr expected Void

  let xpr =
    Apply(
      Value(Var("*prefix", TypeClasses.any)),
      [Value(Lit("0xab", Ptr Int64))]
     )
  let expected =
    Apply(
      Value(Var("*prefix", DT.Function([Ptr Int64], Int64))),
      [Value(Lit("0xab", Ptr Int64))]
     )
  test_check_type "deref" xpr expected Int64

let test_parse_check message str expected_xpr =
  testCase message <|
      fun () ->
        //printfn "%A" (parse_global_scope () (tokenize_text str))
        match parse_global_scope () (tokenize_text str) with
        |Yes(ast, []) -> Assert.Equal(message, (expected_xpr, Void), check_type (empty_symbol_table()) ast)
        |_ -> failwith "parse error"
   |> run
   |> ignore

let test_check_parser_output() =
  let str = "char a = 'a'; int f(void) { return a; }"
  let expected =
    GlobalParse [
      Declare ("a", Byte)
      Assign (Value (Var ("a", Byte)), Value (Lit ("'a'", Byte)))
      Declare ("f", DT.Function ([],Int))
      Assign (
        Value (Var ("f", DT.Function ([],Int))),
        Function ([],
          Block [
            Return (BuiltinASTs.cast Int (Value (Var ("a",Byte))))
           ]
         )
       )
     ]
  test_parse_check "cast return value" str expected

  let str = "int f(void) { for (int e = 0; e < 5; ++e) { if (e == 0) e = e + 1; } }"
  let expected =
    GlobalParse [
      Declare("f", DT.Function([], Int))
      Assign(
        Value(Var("f", DT.Function([], Int))),
        Function([],
          Block [
            Block [
              Declare ("e",Int)
              Assign (Value (Var ("e",Int)),Value (Lit ("0",Int)))
              While(
                Apply(Value(Var("<", DT.Function([Int; Int], Byte))), [Value (Var ("e",Int)); Value (Lit ("5",Int))]),
                Block [
                  If(
                    Apply(Value(Var("==", DT.Function([Int; Int], Byte))), [Value(Var("e", Int)); Value(Lit("0", Int))]),
                    Block [
                      Assign(
                        Value(Var("e", Int)),
                        Apply(Value(Var("+", DT.Function([Int; Int], Int))), [Value(Var("e", Int)); Value(Lit("1", Int))])
                       )
                     ],
                    Block []
                   )
                  Assign(Value(Var("e", Int)),
                    Apply(Value(Var("+", DT.Function([Int; Int], Int))), [Value(Var("e", Int)); Value(Lit("1", Int))])
                   )
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
      Declare("f", DT.Function([Int64; Ptr Byte], Int))
      Assign(
        Value(Var("f", DT.Function([Int64; Ptr Byte], Int))),
        Function([("a", Int64); ("b", Ptr Byte)],
          Block [
            Declare("c", Ptr (Ptr (Ptr Int)))
            Declare("d", Ptr Int)
            Assign(Value (Var ("d", Ptr Int)),
              BuiltinASTs.stack_alloc Int (Value (Lit("10", Int)))
             )
            Value(Var("a", Int64)); Value(Var("b", Ptr Byte))
            Value(Var("c", Ptr (Ptr (Ptr Int)))); Value(Var("d", Ptr Int))
           ]
         )
       )
     ]
  test_parse_check "various declarations" str expected

  let str = "int f(void) {} \n void g() { f() + 3L; }"
  let expected =
    GlobalParse [
      Declare("f", DT.Function([], Int))
      Assign(Value(Var("f", DT.Function([], Int))), Function([], Block []))
      Declare("g", Function2 Void)
      Assign(
        Value(Var("g", Function2 Void)),
        Function([],
          Block [
            Apply(
              Value(Var("+", DT.Function([Int64; Int64], Int64))),
              [BuiltinASTs.cast Int64 (Apply(Value(Var("f", DT.Function([], Int))), [])); Value(Lit("3L", Int64))]
             )
           ]
         )
       )
     ]
  test_parse_check "function declaration" str expected

  let str = "int f() {} \n void g(void) { f(); f(1); f(1, 2, 3); }"
  let expected =
    GlobalParse [
      Declare("f", Function2 Int)
      Assign(Value(Var("f", Function2 Int)), Function([], Block []))
      Declare("g", DT.Function([], Void))
      Assign(
        Value(Var("g", DT.Function([], Void))),
        Function([],
          Block [
            Apply(Value(Var("f", Function2 Int)), [])
            Apply(Value(Var("f", Function2 Int)), [Value(Lit("1", Int))])
            Apply(Value(Var("f", Function2 Int)), [Value(Lit("1", Int)); Value(Lit("2", Int)); Value(Lit("3", Int))])
           ]
         )
       )
     ]
  test_parse_check "function declaration with flexible args" str expected

let run_all() =
  test_basics()
  test_check_parser_output()