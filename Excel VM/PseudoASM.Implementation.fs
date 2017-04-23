module PseudoASM.Implementation
open AST.Definition
open Definition
open Compile

let fromAST: AST -> PseudoASM seq = CompileToASM