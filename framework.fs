module Framework
open System
open System.Text.RegularExpressions

type AST = T of string * AST list

type rule = string list -> (AST * string list) option  // could be generic

let addRecursionGuard (rule: rule) : rule =
  let prevArg = ref None
  fun tokens ->
    try
      if Some tokens = !prevArg
       then None
       else rule tokens
    finally prevArg := Some tokens

type Rule(name) =
  new() = Rule("")
  // create matching function from <type>
  member it.isOneOf([<ParamArray>]tokens: string[]) : rule = function
    |x::rest when Array.exists ((=) x) tokens -> Some(T(x, []), rest)
    |_ -> None
  member it.is(token: string) : rule = it.isOneOf token
  member it.matches(xpr: string) : rule = function
    |x::rest when Regex.IsMatch(x, xpr) -> Some(T(x, []), rest)
    |_ -> None
  member it.isSequenceOf([<ParamArray>]rules: Lazy<rule>[]) : rule = fun tokens ->
    // if not (Array.isEmpty rules) then  // be careful doing this! adding it to multiple rules in `isOneOf` will cause collisions
    //   let x = rules.[0]
    //   rules.[0] <- lazy addRecursionGuard (x.Force())
    Array.fold (fun state (rule: Lazy<rule>) ->
      let (|Rule|_|) = rule.Force()
      match state with
      |Some(acc, remainingTokens) ->
        match remainingTokens with
        |Rule(parsed, rest) -> Some(parsed::acc, rest)
        |_ -> None
      |None -> None
     ) (Some([], tokens)) rules
     |> function Some(acc, rest) -> Some(T(name, List.rev acc), rest) | _ -> None
  member it.isOneOf([<ParamArray>]rules: Lazy<rule>[]) : rule = fun tokens ->
    Array.tryPick ((|>) tokens) (Array.map (fun (e: Lazy<rule>) -> e.Force()) rules)
  member it.isManyOf(rule: Lazy<rule>) : rule =
    let (|Rule|_|) = rule.Force()
    let rec matchWhile acc = function
      |Rule(x, rest) -> matchWhile (T(name, [acc; x])) rest
      |tokens -> acc, tokens
    function Rule(x, rest) -> Some(matchWhile x rest) | _ -> None

let optional ((|Rule|_|): rule) = function Rule x -> Some x | tokens -> Some(T("", []), tokens)

let debug msg (rule: rule) e =
  printfn "%s" msg
  printfn "%A" e
  let yld = rule e
  printfn "result: %A" yld
  yld