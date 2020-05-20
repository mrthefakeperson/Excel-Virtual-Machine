module Sim.NRegisterMachine
// input: list of PAsm instruction "batches"
// batches execute sequentially
// instructions in each batch execute in parallel using current stable register values

open Utils
open PAsmMachine
open CompilerDatatypes.PseudoASM.Simple

let inline at_most_one_update original updated1 updated2 =
  if original <> updated1 && original <> updated2 then
    failwith "expecting at most one update"
  else if original <> updated1 then updated1
  else updated2

let inline updates_must_agree original updated1 updated2 =
  if original <> updated1 && original <> updated2 then
    if updated1 <> updated2 then failwith "inconsistent update"
    updated1
  else if original <> updated1 then updated1
  else updated2

let merge_updated_states (original : State) (updated1 : State) (updated2 : State) : State =
  let pc =
    try updates_must_agree original.pc updated1.pc updated2.pc
    with _ -> failwith "inconsistent PC in instruction batch"
  let regs =
    Map.map (fun reg value ->
      let updated_value1 = updated1.regs.[reg]
      let updated_value2 = updated2.regs.[reg]
      try updates_must_agree value updated_value1 updated_value2
      with _ -> failwith "inconsistent reg in instruction batch"
     ) original.regs
  let next_alloc =
    try at_most_one_update original.next_alloc updated1.next_alloc updated2.next_alloc
    with _ -> failwith "multiple memory allocations in instruction batch"
  let stdout =
    try at_most_one_update original.stdout updated1.stdout updated2.stdout
    with _ -> failwith "multiple prints in instruction batch"
  let mem =
    let all_addrs =
      Map.toList original.mem @ Map.toList updated1.mem @ Map.toList updated2.mem
       |> List.map fst
       |> Set.ofList
    Set.fold (fun mem addr ->
      let value = State.memory_at addr original
      let updated_value1 = State.memory_at addr updated1
      let updated_value2 = State.memory_at addr updated2
      let value =
        try updates_must_agree value updated_value1 updated_value2
        with _ -> failwith "inconsistent memory value in instruction batch"
      Map.add addr value mem
     ) Map.empty all_addrs
  { State.mem = mem; pc = pc; regs = regs; next_alloc = next_alloc; stdout = stdout }

let eval (batches : Asm list list) : State =
  let all_instrs = List.concat batches
  let state = State.initialize all_instrs
  List.fold (fun state batch ->
    let all_updated = List.map (fun instr -> eval_one_instr instr state) batch
    List.fold (merge_updated_states state) state all_updated
   ) state batches