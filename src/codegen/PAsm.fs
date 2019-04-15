module Codegen.PAsm
open Parser.Datatype

type Boxed =  // constant value
  |Int of int
  |Int64 of int64
  |Byte of byte
  |Float of float32
  |Double of float
  |Void
  |Ptr of addr: int * DT
  with
    member x.datatype =
      match x with
      |Int _ -> DT.Int
      |Int64 _ -> DT.Int64
      |Byte _ -> DT.Byte
      |Float _ -> DT.Float
      |Double _ -> DT.Double
      |Void -> DT.Void
      |Ptr(_, dt) -> DT.Ptr dt
    static member check_type (dt: DT) (x: Boxed) =
      let dt' = (Boxed.default_value dt).datatype
      if x.datatype <> dt' then failwithf "type check failed: %A <> %A (from %A)" x dt' dt

    member x.to_int64 =
      match x with
      |Int x -> Some (int64 x)
      |Int64 x -> Some x
      |Byte x -> Some (int64 x)
      |Ptr(x, _) -> Some (int64 x)
      |_ -> None
    member x.to_double =
      match x.to_int64, x with
      |Some x, _ -> Some (float x)
      |_, Float x -> Some (float x)
      |_, Double x -> Some x
      |_ -> None
    static member inline from_t dt (x: 'a) =
      match dt with
      |DT.Int -> Some <| Int (int x)
      |DT.Int64 -> Some <| Int64 (int64 x)
      |DT.Byte -> Some <| Byte (byte x)
      |DT.Float -> Some <| Float (float32 x)
      |DT.Double -> Some <| Double (float x)
      |DT.Ptr dt -> Some <| Ptr(int x, dt)
      |DT.Function _ | DT.Function2 _ -> Some <| Ptr(int x, DT.Void)
      |_ -> None
    static member from_int64 dt (x: int64) = Boxed.from_t dt x
    static member from_double dt (x: double) = Boxed.from_t dt x
    static member from_string dt (x: string) =
      try
        match dt with
        |DT.Byte when x.Length >= 3 && x.[0] = ''' && x.[x.Length - 1] = ''' ->
          let c =
            match x.[1..x.Length - 2] with
            |"\\n" -> '\n' | "\\\\" -> '\\' | "\\0" -> '\000'
            |x' -> char x'
          Some <| Byte (byte c)
        |DT.Int64 -> Boxed.from_t dt (x.TrimEnd 'L')
        |_ -> Boxed.from_t dt x
      with :? System.FormatException -> None
    static member default_value dt : Boxed =
      match Boxed.from_int64 dt 0L, dt with
      |Some x, _ -> x
      |None, DT.Void -> Void
      |None, unknown -> failwithf "no default value for %A" unknown
    static member cast dt (x: Boxed) =
      let fail () = failwithf "can't cast %A to %A" x dt
      Option.defaultWith fail (Boxed.from_double dt (Option.defaultWith fail x.to_double))

    static member make_binary_op f_int f_dbl (a: Boxed) (b: Boxed) =
      if a.datatype <> b.datatype
       then failwithf "can't combine %A: datatypes don't match" (a, b)
       else
        match a.to_int64, b.to_int64 with
        |Some a', Some b' -> Choice1Of2 (f_int a' b')
        |_ ->
          match a.to_double, b.to_double with
          |Some a'', Some b'' -> Choice2Of2 (f_dbl a'' b'')
          |_ -> failwithf "can't combine %A: couldn't normalize one of the operands" (a, b)
    static member make_arith_op f_int f_dbl a b =
      match Boxed.make_binary_op f_int f_dbl a b with
      |Choice1Of2 i64 -> Option.get <| Boxed.from_int64 a.datatype i64
      |Choice2Of2 dbl -> Option.get <| Boxed.from_double b.datatype dbl
    static member (+) (a, b) = Boxed.make_arith_op (+) (+) a b
    static member (-) (a, b) = Boxed.make_arith_op (-) (-) a b
    static member ( * ) (a, b) = Boxed.make_arith_op ( * ) ( * ) a b
    static member (/) (a, b) = Boxed.make_arith_op (/) (/) a b
    static member (%) (a, b) = Boxed.make_arith_op (%) (%) a b

    static member make_cmp_op (=><) a b =
      let cmp_i x y = if double x =>< double y then 1L else 0L
      let cmp_d x y = if x =>< y then 1. else 0.
      match Boxed.make_binary_op cmp_i cmp_d a b with
      |Choice1Of2 0L | Choice2Of2 0. -> Byte 0uy
      |_ -> Byte 1uy
    member x.equals y = Boxed.make_cmp_op (=) x y
    member x.greater_than y = Boxed.make_cmp_op (>) x y
    member x.less_than y = Boxed.make_cmp_op (<) x y

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
        |_ -> (i + 1, acc)  // TODO: instruction size
       ) (0, []) instrs |> snd
       |> dict

    let label_addr lbl =
      if labels_map.ContainsKey lbl then CODE_START + labels_map.[lbl] else failwithf "label not declared: %s" lbl
    let mov_rh reg: Handle -> Asm List = function
      |HandleLbl(lbl, (DT.Ptr dt | Strict dt)) -> [Mov(RM.R reg, RMC.C (Ptr(label_addr lbl, dt)))]
      |HandleReg 0 -> failwith "code generation error: handle of R0?"
      |HandleReg n ->
        [Mov(RM.R reg, RMC.R BP); Arith(Add, 4, reg, RC.C (if n > 0 then Ptr(n - 1, DT.Byte) else Ptr(n, DT.Byte)))]
    let convert_memory_rm = function
      |Memory.Lbl(lbl, dt) -> RM.M (label_addr lbl, dt)
      |Memory.Indirect r -> RM.I r
    let convert_memory_rmc = function
      |Memory.Lbl(lbl, dt) -> RMC.M (label_addr lbl, dt)
      |Memory.Indirect r -> RMC.I r
    
    List.collect (function
      |Flat.Label lbl -> [Label (label_addr lbl)]
      |Flat.Call lbl -> [Call (label_addr lbl)]
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

      |Flat.Data x -> [Data x] | Flat.PushRealRs -> [PushRealRs] | Flat.Pop r -> [Pop r] | Flat.PopRealRs -> [PopRealRs]
      | Flat.ShiftStackDown(off, len) -> [ShiftStackDown(off, len)] | Flat.Ret -> [Ret] | Flat.Cast(dt, r) -> [Cast(dt, r)]
      | Flat.Alloc(n, dt) -> [Alloc (n * dt.sizeof); Cast(dt, Register.R 0)]
     ) instrs