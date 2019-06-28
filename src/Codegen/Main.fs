module Codegen.Main
open Utils

let generate_pasm = GenPAsm.generate_pasm

[<EntryPoint>]
let main argv =
  CLI.interact "Code Generator" (Transformers.Main.transform_from_string >> generate_pasm)
  0