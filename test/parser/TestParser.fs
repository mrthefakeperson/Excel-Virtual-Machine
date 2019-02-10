module TestParser
open Fuchu
open Lexer.Main
open Parser.Combinators
open Parser.AST
open Parser.Main

let test_rule message (rule: 'a Rule) (input: string) (expected_output: 'a Result) =
  testCase message <|
    fun () -> Assert.Equal(message, expected_output, rule () (tokenize_text input))
   |> run
   |> ignore

let test_parser_elements() =
  let expected = Value(Var("-prefix", tf_arith_prefix))
  test_rule "prefix" prefix "-" <| Yes(expected, [])

  let expected = Value(Var("++suffix", tf_arith_prefix))
  test_rule "suffix" suffix "++" <| Yes(expected, [])

  let expected = Char
  test_rule "datatype" datatype "char" <| Yes(expected, [])

let test_parser_value() =
  let expected_inner = Apply(Value(Var("a", t_any)), [Value(Var("b", t_any))])
  let expected = Apply(expected_inner, [Value(Var("c", t_any)); Value(Var("d", t_any))])
  test_rule "value - apply" value "a(b)(c, d)" <| Yes(expected, [])

  let expected_inner = Index(expected_inner, Value(Var("c", t_any)))
  let expected = Apply(expected_inner, [Value(Var("d", t_any))])
  test_rule "value - apply, index" value "a(b)[c](d)" <| Yes(expected, [])

  let expected =
    Apply(
      Value(Var("*prefix", Datatype.Function([Pointer(t_any, None)], t_any))),
      [Value(Var("a", t_any))]
     )
  test_rule "value - prefix" value "*a" <| Yes(expected, [])

  let expected_inner = Apply(Value(Var("++suffix", tf_arith_prefix)), [Value(Var("a", t_any))])
  let expected = Apply(Value(Var("++suffix", tf_arith_prefix)), [expected_inner])
  test_rule "value - suffix" value "a++++" <| Yes(expected, [])

  let expected_left =
    Apply(
      Value(Var("+", tf_arith_infix)),
      [
        Value(Var("a", t_any))
        Apply(
          Value(Var("*", tf_arith_infix)),
          [
            Value(Var("b", t_any))
            Value(Var("c", t_any))
          ]
         )
      ]
     )
  let expected_right =
    Apply(
      Value(Var("+", tf_arith_infix)),
      [Value(Var("d", t_any)); Value(Var("e", t_any))]
     )
  let expected =
    Apply(
      Value(Var("&&", tf_logic_infix)),
      [expected_left; expected_right]
     )
  test_rule "value - infix" value "a + b * c && d + e" <| Yes(expected, [])

  let expected_right =
    Apply(
      Apply(
        Value(Var("+", tf_arith_infix)),
        [Value(Var("c", t_any)); Value(Var("d", t_any))]
       ),
      [Value(Var("e", t_any))]
     )
  let expected_inner_assign =
    let b = Value(Var("b", t_any))
    Assign(b, Apply(Value(Var("/", tf_arith_infix)), [b; expected_right]))
  let expected = Assign(Value(Var("a", t_any)), expected_inner_assign)
  test_rule "value - assign, brackets, infix, apply" value "a = b /= (c + d)(e)" <| Yes(expected, [])

let test_parser_declare() =
  test_rule "declare" declare_value "extern long a" <| Yes(DeclareHelper [Declare("a", Long)], [])

  let expected =
    DeclareHelper [
      Declare("a", Pointer(Int, Some 8))
      Declare("b", Int)
      Assign(Value(Var("b", Int)), Value(Lit("2", Int)))
      Declare("c", Pointer(Int, Some 4))
      Assign(Value(Var("c", Pointer(Int, Some 4))), Value(Lit("3", Int)))
      Declare("d", Pointer(Int, Some 4))
     ]
  test_rule "declare multiple" declare_value "int a[2], b = 2, *c = 3, *d" <| Yes(expected, [])

  let expected =
    DeclareHelper [
      Declare("a", Pointer(Pointer(Int, Some 4), Some 4))
      Declare("b", Pointer(Int, Some 8))
      Assign(Index(Value(Var("b", Pointer(Int, Some 8))), Value(Lit("0", Int))), Value(Lit("0", Int)))
      Assign(Index(Value(Var("b", Pointer(Int, Some 8))), Value(Lit("1", Int))), Value(Lit("1", Int)))
     ]
  test_rule "declare special cases" declare_value "int **a, b[2] = {0, 1}" <| Yes(expected, [])

