module Lexer

let groupTokens (groupingRules:string->string->bool) =
  List.fold (fun (prev::rest as acc) e ->
    if groupingRules prev e
     then (prev+e)::rest
     else e::acc
   ) [""]
   >> List.rev
let ruleset =
  let charIDs = Array.create 256 0
  for e in ['a'..'z'] @ ['A'..'Z'] @ ['0'..'9'] @ ['_'] do charIDs.[int e] <- 1
  for e in [' '; '\n'] do charIDs.[int e] <- -1
  let symbols = "+-*/%<>=&|^@!$?"
  for e in symbols do charIDs.[int e] <- 2
  let (|UnfinishedString|_|) (s:string) =
    if s = "" || s.[0] <> '"' then None
    elif s = "\"" then Some UnfinishedString
    elif s.Length >= 2 && s.[s.Length-2] = '\\' then Some UnfinishedString
    elif s.[s.Length-1] = '"' then None
    else Some UnfinishedString
  let (|AlphabeticString|_|) (ss:string) =
    if ss.Length > 0 && ss.[ss.Length-1] = '\\' then failwith "unfinished string"
    let mutable s, pl = "", 0
    while pl < ss.Length do
      if ss.[pl] = '\\' then
        s <- s+string ss.[pl]     //to be replaced by real escape sequence parsing
        pl <- pl+1
      s <- s+string ss.[pl]
      pl <- pl+1
    if String.forall (fun e -> List.exists ((=) e) (['a'..'z'] @ ['A'..'Z'] @ ['"'; '''; '\\'])) s
     && (s.[s.Length-1] <> '"' || s = "\"")
     then Some AlphabeticString
     else None
  let (|Numeric|_|) (ss:string) =
    if '0' <= ss.[0] && ss.[0] <= '9' || (ss.Length > 1 && ss.[0] = '-' && '0' <= ss.[1] && ss.[1] <= '9')
     then Some Numeric
     else None
  let (|Symbolic|_|) ss =
    if String.forall (fun e -> String.exists ((=) e) symbols) ss
     then Some Symbolic
     else None
  let (|Not_Symbolic|_|) = function Symbolic -> None | _ -> Some Not_Symbolic
  let (|Space|_|) = function "" -> None |(ss:string) -> if charIDs.[int ss.[0]] = -1 then Some Space else None
  //special rules: -   ~   .   "   '
  // ' could use some work
  fun a b ->
    match a,b with
    |UnfinishedString, _    //newlines behave strangely
    |Space,Space -> true
    |Space,_ | _,Space -> false
    |"",_
    //|"\"",AlphabeticString
    |(AlphabeticString | Numeric),(AlphabeticString | Numeric)
    |"\'",_ | _,"\'"
    |"-",Numeric
    |Numeric,"."
    |"~",Symbolic -> true
    |_ when charIDs.[int a.[0]] = 0 || charIDs.[int b.[0]] = 0 -> false
    |_ -> charIDs.[int a.[0]] = charIDs.[int b.[0]]
//special rules: if a subsequence of characters matches one of the patterns below, it is always parsed as a token
let specialRules =
  List.fold (fun acc e ->
    match acc, e with       // `..` is the only known one for now, a different method might be used for the future
    |"."::tl, "." -> ".."::tl
    |_ -> e::acc
   ) []
   >> List.rev
let groupByRuleset (text:string) =
  text.Replace("\r", "\n").ToCharArray()
   |> List.ofArray
   |> List.map string
   |> specialRules
   //normal rules
   |> groupTokens ruleset