module Transformers.SimplifyAST
open Utils
open CompilerDatatypes.Token
open CompilerDatatypes.DT
open CompilerDatatypes.AST
open CompilerDatatypes.AST.Hooks

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
  let acc_strings = ref Map.empty
  let find_strings_hook: MappingASTHook = fun (|P|) (|Q|) -> function
    |V (Lit(Regex RegexUtils.STRING _ as s, Ptr Byte)) ->
      let raw_string = s.[1..s.Length - 2]
      acc_strings := Map.add (label_of raw_string) raw_string !acc_strings
      None
    |_ -> None
  ignore (apply_mapping_hook find_strings_hook ast)
  List.sortBy snd (Map.toList !acc_strings)

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

// implement prototyping by removing all but the first declaration of anything - keep the assigns which follow
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
  let apply infix a b = Apply(V (Var(infix, TypeClasses.any)), [a; b])
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
  |Apply(V (Var("->", _)), [x; field]) -> Some (Apply'.fn2 "." (Apply'.fn "*" x) field)
  |_ -> None

// arrays -> pointers (including multidimensional)
let convert_arrays_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Declare(v, TypeDef (Array(1, dt))) -> Some (Declare(v, Ptr dt))
  |V (Var(v, TypeDef (Array(1, dt)))) -> Some (V (Var(v, Ptr dt)))
  |Declare(_, TypeDef (Array(_, _)))
  |V (Var(_, TypeDef (Array(_, _)))) -> failwith "multidimensional arrays not currently supported"
  |_ -> None
  
let pre_inference =
  List.map apply_mapping_hook [
    eval_sizeof_hook
    extract_strings_to_global_hook
    prototype_hook
    convert_logic_hook
    convert_arrow_hook
    convert_arrays_hook
   ]
   |> List.reduce (>>)

// (a: Ptr int)[i: int] -> *(a + (Ptr int)i) -> *(a + 4 * i)
// gets run after type inference hook - all types should be concrete
let scale_ptr_offsets_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Apply(V (Var("\cast", Ptr t)), [P x]) ->
    Some <|
      Apply'.fn2("*", DT.Function([Ptr t; Ptr t], Ptr t))
       (V (Lit(string t.sizeof, Ptr t)))
       (BuiltinASTs.cast (Ptr t) x)
  |_ -> None

let post_inference =
  List.map apply_mapping_hook [
    scale_ptr_offsets_hook
   ]
   |> List.reduce (>>)