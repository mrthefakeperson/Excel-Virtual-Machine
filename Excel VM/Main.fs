// test hyphen rules: 5-4, 5 -4, 5 - 4
open PseudoASM.Definition
open Write_File.ASM
open Write_File.Excel
open Testing.IntegrationTests
open System.IO
open System.Diagnostics

let sep (fileNameWithExtension:string) =
  match fileNameWithExtension.LastIndexOf '.' with
  | -1 -> fileNameWithExtension, ""
  |pl -> fileNameWithExtension.[..pl-1], fileNameWithExtension.[pl+1..] 
let openAndCompile: string[] -> PseudoASM[] =
  Project.Input.Implementation.fromCommandLine
   >> Parser.Implementation.fromString
   >> AST.Implementation.fromToken
   >> PseudoASM.Implementation.fromAST
   >> fst >> Array.ofSeq
[<EntryPoint>]
let main argv =
  match argv with
  |[|"test"|] ->       // `generate` to create outputs, `verify` to test, `ignore` to print
    //testExcelInterpreter verify 1
    //testPAsm verify 1
    //testExcelFile 1
    //testAsmCompilerSimple 1
    //testTypeSystem 26

    testParser verify 1
    testCompilerAST verify 1
    //testExcelCompiler 1
    //testAsmCompiler 27
    printfn "done"
    ignore (stdin.ReadLine())
  |[|"help"|] -> printfn "first argument should be the input file; -outputExcelFile outputs an Excel file"

// implement something for output
  |[|fileNameWithExtension; "-outputExcelFile"|] ->
    let fileName, extension = sep fileNameWithExtension
    openAndCompile [|fileNameWithExtension|]
     |> writeExcelFile (fileName + ".xlsx")
  |[|fileNameWithExtension|] ->
    let fileName, extension = sep fileNameWithExtension
    openAndCompile [|fileNameWithExtension|]
     |> writeBytecode (fileName + ".s")
    if not <| Process.Start("g++", sprintf "%s.s -o %s.exe" fileName fileName).WaitForExit 17000 then
      failwith "g++ not found; please install gcc before using"
  |_ -> printfn "command not recognized; the single argument \"help\" will display instructions"
    
  0

