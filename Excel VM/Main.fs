open Compiler
open Write_File
open Testing
open System.IO

[<EntryPoint>]
let main argv =
  match argv with
  |[|"test"|] -> runSpecificTest()
  |[|"help"|] -> printfn "first argument should be the input file; -outputExcelFile outputs an Excel file"
  |[|fileName|] ->
    let openAndParse file =
      let txt = File.ReadAllText file
      let parseSyntax =
        let last (a:'t[]) = if a.Length > 0 then Some a.[a.Length-1] else None
        match last (file.Split('.')) with
        |Some ("fs" | "fsx") -> Parser.FSharp.parseSyntax
        |Some "py" -> Parser.Python2.parseSyntax
        |_ -> Parser.C.parseSyntax
      txt
       |> Lexer.groupByRuleset
       |> parseSyntax
    let parsed = openAndParse fileName
    parsed.Clean()
     |> ASTCompile
     |> compile
     |> Array.ofList
     |> writeBytecode (fileName + ".s")
  |[|fileName; "-outputExcelFile"|] ->
    let parsed = openAndParse fileName
    parsed.Clean()
     |> ASTCompile
     |> compile
     |> Array.ofList
     |> writeExcelFile (fileName + ".xlsx")
  |_ -> printfn "command not recognized; the single argument \"help\" will display instructions"
    
  0

