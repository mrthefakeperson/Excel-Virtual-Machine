#r "paket:
nuget Fuchu"

open Fuchu
// open CompilerDatatypes.AST
// open Parser

printfn "A"

// let test_rule message (rule: 'a Rule) (input: string) (expected_output: 'a Result) =
//   testCase message <|
//     fun () -> Assert.Equal(message, expected_output, rule () (tokenize_text input))
//    |> run
//    |> ignore

// let test_parser_elements() =
//   let expected = Value(Var("-prefix", TypeClasses.f_arith_prefix))
//   test_rule "prefix" prefix "-" <| Yes(expected, [])

//   let expected = Value(Var("++suffix", TypeClasses.f_arith_prefix))
//   test_rule "suffix" suffix "++" <| Yes(expected, [])

//   let expected = Byte
//   test_rule "datatype" datatype "char" <| Yes(expected, [])

// let test_parser_value() =
//   let expected_inner = Apply(Value(Var("a", TypeClasses.any)), [Value(Var("b", TypeClasses.any))])
//   let expected = Apply(expected_inner, [Value(Var("c", TypeClasses.any)); Value(Var("d", TypeClasses.any))])
//   test_rule "value - apply" value "a(b)(c, d)" <| Yes(expected, [])

//   let expected_inner = Index(expected_inner, Value(Var("c", TypeClasses.any)))
//   let expected = Apply(expected_inner, [Value(Var("d", TypeClasses.any))])
//   test_rule "value - apply, index" value "a(b)[c](d)" <| Yes(expected, [])

//   let expected =
//     Apply(
//       Value(Var("*prefix", DT.Function([Ptr TypeClasses.any], TypeClasses.any))),
//       [Value(Var("a", TypeClasses.any))]
//      )
//   test_rule "value - prefix" value "*a" <| Yes(expected, [])
  
//   let expected_inner = Apply(Value(Var("+", TypeClasses.any)), [Value(Var("a", TypeClasses.any)); Value (Lit ("1",Int))])
//   let expected =
//     Apply(
//       Value(Var("-", TypeClasses.any)),
//       [Assign(Value(Var("a", TypeClasses.any)), expected_inner); Value(Lit("1", Int))]
//      )
//   test_rule "value - suffix" value "a++++" <| Yes(expected, [])

//   let expected =
//     Apply'.fn2("+", TypeClasses.f_arith_infix)
//      (Apply'.fn2 "." (Apply'.fn2 "->" (Value(Var("a", TypeClasses.any))) (Value(Var("b", TypeClasses.any)))) (Value(Var("c", TypeClasses.any))))
//      (Apply'.fn2 "->" (Apply'.fn2 "." (Value(Var("d", TypeClasses.any))) (Value(Var("e", TypeClasses.any)))) (Value(Var("f", TypeClasses.any))))
//   test_rule "value - access" value "a->b.c + d . e->f" <| Yes(expected, [])

//   let expected_left =
//     Apply(
//       Value(Var("+", TypeClasses.f_arith_infix)),
//       [
//         Value(Var("a", TypeClasses.any))
//         Apply(
//           Value(Var("*", TypeClasses.f_arith_infix)),
//           [
//             Value(Var("b", TypeClasses.any))
//             Value(Var("c", TypeClasses.any))
//           ]
//          )
//       ]
//      )
//   let expected_right =
//     Apply(
//       Value(Var("+", TypeClasses.f_arith_infix)),
//       [Value(Var("d", TypeClasses.any)); Value(Var("e", TypeClasses.any))]
//      )
//   let expected =
//     Apply(
//       Value(Var("&&", TypeClasses.f_logic_infix)),
//       [expected_left; expected_right]
//      )
//   test_rule "value - infix" value "a + b * c && d + e" <| Yes(expected, [])

