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
  let expected = state |> State.write_register (Register.R 0) (Int 77)
  test_single "add" (Arith(Add, 4, RX, RC.C (Int 70))) state expected

  let state = State.initialize [] |> State.write_register RX (Int 7)
  let expected =
    state
     |> State.write_register (Register.R 0) (Int 2)
     |> State.write_register RX (Int 1)
  test_single "divmod int" (Arith(DivMod, 4, RX, RC.C (Int 3))) state expected

  let state = State.initialize [] |> State.write_register RX (Double 3.3)
  let expected =
    state
     |> State.write_register (Register.R 0) (Double (3.3 / 2.2))
     |> State.write_register RX (Double (3.3 % 2.2))
  test_single "divmod double" (Arith(DivMod, 8, RX, RC.C (Double 2.2))) state expected

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

let test_full_pipeline message code verifier =
  let pipeline = Lexer.Main.tokenize_text >> Parser.Main.parse_tokens_to_ast >> Codegen.Main.generate_from_ast >> convert_from_flat
  testCase message <| fun () -> Assert.Equal(message, true, verifier (pipeline code))
   |> run |> ignore

let e2e() =
  ()

let run_all() =
  unit_tests()
  asm_samples()
  e2e()