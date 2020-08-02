#if !TEST
#load "../../build_utils/.fake/build.fsx/intellisense.fsx"
#endif
#r "../../build/Utils.dll"
#r "../../build/CompilerDatatypes.dll"
#r "../../build/Parser.dll"

open Fuchu
open ParserCombinators
open CompilerDatatypes.DT
open CompilerDatatypes.Token
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SyntaxAST
open Parser.Lex
open Parser.Parse
open Parser.Main

type 'a Rule = 'a Expr.Rule

let test_rule message (rule: 'a Rule) (input: string) (expected_output: 'a) =
  testCase message (fun () ->
    let result = run_parser rule {stream = tokenize_text input}
    Assert.Equal(message, expected_output, result)
   )
   |> run
   |> ignore

let test_parser_elements() =
  let expected = Var("-prefix", TypeClasses.f_arith_prefix)
  test_rule "prefix" Expr.prefix "-" expected

  let expected = Var("++suffix", TypeClasses.f_arith_prefix)
  test_rule "suffix" Expr.suffix "++" expected

  let expected = Byte
  test_rule "datatype" Expr.datatype "char" expected

test_parser_elements()


let test_ast_inverse message (rule: AST Rule) (input: string) =
  testCase message (fun () ->
    let pprint s = pprint_c_program (run_parser rule {stream = tokenize_text s})
    let pprinted = pprint input
    // printfn ">>> %A" pprinted
    Assert.Equal("pretty print inverse property", pprinted, pprint pprinted)
   )
   |> run
   |> ignore

let test_ast_rule message rule input expected_output =
  test_rule message rule input expected_output
  test_ast_inverse message rule input

