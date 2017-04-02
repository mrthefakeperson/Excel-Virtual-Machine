namespace Parser
open Lexer
open Token
// ignore all pattern match warnings (turn this off when adding code)
#nowarn "25"

module Python2 =
  let preprocess: string->Token list list =
    FSharp.preprocess
     >> fun e -> printfn "%A" e; e
     >> List.fold (fun acc e ->
          match acc with
          |(hd:Token::tl1)::tl2 when fst e.Indentation = fst hd.Indentation -> (e::hd::tl1)::tl2
          |_ -> [e]::acc
         ) []
     >> List.map List.rev
     >> List.rev

  let indent (e:Token) = snd e.Indentation
  let rec parseLineByLine left right =
    printfn "%A" (left, right)
    match (left:Token list), (right:Token list) with
    |IfL x -> printfn "aaaa: %O -> %O" (left, right) x; x
    (*
    |ExprWithBlock & (X(s, dep) as a::_, b::_) when indent a < indent b ->
      let parsed, restr = parseLineByLine [] right
      Token(s, (0, indent a), dep @ [parsed]), restr
    |ExprWithBlock -> failwith "block needs an expression"
    *)
    |a::restl, b::restr when indent a = indent b ->
      parseLineByLine (b::a::restl) restr
    |a::restl, b::restr when indent a > indent b ->
      Token("sequence", (0, indent a), List.rev left), right
    |_, [] -> Token("sequence", List.rev left), []
    |TransferL x -> x
    |x -> failwithf "indented block doesn't belong there: %A" x
  and (|IfL|_|) = function
    |X("if", [cond]) as a::restl, b::restr when indent a < indent b ->
      let aff, restr = parseLineByLine [] (b::restr)
      let parsed = Token("if", (0, indent a), [cond; aff])
      Some (parseLineByLine (parsed::restl) restr)
    |X("if", [cond; aff]) as a::restl, right ->
      let neg, restr =
        match right with
        |X("else", [xprs]) as c::restr when indent a = indent c -> xprs, restr
        |T "else" as c::restr when indent a = indent c -> parseLineByLine [] restr
        |X("elif", dep) as c::restr when indent a = indent c ->
          parseLineByLine [] (Token("if", (0, indent c), dep)::restr)
        |c::_ when indent a >= indent c -> Token("()", []), right
        |[] -> Token("()", []), right
        |_ -> failwith "indented block doesn't belong there"
      let parsed = Token("if", (0, indent a), [cond; aff; neg])
      Some (parseLineByLine restl (parsed::restr))
    |_ -> None
//  and (|While|_|) = function
//    |X("while", [cond])
  and (|ExprWithBlock|_|) = function   //consider making a generalization for all the colon keywords (put them all in a list)
    |(X("if", ([_] as dep)) | X("for", ([_; _] as dep)) | X("while", ([_] as dep)) as t)
      ::_, _ ->
      Some ExprWithBlock
    |(X(name, dep)::_, _) as k -> printfn "%A %A %A" name dep k; None
    |_ -> None
  and (|TransferL|_|) = function
    |left, x::restr -> Some (parseLineByLine (x::left) restr)
    |_ -> None
  let rec parseLine stop fail left right =
    match (left:Token list), (right:Token list) with
    |_ when stop right ->
      let col =
        match left, right with
        |a::_, _ | _, a::_ -> indent a
        |[], [] -> failwith "oh no a blank line"
      Token("sequence", (0, col), List.rev left), right
    |_ when fail right -> failwithf "unexpected end of context (in line) %O" right
    |Bracket stop fail x
    |ExprWithColon stop fail x
    |Apply stop fail x
    |Tuple stop fail x
    |Transfer stop fail x
                             -> x
    |_, [] -> Token("()", (0,0)), []
    |x -> failwithf "unknown: %O" x
  and (|Bracket|_|) stop fail = function
    |T "("::restl, right ->
      let parsed, T ")"::restr =
        parseLine (function T ")"::_ -> true | _ -> false) (fun e -> stop e || fail e)
         [] right
      Some (parseLine stop fail (parsed::restl) restr)
    |_ -> None
  and (|ExprWithColon|_|) stop fail = function   //consider making a generalization for all the colon keywords
    |T("if" | "elif" | "else" | "while" | "for" as s) as t::restl, right ->    //else
      let beforeColon, restr =
        match s with
        |"for" ->
          let namePattern, T "in"::restr =     //maybe a different state here
            parseLine (function T "in"::_ -> true | _ -> false) (fun e -> stop e || fail e)
             [] right
          let iterable, T ":"::restr =
            parseLine (function T ":"::_ -> true | _ -> false) (fun e -> stop e || fail e)
             [] right
          [namePattern; iterable], restr
        |"else" ->
          let T ":"::restr = right
          [], restr
        |_ ->
          let cond, T ":"::restr =
            parseLine (function T ":"::_ -> true | _ -> false) (fun e -> stop e || fail e)
             [] right
          [cond], restr
      let afterColon =
        match restr with
        |[] -> []
        |_ ->
          let body, restr =     // restr = []
            parseLine (function [] -> true | _ -> false) (fun e -> stop e || fail e)
             [] restr
          [body]
      Some (parseLine stop fail (Token(s, (0, indent t), beforeColon @ afterColon)::restl) [])
    |_ -> None
  //todo: operators, dot expressions, function calls, commands (eg. print)
  and (|Apply|_|) stop fail = function
    |a::restl, (T "("::_ as right) ->
      let argsTuple, restr = parseLine stop fail [] right
      let parsed = Token("apply", (0, indent a), [a; argsTuple])
      Some (parseLine stop fail restl (parsed::restr))
    |_ -> None
  and (|Tuple|_|) stop fail = function
    |t::restl, T ","::restr ->
      let parsed, restr =
        match parseLine stop fail [] restr with
        |X(",", es), restr -> Token(",", (0, indent t), t::es), restr
        |parsed, restr -> Token(",", (0, indent t), [t; parsed]), restr
      Some (parseLine stop fail restl (parsed::restr))
    |_ -> None
  and (|Transfer|_|) stop fail = function
    |left, x::restr -> Some (parseLine stop fail (x::left) restr)
    |_ -> None

  let parseSyntax =
    preprocess
     >> List.map (parseLine (function [] -> true | _ -> false) (fun _ -> false) [])
     >> List.map (fun e -> (fst e).Clean())
     >> parseLineByLine []
     >> fst