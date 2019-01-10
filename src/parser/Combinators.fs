module Parser.Combinators
open System.Text.RegularExpressions
open Lexer.Token

// syntax:
// let rec nameOfRule() = () |> buildRule()       <<-- the `() |> ` is for lazy eval. manipulation
// and otherRule = () |> buildRule(nameOfRule)    <<-- don't pass `()` to nameOfRule

type ASTx = T of string * ASTx list

type Result =
  |Yes of ASTx * Token list
  |No of string * Token list  // fail message associates with longest successful parse path

type rule = Token list -> Result

type Rule = unit -> rule
let transformASTx f (rule: Rule) e =
  match rule() e with
  |Yes(ast, rest) -> Yes(f ast, rest)
  |no -> no

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
  |{value=v}::rest when v = token -> Yes(T(v, []), rest)
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    let error = sprintf "expected %s before %A" token token_values
    No(error, tokens)
let (!) = Equal >> Pass
let (~%) = Equal

let Match(xpr: string) () : rule = function
  |{value=v}::rest when Regex.Match(v, xpr).Value = v -> Yes(T(v, []), rest)
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    let error = sprintf "expected (%s) before %A" xpr token_values
    No(error, tokens)
let (!!) = Match >> Pass
let (~%%) = Match

let EOF () : rule = function
  |[] -> Yes(T("", []), [])
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    No(sprintf "expected EOF at %A" token_values, tokens)

let (+/) (x: Rule) (y: Rule) () : rule = fun tokens ->
  match x() tokens with
  |Yes(xAST, rest) -> transformASTx (fun yAST -> T("", [xAST; yAST])) y rest
  |no -> no
let SequenceOf(rules: #seq<Rule>) : Rule = Seq.reduce (+/) rules

let (|/) (x: Rule) (y: Rule) () : rule = fun tokens ->
  match x() tokens with
  |No(err1, r1) ->
    match y() tokens with
    |No(err2, r2) when r2.Length < r1.Length -> No(err2, r2)
    |No _ -> No(err1, r1)
    |yes -> yes
  |yes -> yes
let OneOf(rules: #seq<Rule>) : Rule = Seq.reduce (|/) rules
//let OneOf(rules: #seq<Rule>) () : rule = fun tokens ->
//  Seq.fold (fun acc x ->
//    match acc with
//    |No(err, r) ->
//      match x() tokens with
//      |No(err2, r2) when r2.Length < r.Length -> No(err2, r2)
//      |No _ -> No(err, r)
//      |yes -> yes
//    |yes -> yes
//   ) (No("placeholder", tokens)) rules

let Optional(rule: Rule) () : rule =
  fun tokens -> match rule() tokens with No _ -> Yes(T("", []), tokens) | yes -> yes
let (~~) = Optional

let ListOf(rule: Rule) () : rule =
  let rec list() = () |> rule +/ ~~list
  list()
let (~+) = ListOf

let JoinedListOf (rule: Rule) (delimiter: Rule) = rule +/ ~~(+(delimiter +/ rule))
let (&/) = JoinedListOf

