module Codewriters.Interpreter
open Parser.Datatype
open Codegen.PAsm.Simple
open Codegen.PAsm

let REAL_REGS = [R 0; RX; BP; SP; PSR_EQ; PSR_GT; PSR_LT]

type State = {
  mem: Boxed[]
  pc: int
  regs: Map<Register, Boxed>
  next_alloc: int
 }
  with
    static member copy (state: State) = {state with mem = Array.copy state.mem}
    static member initialize (code: #(Asm seq)) =
      let initial = {
        mem = Array.create (max STACK_START CODE_START * 2) Void
        pc = CODE_START
        regs = Map (List.map (fun r -> (r, Void)) REAL_REGS)
        next_alloc = 1
       }
      let code_as_boxed = Array.ofSeq code |> Array.collect (function Data data -> data | Label _ -> [||] | _ -> [|Void|])
      Array.blit code_as_boxed 0 initial.mem CODE_START code_as_boxed.Length
      let init_registers = [(BP, Ptr(STACK_START + 1, DT.Byte)); (SP, Ptr(STACK_START, DT.Byte))]
      { initial with
          regs = List.fold (fun acc (k, v) -> Map.add k v acc) initial.regs init_registers }
    static member branch pc' state = {state with pc = pc'}
    static member to_next_pc state = State.branch (state.pc + 1) state
    static member current_pc (state: State) = Ptr(state.pc, DT.Void)
    
    static member read_register reg (state: State) =
      match reg with
      |_ when state.regs.ContainsKey reg -> state.regs.[reg]
      |Register.R n | Strict n ->
        let Ptr(bp, DT.Byte) | Strict bp = state.regs.[BP]
        state.mem.[bp + (if n > 0 then n - 1 else n)]
    static member write_register reg value (state: State) =
      match reg with
      |_ when state.regs.ContainsKey reg -> { state with regs = Map.add reg value state.regs }
      |Register.R n | Strict n ->
        let Ptr(bp, DT.Byte) | Strict bp = state.regs.[BP]
        state.mem.[bp + (if n > 0 then n - 1 else n)] <- value; state
    static member read_mem (Ptr(addr, _) | Strict addr) (state: State) = state.mem.[addr]
    static member write_mem (Ptr(addr, _) | Strict addr) value (state: State) =
      state.mem.[addr] <- value; state
      
    // convention: current stack ptr is the top of the stack (full); do stack_peek before stack_pop
    static member stack_peek state =
      State.read_mem (State.read_register SP state) state
    static member stack_pop state =
      State.write_register SP (State.read_register SP state - Ptr(1, DT.Byte)) state
    static member stack_push value state =
      let sp' = State.read_register SP state + Ptr(1, DT.Byte)
      State.write_mem sp' value state
       |> State.write_register SP sp'

    static member compare (a: Boxed) (b: Boxed) state =
      State.write_register PSR_EQ (a.equals b) state
       |> State.write_register PSR_LT (a.less_than b)
       |> State.write_register PSR_GT (a.greater_than b)

    static member current_alloc (state: State) = Ptr(state.next_alloc, DT.Byte)
    static member alloc n state = {state with next_alloc = state.next_alloc + n}

    static member trace ({pc = pc; mem = mem; regs = regs; next_alloc = next_alloc} as state) =
      String.concat "\n" [
        sprintf "pc: %i" pc
        sprintf "psr: (eq %A, lt %A, gt %A)" regs.[PSR_EQ] regs.[PSR_LT] regs.[PSR_GT]
        sprintf "sp: %A, bp: %A, stack: %A" regs.[SP] regs.[BP] mem.[STACK_START..STACK_START + 30]
        sprintf "next alloc: %A, mem: %A" next_alloc mem.[..50]
        sprintf "code: %A" mem.[CODE_START..CODE_START + 50]
        sprintf "R0: %A, RX: %A" regs.[Register.R 0] regs.[RX]
       ]
       |> printfn "%s"
      state

