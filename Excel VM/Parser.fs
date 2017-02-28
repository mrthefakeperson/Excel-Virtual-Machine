namespace Parser
open Lexer
open Token

module FSharp =
  let preprocess =
    List.fold (fun ((r,c),acc) (e:string) ->
      if e.[0] = '\n' then        (r+1,e.Replace("\n", "").Length),acc
      else if ruleset " " e then  (r,c+e.Length),acc
      else if e = "." then        (r,c+e.Length),Token(e, (r,c), false, [])::acc
      else                        (r,c+e.Length),Token(e, (r,c))::acc
     ) ((1,0),[])
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

module Python2 =
  let preprocess: string list->Token list list =
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


module C =
  let listOfDatatypeNames = ref ["int"; "long long"; "long"]
  let (|BrokenDatatypeName|_|) (ll:string list) =
    let matchString (s:string) =
      let matching = s.Split ' ' |> Array.toList
      let rec findMatch = function
        |[], x -> Some (s, x)
        |a::resta, b::restb when a = b -> findMatch (resta, restb)
        |_ -> None
      findMatch (matching, ll)
    List.map matchString !listOfDatatypeNames
     |> List.tryFind (function Some _ -> true | None -> false)
     |> function Some yld -> yld | None -> None
  let rec tokenizeDatatypes = function
    |[] -> []
    |BrokenDatatypeName(hd, tl)
    |hd::tl -> hd::tokenizeDatatypes tl
  let preprocess =
    List.filter (ruleset " " >> not)
     >> tokenizeDatatypes
     >> List.map (fun e -> Token e)
  let (|DatatypeName|_|) = function
    |T s::rest when List.exists ((=) s) !listOfDatatypeNames -> Some(s, rest)
    |_ -> None
  //todo: preprocessor commands
  type State =
    |Global
    |FunctionArgs
    |Local
    |LocalImd    //keywords must appear at the beginning of a statement; LocalImd doesn't include them
  let rec parse state stop fail left right =
    //printfn "%A: %A" state (left, right)
    match state with
    |Global ->
      match left, right with
      |_ when stop right -> Token("sequence", List.rev left), right
      |_ when fail right -> failwithf "tokens are incomplete: %A" (left, right)
      |DatatypeGlobal state stop fail x
      //|Struct
      |Apply state stop fail x
      |Brackets state stop fail x
      |Braces state stop fail x
      |Assignment state stop fail x
      |Operator state stop fail x
      |Transfer state stop fail x     -> x
      |_ -> failwithf "unknown: %A" (left, right)
    |FunctionArgs ->
      match left, right with
      |_ when stop right -> Token("sequence", List.rev left), right
      |_ when fail right -> failwithf "tokens are incomplete: %A" (left, right)
      |DatatypeFunction state stop fail x
      |CommaFunction state stop fail x
      |Transfer state stop fail x     -> x
      |_ -> failwithf "unknown: %A" (left, right)
    |Local ->
      match left, right with
      |_ when stop right -> Token("sequence", List.rev left), right
      |_ when fail right -> failwithf "tokens are incomplete: %A" (left, right)
      |T ";"::T _::restl, right    //single value as a statement is meaningless
      |T ";"::restl, right -> parse Local stop fail restl right        //handles a(b); after a(b) has been parsed
      |Brackets state stop fail x
      |Braces state stop fail x
      |If state stop fail x
      |While state stop fail x
