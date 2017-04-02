namespace Parser
open Token
open Lexer
open Lexer.CommonClassifiers
// ignore all pattern match warnings (turn this off when adding code)
#nowarn "25"

// currently a lot of issues with using ; as an end token
// ex. if (cond) return v;
//       => if (cond) (return v)     ; is used to end the return
//       => if (cond) then (return v)...failed, incomplete without a terminating ;
//  fix: don't remove the ; upon finishing parsing?
module C =
  let listOfDatatypeNames = ref ["int"; "long long"; "long"; "bool"]
  let restoreDefault() = listOfDatatypeNames := ["int"; "long long"; "long"; "bool"]
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
  let preprocess:string -> Token list =
    let isComment (s:string) =
      (isPrefix "//" s && isSuffix "\n" s)
       || (s.Length >= 4 && isPrefix "(*" s && isSuffix "*)" s)
       || (isPrefix "#" s && isSuffix "\n" s)
    let mainRules =
      singleLineCommentRules "#"
       @ singleLineCommentRules "//"
       @ delimitedCommentRules "/*" "*/"
       @ createSymbol "=="
       @ createSymbol "!="
       @ createSymbol "<="
       @ createSymbol ">="
       @ createSymbol "++"
       @ createSymbol "--"
       @ commonRules
    List.ofSeq
     >> List.map string
     >> tokenize mainRules
     >> List.collect (function       // preprocessor commands
          |e when e.[0] = '#' ->
            match e.Replace(" ", "") with
            |"#include<stdio.h>\n" -> []
            |_ -> failwith "preprocessor commands are not supported yet"
          |e -> [e]
         )
     >> List.filter        // strip whitespace, strip comments
         (negate
           (isWhitespace >>|| isDelimitedString "//" "\n" >>|| isDelimitedString "/*" "*/"))
     >> tokenizeDatatypes
     >> List.map (fun e -> Token e)
     >> List.fold (fun acc e ->
          match acc, e with
          |T "{"::_::T "struct"::_, T "}"     // find a better way to do this
          |T "{"::T "struct"::_, T "}" -> e::acc
          |T "{"::rest, T "}" -> Token ";"::rest
          |_ -> e::acc
         ) []
     >> List.rev
  let (|DatatypeName|_|) = function
    |T s::rest when List.exists ((=) s) !listOfDatatypeNames -> Some(s, rest)
    |_ -> None
  type State =
    |Global
    |FunctionArgs
    |Local
    |LocalImd    // keywords must appear at the beginning of a statement; LocalImd doesn't include them
  let rec parse state stop fail left right =
    //printfn "%A" (left, right)
    match state with
    |Global ->
      match left, right with
      |_ when stop right -> Token("sequence", List.rev left), right
      |_ when fail right -> failwithf "tokens are incomplete: %A" (left, right)
      |DatatypeGlobal state stop fail x
      |Struct state stop fail x
      |Apply state stop fail x
      |Index state stop fail x
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
//      |T ";"::T _::restl, right    //single value as a statement is meaningless
//      |T ";"::restl, right -> parse Local stop fail restl right        //handles a(b); after a(b) has been parsed
      |T ";"::_, a::restr -> parse Local stop fail (a::left) restr
      |DatatypeLocal state stop fail x
      |Brackets state stop fail x
      |Braces state stop fail x
      |If state stop fail x
      |While state stop fail x
      |For state stop fail x
      |Return state stop fail x
      |Assignment state stop fail x
      |Dot state stop fail x
      |Prefix state stop fail x
      |Operator state stop fail x
      |Apply state stop fail x
      |Index state stop fail x
      |Transfer state stop fail x     -> x
      |_ -> failwithf "unknown: %A" (left, right)
    |LocalImd ->
      match left, right with
      |_ when stop right -> Token("sequence", List.rev left), right
      |_ when fail right -> failwithf "tokens are incomplete: %A" (left, right)
