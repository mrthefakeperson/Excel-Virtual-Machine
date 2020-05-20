[<AutoOpenAttribute>]
module Codewriters.Main
open Utils
open CompilerDatatypes.PseudoASM.Flat

// let write_nasm : Asm list -> string = Seq.ofList >> Nasm.write_nasm
let write_nasm (x : Asm list) : string =
  Nasm.write_nasm (Seq.ofList x)

let compile_to_nasm = Transformers.Main.transform_from_string >> Codegen.Main.generate_pasm >> write_nasm

[<EntryPoint>]
let main argv =
  let flags = CLI.get_cli_flags argv
  let inline interact repl = CLI.interact "NASM output" repl
  interact compile_to_nasm
  0