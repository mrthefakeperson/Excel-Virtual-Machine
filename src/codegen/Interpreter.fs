module Codegen.Interpreter
open System
open System.Collections.Generic
open Parser.Datatype
open Parser.AST
open Codegen.Tables
open Codegen.PAsm
open Codegen.Hooks
open Codegen.TypeCheck

//let next_ptr = ref 1
//let next_function_ptr = ref 100000

//type Boxed =
//  |Int of int
//  |Int64 of int64
//  |Byte of byte
//  |Float of float32
//  |Double of float
//  |Void
//  |Ptr of addr: int * DT
//   with
//    static member parse (s: string) = function
//      |DT.Int -> Int(int s)
//      |DT.Int64 -> Int64(int64 (s.Replace("L", "")))
//      |DT.Byte ->
//        match s.Replace("'", "").ToByteArray() with
//        |[|x|] -> Byte(byte x)
//        |[|'\\'; esc|] ->
//          match esc with 'n' -> Byte(byte '\n') | '\\' -> Byte(byte '\\') | '0' -> Byte 0uy | _ -> failwithf "bad escape %c" esc
//        |_ -> failwithf "can't parse %s into char" s
//      |DT.Float -> Float(float32 s)
//      |DT.Double -> Double(float s)
//      |DT.Void -> Void
//      |DT.Pointer(dt, _) -> Ptr(int s, dt)
//      |DT.Function _ ->
//        try Ptr(!next_ptr, DT.Void)
//        finally next_ptr := !next_ptr + 1
//      |_ -> failwith "can't parse into boxed value"
//    static member default_value = function
//      |DT.Int -> Int 0
//      |DT.Int64 -> Int64 0L
//      |DT.Byte -> Byte 0uy
//      |DT.Float -> Float 0.f
//      |DT.Double -> Double 0.
//      |DT.Void -> Void
//      |DT.Pointer(dt, _) -> Ptr(0, dt)
//      |DT.Function _ -> Ptr(0, DT.Void)
//      |_ -> failwith "can't get default value"
//    static member check_type assign_target assign_value =
//      match assign_target, assign_value with
//      |Int _, DT.Int | Int64 _, DT.Int64 | Byte _, DT.Byte | Float _, DT.Float
//      |Double _, DT.Double | Void, DT.Void | Ptr(_, DT.Void), DT.Function _ -> ()
//      |Ptr(_, dt), DT.Pointer(dt2, _) when dt = dt2 -> ()
//      |unexpected -> failwithf "failed type check: %A" unexpected
//    static member cast t v =
//      match t, v with
//      |DT.Void, _ -> failwith "can't cast anything to void"
//      |_, Void -> failwith "can't cast void to anything"
//      |DT.Function _, _ -> failwith "can't cast anything to function"
//      |DT.Pointer(dt, _), (Ptr(addr, _) | Int addr) -> Ptr(addr, dt)
//      |DT.Pointer _, _ -> failwithf "can't cast %A to pointer" v
//      |(DT.Int | DT.Int64 | DT.Float | DT.Double),
//       (Int _ | Int64 _ | Byte _ | Float _ | Double _) ->
//        let s =
//          match v with
//          |Int x -> string x | Int64 x -> string x | Byte x -> string x | Float x -> string x | Double x -> string x
//          |_ -> failwith "should never be reached"
//        Boxed.parse s t
//      |DT.Byte, (Int _ | Int64 _ | Byte _ | Float _ | Double _) ->
//        match v with
//        |Int x -> Byte (byte x)
//        |Int64 x -> Byte (byte x)
//        |Byte x -> Byte x
//        |Float x -> Byte (byte x)
//        |Double x -> Byte (byte x)
//        |_ -> failwith "should never be reached"
//      |_, Ptr _ -> failwith "can't cast ptr to other type"
//      |Unknown _, _ -> failwith "can't cast to unknown"
//      |DT.Struct _, _ -> failwith "struct not supported"
//    static member (+) (x, y) =
//      match x, y with
//      |Int x, Int y -> Int(x + y)
//      |Int64 x, Int64 y -> Int64(x + y)
//      |Byte x, Byte y -> Byte(x + y)
//      |Float x, Float y -> Float(x + y)
//      |Double x, Double y -> Double(x + y)
//      |Ptr(addr, dt), Int i | Int i, Ptr(addr, dt) -> Ptr(addr + i * dt.sizeof, dt)
//      |a, b -> failwithf "bad add: (%A) + (%A)" a b
//    static member (-) (x, y) =
//      match x, y with
//      |Int x, Int y -> Int(x - y)
//      |Int64 x, Int64 y -> Int64(x - y)
//      |Byte x, Byte y -> Byte(x - y)
//      |Float x, Float y -> Float(x - y)
//      |Double x, Double y -> Double(x - y)
//      |Ptr(addr, dt), Ptr(addr2, dt2) when dt = dt2 -> Int((addr - addr2) / dt.sizeof)
//      |Ptr(addr, dt), Int i | Int i, Ptr(addr, dt) -> Ptr(addr, dt) + -(Int i)
//      |_ -> failwith "bad sub"
//    static member ( * ) (x, y) =
//      match x, y with
//      |Int x, Int y -> Int(x * y)
//      |Int64 x, Int64 y -> Int64(x * y)
//      |Byte x, Byte y -> Byte(x * y)
//      |Float x, Float y -> Float(x * y)
//      |Double x, Double y -> Double(x * y)
//      |_ -> failwith "bad mul"
//    static member (/) (x, y) =
//      match x, y with
//      |Int x, Int y -> Int(x / y)
//      |Int64 x, Int64 y -> Int64(x / y)
//      |Byte x, Byte y -> Byte(x / y)
//      |Float x, Float y -> Float(x / y)
//      |Double x, Double y -> Double(x / y)
//      |_ -> failwith "bad div"
//    static member (%) (x, y) =
//      match x, y with
//      |Int x, Int y -> Int(x % y)
//      |Int64 x, Int64 y -> Int64(x % y)
//      |Byte x, Byte y -> Byte(x % y)
//      |Float x, Float y -> Float(x % y)
//      |Double x, Double y -> Double(x % y)
//      |_ -> failwith "bad mod"
//    member x.equals y =
//      match x, y with
//      |Int _, Int _ | Int64 _, Int64 _ | Byte _, Byte _ | Float _, Float _ | Double _, Double _ | Ptr _, Ptr _ -> ()
//      |_ -> failwith "type mismatch for equality"
//       TODO: check for same type
//      if x = y then Byte 1uy else Byte 0uy
//    member x.greater_than y =
//      match x, y with
//      |Int x, Int y -> if x > y then Byte 1uy else Byte 0uy
//      |Int64 x, Int64 y -> if x > y then Byte 1uy else Byte 0uy
//      |Byte x, Byte y -> if x > y then Byte 1uy else Byte 0uy
//      |Float x, Float y -> if x > y then Byte 1uy else Byte 0uy
//      |Double x, Double y -> if x > y then Byte 1uy else Byte 0uy
//      |_ -> failwith "type mismatch for equality"
//    member x.less_than y =
//      match x, y with
//      |Int x, Int y -> if x < y then Byte 1uy else Byte 0uy
//      |Int64 x, Int64 y -> if x < y then Byte 1uy else Byte 0uy
//      |Byte x, Byte y -> if x < y then Byte 1uy else Byte 0uy
//      |Float x, Float y -> if x < y then Byte 1uy else Byte 0uy
//      |Double x, Double y -> if x < y then Byte 1uy else Byte 0uy
//      |_ -> failwith "type mismatch for equality"
//    static member (~-) x =
//      match x with
//      |Int x -> Int(-x)
//      |Int64 x -> Int64(-x)
//      |Float x -> Float(-x)
//      |Double x -> Double(-x)
//      |_ -> failwith "bad negative"
//    member x.not =
//      if
//       (
//        match x with
//        |Int x -> x = 0
//        |Int64 x -> x = 0L
//        |Byte x -> x = 0uy
//        |Float x -> x = 0.f
//        |Double x -> x = 0.
//        |_ -> failwith "bad not"
//       )
//       then Byte 1uy else Byte 0uy

