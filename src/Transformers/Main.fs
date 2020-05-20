module Transformers.Main
open CompilerDatatypes
open Transformers.SimplifyAST
open Transformers.TypeCheck

let transform = pre_inference >> resolve_types >> post_inference
let transform_from_string = Parser.Main.parse_string_to_ast >> transform

open Utils

[<EntryPoint>]
let main argv =
  CLI.interact "Transformers" (transform_from_string >> AST.pprint_ast_structure)
  0