let test_parser_expr() =
  let expected_inner = Apply'.fn "a" (V (Var("b", TypeClasses.any)))
  let expected = Apply(expected_inner, [V (Var("c", TypeClasses.any)); V (Var("d", TypeClasses.any))])
  test_ast_rule "value - apply" Expr.expr "a(b)(c, d)" expected

  let expected_inner = BuiltinASTs.index expected_inner (V (Var("c", TypeClasses.any)))
  let expected = Apply(expected_inner, [V (Var("d", TypeClasses.any))])
  test_ast_rule "value - apply, index" Expr.expr "a(b)[c](d)" expected

  let expected = V (Var("a", TypeClasses.any))
  test_ast_rule "value - bracket/value recursion" Expr.expr "(((((a)))))" expected

  let expected =
    Apply'.fn("*prefix", DT.Function([Ptr TypeClasses.any], TypeClasses.any)) (V (Var("a", TypeClasses.any)))
  test_ast_rule "value - prefix" Expr.expr "*a" expected

  let expected = Apply'.fn("!", TypeClasses.f_arith_prefix) expected
  test_ast_rule "value - prefix 2" Expr.expr "!*a" expected
  
  let expected_inner = Apply'.fn2("+", TypeClasses.f_arith_infix) (V (Var("a", TypeClasses.any))) (V (Lit("1", Int)))
  let expected =
    Apply'.fn2("-", TypeClasses.f_arith_infix)
     (Assign(V (Var("a", TypeClasses.any)), expected_inner))
     (V (Lit("1", Int)))
  test_ast_rule "value - suffix" Expr.expr "a++" expected

  let expected =
    Apply'.fn2("+", TypeClasses.f_arith_infix)
     (Apply'.fn2 "." (Apply'.fn2 "->" (V (Var("a", TypeClasses.any))) (V (Var("b", TypeClasses.any)))) (V (Var("c", TypeClasses.any))))
     (Apply'.fn2 "->" (Apply'.fn2 "." (V (Var("d", TypeClasses.any))) (V (Var("e", TypeClasses.any)))) (V (Var("f", TypeClasses.any))))
  test_ast_rule "value - access" Expr.expr "a->b.c + d . e->f" expected

  let expected_left =
    Apply'.fn2("+", TypeClasses.f_arith_infix) (V (Var("a", TypeClasses.any)))
     (Apply'.fn2("*", TypeClasses.f_arith_infix) (V (Var("b", TypeClasses.any))) (V (Var("c", TypeClasses.any))))
  let expected_right =
    Apply'.fn2("+", TypeClasses.f_arith_infix) (V (Var("d", TypeClasses.any))) (V (Var("e", TypeClasses.any)))
  let expected = Apply'.fn2("&&", TypeClasses.f_logic_infix) expected_left expected_right
  test_ast_rule "value - infix" Expr.expr "a + b * c && d + e" expected

  let expected_inner =
    Apply'.fn2("+", TypeClasses.f_arith_infix) (V (Var("c", TypeClasses.any))) (V (Var("d", TypeClasses.any)))
  let expected_right = Apply(expected_inner, [V (Var("e", TypeClasses.any))])
  let expected_inner_assign =
    let b = V (Var("b", TypeClasses.any))
    Assign(b, Apply'.fn2("/", TypeClasses.f_arith_infix) b expected_right)
  let expected = Assign(V (Var("a", TypeClasses.any)), expected_inner_assign)
  test_ast_rule "value - assign, brackets, infix, apply" Expr.expr "a = b /= (c + d)(e)" expected

  let expected = BuiltinASTs.cast (Ptr Int) (V (Var("a", TypeClasses.any)))
  test_ast_rule "value - cast" Expr.expr "(int *)a" expected

  let expected = If(V (Lit("1", Int)), V (Lit("7", Int)), V (Lit("8", Int)))
  test_ast_rule "value - ternary operator" Expr.expr "1 ? 7 : 8" expected

test_parser_expr()


let test_ast_list_inverse message (rule : AST list Rule) (input: string) =
  testCase message (fun () ->
    let ast_list = run_parser rule {stream = tokenize_text input}
    let code_block = pprint_c_program (Block ast_list)
    // printfn ">>> %A" code_block
    let rule = Control.code_body ->/ Block
    let pprint s = pprint_c_program (run_parser rule {stream = tokenize_text s})
    let pprinted = pprint code_block
    Assert.Equal("pretty print inverse property (list)", pprinted, pprint pprinted)
    List.iter (fun ast ->
      let ast = Block [ast]
      let pprint s = pprint_c_program (run_parser rule {stream = tokenize_text s})
      let pprinted = pprint (pprint_c_program ast)
      Assert.Equal("pretty print inverse property (list element)", pprinted, pprint pprinted)
     ) ast_list
   )
   |> run
   |> ignore

let test_ast_list_rule message rule input expected_output =
  test_rule message rule input expected_output
  test_ast_list_inverse message rule input

let test_parser_declare() =
  let input = "extern long a"
  let expected = [ Declare("a", Int64) ]
  test_ast_list_rule "declare" Control.declare_expr input expected

  let input = "int a[2], b = 2, *c = 3, *d"
  let expected = [
    Declare("a", TypeDef (Array(1, Int)))
    Assign(V (Var("a", TypeDef (Array(1, Int)))), BuiltinASTs.stack_alloc Int [V (Lit("2", Int))])
    Declare("b", Int)
    Assign(V (Var("b", Int)), V (Lit("2", Int)))
    Declare("c", Ptr Int)
    Assign(V (Var("c", Ptr Int)), V (Lit("3", Int)))
    Declare("d", Ptr Int)
   ]
  test_ast_list_rule "declare multiple" Control.declare_expr input expected

  let input = "int **a, b[2] = {0, 1}"
  let expected = [
    Declare("a", Ptr (Ptr Int))
    Declare("b", TypeDef (Array(1, Int)))
    Assign(V (Var("b", TypeDef (Array(1, Int)))), BuiltinASTs.stack_alloc Int [V (Lit("2", Int))])
    Assign(BuiltinASTs.index (V (Var("b", TypeDef (Array(1, Int))))) (V (Lit("0", Int))), V (Lit("0", Int)))
    Assign(BuiltinASTs.index (V (Var("b", TypeDef (Array(1, Int))))) (V (Lit("1", Int))), V (Lit("1", Int)))
   ]
  test_ast_list_rule "declare special cases" Control.declare_expr input expected

  let input = "char a[1][4 + 5], b[a[1]][f()][0]"
  let expected_alloc_1 =
    BuiltinASTs.stack_alloc Byte [
      V (Lit("1", Int))
      Apply'.fn2("+", TypeClasses.f_arith_infix) (V (Lit("4", Int))) (V (Lit("5", Int)))
     ]
  let expected_alloc_2 =
    BuiltinASTs.stack_alloc Byte [
      BuiltinASTs.index (V (Var("a", TypeClasses.any))) (V (Lit("1", Int)))
      Apply(V (Var("f", TypeClasses.any)), [])
      V (Lit("0", Int))
     ]
  let expected = [
    Declare("a", TypeDef (Array(2, Byte)))
    Assign(V (Var("a", TypeDef (Array(2, Byte)))), expected_alloc_1)
    Declare("b", TypeDef (Array(3, Byte)))
    Assign(V (Var("b", TypeDef (Array(3, Byte)))), expected_alloc_2)
   ]
  test_ast_list_rule "declare multi-dim arrays" Control.declare_expr input expected

test_parser_declare()


let test_parser_code() =
  let expected = [Apply'.fn "f" (V (Var("x", TypeClasses.any)))]
  test_ast_list_rule "statement" Control.code_body "f(x);" expected

  test_ast_list_rule "statement (only `;`)" Control.code_body ";" []

  let expected = [
    Apply'.fn "f" (V (Var("x", TypeClasses.any)))
    Return(Value.unit)
   ]
  test_ast_list_rule "block" Control.code_body "{ f(x); return; }" expected

  let expected = [ Block [ Block [ V (Var("a", TypeClasses.any)) ] ] ]
  test_ast_list_rule "block recursion" Control.code_body "{{{a;}}}" expected

  let expected = [If(V (Var("a", TypeClasses.any)), Block [V (Var("b", TypeClasses.any))], Value.unit)]
  test_ast_list_rule "if" Control.code_body "if (a) { b; }" expected

  let expected = [If(V (Var("x", TypeClasses.any)), Block [V (Var("y", TypeClasses.any))], Block expected)]
  test_ast_list_rule "if - else" Control.code_body "if (x) y; else if (a) { b; }" expected

  let expected =
    [While(V (Var("a", TypeClasses.any)), Block [V (Var("b", TypeClasses.any)); V (Var("c", TypeClasses.any))])]
  test_ast_list_rule "while" Control.code_body "while (a) { b; c; }" expected

  let expected_loop_cond =
    Apply'.fn2("<", TypeClasses.f_logic_infix) (V (Var("e", TypeClasses.any))) (V (Lit("4", Int)))
  let expected_loop_incr =
    let updated = Apply'.fn2("+", TypeClasses.f_arith_infix) (V (Var("e", TypeClasses.any))) (V (Lit("1", Int)))
    Apply'.fn2("-", TypeClasses.f_arith_infix) (Assign(V (Var("e", TypeClasses.any)), updated)) (V (Lit("1", Int)))
  let expected = [
    Block [
      Declare("e", Int); Assign(V (Var("e", Int)), V (Lit("0", Int)))
      While(expected_loop_cond, Block [expected_loop_incr])
     ]
   ]
  test_ast_list_rule "for" Control.code_body "for (int e = 0; e < 4; e++);" expected

  let expected = [
    Block [
      Assign(V (Var("e", TypeClasses.any)), V (Lit("0", Int)))
      While(V (Lit("1", Byte)), Block [Value.unit])
     ]
   ]
  test_ast_list_rule "for2" Control.code_body "for (e = 0; ;);" expected

test_parser_code()


let test_parser_global() =
  let input = "int f(int x, int* y) { return x; }"
  let expected =
    GlobalParse [
      Declare("f", DT.Function([Int; Ptr Int], Int))
      Assign(
        V (Var("f", DT.Function([Int; Ptr Int], Int))),
        Function(Int, [("x", Int); ("y", Ptr Int)], Block [Return(V (Var("x", TypeClasses.any)))])
       )
     ]
  test_ast_rule "function" (GlobalParse <-/ Control.declare_function) input expected
  
  test_rule "typedef" Typedef.typedef "typedef int a" (DeclAlias("a", Int))
  
  test_rule "typedef 2" Typedef.typedef "typedef int** a" (DeclAlias("a", Ptr (Ptr Int)))

  let expected = DeclStruct("A", [
    StructField("x", Int, 0, 4)
    StructField("y", TypeDef (Array(1, Int)), 32, 40)
   ])
  test_rule "struct" Typedef.declare_struct "struct A {int x; int y[10];}" expected

  let expected = DeclUnion("A", [
    StructField("x", Int, 0, 4)
    StructField("y", TypeDef (Array(1, Int)), 0, 40)
   ])
  test_rule "union" Typedef.declare_union "union A {int x; int y[10];}" expected

  let expected =
    GlobalParse [
      Declare("?", TypeDef (DeclAlias("X", Int)))
      Declare("x", TypeDef (Alias "X")); Assign(V (Var("x", TypeDef (Alias "X"))), V (Lit("1", Int)))
      Declare("anon", TypeDef (DeclStruct("_anon", [StructField("a", Int, 0, 4)])))
      Declare("?", TypeDef (DeclStruct("Y", [StructField("b", Int, 3, 4)])))
      Declare("y", TypeDef (Struct "Y"))
      Declare("?",
        TypeDef (DeclUnion("Y", [
          StructField("a", Int, 0, 4); StructField("b", Byte, 0, 1)
          StructField("c", TypeDef (Array(1, Int64)), 0, 320)
         ]))
       )
      Declare("z", TypeDef (Union "Y"))
      Declare("?", TypeDef (DeclUnion("ZZ", [])))
      Declare("?", TypeDef (DeclAlias("A", TypeDef (Union "ZZ"))))
      Declare("?", TypeDef (DeclAlias("B", TypeDef (Union "ZZ"))))
      Declare("?", TypeDef (DeclAlias("C", TypeDef (Union "ZZ"))))
     ]
  test_rule "typedef, struct, union" parse_global_scope """
    typedef int X; X x = 1;
    struct {int a;} anon;
    struct Y {int b:3;}; struct Y y;
    union Y {int a; char b; long c[40];}; union Y z;
    typedef union ZZ {} A, B, C;
    """ expected

  let expected =
    GlobalParse [
      Declare("x", Int64); Assign(V (Var("x", Int64)), V (Lit("1", Int)))
      Declare("f", DT.Function([], Void))
      Declare("main", Function2 Int)
      Assign(V (Var("main", Function2 Int)), Function(Int, [], Block []))
     ]
  test_ast_rule "global parse" parse_global_scope "long x = 1; ; void f(void); main() {}" expected

  let expected =
    GlobalParse [
      Declare("main", DT.Function2 Int)
      Assign(V (Var("main", DT.Function2 Int)), Function(Int, [], Block [Declare("y", Ptr Int)]))
     ]
  test_ast_rule "declare ptr, not multiply" parse_global_scope """
    main() {
      int *y;
    }
    """ expected

  // fails, datatype rule needs context about known user-defined type names
  // let expected_inner =
  //   Apply'.fn2("*", TypeClasses.f_arith_infix)
  //    (V (Var("x", TypeClasses.any))) (V (Var("y", TypeClasses.any)))
  // let expected =
  //   GlobalParse [
  //     Declare("main", DT.Function2 Int)
  //     Assign(V (Var("main", DT.Function2 Int)), Function(Int, [], Block [expected_inner]))
  //    ]
  // test_rule "multiply ptr, not declare ptr" parse_global_scope """
  //   main() {
  //     x *y;
  //   }
  //   """ expected

test_parser_global()