//      |T ";"::T _::restl, right    //single value as a statement is meaningless !!! unless it is `break` or `return` !!!
//      |T ";"::restl, right -> parse Local stop fail restl right        //handles a(b); after a(b) has been parsed
      |T ";"::_, a::restr -> parse Local stop fail (a::left) restr
      |Brackets state stop fail x
      //|Braces state stop fail x    ({statement;}) with one pair of braces gets parsed, don't know the purpose of this
      |Assignment state stop fail x
      |Dot state stop fail x
      |Prefix state stop fail x
      |Operator state stop fail x
      |Apply state stop fail x
      |Index state stop fail x
      |CommaFunction state stop fail x
      |Transfer state stop fail x     -> x
      |_ -> failwithf "unknown: %A" (left, right)
  and (|DatatypeGlobal|_|) state stop fail = function
    |left, DatatypeName(datatypeName, restr) ->
      let identifierName, restr =
        parse Global (function T(";" | "{" | ",")::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let parsed, restr =
        match identifierName.Clean() with     // int (a);     is that valid?
        |T _ ->           // remember that `nothing` initializes to an empty struct
          Token("let", [Token("declare", [Token datatypeName; identifierName]); Token "nothing"]), restr
        |X("assign", [identifierName; value]) ->
          Token("let", [Token("declare", [Token datatypeName; identifierName]); value]), restr
        |X("dot", [identifierName; X("[]", arraySize)]) ->    // declared type should be a pointer type
          Token("let", [Token("declare", [Token datatypeName; identifierName]); Token("array", arraySize)]), restr
//        |T _ | X("sequence", [T _]) | X("assign", [_; _]) | X(",", _) | X("dot", [_; X("[]", _)]) ->
//          Token("declare", [Token datatypeName; identifierName]), restr
        |X("apply", [identifierName; args]) ->
          let X("sequence", []), functionBody::restr =
            parse Local (function X("{}", _)::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
          Token("declare function", [Token datatypeName; identifierName; args; functionBody]), restr
        |o -> failwithf "expression following data type declaration is invalid %O" o
      let restr =       // for declarations with `,`
        match restr with
        |T ","::restr -> Token ";"::Token datatypeName::restr
        |T ";"::restr | restr -> restr
      Some (parse Global stop fail left (parsed::restr))
    |_ -> None
  and (|Struct|_|) state stop fail = function
    |T "struct"::restl, right ->
      let structureTag, restr =
        match right with
        |T "{"::restr -> "anonymous structure", restr   // gets shadowed, but that's fine since it's never used
        |T s::T "{"::restr -> s, restr
        |ex -> failwithf "not a valid struct declaration: %A" ex
      listOfDatatypeNames := structureTag:: !listOfDatatypeNames
      let memberList, T "}"::restr =   // validation needed: only declarations allowed
        parse Local (function T "}"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let restr =
        match restr with
        |T ";"::restr -> restr
        |restr -> Token structureTag::restr
      let parsed = Token("struct", [Token structureTag; memberList])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|DatatypeFunction|_|) state stop fail = function           //todo: array parameters
    |left, DatatypeName(datatypeName, Pref(T s)::declaredName::restr) ->
      let parsed = Token("declare", [Token(datatypeName + s.[1..]); declaredName])
      Some (parse FunctionArgs stop fail left (parsed::restr))
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
    |DatatypeName(datatypeName, restl), right ->
    //|left, DatatypeName(datatypeName, restr) ->
      let parsed, restr =
        parse LocalImd (function T(";" | ",")::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let parsed =
        match parsed.Clean() with
        |X("assign", [T declaredName as t; value]) ->
          Token("let", [Token("declare", [Token datatypeName; t]); value])
        |T declaredName as t ->
          Token("let", [Token("declare", [Token datatypeName; t]); Token "nothing"])
        |X("dot", [T declaredName as t; X("[]", a)]) ->   // todo: declared type should be a pointer type
          Token("let", [Token("declare", [Token datatypeName; t]); Token("array", a)])
        |X("assign", [X("apply", [Pref'(T "~*"); T _ as t]); value]) ->
          Token("let", [Token("declare", [Token(datatypeName + "*"); t]); value])
        |X("apply", [Pref'(T "~*"); T _ as t]) ->
          Token("let", [Token("declare", [Token(datatypeName + "*"); t]); Token "nothing"])
        |X("dot", [X("apply", [Pref'(T "~*"); T _ as t]); X("[]", a)]) ->  // todo: declared type should be a pointer to a pointer type
          Token("let", [Token("declare", [Token(datatypeName + "*"); t]); Token("array", a)])
        |e -> failwithf "could not recognize declaration: %A" e
      let restr =     // for declarations with `,`
        match restr with
        |T ","::restr -> Token ";"::Token datatypeName::restr
        |_ -> restr
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Brackets|_|) state stop fail = function
    |T "("::restl, right ->
      let parsed, T ")"::restr =
        parse LocalImd (function T ")"::_ -> true | _ -> false) (fun _ -> false) [] right
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
      body, Token ";"::restr      //add the ; to make it consistent
    |_ ->
      let body, restr =
        parse Local (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      body, restr
  and (|If|_|) state stop fail = function
    |T "if"::restl, (T "("::_ as right) ->
      let X("sequence", []), X("()", [cond])::restr =
        parse LocalImd (function X("()", _)::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let aff, restr = getStatementBody stop fail restr
      let neg, restr =
        match restr.Tail with
        |T "else"::T "if"::restr -> parse Local stop fail [] (Token "if"::restr)  // todo: make this redundant (parsing statement body is buggy at the moment, when nested statements without braces are involved)
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
  and (|For|_|) state stop fail = function
    |T "for"::restl, T "("::restr ->
      let decl, T ";"::restr =
        parse Local (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let X("declare", [datatypeName; name]) = decl
      let cond, T ";"::restr =
        parse Local (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let incr, T ")"::restr =
        parse Local (function T ")"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let body, restr = getStatementBody stop fail restr
      let parsed = Token("sequence", [decl; Token("while", [cond; Token("sequence", [body; incr])])])
      Some (parse Local stop fail restl (parsed::restr))
    |_ -> None
  and (|Return|_|) state stop fail = function
    |T "return"::restl, right ->
      let returnedValue, restr =
        parse LocalImd (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let parsed = Token("return", [returnedValue])
      Some (parse Local stop fail restl (parsed::restr))
    |_ -> None
  and (|Assignment|_|) state stop fail = function
    |a::restl, T "="::restr ->
      let assignment, restr =
        parse LocalImd (function T ";"::_ -> true | e -> stop e) fail [] restr     //consider: a = b }    close block w/o ;
      let parsed = Token("assign", [a; assignment])
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Dot|_|) state stop fail = function
    |a::restl, T "."::T b::restr ->     // if the next token is a symbol ( eg. `(` ) then do some error handling
      let parsed = Token("dot", [a; Token b])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Prefix|_|) state stop fail = function
    |Pref a::restl, right ->     // todo: check relative indentation of prefix operator and its token
      let b, restr =
        match right with            // dANGER >:(   (untested)
        |T s::(T "["::_ as restr) ->    //special case
          let (T "()" | X("sequence", [])), b::restr =
            parse state (function T _::_ -> false | _ -> true) (fun e -> stop e || fail e) [] right
          b, restr
        |T s as b::restr when (isNumeric >>|| isVariable) s -> b, restr
        |_ ->
          let (T "()" | X("sequence", [])), b::restr =
            parse state (function T _::_ -> false | _ -> true) (fun e -> stop e || fail e) [] right
          b, restr
      let parsed = Token("apply", a.Indentation, true, [a; b])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Operator|_|) state stop fail = function
    |a::restl, (T s as nfx)::restr when nfx.Priority <> -1 ->
      let operand, restr =
        parse LocalImd
         (function T _ as x::_ when x.Priority <= nfx.Priority && x.Priority <> -1 -> true | T(";" | ",")::_ -> true | x -> stop x)
         fail [] restr
      let parsed = Token("apply", [Token("apply", [Token s; a]); operand])
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Apply|_|) state stop fail = function
    |a::restl, T "("::restr ->
      let state' = match state with Global -> FunctionArgs | _ -> LocalImd
      let args, restr =
        match parse state' (function T ")"::_ -> true | _ -> false) (fun _ -> false) [] restr with
        |X("sequence", [args]), T ")"::restr -> args, restr
        |X("sequence", []), T ")"::restr -> Token("_", []), restr
        |e -> failwithf "arguments were not formatted correctly %A" e
      let parsed = Token("apply", [a; args])
      Some (parse LocalImd stop fail restl (parsed::restr))
    |_ -> None
  and (|Index|_|) state stop fail = function
    |a::restl, T "["::restr ->
      let indexXpr, T "]"::restr =
        parse state (function T "]"::_ -> true | _ -> false) (fun _ -> false) [] restr
      let parsed = Token("dot", [a; Token("[]", [indexXpr])])
      Some (parse state stop fail restl (parsed::restr))
    |_ -> None
  and (|Transfer|_|) state stop fail = function
    |left, x::restr -> Some (parse state stop fail (x::left) restr)
    |_ -> None

  let rec postProcess = function
    |X("declare function", [datatype; name; args; xprs]) ->
      Token("let", [name; Token("fun", [postProcess args; postProcess xprs])])
    |X("{}", xprs) -> postProcess (Token("sequence", xprs))
    |X("sequence", xprs) ->
      let xprs' = List.filter (function T ";" -> false | _ -> true) xprs
      Token("sequence", List.map postProcess xprs')
    |X("==", xprs) -> Token("=", List.map postProcess xprs)
    |X("apply", [T ("printf" | "sprintf" | "scanf") as formatFunction; X(",", format::args)]) ->
      List.fold (fun acc e ->
        Token("apply", [acc; e])
       ) (Token("apply", [formatFunction; format])) args
//    |X("apply", [T "printf"; X(",", [T "\"%i\\n\""; a])]) -> Token("apply", [Token("apply", [Token "printfn"; Token "\"%i\""]); a])
//    |X("apply", [T "printf"; X(",", [T "\"%s\\n\""; a])]) -> Token("apply", [Token("apply", [Token "printfn"; Token "\"%s\""]); a])
//    |X("apply", [T "scanf"; X(",", [T "\"%i\""; a])]) -> Token("assign", [a; Token("apply", [Token "scan"; Token "%i"])])
//    |X("apply", [T "scanf"; X(",", [T "\"%s\""; a])]) -> Token("assign", [a; Token("apply", [Token "scan"; Token "%s"])])
    |X(s, xprs) -> Token(s, List.map postProcess xprs)
  let parseSyntax e =
    restoreDefault()
    preprocess e
     |> parse Global (function [] -> true | _ -> false) (fun _ -> false) []
     |> fst
     |> postProcess
     |> function X("sequence", x) -> Token("sequence", x @ [Token("apply", [Token "main"; Token "()"])])
     |> String_Formatting.processStringFormatting