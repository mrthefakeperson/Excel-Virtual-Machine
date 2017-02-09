module Parser
open Lexer
open Token

let preprocess =
  List.fold (fun ((r,c),acc) (e:string) ->
    if e.[0] = '\n' then        (r+1,e.Replace("\n", "").Length),acc
    else if ruleset " " e then  (r,c+e.Length),acc
    else if e = "." then        (r,c+e.Length),Token(e, (r,c), false, [])::acc
    else                        (r,c+e.Length),Token(e, (r,c))::acc
   ) ((1,1),[])
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

let rec parse (stop:Token list->bool) (fail:Token list->bool) left right =
  //printfn "%A" (left, right)
  match (left:Token list), (right:Token list) with
  |t::_, _ when stop right -> Token("sequence", t.Indentation, List.rev left), right
  |_ when stop right -> Token("()", (0,0), left), right
  |_ when fail right -> failwithf "unexpected end of context %A" right
  |_, (T "(" as t)::restr ->
//  |T "(" as t::restl, _ ->
    let indent =
      match left with
      |p::_ when fst p.Indentation = fst t.Indentation -> p.Indentation
      |_ -> t.Indentation
    let parsed, T ")"::restr =
      match restr with
      |T ")"::_ -> Token("()", t.Indentation, true, []), restr
      |_ ->
        parse (function T ")"::_ -> true | t'::_ when snd indent > snd t'.Indentation -> true | _ -> false)
         (fun _ -> false)    //intermediate step keywords should fail
         [] restr
    let restr = Token("()", t.Indentation, true, [parsed])::restr
    parse stop fail left restr
  //other rules
  |T "if" as t::restl, _ ->
    let cond, T "then"::restr =
      parse (function T "then"::_ -> true | _ -> false)
       (fun e -> stop e || fail e) [] right
    let aff, restr =
      parse (function T "else"::_ -> true | k when stop k -> true | t'::_ -> not (t.IndentedLess t'))
       fail [] restr
    let neg, restr =
      match restr with
      |T "else"::restr ->
        parse (function k when stop k -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
         fail [] restr
      |_ -> Token("()", t.Indentation), restr
    let parsed = Token("if", t.Indentation, [cond; aff; neg])
    parse stop fail restl (parsed::restr)
         //more testing needed
  |T "do" as t::restl, _ ->
    parse (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
     fail restl right
  |T "while" as t::restl, _ ->
    let cond, T "do"::restr =
      parse (function T "do"::_ -> true | _ -> false)
       (fun e -> stop e || fail e) [] right
    let body, restr = parse stop fail [] (Token("do", t.Indentation)::restr)
    let parsed = Token("while", t.Indentation, [cond; body])
    parse stop fail restl (parsed::restr)
  |T "for" as t::restl, _ ->
    let name, T "in"::restr =
      parsePattern (function T "in"::_ -> true | _ -> false)
       (fun e -> stop e || fail e) [] right
    let iterable, restr =
      match parse (function T("do" | "..")::_ -> true | _ -> false)
       (fun e -> stop e || fail e) [] restr with
      |iterable, T "do"::restr -> iterable, restr
      |a, T ".."::restr ->
        match parse (function T("do" | "..")::_ -> true | _ -> false)
         (fun e -> stop e || fail e) [] restr with
        |b, T "do"::restr -> Token("..", [a; Token("1", []); b]), restr
        |step, T ".."::restr ->
          let b, T "do"::restr =
            parse (function T "do"::_ -> true | _ -> false)
             (fun e -> stop e || fail e) [] restr
          Token("..", [a; step; b]), restr
    let body, restr = parse stop fail [] (Token("do", t.Indentation)::restr)
    let parsed = Token("for", t.Indentation, [name; iterable; body])
    parse stop fail restl (parsed::restr)
  |T "let" as t::restl, _ ->
    let name, T "="::restr =
      parsePattern (function T "="::_ -> true | _ -> false)
       (fun e -> stop e || fail e) [] right
    let body, restr =
      parse (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
       fail [] restr
    let parsed = Token("let", t.Indentation, [name; body])
    parse stop fail restl (parsed::restr)
  |T "fun" as t::restl, _ ->
    let name, T "->"::restr =
      parsePattern (function T "->"::_ -> true | _ -> false)
       (fun e -> stop e || fail e) [] right
    let body, restr =
      parse (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
       fail [] restr
    let parsed = Token("fun", t.Indentation, [name; body])
    parse stop fail restl (parsed::restr)
  |T "|" as t::restl, _ ->
    let pattern, T "->"::restr =
      parsePattern (function T "->"::_ -> true | _ -> false)
       (fun e -> stop e || fail e)    //buggy infixes: test      a+b; |c & d ->        (maybe move infixes up)
       [] right
    let body, restr =
      parse (function e when stop e -> true | t'::_ -> not (t.IndentedLess t') | _ -> false)
       fail [] restr
    let parsed = Token("pattern", t.Indentation, [pattern; body])
    parse stop fail restl (parsed::restr)
  |T "match" as t::restl, _ ->
    let target, T "with"::restr =
      parsePattern (function T "with"::_ -> true | _ -> false)
       (fun e -> stop e || fail e) restl right
    let restr =
      match restr with
      |T "|"::_ -> restr
      |_ -> Token("|", t.Indentation)::restr
    let cases, restr = parse stop fail [] restr
    let parsed = Token("match", t.Indentation, [target; cases])
    parse stop fail restl (parsed::restr)
  |T "function" as t::restl, _ ->
    let restr =
      match right with
      |T "|"::_ -> right
      |_ -> Token("|", t.Indentation)::right
    let cases, restr = parse stop fail [] restr
    let parsed = Token("function", t.Indentation, [cases])
    parse stop fail restl (parsed::restr)
         //todo: `type`

  //dot
  |T "."::(A as a)::restl, (A as b)::restr ->  //not perfect: catches bracketed expressions as weird dotted accesses
    let parsed = Token("dot", a.Indentation, true, [a; b])
    parse stop fail restl (parsed::restr)
  //fix priority wrt apply rule
  |_, a::(T "." as t)::restr ->
    parse stop fail (t::a::left) restr
  //infix (list of infixes doesn't overlap with previous matches)
  //remember to change the indentation of infixes in ``preprocess``
  |_, (T _ as nfx)::restr when nfx.Priority > -1 ->
    let a::restl =
      match left with
      |[] -> failwith "infix applied, but no preceding argument"
      |_ -> left
    let b,restr =
      parse (function nfx'::_ when nfx.EvaluatedFirst nfx' -> true | k -> stop k)
       fail [] restr
    match b.Name, b.Dependants with
    |"sequence", parsed::r ->
      let parsed = Token("apply", a.Indentation, [Token("apply", a.Indentation, [nfx; a]); parsed])
      parse stop fail restl (parsed::r @ restr)
    |_ -> failwith "infix failed"
    //let parsed = Token("apply", a.Indentation, [Token("apply", a.Indentation, [nfx; a]); b])
    //parse stop fail restl (parsed::restr)
  //apply rule
  |(A as a)::restl, (A as b)::restr when a.IndentedLess b ->
    let restr = Token("apply", a.Indentation, true, [a; b])::restr
    parse stop fail restl restr
  //transfer rule
  |_, x::restr ->
    parse stop fail (x::left) restr
  |_, [] -> Token("()", (0,0)), []
and parsePattern stop fail left right =
  //printfn "%A" (left,right)
  match left, (right:Token list) with
  |[p], _ when stop right -> p, right
  |_ when stop right -> failwith "no pattern"
  |_ when fail right -> failwithf "unexpected end of context %A" right
  |_, (T "(" as t)::restr ->
    let parsed, T ")"::restr =
      match restr with
      |T ")"::_ -> Token("()", t.Indentation, true, []), restr
      |_ -> parsePattern (function T ")"::_ -> true | _ -> false) (fun _ -> false) [] restr
    let parsed = Token("()", t.Indentation, true, [parsed])
    parsePattern stop fail left (parsed::restr)
  |_, (T ("|" | "&" as s) as nfx)::restr ->
    let a::restl =
      match left with
      |[] -> failwith "pattern infix rule applied with no preceeding argument"
      |_ -> left
    let b, restr =
      parsePattern (function T ("|" | "&")::_ -> true | k -> stop k) fail [] restr
    let parsed = Token(s, a.Indentation, [a; b])
    parsePattern stop fail restl (parsed::restr)
  |(A as a)::restl, (A as b)::restr when a.IndentedLess b ->
    let restr = Token("apply", a.Indentation, true, [a; b])::restr
    parsePattern stop fail restl restr
  |[], a::restr -> parsePattern stop fail (a::left) restr
  |k -> printfn "failed to match %A" k; failwith ""
  //todo: `;`, `[]`, `[||]`, tupled arguments
