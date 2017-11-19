open System
open System.Text.RegularExpressions
open System.Runtime.Remoting.Metadata.W3cXsd2001
#r "lex.dll"
#load "framework.fs"
open Framework

let _var = %%"[a-z A-Z][a-z 0-9 A-Z _]*"
let _string = %%"\"(\\\"|[^\"])*\""
let _int = %%"-?[0-9]+"
let datatype = %"int" |/ %"char" |/ %"bool" |/ %"long" |/ %"double" |/ %"float" |/ %"void"
let prefix = %"-" |/ %"&" |/ %"*" |/ %"!" // %%"[-&*]" doesn't work for some reason
let mathInfix1, mathInfix2, logicInfix = %%"[*/]", %%"[+-]", %"||" |/ %"&&"
let comparison = %%"[><]" |/ %"!=" |/ %"==" |/ %">=" |/ %"<="
let assignment = %"=" |/ %%"[+\-*/&|]="
let rec value() =
  let basicValue = _var |/ _string |/ _int |/ bracketed
  let valueWithApplyAndIndex =
    basicValue +/ ~~(+(!"(" +/ ~~argList +/ !")" ->/ "bracketed" |/ squareBracketed)) ->/ "apply/index"
  let rec prefixedValue() = () |> prefix +/ (valueWithApplyAndIndex |/ prefixedValue) ->/ "prefix"
  // todo: suffixes
  let valueWithInfix =
    ((((prefixedValue |/ valueWithApplyAndIndex) &/ mathInfix1) ->/ "infix"
     &/ mathInfix2) ->/ "infix" &/ comparison) ->/ "infix" &/ logicInfix
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
and _for =
  !"for" +/ !"("
   +/ (~~(declareValue |/ value) +/ !";" ->/ "for_a")
   +/ (~~value +/ !";" ->/ "for_b")
   +/ (~~value +/ !")" ->/ "for_c") +/ codeBody
   ->/ "for"
let declareFunction =
  let argList = (declareValue &/ !",") ->/ "arglist"
  datatype +/ _var +/ !"(" +/ ~~argList +/ !")" +/ (codeBlock |/ !";") ->/ "declare function"
let parseGlobalScope = ~~(+(declareFunction |/ declareValue)) ->/ "global level parse"

type Datatype =
  |Int
  |Char
  |Long
  |Void
  |Pointer of Datatype
  |Function of Datatype list * Datatype

type Value =
  |Unit
  |Literal of string * Datatype
  |Variable of string * Datatype
  with
    override x.ToString() =
      match x with
      |Unit -> "()"
      |Literal(s, t) -> s
      |Variable(s, t) -> sprintf "%s: %A" s t

type AST =
  |Bracketed of AST
  |Apply of AST * AST list
  |Assign of AST * AST
  |Index of AST * AST
  |Declare of Value
  |Return of AST
  |Block of AST list
  |If of AST * AST * AST
  |While of AST * AST
  |Function of Value list * AST
  |Value of Value
  |GlobalParse of AST list
  // unused except in build phase
  |SquareBracketed of AST
  |ArgList of AST list
  |VarType of Datatype
  with
    override x.ToString() =
      let concatWith str = List.map (sprintf "%O") >> String.concat str
      let rec pr = function
        |Bracketed(x) -> sprintf "(%O)" x
        |Apply(a, [b]) -> sprintf "(%O %O)" a b
        |Apply(a, b) -> sprintf "(%O (%s))" a (concatWith ", " b)
        |Index(a, b) -> sprintf "%O[%O]" a b
        |Assign(a, b) -> sprintf "(%O <- %O)" a b
        |Declare(x) -> sprintf "let %O" x
        |Return(x) -> sprintf "return %O" x
        |Block(children) -> sprintf "(%s)" (concatWith " ; " children)
        |If(cond, aff, neg) -> sprintf "if %O then %O else %O" cond aff neg
        |While(cond, body) -> sprintf "while %O do %O" cond body
        |Function(args, body) -> sprintf "(%s) -> %O" (concatWith ", " args) body
        |GlobalParse(children) -> concatWith "\n" children
        |Value(x) -> x.ToString()
        |x -> failwithf "unexpected case: %A" x
      pr x

