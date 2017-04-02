namespace Parser
open Token
open Symbols
open Lexer
open Lexer.CommonClassifiers
// ignore all pattern match warnings (turn this off when adding code)
#nowarn "25"

module FSharp =
  let preprocess:string->Token list =
    let mainRules =     // todo: stick infixes together
      singleLineCommentRules "//"
       @ delimitedCommentRules "(*" "*)"
       @ createSymbol "->"
       @ createSymbol ".."
       @ commonRules
    let rec fixHyphens = function    // "-"::b::tl is handled because Infix doesn't match it
      |a::"-"::b::tl when isWhitespace a && (isNumeric >>|| isVariable) b ->
        a::"-" + b::fixHyphens tl
      |hd::tl -> hd::fixHyphens tl
      |[] -> []
    List.ofSeq
     >> List.map string
     >> tokenize mainRules
     >> fixHyphens
     >> List.fold (fun ((r, c), acc) -> function
          |"\n" -> (r + 1, 0), acc
          |" " -> (r, c + 1), acc
          |e when e = "\r" || isDelimitedString "//" "\n" e
           || isDelimitedString "(*" "*)" e -> (r, c), acc
          |"\t" -> failwith "tabs not allowed"
          |e when e = "." || List.exists ((=) e) prefixes ->
            (r, c + e.Length), Token(e, (r, c), false, [])::acc
          |e -> (r, c + e.Length), Token(e, (r, c))::acc
         ) ((1, 0), [])
     >> snd >> List.rev
     >> List.fold (fun (earliestInRow, acc) e ->
          if e.Name = "fun" && fst e.Indentation = fst earliestInRow then
            (earliestInRow, Token("fun", earliestInRow)::acc)
          elif fst e.Indentation > fst earliestInRow then
            (e.Indentation, e::acc)
          else
            (earliestInRow, e::acc)
         ) ((-1, 0), [])
     >> snd >> List.rev
     >> List.map (function
          |T s as t when s.Length > 1 && s.[0] = '-' && (isVariable >>|| isNumeric) s.[1..] ->
            Token("apply", t.Indentation, true, [Token "~-"; Token s.[1..]])
          |x -> x
         )

  let (|ConsqLR|_|) = function
    |(T _ as a)::_, (T _ as b)::_ when a.IndentedLess b -> Some ConsqLR
    |_ -> None
  let (|ConsqS|_|) = function
    |(T _ as a)::(T _ as b)::_ when a.IndentedLess b -> Some ConsqS
    |_ -> None

  type State =
    |Pattern
    |Normal
  let rec parse state (stop:Token list->bool) (fail:Token list->bool) left right =
    //printfn "%A" (left, right)
    match state with
    |Normal ->
      match (left:Token list), (right:Token list) with
      |t::_, _ when stop right -> Token("sequence", t.Indentation, List.rev left), right
      |_ when stop right -> Token("()", (0,0), left), right
      |_ when fail right -> failwithf "unexpected end of context %A" right
      |Brackets state stop fail x
      |SquareBrackets state stop fail x
      |If state stop fail x
      |Do state stop fail x
      |While state stop fail x
      |For state stop fail x
      |Let state stop fail x
      |Fun state stop fail x
      |PatternCase state stop fail x
      |Match state stop fail x
      |Function state stop fail x
      |Dot state stop fail x
      |Prefix state stop fail x
      |Infix state stop fail x
      |Tuple state stop fail x           //more testing needed
      |Apply state stop fail x
      |Transfer state stop fail x             -> x
      |_, [] -> Token("()", (0,0)), []
    |Pattern ->
      match (left:Token list), (right:Token list) with
      |t::_, _ when stop right -> Token("sequence", t.Indentation, List.rev left), right
      |_ when stop right -> Token("()", (0,0), left), right
      |_ when fail right -> failwithf "unexpected end of context %A" right
      |PatternBrackets state stop fail x
      |PatternComb state stop fail x
      |Tuple state stop fail x
      |Apply state stop fail x
      |Transfer state stop fail x         -> x
      |_, [] -> Token("()", (0,0)), []
  and (|Brackets|_|) state stop fail = function
    |left:Token list, (T "(" as t)::restr ->
      let indent =
        match left with
        |p::_ when fst p.Indentation = fst t.Indentation -> p.Indentation
        |_ -> t.Indentation
      let parsed, T ")"::restr =
        match restr with
        |T ")"::_ -> Token("()", t.Indentation, true, []), restr
        |_ ->
          parse state (function T ")"::_ -> true | t'::_ when snd indent > snd t'.Indentation -> true | _ -> false)
           (fun _ -> false)    //intermediate step keywords should fail
           [] restr
      let restr = Token("()", t.Indentation, true, [parsed])::restr
      Some (parse state stop fail left restr)
    |_ -> None
  and (|SquareBrackets|_|) state stop fail = function
    |left:Token list, (T "[" as t)::restr ->
      let indent =
        match left with
        |p::_ when fst p.Indentation = fst t.Indentation -> p.Indentation
        |_ -> t.Indentation
      let parsed, T "]"::restr =
        match restr with
        |T "]"::_ -> Token("[]", t.Indentation, true, []), restr
        |_ ->
          parse state (function T "]"::_ -> true | t'::_ when snd indent > snd t'.Indentation -> true | _ -> false)
           (fun _ -> false)
           [] restr
      let restr = Token("[]", t.Indentation, true, [parsed])::restr
      Some (parse state stop fail left restr)
    |_ -> None
  and (|If|_|) state stop fail = function
    |T "if" as t::restl, right ->
      let cond, T "then"::restr =
        parse state (function T "then"::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] right
      let aff, restr =
        parse state (function T ("elif" | "else")::_ -> true | k when stop k -> true | t'::_ -> not (t.IndentedLess t'))
         fail [] restr
      let neg, restr =
        match restr with
        |T "elif"::restr | (T "else"::T "if"::restr & ConsqS) ->
          parse state stop fail [] (t::restr)
        |T "else"::restr ->
          parse state (function k when stop k -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
           fail [] restr
        |_ -> Token("()", t.Indentation), restr
      let parsed = Token("if", t.Indentation, [cond; aff; neg])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Do|_|) state stop fail = function
    |T "do" as t::restl, right ->
      Some (
        parse state (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
         fail restl right
       )
    |_ -> None
  and (|While|_|) state stop fail = function
    |T "while" as t::restl, right ->
      let cond, T "do"::restr =
        parse state (function T "do"::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] right
      let body, restr = parse state stop fail [] (Token("do", t.Indentation)::restr)
      let parsed = Token("while", t.Indentation, [cond; body])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|For|_|) state stop fail = function
    |T "for" as t::restl, right ->
      let name, T "in"::restr =
        parse Pattern (function T "in"::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] right
      let iterable, restr =
        match parse state (function T("do" | "..")::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] restr with
        |iterable, T "do"::restr -> iterable, restr
        |a, T ".."::restr ->
          match parse state (function T("do" | "..")::_ -> true | _ -> false)
           (fun e -> stop e || fail e) [] restr with
          |b, T "do"::restr -> Token("..", [a; Token("1", []); b]), restr
          |step, T ".."::restr ->
            let b, T "do"::restr =
              parse state (function T "do"::_ -> true | _ -> false)
               (fun e -> stop e || fail e) [] restr
            Token("..", [a; step; b]), restr
      let body, restr = parse state stop fail [] (Token("do", t.Indentation)::restr)
      let parsed = Token("for", t.Indentation, [name; iterable; body])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Let|_|) state stop fail = function
    |((T "let" as t::restl, T ("rec" as s)::right) & ConsqLR)
    |(T ("let" as s) as t::restl, right) ->
      let name, T "="::restr =
        parse Pattern (function T "="::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] right
      let body, restr =
        parse state (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
         fail [] restr
      let parsed = Token((if s = "let" then s else "let rec"), t.Indentation, [name; body])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Fun|_|) state stop fail = function
    |T "fun" as t::restl, right ->
      let name, T "->"::restr =
        parse Pattern (function T "->"::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] right
      let body, restr =
        parse state (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
         fail [] restr
      let parsed = Token("fun", t.Indentation, [name; body])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|PatternCase|_|) state stop fail = function
    |T "|" as t::restl, right ->
      let pattern, T "->"::restr =
        parse Pattern (function T "->"::_ -> true | _ -> false)
         (fun e -> stop e || fail e)    //buggy infixes: test      a+b; |c & d ->        (maybe move infixes up)
         [] right
      let body, restr =
        parse state (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
         fail [] restr
      let parsed = Token("pattern", t.Indentation, [pattern; body])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Match|_|) state stop fail = function
    |T "match" as t::restl, right ->
      let target, T "with"::restr =
        parse Pattern (function T "with"::_ -> true | _ -> false)
         (fun e -> stop e || fail e) restl right
      let restr =
        match restr with
        |T "|"::_ -> restr
        |_ -> Token("|", t.Indentation)::restr
      let cases, restr = parse state stop fail [] restr
      let parsed = Token("match", t.Indentation, [target; cases])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Function|_|) state stop fail = function
    |T "function" as t::restl, right ->
      let restr =
        match right with
        |T "|"::_ -> right
        |_ -> Token("|", t.Indentation)::right
      let cases, restr = parse state stop fail [] restr
      let parsed = Token("function", t.Indentation, [cases])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Dot|_|) state stop fail = function
    |T "."::(A as a)::restl, (A as b)::restr ->  //not perfect: catches bracketed expressions as weird dotted accesses
      let parsed = Token("dot", a.Indentation, true, [a; b])
      Some (parse state stop fail restl (parsed::restr))
    //fix priority
    |left, a::(T "." as t)::restr -> Some (parse state stop fail (t::a::left) restr)
    |_ -> None
  and (|Prefix|_|) state stop fail = function
    |Pref a::restl, right ->     // todo: check relative indentation of prefix operator and its token
      let b, restr =
        match right with            // dANGER >:(   (untested)
        |T s::(T "."::_ as restr) ->    // special case
          let (T "()" | X("sequence", [])), b::restr =
            parse state (function T _::_ -> false | _ -> true) (fun e -> stop e || fail e) [] right
          b, restr
        |T s as b::restr when (isVariable >>|| isNumeric) s -> b, restr
        |_ ->
          let (T "()" | X("sequence", [])), b::restr =
            parse state (function T _::_ -> false | _ -> true) (fun e -> stop e || fail e) [] right
          b, restr
      let parsed = Token("apply", a.Indentation, true, [a; b])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Infix|_|) state stop fail = function
    |left:Token list, (T _ as nfx)::restr when nfx.Priority > -1 ->
      let a::restl =
        match left with
        |[] -> failwith "infix applied, but no preceding argument"
        |_ -> left
      let b,restr =
        parse state (function nfx'::_ when nfx.EvaluatedFirst nfx' -> true | k -> stop k)
         fail [] restr
      match b.Name, b.Dependants with
      |"sequence", parsed::r ->
        let parsed = Token("apply", a.Indentation, [Token("apply", a.Indentation, [nfx; a]); parsed])
        Some (parse state stop fail restl (parsed::r @ restr))
      |_ -> failwith "infix failed"
    |_ -> None
  and (|Tuple|_|) state stop fail = function      //testing needed
    |t::restl, T ","::restr ->
      let parsed, restr =
        match parse state stop fail [] restr with
        |X(",", es), restr -> Token(",", t::es), restr
        |parsed, restr -> Token(",", t.Indentation, [t; parsed]), restr
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Apply|_|) state stop fail = function
    |(A as a)::restl, (A as b)::restr when a.IndentedLess b ->
      let restr = Token("apply", a.Indentation, true, [a; b])::restr
      Some (parse state stop fail restl restr)
    |_ -> None
  and (|Transfer|_|) state stop fail = function
    |left, x::restr -> Some (parse state stop fail (x::left) restr)
    |_ -> None
  and (|PatternBrackets|_|) state stop fail = function
    |left, (T "(" as t)::restr ->
      let parsed, T ")"::restr =
        match restr with
        |T ")"::_ -> Token("()", t.Indentation, true, []), restr
        |_ -> parse Pattern (function T ")"::_ -> true | _ -> false) (fun _ -> false) [] restr
      let parsed = Token("()", t.Indentation, true, [parsed])
      Some (parse Pattern stop fail left (parsed::restr))
    |_ -> None
  and (|PatternComb|_|) state stop fail = function
    |left:Token list, (T ("|" | "&" as s) as nfx)::restr ->
      let a::restl =
        match left with
        |[] -> failwith "pattern infix rule applied with no preceeding argument"
        |_ -> left
      let b, restr =
        parse Pattern (function T ("|" | "&")::_ -> true | k -> stop k) fail [] restr
      let parsed = Token(s, a.Indentation, [a; b])
      Some (parse Pattern stop fail restl (parsed::restr))
    |_ -> None

  let parseSyntax =
    preprocess
     >> parse Normal (function [] -> true | _ -> false) (fun _ -> false) []
     >> fst
     >> String_Formatting.processStringFormatting