module private Parser.StringFormatting
open Definition
open System

let buildString builder = String (List.toArray (List.rev builder))
let escapeSequences (s:string) =
  let charList = List.ofArray (s.ToCharArray())
  let rec parseCharList builder = function
    |[] -> buildString builder
    |'\\'::c::tl ->
      let k = 
        match c with
        |'\\' | '"' -> c
        |'n' -> '\n'
        |'t' -> '\t'
        |_ -> failwith "escape sequence not recognized"
      parseCharList (k::builder) tl
    |c::tl -> parseCharList (c::builder) tl
  parseCharList [] charList

let isFormatString (s:string) = s.[0] = '%' && s.Length > 1
let (|StringContents|_|) (s:string) =
  if s.Length >= 2 && s.[0] = '"' && s.[s.Length - 1] = '"'
   then Some s.[1..s.Length - 2]
   else None
let separateFormatSymbols (s:string) =
  let charList = List.ofArray (s.ToCharArray())
  // todo: more complex format rules, eg. "%5.4[1-9]"
  let rec parseCharList builder = function
    |[] -> List.rev (List.map buildString (List.filter ((<>) []) builder))
    |'%'::c::tl ->
      let formatStringBuilder = [c; '%']
      parseCharList ([]::formatStringBuilder::builder) tl
    |['%'] -> failwith "incomplete format"
    |c::tl -> parseCharList ((c::builder.Head)::builder.Tail) tl
  parseCharList [[]] charList
  
let buildFormatFunction makeFormat nonFormat =
  let rec buildFormatFunction arg ret = function
    |fmt::tl when isFormatString fmt ->
      let argName = Token (sprintf "_%i" arg)
      Token("fun", [argName;
        buildFormatFunction (arg + 1) (makeFormat (sprintf "\"%s\"" fmt) argName::ret) tl])
    |s::tl -> buildFormatFunction arg (nonFormat (sprintf "\"%s\"" s)::ret) tl
    |[] -> Token("sequence", List.rev ret)
  buildFormatFunction 1 []
     // below: incompatible with F#able
//  let buildFormatFunction =
//    let rec buildFormatFunction arg ret makeFormat nonFormat = function
//      |fmt::tl when isFormatString fmt ->
//        let argName = Token (sprintf "_%i" arg)
//        Token("fun", [argName;
//          buildFormatFunction (arg + 1) (makeFormat (sprintf "\"%s\"" fmt) argName::ret) makeFormat nonFormat tl])
//      |s::tl -> buildFormatFunction arg (nonFormat (sprintf "\"%s\"" s)::ret) makeFormat nonFormat tl
//      |[] -> Token("sequence", List.rev ret)  // |> fun e -> printfn "%A" e; e
//    buildFormatFunction 1 []
let rec mapFormatting = function
  |X("apply", [T "printfn"; T (StringContents format)]) ->
    mapFormatting (Token("apply", [Token "printf"; Token(sprintf "\"%s\\n\"" format)]))
  |X("apply", [(T "printf" | T "scanf" as formatter); T (StringContents format)]) ->
    buildFormatFunction
     (fun fmt v -> Token("apply", [Token("apply", [formatter; Token fmt]); v]))
     (fun s -> Token("apply", [formatter; Token s]))
     (separateFormatSymbols format)
  |X(s, ll) -> Token(s, List.map mapFormatting ll)
  // sprintf can be implemented differently based on medium
//    |X("apply", [T "sprintf"; T (StringContents format)]) ->
//      match buildFormatFunction (fun _ -> id) (separateFormatSymbols format) with
//      |X("sequence", list) -> Token("concatenate", list)

let rec processScan = function
  |X("apply", [X("apply", [T "scanf"; T "\"%i\""]); a]) ->
    Token("assign", [Token("dot", [a; Token("[]", [Token "0"])]); Token("apply", [Token "scan"; Token "%i"])])
  |X("apply", [X("apply", [T "scanf"; T "\"%s\""]); a]) ->
    Token("assign", [Token("dot", [a; Token("[]", [Token "0"])]); Token("apply", [Token "scan"; Token "%s"])])
  |X(s, ll) -> Token(s, List.map processScan ll)

let rec processEscapeSequences = function
  |X(StringContents s, ll) ->
    let s' = escapeSequences s
    Token(sprintf "\"%s\"" s', List.map processEscapeSequences ll)
  |X(s, ll) -> Token(s, List.map processEscapeSequences ll)

let rec processStringFormatting:Token -> Token = mapFormatting >> processScan >> processEscapeSequences