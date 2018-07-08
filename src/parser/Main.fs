module Parser.Main
open System.Collections.Generic
open System.Text.RegularExpressions
open Parser.AST
open Parser.Combinators

let complete_match rgx s = Regex.Match(s, rgx).Value = s

let astx_to_ast_converters: List<ASTx -> AST option> = ResizeArray()
let convert_astx_to_ast astx =
  Seq.pick (fun converter -> converter astx) astx_to_ast_converters

let _var = Match "[a-z A-Z _][a-z 0-9 A-Z _]*"
let _string = Match "\"(\\\"|[^\"])*\""
let _int = Match "-?[0-9]+"
let _float = Match "-?[0-9]+\.[0-9]*\w?"
astx_to_ast_converters.Add(function
  |T(s, []) when complete_match "[a-z A-Z][a-z 0-9 A-Z _]*" s -> Some (Value(Variable(s, Unknown [])))
  |T(s, []) when complete_match "\"(\\\"|[^\"])*\"" s -> Some (Value(Literal(s, Pointer(Char))))
  |T(s, []) when complete_match "-?[0-9]+" s -> Some (Value(Literal(s, Int)))
  |T(s, []) when complete_match "-?[0-9]+\.[0-9]*\w?" s -> Some (Value(Literal(s, Float)))
  |_ -> None
 )

let prefix =
  OneOf [
    %"-" ->/ "-prefix"
    %"*" ->/ "*prefix"
    Match "[!&]"
    %"++"
    %"--"
   ]
let suffix =
  OneOf [
    %"++" ->/ "++suffix"
    %"--" ->/ "--suffix"
   ]
let mathInfix1 = Match "[*/%]"
let mathInfix2 = Match "[+-]"
let logicInfix = %"||" |/ %"&&"
let comparison =
  OneOf [
    Match "[><]"
    %"!="
    %"=="
    %">="
    %"<="
   ]
astx_to_ast_converters.Add(
  let unknown_numeric = Unknown[Int; Char; Long]
  function
  |T("-prefix" | "!" | "++" | "--" | "++suffix" | "--suffix" as s, []) ->
    Some (Value(Variable(s, Datatype.Function([unknown_numeric], unknown_numeric))))
  |T("*prefix" as s, []) -> Some (Value(Variable(s, Datatype.Function([Pointer(Unknown[])], Unknown[]))))
  |T("&", []) -> Some (Value(Variable("&", Datatype.Function([Unknown[]], Pointer(Unknown[])))))
  |T("*" | "/" | "%" | "+" | "-" as s, []) ->
    Some (Value(Variable(s, Datatype.Function([unknown_numeric; unknown_numeric], unknown_numeric))))
  |T("||" | "&&" | "<" | ">" | "!=" | "==" | ">=" | "<=" as s, []) ->
    Some (Value(Variable(s, Datatype.Function([unknown_numeric; unknown_numeric], Unknown[Char]))))
  |_ -> None
 )

let datatype =
  OneOf [
    %"int"
    %"char"
    %"bool"
    %"long"
    %"double"
    %"float"
    %"void"
   ]
let assignment = %"=" |/ Match "[+\-*/&|]="

let string_to_datatype_mappings = dict ["int", Int; "char", Char; "bool", Char; "long", Long; "double", Double; "float", Float; "void", Void]

let rec value() =
  let basicValue = OneOf [_var; _string; _int; _float; bracketed]
  let valueWithApplyAndIndex =
    basicValue
     +/ Optional(
          ListOf(
            !"(" +/ Optional argList +/ !")" ->/ "bracketed"
             |/ squareBracketed
           )
         ) ->/ "apply/index"
  let rec prefixedValue() = () |> prefix +/ (valueWithApplyAndIndex |/ prefixedValue) ->/ "prefix"
  let suffixedValue = (prefixedValue |/ valueWithApplyAndIndex) +/ Optional(ListOf suffix) ->/ "suffix"
  let valueWithInfix =
    JoinedListOf (
      JoinedListOf (
        JoinedListOf (
          JoinedListOf suffixedValue mathInfix1 ->/ "infix"
         ) mathInfix2 ->/ "infix"
       ) comparison ->/ "infix"
     ) logicInfix
  let valueWithAssignment = JoinedListOf (valueWithInfix ->/ "infix") assignment
  () |> valueWithAssignment ->/ "assign"
