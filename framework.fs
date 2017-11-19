module Framework
open System
open System.Text.RegularExpressions

// syntax:
// let rec nameOfRule() = () |> buildRule()       <<-- the `() |> ` is for lazy eval. manipulation
// and otherRule = () |> buildRule(nameOfRule)    <<-- don't pass `()` to nameOfRule

type AST = T of string * AST list

type rule = string list -> (AST * string list) option  // could be generic

type Rule = unit -> rule

let Rename name (rule: Rule) () : rule =
  let (|Rule|_|) = rule()
  function Rule(T(_, ch), rest) -> Some(T(name, ch), rest) | _ -> None
let (/<-) = Rename
let (->/) a b = Rename b a
let Clean (rule: Rule) () : rule =
  let (|Rule|_|) = rule()
  let rec clean = function
    T(name, ch) -> T(name, List.map clean ch |> List.collect (function T("", ch') -> ch' | e -> [e]))
  function Rule(ast, rest) -> Some(clean ast, rest) | _ -> None
let (!!!) = Clean
let Pass = Rename ""
let Equal(token: string) () : rule =
  function x::rest when x = token -> Some(T(x, []), rest) | _ -> None
let (!) = Equal >> Pass
let (~%) = Equal
let Match(xpr: string) () : rule =
  function x::rest when Regex.IsMatch(x, xpr) -> Some(T(x, []), rest) | _ -> None
let (!!) = Match >> Pass
let (~%%) = Match
let (+/) (x: Rule) (y: Rule) () : rule = fun tokens ->
  let (|MatchesX|_|), (|MatchesY|_|) = x(), y()
  match tokens with
  |MatchesX(xAST, MatchesY(yAST, rest)) -> Some(T("", [xAST; yAST]), rest)
  |_ -> None
let SequenceOf(rules: #seq<Rule>) : rule = Seq.reduce (+/) rules ()
let (|/) (x: Rule) (y: Rule) () : rule = fun tokens ->
  let (|MatchesX|_|), (|MatchesY|_|) = x(), y()
  match tokens with
  |MatchesX(ast, rest) | MatchesY(ast, rest) -> Some(ast, rest)
  |_ -> None
let OneOf(rules: #seq<Rule>) : rule = Seq.reduce (|/) rules ()
let ListOf(rule: Rule) () : rule =
  let (|Rule|_|) = rule()
  let rec matchWhile acc = function
    |Rule(ast, rest) -> matchWhile (ast::acc) rest
    |tokens -> T("", List.rev acc), tokens
  function Rule(ast, rest) -> Some(matchWhile [ast] rest) | _ -> None
let (~+) = ListOf
let Optional(rule: Rule) () : rule =
  let (|Rule|_|) = rule()
  function Rule(ast, rest) -> Some(ast, rest) | tokens -> Some(T("", []), tokens)
let (~~) = Optional
let JoinedListOf (rule: Rule) (delimiter: Rule) = rule +/ ~~(+(delimiter +/ rule))
let (&/) = JoinedListOf 

let test (tokenize: string -> string list) (rule: Rule) = List.map (tokenize >> rule())

// type RuleBuilder(detailList') =
//   let mutable detailList = detailList'
//   member x.Bind((value: Rule), xpr) =
//     match detailList with
//     |{name=name; clean=clean}::tl ->
//       detailList <- tl
//       xpr value |> Rename name |> (if clean then Clean else id)
//     |[] -> failwith "found an undefined rule"

// RuleBuilder([]) {
//   let! x = "x", Rule.Is "x"
//   ()
// }

// let addRecursionGuard (rule: rule) : rule =
//   let prevArg = ref None
//   fun tokens ->
//     try
//       if Some tokens = !prevArg
//        then None
//        else rule tokens
//     finally prevArg := Some tokens

// type Rule(name) =
//   new() = Rule("")
//   // create matching function from <type>
//   member it.isOneOf([<ParamArray>]tokens: string[]) : rule = function
//     |x::rest when Array.exists ((=) x) tokens -> Some(T(x, []), rest)
//     |_ -> None
//   member it.is(token: string) : rule = it.isOneOf token
//   member it.matches(xpr: string) : rule = function
//     |x::rest when Regex.IsMatch(x, xpr) -> Some(T(x, []), rest)
//     |_ -> None
//   member it.isSequenceOf([<ParamArray>]rules: Lazy<rule>[]) : rule = fun tokens ->
//     // if not (Array.isEmpty rules) then  // be careful doing this! adding it to multiple rules in `isOneOf` will cause collisions
//     //   let x = rules.[0]
//     //   rules.[0] <- lazy addRecursionGuard (x.Force())
//     Array.fold (fun state (rule: Lazy<rule>) ->
//       let (|Rule|_|) = rule.Force()
//       match state with
//       |Some(acc, remainingTokens) ->
//         match remainingTokens with
//         |Rule(parsed, rest) -> Some(parsed::acc, rest)
//         |_ -> None
//       |None -> None
//      ) (Some([], tokens)) rules
//      |> function Some(acc, rest) -> Some(T(name, List.rev acc), rest) | _ -> None
//   member it.isOneOf([<ParamArray>]rules: Lazy<rule>[]) : rule = fun tokens ->
//     Array.tryPick ((|>) tokens) (Array.map (fun (e: Lazy<rule>) -> e.Force()) rules)
//   member it.isManyOf(rule: Lazy<rule>) : rule =
//     let (|Rule|_|) = rule.Force()
//     let rec matchWhile acc = function
//       |Rule(x, rest) -> matchWhile (T(name, [acc; x])) rest
//       |tokens -> acc, tokens
//     function Rule(x, rest) -> Some(matchWhile x rest) | _ -> None

// let optional ((|Rule|_|): rule) = function Rule x -> Some x | tokens -> Some(T("", []), tokens)

// let debug msg (rule: rule) e =
//   printfn "%s" msg
//   printfn "%A" e
//   let yld = rule e
//   printfn "result: %A" yld
//   yld