module Lexer.Tokenize
open System.Text.RegularExpressions
open RegexUtils
open Lexer.Token

let rec (|Rgx|_|) pattern txt =
  match Regex.Match(txt, "^" + WHITESPACE + pattern).Value with
  |"" -> None
  |matched -> Some(matched :: tokenize (txt.Substring(matched.Length)))

and tokenize: string -> string list = function
  |Rgx PREPROCESSOR_LINE x  // TODO: do something
  |Rgx COMMENT_LINE x
  |Rgx COMMENT_BLOCK x -> x.Tail
  |Rgx NUM_FLOAT x
  |Rgx "[\w]+" x
  |Rgx STRING x
  |Rgx CHAR x
  |Rgx "(--|\+\+|>=|<=|==|!=|&&|\|\||&|\+=|-=|\*=|/=|&=|\|=)" x  // other tokens
  |Rgx "[{}();,=+\-*/%<>\[\].]" x
  |Rgx WHITESPACE x -> x
  |"" -> []
  |unexpected -> failwithf "Lexer: unexpected %s" unexpected

let clean_pass: string list -> Token list =
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
