module Codegen.BuiltinFunctions
open Parser.Datatype
open Codegen.PAsm
open Codegen.PAsm.Flat

let replace_r0 instrs f_append_reg f_append_const append_r0 =
  match List.rev instrs with
  |MovRR(R 0, x)::rev_before -> List.rev rev_before @ f_append_reg x
  |MovRC(R 0, x)::rev_before -> List.rev rev_before @ f_append_const x
  |_ -> instrs @ append_r0

let drop_r0 instrs = replace_r0 instrs (fun _ -> []) (fun _ -> []) []

let to_rn_from_r0 rn instrs =
  replace_r0 instrs (fun x -> [MovRR(rn, x)]) (fun x -> [MovRC(rn, x)]) [MovRR(rn, R 0)]

let push_r0 instrs = replace_r0 instrs (fun x -> [Push x]) (fun x -> [PushC x]) [Push (R 0)]

let generate f dt arg_instrs =
  let t = lazy match dt with DT.Function(t::_, _) | Strict t -> t
  let size = lazy t.Force().sizeof
  match f, arg_instrs with  // TODO: detect id values
  |"+", ([a; [MovRC(R 0, value)]] | [[MovRC(R 0, value)]; a]) -> a @ [AddC(size.Force(), R 0, value)]
  |"+", [a; b] -> push_r0 a @ b @ [Pop RX; Add(size.Force(), R 0, RX)]
    // WARNING: for non-commutative operators, the right value is evaluated first
  |"-", [a; [MovRC(R 0, value)]] -> a @ [SubC(size.Force(), R 0, value)]
  |"-", [a; b] -> push_r0 b @ a @ [Pop RX; Sub(size.Force(), R 0, RX)]
  |"*", ([a; [MovRC(R 0, value)]] | [[MovRC(R 0, value)]; a]) -> a @ [MulC(size.Force(), R 0, value)]
  |"*", [a; b] -> push_r0 a @ b @ [Pop RX; Mul(size.Force(), R 0, RX)]
  |"/", [a; [MovRC(R 0, value)]] -> a @ [DivModC(size.Force(), R 0, value)]
  |"/", [a; b] -> push_r0 b @ a @ [Pop RX; DivMod(size.Force(), R 0, RX)]
  |"%", [a; [MovRC(R 0, value)]] -> a @ [DivModC(size.Force(), R 0, value); MovRR(R 0, RX)]  // assume that mod is stored in RX
  |"%", [a; b] -> push_r0 b @ a @ [Pop RX; DivMod(size.Force(), R 0, RX); MovRR(R 0, RX)]
  |"*prefix", [a] -> a @ [MovRM(R 0, Indirect (R 0))]
  |"-prefix", [a] -> a @ [MulC(size.Force(), R 0, Option.get (Boxed.from_double (t.Force()) -1.))]
  |("&prefix" | "&&" | "||" | "!=" | "<=" | ">=" | "!"), _ -> failwith "should never be reached; handled statically"
  |"==", [a; b] -> push_r0 a @ b @ [Pop RX; Cmp(RX, R 0); MovRR(R 0, PSR_EQ)]
  |">", [a; b] -> push_r0 a @ b @ [Pop RX; Cmp(RX, R 0); MovRR(R 0, PSR_GT)]
  |"<", [a; b] -> push_r0 a @ b @ [Pop RX; Cmp(RX, R 0); MovRR(R 0, PSR_LT)]
  |"printf", args -> [PushRealRs] @ List.collect push_r0 args @ [MovRR(BP, SP); Call "printf"; PopRealRs]
  |_ -> failwithf "builtin function %s (%i args) not found" f (List.length arg_instrs)