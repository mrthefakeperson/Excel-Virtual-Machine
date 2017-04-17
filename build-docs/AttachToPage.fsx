#r "../docs/fable-core/Fable.Core.dll"
open Fable.Core
open Fable.Import
open Fable.Import.Browser

#load "CompileAndRun.fsx"

let getById<'a when 'a :> HTMLElement> s = document.getElementById s :?> 'a

getById<Browser.HTMLButtonElement>("compile").onclick <- fun _ ->
  let inputCode = getById<Browser.HTMLTextAreaElement>("input_code").value;
  let stdInput = getById<Browser.HTMLTextAreaElement>("stdin").value;
  let stdOutput = getById<Browser.HTMLTextAreaElement>("stdout");
  let output = CompileAndRun.compileAndRun inputCode stdInput
  stdOutput.textContent <- output
  System.Object()