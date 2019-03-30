module Codegen.Interpreter
open System
open System.Collections.Generic
open Parser.Datatype
open Parser.AST
open Parser.AST.Hooks
open Codegen.Tables
open Codegen.PAsm
open Codegen.Hooks
open Codegen.TypeCheck

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
  |Apply(Value(Var("*prefix", _)), [b]) -> eval_ast tbl b
  |Index _ -> failwith "should never be reached"
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
        match Array.tryItem addr tbl.mem with
        |Some x when x.datatype = dt -> x
        |Some x -> failwithf "memory contents %A don't match %A" x dt
        |None -> failwith "invalid address"
      |_ -> failwith "bad deref"
    |Value(Var("&prefix", _)), [a] -> get_ref tbl a
    |Value(Var("\cast", t)), [a] -> Boxed.cast t (eval_ast tbl a)
    |Value(Var("\stack_alloc", DT.Function([DT.Int], DT.Ptr t))), [a] ->
      let (Int n | Strict n) = Int t.sizeof * eval_ast tbl a
      try Ptr(!tbl.next_free_mem, t) finally tbl.next_free_mem := !tbl.next_free_mem + n
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
    let (Ptr(addr, dt) | Strict(addr, dt)) = get_ref tbl a
    tbl.mem.[addr] <- eval_ast tbl b
    Boxed.check_type dt tbl.mem.[addr]
    tbl.mem.[addr]
  |Index _ -> failwith "should never be reached"
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
       |> apply_mapping_hook convert_index_hook
       |> check_type (empty_symbol_table()) |> fst
       |> apply_mapping_hook scale_ptr_offsets_hook
       |> eval_ast mem
    with RaiseReturn x -> x
  result, !mem.stdout