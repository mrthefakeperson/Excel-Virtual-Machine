module PseudoASM.Implementation
open Project.Definitions
open AST.Definition
open Definition

let fromAST (ast:AST, args:CommandLineArguments): PseudoASM seq*CommandLineArguments =
  Compile.CompileToASM ast, args