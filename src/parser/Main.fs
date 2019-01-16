module Parser.Main
//open System.Collections.Generic
open System.Text.RegularExpressions
open Parser.AST
open Parser.Combinators

let complete_match rgx s = Regex.Match(s, rgx).Value = s

//let astx_to_ast_converters: List<ASTx -> AST option> = ResizeArray()
//let convert_astx_to_ast astx =
//  Seq.pick (fun converter -> converter astx) astx_to_ast_converters

let t_any = Unknown []
let t_numeric = Unknown [Int; Char; Long]

let _var = Match "[a-z A-Z _][a-z 0-9 A-Z _]*" ->/ fun s -> Value(Variable(s, t_any))
let _string = Match "\"(\\\"|[^\"])*\"" ->/ fun s -> Value(Literal(s, Pointer Char))
let _int = Match "-?[0-9]+" ->/ fun s -> Value(Literal(s, Int))
let _float = Match "-?[0-9]+\.[0-9]*\w?" ->/ fun s -> Value(Literal(s, Float))
//astx_to_ast_converters.Add(function
//  |T(s, []) when complete_match "[a-z A-Z][a-z 0-9 A-Z _]*" s -> Some (Value(Variable(s, Unknown [])))
//  |T(s, []) when complete_match "\"(\\\"|[^\"])*\"" s -> Some (Value(Literal(s, Pointer(Char))))
//  |T(s, []) when complete_match "-?[0-9]+" s -> Some (Value(Literal(s, Int)))
//  |T(s, []) when complete_match "-?[0-9]+\.[0-9]*\w?" s -> Some (Value(Literal(s, Float)))
//  |_ -> None
// )

let unary_ast type1 type2 s = Value(Variable(s, Datatype.Function([type1], type2)))
let binary_ast type1 type2 s = Value(Variable(s, Datatype.Function([type1; type1], type2)))

let prefix =
  OneOf [
    %"-" ->/ fun _ -> unary_ast t_numeric t_numeric "-prefix"
    %"*" ->/ fun _ -> unary_ast (Pointer t_any) t_any "*prefix"
    %"&" ->/ unary_ast t_any (Pointer t_any)
    %"!" ->/ unary_ast t_numeric t_numeric
    %"++" ->/ unary_ast t_numeric t_numeric
    %"--" ->/ unary_ast t_numeric t_numeric
   ]
let suffix =
  OneOf [
    %"++" ->/ fun _ -> unary_ast t_numeric t_numeric "++suffix"
    %"--" ->/ fun _ -> unary_ast t_numeric t_numeric "--suffix"
   ]
let math_infix_1 = Match "[*/%]" ->/ binary_ast t_numeric t_numeric
let math_infix_2 = Match "[+-]" ->/ binary_ast t_numeric t_numeric
let logic_infix = (%"||" |/ %"&&") ->/ binary_ast t_numeric Char
let comparison =
  OneOf [
    Match "[><]"
    %"!="
    %"=="
    %">="
    %"<="
   ] ->/ binary_ast t_numeric Char
//astx_to_ast_converters.Add(
//  let unknown_numeric = Unknown[Int; Char; Long]
//  function
//  |T("-prefix" | "!" | "++" | "--" | "++suffix" | "--suffix" as s, []) ->
//    Some (Value(Variable(s, Datatype.Function([unknown_numeric], unknown_numeric))))
//  |T("*prefix" as s, []) -> Some (Value(Variable(s, Datatype.Function([Pointer(Unknown[])], Unknown[]))))
//  |T("&", []) -> Some (Value(Variable("&", Datatype.Function([Unknown[]], Pointer(Unknown[])))))
//  |T("*" | "/" | "%" | "+" | "-" as s, []) ->
//    Some (Value(Variable(s, Datatype.Function([unknown_numeric; unknown_numeric], unknown_numeric))))
//  |T("||" | "&&" | "<" | ">" | "!=" | "==" | ">=" | "<=" as s, []) ->
//    Some (Value(Variable(s, Datatype.Function([unknown_numeric; unknown_numeric], Unknown[Char]))))
//  |_ -> None
// )

//let string_to_datatype_mappings =
//  dict [
//    "int", Int
//    "char", Char
//    "bool", Char
//    "long", Long
//    "double", Double
//    "float", Float
//    "void", Void
//   ]

let datatype =
  OneOf [
    %"int" ->/ fun _ -> Int
    %"char" ->/ fun _ -> Char
    %"bool" ->/ fun _ -> Char
    %"long" ->/ fun _ -> Long
    %"double" ->/ fun _ -> Double
    %"float" ->/ fun _ -> Float
    %"void" ->/ fun _ -> Void
   ]
