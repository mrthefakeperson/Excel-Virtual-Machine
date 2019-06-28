module Transformers.SimplifyAST
open Utils
open CompilerDatatypes.Token
open CompilerDatatypes.DT
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SyntaxAST

open Hooks

// transform sizeof(DT) to literal
let eval_sizeof_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Apply(V (Var("sizeof", _)), [V (Var(dt, _))]) ->
    let datatype =
      match dt with
      |"int" -> Int | "long" -> Int64 | "char" -> Byte | "float" -> Float | "double" -> Double
      |_ -> failwithf "sizeof %A not found" dt
    Some (V (Lit(string datatype.sizeof, Int)))
  |Apply(V (Var("sizeof", _)), _) -> failwith "invalid sizeof expression"
  |_ -> None

let label_of = Seq.map (sprintf "%x" << int) >> String.concat "_" >> (+) "$"

// get string literals
let find_strings ast : (string * string) list =
  let mutable acc_strings = Map.empty
  let find_strings_hook: MappingASTHook = fun (|P|) (|Q|) -> function
    |V (Lit(Regex RegexUtils.STRING _ as s, Ptr Byte)) ->
      let raw_string = s.[1..s.Length - 2]
      acc_strings <- Map.add (label_of raw_string) raw_string acc_strings
      None
    |_ -> None
  ignore (apply_mapping_hook find_strings_hook ast)
  List.sortBy snd (Map.toList acc_strings)

// add string literals to global scope
let extract_strings_to_global_hook: MappingASTHook =
  fun (|P|) (|Q|) -> function
    |GlobalParse(Q xprs) as ast ->
      let decls =
        find_strings ast
         |> List.collect (fun (vname, str_value) ->
              let length = str_value.Length + 1  // + 1 for null terminator
              // TODO: data_alloc instead of stack_alloc
              let alloc = BuiltinASTs.stack_alloc Byte [V (Lit(string length, Int))]
              let assigns =
                List.ofArray (str_value.ToCharArray()) @ [char 0]
                 |> List.mapi (fun i c ->
                      Assign(BuiltinASTs.index (V (Var(vname, TypeClasses.any))) (V (Lit(string i, Int))),
                        V (Lit((match c with '\000' -> "0" | _ -> sprintf "'%c'" c), Byte))
                       )
                     )
              Declare(vname, Ptr Byte)::Assign(V (Var(vname, Ptr Byte)), alloc)::assigns
             )
      Some(GlobalParse(decls @ xprs))
    |V (Lit(Regex RegexUtils.STRING _ as s, Ptr Byte)) ->
      Some(V (Var(label_of s.[1..s.Length - 2], TypeClasses.any)))
    |_ -> None

// implement prototyping by removing all but the first declaration of anything
let prototype_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |GlobalParse xprs ->
    List.fold (fun (acc, acc_decls) -> function
      |Declare(name, _) when Set.exists ((=) name) acc_decls -> (acc, acc_decls)
      |Declare(name, _) as xpr -> (xpr::acc, Set.add name acc_decls)
      |xpr -> (xpr::acc, acc_decls)
     ) ([], Set.empty) xprs
     |> (Some << GlobalParse << List.rev << fst)
  |_ -> None