and bracketed = !"(" +/ value +/ !")" ->/ "bracketed"
and squareBracketed = !"[" +/ value +/ !"]" ->/ "square bracketed"
and argList = JoinedListOf value !"," ->/ "argument list"
astx_to_ast_converters.Add(function
  |T("apply/index", a::rest) ->
    List.fold (fun acc -> function
      |T("bracketed", [T("argument list", args)]) -> Apply(acc, List.map convert_astx_to_ast args)
      |T("bracketed", []) -> Apply(acc, [])
      |T("square bracketed", [index]) -> Index(acc, convert_astx_to_ast index)
      |ex -> failwithf "failed while parsing apply/index: %A" ex
     ) (convert_astx_to_ast a) rest
     |> Some
  |T("prefix", [prefix; value]) -> Some (Apply(convert_astx_to_ast prefix, [convert_astx_to_ast value]))
  |T("suffix", value::suffixes) -> 
    List.fold (fun acc suffix ->
      Apply(convert_astx_to_ast suffix, [acc])
     ) (convert_astx_to_ast value) suffixes
     |> Some
  |T("infix", op_1::joined_list) ->
    let ops = Seq.chunkBySize 2 joined_list
    Seq.fold (fun acc [|infix; op_n|] ->
      Apply(convert_astx_to_ast infix, [acc; convert_astx_to_ast op_n])
     ) (convert_astx_to_ast op_1) ops
     |> Some
  |T("assign", joined_list) ->
    let rec convert = function
      |[last_value] -> convert_astx_to_ast last_value
      |nth_value::_::rest -> Assign(convert_astx_to_ast nth_value, convert rest)
      |[] -> failwith "failed while parsing assign"
    Some (convert joined_list)
  |T("bracketed", [value]) -> Some (convert_astx_to_ast value)
  |_ -> None
 )

let declareValue = Optional (%"extern" |/ %"static") +/ datatype +/ JoinedListOf value !"," ->/ "declare"  // TODO: update static and extern
let returnValue = !"return" +/ Optional value ->/ "return"
astx_to_ast_converters.Add(function
  |T("declare", T(("extern" | "static"), [])::T(datatype_name, [])::declared_values)
  |T("declare", T(datatype_name, [])::declared_values) ->
    let datatype = string_to_datatype_mappings.[datatype_name]
    Some (
      Declare <|
        List.map (function
          |Assign(Value(Variable(value_name, Unknown ll)), ast) when ll = [] || List.exists ((=) datatype) ll ->
            Assign(Value(Variable(value_name, datatype)), ast)
          |Value(Variable(value_name, Unknown ll)) when ll = [] || List.exists ((=) datatype) ll ->
            Value(Variable(value_name, datatype))
          |unexpected -> failwithf "bad declare statement: %A" unexpected
         ) (List.map convert_astx_to_ast declared_values)
     )
  |T("return", []) -> Some (Return (Value Unit))
  |T("return", [value]) -> Some (Return (convert_astx_to_ast value))
  |_ -> None
 )

let rec statement() =
  () |> (
    OneOf [
      Optional(declareValue |/ returnValue |/ value) +/ !";" ->/ "statement"
      _if
      _for
      _while
     ]
   )
