module Parser.Combinators
open System.Text.RegularExpressions
open Lexer.Token

// syntax:
// let rec nameOfRule() = () |> buildRule()       <<-- the `() |> ` is for lazy eval. manipulation
// and otherRule = () |> buildRule(nameOfRule)    <<-- don't pass `()` to nameOfRule

//type ASTx = T of string * ASTx list

type 'a Result =
  |Yes of 'a * Token list
  |No of string * Token list  // fail message associates with longest successful parse path

type 'a rule = Token list -> 'a Result

type 'a Rule = unit -> 'a rule
//let transformASTx f (rule: ASTx Rule) e =
//  match rule() e with
//  |Yes(ast, rest) -> Yes(f ast, rest)
//  |no -> no

let Bind (binding: 'a -> 'b) (rule: 'a Rule) () : 'b rule =
  fun tokens ->
    match rule() tokens with
    |Yes(a, rest) -> Yes(binding a, rest)
    |No(e, r) -> No(e, r)
let (<-/) = Bind
let (->/) a b = Bind b a

//let Clean (rule: ASTx Rule) () : ASTx rule =
//  let rec clean = function
//    T(name, ch) -> T(name, List.map clean ch |> List.collect (function T("", ch') -> ch' | e -> [e]))
//  transformASTx clean rule
//let (!!!) = Clean

let Pass = Bind (fun _ -> ())
let Equal(token: string) () : string rule = function
  |{value=v}::rest when v = token -> Yes(v, rest)
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    let error = sprintf "expected %s before %A" token token_values
    No(error, tokens)
let (!) = Equal >> Pass
let (~%) = Equal

let Match(xpr: string) () : string rule = function
  |{value=v}::rest when Regex.Match(v, xpr).Value = v -> Yes(Regex.Match(v, xpr).Value, rest)
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    let error = sprintf "expected (%s) before %A" xpr token_values
    No(error, tokens)
let (!!) = Match >> Pass
let (~%%) = Match

let EOF () : unit rule = function
  |[] -> Yes((), [])
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    No(sprintf "expected EOF at %A" token_values, tokens)

let (+/) (x: 'a Rule) (y: 'b Rule) () : ('a * 'b) rule = fun tokens ->
  match x() tokens with
  |Yes(a, rest1) ->
    match y() rest1 with
    |Yes(b, rest2) -> Yes((a, b), rest2)
    |No(e, r) -> No(e, r)
  |No(e, r) -> No(e, r)
//let SequenceOf(rules: #seq<Rule>) : Rule = Seq.reduce (+/) rules

let (|/) (x: 'a Rule) (y: 'a Rule) () : 'a rule = fun tokens ->
  match x() tokens with
  |No(err1, r1) ->
    match y() tokens with
    |No(err2, r2) when r2.Length < r1.Length -> No(err2, r2)
    |No _ -> No(err1, r1)
    |yes -> yes
  |yes -> yes
let OneOf(rules: #seq<'a Rule>) : 'a Rule = Seq.reduce (|/) rules
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

let Optional(rule: 'a Rule) () : 'a Option rule =
  fun tokens ->
    match rule() tokens with
    |Yes(a, rest) -> Yes(Some a, rest)
    |No(_, rest) -> Yes(None, tokens)
let (~~) = Optional

let ListOf(rule: 'a Rule) () : 'a list rule =
  let rec list() =
    () |>
      rule +/ ~~list ->/
        function
        |(a, None) -> [a]
        |(a, Some bs) -> a::bs
  list()
let (~+) = ListOf

let OptionalListOf rule = Optional (ListOf rule) ->/ function Some parsed_list -> parsed_list | None -> []

let JoinedListOf (rule: 'a Rule) (delimiter: 'b Rule) = rule +/ OptionalListOf (delimiter +/ rule)
let (&/) = JoinedListOf