// convert logic functions to a subset to simplify code generation
let convert_logic_hook: MappingASTHook =
  let apply infix a b = Apply(V (Var(infix, TypeClasses.f_logic_infix)), [a; b])
  let apply_not a = apply "==" a (V (Lit("0", Byte)))
  let (|Builtin|_|) x = function Apply(V (Var(x', _)), args) when x = x' -> Some args | _ -> None
  fun (|P|) (|Q|) -> function
    |Builtin "&&" [a; b] -> Some (If(a, b, V (Lit("0", Byte))))
    |Builtin "||" [a; b] -> Some (If(a, V (Lit("1", Byte)), b))
    |Builtin "!=" [a; b] -> Some (apply_not (apply "==" a b))
    |Builtin "<=" [a; b] -> Some (apply_not (apply ">" a b))
    |Builtin ">=" [a; b] -> Some (apply_not (apply "<" a b))
    |Builtin "!" [a] -> Some (apply_not a)
    |_ -> None

// a->b <=> (*a).b
let convert_arrow_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Apply(V (Var("->", _)), [x; field]) ->
    let deref = Apply'.fn("*prefix", DT.Function([Ptr TypeClasses.any], TypeClasses.any))
    Some (Apply'.fn2 "." (deref x) field)
  |_ -> None

// arrays -> pointers (including multidimensional)
let convert_arrays_hook: MappingASTHook =
  let rec replace = function
    |TypeDef (Array(1, dt)) -> Ptr dt
    |TypeDef (Array _) -> failwith "multidimensional arrays not supported"
    |Ptr dt -> Ptr (replace dt)
    |DT.Function(args, ret) -> DT.Function(List.map replace args, replace ret)
    |Function2 ret -> Function2 (replace ret)
    |dt -> dt  // TODO: arrays inside structs
  fun (|P|) (|Q|) -> function
    |Declare(v, dt) -> Some (Declare(v, replace dt))
    |V (Var(v, dt)) -> Some (V (Var(v, replace dt)))
    |_ -> None

// static globals are treated as normal globals
// static locals copy a global and write it back at the end of scope
let create_static_vars ast : AST =
  let mutable gvar = 0
  let mutable all_gvars = []
  let next_gvar dt =
    let gvar_name = "$$static_var_" + string gvar
    let flag_name = "$$static_flag_" + string gvar
    gvar <- gvar + 1
    all_gvars <-
      Declare(gvar_name, dt)
       :: Declare(flag_name, Byte)  // initialize a flag
       :: Assign(V (Var(flag_name, Byte)), V (Lit("1", Byte)))
       :: all_gvars
    (V (Var(gvar_name, dt)), V (Var(flag_name, Byte)))
  apply_mapping_hook (fun (|P|) (|Q|) -> function
    |GlobalParse (Q xprs) ->
      let xprs = List.collect (function BuiltinASTs.GetStatic(_, init) -> init | x -> [x]) xprs
      Some (GlobalParse (all_gvars @ xprs))
    |Block (Q xprs) ->
      let (xprs, writeback) =
        List.mapFold (fun acc -> function
          |BuiltinASTs.GetStatic(V (Var(_, dt)) as var, init) ->
            let (gvar, init_flag) = next_gvar dt
            let initialize = init @ [Assign(gvar, var); Assign(init_flag, V (Lit("0", Byte)))]
            let write_init = If(init_flag, Block initialize, Block [Assign(var, gvar)])
            (write_init, Assign(gvar, var)::acc)
          |BuiltinASTs.GetStatic _ & Strict x -> x
          |ast -> (ast, acc)
         ) [] xprs
      Some (Block (xprs @ writeback))
    |_ -> None
   ) ast
  
let pre_inference: AST -> AST =
  List.map apply_mapping_hook [
    eval_sizeof_hook
    extract_strings_to_global_hook
    prototype_hook
    convert_logic_hook
    convert_arrow_hook
    convert_arrays_hook
   ]
   @ [create_static_vars]
   |> List.reduce (>>)

open CompilerDatatypes.Semantics.RegisterAlloc
open CompilerDatatypes.Semantics.InterpreterValue
open CompilerDatatypes.AST.SemanticAST

// (a: Ptr int)[i: int] -> *(a + (Ptr int)i) -> *(a + 4 * i)
// gets run after type inference hook - all types should be concrete
let scale_ptr_offsets_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Apply(V (Var("\cast", Global (DT.Ptr dt as ptr))), [P x]) ->
    let deref_dt = DT.Function([ptr; ptr], ptr)
    let sizeof = Boxed.Ptr(dt.sizeof, dt)
    Some <|
      Apply(V (Var("*", Global deref_dt)), [
        V (Lit sizeof)
        BuiltinASTs.cast ptr x
       ])
  |_ -> None

let post_inference: AST -> AST =
  List.map apply_mapping_hook [
    scale_ptr_offsets_hook
   ]
   |> List.reduce (>>)