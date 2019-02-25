module Codewriters.Nasm
open Codegen.PAsm
open Codegen.Interpreter

let write_nasm (instrs: Boxed Asm list) : string =
  ""