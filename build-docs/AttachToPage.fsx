#r "../docs/fable-core/Fable.Core.dll"
open Fable.Core
open Fable.Import
open Fable.Import.Browser

#load "CompileAndRun.fsx"

let getById<'a when 'a :> HTMLElement> s = document.getElementById s :?> 'a
let stdinElement = getById<Browser.HTMLTextAreaElement>("stdinData")
let consoleElement = getById<Browser.HTMLTextAreaElement>("consoleContainer")
let compileConsole = getById<Browser.HTMLButtonElement>("compileAndRunButton")
let compileSpreadsheet = getById<Browser.HTMLButtonElement>("compileToSpreadsheetButton")
let spreadsheetCell = getById<Browser.HTMLTableDataCellElement>
let samplesPanel = getById<Browser.HTMLDivElement>("samples")
let spreadsheetPanel = getById<Browser.HTMLDivElement>("ExcelSheet")

// make a function which will extract tokens from the stdinData element in order
let makeTokenGenerator() =
  let stdinData = stdinElement.value
  let tokenStream = stdinData.Split ' ' |> List.ofArray |> ref
  let popStream() =
    try List.head !tokenStream
    finally tokenStream := List.tail !tokenStream
  function
    |"%i" ->
      tokenStream := Seq.skipWhile (System.Int32.TryParse >> fst >> not) !tokenStream |> List.ofSeq
      popStream()
    |"%s" -> popStream()
    |_ -> failwith "unrecognized or unsupported input format string"

let printToConsole s = 
  consoleElement.value <- consoleElement.value + s
compileConsole.onclick <- fun _ ->
  consoleElement.value <- "> Excel_VM code.c\n> code\n"
  getById<Browser.HTMLTextAreaElement>("codeContainer").value  // get input code
   |> CompileAndRun.compileAndRun (fun _ _ _ -> ()) (makeTokenGenerator()) printToConsole  // compile and run
  System.Object()
  
let printToSheet s =
  spreadsheetCell("B2").textContent <- spreadsheetCell("B2").textContent + s
let updateSheet (stacks:System.Collections.Generic.IDictionary<string, string list ref>) (heap:System.Collections.Generic.List<string>) (cmd:PseudoASM.Definition.PseudoASM) =
  // stdin
  spreadsheetCell("A2").textContent <- stdinElement.value
  // instruction pointer
  let instr = List.head !stacks.["A"]
  spreadsheetCell("A3").textContent <- string ((int instr - 1) * 2)
  spreadsheetCell("A4").textContent <- string (int instr * 2)
  let cmdname, arg = cmd.CommandInfo.StringPair
  spreadsheetCell("A5").textContent <- cmdname
  spreadsheetCell("A6").textContent <- arg
  // stack
  spreadsheetCell("C3").textContent <- match !stacks.["B"] with x::_ -> x | [] -> ""
  spreadsheetCell("C4").textContent <- string (List.length !stacks.["B"] + 1)
  List.iteri (fun i e ->
    if 5 + i < 20 then
      spreadsheetCell("C" + string (5 + i)).textContent <- e
   ) !stacks.["B"]
  // heap
  spreadsheetCell("B3").textContent <- if heap.Count = 0 then "" else heap.[heap.Count - 1]
  spreadsheetCell("B4").textContent <- string (heap.Count + 1)
  Seq.iteri (fun i e ->
    if 5 + i < 20 then
      spreadsheetCell("B" + string (5 + i)).textContent <- e
   ) heap
  // delay
  let rec delay = function 0 -> () | n -> delay (n - 1)
  delay 0
compileSpreadsheet.onclick <- fun _ ->
  // disable samples panel, display spreadsheet panel
  samplesPanel.style.display <- "none"
  spreadsheetPanel.style.display <- "block"
  // wipe spreadsheet
  for e in 1..19 do
    spreadsheetCell("A" + string e).textContent <- ""
    spreadsheetCell("B" + string e).textContent <- ""
    spreadsheetCell("C" + string e).textContent <- ""
  spreadsheetCell("B2").textContent <- "stdout:\n"
  consoleElement.value <- "> Excel_VM code.c -outputExcelFile\n"
  getById<Browser.HTMLTextAreaElement>("codeContainer").value
   |> CompileAndRun.compileAndRun updateSheet (makeTokenGenerator()) printToSheet
  System.Object()