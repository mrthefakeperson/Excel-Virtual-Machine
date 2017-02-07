module Write_File
open Microsoft.Office.Core
open Microsoft.Office.Interop.Excel
open System.Reflection
open System.IO
open Excel_Language
open Compiler
open Excel_Conversion

let writeExcelFile fileName cmds =
  if File.Exists fileName then File.Delete fileName
  let back = ApplicationClass()
  let sheet = back.Workbooks.Add().Worksheets.[1] :?> _Worksheet
  back.Calculation <- XlCalculation.xlCalculationManual
  back.CalculateBeforeSave <- false
  back.Iteration <- true
  back.MaxIterations <- 100
  back.MaxChange <- 0.
  back.Visible <- true
  let set cellname txt =
    printfn "%A" (cellname, txt)
    sheet.Range(cellname).Value(Missing.Value) <- txt
  makeProgram cmds
   |> Seq.iter (fun (Cell(s, _) as f) -> set s (f.ToString()))
  sheet.SaveAs fileName
  back.Quit()