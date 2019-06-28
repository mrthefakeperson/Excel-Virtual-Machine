module Codewriters.Nasm
open Utils
open CompilerDatatypes.Semantics.InterpreterValue
open CompilerDatatypes.PseudoASM
open CompilerDatatypes.PseudoASM.Flat

let data_to_string: Boxed -> string = function
  |Int x -> sprintf ".int %i" x
  |Int64 x -> sprintf ".long %i" x
  |Byte x -> sprintf ".byte %i" x
  |Float x -> sprintf ".float %f" x
  |Double x -> sprintf ".double %f" x
  |Void -> failwith "cannot write void"
  |Ptr(x, _) -> sprintf ".int %i" x

let reg_to_string: Register -> string = function
  |R 0 -> "eax"
  |RX -> "ebx"
  |R 2 -> "ecx"
  |R 3 -> "edx"
  |R n -> failwithf "register %i does not exist" n
  |BP -> "ebp"
  |SP -> "esp"
  |PSR_EQ | PSR_GT | PSR_LT -> failwith "cannot access PSR directly"

let write_data: Asm seq -> string seq =
  let rec write_data = function
    |[] -> Seq.empty
    |Label var::Data data::rest -> seq {
      yield var + ":"
      yield! Seq.map data_to_string data
      yield! write_data rest
     }
    |_::rest -> write_data rest
  List.ofSeq >> write_data

let extern_calls (instrs: Asm seq) : string seq =
  let labels = Seq.choose (function Label lbl -> Some lbl | _ -> None) instrs
  let calls = Seq.choose (function Call lbl -> Some lbl | _ -> None) instrs
  let externs = Set.difference (Set calls) (Set labels)
  Seq.map ((+) "extern ") externs

let write_code: Asm seq -> string seq =
  let rec write_code = function
    |[] -> Seq.empty
    |Label _::Data _::rest -> write_code rest
    |instr::rest -> seq {
      match instr with
      |Label lbl -> yield lbl + ":"
      |Push reg -> yield "push " + reg_to_string reg
      |PushC value -> yield sprintf "push %A" value
      |PushRealRs -> yield! write_code [Push SP; Push BP; Push RX]  // let's see if not pushing PSR works
      |Pop reg -> yield "pop " + reg_to_string reg
      |PopRealRs -> yield! write_code [Pop SP; Pop BP; Pop RX]
      |ShiftStackDown _ -> failwith "shift stack down not supported yet"
      |MovRR(ra, rb) -> yield sprintf "mov %s %s" (reg_to_string ra) (reg_to_string rb)
      |MovRM(ra, mem) -> yield sprintf ""
      |
      yield! write_code rest
     }
  List.ofSeq >> write_code

let write_nasm (instrs: Asm seq) : string =
  sprintf
   """
section .data
  %s

section .text
  global start
  %s

start:
  %s"""
   (String.concat "\n  " (write_data instrs))
   (String.concat "\n  " (extern_calls instrs))
   (String.concat "\n  " (write_code instrs))