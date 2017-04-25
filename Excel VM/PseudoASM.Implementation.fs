module PseudoASM.Implementation
open Project.Util
open AST.Definition
open Definition

let fromAST (ast:AST, args:CommandLineArguments): PseudoASM seq*CommandLineArguments =
  Compile.CompileToASM ast, args