module Write_File
open Microsoft.Office.Core
open Microsoft.Office.Interop.Excel
open System.Reflection
open System.IO
open Excel_Language
open Compiler_Definitions
open Excel_Conversion

//the file containing all cells except the instructions
let defaultFileName = sprintf "%s\default_file.xlsx" __SOURCE_DIRECTORY__
let writeDefaultFile() =
  let back = ApplicationClass()
  let sheet = back.Workbooks.Add().Worksheets.[1] :?> _Worksheet
  back.Calculation <- XlCalculation.xlCalculationManual
  back.CalculateBeforeSave <- false
  back.Iteration <- true
  back.MaxIterations <- 500
  back.MaxChange <- 0.
  let set cellname txt =
    //printfn "%A" (cellname, txt)
    sheet.Range(cellname).Value(Missing.Value) <- txt
  makeProgram (Array.init 25 (fun e -> Store (string e)))         // 25 variables
   |> Seq.iter (fun (Cell(s, _) as f) -> set s (f.ToString()))
  sheet.SaveAs defaultFileName
  back.Quit()
  printfn "done"

let writeExcelFile fileName cmds =
  if File.Exists fileName then File.Delete fileName
  if not (File.Exists defaultFileName) then writeDefaultFile()
  let back = ApplicationClass()
  let sheet = back.Workbooks.Open(defaultFileName).Worksheets.[1] :?> _Worksheet
  let set cellname txt =
    //printfn "%A" (cellname, txt)
    sheet.Range(cellname).Value(Missing.Value) <- txt
  let cells =
    Array.filter (fun (Cell(c, _)) ->
      fst (coordinates c) = 1
     ) (makeProgram cmds)
  Seq.iter (fun (Cell(s, _) as f) -> set s (f.ToString())) cells
  sheet.SaveAs fileName
  back.Quit()
  printfn "done"