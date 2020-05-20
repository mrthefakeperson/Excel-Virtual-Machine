module Codewriters.Nasm
open Utils
open CompilerDatatypes.Semantics.InterpreterValue
open CompilerDatatypes.PseudoASM
open CompilerDatatypes.PseudoASM.Flat

let data_to_string : Boxed -> string = function
  |Int x -> sprintf ".int %i" x
  |Int64 x -> sprintf ".long %i" x
  |Byte x -> sprintf ".byte %i" x
  |Float x -> sprintf ".float %f" x
  |Double x -> sprintf ".double %f" x
  |Void -> failwith "cannot write void"
  |Ptr(x, _) -> sprintf ".int %i" x

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

let reg_to_string : Register -> string = function
  |R 0 -> "eax"
  |RX -> "ebx"
  |R 2 -> "ecx"
  |R 3 -> "edx"
  |R n -> failwithf "register %i does not exist" n
  |BP -> "ebp"
  |SP -> "esp"
  |PSR_EQ | PSR_GT | PSR_LT -> failwith "cannot access PSR directly"

let const_to_string : Boxed -> string = function
  |Int x -> sprintf "%i" x
  |Int64 x -> sprintf "%iL" x
  |Byte x -> sprintf "%i" x
  |Float x -> sprintf "%ff" x
  |Double x -> sprintf "%f" x
  |Void -> failwith "cannot write void"
  |Ptr(x, _) -> sprintf "%i" x

let write_code : Asm seq -> string seq =
  let rec write_code = function
    |[] -> Seq.empty
    |Label _::Data _::rest -> write_code rest
    |instr::rest -> seq {
      let (|Rs|) = reg_to_string
      match instr with
      |Label lbl -> yield lbl + ":"
      |Push reg -> yield "push " + reg_to_string reg
      |PushC value -> yield sprintf "push %A" value
      |PushRealRs -> yield! write_code [Push SP; Push BP; Push RX]  // let's see if not pushing PSR works
      |Pop reg -> yield "pop " + reg_to_string reg
      |PopRealRs -> yield! write_code [Pop SP; Pop BP; Pop RX]
      |ShiftStackDown _ -> failwith "shift stack down not supported yet"
      |MovRR(Rs ra, Rs rb) -> yield sprintf "mov %s, %s" ra rb
      |MovRM(Rs ra, Memory.Indirect (Rs rb)) -> yield sprintf "mov %s, [%s]" ra rb
      |MovRM(Rs ra, Memory.Lbl(lbl, _)) -> yield sprintf "mov %s, %s" ra lbl
      |MovRC(Rs ra, constval) -> yield sprintf "mov %s, %s" ra (const_to_string constval)
      |MovRHandle(Rs ra, HandleLbl(label, _)) -> yield sprintf "mov %s, %s" ra label
      |MovRHandle(Rs ra, HandleReg _) -> failwith "register handles not supported yet"
      |MovMR(Memory.Indirect (Rs ra), Rs rb) -> yield sprintf "mov [%s], %s" ra rb
      |MovMR(Memory.Lbl(lbl, _), Rs rb) -> yield sprintf "mov %s, %s" lbl rb
      |Add(4, Rs ra, Rs rb) -> yield sprintf "add %s, %s" ra rb
      |Add(8, Rs ra, Rs rb) -> yield sprintf "addl %s, %s" ra rb
      |Add _ -> failwith "unsupported size"
      |AddC(4, Rs ra, constval) -> yield sprintf "add %s, %s" ra (const_to_string constval)
      |AddC(8, Rs ra, constval) -> yield sprintf "addl %s, %s" ra (const_to_string constval)
      |AddC _ -> failwith "unsupported size"
      |Alloc(n, dt) -> yield! write_code [PushC (Int (n * dt.sizeof)); Call "malloc"]
      |Br lbl -> yield sprintf "jmp %s" lbl
      |Br0 lbl -> yield sprintf "jz %s" lbl
      |BrGT lbl -> yield sprintf "jg %s" lbl
      |BrLT lbl -> yield sprintf "jl %s" lbl
      |BrT lbl -> yield sprintf "jne %s" lbl
      |Call lbl -> yield sprintf "call %s" lbl
      |Cast(dt, r) -> ()
      |Cmp(Rs ra, Rs rb) -> yield sprintf "cmp %s, %s" ra rb
      |CmpC(Rs ra, constval) -> yield sprintf "cmp %s, %s" ra (const_to_string constval)
      |Data _ -> failwith "error: data in code section"
      |DivMod(4, Rs ra, Rs rb) -> failwith "div/mod not supported yet"
      |DivMod(8, Rs ra, Rs rb) -> failwith "div/mod not supported yet"
      |DivMod _ -> failwith "unsupported size"
      |DivModC(4, Rs ra, constval) -> failwith "div/mod not supported yet"
      |DivModC(8, Rs ra, constval) -> failwith "div/mod not supported yet"
      |DivModC _ -> failwith "unsupported size"
      |Mul(4, Rs ra, Rs rb) -> yield sprintf "imul %s, %s" ra rb
      |Mul(8, Rs ra, Rs rb) -> yield sprintf "imull %s, %s" ra rb
      |Mul _ -> failwith "unsupported size"
      |MulC(4, Rs ra, constval) -> yield sprintf "imul %s, %s" ra (const_to_string constval)
      |MulC(8, Rs ra, constval) -> yield sprintf "imull %s, %s" ra (const_to_string constval)
      |MulC _ -> failwith "unsupported size"
      |Ret -> yield "ret"
      |Sub(4, Rs ra, Rs rb) -> yield sprintf "sub %s, %s" ra rb
      |Sub(8, Rs ra, Rs rb) -> yield sprintf "subl %s, %s" ra rb
      |Sub _ -> failwith "unsupported size"
      |SubC(4, Rs ra, constval) -> yield sprintf "sub %s, %s" ra (const_to_string constval)
      |SubC(8, Rs ra, constval) -> yield sprintf "subl %s, %s" ra (const_to_string constval)
      |SubC _ -> failwith "unsupported size"
      yield! write_code rest
     }
  List.ofSeq >> write_code

let write_nasm (instrs: Asm seq) : string =
  let data =
    try write_data instrs |> List.ofSeq
    with _ -> [ "ERROR" ]
  let extern_calls =
    try extern_calls instrs |> List.ofSeq
    with _ -> [ "ERROR" ]
  let code =
    try write_code instrs |> List.ofSeq
    with _ -> [ "ERROR" ]
  sprintf
   """
section .data
  %s

section .text
  global start
  %s

start:
  %s"""
   (String.concat "\n  " data)
   (String.concat "\n  " extern_calls)
   (String.concat "\n  " code)