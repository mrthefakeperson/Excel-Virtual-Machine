#if !TEST
#load "../.fake/build.fsx/intellisense.fsx"
#endif
#r "../build/Utils.dll"
#r "../build/CompilerDatatypes.dll"
#r "../build/Parser.dll"
#r "../build/Transformers.dll"

open Fuchu
open Utils
open CompilerDatatypes.Semantics.InterpreterValue
open CompilerDatatypes.DT
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SemanticAST
open CompilerDatatypes.Semantics.RegisterAlloc
open Parser.Main
open Transformers

let test_transform message f input expected_output =
  testCase message (fun () ->
    let result = f (parse_string_to_ast input)
    Assert.Equal(message, result, expected_output)
   )
   |> run
   |> ignore

let test_typecheck() =
  let mutable input = ""
  let mutable expected = V (Lit Boxed.Void)
  let test msg =
    test_transform msg TypeCheck.resolve_types input expected

  let parse_semantics = parse_string_to_ast >> from_syntax_ast
  
  input <- "int a = 0L;"
  expected <- parse_semantics "int a = (int)0L;"
  test "simple cast"

  input <- "int main(void) { return '\0'; }"
  expected <- parse_semantics "int main(void) { return (int)'\0'; }"
  test "return cast"

  input <- "void f(int x) { x; f(1); long a; a; }"
  expected <- GlobalParse [
    Declare("f", DT.Function([Int], Void))
    Assign(V (Var("f", Global (DT.Function([Int], Void)))),
      Function(Void, [("x", Int)], Block [
        V (Var("x", Local(-1, Int)))
        Apply(V (Var("f", Global (DT.Function([Int], Void)))), [V (Lit (Boxed.Int 1))])
        Declare("a", Int64); V (Var("a", Local(1, Int64)))
       ])
     )
   ]
  test "name resolution"

  input <- "int main(void) { int a; a + 1L; 1L + a; a + 'x'; 'x' + a; }"
  expected <- GlobalParse [
    Declare("main", DT.Function([], Int))
    Assign(V (Var("main", Global (DT.Function([], Int)))),
      Function(Int, [], Block [
        yield Declare("a", Int)
        let plus dt a b = Apply(V (Var("+", Global (DT.Function([dt; dt], dt)))), [a; b])
        let a = V (Var("a", Local(1, Int)))
        yield plus Int64 (BuiltinASTs.cast Int64 a) (V (Lit (Boxed.Int64 1L)))
        yield plus Int64 (V (Lit (Boxed.Int64 1L))) (BuiltinASTs.cast Int64 a)
        yield plus Int a (BuiltinASTs.cast Int (V (Lit (Boxed.Byte 120uy))))
        yield plus Int (BuiltinASTs.cast Int (V (Lit (Boxed.Byte 120uy)))) a
       ])
     )
   ]
  test "arithmetic casts"

  input <- "void f(int w, char x, long y, int *z); int main(void) { f(0, 1, 2, 3); }"
  expected <-
    let GlobalParse defn | Strict defn = parse_semantics "void f(int w, char x, long y, int *z);"
    GlobalParse [
      yield! defn
      yield Declare ("main", DT.Function([], Int))
      yield Assign(V (Var("main", Global (DT.Function([], Int)))),
        Function(Int, [], Block [
          Apply(V (Var("f", Global (DT.Function([Int; Byte; Int64; Ptr Int], Void)))), [
            V (Lit (Boxed.Int 0))
            BuiltinASTs.cast Byte (V (Lit (Boxed.Int 1)))
            BuiltinASTs.cast Int64 (V (Lit (Boxed.Int 2)))
            BuiltinASTs.cast (Ptr Int) (V (Lit (Boxed.Int 3)))
           ])
         ])
       )
     ]
  test "function signature casts"

  input <- "void f(void) { 1 ? 2 : 3L; 'a' ? 2L : 3; }"
  expected <- parse_semantics "void f(void) { (char)1 ? (long)2 : 3L; 'a' ? 2L : (long)3; }"
  test "if casts for branches and conditional"

  input <- "void f(void) { while (1); }"
  expected <- parse_semantics "void f(void) { while ((char)1); }"
  test "while conditional cast"

  input <- "typedef char K; K x;"
  expected <- GlobalParse [Declare("?", Byte); Declare("x", Byte)]
  test "resolve simple typedef"

  input <- "typedef char** K; K x = 1;"
  expected <- GlobalParse [
    Declare("?", Ptr (Ptr Byte))
    Declare("x", Ptr (Ptr Byte))
    Assign(V (Var("x", Global (Ptr (Ptr Byte)))),
      BuiltinASTs.cast (Ptr (Ptr Byte)) (V (Lit (Boxed.Int 1)))
     )
   ]
  test "resolve typedef and cast"

  input <- "struct X {}; union Y {}; typedef union Y Z; void f(void) { struct X x; union Y y; Z z; }"
  expected <- GlobalParse [
    let x = TypeDef (DeclStruct("X", [])) in yield Declare("?", x)
    let y = TypeDef (DeclUnion("Y", [])) in yield Declare("?", y)
    yield Declare("?", y)
    yield Declare("f", DT.Function([], Void))
    yield Assign(V (Var("f", Global (DT.Function([], Void)))), Function(Void, [], Block [
      Declare("x", x); Declare("y", y); Declare("z", y)
     ]))
   ]
  test "resolve struct and union"

  input <- "typedef struct {} X, X2; typedef struct {int a;} Y; X x; X2 x2; Y y;"
  expected <- GlobalParse [
    let _1 = TypeDef (DeclStruct("_anon", []))
    yield Declare("?", _1)
    yield Declare("?", _1)
    yield Declare("?", _1)
    let _2 = TypeDef (DeclStruct("_anon", [StructField("a", Int, 0, 4)]))
    yield Declare("?", _2)
    yield Declare("?", _2)
    yield Declare("x", _1)
    yield Declare("x2", _1)
    yield Declare("y", _2)
   ]
  test "resolve typedef struct"

  input <- "struct X {char a;} x; int f(void) { return x.a; }"
  expected <- GlobalParse [
    let x = TypeDef (DeclStruct("X", [StructField("a", Byte, 0, 1)]))
    yield Declare("x", x)
    yield Declare("f", DT.Function([], Int))
    let access =
      Apply(V (Var("*prefix", Global (TypeClasses.f_unary (Ptr Byte) Byte))), [
        Apply(V (Var("+", Global (TypeClasses.f_binary (Ptr Byte) (Ptr Byte) (Ptr Byte)))), [
          BuiltinASTs.cast (Ptr Byte) (V (Var("x", Global x)))
          V (Lit (Boxed.Ptr(0, Byte)))
         ])
       ])
    yield Assign(V (Var("f", Global (DT.Function([], Int)))), Function(Int, [], Block [
      Return (BuiltinASTs.cast Int access)
     ]))
   ]
  test "resolve struct field"

  input <- "struct X {long a:8;} x; int f(void) { return x.a; }"
  expected <- GlobalParse [
    let x = TypeDef (DeclStruct("X", [StructField("a", Int64, 8, 8)]))
    yield Declare("x", x)
    yield Declare("f", DT.Function([], Int))
    let access =
      Apply(V (Var("*prefix", Global (TypeClasses.f_unary (Ptr Byte) Byte))), [
        Apply(V (Var("+", Global (TypeClasses.f_binary (Ptr Byte) (Ptr Byte) (Ptr Byte)))), [
          BuiltinASTs.cast (Ptr Byte) (V (Var("x", Global x)))
          V (Lit (Boxed.Ptr(1, Byte)))
         ])
       ])
       |> BuiltinASTs.cast Int64
    yield Assign(V (Var("f", Global (DT.Function([], Int)))), Function(Int, [], Block [
      Return (BuiltinASTs.cast Int access)
     ]))
   ]
  test "resolve struct field"

