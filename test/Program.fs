open Fuchu
open System.IO
open Lexer.Main
open Parser.Combinators
open Parser.AST
open Parser.Main

let test_rule message (rule: 'a Rule) (input: string) (expected_output: 'a Result) =
  testCase message <|
    fun () -> Assert.Equal(message, expected_output, rule () (tokenize_text input))
   |> run
   |> ignore

let t_any = Unknown []
let t_num = Unknown [Int; Char; Long]

let test_parser_elements() =
  let expected = Value(Variable("-prefix", Datatype.Function([t_num], t_num)))
  test_rule "prefix" prefix "-" <| Yes(expected, [])

  let expected = Value(Variable("++suffix", Datatype.Function([t_num], t_num)))
  test_rule "suffix" suffix "++" <| Yes(expected, [])

  let expected = Char
  test_rule "datatype" datatype "char" <| Yes(expected, [])

let test_parser_value() =
  let expected_inner = Apply(Value(Variable("a", t_any)), [Value(Variable("b", t_any))])
  let expected = Apply(expected_inner, [Value(Variable("c", t_any)); Value(Variable("d", t_any))])
  test_rule "value - apply" value "a(b)(c, d)" <| Yes(expected, [])

  let expected_inner = Index(expected_inner, Value(Variable("c", t_any)))
  let expected = Apply(expected_inner, [Value(Variable("d", t_any))])
  test_rule "value - apply, index" value "a(b)[c](d)" <| Yes(expected, [])

  let expected =
    Apply(
      Value(Variable("*prefix", Datatype.Function([Pointer t_any], t_any))),
      [Value(Variable("a", t_any))]
     )
  test_rule "value - prefix" value "*a" <| Yes(expected, [])

  let expected_inner = Apply(Value(Variable("++suffix", Datatype.Function([t_num], t_num))), [Value(Variable("a", t_any))])
  let expected = Apply(Value(Variable("++suffix", Datatype.Function([t_num], t_num))), [expected_inner])
  test_rule "value - suffix" value "a++++" <| Yes(expected, [])

  let expected_left =
    Apply(
      Value(Variable("+", Datatype.Function([t_num; t_num], t_num))),
      [
        Value(Variable("a", t_any))
        Apply(
          Value(Variable("*", Datatype.Function([t_num; t_num], t_num))),
          [
            Value(Variable("b", t_any))
            Value(Variable("c", t_any))
          ]
         )
      ]
     )
  let expected_right =
    Apply(
      Value(Variable("+", Datatype.Function([t_num; t_num], t_num))),
      [Value(Variable("d", t_any)); Value(Variable("e", t_any))]
     )
  let expected =
    Apply(
      Value(Variable("&&", Datatype.Function([t_num; t_num], Char))),
      [expected_left; expected_right]
     )
  test_rule "value - infix" value "a + b * c && d + e" <| Yes(expected, [])

  let expected_right =
    Apply(
      Apply(
        Value(Variable("+", Datatype.Function([t_num; t_num], t_num))),
        [Value(Variable("c", t_any)); Value(Variable("d", t_any))]
       ),
      [Value(Variable("e", t_any))]
     )
  let expected_inner_assign =
    let b = Value(Variable("b", t_any))
    Assign(b, Apply(Value(Variable("/", Datatype.Function([t_num; t_num], t_num))), [b; expected_right]))
  let expected = Assign(Value(Variable("a", t_any)), expected_inner_assign)
  test_rule "value - assign, brackets, infix, apply" value "a = b /= (c + d)(e)" <| Yes(expected, [])

let test_parser_declare() =
  test_rule "declare" declare_value "extern long a" <| Yes(Declare [Value(Variable("a", Long))], [])

  let expected =
    Declare [
      Assign(Value(Variable("a", Pointer Int)), Value(Variable("placeholder", t_any)))
      Assign(Value(Variable("b", Int)), Value(Literal("2", Int)))
      Assign(Value(Variable("c", Pointer Int)), Value(Literal("3", Int)))
      Value(Variable("d", Pointer Int))
     ]
  test_rule "declare multiple" declare_value "int a[1], b = 2, *c = 3, *d" <| Yes(expected, [])

