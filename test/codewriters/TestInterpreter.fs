module TestCodewriters.TestInterpreter
//open Fuchu
//open Parser.AST
//open Codegen.PAsm
//open Codegen.PAsm.Simple
//open Codewriters.Interpreter

//let test_single message instr state expected_state =
//  let state_cp = { state with mem = Array.copy state.mem }
//  testCase message <| fun () -> Assert.Equal(message, expected_state, eval' instr state_cp)
//   |> run |> ignore

//let unit_tests() =
//  let state =
//    { State.initialize [] with mem = Array.create 5 Void }
//     |> State.write_register SP (Ptr(3, Datatype.Void))
//  let expected =
//    { state with mem = [|Void; Void; Void; Int 7; Void|] }
//     |> State.write_register SP (Ptr(4, Datatype.Void))
//  test_single "push" (Push (RC.C (Int 7))) state expected

//let test_instrs message instrs verifier =
//  testCase message <| fun () -> Assert.Equal(message, true, verifier (eval instrs))
//   |> run |> ignore



//let run_all() =
//  unit_tests()