//   let expected_right =
//     Apply(
//       Apply(
//         Value(Var("+", TypeClasses.f_arith_infix)),
//         [Value(Var("c", TypeClasses.any)); Value(Var("d", TypeClasses.any))]
//        ),
//       [Value(Var("e", TypeClasses.any))]
//      )
//   let expected_inner_assign =
//     let b = Value(Var("b", TypeClasses.any))
//     Assign(b, Apply(Value(Var("/", TypeClasses.f_arith_infix)), [b; expected_right]))
//   let expected = Assign(Value(Var("a", TypeClasses.any)), expected_inner_assign)
//   test_rule "value - assign, brackets, infix, apply" value "a = b /= (c + d)(e)" <| Yes(expected, [])

// let test_parser_declare() =
//   test_rule "declare" declare_value "extern long a" <| Yes(DeclareHelper [Declare("a", Int64)], [])

//   let expected =
//     DeclareHelper [
//       Declare("a", Ptr Int)
//       Assign(Value(Var("a", TypeClasses.any)), BuiltinASTs.stack_alloc Int (Value(Lit("2", Int))))
//       Declare("b", Int)
//       Assign(Value(Var("b", TypeClasses.any)), Value(Lit("2", Int)))
//       Declare("c", Ptr Int)
//       Assign(Value(Var("c", TypeClasses.any)), Value(Lit("3", Int)))
//       Declare("d", Ptr Int)
//      ]
//   test_rule "declare multiple" declare_value "int a[2], b = 2, *c = 3, *d" <| Yes(expected, [])

//   let expected =
//     DeclareHelper [
//       Declare("a", Ptr (Ptr Int))
//       Declare("b", Ptr Int)
//       Assign(Value(Var("b", TypeClasses.any)), BuiltinASTs.stack_alloc Int (Value(Lit("2", Int))))
//       Assign(Index(Value(Var("b", TypeClasses.any)), Value(Lit("0", Int))), Value(Lit("0", Int)))
//       Assign(Index(Value(Var("b", TypeClasses.any)), Value(Lit("1", Int))), Value(Lit("1", Int)))
//      ]
//   test_rule "declare special cases" declare_value "int **a, b[2] = {0, 1}" <| Yes(expected, [])

// let test_parser_code() =
//   let expected = Apply(Value(Var("f", TypeClasses.any)), [Value(Var("x", TypeClasses.any))])
//   test_rule "statement" code_body "f(x);" <| Yes(expected, [])

//   test_rule "statement (only `;`)" code_body ";" <| Yes(Block [], [])

//   let expected = Block [expected; Return(Value.unit)]
//   test_rule "block" code_body "{ f(x); return; }" <| Yes(expected, [])

//   let expected = If(Value(Var("a", TypeClasses.any)), Block [Value(Var("b", TypeClasses.any))], Block [])
//   test_rule "if" code_body "if (a) { b; }" <| Yes(expected, [])

//   let expected = If(Value(Var("x", TypeClasses.any)), Block [Value(Var("y", TypeClasses.any))], Block [expected])
//   test_rule "if - else" code_body "if (x) y; else if (a) { b; }" <| Yes(expected, [])

//   let expected = While(Value(Var("a", TypeClasses.any)), Block [Value(Var("b", TypeClasses.any)); Value(Var("c", TypeClasses.any))])
//   test_rule "while" code_body "while (a) { b; c; }" <| Yes(expected, [])

//   let expected_loop_cond =
//     Apply(
//       Value(Var("<", TypeClasses.f_logic_infix)),
//       [Value(Var("e", TypeClasses.any)); Value(Lit("4", Int))]
//      )
//   let expected_loop_incr =
//     Apply(
//       Value(Var("-", TypeClasses.any)), [
//         Assign(Value(Var("e", TypeClasses.any)), Apply(Value(Var("+", TypeClasses.any)), [Value(Var("e", TypeClasses.any)); Value(Lit("1", Int))]))
//         Value(Lit("1", Int))
//        ]
//      )
//   let expected =
//     Block [
//       DeclareHelper [Declare("e", Int); Assign(Value(Var("e", TypeClasses.any)), Value(Lit("0", Int)))]
//       While(
//         expected_loop_cond,
//         Block [expected_loop_incr]
//        )
//      ]
//   test_rule "for" code_body "for (int e = 0; e < 4; e++);" <| Yes(expected, [])

