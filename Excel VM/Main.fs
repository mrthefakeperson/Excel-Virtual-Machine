open Compiler
open Write_File
open Testing
open System.IO
open System.Diagnostics

let sep (fileNameWithExtension:string) =
  match fileNameWithExtension.LastIndexOf '.' with
  | -1 -> fileNameWithExtension, ""
  |pl -> fileNameWithExtension.[..pl-1], fileNameWithExtension.[pl+1..] 
let openAndParse file =
  let txt = File.ReadAllText file
  let _, extension = sep file
  let parseSyntax =
    match extension with
    |"fs" | "fsx" -> Parser.FSharp.parseSyntax
    |"py" -> Parser.Python2.parseSyntax
    |_ -> Parser.C.parseSyntax
  txt
   |> Lexer.groupByRuleset
   |> parseSyntax
   |> fun e -> e.Clean()
   |> Type_System.applyTypeSystem
[<EntryPoint>]
let main argv =
  match argv with
  |[|"test"|] -> runSpecificTest()
  |[|"help"|] -> printfn "first argument should be the input file; -outputExcelFile outputs an Excel file"
  |[|fileNameWithExtension; "-outputExcelFile"|] ->
    let fileName, extension = sep fileNameWithExtension
    let parsed = openAndParse fileNameWithExtension
    parsed.Clean()
     |> ASTCompile |> debug
     |> compile
     |> Array.ofList
     |> writeExcelFile (fileName + ".xlsx")
  |[|fileNameWithExtension|] ->
    let fileName, extension = sep fileNameWithExtension
    let parsed = openAndParse fileNameWithExtension
    parsed.Clean()
     |> ASTCompile
     |> compile
     |> Array.ofList
     |> writeBytecode (fileName + ".s")
    if not <| Process.Start("g++", sprintf "%s.s -o %s.exe" fileName fileName).WaitForExit 17000 then
      failwith "g++ not found; please install gcc before using"
  |_ -> printfn "command not recognized; the single argument \"help\" will display instructions"
    
  0

