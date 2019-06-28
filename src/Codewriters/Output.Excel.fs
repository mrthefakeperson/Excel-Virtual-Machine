module private Output.Excel
open ExcelLanguage.Definition
open System.IO
open System.Reflection
open Microsoft.Office.Core
open Microsoft.Office.Interop.Excel

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
  let cells = ExcelLanguage.Implementation.fromPseudoASMSeq (cmds, Map.empty)       // integrate this later
  let colA, colB, colC, colD, row1, row3, row4 =
    ref [], ref [], ref [], ref [], ref [], ref [], ref []
  Seq.iter (fun (Cell(xy, _) as cell) ->
    let a, ord =
      match ExcelLanguage.Definition.coordinates xy with
      |o, 1 -> colA, o 
      |o, 2 -> colB, o
      |o, 3 -> colC, o
      |o, 4 -> colD, o
      |1, o -> row1, o
      |3, o -> row3, o
      |4, o -> row4, o
      |a -> failwithf "found cell %A" a
    a := (ord, cell.ToString()):: !a
   ) (fst cells)
  let setCol c data =
    let start = Seq.minBy fst data |> fst
    let finish = Seq.maxBy fst data |> fst
    let write = Array2D.create (finish - start + 1) 1 ("" :> obj)
    List.iter (fun (r, e) -> write.[r - start, 0] <- e :> obj) data
    sheet.Range(coordsToS (start, c), coordsToS (finish, c)).Formula <- write
  let setRow r data =
    let start = Seq.minBy fst data |> fst
    let finish = Seq.maxBy fst data |> fst
    let write = Array2D.create 1 (finish - start + 1) ("" :> obj)
    List.iter (fun (c, e) -> write.[0, c - start] <- e :> obj) data
    sheet.Range(coordsToS (r, start), coordsToS (r, finish)).Formula <- write
  setCol 1 !colA
  setCol 2 !colB
  setCol 3 !colC
  setCol 4 !colD
  setRow 1 !row1
  setRow 3 !row3
  setRow 4 !row4

//  let set cellname txt =
//    sheet.Range(cellname).Value(Missing.Value) <- txt
//  let cells = ExcelLanguage.Implementation.fromPseudoASMSeq (cmds, Map.empty)       // integrate this later
//  Seq.iter (fun (Cell(xy, _) as cell) ->
//    set xy (cell.ToString())
//   ) (fst cells)
  try sheet._SaveAs (Directory.GetCurrentDirectory() + @"\" + fileName)
  with _ -> sheet.SaveAs fileName
  back.Quit()
  printfn "done"