let test_parser_code() =
  let expected = Apply(Value(Var("f", t_any)), [Value(Var("x", t_any))])
  test_rule "statement" code_body "f(x);" <| Yes(expected, [])

  test_rule "statement (only `;`)" code_body ";" <| Yes(Block [], [])

  let expected = Block [expected; Return(Value Unit)]
  test_rule "block" code_body "{ f(x); return; }" <| Yes(expected, [])

  let expected = If(Value(Var("a", t_any)), Block [Value(Var("b", t_any))], Block [])
  test_rule "if" code_body "if (a) { b; }" <| Yes(expected, [])

  let expected = If(Value(Var("x", t_any)), Block [Value(Var("y", t_any))], Block [expected])
  test_rule "if - else" code_body "if (x) y; else if (a) { b; }" <| Yes(expected, [])

  let expected = While(Value(Var("a", t_any)), Block [Value(Var("b", t_any)); Value(Var("c", t_any))])
  test_rule "while" code_body "while (a) { b; c; }" <| Yes(expected, [])

  let expected_loop_cond =
    Apply(
      Value(Var("<", tf_logic_infix)),
      [Value(Var("e", t_any)); Value(Lit("4", Int))]
     )
  let expected_loop_incr =
    Apply(Value(Var("++suffix", tf_arith_prefix)), [Value(Var("e", t_any))])
  let expected =
    Block [
      DeclareHelper [Declare("e", Int); Assign(Value(Var("e", Int)), Value(Lit("0", Int)))]
      While(
        expected_loop_cond,
        Block [expected_loop_incr]
       )
     ]
  test_rule "for" code_body "for (int e = 0; e < 4; e++);" <| Yes(expected, [])

  let expected =
    Block [
      Assign(Value(Var("e", t_any)), Value(Lit("0", Int)))
      While(
        Value(Lit("1", Char)),
        Block [Value(Lit("1", Char))]
       )
     ]
  test_rule "for2" code_body "for (e = 0; ;);" <| Yes(expected, [])

let test_parser_global() =
  let expected =
    DeclareHelper [
      Declare("f", Datatype.Function([Int; Pointer(Int, Some 4)], Int))
      Assign(
        Value(Var("f", Datatype.Function([Int; Pointer(Int, Some 4)], Int))),
        Function([("x", Int); ("y", Pointer(Int, Some 4))], Block [Return(Value(Var("x", t_any)))])
       )
     ]
  test_rule "function" declare_function "int f(int x, int* y) { return x; }" <| Yes(expected, [])

  let expected =
    GlobalParse [
      DeclareHelper [Declare("x", Long); Assign(Value(Var("x", Long)), Value(Lit("1", Int)))]
      Block []
      DeclareHelper [
        Declare("f", Datatype.Function([], Void))
        Assign(Value(Var("f", Datatype.Function([], Void))), Function([], Value Unit))
       ]
      DeclareHelper [
        Declare("main", Datatype.Function([], Void))
        Assign(Value(Var("main", Datatype.Function([], Void))), Function([], Block []))
       ]
     ]
  test_rule "global parse" parse_global_scope "long x = 1; ; void f(void); main() {}" <| Yes(expected, [])

let run_all() =
  test_parser_elements()
  test_parser_value()
  test_parser_declare()
  test_parser_code()
  test_parser_global()
