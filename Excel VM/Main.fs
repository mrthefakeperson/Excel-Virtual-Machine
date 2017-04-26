// test hyphen rules: 5-4, 5 -4, 5 - 4
open PseudoASM.Definition
open Output.Implementation
open Testing.IntegrationTests
open System.IO
open System.Diagnostics

[<EntryPoint>]
let main argv =
  match argv with
  |[|"test"|] ->       // `generate` to create outputs, `verify` to test, `ignore` to print
    //testExcelInterpreter verify 1
    //testPAsm verify 1
    //testExcelFile 1
    //testAsmCompilerSimple 1
    //testTypeSystem 26

    //testParser verify 1
    testCompilerAST verify 1
    //testExcelCompiler 1
    //testAsmCompiler 27
    printfn "done"
    ignore (stdin.ReadLine())
  |[|"help"|] -> printfn "first argument should be the input file; -outputExcelFile outputs an Excel file"
  |args ->
    let openAndCompile: string[] -> unit =
      Project.Input.Implementation.fromCommandLine
       >> Parser.Implementation.fromString
       >> AST.Implementation.fromToken
       >> PseudoASM.Implementation.fromAST
       >> Output.Implementation.fromPseudoASM
    openAndCompile args
    
  0

