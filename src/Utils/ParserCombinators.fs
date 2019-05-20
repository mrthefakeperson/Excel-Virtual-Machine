module ParserCombinators
open System.Text.RegularExpressions
open Utils

// syntax:
// let rec nameOfRule() = () |> buildRule()       <<-- the `() |> ` is for lazy eval. manipulation
// and otherRule = () |> buildRule(nameOfRule)    <<-- don't pass `()` to nameOfRule

type ParseResult<'input, 'output> = Result<'input * 'output, string Lazy>

// use the below for cleaner recursive parsers
// type Rule<'input, 'output_inner, 'output> =
//   |Match of recognizer: ('input -> 'output ParseResult)
//   |Map of ('output_inner -> 'output)
//   |Series of ('input -> 'output_inner ParseResult) * ('input -> 'output ParseResult)  // returns ('output_inner, 'output)
//   |Parallel of ('input -> 'output ParseResult) * ('input -> 'output ParseResult)
  
type Rule<'input, 'output> = 'input -> ParseResult<'input, 'output>

let ParseResultMap (binding: 'o1 -> 'o2) = Result.map (fun (i, o) -> (i, binding o))
  
let Map (binding: 'o1 -> 'o2) (rule: Rule<'i, 'o1>) : Rule<'i, 'o2> =
  rule >> ParseResultMap binding
let (->/) r f = Map f r

let Series (rule1: Rule<'i, 'o1>) (rule2: Rule<'i, 'o2>) : Rule<'i, 'o1 * 'o2> = fun input ->
  rule1 input
   |> Result.bind (fun (input', res1) -> ParseResultMap (mkpair res1) (rule2 input'))
let (+/) = Series
type SequenceBuilder() =
  member __.Bind(rule: Rule<'i, 'o1>, cont: 'o1 -> Rule<'i, 'o2>) : Rule<'i, 'o2> = fun input ->
    match rule input with
    |Ok(input', x) -> cont x input'
    |Error msg -> Error msg
  member __.Return(x: 'o) : Rule<'i, 'o> = fun input -> Ok(input, x)
  // member __.Zero() : Rule<'i, unit> = fun input -> Ok(input, ())
let SequenceOf = SequenceBuilder()
// SequenceOf {
//   let! x = fun i -> Error ""
//   let! k = fun i -> Ok([], x)
//   let! _ = fun i -> Error ""
//   return k
// }

let combine_errors _ = id
let Parallel (rule1: Rule<'i, 'o>) (rule2: Rule<'i, 'o>) : Rule<'i, 'o> = fun input ->
  match rule1 input with
  |Ok res -> Ok res
  |Error err -> combine_errors err (rule2 input)
let (|/) = Parallel
let OneOf (rules: Rule<'i, 'o> list) : Rule<'i, 'o> = List.reduce (|/) rules

let Optional (rule: Rule<'i, 'o>) : Rule<'i, 'o option> = fun input ->
  match rule input with
  |Ok(input', res) -> Ok(input', Some res)
  |Error _ -> Ok(input, None)

let OptionalListOf (rule: Rule<'i, 'o>) : Rule<'i, 'o list> =
  let rec parse_list input =
    match rule input with
    |Ok(input', res) -> ParseResultMap (fun resn -> res::resn) (parse_list input')
    |Error _ -> Ok(input, [])
  parse_list

let ListOf (rule: Rule<'i, 'o>) : Rule<'i, 'o list> = rule +/ OptionalListOf rule ->/ List.Cons

let JoinedListOf (rule: Rule<'i, 'o>) (sep: Rule<'i, unit>) : Rule<'i, 'o list> =
  rule +/ (OptionalListOf (sep +/ rule ->/ snd)) ->/ List.Cons

let LookAhead (rule: Rule<'i, 'o>) : Rule<'i, 'o> = fun input ->
  Result.map (fun (i, o) -> (input, o)) (rule input)
let (&/) rule1 rule2 = LookAhead rule1 +/ rule2 ->/ snd

module Atoms =
  module List =
    let Equal (token: 'i) : Rule<'i list, 'i> = function
      |hd::tl when hd = token -> Ok(tl, hd)
      |_ -> Error (lazy sprintf "expected token %A" token)
    let (~%) = Equal
    let (!) x = Equal x ->/ ignore

    let Match (rgx: string) : Rule<string list, string> = function
      |hd::tl when Regex.Match(hd, rgx).Value = hd -> Ok(tl, hd)
      |_ -> Error (lazy sprintf "expected regex %A" rgx)
    let (~%%) = Match
    let (!!) rgx = Match rgx ->/ ignore

    let End: Rule<'i list, unit> = function [] -> Ok([], ()) | _ -> Error (lazy "expected EOF")
    
    let run_parser (parse_rule: Rule<'i list, 'o>) (input: 'i list) : 'o =
      match parse_rule input with
      |Ok([], result) -> result
      |Error msg -> failwith (msg.Force())
      |Ok(rest, result) ->
        failwithf "parser did not reach EOF: result = %A, remaining = %A" result rest

  module String =
    let Equal (token: string) : Rule<string, string> = fun input ->
      if input.StartsWith token
       then Ok(input.[token.Length..], token)
       else Error (lazy sprintf "expected token %A" token)
    let (~%) = Equal
    let (!) x = Equal x ->/ ignore
     
    let Match (rgx: string) : Rule<string, string> = fun input ->
      let mtch = Regex.Match(input, rgx)
      if mtch.Success && mtch.Index = 0
       then Ok(input.[mtch.Value.Length..], mtch.Value)
       else Error (lazy sprintf "expected regex %A" rgx)
    let (~%%) = Match
    let (!!) rgx = Match rgx ->/ ignore

    let End: Rule<string, unit> = function "" -> Ok("", ()) | _ -> Error (lazy "expected end of string")
    
    let run_parser (parse_rule: Rule<string, 'o>) (input: string) : 'o =
      match parse_rule input with
      |Ok("", result) -> result
      |Error msg -> failwith (msg.Force())
      |Ok(rest, result) ->
        failwithf "parser did not reach EOF: result = %A, remaining = %A" result rest
