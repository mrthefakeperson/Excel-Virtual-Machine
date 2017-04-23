module AST.Implementation
open Parser.Definition
open Definition
open Compile
open Optimize

let fromToken: Token -> AST = transformFromToken   //  >> validate >> optimize