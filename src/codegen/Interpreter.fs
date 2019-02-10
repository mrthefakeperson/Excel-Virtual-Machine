module Codegen.Interpreter
open System
open System.Collections.Generic
open Parser.AST
open Codegen.PAsm

let next_ptr = ref 1
let next_function_ptr = ref 100000

type Boxed =
  |Int of int
  |Int64 of int64
  |Byte of byte
  |Float of float32
  |Double of float
  |Void
  |Ptr of addr: int * Datatype
   with
    static member parse (s: string) = function
      |Datatype.Int -> Int(int s)
      |Datatype.Long -> Int64(int64 (s.Replace("L", "")))
      |Datatype.Char -> Byte(byte <| char(s.Replace("'", "")))
      |Datatype.Float -> Float(float32 s)
      |Datatype.Double -> Double(float s)
      |Datatype.Void -> Void
      |Datatype.Pointer(dt, _) -> Ptr(int s, dt)
      |Datatype.Function _ ->
        try Ptr(!next_ptr, Datatype.Void)
        finally next_ptr := !next_ptr + 1
      |_ -> failwith "can't parse into boxed value"
    static member default_value = function
      |Datatype.Int -> Int 0
      |Datatype.Long -> Int64 0L
      |Datatype.Char -> Byte 0uy
      |Datatype.Float -> Float 0.f
      |Datatype.Double -> Double 0.
      |Datatype.Void -> Void
      |Datatype.Pointer(dt, _) -> Ptr(0, dt)
      |Datatype.Function _ -> Ptr(0, Datatype.Void)
      |_ -> failwith "can't get default value"
    static member check_type assign_target assign_value =
      match assign_target, assign_value with
      |Int _, Datatype.Int | Int64 _, Datatype.Long | Byte _, Datatype.Char | Float _, Datatype.Float
      |Double _, Datatype.Double | Void, Datatype.Void | Ptr(_, Datatype.Void), Datatype.Function _ -> ()
      |Ptr(_, dt), Datatype.Pointer(dt2, _) when dt = dt2 -> ()
      |unexpected -> failwithf "failed type check: %A" unexpected
    static member cast t v =
      match t, v with
      |Datatype.Void, _ -> failwith "can't cast anything to void"
      |_, Void -> failwith "can't cast void to anything"
      |Datatype.Function _, _ -> failwith "can't cast anything to function"
      |Datatype.Pointer(dt, _), (Ptr(addr, _) | Int addr) -> Ptr(addr, dt)
      |Datatype.Pointer _, _ -> failwithf "can't cast %A to pointer" v
      |(Datatype.Char | Datatype.Int | Datatype.Long | Datatype.Float | Datatype.Double),
       (Int _ | Int64 _ | Byte _ | Float _ | Double _) ->
        let s =
          match v with
          |Int x -> string x | Int64 x -> string x | Byte x -> string x | Float x -> string x | Double x -> string x
          |_ -> failwith "should never be reached"
        Boxed.parse s t
      |_, Ptr _ -> failwith "can't cast ptr to other type"
      |Unknown _, _ -> failwith "can't cast to unknown"
      |Datatype.Struct _, _ -> failwith "struct not supported"
    static member (+) (x, y) =
      match x, y with
      |Int x, Int y -> Int(x + y)
      |Int64 x, Int64 y -> Int64(x + y)
      |Byte x, Byte y -> Byte(x + y)
      |Float x, Float y -> Float(x + y)
      |Double x, Double y -> Double(x + y)
      |Ptr(addr, dt), Int i | Int i, Ptr(addr, dt) -> Ptr(addr + i * dt.sizeof, dt)
      |_ -> failwith "bad add"
    static member (-) (x, y) =
      match x, y with
      |Int x, Int y -> Int(x - y)
      |Int64 x, Int64 y -> Int64(x - y)
      |Byte x, Byte y -> Byte(x - y)
      |Float x, Float y -> Float(x - y)
      |Double x, Double y -> Double(x - y)
      |Ptr(addr, dt), Ptr(addr2, dt2) when dt = dt2 -> Int((addr - addr2) / dt.sizeof)
      |_ -> failwith "bad sub"
    static member ( * ) (x, y) =
      match x, y with
      |Int x, Int y -> Int(x * y)
      |Int64 x, Int64 y -> Int64(x * y)
      |Byte x, Byte y -> Byte(x * y)
      |Float x, Float y -> Float(x * y)
      |Double x, Double y -> Double(x * y)
      |_ -> failwith "bad mul"
    static member (/) (x, y) =
      match x, y with
      |Int x, Int y -> Int(x / y)
      |Int64 x, Int64 y -> Int64(x / y)
      |Byte x, Byte y -> Byte(x / y)
      |Float x, Float y -> Float(x / y)
      |Double x, Double y -> Double(x / y)
      |_ -> failwith "bad div"
    static member (%) (x, y) =
      match x, y with
      |Int x, Int y -> Int(x % y)
      |Int64 x, Int64 y -> Int64(x % y)
      |Byte x, Byte y -> Byte(x % y)
      |Float x, Float y -> Float(x % y)
      |Double x, Double y -> Double(x % y)
      |_ -> failwith "bad mod"
    member x.equals y =
      // TODO: check for same type
      if x = y then Byte 1uy else Byte 0uy
    static member (~-) x =
      match x with
      |Int x -> Int(-x)
      |Int64 x -> Int64(-x)
      |Float x -> Float(-x)
      |Double x -> Double(-x)
      |_ -> failwith "bad negative"
    member x.not =
      if
       (
        match x with
        |Int x -> x = 0
        |Int64 x -> x = 0L
        |Byte x -> x = 0uy
        |Float x -> x = 0.f
        |Double x -> x = 0.
        |_ -> failwith "bad not"
       )
       then Byte 1uy else Byte 0uy

