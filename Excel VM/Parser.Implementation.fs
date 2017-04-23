module Parser.Implementation
open Definition
open Lexer
open StringFormatting
open FSharpParser
open CParser
open TypeValidation

let fromInput: string -> Token = parseSyntax >> validateTypes