let inline ( *>> ) f1 f2 x = f2 (f1 x) x
let inline ( <<* ) f2 f1 = f1 *>> f2
let inline ( <<*. ) f f1 c x = f (f1 x) c x  // feed 1st arg of 2
let inline lift x _ = x
let rec eval': Asm -> State -> State = function
  |Data _ -> failwith "tried to execute data as code"
  |Label _ -> id
  |Push rc -> State.stack_push <<* match rc with RC.R r -> State.read_register r | RC.C c -> lift c
  |PushRealRs -> List.fold (fun acc real_reg -> acc >> eval' (Push (RC.R real_reg))) id REAL_REGS
  |Pop r -> (State.stack_peek *>> State.write_register r) >> State.stack_pop
  |PopRealRs -> List.fold (fun acc real_reg -> acc >> eval' (Pop real_reg)) id REAL_REGS
  |ShiftStackDown(off, len) ->
    [1..len]
     |> List.fold (fun acc i ->
          let shift_position_i = 
            State.write_mem
             <<*. (State.read_register SP >> ((+) (Ptr(i - off, DT.Byte))))
             <<* (State.read_mem <<* (State.read_register SP >> ((+) (Ptr(i, DT.Byte)))))
          acc >> shift_position_i
         ) id
  |Mov(rm, rmc) ->
    let write =
      match rm with
      |RM.R r -> State.write_register r
      |RM.M addr -> State.write_mem (Ptr(addr, DT.Void))
      |RM.I r -> State.write_mem <<*. State.read_register r
    let read =
      match rmc with
      |RMC.C c -> fun _ -> c
      |RMC.R r -> State.read_register r
      |RMC.M addr -> State.read_mem (Ptr(addr, DT.Void))
      |RMC.I r -> State.read_register r *>> State.read_mem
    read *>> write
  |Cmp(r, rc) ->
    State.compare <<*. State.read_register r
     <<* match rc with RC.R r' -> State.read_register r' | RC.C c -> lift c
  |Br(br_t, addr) ->
    let apply_branch flag = if flag then State.branch (addr - 1) else id
    let read_psr r = State.read_register r >> function Byte 0uy -> false | _ -> true
    match br_t with
    |B -> State.branch (addr - 1)
    |Z -> read_psr PSR_EQ *>> apply_branch
    |T -> read_psr PSR_EQ *>> (not >> apply_branch)
    |LT -> read_psr PSR_LT *>> apply_branch
    |GT -> read_psr PSR_GT *>> apply_branch
  |Call addr ->  // TODO: consider adding custom instruction for print
    State.stack_push <<* State.current_pc
     >> State.branch (addr - 1)
  |Ret ->
    let extract (Ptr(x, _) | Strict x) = x
    ((State.stack_peek >> extract) *>> State.branch) >> State.stack_pop
  |Cast(dt, reg) -> State.write_register reg <<* (State.read_register reg >> Boxed.cast dt)
  |Alloc n -> (State.stack_push <<* State.current_alloc) >> State.alloc n
  |Arith(arith_t, sz, reg, rc) ->
    let check (v: Boxed) = if v.datatype.sizeof = sz then id else failwith "data size mismatch"
    let opnd1 = State.read_register reg
    let opnd2 = match rc with RC.R r -> State.read_register r | RC.C c -> lift c
    let op a b =
      match arith_t with
      |Add -> State.write_register reg (a + b)
      |Sub -> State.write_register reg (a - b)
      |Mul -> State.write_register reg (a * b)
      |DivMod -> State.write_register reg (a / b) >> State.write_register RX (a % b)
    check <<* opnd1
     >> (op <<*. opnd1 <<* opnd2)
     
let EXTERN_CALL_ADDR = CODE_START - 7

let DEBUG = true

let eval: Asm list -> State = fun instrs ->
  let instrs = Array.ofList (List.filter (function Label _ -> false | _ -> true) instrs)  // labels get removed because they are not considered when calculating label addresses
  let rec eval = function
    |state when state.pc = EXTERN_CALL_ADDR -> state
    |state when not (CODE_START <= state.pc && state.pc < CODE_START + instrs.Length) ->
      failwithf "executing non-code as code: pc = %A" state.pc
    |state ->
      if DEBUG then printfn ">>> %A" (instrs.[state.pc - CODE_START])
      let state' = eval' (instrs.[state.pc - CODE_START]) state
      state'
       |> State.to_next_pc
       |> if DEBUG then State.trace else id
       |> eval
  State.initialize instrs
   // initially, push a magic number onto the stack; returning from main will return here, and the interpreter stops running
   |> State.write_mem (Ptr(STACK_START, DT.Void)) (Ptr(EXTERN_CALL_ADDR - 1, DT.Void))
   |> eval