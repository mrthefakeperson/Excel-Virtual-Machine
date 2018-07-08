module Parser.Combinators
open System.Text.RegularExpressions
open Lexer.Token

// syntax:
// let rec nameOfRule() = () |> buildRule()       <<-- the `() |> ` is for lazy eval. manipulation
// and otherRule = () |> buildRule(nameOfRule)    <<-- don't pass `()` to nameOfRule

type ASTx = T of string * ASTx list

type Result = Yes of ASTx * Token list | No of failMessage: string

type rule = Token list -> Result

type Rule = unit -> rule
let transformASTx f (rule: Rule) e =
  match rule() e with
  |Yes(ast, rest) -> Yes(f ast, rest)
  |No err -> No err

let Rename name (rule: Rule) () : rule =
  transformASTx (fun (T(_, ch)) -> T(name, ch)) rule
let (/<-) = Rename
let (->/) a b = Rename b a

let Clean (rule: Rule) () : rule =
  let rec clean = function
    T(name, ch) -> T(name, List.map clean ch |> List.collect (function T("", ch') -> ch' | e -> [e]))
  transformASTx clean rule
let (!!!) = Clean

let Pass = Rename ""
let Equal(token: string) () : rule = function
  |x::rest when x.value = token -> Yes(T(x.value, []), rest)
  |tokens -> No(sprintf "expected %s before %A" token (List.map (fun e -> e.value) tokens))
let (!) = Equal >> Pass
let (~%) = Equal

let Match(xpr: string) () : rule = function
  |x::rest when Regex.Match(x.value, xpr).Value = x.value -> Yes(T(x.value, []), rest)
  |tokens -> No(sprintf "expected (%s) before %A" xpr (List.map (fun e -> e.value) tokens))
let (!!) = Match >> Pass
let (~%%) = Match

let EOF () : rule = function
  |[] -> Yes(T("", []), [])
  |tokens -> No(sprintf "expected EOF at %A" (List.map (fun e -> e.value) tokens))

let (+/) (x: Rule) (y: Rule) () : rule = fun tokens ->
  match x() tokens with
  |Yes(xAST, rest) -> transformASTx (fun yAST -> T("", [xAST; yAST])) y rest
  |no -> no
let SequenceOf(rules: #seq<Rule>) : Rule = Seq.reduce (+/) rules

let (|/) (x: Rule) (y: Rule) () : rule = fun tokens ->
  match x() tokens with
  |No err1 ->
    match y() tokens with
    |No err2 -> No(err1 + "\n" + err2)
    |yes -> yes
  |yes -> yes
let OneOf(rules: #seq<Rule>) : Rule = Seq.reduce (|/) rules

let Optional(rule: Rule) () : rule =
  fun tokens -> match rule() tokens with No _ -> Yes(T("", []), tokens) | yes -> yes
let (~~) = Optional

let ListOf(rule: Rule) () : rule =
  let rec list() = () |> rule +/ ~~list
  list()
let (~+) = ListOf

let JoinedListOf (rule: Rule) (delimiter: Rule) = rule +/ ~~(+(delimiter +/ rule))
let (&/) = JoinedListOf

