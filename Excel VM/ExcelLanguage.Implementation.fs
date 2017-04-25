module ExcelLanguage.Implementation
open Project.Util
open PseudoASM.Definition
open Definition

let fromPseudoASMSeq (cmds:#seq<PseudoASM>, args:CommandLineArguments): Cell seq*CommandLineArguments =
  DefineVM.makeProgram (cmds |> Array.ofSeq) |> Seq.ofArray, args