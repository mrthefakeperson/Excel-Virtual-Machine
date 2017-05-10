module Output.Implementation
open Project.Util
open PseudoASM.Definition
open System.Diagnostics

let fromPseudoASM (cmds:PseudoASM seq, args:CommandLineArguments): unit =
  let inputFileNoExtension = fst (splitFileExtension args.["input_file"])
  if args.ContainsKey "outputExcelFile" then
    let outputFile = if args.ContainsKey "o" then args.["o"] else inputFileNoExtension + ".xlsx"
    Output.Excel.writeExcelFile outputFile cmds   // uses ExcelLanguage.Implementation
  else
    let outputFileS, outputFileExe =
      if args.ContainsKey "o"
       then args.["o"] + ".s", args.["o"]
       else inputFileNoExtension + ".s", inputFileNoExtension + ".exe"
    Output.ASM.writeASM outputFileS cmds
    try ignore <| Process.Start("gcc", sprintf "%s -o %s" outputFileS outputFileExe)
    with ex -> failwith "please make sure GCC is configured"

let debugPseudoASM = Output.ASM.debugASM

let writeExcelFile = Output.Excel.writeExcelFile