exception RaiseReturn of Boxed
exception RaiseContinue
exception RaiseBreak

type Memory = {
  mem: Boxed[]
  var_addrs: Map<string, int>
  next_ptr: int
  next_function_ptr: int ref
  next_free_mem: int ref
  functions: Dictionary<int, Boxed list -> Boxed>
  stdout: string ref
 }

let default_memory() = {
  mem = Array.create 10000 (Int 0)
  var_addrs = Map.empty
  next_ptr = 1
  next_function_ptr = ref 100000
  next_free_mem = ref 1000
  functions = Dictionary()
  stdout = ref ""
 }

let rec get_ref tbl = function
  |Value(Var(name, dt)) -> Ptr(tbl.var_addrs.[name], dt)
  |Apply(Value(Var("*prefix", _)), [b]) as a ->
    ignore (eval_ast tbl a)
    eval_ast tbl b
  |Index(a, i) ->
    eval_ast tbl (Apply(Value(Var("+", TypeClasses.any)), [a; i]))
  |error -> failwithf "bad ref: %A" error
and eval_ast: Memory -> AST -> Boxed = fun tbl -> function
  |Apply(f, args) ->
    match f, args with
    |Value(Var("+", _)), [a; b] -> eval_ast tbl a + eval_ast tbl b
    |Value(Var("-", _)), [a; b] -> eval_ast tbl a - eval_ast tbl b
    |Value(Var("*", _)), [a; b] -> eval_ast tbl a * eval_ast tbl b
    |Value(Var("/", _)), [a; b] -> eval_ast tbl a / eval_ast tbl b
    |Value(Var("%", _)), [a; b] -> eval_ast tbl a % eval_ast tbl b
    |Value(Var("==", _)), [a; b] -> (eval_ast tbl a).equals(eval_ast tbl b)
    |Value(Var(">", _)), [a; b] -> (eval_ast tbl a).greater_than(eval_ast tbl b)
    |Value(Var("<", _)), [a; b] -> (eval_ast tbl a).less_than(eval_ast tbl b)
    |Value(Var("*prefix", _)), [a] ->
      match eval_ast tbl a with
      |Ptr(addr, dt) ->
        match Array.tryItem addr tbl.mem, dt with
        |Some(Int x), DT.Int -> Int x
        |Some(Int64 x), DT.Int64 -> Int64 x
        |Some(Byte x), DT.Byte -> Byte x
        |Some(Float x), DT.Float -> Float x
        |Some(Double x), DT.Double -> Double x
        |_, DT.Void -> failwith "cannot dereference void*"
        |Some _, _ -> failwith "pointer casting unsupported"
        |None, _ -> failwith "invalid address"
      |_ -> failwith "bad deref"
    |Value(Var("&prefix", _)), [a] -> get_ref tbl a
    |Value(Var("\cast", t)), [a] -> Boxed.cast t (eval_ast tbl a)
    |Value(Var("\stack_alloc", DT.Function([DT.Int], DT.Ptr t))), [a] ->
      match Int t.sizeof * eval_ast tbl a with
      |Int n -> try Ptr(!tbl.next_free_mem, t) finally tbl.next_free_mem := !tbl.next_free_mem + n
      |_ -> failwith "should never be reached"
    |Value(Var("printf", _)), fmt::args ->
      match eval_ast tbl fmt with
      |Ptr(addr, DT.Byte) ->
        let fmt_string =
          Seq.unfold (fun addr -> Some (tbl.mem.[addr], addr + 1)) addr
           |> Seq.takeWhile ((<>) (Byte 0uy))
           |> Seq.map (function Byte uy -> string (char uy) | _ -> failwith "can't print that")
           |> String.concat ""
        tbl.stdout := !tbl.stdout + sprintf "%s (fmt %A)" fmt_string (List.map (eval_ast tbl) args)
        if tbl.stdout.Value.Length > 500 then
          raise (RaiseReturn Void)
      |x -> failwithf "can't print non-string: %A -> %A" fmt x
      Void
    |Value(Var(_, DT.Function _)) as fast, args ->
      match eval_ast tbl fast with
      |Ptr(addr, DT.Void) -> tbl.functions.[addr] <| List.map (eval_ast tbl) args
      |unexpected -> failwithf "not a function: boxed value %A" unexpected
    |Value(Var(_, DT.Function2 _)) as fast, args ->
      match eval_ast tbl fast with
      |Ptr(addr, DT.Void) ->
        ignore (List.map (eval_ast tbl) args)
        tbl.functions.[addr] []
      |unexpected -> failwithf "not a function: boxed value %A" unexpected
    |unexpected -> failwithf "not a function: expression %A" unexpected
  |Assign(a, b) ->
    match get_ref tbl a with
    |Ptr(addr, dt) ->
      tbl.mem.[addr] <- eval_ast tbl b
      Boxed.check_type dt tbl.mem.[addr]
      tbl.mem.[addr]
    |_ -> failwith "should never be reached"
  |Index(a, i) -> eval_ast tbl (Apply(Value(Var("*prefix", TypeClasses.any)), [Apply(Value(Var("+", TypeClasses.any)), [a; i])]))
  |Declare _ -> failwith "should never be reached, handled in outer scope"
  |DeclareHelper _ -> failwith "should never be reached"
  |Return x -> raise (RaiseReturn (eval_ast tbl x))
  |Block xprs | GlobalParse xprs as ast ->
    let rec eval_scope tbl = function
      |Declare(name, dt)::rest ->
        let tbl' = {tbl with var_addrs = tbl.var_addrs.Add(name, tbl.next_ptr); next_ptr = tbl.next_ptr + 1}
        tbl'.mem.[tbl.next_ptr] <- Boxed.default_value dt
        eval_scope tbl' rest
      |DeclareHelper xprs::rest -> eval_scope tbl (xprs @ rest)
      |xpr::rest ->
        ignore (eval_ast tbl xpr)
        eval_scope tbl rest
      |[] -> Void, tbl
    let t, tbl' = eval_scope tbl xprs
    match ast with
    |GlobalParse _ when tbl'.var_addrs.ContainsKey "main" ->
      eval_ast tbl' (Apply(Value(Var("main", DT.Function2 DT.Int)), []))
    |_ -> t
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
          acc_tbl.mem.[acc_tbl.next_ptr] <- arg
          Boxed.check_type dt arg
          {acc_tbl with var_addrs = acc_tbl.var_addrs.Add(arg_name, acc_tbl.next_ptr); next_ptr = acc_tbl.next_ptr + 1}
         ) tbl args args'
      try eval_ast tbl' body
      with RaiseReturn x -> x
    tbl.functions.[!tbl.next_function_ptr] <- f
    try Ptr(!tbl.next_function_ptr, DT.Void)
    finally tbl.next_function_ptr := !tbl.next_function_ptr + 1
  |Value(Var(name, _)) ->
    tbl.mem.[tbl.var_addrs.[name]]
  |Value(Lit(_, DT.Void)) -> Void
  |Value(Lit(s, dtype)) -> Option.defaultWith (fun () -> failwith "failed parse") <| Boxed.from_string dtype s

let preprocess_eval_ast ast =
  let mem = default_memory()
  let result =
    try
      ast
       |> apply_mapping_hook transform_sizeof_hook
       |> apply_mapping_hook extract_strings_to_global_hook
       |> apply_mapping_hook convert_logic_hook
       |> apply_mapping_hook prototype_hook
       |> check_type (empty_symbol_table()) |> fst
       |> eval_ast mem
    with RaiseReturn x -> x
  result, !mem.stdout