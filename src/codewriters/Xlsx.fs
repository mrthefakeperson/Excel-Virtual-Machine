module Codewriters.Xlsx
open System.IO
open Microsoft.Office.Core
open Microsoft.Office.Interop.Excel
open CompilerDatatypes.PseudoASM
open CompilerDatatypes.PseudoASM.Simple

type BackgroundSettings = Microsoft.Office.Interop.Excel.ApplicationClass
type Spreadsheet = Microsoft.Office.Interop.Excel._Worksheet

let setup_spreadsheet() : Spreadsheet * BackgroundSettings =
  let workbook = ApplicationClass()
  let spreadsheet = workbook.Workbooks.Add().Worksheets.[1] :?> Spreadsheet
  workbook.Calculation <- XlCalculation.xlCalculationManual
  workbook.CalculateBeforeSave <- false
  workbook.Iteration <- true
  workbook.MaxIterations <- 500
  workbook.MaxChange <- 0.
  (spreadsheet, workbook)

let write_cells_topleft (cells: 'a[,]) (sheet: Spreadsheet, b: BackgroundSettings) =
  let cells = Array2D.map (fun e -> e :> obj) cells
  let num_rows = cells.GetLength 0
  let num_cols = cells.GetLength 1
  let get_column_name = function
    |col when col < 26 -> string ('A' + char col)
    |col when col < 26 + 26 * 26 ->
      let shifted = col - 26
      let radix1 = shifted / 26
      let radix0 = shifted % 26
      string ('A' + char radix1) + string ('A' + char radix0)
    |_ -> failwith "Excel column exceeded maximum of 702"
  let topleft = "A1"
  let bottomright = get_column_name num_cols + string num_rows
  sheet.Range(topleft, bottomright).Formula <- cells
  (sheet, b)

let save_spreadsheet (filename: string) (sheet: Spreadsheet, b: BackgroundSettings) =
  let dir = Directory.GetCurrentDirectory()
  let full_path = Path.Combine(dir, filename)
  try sheet._SaveAs full_path
  with _ -> sheet.SaveAs filename
  b.Quit()


type CellInstruction = {
  modify_reg: Register option
  name: string
  read_values: RMC list
 }

let to_cell_instrs: Asm seq -> CellInstruction seq =
  let incr reg = {modify_reg = Some reg; name = "+"; read_values = [RMC.R reg]}
  let store ``*reg`` x = {modify_reg = None; name = "store"; read_values = [RMC.I ``*reg``; x]}
  Seq.collect (fun e -> seq {
    match e with
    |Data _ | Label _ -> ()
    |Push rc ->
      let push_value = match rc with RC.R r -> RMC.R r | RC.C c -> RMC.C c
      yield! [incr SP; store SP push_value]
    |_ -> failwith "not implemented yet"
   })

let write_cell_array (instrs: Asm seq) (cells: string[,]) =
  let instrs = to_cell_instrs instrs
  
  // PC is in column A1
  let pc = "=IF(D2=\"branch\", D2, A1 + 1)"  // something like this

  // current stack value, stack in column B
  let STACK_SIZE = 100
  let topstack = sprintf "=INDEX(B2:B%i, F1)" (STACK_SIZE + 1)
  let stack =
    Array.init STACK_SIZE (fun row ->
      let self = sprintf "B%i" (row + 3)
      sprintf "=IF(F1=%i && D2=\"push\", D2, %s)" row self
     )

  // memory ptr, memory in column C
  let MEM_SIZE = 100
  let memptr = "=IF(D2=\"alloc\", C2 + D2, C1)"
  let memory =
    Array.init MEM_SIZE (fun row ->
      let self = sprintf "C%i" (row + 3)
      sprintf "=IF(D2=\"store\", INDEX(E2:ZZ2, D2), %s)" self
     )

  // registers are each assigned a column of instructions, preserving data dependencies
  // registers starting from column E (col D for instructions which don't modify registers)
  let (reg_to_col, last_col) =
    let fixed_columns =
      [(None, 0); (Some BP, 1); (Some SP, 2); (Some (Register.R 0), 3); (Some RX, 4)]
    Seq.map (fun instr -> instr.modify_reg) instrs
     |> Seq.fold (fun (acc_map, col) reg ->
          if Map.containsKey reg acc_map
           then (acc_map, col)
           else (Map.add reg col acc_map, col + 1)
         ) (Map fixed_columns, List.length fixed_columns)
  let instr_rows = Seq.map Seq.singleton instrs  // TODO: better grouping algorithm
  let register_columns: string[,] =
    let rows = Seq.length instr_rows
    let cols = last_col
    let yld = Array2D.create rows cols ""
    Map.iter (fun _ col ->
      // all registers are in the first row
      yld.[0, col] <- "=register formula based on current instruction"  // TODO
      let fetch_current_instr = sprintf "=INDEX(%s%i:%s%i, A1)" (string col) 3 (string col) (cols + 2)  // TODO: make the formula actually work
      yld.[1, col] <- fetch_current_instr
     ) reg_to_col
    Seq.iteri (fun row ->
      Seq.iter (fun instr ->
        let operands = List.map string instr.read_values
        let cell_instr = String.concat " " (instr.name::operands)
        yld.[row + 2, reg_to_col.[instr.modify_reg]] <- cell_instr
       )
     ) instr_rows
    yld

  ()