//   let expected =
//     Block [
//       Assign(Value(Var("e", TypeClasses.any)), Value(Lit("0", Int)))
//       While(
//         Value(Lit("'1'", Byte)),
//         Block [Value(Lit("'1'", Byte))]
//        )
//      ]
//   test_rule "for2" code_body "for (e = 0; ;);" <| Yes(expected, [])

// let test_parser_global() =
//   let expected =
//     DeclareHelper [
//       Declare("f", DT.Function([Int; Ptr Int], Int))
//       Assign(
//         Value(Var("f", DT.Function([Int; Ptr Int], Int))),
//         Function([("x", Int); ("y", Ptr Int)], Block [Return(Value(Var("x", TypeClasses.any)))])
//        )
//      ]
//   test_rule "function" declare_function "int f(int x, int* y) { return x; }" <| Yes(expected, [])
  
//   let expected = DeclAlias("a", Int)
//   test_rule "typedef" typedef "typedef a int" <| Yes(expected, [])
  
//   //let expected = DeclAlias("a", Ptr Int)  // TODO
//   //test_rule "typedef" typedef "typedef a int*" <| Yes(expected, [])

//   let expected = DeclStruct("A", [StructField("x", Int, 0, 4); StructField("y", Ptr Int, 32, 44)])
//   test_rule "struct" declare_struct "struct A {int x; int y[10];}" <| Yes(expected, [])

//   let expected = DeclUnion("A", [StructField("x", Int, 0, 4); StructField("y", Ptr Int, 0, 44)])
//   test_rule "struct" declare_union "union A {int x; int y[10];}" <| Yes(expected, [])

//   let expected =
//     GlobalParse [
//       Declare("?", TypeDef (DeclAlias("X", Int)))
//       DeclareHelper [Declare("x", TypeDef (Alias "X")); Assign(Value(Var("x", TypeClasses.any)), Value(Lit("1", Int)))]
//       Declare("anon", TypeDef (DeclStruct("_anon", [StructField("a", Int, 0, 4)])))
//       Declare("?", TypeDef (DeclStruct("Y", [StructField("b", Int, 3, 4)])))
//       DeclareHelper [Declare("y", TypeDef (Struct "Y"))]
//       Declare("?",
//         TypeDef (DeclUnion("Y", [StructField("a", Int, 0, 4); StructField("b", Byte, 0, 1); StructField("c", Ptr Int64, 0, 324)]))
//        )
//       DeclareHelper [Declare("z", TypeDef (Union "Y"))]
//       Declare("?", TypeDef (DeclUnion("ZZ", [])))
//       Declare("?", TypeDef (DeclAlias("A", TypeDef (Union "ZZ"))))
//       Declare("?", TypeDef (DeclAlias("B", TypeDef (Union "ZZ"))))
//       Declare("?", TypeDef (DeclAlias("C", TypeDef (Union "ZZ"))))
//      ]
//   test_rule "typedef, struct, union" parse_global_scope """
//     typedef X int; X x = 1;
//     struct {int a;} anon;
//     struct Y {int b:3;}; struct Y y;
//     union Y {int a; char b; long c[40];}; union Y z;
//     typedef union ZZ {} A, B, C;
//     """ <| Yes(expected, []) 

//   let expected =
//     GlobalParse [
//       DeclareHelper [Declare("x", Int64); Assign(Value(Var("x", TypeClasses.any)), Value(Lit("1", Int)))]
//       Block []
//       DeclareHelper [
//         Declare("f", DT.Function([], Void))
//         Assign(Value(Var("f", DT.Function([], Void))), Function([], Block []))
//        ]
//       DeclareHelper [
//         Declare("main", Function2 Int)
//         Assign(Value(Var("main", Function2 Int)), Function([], Block []))
//        ]
//      ]
//   test_rule "global parse" parse_global_scope "long x = 1; ; void f(void); main() {}" <| Yes(expected, [])

// let run_all() =
//   test_parser_elements()
//   test_parser_value()
//   test_parser_declare()
//   test_parser_code()
//   test_parser_global()
