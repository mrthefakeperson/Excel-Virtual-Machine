[<AutoOpenAttribute>]
module Utils.Utils
open System.Text.RegularExpressions

let inline trace x = printfn "%A" x; x

let inline mkpair a b = (a, b)

let inline (|Strict|) x = failwithf "should never be reached (%A)" x

let normalize_newlines (s: string) = Regex.Replace(s, @"\r\n|\n\r|\n|\r", "\r\n")

let (|Regex|_|) rgx (s: string) =
  let ``match`` = Regex.Match(s, rgx)
  if ``match``.Success then Some ``match`` else None

let unescape_string (s: string) =
  let rec escapes = function
    |'\\'::rest ->
      match rest with
      |'n'::rest -> '\n'::rest
      |'t'::rest -> '\t'::rest
      |'0'::rest -> '\000'::rest
      |'\\'::rest -> '\\'::rest
      |'"'::rest -> '\"'::rest
      |_ -> failwithf "unsupported escape: %A" rest
    |hd::rest -> hd::escapes rest
    |[] -> []
  s.[1..s.Length - 2]
   |> Seq.toList
   |> escapes
   |> Seq.map string
   |> String.concat ""

module RegexUtils =
  let WHITESPACE = "\\s*"
  let PREPROCESSOR_LINE = "#[^\n]*"
  let COMMENT_LINE = "//[^\n]*"
  let COMMENT_BLOCK = "/\*((?!\*/).)*\*/"
  let VAR = "[a-z A-Z _][a-z 0-9 A-Z _]*"
  let NUM_INT32 = "-?[0-9]+"
  let NUM_INT64 = "-?[0-9]+(L|l){1,2}"
  let NUM_FLOAT = "-?[0-9]+\.[0-9]*\w?"
  let STRING = "\"(\\\\\\\"|[^\"])*\""  // "(\\\"|[^"])*"
  let CHAR = "'\\\\?.'"

module CLI =
  open System

  let interact app_name (repl: string -> 'a) =
    printfn app_name
    let rec input_block() =
      match Console.ReadLine() with
      |"quit" -> None
      |line when line.EndsWith "\\" ->
        Option.map ((+) line.[..line.Length - 2]) (input_block())
      |line -> Some line
    let rec loop() =
      printf "in > "
      match input_block() with
      |None -> ()
      |Some input ->
        try printfn "out > %A" (repl input)
        with ex -> printfn "error > %A" ex
        loop()
    loop()

  let get_cli_flags argv =
    let argv = Array.collect (fun (e: string) -> e.Split '=') argv
    let is_flag_name (s: string) = s.StartsWith "-"
    let rec parse_argv = function
      |[] -> []
      |hd::tl when is_flag_name hd ->
        let values = List.takeWhile (not << is_flag_name) tl
        (hd, values)::parse_argv (List.skip (List.length values) tl)
      |args -> parse_argv ("-unnamed"::args)
    List.ofArray argv
     |> parse_argv
     |> dict