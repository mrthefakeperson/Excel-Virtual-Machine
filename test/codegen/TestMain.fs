module TestCodegen.TestMain
open Fuchu
open Parser.AST
open Codegen.PAsm
open Codegen.Interpreter
open Codegen.Main

let test_codegen_direct message ast expected_code =
  testCase message <|
    fun () -> Assert.Equal(message, expected_code, generate_from_ast ast)
   |> run
   |> ignore

let test_codegen_interpret message ast expected_value =
  testCase message <|
    fun () -> Assert.Equal(message, expected_value, eval_pasm (generate_from_ast ast))
   |> run
   |> ignore

let test_basics() =
  test_codegen_direct "single value" (Value(Lit("1", Datatype.Int))) [MovRC(R 0, Int 1)]

  test_codegen_direct "declare single value" (
    Block [
      Declare("a", Datatype.Char); Assign(Value(Var("a", t_any)), Value(Lit("'a'", Datatype.Char))); Value(Var("a", Datatype.Char))
     ]
   ) [MovRC(R 1, Byte(byte 'a')); MovRR(R 0, R 1)]
  test_codegen_direct "declare array (global)" (
    GlobalParse [
      Declare("a", Pointer(Datatype.Long, Some 16))
      Assign(Index(Value(Var("a", t_any)), Value(Lit("0", Datatype.Int))), Value(Lit("11L", Datatype.Long)))
      Assign(Index(Value(Var("a", t_any)), Value(Lit("0", Datatype.Int))), Value(Lit("22L", Datatype.Long)))
     ]
   ) [Br "main"; Label "a"; Data [|Int64 11L; Int64 22L|]]

  let expected = [
    Alloc 16; MovRR(R 1, R 0)
    MovRC(R 0, Int64 11L); Push (R 0); MovRR(R 0, R 1); MovMR(Indirect (R 0), SP); Pop (R 0)
    MovRC(R 0, Int64 22L); Push (R 0); MovRR(R 0, R 1); AddC(R 0, Int 1); MovMR(Indirect (R 0), SP); Pop (R 0)
   ]
  test_codegen_direct "declare array (local)" (
    Block [
      Declare("a", Pointer(Datatype.Long, Some 16))
      Assign(
        Apply(
          Value(Var("*prefix", t_any)),
          [Value(Var("a", t_any))]
         ),
        Value(Lit("11L", Datatype.Long))
       )
      Assign(Index(Value(Var("a", t_any)), Value(Lit("1", Datatype.Int))), Value(Lit("22L", Datatype.Long)))
     ]
   ) expected

  let expected = [
    MovRC(R 0, Int 1); AddC(R 0, Int64 0L)
    AddC(R 0, Int64 2L)
    MovRC(R 0, Byte 4uy); AddC(R 0, Int 0)
    AddC(R 0, Int 3)
   ]
  test_codegen_direct "implicit casting with arithmetic" (
    Block [
      Apply(Value(Var("+", t_any)), [Value(Lit("1", Datatype.Int)); Value(Lit("2", Datatype.Long))])
      Apply(Value(Var("+", t_any)), [Value(Lit("3", Datatype.Int)); Value(Lit(string (char 4), Datatype.Char))])
     ]
   ) expected

  let expected = [
    MovRC(R 0, Byte 97uy); Br0 "else_1"; MovRC(R 0, Int 1); Br "cont_2"; Label "else_1"; MovRC(R 0, Int 2); Label "cont_2"
   ]
  test_codegen_direct "if basic"
   (If(Value(Lit("'a'", Datatype.Char)), Value(Lit("1", Datatype.Int)), Value(Lit("2", Datatype.Int)))) expected

  let expected = [
    MovRC(R 0, Int 1); AddC(R 0, Byte 0uy); Br0 "else_1"
    MovRC(R 1, Int 7); Br "cont_2"
    Label "else_1";
      MovRR(R 0, R 1); AddC(R 0, Byte 0uy); Br0 "else_3"; MovRC(R 2, Int 77)
    Label "else_3"; Label "cont_2"
   ]
  test_codegen_direct "if nested 1" (
    If(Value(Lit("1", Datatype.Int)),
      Block [Declare("x", Datatype.Int); Assign(Value(Var("x", t_any)), Value(Lit("7", Datatype.Int)))],
      Block [
        Declare("y", Datatype.Int)
        If(Value(Var("y", t_any)),
          Block [Declare("z", Datatype.Int); Assign(Value(Var("z", t_any)), Value(Lit("77", Datatype.Int)))],
          Block []
         )
       ]
     )
   ) expected

  let expected = [  // x -> R1, y -> R2
    PushC (Int 1); MovRR(R 0, R 2); Cmp(R 0, SP); SubC(SP, Int 1);  Br0 "else_1";
      MovRR(R 0, R 1); AddC(R 0, Byte 0uy); BrT "cont_4";
        MovRR(R 2, R 1);
      Label "cont_4"
    Br "cont_2"; Label "else_1";
      MovRR(R 1, R 2)
    Label "cont_2"
   ]
  test_codegen_direct "if nested 2" (
    Block [
      Declare("x", Datatype.Int); Declare("y", Datatype.Int)
      If(Apply(Value(Var("==", t_any)), [Value(Lit("1", Datatype.Int)); Value(Var("y", t_any))]),
        If(Value(Var("x", t_any)),
          Block [],
          Block [Assign(Value(Var("y", t_any)), Value(Var("x", t_any)))]
         ),
        Block [Assign(Value(Var("x", t_any)), Value(Var("y", t_any)))]
       )
     ]
   ) expected

  let expected = [
    Br "enter_loop_1"; Label "loop_2"
    MovRC(R 0, Int 4); DivModC(R 0, Int 3); MovRR(R 0, RX)
    Label "enter_loop_1"; MovRC(R 0, Byte 1uy); BrT "loop_2"
   ]
  test_codegen_direct "while basic" (
    While(Value(Lit(string (char 1), Datatype.Char)),
      Block [Apply(Value(Var("%", t_any)), [Value(Lit("4", Datatype.Int)); Value(Lit("3", Datatype.Int))])]
     )
   ) expected

  let expected = [
    Br "enter_loop_1"; Label "loop_2";
      Label "loop_4"; MovRC(R 0, Byte 77uy); BrT "loop_4";
      Br "enter_loop_5"; Label "loop_6";
        Br "enter_loop_7"; Label "loop_8";
          MovRR(R 0, R 1);
        Label "enter_loop_7"; MovRC(R 0, Byte 79uy); BrT "loop_8";
      Label "enter_loop_5"; MovRC(R 0, Byte 78uy); BrT "loop_6"
    Label "enter_loop_1"; MovRC(R 0, Byte 1uy); BrT "loop_2"
   ]
  test_codegen_direct "while nested" (
    While(Value(Lit(string (char 1), Datatype.Char)),
      Block [
        Declare("x", Long)
        While(Value(Lit(string (char 77), Datatype.Char)), Block [])
        While(Value(Lit(string (char 78), Datatype.Char)), Block [While(Value(Lit(string (char 79), Datatype.Char)), Value(Var("x", t_any)))])
       ]
     )
   ) expected

  let expected = [
    Br "main"; Label "a"; Data [|Int 1|]; Label "main"; MovRC(R 0, Int64 1L); AddC(R 0, Int 0); MovMR(Lbl "a", R 0); Ret
   ]
  test_codegen_direct "assign global" (
    GlobalParse [
      Declare("a", Datatype.Int); Assign(Value(Var("a", t_any)), Value(Lit("1", Datatype.Int)))
      Declare("main", Datatype.Function([], Datatype.Int))
      Assign(Value(Var("main", t_any)),
        Function([],
          Block [Assign(Value(Var("a", t_any)), Value(Lit("1", Datatype.Long)))]
         )
       )
     ]
   ) expected

  let expected = [
    Br "main"; Label "f"; AddC(SP, Int 1)  // x -> -2, y -> -1, z -> 1
    MovRR(R 1, R -2); MovRR(R 0, R -1)
    Ret; Ret  // TODO: consider inserting Return nodes on every path to avoid redundant returns at the end of functions
   ]
  test_codegen_direct "function" (
    GlobalParse [
      Declare("f", Datatype.Function([Datatype.Int; Datatype.Float], Datatype.Float))
      Assign(Value(Var("f", t_any)),
        Function(["x", Datatype.Int; "y", Datatype.Float],
          Block [
            Declare("z", Datatype.Int); Assign(Value(Var("z", t_any)), Value(Var("x", t_any)))
            Return (Value(Var("y", t_any)))
           ]
         )
       )
     ]
   ) expected

  let expected = [
    Br "main"; Label "f"; AddC (SP,Int 1)
    PushRealRs; Push (R 1); MovRR (BP,SP); Call "f"; PopRealRs
    MovRR (R 0,R -1); AddC (R 0,Float 0.0f)
    Ret; Ret
   ]
  test_codegen_direct "recursion" (
    GlobalParse [
      Declare("f", Datatype.Function([Datatype.Int], Datatype.Float))
      Assign(Value(Var("f", t_any)),
        Function(["x", Datatype.Int],
          Block [
            Declare("z", Datatype.Int)
            Apply(Value(Var("f", t_any)), [Value(Var("z", t_any))])
            Return (Value(Var("x", t_any)))
           ]
         )
       )
     ]
   ) expected

let run_all() =
  test_basics()