module Lexer.Main
open Lexer.Tokenize
open Lexer.Preprocessor

let tokenize_text (txt: string) =
  txt
   |> tokenize
   |> clean_pass
   |> preprocessor_pass
