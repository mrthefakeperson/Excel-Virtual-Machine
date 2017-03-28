namespace Testing
open Parser
open Lexer
open AST_Compiler
open Excel_Language.Definitions
open ASM_Compiler
open Excel_Language.Define_VM
open Write_File.ASM
open Write_File.Excel
open Interpreters
open System
open System.Diagnostics
open System.IO

module IntegrationTests =
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
      //ignore (stdin.ReadLine())
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
    |"store", x -> Store x | "load", x -> Load x | "popv", x -> Popv x
    |"goto", x -> GotoFwdShift (int x - i) |"gotoiftrue", x -> GotoIfTrueFwdShift (int x - i)
    |"call", _ -> Call | "return", _ -> Return
    |"getheap", _ -> GetHeap | "newheap", _ -> NewHeap | "writeheap", _ -> WriteHeap
    |"inputline", _ -> Input "%i" | "outputline", _ -> OutputLine type_int32
    |name, "" when
      List.exists (function Combinator_2 c -> c.Name = name | _ -> false) allCombinators ->
      List.find (function Combinator_2 c -> c.Name = name | _ -> false) allCombinators
    |unknown -> failwithf "unknown: %A" unknown
    
  let testExcelInterpreter a =
    test a "test cases - Excel pseudo-asm" (fun file outFile ->
      Console.WindowWidth <- 170
      let instructions =
        let input = File.ReadAllLines file
        Array.append input [|sprintf "goto %i" (Array.filter ((<>) "") input).Length|]
         |> parseInstructionList
         |> Array.mapi getInstruction
      makeProgram instructions
       |> interpret 80
       |> printCells outFile
     )

  let openAndPartiallyParse file =
    let txt = File.ReadAllLines file
    let parseSyntax, txt =
      match txt.[0] with
      |"F#" -> FSharp.parseSyntax, String.concat "\n" txt.[1..]
      |"Py2" -> Python2.parseSyntax, String.concat "\n" txt.[1..]
      |"C" -> C.parseSyntax, String.concat "\n" txt.[1..]
      |_ -> FSharp.parseSyntax, String.concat "\n" txt
    txt
     |> groupByRuleset
     |> parseSyntax
  let openAndParse =
    openAndPartiallyParse
     >> fun e -> e.Clean()
     >> Type_System.applyTypeSystem

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
      let parsed = (openAndParse file).Clean()
      printfn "%O" (parsed.ToStringExpr())
     )

  let testCompilerAST a =
    test a "test cases - compiler AST" (fun file outFile ->
      Console.WindowWidth <- 170
      let parsed = openAndParse file
      printfn "%A" (parsed.Clean().ToStringExpr())
      let cmds =
        parsed.Clean()
         |> ASTCompile |> debug
         |> compile
         |> Array.ofList
      Array.iter (printf "%A   ") cmds
      let stack, heap, output = interpretPAsm cmds
      logPrintf outFile "stack %A\n" stack
      logPrintf outFile "\noutput %A\n" output
    //logPrintf outFile "heap [%s]\n" (String.concat "; " (Seq.map (sprintf "%A") heap))
    //makeProgram cmds
    // |> interpret 800
    // |> printCells outFile
     )
   
  let testPAsm a =
    test a "test cases - Excel pseudo-asm" (fun file outFile ->
      File.ReadAllLines file
       |> parseInstructionList
       |> Array.mapi getInstruction
       |> Array.map (fun e -> printfn "%A" e; e)
       |> interpretPAsm
       |> logPrintf outFile "%A\n"
     )

  let testExcelFile =
    test ignore "test cases - Excel pseudo-asm" (fun file _ ->
      File.ReadAllLines file
       |> parseInstructionList
       |> Array.mapi getInstruction
       |> writeExcelFile (file + ".xlsx")
     )

  let testExcelCompiler =
    test ignore "test cases - compiler AST" (fun file _ ->
      Console.WindowWidth <- 170
      let parsed = openAndParse file
      let cmds =
        parsed.Clean()
         |> ASTCompile |> debug
         |> compile
         |> Array.ofList
      Array.iter (printf "%A   ") cmds
      writeExcelFile (file + ".xlsx") cmds
     )

  let testAsmCompilerSimple =
    test ignore "test cases - Excel pseudo-asm" (fun file _ ->
      File.ReadAllLines file
       |> parseInstructionList
       |> Array.mapi getInstruction
       |> writeBytecode (file + ".s")
     )

  let testAsmCompiler =
    test ignore "test cases - compiler AST" (fun file _ ->
      let parsed = openAndParse file
      let cmds =
        parsed.Clean()
         |> ASTCompile |> debug
         |> compile
         |> Array.ofList
      writeBytecode (file + ".s") cmds
     )

  let runSpecificTest() =       // `generate` to create outputs, `verify` to test, `ignore` to print
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