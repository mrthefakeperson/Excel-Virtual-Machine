module CompilerDatatypes.PseudoASM
open Utils
open CompilerDatatypes.DT
open CompilerDatatypes.Semantics.InterpreterValue

type Register =
  |R of int | RX
  |BP | SP
  |PSR_EQ | PSR_LT | PSR_GT

type Memory = Lbl of string * DT | Indirect of Register

// contains reference to a var whose reference cannot be directly expressed
type Handle = HandleLbl of string * DT | HandleReg of int

module Flat =
  type Asm =
    |Data of Boxed[]
    |Label of name: string
    |Push of Register
    |PushC of Boxed
    |PushRealRs
    |Pop of Register
    |PopRealRs
    |ShiftStackDown of offset: int * length: int  // tail recursion shortcut: shift {length} items below SP starting at {offset} spaces above SP
    |MovRR of Register * Register
    |MovRM of Register * Memory
    |MovMR of Memory * Register
    |MovRC of Register * Boxed
    |MovRHandle of Register * Handle  // move the address value directly into a register; useful when address is not available at time of codegen
    |Cmp of Register * Register  // sets comparison flags (a - b, (> 0)? (= 0)? (< 0)?)
    |CmpC of Register * Boxed
    |Br of label: string  // unconditional branch
    |Br0 of label: string  // branch if EQ flag is 0
    |BrT of label: string  // branch if EQ flag is not 0
    |BrLT of label: string
    |BrGT of label: string
    |Call of label: string
    |Ret
    // operations
    |Cast of DT * Register
    |Alloc of blocks: int * DT  // alloc an address into R0
    |Add of sz: int * Register * Register
    |AddC of sz: int * Register * Boxed
    |Sub of sz: int * Register * Register
    |SubC of sz: int * Register * Boxed
    |Mul of sz: int * Register * Register
    |MulC of sz: int * Register * Boxed
    |DivMod of sz: int * Register * Register
    |DivModC of sz: int * Register * Boxed
    
module Simple =
  type RC = R of Register | C of Boxed
  type RM = R of Register | M of addr: int * dt: DT | I of deref: Register
  type RMC = R of Register | C of Boxed | M of addr: int * dt: DT | I of deref: Register

  type Asm =
    |Data of Boxed[]
    |Label of addr: int
    |Push of RC
    |PushRealRs
    |Pop of Register
    |PopRealRs
    |ShiftStackDown of offset: int * length: int
    |Mov of RM * RMC
    |Cmp of Register * RC
    |Br of BrType * label: int
    |Call of label: int
    |ExternCall of label: string
    |Ret
    |Cast of DT * Register
    |Alloc of bytes: int
    |Arith of ArithType * sz: int * Register * RC

  and BrType = B | Z | T | LT | GT
  and ArithType = Add | Sub | Mul | DivMod
  
  let NUM_REAL_REGS = 4  // unused; TODO: convert some virtual registers into real ones
  let STACK_START = 100000
  let CODE_START = 200000

  let convert_from_flat (instrs: Flat.Asm list) : Asm list =
    let labels_map =
      List.fold (fun (i, acc) -> function
        |Flat.Label lbl -> (i, (lbl, i)::acc)
        |Flat.Data data -> (i + data.Length, acc)
        |Flat.Alloc _ -> (i + 2, acc)
        |Flat.MovRHandle(_, HandleReg _) -> (i + 2, acc)
        |_ -> (i + 1, acc)  // TODO: instruction size
       ) (0, []) instrs |> snd
       |> dict

    let label_addr lbl =
      if labels_map.ContainsKey lbl
       then CODE_START + labels_map.[lbl]
       else failwithf "label not declared: %s" lbl
    let mov_rh reg: Handle -> Asm List = function
      |HandleLbl(lbl, (DT.Ptr dt | Strict dt)) -> [Mov(RM.R reg, RMC.C (Ptr(label_addr lbl, dt)))]
      |HandleReg 0 -> failwith "code generation error: handle of R0?"
      |HandleReg n ->
        let bp_offset = Ptr(n, DT.Byte)
        [Mov(RM.R reg, RMC.R BP); Arith(Add, 4, reg, RC.C bp_offset)]
    let convert_memory_rm = function
      |Memory.Lbl(lbl, dt) -> RM.M (label_addr lbl, dt)
      |Memory.Indirect r -> RM.I r
    let convert_memory_rmc = function
      |Memory.Lbl(lbl, dt) -> RMC.M (label_addr lbl, dt)
      |Memory.Indirect r -> RMC.I r
    
    List.collect (function
      |Flat.Label lbl -> [Label (label_addr lbl)]
      // if label is not found, assume extern call
      |Flat.Call lbl -> [(try Call (label_addr lbl) with _ -> ExternCall lbl)]
      |Flat.Push r -> [Push (RC.R r)] | Flat.PushC c -> [Push (RC.C c)]
      |Flat.MovRR(r1, r2) -> [Mov(RM.R r1, RMC.R r2)]
      | Flat.MovRM(r, m) -> [Mov(RM.R r, convert_memory_rmc m)]
      | Flat.MovMR(m, r) -> [Mov(convert_memory_rm m, RMC.R r)]
      | Flat.MovRC(r, c) -> [Mov(RM.R r, RMC.C c)]
      | Flat.MovRHandle(r, h) -> mov_rh r h  // TODO: does this work? Ptr to Void
      |Flat.Cmp(r1, r2) -> [Cmp(r1, RC.R r2)] | Flat.CmpC(r1, c) -> [Cmp(r1, RC.C c)]
      |Flat.Br lbl -> [Br(B, label_addr lbl)] | Flat.Br0 lbl -> [Br(Z, label_addr lbl)] | Flat.BrT lbl -> [Br(T, label_addr lbl)]
      | Flat.BrLT lbl -> [Br(LT, label_addr lbl)] | Flat.BrGT lbl -> [Br(GT, label_addr lbl)]
      |Flat.Add(sz, r1, r2) -> [Arith(Add, sz, r1, RC.R r2)] | Flat.AddC(sz, r, c) -> [Arith(Add, sz, r, RC.C c)]
      |Flat.Sub(sz, r1, r2) -> [Arith(Sub, sz, r1, RC.R r2)] | Flat.SubC(sz, r, c) -> [Arith(Sub, sz, r, RC.C c)]
      |Flat.Mul(sz, r1, r2) -> [Arith(Mul, sz, r1, RC.R r2)] | Flat.MulC(sz, r, c) -> [Arith(Mul, sz, r, RC.C c)]
      |Flat.DivMod(sz, r1, r2) -> [Arith(DivMod, sz, r1, RC.R r2)] | Flat.DivModC(sz, r, c) -> [Arith(DivMod, sz, r, RC.C c)]
      |Flat.Data x -> List.map (Data << Array.singleton) (List.ofArray x)
      | Flat.PushRealRs -> [PushRealRs] | Flat.Pop r -> [Pop r] | Flat.PopRealRs -> [PopRealRs]
      | Flat.ShiftStackDown(off, len) -> [ShiftStackDown(off, len)] | Flat.Ret -> [Ret] | Flat.Cast(dt, r) -> [Cast(dt, r)]
      | Flat.Alloc(n, dt) -> [Alloc (n * dt.sizeof); Cast(dt, Register.R 0)]
     ) instrs