let assignment = (%"=" |/ Match "[+\-*/&|]=") ->/ binary_ast t_any t_any

let rec value() =
  let basic_value = OneOf [_var; _string; _int; _float; bracketed]
  let rec value_with_apply_and_index: AST Rule =
    basic_value
     +/ OptionalListOf (
          !"(" +/ arg_list +/ !")" ->/ function ((), ll), () -> Apply(Value Unit, ll)
           |/ square_bracketed ->/ function i -> Index(Value Unit, i)
         )
     ->/ fun (v, ops) ->
           List.fold (fun acc -> function
             |Apply(Value Unit, ll) -> Apply(acc, ll)
             |Index(Value Unit, i) -> Index(acc, i)
             |_ -> failwith "should never be reached"
            ) v ops
  let rec prefixed_value() = () |> prefix +/ (value_with_apply_and_index |/ prefixed_value) ->/ fun (pf, v) -> Apply(pf, [v])
  let suffixed_value =
    (prefixed_value |/ value_with_apply_and_index) +/ OptionalListOf suffix
     ->/ fun (v, sfs) -> List.fold (fun acc sf -> Apply(sf, [acc])) v sfs
  let value_with_infix =
    let ast_from_infixes (iv: AST, ops: ((AST * AST) list)) =
      List.fold (fun acc (op, oprnd) -> Apply(op, [acc; oprnd])) iv ops
    JoinedListOf (
      JoinedListOf (
        JoinedListOf (
          JoinedListOf suffixed_value math_infix_1 ->/ ast_from_infixes
         ) math_infix_2 ->/ ast_from_infixes
       ) comparison ->/ ast_from_infixes
     ) logic_infix ->/ ast_from_infixes
  let rec value_with_assignment (): AST rule =
    let assign_transformation ((a, op), b) =
      match op with
      |Value(Variable(s, Datatype.Function([Unknown []; Unknown []], Unknown []))) ->
        match s with
        |"=" -> Assign(a, b)
        |"+=" | "-=" | "*=" | "/=" ->
          let xpr = Apply(Value(Variable(s.[..0], Datatype.Function([t_numeric; t_numeric], t_numeric))), [a; b])
          Assign(a, xpr)
        |"&=" | "|=" ->
          let xpr = Apply(Value(Variable(s.[..0], Datatype.Function([t_numeric; t_numeric], Char))), [a; b])
          Assign(a, xpr)
        |unexpected -> failwithf "unsupported assign option: %s" unexpected
      |_ -> failwith "should never be reached"
    () |> OneOf [
      value_with_infix +/ assignment +/ value_with_assignment ->/ assign_transformation
       |/ value_with_infix
     ]
  () |> value_with_assignment
and bracketed: AST Rule = !"(" +/ value +/ !")" ->/ function ((), v), () -> v
and square_bracketed = !"[" +/ value +/ !"]" ->/ function ((), v), () -> v
and arg_list = Optional (JoinedListOf value !",") ->/ function None -> [] | Some(arg1, args) -> arg1::List.map snd args
//astx_to_ast_converters.Add(function
//  |T("apply/index", a::rest) ->
//    List.fold (fun acc -> function
//      |T("bracketed", [T("argument list", args)]) -> Apply(acc, List.map convert_astx_to_ast args)
//      |T("bracketed", []) -> Apply(acc, [])
//      |T("square bracketed", [index]) -> Index(acc, convert_astx_to_ast index)
//      |ex -> failwithf "failed while parsing apply/index: %A" ex
//     ) (convert_astx_to_ast a) rest
//     |> Some
//  |T("prefix", [prefix; value]) -> Some (Apply(convert_astx_to_ast prefix, [convert_astx_to_ast value]))
//  |T("suffix", value::suffixes) -> 
//    List.fold (fun acc suffix ->
//      Apply(convert_astx_to_ast suffix, [acc])
//     ) (convert_astx_to_ast value) suffixes
//     |> Some
//  |T("infix", op_1::joined_list) ->
//    let ops = Seq.chunkBySize 2 joined_list
//    Seq.fold (fun acc [|infix; op_n|] ->
//      Apply(convert_astx_to_ast infix, [acc; convert_astx_to_ast op_n])
//     ) (convert_astx_to_ast op_1) ops
//     |> Some
//  |T("assign", joined_list) ->
//    let rec convert = function
//      |[last_value] -> convert_astx_to_ast last_value
//      |nth_value::_::rest -> Assign(convert_astx_to_ast nth_value, convert rest)
//      |[] -> failwith "failed while parsing assign"
//    Some (convert joined_list)
//  |T("bracketed", [value]) -> Some (convert_astx_to_ast value)
//  |_ -> None
// )

