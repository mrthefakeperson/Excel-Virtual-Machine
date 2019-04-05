module TestCodewriters.TestInterpreter
open Fuchu
open Parser.Datatype
open Codegen.PAsm
open Codegen.PAsm.Simple
open Codewriters.Interpreter

let test_single message instr state expected_state =
  testCase message <| fun () -> Assert.Equal(message, expected_state, eval' instr (State.copy state))
   |> run |> ignore

let unit_tests() =
  let state =
    { State.initialize [] with mem = Array.create 5 Void }
     |> State.write_register SP (Ptr(2, DT.Byte))
  let expected =
    { state with mem = [|Void; Void; Void; Int 7; Void|] }
     |> State.write_register SP (Ptr(3, DT.Byte))
  test_single "push" (Push (RC.C (Int 7))) state expected

  let state =
    { State.initialize [] with mem = [|Void; Int 7; Void|] }
     |> State.write_register SP (Ptr(1, DT.Byte))
  let expected =
    state |> State.write_register SP (Ptr(0, DT.Byte)) |> State.write_register RX (Int 7)
  test_single "pop" (Pop RX) state expected

  let state =
    { State.initialize [] with mem = [|Void; Void; Void; Void; Int 1; Int 2; Int 3|] }
     |> State.write_register SP (Ptr(3, DT.Byte))
  let expected =
    { state with mem = [|Void; Void; Int 1; Int 2; Int 3; Int 2; Int 3|] }
  test_single "shift stack down" (ShiftStackDown(2, 3)) state expected

  let state = State.initialize [] |> State.write_register RX (Int 7)
  let expected = state |> State.write_register (Register.R 0) (Int 7)
  test_single "move between registers" (Mov(RM.R (Register.R 0), RMC.R RX)) state expected

  let state = { State.initialize [] with mem = Array.create 3 Void }
  let expected = { state with mem = [|Void; Int 7; Void|] }
  test_single "move into memory" (Mov(RM.M 1, RMC.C (Int 7))) state expected

  let state =
    { State.initialize [] with mem = [|Void; Int 7; Void|] }
     |> State.write_register RX (Ptr(0, DT.Void))
  let expected = { state with mem = [|Int 7; Int 7; Void|] }
  test_single "move memory contents into addr stored in register" (Mov(RM.I RX, RMC.M 1)) state expected

  let state = State.initialize [] |> State.write_register RX (Int 9)
  let expected =
    state
     |> State.write_register PSR_EQ (Byte 0uy)
     |> State.write_register PSR_LT (Byte 0uy)
     |> State.write_register PSR_GT (Byte 1uy)
  test_single "compare gt" (Cmp(RX, RC.C (Int 7))) state expected

  let state = State.initialize [] |> State.write_register RX (Int 7)
  let expected =
    state
     |> State.write_register PSR_EQ (Byte 1uy)
     |> State.write_register PSR_LT (Byte 0uy)
     |> State.write_register PSR_GT (Byte 0uy)
  test_single "compare eq" (Cmp(RX, RC.C (Int 7))) state expected

  let state = State.initialize [] |> State.write_register RX (Int 5)
  let expected =
    state
     |> State.write_register PSR_EQ (Byte 0uy)
     |> State.write_register PSR_LT (Byte 1uy)
     |> State.write_register PSR_GT (Byte 0uy)
  test_single "compare lt" (Cmp(RX, RC.C (Int 7))) state expected

  let state = State.initialize []
  let expected = { state with pc = 7 - 1 }
  test_single "branch" (Br(B, 7)) state expected

  let state = State.initialize [] |> State.write_register PSR_LT (Byte 1uy)
  let expected = { state with pc = 7 - 1 }
  test_single "branch lt success" (Br(LT, 7)) state expected

  let state = State.initialize [] |> State.write_register PSR_GT (Byte 0uy)
  let expected = state
  test_single "branch gt failure" (Br(GT, 7)) state expected

  let state =
    { State.initialize [] with mem = [|Void; Void; Void|]; pc = 7 }
     |> State.write_register SP (Ptr(0, DT.Byte))
  let expected =
    { state with mem = [|Void; Ptr(7, DT.Void); Void|]; pc = 77 - 1 }
     |> State.write_register SP (Ptr(1, DT.Byte))
  test_single "call" (Call 77) state expected
  
  let state = expected |> State.write_register SP (Ptr(1, DT.Byte))
  let expected = { state with pc = 7 } |> State.write_register SP (Ptr(0, DT.Byte))
  test_single "ret" Ret state expected

  let state = State.initialize [] |> State.write_register RX (Int 7)
  let expected = state |> State.write_register (Register.RX) (Int 77)
  test_single "add" (Arith(Add, 4, RX, RC.C (Int 70))) state expected

  let state = State.initialize [] |> State.write_register (Register.R 0) (Int 7)
  let expected =
    state
     |> State.write_register (Register.R 0) (Int 2)
     |> State.write_register RX (Int 1)
  test_single "divmod int" (Arith(DivMod, 4, Register.R 0, RC.C (Int 3))) state expected

  let state = State.initialize [] |> State.write_register (Register.R 0) (Double 3.3)
  let expected =
    state
     |> State.write_register (Register.R 0) (Double (3.3 / 2.2))
     |> State.write_register RX (Double (3.3 % 2.2))
  test_single "divmod double" (Arith(DivMod, 8, Register.R 0, RC.C (Double 2.2))) state expected

let test_instrs message instrs verifier =
  testCase message <| fun () -> Assert.Equal(message, true, verifier (eval instrs))
   |> run |> ignore

let asm_samples() =
  test_instrs "simple" [Ret] (fun _ -> true)

  test_instrs "set values" [Mov(RM.R RX, RMC.C (Int 7)); Mov(RM.R (Register.R 0), RMC.R RX); Ret]
   (fun result -> result.regs.[RX] = Int 7 && result.regs.[Register.R 0] = Int 7)

  test_instrs "stack manipulation" [Push (RC.C (Int 7)); Mov(RM.R RX, RMC.I SP); Push (RC.R RX); Pop RX; Pop (Register.R 0); Ret]
   (fun result ->
      result.regs.[RX] = Int 7 && result.regs.[Register.R 0] = Int 7
       && result.mem.[STACK_START + 1..STACK_START + 4] = [|Int 7; Int 7; Void; Void|])

let test_ast message code verifier =
  let pipeline = Codegen.Main.generate_from_ast >> convert_from_flat >> eval
  testCase message <| fun () -> Assert.Equal(message, true, verifier (pipeline code))
   |> run |> ignore

let from_ast() =
  ()

let test_full_pipeline message code expected_ret =
  let pipeline =
    Lexer.Main.tokenize_text
     >> Parser.Main.parse_tokens_to_ast
     >> Codegen.Main.generate_from_ast >> convert_from_flat
     >> trace
     >> eval
  testCase message <| fun () ->
        let result = pipeline code
        Assert.Equal(message, EXTERN_CALL_ADDR, result.pc)
        Assert.Equal(message, expected_ret, result.regs.[Register.R 0])
   |> run |> ignore

let e2e() =
  test_full_pipeline "basic" "int main() { return 7; }" (Int 7)

  test_full_pipeline "local variables 1" "int main() { char x = 'a'; return x + 1; }" (Int 98)

  test_full_pipeline "local variables 2" "int main() { int x = 2, y = -3; return -x + y * x; }" (Int -8)

  // "int main() { int x[3] = { 1, 2, 3 }; return 2 * *x + x[2]; }" is bugged because the stack space is not being properly allocated
  //test_full_pipeline "local variables 3" "int main() { int x[3] = { 1, 2, 3 }, x2, x3, x4 = 7777777; return 2 * *x + x[2]; }" (Int 5)

  test_full_pipeline "local variables 4" "int main() { int x = 4, *y = &x; return *y; }" (Int 4)

  test_full_pipeline "if 1" "int main() { int a = 1, b = 2; if (a > 2) return -1; else if (b >= 2) b++; return b; }" (Int 3)

  test_full_pipeline "while 1" "int main() { int x = 1; while (1) if (x++ == 3) return x; }" (Int 4)

  test_full_pipeline "for 1" "int main() { int y = 1; for (int x = 0; x < 5; x++) y *= 2; return y; }" (Int 32)

  test_full_pipeline "global variables 1" "int x = 1, y = 7; int main() { int x = 7; return x + y; }" (Int 14)

  // bugged: (uninitialized?) global arrays don't work, types don't match
  //test_full_pipeline "global variables 2" "int x[50]; int main() { x[2] = 3; return x[2]; }" (Int 3)

  test_full_pipeline "function call 1" "int f(int a, int b) { return a + b; } int main(void) { return f(7, 70); }" (Int 77)

  // bugged?
  //test_full_pipeline "function call 2 (recursion)" "int f(int a) { if (a == 2) return a; else return f(a - 1) + 2; } int main() { return f(4); }"
  // (Int 6)

let run_all() =
  unit_tests()
  asm_samples()
  from_ast()
  e2e()