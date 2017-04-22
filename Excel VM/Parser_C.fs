namespace Parser
open Token
open Lexer
open Lexer.CommonClassifiers
// ignore all pattern match warnings (turn this off when adding code)
#nowarn "25"

module C =
  let preprocess:string -> Token list =
    let mainRules =
      createSingleLineComment "#"
       @ createSingleLineComment "//"
       @ createDelimitedComment "/*" "*/"
       @ createStrings
       @ createSymbol "==" @ createSymbol "!=" @ createSymbol "<=" @ createSymbol ">="
       @ createSymbol "++" @ createSymbol "--"
       @ createVariablesAndNumbers
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
     >> List.map (fun e -> Token e)
  let listOfDatatypeNamesDefault =    // can be simplified if all datatypes are 1 or 2 tokens
    List.sortBy (fun (e:string) -> -(e.Split ' ').Length) [
      "int"; "long long"; "long"; "bool"; "char"; "unsigned"; "unsigned int";
      "unsigned long int"; "unsigned long long int"; "long int"; "long long int"
     ]
  let listOfDatatypeNames = ref listOfDatatypeNamesDefault
  let restoreDefault() = listOfDatatypeNames := listOfDatatypeNamesDefault
  type State =
    |Global
    |FunctionArgs
    |Local
    |LocalImd    // keywords must appear at the beginning of a statement; LocalImd doesn't include them
  let rec parse state stop fail left right =
//    printfn "%A" (left, right)
    match stop right, fail right, state with
    |true, _, _ -> Token("sequence", List.rev left), right
    |_, true, _ -> failwithf "tokens are incomplete: %A" (left, right)
    |_ ->
      let continueParsing x =
        let state', left', right' = x
        parse state' stop fail left' right'
      match state with
      |Global ->
        match left, right with
        |DatatypeGlobal state stop fail x
        |Struct state stop fail x
        |Apply state stop fail x
        |Index state stop fail x
        |Brackets state stop fail x
        |Braces state stop fail x
        |Assignment state stop fail x
        |Operator state stop fail x
        |Transfer state stop fail x     -> continueParsing x
        |_ -> failwithf "unknown: %A" (left, right)
      |FunctionArgs ->
        match left, right with
        |DatatypeFunction state stop fail x
        |CommaFunction state stop fail x
        |Transfer state stop fail x     -> continueParsing x
        |_ -> failwithf "unknown: %A" (left, right)
      |Local ->
        match left, right with
