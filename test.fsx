#r "lex.dll"
#load "framework.fs"
open Framework

let _var = !!"[a-z A-Z][a-z 0-9 A-Z _]*"
let _string = !!"\"(\\\"|[^\"])*\""
let _int = !!"-?[0-9]+"
let datatype = !"int" |/ !"char" |/ !"bool" |/ !"long" |/ !"double" |/ !"float" |/ !"void"
let comparison = !!"[><]" |/ !"!=" |/ !"==" |/ !">=" |/ !"<="
let assignment = !"=" |/ !!"[+\-*/&|]="
let rec value() =
  let basicValue = _var |/ _string |/ _int |/ bracketed
  let rec prefixedBasicValue() = () |> (!"-" |/ !"&" |/ !"*" |/ !"!") +/ (basicValue |/ prefixedBasicValue) ->/ "prefix" // !!"[-&*]" doesn't work for some reason
  // todo: suffixes
  let valueWithApplyAndIndex =
    (prefixedBasicValue |/ basicValue) +/ ~~(+(!"(" +/ ~~argList +/ !")" ->/ "bracketed" |/ squareBracketed)) ->/ "apply/index"
  let valueWithInfix =
    (((valueWithApplyAndIndex &/ !!"[*/]") ->/ "infix"
     &/ !!"[+-]") ->/ "infix" &/ comparison) ->/ "infix" &/ (!"||" |/ !"&&")
  let valueWithAssignment = valueWithInfix ->/ "infix" &/ assignment
  () |> valueWithAssignment ->/ "assign"
and bracketed = !"(" +/ value +/ !")" ->/ "bracketed"
and squareBracketed = !"[" +/ value +/ !"]" ->/ "square bracketed"
and argList = (value &/ !",") ->/ "argument list"
let declareValue = datatype +/ value ->/ "declare"
let returnValue = !"return" +/ value ->/ "return"
let rec statement() = () |> (~~(declareValue |/ returnValue |/ value) +/ !";" ->/ "statement" |/ _if |/ _for |/ _while)
and codeBlock = !"{" +/ (~~(+statement) ->/ "block statements") +/ !"}" ->/ "block"
and codeBody = statement |/ codeBlock
and _if = !"if" +/ bracketed +/ codeBody +/ ~~(!"else" +/ codeBody) ->/ "if"
and _while = !"while" +/ bracketed +/ codeBody ->/ "while"
and _for = !"for" +/ !"(" +/ ~~(declareValue |/ value) +/ !";" +/ ~~value +/ !";" +/ ~~value +/ !")" +/ codeBody ->/ "for"
let declareFunction =
  let argList = (declareValue &/ !",") ->/ "arglist"
  datatype +/ _var +/ !"(" +/ ~~argList +/ !")" +/ (codeBlock |/ !";") ->/ "declare function"
let parseGlobalScope = ~~(+(declareFunction |/ declareValue)) ->/ "global level parse"
let rec simplifyTree node =
  let node = match node with T(s, c) -> T(s, List.map simplifyTree c)
  match node with
  |T("bracketed" | "square bracketed" as str, [_; value; _]) -> T(str, [value])
  |T("bracketed", [T("(", []); T(")", [])]) -> T("()", [])
  |T("infix", hd::children) ->
    List.fold (fun (acc, state) e ->
      (if state then T("apply", [acc; e]) else T("apply", [e; acc])), not state
     ) (hd, false) children |> fst
  |T("assign", children) ->
    let rec buildAssign = function l::_::r -> T("assign", [l; buildAssign r]) | [r] -> r
    buildAssign children
  |T("apply/index", children) ->
    List.reduce (fun acc -> function
      |T("bracketed", [T("argument list", x)]) -> T("apply", acc::List.map List.head (List.chunkBySize 2 x))
      |T("()", []) -> T("apply", [acc; T("()", [])])
      |T(_, [x]) -> T("index", [acc; x])
     ) children
  |T("prefix", [a; b]) -> T("apply", [a; b])
  |T("declare", [varType; T("assign", [l; r])]) -> T("assign", [T("declare", [l; varType]); r])
  |T("declare", [varType; x]) -> T("declare", [x; varType])
  |T("return", [_; x]) -> T("return", [x])
  |T("statement", [T(";", [])]) -> T("()", [])
  |T("statement", ([x; T(";", [])] | [x])) -> x
  |T("block", [_; x; _]) -> x
  |T("block statements", children) -> T("block", children)
  |T("if", [_; T("bracketed", [cond]); aff]) -> T("if", [cond; aff; T("()", [])])
  |T("if", [_; T("bracketed", [cond]); aff; _; neg]) -> T("if", [cond; aff; neg])
  |T("while", [_; T("bracketed", [cond]); body]) -> T("while", [cond; body])
  |T("for", _::_::children) ->
    let a, children =
      match children with T(";", [])::children -> T("()", []), children | a::_::children -> a, children
    let b, children =
      match children with T(";", [])::children -> T("()", []), children | b::_::children -> b, children
    let c, body =
      match children with [T(";", []); body] -> T("()", []), body | [c; _; body] -> c, body
    T("block", [a; T("while", [b; T("block", [body; c])])])
  |T("declare function", T(returnType, [])::x::rest) ->
    let args, rest =
      match rest with
      |T("(", [])::T(")", [])::rest -> [T("declare", [T("()", []); T("unit", [])])], rest
      |_::T("arglist", args)::_::rest -> List.map List.head (List.chunkBySize 2 args), rest
    let body = match rest with [T(";", [])] -> T("()", []) | [body] -> body
    let signature = T(
      List.map (function
        T(_, [_; T(typeName, [])]) -> typeName
       ) args
       @ [returnType]
       |> String.concat "->",
       []
     )
    T("assign", [T("declare", [x; signature]); T("function", body::args)])
  |x -> x