let makePattern (rule: Rule) x =
  let (|M|_|) = rule <| ()
  match [x] with
  |M(T(name, []), _) -> Some name
  |_ -> None
let (|Lit|_|) = makePattern (_string |/ _int |/ prefix |/ mathInfix1 |/ mathInfix2 |/ logicInfix |/ comparison |/ assignment)
let (|Type|_|) =
  let (|M|_|) = makePattern datatype
  function
    |M "void" -> Some Void
    |M "int" -> Some Int
    |M "long" -> Some Long
    |M "char" -> Some Char
    |_ -> None
let rec simplifyTree node =
  let node = match node with T(s, c) -> s, List.map simplifyTree c
  match node with
  |"bracketed", [value] -> Bracketed(value)
  |"square bracketed", [value] -> SquareBracketed(value)
  |"bracketed", [] -> Value(Unit)
  |"infix", hd::children ->
    List.chunkBySize 2 children
     |> List.fold (fun acc [infix; e] -> Apply(infix, [acc; e])) hd
  |"assign", children ->
    let rec buildAssign = function
      |l::op::r ->
        match op with
        |Value(Literal("=", _)) -> Assign(l, buildAssign r)
      |[r] -> r
    buildAssign children
  |("arglist" | "argument list"), children -> ArgList(children)
  |"apply/index", children ->
    List.reduce (fun acc -> function
      |Bracketed(ArgList(x)) -> Apply(acc, x)
      |Value(Unit) as u -> Apply(acc, [u])
      |SquareBracketed(x) -> Index(acc, x)
     ) children
  |"prefix", [a; b] -> Apply(a, [b])
  |"declare", [VarType t; xpr] ->
    let rec (|RootType|) = function
      |Apply(Value(Literal("*", _)), [x]) ->
        let t, ast = (|RootType|) x
        Pointer(t), ast
      |Value(Variable(s, _)) as ast -> t, s
      |x -> failwithf "unrecognized assignment %A" x
    match xpr with
    |Assign(RootType(t, name), r) -> Assign(Declare(Variable(name, t)), r)
    |RootType(t, name) -> Declare(Variable(name, t))
  |"declare", [VarType s; Assign(Value(Variable(l, _)), r)] -> Assign(Declare(Variable(l, s)), r)
  |"declare", [VarType s; Value(Variable(x, _))] -> Declare(Variable(x, s))
  |"return", [x] -> Return(x)
  |"statement", [] -> Value(Unit)
  |"statement", [x] -> x
  |"block", [x] -> x
  |"block statements", children -> Block(children)
  |"if", [Bracketed(cond); aff] -> If(cond, aff, Value(Unit))
  |"if", [Bracketed(cond); aff; neg] -> If(cond, aff, neg)
  |"while", [Bracketed(cond); body] -> While(cond, body)
  |("for_a" | "for_b" | "for_c"), [x] -> x
  |("for_a" | "for_b" | "for_c"), [] -> Value(Unit)
  |"for", [a; b; c; body] -> Block [a; While(b, Block [body; c])]
  |"declare function", VarType ret::Value(Variable(x, _))::rest ->
    let args, rest =
      match rest with
      |ArgList(args)::rest -> List.map (function Declare(x) -> x) args, rest
      |rest -> [Unit], rest
    let body = match rest with [] -> Value(Unit) | [body] -> body
    let signature = // nested signature?
      let argumentSig = List.map (function Variable(_, typeName) -> typeName | Unit -> Void) args
      Datatype.Function(argumentSig, ret)
    Assign(Declare(Variable(x, signature)), Function(args, body))
  |"global level parse", children -> GlobalParse(children)
  |Lit x, [] -> Value(Literal(x, Int))
  |Type t, [] -> VarType(t)
  |x, [] -> Value(Variable(x, Int)) // the default type is Int
  |x -> failwithf "not recognized: %A" x

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
     "int *a;"
    ]
 @ test Lexer.tokenize !!!parseGlobalScope [
     "void f(int x, long y) {}"
     "void f();"
     "void f(){a; b; return c;}"
    ]
 |> List.map (function
      |Some(result, rest) -> printfn "%O ... %A" (simplifyTree result) rest
      |_ -> printfn "did not match"
     )