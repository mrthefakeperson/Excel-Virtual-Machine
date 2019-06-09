module Transformers.Main
open Transformers.SimplifyAST
open Transformers.Tables
open Transformers.TypeCheck

let transform = pre_inference >> resolve_types >> post_inference

[<EntryPoint>]
let main argv =
  0