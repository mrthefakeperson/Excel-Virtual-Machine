#r "../docs/fable-core/Fable.Core.dll"
open Fable.Core
open Fable.Import
open Fable.Import.Browser

#load "CompileAndRun.fsx"

let getById<'a when 'a :> HTMLElement> s = document.getElementById s :?> 'a

// make a function which will extract tokens from the stdinData element in order
let makeTokenGenerator() =
  let stdinData = getById<Browser.HTMLTextAreaElement>("stdinData").value
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

let consoleElement = getById<Browser.HTMLDivElement>("consoleContainer")
let printToConsole s = 
  consoleElement.textContent <- consoleElement.textContent + s

getById<Browser.HTMLButtonElement>("compileAndRuttonButton").onclick <- fun _ ->
  consoleElement.textContent <- ""
  getById<Browser.HTMLTextAreaElement>("codeContainer").value  // get input code
   |> CompileAndRun.compileAndRun (makeTokenGenerator()) printToConsole  // compile and run
  System.Object()