exception RaiseReturn of Boxed
exception RaiseContinue
exception RaiseBreak

type Memory = {
  mem: Boxed[]
  var_addrs: Map<string, int>
  functions: Dictionary<int, Boxed list -> Boxed>
 }

let default_memory() = {
  mem = Array.create 10000 (Int 0)
  var_addrs = Map.empty
  functions = Dictionary()
 }

let rec get_ref tbl = function
  |Value(Var(name, dt)) -> Ptr(tbl.var_addrs.[name], dt)
  |Apply(Value(Var("*prefix", _)), [b]) as a ->
    ignore (eval_ast tbl a)
    eval_ast tbl b
  |_ -> failwith "bad ref"
and eval_ast (tbl: Memory) : AST -> Boxed = function
  |Apply(f, args) ->
    match f, args with
    |Value(Var("+", _)), [a; b] -> eval_ast tbl a + eval_ast tbl b
    |Value(Var("-", _)), [a; b] -> eval_ast tbl a - eval_ast tbl b
    |Value(Var("*", _)), [a; b] -> eval_ast tbl a * eval_ast tbl b
    |Value(Var("/", _)), [a; b] -> eval_ast tbl a / eval_ast tbl b
    |Value(Var("%", _)), [a; b] -> eval_ast tbl a % eval_ast tbl b
    |Value(Var("==", _)), [a; b] -> (eval_ast tbl a).equals(eval_ast tbl b)
    |Value(Var("-prefix", _)), [a] -> -(eval_ast tbl a)
    |Value(Var("!", _)), [a] -> (eval_ast tbl a).not
    |Value(Var("*prefix", _)), [a] ->
      match eval_ast tbl a with
      |Ptr(addr, dt) ->
        match Array.tryItem addr tbl.mem, dt with
        |Some(Int x), Datatype.Int -> Int x
        |Some(Int64 x), Datatype.Long -> Int64 x
        |Some(Byte x), Datatype.Char -> Byte x
        |Some(Float x), Datatype.Float -> Float x
        |Some(Double x), Datatype.Double -> Double x
        |_, Datatype.Void -> failwith "cannot dereference void*"
        |Some _, _ -> failwith "pointer casting unsupported"
        |None, _ -> failwith "invalid address"
      |_ -> failwith "bad deref"
    |Value(Var("&prefix", _)), [a] -> get_ref tbl a
    |Value(Var("\cast", t)), [a] -> Boxed.cast t (get_ref tbl a)
    |Value(Var _) as fast, args ->
      match eval_ast tbl fast with
      |Ptr(addr, Datatype.Void) -> tbl.functions.[addr] <| List.map (eval_ast tbl) args
      |unexpected -> failwithf "not a function: boxed value %A" unexpected
    |unexpected -> failwithf "not a function: expression %A" unexpected
  |Assign(a, b) ->
    match get_ref tbl a with
    |Ptr(addr, dt) ->
      tbl.mem.[addr] <- eval_ast tbl b
      Boxed.check_type tbl.mem.[addr] dt
      tbl.mem.[addr]
    |_ -> failwith "should never be reached"
  |Index(a, i) -> eval_ast tbl (Apply(Value(Var("*prefix", Unknown[])), [Apply(Value(Var("+", Unknown[])), [a; i])]))
  |Declare _ -> failwith "should never be reached, handled in outer scope"
  |DeclareHelper _ -> failwith "should never be reached"
  |Return x -> raise (RaiseReturn(eval_ast tbl x))
  |Block xprs
  |GlobalParse xprs ->
    let rec eval_scope tbl = function
      |Declare(name, dt)::rest ->
        let tbl' = {tbl with var_addrs = tbl.var_addrs.Add(name, !next_ptr)}
        tbl'.mem.[!next_ptr] <- Boxed.default_value dt
        incr next_ptr
        try eval_scope tbl' rest
        finally decr next_ptr
      |DeclareHelper xprs::rest -> eval_scope tbl (xprs @ rest)
      |xpr::rest ->
        ignore (eval_ast tbl xpr)
        eval_scope tbl rest
      |[] -> Void
    eval_scope tbl xprs
  |If(cond, thn, els) ->
    match eval_ast tbl cond with
    |Byte 0uy -> eval_ast tbl els
    |Byte _ -> eval_ast tbl thn
    |_ -> failwith "invalid condition in 'if'"
  |While(cond, body) ->
    while
     (match eval_ast tbl cond with Byte x -> x <> 0uy | _ -> failwith "invalid condition in 'while'")
     do ignore <| eval_ast tbl body
    Void
  |Function(args, body) ->
    let f (args': Boxed list) =
      if args'.Length <> args.Length then failwith "wrong args length"
      let tbl' =
        List.fold2 (fun acc_tbl (arg_name, dt) arg ->
          tbl.mem.[!next_ptr] <- arg
          Boxed.check_type arg dt
          incr next_ptr
          {acc_tbl with var_addrs = acc_tbl.var_addrs.Add(arg_name, !next_ptr - 1)}
         ) tbl args args'
      try
        try eval_ast tbl' body
        finally next_ptr := !next_ptr - args.Length
      with RaiseReturn x -> x
    tbl.functions.[!next_function_ptr] <- f
    try Ptr(!next_function_ptr, Datatype.Void)
    finally next_function_ptr := !next_function_ptr + 1
  |Value(Var(name, _)) ->
    tbl.mem.[tbl.var_addrs.[name]]
  |Value Unit -> Void
  |Value(Lit(s, dtype)) -> Boxed.parse s dtype


let NUM_REAL_REGS = 4
let STACK_START = 100000

type AsmMemory = {
  code: Lazy<Boxed>[]
  pc: int
  reg_sp: Boxed
  reg_bp: Boxed
  real_regs: Boxed[]
  labels: Map<string, int>
  stack: Boxed[]
  mem: Boxed[]
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

    member x.stack_push v =
      match x.read_register SP with Ptr(addr, _) -> x.mem.[addr] <- v | _ -> failwith "SP should be a Ptr"
      x.write_register SP (x.read_register SP + Int 1)

    member x.stack_pop =
      match x.read_register SP with
      |Ptr(addr, _) when addr - 1 < STACK_START -> failwith "stack underflow?"
      |Ptr(addr, _) -> x.mem.[addr], x.write_register SP (x.read_register SP - Int 1)
      |_ -> failwith "SP should be a Ptr"

    member x.all_real_regs = BP::SP::R 0::RX::[for n in 1..NUM_REAL_REGS - 1 -> R n]
    member x.next = {x with pc = x.pc + 1}

let init_memory = {
  code = [||]
  pc = 0
  reg_sp = Ptr(STACK_START, Datatype.Void)
  reg_bp = Ptr(STACK_START, Datatype.Void)
  real_regs = Array.create NUM_REAL_REGS Void
  labels = Map.empty
  stack = Array.create 10000 Void
  mem = Array.create 10000 Void
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
    |Br label -> eval {mem with pc = mem.labels.[label]}
    // TODO: consider flag?
    |Br0 label ->
      match mem.read_register (R 0) with
      |Int 0 | Byte 0uy | Int64 0L -> eval {mem.stack_push(Ptr(mem.labels.[label] + 200000, Datatype.Void)) with pc = mem.labels.[label]}
      |_ -> eval mem.next
    |BrT label ->
      match mem.read_register (R 0) with
      |Int 0 | Byte 0uy | Int64 0L -> eval mem.next  // TODO: cast
      |_ -> eval {mem.stack_push(Ptr(mem.labels.[label] + 200000, Datatype.Void)) with pc = mem.labels.[label]}
    |Call label -> eval {mem.stack_push(Ptr(mem.labels.[label] + 200000, Datatype.Void)) with pc = mem.labels.[label]}
    |Ret ->
      match mem.stack_pop with
      |Ptr(addr, Datatype.Void), mem' when addr >= 200000 -> eval {mem' with pc = addr - 200000}
      |_ -> failwith "failed to return"
    // operations
    |Add(r1, r2) -> eval (mem.write_register r1 (mem.read_register r1 + mem.read_register r2)).next
    |AddC(r, x) -> eval (mem.write_register r (mem.read_register r + x)).next

  eval init_memory