// TODO: update static and extern
let transform_declare dtype vs =
  let (|Possible|_|) = function
    |Unknown ptypes when ptypes = [] || List.exists ((=) dtype) ptypes -> Some ()
    |_ -> None
  Declare <|
    List.map (function
      |Assign(Value(Variable(name, Possible)), ast) ->
        Assign(Value(Variable(name, dtype)), ast)
      |Value(Variable(name, Possible)) ->
        Value(Variable(name, dtype))
      |Index(Value(Variable(name, Possible)), Value(Literal _)) ->
        Assign(Value(Variable(name, Pointer dtype)), Value(Variable("placeholder", t_any)))
      |Apply(Value(Variable("*prefix", _)), [Value(Variable(name, Unknown ptypes))]) ->
        Value(Variable(name, Pointer dtype))
      |Assign(Apply(Value(Variable("*prefix", _)), [Value(Variable(name, Unknown ptypes))]), ast) ->
        Assign(Value(Variable(name, Pointer dtype)), ast)
      |unexpected -> unexpected
     ) vs
let declare_value: AST Rule =
  Optional (%"extern" |/ %"static") +/ datatype +/ JoinedListOf value !","
   ->/ fun ((_, dtype), (v1, vlist)) -> transform_declare dtype (v1::List.map snd vlist)
let return_value =
  !"return" +/ Optional value
   ->/ function
       |(), Some v -> Return v
       |(), None -> Return (Value Unit)
//astx_to_ast_converters.Add(function
//  |T("declare", T(("extern" | "static"), [])::T(datatype_name, [])::declared_values)
//  |T("declare", T(datatype_name, [])::declared_values) ->
//    let datatype = string_to_datatype_mappings.[datatype_name]
//    Some (
//      Declare <|
//        List.map (function
//          |Assign(Value(Variable(value_name, Unknown ll)), ast) when ll = [] || List.exists ((=) datatype) ll ->
//            Assign(Value(Variable(value_name, datatype)), ast)
//          |Value(Variable(value_name, Unknown ll)) when ll = [] || List.exists ((=) datatype) ll ->
//            Value(Variable(value_name, datatype))
//          |Index(Value(Variable(value_name, Unknown ll)), Value(Literal _)) ->
//            Assign(Value(Variable(value_name, Pointer(datatype))), Value(Variable("placeholder", Unknown [])))
//          |Apply(Value(Variable("*prefix", _)), [Value(Variable(value_name, Unknown ll))]) ->
//            Value(Variable(value_name, Pointer(datatype)))
//          |Assign(Apply(Value(Variable("*prefix", _)), [Value(Variable(value_name, Unknown ll))]), ast) ->
//            Assign(Value(Variable(value_name, Pointer(datatype))), ast)
//          |unexpected -> failwithf "bad declare statement: %A" unexpected
//         ) (List.map convert_astx_to_ast declared_values)
//     )
//  |T("return", []) -> Some (Return (Value Unit))
//  |T("return", [value]) -> Some (Return (convert_astx_to_ast value))
//  |_ -> None
// )

let rec statement() =
  () |>
    OneOf [
      _if
      _for
      _while
      Optional(declare_value |/ return_value |/ value) +/ !";"
       ->/ function Some x, () -> x | None, () -> Block []
     ]
and code_block = !"{" +/ OptionalListOf statement +/ !"}" ->/ function ((), stmts), () -> Block stmts
and code_body = statement |/ code_block
and _if =
  !"if" +/ bracketed +/ code_body +/ Optional(!"else" +/ code_body)
   ->/ function
       |(((), cond_body), then_body), Some((), else_body) ->
         If(cond_body, then_body, else_body)
       |(((), cond_body), then_body), None ->
         If(cond_body, then_body, Block [])
and _while =
  !"while" +/ bracketed +/ code_body
   ->/ function ((), cond_body), loop_body -> While(cond_body, loop_body)
and _for =
  !"for" +/ !"("
   +/ Optional(declare_value |/ value) +/ !";"
   +/ Optional value +/ !";"
   +/ Optional value +/ !")"
   +/ code_body
   ->/ fun (((((((((), ()), decl), ()), cond), ()), incr), ()), loop_body) ->
         let extract_for_clause = function Some x -> x | None -> Value(Literal("1", Char))
         Block [
           extract_for_clause decl
           While(extract_for_clause cond, Block [loop_body; extract_for_clause incr])
          ]
