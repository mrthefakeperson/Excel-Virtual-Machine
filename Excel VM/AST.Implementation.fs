module AST.Implementation
open Project.Definitions
open Parser.Definition
open Definition

let fromToken (parseTree:Token, args:CommandLineArguments): AST*CommandLineArguments =
  if args.ContainsKey "optimizations" && args.["optimizations"] = "off"
   then Compile.transformFromToken parseTree, args
   else Compile.transformFromToken parseTree, args   // optimize, maybe pass the parameter to allow more control