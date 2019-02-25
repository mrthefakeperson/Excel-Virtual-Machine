module Codewriters.Interpreter
open Parser.AST
open Codegen.PAsm
open Codegen.Interpreter

type AsmMemory = {
  code: Lazy<Boxed>[]
  pc: int
  reg_sp: Boxed
  reg_bp: Boxed
  reg_psr: Boxed * Boxed * Boxed
  real_regs: Boxed[]
  labels: Map<string, int>
  stack: Boxed[]
  mem: Boxed[]
  alloc_ptr: int
 }
   with
    member x.deref v =
      match v with
      |Ptr(addr, t) ->
        let value = if addr >= STACK_START then x.stack.[addr - STACK_START] else x.mem.[addr]
        // type check here
        value
      |_ -> failwith "can't deref a non-pointer value"

    member x.read_register reg =
      match reg with
      |SP -> x.reg_sp
      |BP -> x.reg_bp
      |R 0 -> x.real_regs.[0]
      |RX -> x.real_regs.[1]
      |R n when 0 < n && n < NUM_REAL_REGS - 1 -> x.real_regs.[n + 1]
      |R n ->
        match x.reg_bp, x.reg_sp with
        |Ptr(base_addr, _), Ptr _ ->
          let sp_offset = if n < 0 then n else n - NUM_REAL_REGS
          x.stack.[base_addr + sp_offset]
        |_ -> failwith "bp and sp should be Ptr's"
      |PSR_EQ -> x.reg_psr |> fun (a, _, _) -> a
      |PSR_GT -> x.reg_psr |> fun (_, a, _) -> a
      |PSR_LT -> x.reg_psr |> fun (_, _, a) -> a

    member x.write_register reg v =
      match reg with
      |SP -> {x with reg_sp = v}
      |BP -> {x with reg_bp = v}
      |R 0 -> x.real_regs.[0] <- v; x
      |RX -> x.real_regs.[1] <- v; x
      |R n when 0 < n && n < NUM_REAL_REGS - 1 -> x.real_regs.[n + 1] <- v; x
      |R n ->
        match x.reg_bp, x.reg_sp with
        |Ptr(base_addr, _), Ptr _ ->
          let sp_offset = if n < 0 then n else n - NUM_REAL_REGS
          x.stack.[base_addr + sp_offset] <- v; x
        |_ -> failwith "bp and sp should be Ptr's"
      |PSR_EQ | PSR_LT | PSR_GT -> failwith "can't write to PSR"

    member x.stack_push v =
      match x.read_register SP with Ptr(addr, _) -> x.mem.[addr] <- v | _ -> failwith "SP should be a Ptr"
      x.write_register SP (x.read_register SP + Int 1)

    member x.stack_pop =
      match x.read_register SP with
      |Ptr(addr, _) when addr - 1 < STACK_START -> failwith "stack underflow?"
      |Ptr(addr, _) -> x.mem.[addr], x.write_register SP (x.read_register SP - Int 1)
      |_ -> failwith "SP should be a Ptr"

    member x.compare (a: Boxed) b =
      { x with reg_psr = (a.equals b, a.greater_than b, a.less_than b) }

    member x.all_real_regs = BP::SP::R 0::RX::[for n in 1..NUM_REAL_REGS - 1 -> R n]
    member x.next = {x with pc = x.pc + 1}

let init_memory = {
  code = [||]
  pc = 0
  reg_sp = Ptr(STACK_START, Datatype.Void)
  reg_bp = Ptr(STACK_START, Datatype.Void)
  reg_psr = Byte 0uy, Byte 0uy, Byte 0uy
  real_regs = Array.create NUM_REAL_REGS Void
  labels = Map.empty
  stack = Array.create 10000 Void
  mem = Array.create 10000 Void
  alloc_ptr = 0
 }

