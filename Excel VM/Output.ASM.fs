module private Output.ASM
open PseudoASM.Definition
open System
open System.IO

// compile to asm
let writeASM fileName cmds =
  ()