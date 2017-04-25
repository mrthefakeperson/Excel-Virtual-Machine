module AST.Implementation
open Project.Util
open Parser.Definition
open Definition

let fromToken (parseTree:Token, args:CommandLineArguments): AST*CommandLineArguments =
  let transform =
    if args.ContainsKey "optimizations" && args.["optimizations"] = "off"
     then Compile.transformFromToken
     else Compile.transformFromToken >> Optimize.TCOPreprocess   // optimize, maybe pass the parameter to allow more control
  transform parseTree, args