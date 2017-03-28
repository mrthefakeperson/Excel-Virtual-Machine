namespace Write_File
open Excel_Language
open Excel_Conversion
open System.IO
open System.Reflection
open Microsoft.Office.Core
open Microsoft.Office.Interop.Excel

module Excel =
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
