module Parser.Combinators
open System.Text.RegularExpressions
open System.Collections.Generic
open Lexer.Token

// syntax:
// let rec nameOfRule() = () |> buildRule()       <<-- the `() |> ` is for lazy eval. manipulation
// and otherRule = () |> buildRule(nameOfRule)    <<-- don't pass `()` to nameOfRule

type 'a Result =
  |Yes of 'a * Token list
  |No of int * Token list  // fail message associates with longest successful parse path

let match_symbols = ResizeArray()
let symbol_code = Dictionary()
let register_symbol s =
  if not (symbol_code.ContainsKey s) then
    symbol_code.[s] <- match_symbols.Count
    match_symbols.Add s
  symbol_code.[s]

let (|Error|) = function
  |No(code, rest) ->
    let token_values = List.map (fun e -> e.value) rest
    let error = sprintf "expected %s before %A" match_symbols.[code] token_values
    Error(error, rest)
  |_ -> failwith "not how this is used; match Yes(...) separately"

type 'a rule = Token list -> 'a Result

type 'a Rule = unit -> 'a rule

let Bind (binding: 'a -> 'b) (rule: 'a Rule) () : 'b rule =
  fun tokens ->
    match rule() tokens with
    |Yes(a, rest) -> Yes(binding a, rest)
    |No(e, r) -> No(e, r)
let (<-/) = Bind
let (->/) a b = Bind b a

let Pass = Bind (fun _ -> ())
let Equal(token: string) () : string rule = function
  |{value=v}::rest when v = token -> Yes(v, rest)
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    No(register_symbol token, tokens)
let (!) = Equal >> Pass
let (~%) = Equal

let Match(xpr: string) () : string rule = function
  |{value=v}::rest when Regex.Match(v, xpr).Value = v -> Yes(Regex.Match(v, xpr).Value, rest)
  |tokens ->
    let token_values = List.map (fun e -> e.value) tokens
    No(register_symbol (sprintf "(%s)" xpr), tokens)
let (!!) = Match >> Pass
let (~%%) = Match

let EOF () : unit rule = function
  |[] -> Yes((), [])
  |tokens -> No(register_symbol "EOF", tokens)

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

