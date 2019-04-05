module Lexer.Preprocessor
open System.IO
open System.Text.RegularExpressions
open RegexUtils

// |Regex' pattern (matched_prefix, rest)
let (|Regex'|_|) pattern (s: string) =
  Option.map (fun (m: Match) -> (m.Value, s.Substring(m.Value.Length)))
   ((|Regex|_|) ("^" + WHITESPACE + pattern) s)

// TODO: test
let rec tokenize_strip_comments: string -> string list = fun txt ->
  match txt with
  |Regex' COMMENT_LINE (cmt, tl) | Regex' COMMENT_BLOCK (cmt, tl) ->
    Regex.Replace(cmt, "[^\n]", "")::tokenize_strip_comments tl
  |Regex' PREPROCESSOR_LINE (hd, tl)
  |Regex' NUM_FLOAT (hd, tl)
  |Regex' "[\w]+" (hd, tl)
  |Regex' STRING (hd, tl)
  |Regex' CHAR (hd, tl)
  |Regex' "(--|\+\+|>=|<=|==|!=|&&|\|\||&|\+=|-=|\*=|/=|&=|\|=|->)" (hd, tl)  // other tokens
  |Regex' "[{}();,=+\-*/%<>\[\].:]" (hd, tl)
  |Regex' WHITESPACE (hd, tl) -> hd::tokenize_strip_comments tl
  |"" -> []
  |unexpected -> failwithf "Lexer: unexpected %s" unexpected

// remove comments, but disregard the contents of strings
let rec strip_comments: string -> string = tokenize_strip_comments >> String.concat ""

type SourceFile = Builtin of string | Local of string

type LexerLine = {
  value: string
  file: SourceFile
  line: int
 }

let lines: SourceFile -> string -> LexerLine list = fun src_file src ->
  src.Split '\n' |> List.ofArray
   |> List.mapi (fun i (e: string) -> {value = e.TrimEnd(); file = src_file; line = i + 1})
   |> List.rev
   |> List.fold (fun acc line ->
        match acc with
        |[] -> [line]
        |hd::tl when line.value.EndsWith(@"\") -> {line with value = line.value + hd.value}::tl
        |_ -> line::acc
       ) []
   |> List.map (fun e -> {e with value = e.value.Replace("\\\n", "")})

let (|Directive|_|) (content: string) =
  match content.TrimStart() with
  |Regex PREPROCESSOR_LINE _ ->
    let [|""; content'|] | Strict content' = content.Split([|'#'|], 2)
    let [|dir; content''|] | Strict(dir, content'') = content'.Split([|' '|], 2) 
    Some(dir, content''.Trim())
  |_ -> None

type PreprocessState = {
  macros: Map<string, string Option>
 }

let builtin_files: Map<string, LexerLine list> =
  Map <| List.map (fun (name, src) -> (name, lines (Builtin name) src)) [
    "stdio.h", ""
    "limits.h", ""
    "string.h", ""
   ]

let rec preprocessor: LexerLine list -> LexerLine list = function
  |[] -> []
  |{value = Directive("include", content)}::tl ->
    match content with
    |Regex "^<(.*)>$" m ->
      let filename = m.Groups.[1].Value
      let file =
        Map.tryFind filename builtin_files
         |> Option.defaultWith (fun () -> failwithf "could not include <%s>" filename)
      file @ preprocessor tl
    |Regex "^\"(.*)\"$" m ->
      let filename = m.Groups.[1].Value
      let file =
        try File.ReadAllText filename
        with _ -> failwithf "could not include \"%s\"" filename
         |> lines (Local filename)
      file @ preprocessor tl
    |_ -> failwith "bad include format"
  |{value = Directive("define", content)}::tl ->
    match content.Split(' ') with
    |_ -> failwith "macros not supported"
  |{value = Directive _}::_ -> failwith "unknown directive"
  |hd::tl -> hd::preprocessor tl
  
let rec preprocessor_pass: string -> string -> LexerLine list = fun src_file src ->
  lines (Local src_file) (strip_comments src) |> preprocessor
