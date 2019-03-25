module Codewriters.Xlsx
//open Codegen.PAsm
//open Codegen.PAsm.Flat

//let write_xlsx_instrs (instrs: Asm list list) (cells: string[,]) =  // cells.[row, col]
//  // registers are assigned a column of instructions
//  let COL_MEM = 15
//  let col_of = function
//    |BP -> 16
//    |SP -> 17
//    |PSR_EQ -> 18
//    |PSR_LT -> 19
//    |PSR_GT -> 20
//    |RX -> 21
//    |R n -> 22 + n
//  let labels =
//    List.mapi (fun row ->
//      List.choose (function Label name -> Some(name, row) | _ -> None)
//     ) instrs
//     |> List.concat
//     |> dict
//  List.iteri (fun row ->
//    List.iter (function
//      |Data _ | Label _ -> ()
//      |Push r ->
//        cells.[row, col_of SP] <- "Add 1"
//        cells.[row, COL_MEM] <- sprintf "MovMR SP (%A)" r
//      |_ -> ()
//      //|PushC of 'a
//      //|PushRealRs
//      //|Pop of Register
//      //|PopRealRs
//      //|ShiftStackDown of offset: int * length: int  // tail recursion shortcut: shift {length} items below SP starting at {offset} spaces above SP
//      //|MovRR of Register * Register
//      //|MovRM of Register * Memory
//      //|MovMR of Memory * Register
//      //|MovRC of Register * 'a
//      //|MovRHandle of Register * Handle  // move the address value directly into a register; useful when address is not available at time of codegen
//      //|Cmp of Register * Register  // sets comparison flags (a - b, (> 0)? (= 0)? (< 0)?)
//      //|CmpC of Register * 'a
//      //|Br of label: string  // unconditional branch
//      //|Br0 of label: string  // branch if EQ flag is 0
//      //|BrT of label: string  // branch if EQ flag is not 0
//      //|BrLT of label: string
//      //|BrGT of label: string
//      //|Call of label: string
//      //|Ret
//      //// operations
//      //|Alloc of bytes: int  // alloc an address into R0
//      //|Add of Register * Register
//      //|AddC of Register * 'a
//      //|Sub of Register * Register
//      //|SubC of Register * 'a
//      //|Mul of Register * Register
//      //|MulC of Register * 'a
//      //|DivMod of Register * Register
//      //|DivModC of Register * 'a
//     )
//   ) instrs

//let write_xlsx (instrs: Asm list) : string =
//  ""