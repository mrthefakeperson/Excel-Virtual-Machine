module Lexer.Main
open RegexUtils
open Lexer.Preprocessor

type Token = {
  value: string
  line: int
  column: int
  file: SourceFile
 }
   
// TODO: test
let rec tokenize_line: int -> LexerLine -> Token list = fun col line ->
  match line.value with
  |Regex' NUM_FLOAT (hd, tl)
  |Regex' "[\w]+" (hd, tl)
  |Regex' STRING (hd, tl)
  |Regex' CHAR (hd, tl)
  |Regex' "(--|\+\+|>=|<=|==|!=|&&|\|\||&|\+=|-=|\*=|/=|&=|\|=|->)" (hd, tl)  // other tokens
  |Regex' "[{}();,=+\-*/%<>\[\].:]" (hd, tl)
  |Regex' WHITESPACE (hd, tl) ->
    {value = hd.Trim(); line = line.line; column = col + hd.Length - hd.TrimStart().Length; file = line.file}
     :: tokenize_line (col + hd.Length) {line with value = tl}
  |"" -> []
  |unexpected -> failwithf "Lexer: unexpected %s" unexpected

let tokenize: LexerLine list -> Token list =
  List.collect (tokenize_line 0) >> List.filter (function {value = ""} -> false | _ -> true)

let tokenize_text (txt: string) =
  txt
   |> preprocessor_pass "source"
   |> tokenize
