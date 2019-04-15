module TestCodegen.TestMain
open Fuchu
open Parser.Datatype
open Parser.AST
open Parser.Main
open Codegen.PAsm
open Codegen.PAsm.Flat
open Codegen.Main

let test_codegen_direct message ast expected_code =
  testCase message <|
    fun () -> Assert.Equal(message, expected_code, generate_from_ast ast)
   |> run
   |> ignore

let test_basics() =
  test_codegen_direct "single value" (Value(Lit("1", DT.Int))) [MovRC(R 0, Int 1)]
  
  let expr = parse_string_to_ast_with code_body "{ char a = 'a'; a; }"
  test_codegen_direct "declare single value" expr [MovRC(R 1, Byte(byte 'a'))]

  let expr = parse_string_to_ast_with parse_global_scope "long a[2] = {11L, 22L};"
  test_codegen_direct "declare array (global)" expr [Br "main"; Label "a"; Data [|Int64 11L; Int64 22L|]]

  let expr = parse_string_to_ast_with code_body "{ long a[2]; *a = 11L; a[1] = 22L; }"  // TODO: update all stack changes to Ptr types
  let expected = [
    MovRC(R 0, Int 2); MovRR(RX, SP); Add(4, SP, R 0); MovRR(R 1, RX)
    MovRC(R 0, Int64 11L); Push (R 0); MovRR(R 0, R 1); Pop RX; MovMR(Indirect (R 0), RX)
    MovRC(R 0, Int64 22L); Push (R 0);
    Push (R 1); MovRC(R 0, Int 1); Cast(DT.Ptr DT.Int64, R 0); MulC(4, R 0, Ptr(8, DT.Int64))
    Pop RX; Add(4, RX, R 0); Pop RX; MovMR(Indirect (R 0), RX)
   ]
  test_codegen_direct "declare array (local)" expr expected

  let expr =
    Block [
      Apply(Value(Var("+", TypeClasses.any)), [Value(Lit("1", DT.Int)); Value(Lit("2", DT.Int64))])
      Apply(Value(Var("+", TypeClasses.any)), [Value(Lit("3", DT.Int)); Value(Lit(sprintf "'%c'" (char 4), DT.Byte))])
     ]
  let expected = [
    MovRC(R 0, Int 1); Cast(DT.Int64, R 0)
    AddC(8, R 0, Int64 2L)
    MovRC(R 0, Byte 4uy); Cast(DT.Int, R 0)
    AddC(4, R 0, Int 3)
   ]
  test_codegen_direct "implicit casting with arithmetic" expr expected

  let expr = If(Value(Lit("'a'", DT.Byte)), Value(Lit("1", DT.Int)), Value(Lit("2", DT.Int)))
  let expected = [
    MovRC(R 0, Byte 97uy); CmpC(R 0, Byte 0uy); Br0 "else_1"; MovRC(R 0, Int 1); Br "cont_2"; Label "else_1"; MovRC(R 0, Int 2); Label "cont_2"
   ]
  test_codegen_direct "if basic" expr expected

  let expr = parse_string_to_ast_with _if "if (1) int x = 7; else { int y; if (y) int z = 77; }"
  let expected = [
    MovRC(R 0, Int 1); Cast(DT.Byte, R 0); CmpC(R 0, Byte 0uy); Br0 "else_1"
    MovRC(R 1, Int 7); Br "cont_2"
    Label "else_1";
      MovRR(R 0, R 1); Cast(DT.Byte, R 0); CmpC(R 0, Byte 0uy); Br0 "else_3"; MovRC(R 2, Int 77)
    Label "else_3"; Label "cont_2"
   ]
  test_codegen_direct "if nested 1" expr expected

  let expr = parse_string_to_ast_with code_body "{ int x, y; if (1 == y) if (x) ; else y = x; else x = y; }"
  let expected = [  // x -> R1, y -> R2
    PushC (Int 1); MovRR(R 0, R 2); Pop RX; Cmp(RX, R 0); MovRR(R 0, PSR_EQ); CmpC(R 0, Byte 0uy); Br0 "else_1";
      MovRR(R 0, R 1); Cast(DT.Byte, R 0); CmpC(R 0, Byte 0uy); BrT "cont_4";
        MovRR(R 2, R 1);
      Label "cont_4"
    Br "cont_2"; Label "else_1";
      MovRR(R 1, R 2)
    Label "cont_2"
   ]
  test_codegen_direct "if nested 2" expr expected

  let expr = parse_string_to_ast_with _while "while ('a') { 4 % 3; }"
  let expected = [
    Br "enter_loop_1"; Label "loop_2"
    MovRC(R 0, Int 4); DivModC(4, R 0, Int 3)  // MovRR(R 0, RX) (optimized out)
    Label "enter_loop_1"; MovRC(R 0, Byte 97uy); CmpC(R 0, Byte 0uy); BrT "loop_2"
   ]
  test_codegen_direct "while basic" expr expected

  let expr = parse_string_to_ast_with _while "while ('L') { long x; while ('M'); while ('N') { while ('O') x; } }"
  let expected = [
    Br "enter_loop_1"; Label "loop_2";
      Label "loop_4"; MovRC(R 0, Byte 77uy); CmpC(R 0, Byte 0uy); BrT "loop_4";
      Br "enter_loop_5"; Label "loop_6";
        Br "enter_loop_7"; Label "loop_8";
          MovRR(R 0, R 1);
        Label "enter_loop_7"; MovRC(R 0, Byte 79uy); CmpC(R 0, Byte 0uy); BrT "loop_8";
      Label "enter_loop_5"; MovRC(R 0, Byte 78uy); CmpC(R 0, Byte 0uy); BrT "loop_6"
    Label "enter_loop_1"; MovRC(R 0, Byte 76uy); CmpC(R 0, Byte 0uy); BrT "loop_2"
   ]
  test_codegen_direct "while nested" expr expected

  let expr = parse_string_to_ast_with parse_global_scope "int a = 1; int main(void) { a = 1L; }"
  let expected = [
    Br "main"; Label "a"; Data [|Int 1|];
    Label "main"; MovRC(R 0, Int64 1L); Cast(DT.Int, R 0); MovMR(Lbl("a", DT.Int), R 0); Ret
   ]
  test_codegen_direct "assign global" expr expected

  let expr = parse_string_to_ast_with parse_global_scope "float f(int x, float y) { int z = x; return y; }"
  let expected = [
    Br "main"; Label "f"; AddC(4, SP, Ptr(1, DT.Byte))  // x -> -2, y -> -1, z -> 1
    MovRR(R 1, R -2); MovRR(R 0, R -1)
    SubC(4, SP, Ptr(1, DT.Byte)); Ret; SubC(4, SP, Ptr(1, DT.Byte)); Ret  // TODO: consider inserting Return nodes on every path to avoid redundant returns at the end of functions
   ]
  test_codegen_direct "function" expr expected

  let expr = parse_string_to_ast_with parse_global_scope "float f(int x) { int z; f(z); return x; }"
  let expected = [
    Br "main"; Label "f"; AddC(4, SP, Ptr(1, DT.Byte))
    PushRealRs; Push (R 1); MovRR(BP, SP); Call "f"; PopRealRs
    MovRR(R 0, R -1); Cast(DT.Float, R 0)
    SubC(4, SP, Ptr(1, DT.Byte)); Ret; SubC(4, SP, Ptr(1, DT.Byte)); Ret
   ]
  test_codegen_direct "recursion" expr expected

  let expr = parse_string_to_ast_with parse_global_scope "float f(int x) { int z; return f(z); }"
  let expected = [
    Br "main"; Label "f"; AddC(4, SP, Ptr(1, DT.Byte))
    //PushRealRs; Push (R 1); MovRR (BP,SP); Call "f"; PopRealRs; Pop SP; Ret
    Push (R 1); SubC(4, SP, Ptr(1, DT.Byte)); ShiftStackDown(0, 1); MovRR(BP, SP); Br "f"
    SubC(4, SP, Ptr(1, DT.Byte)); Ret
   ]
  // TODO: adding Return nodes as indicated above will also apply tail recursion to calls at the end of blocks, even if no explicit Return is initially specified
  test_codegen_direct "tail recursion" expr expected

let run_all() =
  test_basics()