//      |For state stop fail x
      |Return state stop fail x
      |Assignment state stop fail x
      |Operator state stop fail x
      |Apply state stop fail x
      |DatatypeLocal state stop fail x         //do this better: preprocess all datatypes into one string
      |Transfer state stop fail x     -> x
      |_ -> failwithf "unknown: %A" (left, right)
    |LocalImd ->
      match left, right with
      |_ when stop right -> Token("sequence", List.rev left), right
      |_ when fail right -> failwithf "tokens are incomplete: %A" (left, right)
      |T ";"::T _::restl, right    //single value as a statement is meaningless !!! unless it is `break` or `return`+ !!!
      |T ";"::restl, right -> parse Local stop fail restl right        //handles a(b); after a(b) has been parsed
      |Brackets state stop fail x
      //|Braces state stop fail x    ({statement;}) with one pair of braces gets parsed, don't know the purpose of this
      |Assignment state stop fail x
      |Operator state stop fail x
      |Apply state stop fail x
      |CommaFunction state stop fail x
      |Transfer state stop fail x     -> x
      |_ -> failwithf "unknown: %A" (left, right)
  and (|DatatypeGlobal|_|) state stop fail = function
    |left, DatatypeName(datatypeName, restr) ->
      let identifierName, restr =
        parse Global (function T(";" | "{")::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let parsed, restr =
        match identifierName with
        |T _ | X("assign", [_; _]) | X(",", _) -> Token("declare", [Token datatypeName; identifierName]), restr.Tail
        |X("sequence", [X("apply", [identifierName; args])]) ->
          let X("sequence", []), functionBody::restr =
            parse Local (function X("{}", _)::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
          Token("declare function", [Token datatypeName; identifierName; args; functionBody]), restr
        |o -> failwithf "expression following data type declaration is invalid %O" o
      Some (parse Global stop fail left (parsed::restr))
    |_ -> None
  and (|DatatypeFunction|_|) state stop fail = function
    |left, DatatypeName(datatypeName, declaredName::restr) ->
      let parsed = Token("declare", [Token datatypeName; declaredName])
      Some (parse FunctionArgs stop fail left (parsed::restr))
    |_ -> None
  and (|CommaFunction|_|) state stop fail = function
    |a::restl, T ","::restr ->
      let parsed, restr = parse state stop fail [] restr
      let parsed =
        match parsed with
        |X(",", args) -> Token(",", a::args)
        |_ -> Token(",", [a; parsed])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|DatatypeLocal|_|) state stop fail = function
    |left, DatatypeName(datatypeName, restr) ->
      let parsed, restr =
        parse LocalImd (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let parsed =
        match parsed.Clean() with
        |X("assign", [T declaredName; value]) ->
          Token("let", [Token("declare", [Token datatypeName; Token declaredName]); value])
        |T declaredName ->
          Token("let", [Token("declare", [Token datatypeName; Token declaredName]); Token "nothing"])
        |e -> failwithf "could not recognize declaration: %A" e
      Some (parse LocalImd stop fail left (parsed::restr))
    |_ -> None
  and (|Brackets|_|) state stop fail = function
    |T "("::restl, right ->
      let parsed, T ")"::restr =
        parse LocalImd (function T ")"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let parsed = Token("()", [parsed])
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Braces|_|) state stop fail = function
    |T "{"::restl, right ->
      let parsed, T "}"::restr =
        parse Local (function T "}"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let parsed = Token("{}", [parsed])
      Some (parse Local stop fail restl (parsed::restr))
    |_ -> None
  and getStatementBody stop fail right =
    match right with
    |T "{"::_ ->
      let X("sequence", []), X("{}", [body])::restr =
        parse Local (function X("{}", [_])::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] right
      body, restr
    |_ ->
      let body, T ";"::restr =
        parse Local (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      body, restr
  and (|If|_|) state stop fail = function
    |T "if"::restl, (T "("::_ as right) ->
      let X("sequence", []), X("()", [cond])::restr =
        parse LocalImd (function X("()", _)::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let aff, restr = getStatementBody stop fail restr
      let neg, restr =
        match restr with
        |T "else"::restr -> getStatementBody stop fail restr
        |_ -> Token("sequence", []), restr
      let parsed = Token("if", [cond; aff; neg])
      Some (parse Local stop fail restl (parsed::restr))
    |_ -> None
  and (|While|_|) state stop fail = function
    |T "while"::restl, (T "("::_ as right) ->
      let X("sequence", []), X("()", [cond])::restr =
        parse LocalImd (function X("()", _)::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let body, restr = getStatementBody stop fail restr
      let parsed = Token("while", [cond; body])
      Some (parse Local stop fail restl (parsed::restr))
    |_ -> None
  and (|Return|_|) state stop fail = function
    |T "return"::restl, right ->
      let returnedValue, T ";"::restr =
        parse LocalImd (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let parsed = Token("return", [returnedValue])
      Some (parse Local stop fail restl (parsed::restr))
    |_ -> None
  and (|Assignment|_|) state stop fail = function
    |a::restl, T "="::restr ->
      let assignment, restr =
        parse LocalImd (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let parsed = Token("assign", [a; assignment])
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Operator|_|) state stop fail = function
    |a::restl, (T s as nfx)::restr when nfx.Priority <> -1 ->
      let operand, restr =
        parse LocalImd
         (function T _ as x::_ when x.Priority <= nfx.Priority && x.Priority <> -1 -> true | x -> stop x)
         fail [] restr
      let parsed = Token("apply", [Token("apply", [Token s; a]); operand])
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Apply|_|) state stop fail = function
    |a::restl, T "("::restr ->
      let state' = match state with Global -> FunctionArgs | _ -> LocalImd
      let args, restr =
        match parse state' (function T ")"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr with
        |X("sequence", [args]), T ")"::restr -> args, restr
        |X("sequence", []), T ")"::restr -> Token("_", []), restr
        |e -> failwithf "arguments were not formatted correctly %A" e
      let parsed = Token("apply", [a; args])
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Transfer|_|) state stop fail = function
    |left, x::restr -> Some (parse state stop fail (x::left) restr)
    |_ -> None

  let rec postProcess = function
    |X("declare function", [datatype; name; args; xprs]) ->
      Token("let", [name; Token("fun returnable", [postProcess args; postProcess xprs])])
    |X("{}", xprs) -> postProcess (Token("sequence", xprs))
    |X("sequence", xprs) ->
      let xprs' = List.filter (function T ";" -> false | _ -> true) xprs
      Token("sequence", List.map postProcess xprs')
    |X(s, xprs) -> Token(s, List.map postProcess xprs)

  let parseSyntax =
    preprocess
     >> parse Global (function [] -> true | _ -> false) (fun _ -> false) []
     >> fst
     >> postProcess
     >> function X("sequence", x) -> Token("sequence", x @ [Token("apply", [Token "main"; Token "()"])])