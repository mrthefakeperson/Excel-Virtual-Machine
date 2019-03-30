module Codegen.Hooks
// module for preprocessing functionality during AST -> Pseudo-Asm stage
open Parser.Datatype
open Parser.AST
open Parser.AST.Hooks
open System.Text.RegularExpressions

// transform sizeof(DT) to literal
let transform_sizeof_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Apply(Value(Var("sizeof", _)), [Value(Var(dt, _))]) ->
    let datatype =
      match dt with
      |"int" -> Int | "long" -> Int64 | "char" -> Byte | "float" -> Float | "double" -> Double
      |_ -> failwithf "sizeof %A not found" dt
    Some (Value(Lit(string datatype.sizeof, Int)))
  |Apply(Value(Var("sizeof", _)), _) -> failwith "invalid sizeof expression"
  |_ -> None

let label_from_string: string -> string = Seq.map (sprintf "%x" << int) >> String.concat "_" >> sprintf "$%s"

// get string literals
let find_strings_hook: (string * string) seq ASTHook = fun (|P|) (|Q|) -> function
  |Value(Lit(s, Ptr Byte)) when Regex.Match(s, "\"(\\\"|[^\"])*\"").Value = s ->
    let raw_string = unescape_string s
    seq [label_from_string raw_string, raw_string]
  |Value _ | Declare _ -> Seq.empty
  |Apply(P strshd, Q strstl) -> Seq.singleton strshd |> Seq.append strstl |> Seq.concat
  |Assign(P strs1, P strs2) | Index(P strs1, P strs2) | While(P strs1, P strs2) -> Seq.append strs1 strs2
  |DeclareHelper(Q strlst) | Block(Q strlst) | GlobalParse(Q strlst) -> Seq.concat strlst
  |Return(P strs) | Function(_, P strs) -> strs
  |If(P strs1, P strs2, P strs3) -> Seq.concat [strs1; strs2; strs3]

// add string literals to global scope
let extract_strings_to_global_hook: MappingASTHook =
  fun (|P|) (|Q|) -> function
    |GlobalParse(Q xprs) as ast ->
      let decls =
        apply_hook find_strings_hook ast
         |> Map.ofSeq
         |> Map.toList
         |> List.sortBy snd
         |> List.collect (fun (vname, str_value) ->
              let length = str_value.Length + 1  // + 1 for null terminator
              // TODO: data_alloc instead of stack_alloc
              let alloc = BuiltinASTs.stack_alloc Byte (Value(Lit(string length, Int)))
              let assigns =
                List.ofArray (str_value.ToCharArray()) @ [char 0]
                 |> List.mapi (fun i c ->
                      Assign(Index(Value(Var(vname, TypeClasses.any)), Value(Lit(string i, Int))),
                        Value(Lit((match c with '\000' -> "'\\0'" | _ -> sprintf "'%c'" c), Byte))
                       )
                     )
              Declare(vname, Ptr Byte)::Assign(Value(Var(vname, Ptr Byte)), alloc)::assigns
             )
      Some(GlobalParse(decls @ xprs))
    |Value(Lit(s, (Ptr Byte))) when Regex.Match(s, "\"(\\\"|[^\"])*\"").Value = s ->
      Some(Value(Var(label_from_string (unescape_string s), TypeClasses.any)))
    |_ -> None

// convert logic functions to a subset to simplify code generation
let convert_logic_hook: MappingASTHook =
  let apply infix a b = Apply(Value(Var(infix, TypeClasses.any)), [a; b])
  let apply_not a = apply "==" a (Value(Lit("0", Byte)))
  let (|Builtin|_|) x = function Apply(Value(Var(x', _)), args) when x = x' -> Some args | _ -> None
  fun (|P|) (|Q|) -> function
    |Builtin "&&" [a; b] -> Some (If(a, b, Value(Lit("\\0", Byte))))
    |Builtin "||" [a; b] -> Some (If(a, Value(Lit("\\0", Byte)), b))
    |Builtin "!=" [a; b] -> Some (apply_not (apply "==" a b))
    |Builtin "<=" [a; b] -> Some (apply_not (apply ">" a b))
    |Builtin ">=" [a; b] -> Some (apply_not (apply "<" a b))
    |Builtin "!" [a] -> Some (apply_not a)
    |_ -> None

// Index(a, i) -> *(a + i)
let convert_index_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Index(a, i) -> Some <| Apply'.fn "*prefix" (Apply'.fn2 "+" a i)
  |_ -> None

// (a: Ptr int)[i: int] -> *(a + (Ptr int)i) -> *(a + 4 * i)
// gets run after type inference hook - all types should be concrete
let scale_ptr_offsets_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |Apply(Value(Var("\cast", Ptr t)), [P x]) ->
    Some <| Apply'.fn2("*", DT.Function([Ptr t; Ptr t], Ptr t)) (Value(Lit(string t.sizeof, Ptr t))) (BuiltinASTs.cast (Ptr t) x)
  |_ -> None

// reassign existing functions at the global level to implement prototyping
let prototype_hook: MappingASTHook = fun (|P|) (|Q|) -> function
  |GlobalParse xprs ->
    List.collect (function DeclareHelper xprs -> xprs | xpr -> [xpr]) xprs
     |> List.fold (fun (acc, acc_decls) -> function
          |Declare(name, _) when Set.exists ((=) name) acc_decls -> (acc, acc_decls)
          |Declare(name, _) as xpr -> (xpr::acc, Set.add name acc_decls)
          |xpr -> (xpr::acc, acc_decls)
         ) ([], Set.empty)
     |> (Some << GlobalParse << List.rev << fst)
  |_ -> None