//astx_to_ast_converters.Add(function
//  |T("statement", [value]) -> Some (convert_astx_to_ast value)
//  |T("block", []) -> Some (Value Unit)
//  |T("block", [T("block statements", statements)]) -> Some (Block(List.map convert_astx_to_ast statements))
//  |T("if", [cond; aff]) -> Some (If(convert_astx_to_ast cond, convert_astx_to_ast aff, Value Unit))
//  |T("if", [cond; aff; neg]) -> Some (If(convert_astx_to_ast cond, convert_astx_to_ast aff, convert_astx_to_ast neg))
//  |T("while", [cond; body]) -> Some (While(convert_astx_to_ast cond, convert_astx_to_ast body))
//  |T("for", [T("for_a", decl); T("for_b", cond); T("for_c", incr); body]) ->
//    let extract_for_clause = function
//      |[x] -> convert_astx_to_ast x
//      |[] -> Value Unit
//      |fail -> failwithf "failed while parsing `for` clause: %A" fail
//    Block [
//      extract_for_clause decl
//      While(
//        extract_for_clause cond,
//        Block [convert_astx_to_ast body; extract_for_clause incr]
//       )
//     ]
//     |> Some
//  |_ -> None
// )

let declare_function =
  let arg_list =  // TODO: update void args / no args - no args should accept any number of args, void should not accept any args
    JoinedListOf (datatype +/ _var ->/ fun (dtype, v) -> transform_declare dtype [v]) !","
     ->/ fun (a1, args) -> a1::List.map snd args
     |/ Optional !"void" ->/ fun _ -> []
  Optional (!"static" |/ !"extern")  // TODO: update static/extern
   +/ (datatype +/ _var |/ !"main" ->/ fun () -> Void, Value(Variable("main", t_any)))  // int main() and main() are both valid
   +/ !"(" +/ arg_list +/ !")" +/ (code_block |/ !";" ->/ fun () -> Value Unit)
   ->/ fun (((((_, (dtype, name)), ()), args), ()), func_body) ->
         let name = match name with Value(Variable(s, Unknown [])) -> s | _ -> failwith "should never be reached"
         let args, arg_types =
           List.map (function Declare [Value(Variable(_, dt) as v)] -> v, dt | _ -> failwith "should never be reached") args
            |> List.unzip
         let func_dtype = Datatype.Function(arg_types, dtype)
         Declare [Assign(Value(Variable(name, func_dtype)), Function(args, func_body))]
//astx_to_ast_converters.Add(function
//  |T("declare function", T(datatype_name, [])::T(variable_name, [])::rest) ->
//    let args, rest =
//      match rest with
//      |T("arglist", args)::rest ->
//        let args =
//          List.map convert_astx_to_ast args
//           |> List.map (function Declare [Value v] -> v | fail -> failwithf "failed parsing top level function arguments: %A" fail)
//        args, rest
//      |rest -> [], rest
//    let body =
//      match rest with
//      |[body] -> convert_astx_to_ast body
//      |[] -> Value Unit
//      |fail -> failwithf "failed parsing top level function: %A" fail
//    let datatype = Datatype.Function(List.map (fun _ -> Unknown []) args, string_to_datatype_mappings.[datatype_name])
//    Assign(Value(Variable(variable_name, datatype)), Function(args, body))
//     |> Some
//  |T("declare function", T("main", [])::rest) ->
//    Some (convert_astx_to_ast (T("declare function", T("int", [])::T("main", [])::rest)))
//  |_ -> None
// )

let parse_global_scope: AST Rule =
  //let rec parseGlobal() = () |> (declareFunction |/ declareValue +/ !";") +/ (EOF |/ parseGlobal)
  //parseGlobal ->/ "global level parse"
  let try_parse_decl =
    OneOf [
      declare_function
      declare_value +/ !";" ->/ fst
      !";" ->/ fun () -> Block []
     ]
  ListOf try_parse_decl +/ (EOF |/ try_parse_decl ->/ fun _ -> ())  // try_parse_decl in parallel with EOF to get correct error messages
   ->/ (fst >> GlobalParse)
//astx_to_ast_converters.Add(function
//  |T("global level parse", values) -> Some (GlobalParse(List.map convert_astx_to_ast values))
//  |_ -> None
// )

let parse_tokens_to_ast tokens =
  let result = parse_global_scope () tokens
  match result with
  |Yes(parsed, []) -> parsed
  |Yes(_, fail::_) -> failwithf "unexpected token: %A" fail
  |No(err, rest) -> failwithf "error: %s\n%A" err rest
  