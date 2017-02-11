//testing file
open Parser
open Lexer
open Compiler
open Excel_Language
open Excel_Conversion
open Write_File
open System
open System.Diagnostics
open System.IO

let rec test folderName runFile n =
  let name = sprintf @"%s\%s\in%i.txt" __SOURCE_DIRECTORY__ folderName n
  if File.Exists name then
    printfn "%s:" name
    runFile name
    ignore (stdin.ReadLine())
    test folderName runFile (n + 1)
    
let debug e = printfn "%O" e; e
let debugList e = printfn "[%s]" (String.concat "; " (List.map (sprintf "%O") e)); e
let interpret iterations cells =
  Array.map (fun (Cell(a, b)) -> (a, b)) cells
   |> Array.unzip
   ||> interpret iterations 0
let printCells:(string*Interpreted)[] -> unit =
  Array.fold (fun prevNumber (s, e) ->
    if number s = prevNumber
     then printf "(%s: %A) " s e
     else printf "\n(%s: %A) " s e
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
  
let testExcelInterpreter =
  test "test cases - Excel pseudo-asm" (fun file ->
    Console.WindowWidth <- 170
    let instructions =
      let input = File.ReadAllLines file
      Array.append input [|sprintf "goto %i" (Array.filter ((<>) "") input).Length|]
       |> parseInstructionList
       |> Array.collect (fun (a, b) -> [|a; b|])
       |> Array.map Literal
       |> Array.mapi (fun i e -> Cell(numberToAlpha (i+2) + "1", e))
    packageProgram instructions {alphaToNumber "F"..alphaToNumber "M"}
     |> interpret 100
     |> printCells
   )

let testParser =
  test "test cases - parser" (fun file ->
    let parsed =
      File.ReadAllText file
       |> groupByRuleset
       |> preprocess
       |> parse (function [] -> true | _ -> false) (fun _ -> false) []
       |> fst
    printfn "%A" parsed
    parsed.Clean()
     .ToStringExpr()
     |> printfn "%s\n%O" (File.ReadAllText file)
   )

let testCompilerAST =
  test "test cases - compiler AST" (fun file ->
    Console.WindowWidth <- 170
    let parsed =
      File.ReadAllText file
       |> groupByRuleset
       |> preprocess
       |> parse (function [] -> true | _ -> false) (fun _ -> false) []
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
    makeProgram cmds
     |> interpret 800
     |> printCells
   )
   
let testPAsm =
  test "test cases - Excel pseudo-asm" (fun file ->
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
          |"add", _ -> Add | "equals", _ -> Equals
          |unknown -> failwithf "unknown: %A" unknown
         )
     |> Array.map (fun e -> printfn "%A" e; e)
     |> interpretPAsm
     |> printfn "%A"
   )

let testExcelFile =
  test "test cases - Excel pseudo-asm" (fun file ->
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
          |"add", _ -> Add | "equals", _ -> Equals
          |unknown -> failwithf "unknown: %A" unknown
         )
     |> writeExcelFile (file + ".xlsx")
   )

let testExcelCompiler =
  test "test cases - compiler AST" (fun file ->
    Console.WindowWidth <- 170
    let parsed =
      File.ReadAllText file
       |> groupByRuleset
       |> preprocess
       |> parse (function [] -> true | _ -> false) (fun _ -> false) []
       |> fst
    let cmds =
      parsed.Clean()
       |> ASTCompile |> debug
       |> compile
       |> Array.ofList
    Array.iter (printf "%A   ") cmds
    writeExcelFile (file + ".xlsx") cmds
   )

//testExcelInterpreter 1
//testParser 10
//testCompilerAST 11
//testPAsm 1
//testExcelFile 1
testExcelCompiler 11