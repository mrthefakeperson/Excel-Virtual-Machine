module Interpreter.InterpretAST
open System.Collections.Generic
open Utils
open CompilerDatatypes.DT
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SemanticAST
open CompilerDatatypes.Semantics.RegisterAlloc
open CompilerDatatypes.Semantics.InterpreterValue

exception RaiseReturn of Boxed

type Memory = {
  mem: Boxed[]
  var_addrs: Map<string, int>
  next_ptr: int
  next_function_ptr: int ref
  next_free_mem: int ref
  functions: Dictionary<int, Boxed list -> Boxed>
  stdout: string ref
 }
  with
    member x.print s =
      x.stdout := !x.stdout + s
      if x.stdout.Value.Length > 500 then raise (RaiseReturn Void)
    member x.printf s args = x.print (sprintf "%s (fmt %A)\n" s args)
    member x.call fn_addr args ret_dt =
      match fn_addr with
      |0 ->
        x.print "extern call\n"
        Boxed.default_value ret_dt
      |_ -> x.functions.[fn_addr] args

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
  |V (Var(name, (Global dt | Local(_, dt)))) -> Ptr(tbl.var_addrs.[name], dt)
  |Apply(V (Var("*prefix", _)), [b]) -> eval_ast tbl b
  |error -> failwithf "bad ref: %A" error
and eval_ast: Memory -> AST -> Boxed = fun tbl -> function
  |Apply(f, args) ->
    match f, args with
    |V (Var("+", _)), [a; b] -> eval_ast tbl a + eval_ast tbl b
    |V (Var("-", _)), [a; b] -> eval_ast tbl a - eval_ast tbl b
    |V (Var("*", _)), [a; b] -> eval_ast tbl a * eval_ast tbl b
    |V (Var("/", _)), [a; b] -> eval_ast tbl a / eval_ast tbl b
    |V (Var("%", _)), [a; b] -> eval_ast tbl a % eval_ast tbl b
    |V (Var("==", _)), [a; b] -> (eval_ast tbl a).equals(eval_ast tbl b)
    |V (Var(">", _)), [a; b] -> (eval_ast tbl a).greater_than(eval_ast tbl b)
    |V (Var("<", _)), [a; b] -> (eval_ast tbl a).less_than(eval_ast tbl b)
    |V (Var("*prefix", _)), [a] ->
      match eval_ast tbl a with
      |Ptr(addr, dt) ->
        match Array.tryItem addr tbl.mem with
        |Some x when x.datatype = dt -> x
        |Some x -> failwithf "memory contents %A don't match %A" x dt
        |None -> failwith "invalid address"
      |_ -> failwith "bad deref"
    |V (Var("&prefix", _)), [a] -> get_ref tbl a
    |V (Var("\cast", (Global t | Strict t))), [a] -> Boxed.cast t (eval_ast tbl a)
    |V (Var("\stack_alloc", (Global (DT.Function([DT.Int], DT.Ptr t)) | Strict t))), [a] ->
      let (Int n | Strict n) = Int t.sizeof * eval_ast tbl a
      try Ptr(!tbl.next_free_mem, t) finally tbl.next_free_mem := !tbl.next_free_mem + n
    |V (Var("printf", _)), fmt::args ->
      match eval_ast tbl fmt with
      |Ptr(addr, DT.Byte) ->
        let fmt_string =
          Seq.unfold (fun addr -> Some (tbl.mem.[addr], addr + 1)) addr
           |> Seq.takeWhile ((<>) (Byte 0uy))
           |> Seq.map (function Byte uy -> string (char uy) | _ -> failwith "can't print that")
           |> String.concat ""
        tbl.printf fmt_string (List.map (eval_ast tbl) args)
      |x -> failwithf "can't print non-string: %A -> %A" fmt x
      Void
    |V (Var(_, (Global (DT.Function(_, ret_dt)) | Local(_, DT.Function(_, ret_dt))))) as fast, args ->
      match eval_ast tbl fast with
      |Ptr(addr, DT.Void) -> tbl.call addr (List.map (eval_ast tbl) args) ret_dt
      |unexpected -> failwithf "not a function: boxed value %A" unexpected
    |V (Var(_, (Global (DT.Function2 ret_dt) | Local(_, DT.Function2 ret_dt)))) as fast, args ->
      match eval_ast tbl fast with
      |Ptr(addr, DT.Void) ->
        ignore (List.map (eval_ast tbl) args)
        tbl.call addr [] ret_dt
      |unexpected -> failwithf "not a function: boxed value %A" unexpected
    |unexpected -> failwithf "not a function: expression %A" unexpected
  |Assign(a, b) ->
    let (Ptr(addr, dt) | Strict(addr, dt)) = get_ref tbl a
    tbl.mem.[addr] <- eval_ast tbl b
    Boxed.check_type dt tbl.mem.[addr]
    tbl.mem.[addr]
  |Declare _ -> failwith "should never be reached, handled in outer scope"
  |Return x -> raise (RaiseReturn (eval_ast tbl x))
  |Block xprs | GlobalParse xprs as ast ->
    let rec eval_scope tbl = function
      |Declare(name, dt)::rest ->
        let tbl' = {tbl with var_addrs = tbl.var_addrs.Add(name, tbl.next_ptr); next_ptr = tbl.next_ptr + 1}
        tbl'.mem.[tbl.next_ptr] <- Boxed.default_value dt
        eval_scope tbl' rest
      |xpr::rest ->
        ignore (eval_ast tbl xpr)
        eval_scope tbl rest
      |[] -> Void, tbl
    let t, tbl' = eval_scope tbl xprs
    match ast with
    |GlobalParse _ when tbl'.var_addrs.ContainsKey "main" ->
      eval_ast tbl' (Apply(V (Var("main", Global (DT.Function2 DT.Int))), []))
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
  |Function(ret_dt, args, body) ->
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
  |V (Var(name, _)) ->
    tbl.mem.[tbl.var_addrs.[name]]
  |V (Lit value) -> value

let interpret_ast ast =
  let mem = default_memory()
  let result =
    try eval_ast mem ast
    with RaiseReturn x -> x
  (result, !mem.stdout)