let rec pr ast =
  let (|T|) = function T(s, c) -> (s, List.map pr c)
  match ast with
  |T("bracketed", [x]) -> sprintf "(%s)" x
  |T("apply", [a; b]) -> sprintf "(%s %s)" a b
  |T("apply", a::b) -> sprintf "(%s (%s))" a (String.concat ", " b)
  |T("index", [a; b]) -> sprintf "%s[%s]" a b
  |T("assign", [a; b]) -> sprintf "(%s <- %s)" a b
  |T("declare", [x; varType]) -> sprintf "let %s: %s" x varType
  |T("return", [x]) -> sprintf "return %s" x
  |T("block", children) -> sprintf "(%s)" (String.concat " ; " children)
  |T("if", [cond; aff; neg]) -> sprintf "if %s then %s else %s" cond aff neg
  |T("while", [cond; body]) -> sprintf "while %s do %s" cond body
  |T("function", body::args) -> sprintf "(%s) -> %s" (String.concat ", " args) body
  |T("global level parse", children) -> String.concat "\n" children
  |T(str, []) -> str
  |x -> failwithf "unexpected case: %A" x

test Lexer.tokenize !!!value [
  "1 + 2 * (3 - 4 - 5 / 6) + -(7 * -8) * 9"
  "-f(x, y)[3]" // maybe wrong: does prefix come after apply?
  "&!-*1"
  "f()"
]
 @ test Lexer.tokenize !!!statement [
     "void k = 1;"
     "if (a) { x; } else if (b) y;"
     "for (int e = 0; k; v);"
     "while (1) {x; y; z;}"
    ]
 @ test Lexer.tokenize !!!parseGlobalScope [
     "void f(int x, long y) {}"
     "void f();"
     "void f(){a; b; return c;}"
    ]
 |> List.map (function
      |Some(result, rest) -> printfn "%s ... %A" (pr <| simplifyTree result) rest
      |_ -> printfn "did not match"
     )

open System.Collections.Generic

type Datatype =
  |Int
  |Char
  |Long
  |Pointer of Datatype
let (|DeclaredType|) = function
  |T(name, []) ->
    match name with
    |"int" -> Int
    |"char" -> Char
    |"long" -> Long

let staticAnalysis ast =
  let memory, free = Dictionary<AST, int>(), ref 0
  let prealloc name n =
    if memory.ContainsKey name then
      failwithf "duplicate variable %s" name
    else
      memory.[name] <- free.Value
      free := free.Value + n

  let types = Dictionary()
  let rec setType name t =
    match name with
    |T("apply", [T("*", []); x]) -> setType x (Pointer t)
    |_ ->
      types.[name] <- t
      t
  let getType name =
    if types.ContainsKey name
     then types.[name]
     else failwithf "variable %A was not declared" name
  let rec typeAnalysis = function
    |T("declare", [x; DeclaredType t]) ->
      prealloc x 1
      setType x t
    // |T("apply", [T("apply", [T(infix, []); a]); b]) -> // any type is a numeric type? determine stack space used per variable
    //   match infix with
    //   |"+" | "-" | "*" | "/" ->
    //     match getType a with
    //     |Int | Long as aType ->
    //       if getType b = aType
    //        then aType
    //        else failwithf "expected %A to have type %A" b aType
    //     |_ -> failwithf "expected %A to have numeric type" a
    //   |"&&" | "||" ->
        
    |T(_, children) -> List.iter typeAnalysis children
  typeAnalysis ast

  memory, types
// let codeGen ast =
//   let (@) = Seq.append
//   let memory, types = staticAnalysis ast
//   let rec codeGen = function
//     |T("bracketed", [x]) -> codeGen x
//     |T("apply", f::xs) -> Seq.collect codeGen xs @ (codeGen f) @ ["CALL"]
//     |T("index", [a; b]) -> codeGen a @ codeGen b @ ["ADD"] @ ["DEREFERENCE"]
//     |T("assign", [a; b]) -> codeGen b @ codeGen (addressOf a) @ ["STORE"]
//     |T("declare", [x; _]) when memory.ContainsKey x -> [sprintf "PUSH %i" memory.[x]]
//     |T("declare", [x; _]) -> codeGen x @ ["ALLOC"] @ ["STORE but keep only the alloc address"]

    