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