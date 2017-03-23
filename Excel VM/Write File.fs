﻿module Write_File
open Microsoft.Office.Core
open Microsoft.Office.Interop.Excel
open System
open System.Reflection
open System.IO
open Excel_Language
open Compiler_Definitions
open Compiler
open Excel_Conversion

let writeExcelFile fileName cmds =
  if File.Exists fileName then File.Delete fileName     //consider warning the user before doing this
  // set up sheet
  let back = ApplicationClass()
  let sheet = back.Workbooks.Add().Worksheets.[1] :?> _Worksheet
  back.Calculation <- XlCalculation.xlCalculationManual
  back.CalculateBeforeSave <- false
  back.Iteration <- true
  back.MaxIterations <- 500
  back.MaxChange <- 0.
  sheet.Range(``allOutput*``).WrapText <- true
  sheet.Range(``allOutput*``).RowHeight <- 200
  sheet.Range(``allOutput*``).VerticalAlignment <- XlVAlign.xlVAlignTop
  // write finished program and save
  let set cellname txt =
    sheet.Range(cellname).Value(Missing.Value) <- txt
  let cells = makeProgram cmds
  Array.iter (fun (Cell(xy, _) as cell) ->
    set xy (cell.ToString())
   ) cells
  try sheet._SaveAs (Directory.GetCurrentDirectory() + @"\" + fileName)
  with _ -> sheet.SaveAs fileName
  back.Quit()
  printfn "done"

//compile to asm
let writeBytecode fileName cmds =
  let cmds = Array.append cmds [|Return|]
  //map variable names to byte values
  let valueOf =
    Array.choose (function Store s -> Some s | _ -> None) cmds
     |> Set.ofArray |> Set.toList
     |> List.mapi (fun i e -> (e, i + 5))
     |> dict
  //change literal to integer value
  let (|IntValue|_|) (s:string) =
    match s.ToUpper() with
    |"TRUE" -> Some 1
    |"FALSE" -> Some 0
    |_ when Int32.TryParse(s, ref 0) -> Some (int s)
    |"()" -> Some 0
    |"ENDARR" -> Some -7777777       //glitches occur around this value
                                     //todo: get rid of endArr somehow
    |_ -> None
  //get string literals, concatenate with '\0'
  //  - all initial strings already exist in the code, but new strings (eg. formed with concatenation) are created on the heap
  //  - printing from heap is currently a work in progress
  let stringDataBytes, getStringAddress =
    let stringData, stringAddressPairs =
      Array.choose (function Push (IntValue _) -> None | Push s -> Some s | _ -> None) cmds
       |> Array.fold (fun (allStrings, allPairs) e ->
            allStrings + e + "\000", (e, allStrings.Length)::allPairs
           ) ("", [])
    String.collect (fun e -> sprintf "\t.byte %i\n" (byte e)) stringData,
     dict stringAddressPairs
  //convert literal to int/ptr
  let convertLiteral = function
    |IntValue x -> x
    |s -> getStringAddress.[s]
  
  //write all commands (todo: use larger ints than bytes)
  let inputFormatList = ["%i"; "%s"; " "]
  File.WriteAllText (fileName,
    Array.mapi (fun i -> function
      |Push x -> 0, convertLiteral x
      |PushFwdShift n -> 0, i + n
      |Pop -> 1, 0
      |Store x -> 2, valueOf.[x]
      |Load x -> 3, valueOf.[x]
      |Popv x -> 4, valueOf.[x]
      |GotoFwdShift n -> 5, i + n
      |GotoIfTrueFwdShift n -> 6, i + n
      |Call -> 7, 0
      |Return -> 8, 0
      |NewHeap -> 9, 0
      |GetHeap -> 10, 0
      |WriteHeap -> 11, 0
      |Input t -> 12 + List.findIndex ((=) t) inputFormatList, 0
      |OutputLine t -> 14 + List.findIndex ((=) t) allTypes, 0
      |Combinator_2 c -> 16 + List.findIndex ((=) (Combinator_2 c)) allCombinators, 0
     ) cmds
     |> Array.map (fun (a, b) -> sprintf "\t.long %i\n\t.long %i" a b)
     |> String.concat "\n"
     |> fun e ->
          Definitions.TEMPLATE
           .Replace("\t.long k", e)
           .Replace("\t.byte k", stringDataBytes)
   )