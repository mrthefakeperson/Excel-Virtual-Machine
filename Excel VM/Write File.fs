module Write_File
open Microsoft.Office.Core
open Microsoft.Office.Interop.Excel
open System
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

//compile to asm
let writeBytecode fileName cmds =
  let cmds = Array.append cmds [|Return|]
  //map variable names to byte values
  let valueOf =
    Array.choose (function Store s -> Some s | _ -> None) cmds
     |> Set.ofArray |> Set.toList
     |> List.mapi (fun i e -> (e, i + 5))
     |> dict
  //change all literals to integer values
  let convertLiteral (s:string) =
    match s.ToUpper() with
    |"TRUE" -> 1
    |"FALSE" -> 0
    |_ when Int32.TryParse(s, ref 0) -> int s
    |"()" -> 0
    |"ENDARR" -> 0
    |_ -> failwithf "could not convert literal value: %A" s
  
  //write all commands (todo: use larger ints than bytes)
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
      |InputLine -> 12, 0
      |OutputLine -> 13, 0
      |Combinator_2 c ->
        match List.tryFindIndex ((=) (Combinator_2 c)) allCombinators with
        |Some i -> 14 + i, 0
        |None -> failwith "unrecognized combinator"
     ) cmds
     |> Array.map (fun (a, b) -> sprintf "\t.long %i\n\t.long %i" a b)
     |> String.concat "\n"
     |> fun e -> File.ReadAllText("template.txt").Replace("\t.long 7777777777777777777777", e)
//     |> Array.map (fun (a, b) -> sprintf "%i, %i" a b)
//     |> String.concat ", "
//     |> sprintf "{%s}"
   )