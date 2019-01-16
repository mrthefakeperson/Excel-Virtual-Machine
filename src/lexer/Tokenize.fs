module Lexer.Tokenize
open System.Text.RegularExpressions
open Lexer.Token

let WHITESPACE = "[ \t\n\r]*"

let rec (|Rgx|_|) pattern txt =
  match Regex.Match(txt, "^" + WHITESPACE + pattern).Value with
  |"" -> None
  |matched -> Some(matched :: tokenize (txt.Substring(matched.Length)))

and tokenize: string -> string list = function
  |Rgx "#[^\n]*" x  // preprocessor (TODO: do something)
  |Rgx "//[^\n]*" x  // single line comment
  |Rgx "/\*((?!\*/).)*\*/" x  // multiline comment
    -> x.Tail
  |Rgx "[0-9]+\.[0-9]*\w?" x  // float
  |Rgx "[\w]+" x
  |Rgx "\"(\\\\\\\"|[^\"])*\"" x  // string
  |Rgx "'\\\\?.'" x   // char
  |Rgx "(--|\+\+|>=|<=|==|!=|&&|\|\||&|\+=|-=|\*=|/=|&=|\|=)" x  // other tokens
  |Rgx "[{}();,=+\-*/%<>\[\]]" x
  |Rgx WHITESPACE x -> x
  |"" -> []
  |unexpected -> failwithf "Lexer: unexpected %s" unexpected

let clean_pass =
  List.fold (fun (acc, (r, c)) (s: string) ->
    let leading_whitespace = s.Substring(0, s.Length - s.TrimStart().Length)
    let trimmed_length = s.TrimStart().Length
    let (r', c') =
      Seq.fold (fun (r', c') ->
        function
        |' ' -> (r', c' + 1)
        |'\t' -> (r', c' + 4)
        |'\n' -> (r' + 1, 0)
        |_ -> (r', c')
       ) (r, c) leading_whitespace
    let acc' =
      if s.Trim() = "" then acc
      else {value = s.Trim(); line = r'; column = c'}::acc
    acc', (r', c' + trimmed_length)
   ) ([], (1, 0)) >> fst >> List.rev
