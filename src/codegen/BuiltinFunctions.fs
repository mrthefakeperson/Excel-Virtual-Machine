module Codegen.BuiltinFunctions
open Codegen.PAsm
open Codegen.Interpreter

let to_rn_from_r0 rn instrs =
  match List.rev instrs with
  |MovRR(R 0, x)::rev_before -> List.rev rev_before @ [MovRR(rn, x)]
  |MovRC(R 0, x)::rev_before -> List.rev rev_before @ [MovRC(rn, x)]
  |_ -> instrs @ [MovRR(rn, R 0)]

let push_r0 instrs =
  match List.rev instrs with
  |MovRR(R 0, x)::rev_before -> List.rev rev_before @ [Push x]
  |MovRC(R 0, x)::rev_before -> List.rev rev_before @ [PushC x]
  |_ -> instrs @ [Push (R 0)]

let generate f arg_instrs =
  match f, arg_instrs with  // TODO: detect id values
  |"+", ([a; [MovRC(R 0, value)]] | [[MovRC(R 0, value)]; a]) -> a @ [AddC(R 0, value)]
  |"+", [a; b] -> push_r0 a @ b @ [Add(R 0, SP); SubC(SP, Int 1)]
  |"-", [a; [MovRC(R 0, value)]] -> a @ [SubC(R 0, value)]
  |"-", [a; b] -> push_r0 a @ b @ [Sub(R 0, SP); SubC(SP, Int 1)]
  |"*", ([a; [MovRC(R 0, value)]] | [[MovRC(R 0, value)]; a]) -> a @ [MulC(R 0, value)]
  |"*", [a; b] -> push_r0 a @ b @ [Mul(R 0, SP); SubC(SP, Int 1)]
  |"/", [a; [MovRC(R 0, value)]] -> a @ [DivModC(R 0, value)]
  |"/", [a; b] -> push_r0 a @ b @ [DivMod(R 0, SP); SubC(SP, Int 1)]
  |"%", [a; [MovRC(R 0, value)]] -> a @ [DivModC(R 0, value); MovRR(R 0, RX)]
  |"%", [a; b] -> push_r0 a @ b @ [DivMod(R 0, SP); MovRR(R 0, RX); SubC(SP, Int 1)]  // assume that mod is stored in RX
  |"==", [a; b] -> push_r0 a @ b @ [Cmp(R 0, SP); SubC(SP, Int 1)]
  |_ -> failwithf "builtin function %s (%i args) not found" f (List.length arg_instrs)