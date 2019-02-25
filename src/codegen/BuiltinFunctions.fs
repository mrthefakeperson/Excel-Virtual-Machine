module Codegen.BuiltinFunctions
open Codegen.PAsm
open Codegen.Interpreter

let replace_r0 instrs f_append_reg f_append_const append_r0 =
  match List.rev instrs with
  |MovRR(R 0, x)::rev_before -> List.rev rev_before @ f_append_reg x
  |MovRC(R 0, x)::rev_before -> List.rev rev_before @ f_append_const x
  |_ -> instrs @ append_r0

let drop_r0 instrs = replace_r0 instrs (fun _ -> []) (fun _ -> []) []

let to_rn_from_r0 rn instrs =
  replace_r0 instrs (fun x -> [MovRR(rn, x)]) (fun x -> [MovRC(rn, x)]) [MovRR(rn, R 0)]

let push_r0 instrs = replace_r0 instrs (fun x -> [Push x]) (fun x -> [PushC x]) [Push (R 0)]

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
  |"*prefix", [a] -> a @ [MovRM(R 0, Indirect (R 0))]
  |("&prefix" | "&&" | "||" | "!=" | "<=" | ">=" | "!"), _ -> failwith "should never be reached; handled statically"
  |"==", [a; b] -> push_r0 a @ b @ [Cmp(R 0, SP); SubC(SP, Int 1); MovRR(R 0, PSR_EQ)]
  |">", [a; b] -> push_r0 a @ b @ [Cmp(R 0, SP); SubC(SP, Int 1); MovRR(R 0, PSR_GT)]
  |"<", [a; b] -> push_r0 a @ b @ [Cmp(R 0, SP); SubC(SP, Int 1); MovRR(R 0, PSR_LT)]
  |"printf", args -> [PushRealRs] @ List.collect push_r0 args @ [MovRR(BP, SP); Call "printf"; PopRealRs]
  |_ -> failwithf "builtin function %s (%i args) not found" f (List.length arg_instrs)