//        |T ";"::T _::restl, right    //single value as a statement is meaningless
//        |T ";"::restl, right -> parse Local stop fail restl right        //handles a(b); after a(b) has been parsed
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
        |Transfer state stop fail x     -> continueParsing x
        |_ -> failwithf "unknown: %A" (left, right)
      |LocalImd ->
        match left, right with
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
        |Transfer state stop fail x     -> continueParsing x
        |_ -> failwithf "unknown: %A" (left, right)
  // match a string with a part of the tokenized string list
  and matchString tokens stringToStringList (s:string) =
    let rec findMatch = function
      |[], x -> Some (s, x)
      |a::resta, T b::restb when a = b -> findMatch (resta, restb)
      |_ -> None
    findMatch (``stringToStringList`` s, tokens)
  // matches if the tokens at the top of the right stack make one of the known datatypes
  and (|DatatypeName|_|) right =
    List.tryPick (matchString right (fun e -> List.ofArray (e.Split ' '))) !listOfDatatypeNames
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
        |X("apply", [identifierName; args]) ->
          let X("sequence", []), functionBody::restr =
            parse Local (function X("{}", _)::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
          Token("declare function", [Token datatypeName; identifierName; args; functionBody]), restr
        |o -> failwithf "expression following data type declaration is invalid %O" o
      let restr =       // for declarations with `,`
        match restr with
        |T ","::restr -> Token ";"::List.ofArray (datatypeName.Split ' ' |> Array.map (fun e -> Token e)) @ restr
        |T ";"::restr | restr -> restr
      Some (Global, left, parsed::restr)
    |_ -> None
  and (|Struct|_|) state stop fail = function
    |T "struct"::restl, right ->
      let structureTag, restr =
        match right with
        |T "{"::restr -> "anonymousStruct", restr   // gets shadowed, but that's fine since it's never used
        |T s::T "{"::restr -> s, restr
        |ex -> failwithf "not a valid struct declaration: %A" ex
      listOfDatatypeNames := "struct " + structureTag:: !listOfDatatypeNames
//      printfn "datatype names: %A" !listOfDatatypeNames
      let memberList, restr = (|NextBracePair|) Local stop fail ([Token "{"], restr)  // validation needed: only declarations allowed
      let restr =
        match restr with
        |T ";"::restr -> restr
        |restr -> Token "struct"::Token structureTag::restr
      let parsed = Token("struct", [Token structureTag; memberList])
      Some (state, restl, parsed::restr)
    |_ -> None
  and (|DatatypeFunction|_|) state stop fail = function           //todo: array parameters
    |left, DatatypeName(datatypeName, Pref(T s)::declaredName::restr) ->
      let parsed = Token("declare", [Token(datatypeName + s.[1..]); declaredName])
      Some (FunctionArgs, left, parsed::restr)
    |left, DatatypeName(datatypeName, declaredName::restr) ->
      let parsed = Token("declare", [Token datatypeName; declaredName])
      Some (FunctionArgs, left, parsed::restr)
    |_ -> None
  and (|CommaFunction|_|) state stop fail = function
    |a::restl, T ","::restr ->
      let parsed, restr = parse state stop fail [] restr
      let parsed =
        match parsed with
        |X("sequence", [X(",", args)]) -> Token(",", a::args)
        |_ -> Token(",", [a; parsed])
      Some (state, restl, parsed::restr)
    |_ -> None
  and (|DatatypeNameL|_|) = function
    |[], _ -> None
    |hd::restl, right ->
      match hd::right with
      |DatatypeName(datatypeName, restr) -> Some(restl, datatypeName, restr)
      |_ -> None
  and (|DatatypeLocal|_|) state stop fail = function
    |DatatypeNameL(restl, datatypeName, right) ->
      let parsed, restr =
        parse LocalImd (function T(";" | ",")::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let parsed =
        let pars = match parsed with X("sequence", [pars]) -> pars | _ -> failwith "unexpected"
        match pars with
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
      Some (LocalImd, restl, parsed::restr)
    |_ -> None
  and (|NextBracketPair|) state stop fail = function
    |T "("::restl, right ->
      let parsed, T ")"::restr =
        parse LocalImd (function T ")"::_ -> true | _ -> false) ((=) []) [] right
      parsed, restr
    |_ -> failwith "only use this when a bracket is at the top of the left stack"
  and (|NextBracePair|) state stop fail = function   // consider infixes: a + { ... } -> error
    |T "{"::restl, right ->
      let parsed, T "}"::restr =
        parse Local (function T "}"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      parsed, restr
    |_ -> failwith "only use this when a brace is at the top of the left stack"
  and (|Brackets|_|) state stop fail = function
    |(T "("::restl, _) & NextBracketPair state stop fail (parsed, restr) ->
      let parsed = Token("()", [parsed])
      Some (state, restl, parsed::restr)
    |_ -> None
  and (|Braces|_|) state stop fail = function
    |(T "{"::restl, right) & NextBracePair state stop fail (parsed, restr) ->
      let parsed = Token("{}", [parsed])
      Some (state, restl, parsed::restr)
    |_ -> None
  and getNextStatement state stop fail = function
    |T ";"::restr -> Token "()", restr
    |hd::tl ->
      let left, right = [hd], tl
      match left, right with
      |_ when stop right || fail right -> failwithf "tokens are incomplete: %A" (left, right)
      |Braces state stop fail (_, [], X("{}", [x])::restr) -> x, restr
      |If state stop fail (_, [], x::restr)
      |While state stop fail (_, [], x::restr)
      |For state stop fail (_, [], x::restr)     -> x, restr
      |_ ->
        let parsed, T ";"::restr =
          parse state (function T ";"::_ -> true | _ -> false)
           (fun e -> stop e || fail e) left right
        parsed, restr
    |[] -> failwith "invalid end of file"
  and (|If|_|) state stop fail = function
    |T "if"::restl, right ->
      let cond, restr =
        match right with
        |T "("::restr -> (|NextBracketPair|) LocalImd stop fail ([Token "("], restr)
        |_ -> failwith "( expected after if"
      let aff, restr = getNextStatement state stop fail restr
      let neg, restr =
        match restr with
        |T "else"::restr -> getNextStatement state stop fail restr
        |_ -> Token "()", restr
      let parsed = Token("if", [cond; aff; neg])
      Some (Local, restl, parsed::restr)   
    |_ -> None
  and (|While|_|) state stop fail = function
    |T "while"::restl, (T "("::_ as right) ->
      let cond, restr =
        match right with
        |T "("::restr -> (|NextBracketPair|) LocalImd stop fail ([Token "("], restr)
        |_ -> failwith "( expected after while"
      let body, restr = getNextStatement state stop fail restr
      let parsed = Token("while", [cond; body])
      Some (Local, restl, parsed::restr)
    |_ -> None
  and (|For|_|) state stop fail = function
    |T "for"::restl, T "("::restr ->
      let decl, T ";"::restr =
        parse Local (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      //let X("declare", [datatypeName; name]) = decl
      let cond, T ";"::restr =
        parse Local (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let incr, T ")"::restr =
        parse Local (function T ")"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] restr
      let body, restr = getNextStatement state stop fail restr
      let parsed = Token("sequence", [decl; Token("while", [cond; Token("sequence", [body; incr])])])
      Some (Local, restl, parsed::restr)
    |_ -> None
  and (|Return|_|) state stop fail = function
    |T "return"::restl, right ->
      let returnedValue, restr =
        parse LocalImd (function T ";"::_ -> true | _ -> false) (fun e -> stop e || fail e) [] right
      let returnedValue = match returnedValue with T "sequence" -> Token "()" | x -> x
      let parsed = Token("return", [returnedValue])
      Some (Local, restl, parsed::restr)
    |_ -> None
  and (|Assignment|_|) state stop fail = function
    |a::restl, T "="::restr ->
      let assignment, restr =
        parse LocalImd (function T ";"::_ -> true | e -> stop e) fail [] restr     //consider: a = b }    close block w/o ;
      let parsed = Token("assign", [a; assignment])
      Some (LocalImd, restl, parsed::restr)
    |_ -> None
  and (|Dot|_|) state stop fail = function
    |a::restl, T "."::T b::restr ->     // if the next token is a symbol ( eg. `(` ) then do some error handling
      let parsed = Token("dot", [a; Token b])
      Some (state, restl, parsed::restr)
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
      Some (state, restl, parsed::restr)
    |_ -> None
  and (|Operator|_|) state stop fail = function
    |a::restl, (T s as nfx)::restr when nfx.Priority <> -1 ->
      let operand, restr =
        parse LocalImd
         (function T _ as x::_ when x.Priority <= nfx.Priority && x.Priority <> -1 -> true | T(";" | ",")::_ -> true | x -> stop x)
         fail [] restr
      let parsed = Token("apply", [Token("apply", [Token s; a]); operand])
      Some (LocalImd, restl, parsed::restr)
    |_ -> None
  and (|Apply|_|) state stop fail = function
    |X(("{}" | "if" | "while" | "for"), _)::_, _ -> None   // don't apply anything to these
    |a::restl, T "("::restr ->
      let state' = match state with Global -> FunctionArgs | _ -> LocalImd
      let args, restr =
        match parse state' (function T ")"::_ -> true | _ -> false) (fun _ -> false) [] restr with
        |X("sequence", [args]), T ")"::restr -> args, restr
        |X("sequence", []), T ")"::restr -> Token("()", []), restr
        |e -> failwithf "arguments were not formatted correctly %A" e
      let parsed = Token("apply", [a; args])
      Some (LocalImd, restl, parsed::restr)
    |_ -> None
  and (|Index|_|) state stop fail = function
    |a::restl, T "["::restr ->
      let indexXpr, T "]"::restr =
        parse state (function T "]"::_ -> true | _ -> false) (fun _ -> false) [] restr
      let parsed = Token("dot", [a; Token("[]", [indexXpr])])
      Some (state, restl, parsed::restr)
    |_ -> None
  and (|Transfer|_|) state stop fail = function
    |left, x::restr -> Some (state, x::left, restr)
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
    |X(s, xprs) -> Token(s, List.map postProcess xprs)
  let parseSyntax e =
    restoreDefault()
    preprocess e
     |> parse Global (function [] -> true | _ -> false) (fun _ -> false) []
     |> fst
     |> postProcess
     |> function X("sequence", x) -> Token("sequence", x @ [Token("apply", [Token "main"; Token "()"])])
     |> String_Formatting.processStringFormatting
     |> fun e -> printfn "%A" e; e