and codeBlock = !"{" +/ (Optional(ListOf statement) ->/ "block statements") +/ !"}" ->/ "block"
and codeBody = statement |/ codeBlock
and _if = !"if" +/ bracketed +/ codeBody +/ Optional(!"else" +/ codeBody) ->/ "if"
and _while = !"while" +/ bracketed +/ codeBody ->/ "while"
and _for =
  !"for" +/ !"("
   +/ (Optional(declareValue |/ value) +/ !";" ->/ "for_a")
   +/ (Optional value +/ !";" ->/ "for_b")
   +/ (Optional value +/ !")" ->/ "for_c") +/ codeBody
   ->/ "for"
astx_to_ast_converters.Add(function
  |T("statement", [value]) -> Some (convert_astx_to_ast value)
  |T("block", []) -> Some (Value Unit)
  |T("block", [T("block statements", statements)]) -> Some (Block(List.map convert_astx_to_ast statements))
  |T("if", [cond; aff]) -> Some (If(convert_astx_to_ast cond, convert_astx_to_ast aff, Value Unit))
  |T("if", [cond; aff; neg]) -> Some (If(convert_astx_to_ast cond, convert_astx_to_ast aff, convert_astx_to_ast neg))
  |T("while", [cond; body]) -> Some (While(convert_astx_to_ast cond, convert_astx_to_ast body))
  |T("for", [T("for_a", decl); T("for_b", cond); T("for_c", incr); body]) ->
    let extract_for_clause = function
      |[x] -> convert_astx_to_ast x
      |[] -> Value Unit
      |fail -> failwithf "failed while parsing `for` clause: %A" fail
    Block [
      extract_for_clause decl
      While(
        extract_for_clause cond,
        Block [convert_astx_to_ast body; extract_for_clause incr]
       )
     ]
     |> Some
  |_ -> None
 )

let declareFunction =
  let argList = (JoinedListOf (datatype +/ _var ->/ "declare") !"," |/ !"void") ->/ "arglist"  // TODO: update void args
  Optional (!"static" |/ !"extern")  // TODO: update static/extern
   +/ (datatype +/ _var |/ %"main")  // int main() and main() are both valid
   +/ !"(" +/ Optional argList +/ !")" +/ (codeBlock |/ !";") ->/ "declare function"
astx_to_ast_converters.Add(function
  |T("declare function", T(datatype_name, [])::T(variable_name, [])::rest) ->
    let args, rest =
      match rest with
      |T("arglist", args)::rest ->
        let args =
          List.map convert_astx_to_ast args
           |> List.map (function Declare [Value v] -> v | fail -> failwithf "failed parsing top level function arguments: %A" fail)
        args, rest
      |rest -> [], rest
    let body =
      match rest with
      |[body] -> convert_astx_to_ast body
      |[] -> Value Unit
      |fail -> failwithf "failed parsing top level function: %A" fail
    let datatype = Datatype.Function(List.map (fun _ -> Unknown []) args, string_to_datatype_mappings.[datatype_name])
    Assign(Value(Variable(variable_name, datatype)), Function(args, body))
     |> Some
  |T("declare function", T("main", [])::rest) ->
    Some (convert_astx_to_ast (T("declare function", T("int", [])::T("main", [])::rest)))
  |_ -> None
 )

let parseGlobalScope =
  //let rec parseGlobal() = () |> (declareFunction |/ declareValue +/ !";") +/ (EOF |/ parseGlobal)
  //parseGlobal ->/ "global level parse"
  ListOf (declareFunction |/ declareValue +/ !";") +/ EOF ->/ "global level parse"
astx_to_ast_converters.Add(function
  |T("global level parse", values) -> Some (GlobalParse(List.map convert_astx_to_ast values))
  |_ -> None
 )

let parse_tokens_to_ast tokens =
  let result = (
    Clean parseGlobalScope () (
      List.filter (fun e ->
        not (complete_match "^#.*" e.value) && not (complete_match "/\*.*\*/" e.value) && not (complete_match "^//.*" e.value)
       ) tokens
     )
   )
  match result with
  |Yes(astx, []) -> convert_astx_to_ast astx
  |Yes(_, fail::_) -> failwithf "unexpected token: %A" fail
  |No error -> failwith error
  