let test_parser_code() =
  let expected = Apply(Value(Variable("f", t_any)), [Value(Variable("x", t_any))])
  test_rule "statement" code_body "f(x);" <| Yes(expected, [])

  test_rule "statement (only `;`)" code_body ";" <| Yes(Block [], [])

  let expected = Block [expected; Return(Value Unit)]
  test_rule "block" code_body "{ f(x); return; }" <| Yes(expected, [])

  let expected = If(Value(Variable("a", t_any)), Block [Value(Variable("b", t_any))], Block [])
  test_rule "if" code_body "if (a) { b; }" <| Yes(expected, [])

  let expected = If(Value(Variable("x", t_any)), Value(Variable("y", t_any)), expected)
  test_rule "if - else" code_body "if (x) y; else if (a) { b; }" <| Yes(expected, [])

  let expected = While(Value(Variable("a", t_any)), Block [Value(Variable("b", t_any)); Value(Variable("c", t_any))])
  test_rule "while" code_body "while (a) { b; c; }" <| Yes(expected, [])

  let expected_loop_cond =
    Apply(
      Value(Variable("<", Datatype.Function([t_num; t_num], Char))),
      [Value(Variable("e", t_any)); Value(Literal("4", Int))]
     )
  let expected_loop_incr =
    Apply(Value(Variable("++suffix", Datatype.Function([t_num], t_num))), [Value(Variable("e", t_any))])
  let expected =
    Block [
      Declare [Assign(Value(Variable("e", Int)), Value(Literal("0", Int)))]
      While(
        expected_loop_cond,
        Block [Block []; expected_loop_incr]
       )
     ]
  test_rule "for" code_body "for (int e = 0; e < 4; e++);" <| Yes(expected, [])

  let expected =
    Block [
      Assign(Value(Variable("e", t_any)), Value(Literal("0", Int)))
      While(
        Value(Literal("1", Char)),
        Block [Block []; Value(Literal("1", Char))]
       )
     ]
  test_rule "for2" code_body "for (e = 0; ;);" <| Yes(expected, [])

let test_parser_global() =
  let expected =
    Declare [
      Assign(
        Value(Variable("f", Datatype.Function([Int; Int], Int))),
        Function([Variable("x", Int); Variable("y", Int)], Block [Return(Value(Variable("x", t_any)))])
       )
     ]
  test_rule "function" declare_function "int f(int x, int y) { return x; }" <| Yes(expected, [])

  let expected =
    GlobalParse [
      Declare [Assign(Value(Variable("x", Long)), Value(Literal("1", Int)))]
      Block []
      Declare [Assign(Value(Variable("f", Datatype.Function([], Void))), Function([], Value Unit))]
      Declare [Assign(Value(Variable("main", Datatype.Function([], Void))), Function([], Block []))]
     ]
  test_rule "global parse" parse_global_scope "long x = 1; ; void f(void); main() {}" <| Yes(expected, [])

let test_parser_speed() =
  let run_parser fname =
    let tokens = tokenize_text (File.ReadAllText fname)
    try
      let ast = parse_tokens_to_ast tokens
      printf "."
    with ex -> printfn "%A" ex

  let test =
    testCase "parser speed test" <|
      fun _ ->
        Directory.GetFiles("../../../samples")
         |> Array.sortBy (fun fname -> int(fname.Replace(".c", "").Replace("../../../samples\\", "")))
         |> Seq.take 17
         |> Seq.iter run_parser
  ignore (run test)
  ignore (System.Console.ReadLine())
//let simpleTest =
//    testCase "A simple test" <| 
//        fun _ -> Assert.Equal("2+2", 4, 2+2)

[<EntryPoint>]
let main _ =
  test_parser_elements()
  test_parser_value()
  test_parser_declare()
  test_parser_code()
  test_parser_global()

  while true do
    test_parser_speed()
    
  0