test_typecheck()

open CompilerDatatypes.Token
open SyntaxAST

let test_simplify() =
  let mutable input = ""
  let mutable expected = Value.unit
  let test msg transform = test_transform msg transform input expected

  input <- "int main(void) { sizeof(int);; a = sizeof(char); return sizeof(long); }"
  expected <- parse_string_to_ast "int main(void) { 4;; a = 1; return 8; }"
  test "replace sizeof" (Hooks.apply_mapping_hook SimplifyAST.eval_sizeof_hook)

  input <- "char *a = \"xy\"; char *b = \"\";"
  test_transform "find strings" (List.sort << SimplifyAST.find_strings) input [("$", ""); ("$78_79", "xy")]
  expected <- GlobalParse [
    Declare("$", Ptr Byte)
    Assign(V (Var("$", Ptr Byte)), BuiltinASTs.stack_alloc Byte [V (Lit("1", Int))])
    Assign(BuiltinASTs.index (V (Var("$", TypeClasses.any))) (V (Lit("0", Int))), V (Lit("0", Byte)))
    Declare("$78_79", Ptr Byte)
    Assign(V (Var("$78_79", Ptr Byte)), BuiltinASTs.stack_alloc Byte [V (Lit("3", Int))])
    Assign(BuiltinASTs.index (V (Var("$78_79", TypeClasses.any))) (V (Lit("0", Int))), V (Lit("'x'", Byte)))
    Assign(BuiltinASTs.index (V (Var("$78_79", TypeClasses.any))) (V (Lit("1", Int))), V (Lit("'y'", Byte)))
    Assign(BuiltinASTs.index (V (Var("$78_79", TypeClasses.any))) (V (Lit("2", Int))), V (Lit("0", Byte)))
    Declare("a", Ptr Byte); Assign(V (Var("a", Ptr Byte)), V (Var("$78_79", TypeClasses.any)))
    Declare("b", Ptr Byte); Assign(V (Var("b", Ptr Byte)), V (Var("$", TypeClasses.any)))
   ]
  test "extract strings" (Hooks.apply_mapping_hook SimplifyAST.extract_strings_to_global_hook)

  input <- "char *a = \"\", *b = \"\";"
  expected <- GlobalParse [
    Declare("$", Ptr Byte)
    Assign(V (Var("$", Ptr Byte)), BuiltinASTs.stack_alloc Byte [V (Lit("1", Int))])
    Assign(BuiltinASTs.index (V (Var("$", TypeClasses.any))) (V (Lit("0", Int))), V (Lit("0", Byte)))
    Declare("a", Ptr Byte); Assign(V (Var("a", Ptr Byte)), V (Var("$", TypeClasses.any)))
    Declare("b", Ptr Byte); Assign(V (Var("b", Ptr Byte)), V (Var("$", TypeClasses.any)))
   ]
  test "extract strings no duplicates"
   (Hooks.apply_mapping_hook SimplifyAST.extract_strings_to_global_hook)

  input <- "int f(int x); int f(int x) {}"
  expected <- parse_string_to_ast "int f(int x) {}"
  test "prototyping" (Hooks.apply_mapping_hook SimplifyAST.prototype_hook)

  input <- "int f(void) { a && b; c || d; e != f; g <= h; i >= j; !k; }"
  expected <- parse_string_to_ast
    "int f(void) { a ? b : '\\0'; c ? '\\1' : d; e == f == '\\0'; g > h == '\\0'; i < j == '\\0'; k == '\\0'; }"
  test "convert logic operators to branching exprs"
   (Hooks.apply_mapping_hook SimplifyAST.convert_logic_hook)

  input <- "int k = a->b;"
  expected <- parse_string_to_ast "int k = (*a).b;"
  test "convert arrows" (Hooks.apply_mapping_hook SimplifyAST.convert_arrow_hook)

  // TODO: test arrays

test_simplify()