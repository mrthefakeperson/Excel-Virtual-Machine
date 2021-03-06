﻿namespace Testing
open PseudoASM.Definition
open ExcelLanguage.Definition
open Output.Implementation
open Interpreters
open System
open System.IO

module IntegrationTests =
  [<Literal>]
  let DEBUG_INTERPRETER = false
  [<Literal>]
  let PAUSE_BETWEEN_TESTS = false

  let logPrintf file =
    let pr = new StreamWriter(file, true)
    Printf.kprintf (fun e ->
      pr.Write e
      pr.Flush()
      pr.Close()
      printf "%s" e
     )

  let rec test action folderName runFile n =
    let name = sprintf @"%s\%s\in%i.txt" __SOURCE_DIRECTORY__ folderName n
    let outName = sprintf @"%s\%s\out%i.txt" __SOURCE_DIRECTORY__ folderName n
    File.Delete outName
    if File.Exists name then
      printfn "%s:" name
      runFile name outName
      action outName
      if PAUSE_BETWEEN_TESTS then ignore (stdin.ReadLine())
      test action folderName runFile (n + 1)
  let verify name =
    if (File.ReadAllLines name) <> (File.ReadAllLines (name + ".ans"))
     then
      printfn "\n\nincorrect file: %s\n" name
      stdin.ReadLine() |> ignore
     else printfn "\n\ngood\n"
  let generate name =
    if File.Exists (name + ".ans") then File.Delete (name + ".ans")
    File.Copy(name, name + ".ans")
      
  let debug e = printfn "%O" e; e
  let debugList e = printfn "[%s]" (String.concat "; " (List.map (sprintf "%O") e)); e
  let interpret iterations cells =
    Array.map (fun (Cell(a, b)) -> (a, b)) cells
     |> Array.unzip
     ||> interpret iterations 0
  let printCells outFile:(string*Interpreted)[] -> unit =
    Array.fold (fun prevNumber (s, e) ->
      if number s = prevNumber
       then logPrintf outFile "(%s: %A) " s e
       else logPrintf outFile "\n(%s: %A) " s e
      number s
     ) 1
      >> ignore
  let parseInstructionList lines =
    let parseInstruction (s:string) =
      match s.Split ' ' with
      |[|a|] -> (a, "")
      |[|a; b|] -> (a, b)
      |_ -> failwith "unrecognized instruction"
    let input = Array.filter ((<>) "") lines
    Array.map parseInstruction input
  let getInstruction = fun i -> function
    |"push", x -> Push x | "pop", _ -> Pop
    |"store", x -> Store x | "load", x -> Load x
    |"goto", x -> GotoFwdShift (int x - i) |"gotoiftrue", x -> GotoIfTrueFwdShift (int x - i)
    |"call", _ -> Call | "return", _ -> Return
    |"getheap", _ -> GetHeap | "newheap", _ -> NewHeap | "writeheap", _ -> WriteHeap
    |"inputline", _ -> Input "%i" | "output", _ -> Output "%i"
    |name, "" when
      List.exists (function Combinator_2(_, _) as c -> c.CommandInfo.Name = name | _ -> false) PseudoASM.Definition.allCombinators ->
      List.find (function Combinator_2(_, _) as c -> c.CommandInfo.Name = name | _ -> false) PseudoASM.Definition.allCombinators
    |unknown -> failwithf "unknown: %A" unknown
    
  let testExcelInterpreter a =
    test a "test cases - Excel pseudo-asm" (fun file outFile ->
      Console.WindowWidth <- 170
      let instructions =
        let input = File.ReadAllLines file
        Array.append input [|sprintf "goto %i" (Array.filter ((<>) "") input).Length|]
         |> parseInstructionList
         |> Array.mapi getInstruction
      ExcelLanguage.Implementation.fromPseudoASMSeq (instructions, Map.empty) |> fst |> Array.ofSeq // integrate later
       |> interpret 80
       |> printCells outFile
     )

  let openAndPartiallyParse =
    Project.Input.Implementation.fromTestFile >> Parser.Implementation.fromStringRunUntilParsed
  let openAndParse =
    Project.Input.Implementation.fromTestFile >> Parser.Implementation.fromString

  let testParser a =
    test a "test cases - parser" (fun file outFile ->
      let parsed = (openAndPartiallyParse file).Clean()
      printfn "%A" (File.ReadAllText file)
      printfn "%A" parsed
      parsed.ToStringExpr()
       |> logPrintf outFile "%O\n"
     )

  let testTypeSystem =
    test ignore "test cases - parser" (fun file outFile ->
      let parsed = fst <| openAndParse file
      printfn "%O" (parsed.ToStringExpr())
     )

  let testCompilerAST a =
    test a "test cases - compiler AST" (fun file outFile ->
      let parsed = openAndParse file
      printfn "%A" (fst(parsed).ToStringExpr())
      let cmds =
        parsed
         |> AST.Implementation.fromToken |> fun e -> fst e |> printfn "%O"; e
         |> PseudoASM.Implementation.fromAST
         |> fst |> Array.ofSeq
      let stack, heap, output = interpretPAsm DEBUG_INTERPRETER cmds
      logPrintf outFile "stack %A\n" stack
      logPrintf outFile "\noutput %A\n" output
      printfn "heap: %A" heap
     )
   
  let testPAsm a =
    test a "test cases - Excel pseudo-asm" (fun file outFile ->
      File.ReadAllLines file
       |> parseInstructionList
       |> Array.mapi getInstruction
       |> Array.map (fun e -> printfn "%A" e; e)
       |> interpretPAsm false
       |> logPrintf outFile "%A\n"
     )
     
  let testExcelCompiler =
    test ignore "test cases - compiler AST" (fun file _ ->
      let parsed = openAndParse file
      let cmds =
        parsed
         |> AST.Implementation.fromToken
         |> PseudoASM.Implementation.fromAST
         |> fst |> Array.ofSeq
      Output.Implementation.writeExcelFile (file + ".xlsx") cmds
     )

//  let testExcelFile =
//    test ignore "test cases - Excel pseudo-asm" (fun file _ ->
//      File.ReadAllLines file
//       |> parseInstructionList
//       |> Array.mapi getInstruction
//       |> writeExcelFile (file + ".xlsx")
//     )
//
//  let testAsmCompilerSimple =
//    test ignore "test cases - Excel pseudo-asm" (fun file _ ->
//      File.ReadAllLines file
//       |> parseInstructionList
//       |> Array.mapi getInstruction
//       |> writeBytecode (file + ".s")
//     )
//
//  let testAsmCompiler =
//    test ignore "test cases - compiler AST" (fun file _ ->
//      let parsed = openAndParse file
//      let cmds =
//        parsed
//         |> AST.Implementation.fromToken
//         |> PseudoASM.Implementation.fromAST
//         |> fst |> Array.ofSeq
//      writeBytecode (file + ".s") cmds
//     )