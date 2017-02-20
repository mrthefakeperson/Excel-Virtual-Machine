module Testing
//testing file
open Parser
open Lexer
open Compiler_Definitions
open Compiler
open Excel_Language
open Excel_Conversion
open Write_File
open System
open System.Diagnostics
open System.IO

let logPrintf file =
  let pr = new StreamWriter(file:string)
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
    ignore (stdin.ReadLine())
    test action folderName runFile (n + 1)
let verify name =
  if File.ReadAllLines name <> File.ReadAllLines (name + ".ans")
   then printfn "\n\nincorrect file: %s\n" name
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
  
let testExcelInterpreter a =
  test a "test cases - Excel pseudo-asm" (fun file outFile ->
    Console.WindowWidth <- 170
    let instructions =
      let input = File.ReadAllLines file
      Array.append input [|sprintf "goto %i" (Array.filter ((<>) "") input).Length|]
       |> parseInstructionList
       |> Array.collect (fun (a, b) -> [|a; b|])
       |> Array.map Literal
       |> Array.mapi (fun i e -> Cell(numberToAlpha (i+2) + "1", e))
    packageProgram instructions {alphaToNumber "F"..alphaToNumber "M"}
     |> interpret 80
     |> printCells outFile
   )

let testParser a =
  test a "test cases - parser" (fun file outFile ->
    let parsed =
      File.ReadAllText file
       |> groupByRuleset
       |> preprocess
       |> parse Normal (function [] -> true | _ -> false) (fun _ -> false) []
       |> fst
    let parsed = parsed.Clean()
    logPrintf outFile "%A\n" parsed
    parsed.ToStringExpr()
     |> logPrintf outFile "%s\n%O\n" (File.ReadAllText file)
   )

let testCompilerAST a =
  test a "test cases - compiler AST" (fun file outFile ->
    Console.WindowWidth <- 170
    let parsed =
      File.ReadAllText file
       |> groupByRuleset
       |> preprocess
       |> parse Normal (function [] -> true | _ -> false) (fun _ -> false) []
       |> fst
    let cmds =
      parsed.Clean()
       |> ASTCompile |> debug
       |> compile
       |> Array.ofList
    Array.iter (printf "%A   ") cmds
    let stack, heap = interpretPAsm cmds
    printfn "stack %A" stack
    printfn "heap [%s]" (String.concat "; " (Seq.map (sprintf "%A") heap))
    //makeProgram cmds
    // |> interpret 800
    // |> printCells outFile
   )
   
let testPAsm a =
  test a "test cases - Excel pseudo-asm" (fun file outFile ->
    File.ReadAllLines file
     |> parseInstructionList
     |> Array.mapi (fun i ->
          function
          |"push", x -> Push x | "pop", _ -> Pop
          |"store", x -> Store x | "load", x -> Load x | "popv", x -> Popv x
          |"goto", x -> GotoFwdShift (int x - i) |"gotoiftrue", x -> GotoIfTrueFwdShift (int x - i)
          |"call", _ -> Call | "return", _ -> Return
          |"getheap", _ -> GetHeap | "newheap", _ -> NewHeap | "writeheap", _ -> WriteHeap
          |"inputline", _ -> InputLine | "outputline", _ -> OutputLine
//          |"add", _ -> Add | "equals", _ -> Equals
          |name, "" when
            List.exists (function Combinator_2 c -> c.Name = name | _ -> false) allCombinators ->
            List.find (function Combinator_2 c -> c.Name = name | _ -> false) allCombinators
          |unknown -> failwithf "unknown: %A" unknown
         )
     |> Array.map (fun e -> printfn "%A" e; e)
     |> interpretPAsm
     |> logPrintf outFile "%A\n"
   )

let testExcelFile =
  test ignore "test cases - Excel pseudo-asm" (fun file _ ->
    File.ReadAllLines file
     |> parseInstructionList
     |> Array.mapi (fun i ->
          function
          |"push", x -> Push x | "pop", _ -> Pop
          |"store", x -> Store x | "load", x -> Load x | "popv", x -> Popv x
          |"goto", x -> GotoFwdShift (int x - i) |"gotoiftrue", x -> GotoIfTrueFwdShift (int x - i)
          |"call", _ -> Call | "return", _ -> Return
          |"getheap", _ -> GetHeap | "newheap", _ -> NewHeap | "writeheap", _ -> WriteHeap
          |"inputline", _ -> InputLine | "outputline", _ -> OutputLine
          |name, "" when
            List.exists (function Combinator_2 c -> c.Name = name | _ -> false) allCombinators ->
            List.find (function Combinator_2 c -> c.Name = name | _ -> false) allCombinators
          |unknown -> failwithf "unknown: %A" unknown
         )
     |> writeExcelFile (file + ".xlsx")
   )

let testExcelCompiler =
  test ignore "test cases - compiler AST" (fun file _ ->
    Console.WindowWidth <- 170
    let parsed =
      File.ReadAllText file
       |> groupByRuleset
       |> preprocess
       |> parse Normal (function [] -> true | _ -> false) (fun _ -> false) []
       |> fst
    let cmds =
      parsed.Clean()
       |> ASTCompile |> debug
       |> compile
       |> Array.ofList
    Array.iter (printf "%A   ") cmds
    writeExcelFile (file + ".xlsx") cmds
   )

let runSpecificTest() =       // `generate` to create outputs, `verify` to test, `ignore` to print
  //testExcelInterpreter verify 1
  //testParser generate 12
  //testCompilerAST ignore 13
  //testPAsm ignore 8
  //testExcelFile 1
  testExcelCompiler 13