module Codegen.Hooks
// module for preprocessing functionality during AST -> Pseudo-Asm stage
open Parser.Datatype
open Parser.AST
open System.Text.RegularExpressions

// active pattern mapping:
// takes (|P|): AST -> 'state such that, when P x is matched, x is the state returned by recursive application of the hook (Q x is for lists)
type 'state ASTHook = (AST -> 'state) -> (AST list -> 'state list) -> AST -> 'state
// eg. apply_hook (fun (|P|) (|Q|) -> function Return (P x) -> x | Block (Q xs) -> List.sum xs | _ -> 1)
//   (the hook in this case produces an int state out of an AST when applied)
let rec apply_hook (hook: 'state ASTHook) (ast: AST) : 'state =
  hook (apply_hook hook) (List.map (apply_hook hook)) ast

// extension of above - function which optionally returns AST ('state = AST Option) and uses default behavior for None
type MappingASTHook = (AST -> AST) -> (AST list -> AST list) -> AST -> AST Option
let rec apply_mapping_hook (hook: MappingASTHook) : AST -> AST = fun ast ->
  match hook (apply_mapping_hook hook) (List.map (apply_mapping_hook hook)) ast with
  |Some x -> x
  |None ->
    match ast with
    |Value _ | Declare _ -> ast
    |Apply(f, args) -> Apply(apply_mapping_hook hook f, List.map (apply_mapping_hook hook) args)
    |Assign(l, r) -> Assign(apply_mapping_hook hook l, apply_mapping_hook hook r)
    |Index(a, i) -> Index(apply_mapping_hook hook a, apply_mapping_hook hook i)
    |DeclareHelper decls -> DeclareHelper(List.map (apply_mapping_hook hook) decls)
    |Return x -> Return(apply_mapping_hook hook x)
    |Block xprs -> Block(List.map (apply_mapping_hook hook) xprs)
    |If(cond, thn, els) -> If(apply_mapping_hook hook cond, apply_mapping_hook hook thn, apply_mapping_hook hook els)
    |While(cond, body) -> While(apply_mapping_hook hook cond, apply_mapping_hook hook body)
    |Function(args, body) -> Function(args, apply_mapping_hook hook body)
    |GlobalParse xprs -> GlobalParse(List.map (apply_mapping_hook hook) xprs)

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
let get_raw_string (s: string) =
  let rec escapes = function
    |'\\'::rest ->
      match rest with
      |'n'::rest -> '\n'::rest
      |'t'::rest -> '\t'::rest
      |'0'::rest -> '\000'::rest
      |'\\'::rest -> '\\'::rest
      |'"'::rest -> '\"'::rest
      |_ -> failwithf "unsupported escape: %A" rest
    |hd::rest -> hd::escapes rest
    |[] -> []
  s.[1..s.Length - 2]
   |> Seq.toList
   |> escapes
   |> Seq.map string
   |> String.concat ""

// get string literals
let find_strings_hook: (string * string) seq ASTHook = fun (|P|) (|Q|) -> function
  |Value(Lit(s, Ptr Byte)) when Regex.Match(s, "\"(\\\"|[^\"])*\"").Value = s ->
    let raw_string = get_raw_string s
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
              let alloc = Apply(Value(Var("\stack_alloc", DT.Function([Int], Ptr Byte))), [Value(Lit(string length, Int))])
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
      Some(Value(Var(label_from_string (get_raw_string s), TypeClasses.any)))
    |_ -> None

// convert logic functions to a subset to simplify code generation
let convert_logic_hook: MappingASTHook =
  let apply infix a b = Apply(Value(Var(infix, TypeClasses.any)), [a; b])
  let apply_not a = apply "==" a (Value(Lit("\\0", Byte)))
  let (|Builtin|_|) x = function Apply(Value(Var(x', _)), args) when x = x' -> Some args | _ -> None
  fun (|P|) (|Q|) -> function
    |Builtin "&&" [a; b] -> Some (If(a, b, Value(Lit("\\0", Byte))))
    |Builtin "||" [a; b] -> Some (If(a, Value(Lit("\\0", Byte)), b))
    |Builtin "!=" [a; b] -> Some (apply_not (apply "==" a b))
    |Builtin "<=" [a; b] -> Some (apply_not (apply ">" a b))
    |Builtin ">=" [a; b] -> Some (apply_not (apply "<" a b))
    |Builtin "!" [a] -> Some (apply_not a)
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