let eval_pasm pasm_instructions =
  let instructions = Array.ofList pasm_instructions

  let code = ResizeArray<Lazy<Boxed>>()
  let labels: Map<string, int> =
    List.fold (fun labels -> function
      |Data(data: Boxed[]) ->
        code.AddRange(Array.map (fun e -> lazy e) data)
        labels
      |Label name -> labels.Add(name, code.Count)
      |_ ->
        code.Add(lazy failwith "tried to use code as data")
        labels
     ) Map.empty pasm_instructions

  let init_memory = {
    init_memory with
      code = Array.ofSeq code
      labels = labels
      pc = Map.find "main" labels
   }
   
  let rec eval (mem: AsmMemory) =  // pc - program counter / instruction register
    match instructions.[mem.pc] with
    |Data _ -> failwith "tried to execute data as code"
    |Label _ -> eval mem.next
    |Push r -> eval (mem.stack_push(mem.read_register r).next)
    |PushC x -> eval (mem.stack_push(x).next)
    |PushRealRs ->
      let mem' =
        List.fold (fun (acc_mem: AsmMemory) r ->
          acc_mem.stack_push(acc_mem.read_register r)
         ) mem mem.all_real_regs
      eval mem'.next
    |Pop r ->
      let x, mem' = mem.stack_pop
      (mem'.write_register r x).next
    |PopRealRs ->
      let mem' =
        List.fold (fun (acc_mem: AsmMemory) r ->
          let x, acc_mem' = acc_mem.stack_pop
          acc_mem'.write_register r x
         ) mem (List.rev mem.all_real_regs)
      eval mem'.next
    |MovRR(r1, r2) -> eval (mem.write_register r1 (mem.read_register r2)).next
    |MovRM(r, ptr) ->
      match ptr with
      |Indirect rx -> eval (mem.write_register r (mem.deref (mem.read_register rx))).next
      |Lbl name -> eval (mem.write_register r (mem.deref (Ptr(mem.labels.[name], Datatype.Void)))).next  // TODO: labels should have assoc. type too
    |MovMR(ptr, r) ->
      match ptr with
      |Indirect rx ->
        match mem.read_register rx, mem.read_register r with
        |Ptr(addr, dt), v -> mem.mem.[addr] <- v  // TODO: check datatype
        |_ -> failwith "cannot deref non-Ptr register"
      |Lbl name -> mem.code.[mem.labels.[name]] <- lazy mem.read_register r  // TODO: check datatype, code/data integrity
      eval mem.next
    |MovRC(r, x) -> eval (mem.write_register r x).next
    |MovRHandle(r, h) ->
      match h with
      |HandleLbl lbl -> eval (mem.write_register r (Ptr(labels.[lbl], Datatype.Pointer(Datatype.Void, None)))).next
      |HandleReg reg -> eval (mem.write_register r Void).next  // TODO: decipher handle
    |Cmp(r1, r2) -> eval (mem.compare (mem.read_register r1) (mem.read_register r2)).next
    |CmpC(r, x) -> eval (mem.compare (mem.read_register r) x).next
    |Br label -> eval {mem with pc = mem.labels.[label]}
    |Br0 label ->
      match mem.read_register PSR_EQ with
      |Int 0 | Byte 0uy | Int64 0L -> eval {mem.stack_push(Ptr(mem.labels.[label] + CODE_START, Datatype.Void)) with pc = mem.labels.[label]}
      |_ -> eval mem.next
    |BrT label ->
      match mem.read_register PSR_EQ with
      |Int 0 | Byte 0uy | Int64 0L -> eval mem.next
      |_ -> eval {mem.stack_push(Ptr(mem.labels.[label] + CODE_START, Datatype.Void)) with pc = mem.labels.[label]}
    |BrGT label ->
      match mem.read_register PSR_GT with
      |Int 0 | Byte 0uy | Int64 0L -> eval mem.next
      |_ -> eval {mem.stack_push(Ptr(mem.labels.[label] + CODE_START, Datatype.Void)) with pc = mem.labels.[label]}
    |BrLT label ->
      match mem.read_register PSR_LT with
      |Int 0 | Byte 0uy | Int64 0L -> eval mem.next
      |_ -> eval {mem.stack_push(Ptr(mem.labels.[label] + CODE_START, Datatype.Void)) with pc = mem.labels.[label]}
    |Call label -> eval {mem.stack_push(Ptr(mem.labels.[label] + CODE_START, Datatype.Void)) with pc = mem.labels.[label]}
    |Ret ->
      match mem.stack_pop with
      |Ptr(addr, Datatype.Void), mem' when addr >= CODE_START -> eval {mem' with pc = addr - CODE_START}
      |_ -> failwith "failed to return"
    // operations
    |Alloc n -> eval {mem.write_register (R 0) (Int mem.alloc_ptr) with alloc_ptr = mem.alloc_ptr + n}
    |Add(r1, r2) -> eval (mem.write_register r1 (mem.read_register r1 + mem.read_register r2)).next
    // TODO: implement cast here
    |AddC(r, x) -> eval (mem.write_register r (mem.read_register r + x)).next
    |Sub(r1, r2) -> eval (mem.write_register r1 (mem.read_register r1 - mem.read_register r2)).next
    |SubC(r, x) -> eval (mem.write_register r (mem.read_register r - x)).next
    |Mul(r1, r2) -> eval (mem.write_register r1 (mem.read_register r1 * mem.read_register r2)).next
    |MulC(r, x) -> eval (mem.write_register r (mem.read_register r * x)).next
    |DivMod(r1, r2) ->
      let mem' =
        (mem.write_register r1 (mem.read_register r1 / mem.read_register r2))
          .write_register RX (mem.read_register r1 % mem.read_register r2)
      eval mem'.next
    |DivModC(r, x) ->
      let mem' =
        (mem.write_register r (mem.read_register r / x))
          .write_register RX (mem.read_register r % x)
      eval mem'.next
  eval init_memory