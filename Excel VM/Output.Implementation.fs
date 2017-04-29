module Output.Implementation
open Project.Util
open PseudoASM.Definition

let fromPseudoASM (cmds:PseudoASM seq, args:CommandLineArguments): unit =
  let inputFileNoExtension = fst (splitFileExtension args.["input_file"])
  if args.ContainsKey "outputExcelFile" then
    let outputFile = if args.ContainsKey "o" then args.["o"] else inputFileNoExtension + ".xlsx"
    Output.Excel.writeExcelFile outputFile cmds   // uses ExcelLanguage.Implementation
  else
    let outputFile = if args.ContainsKey "o" then args.["o"] else inputFileNoExtension + ".s"
    Output.ASM.writeASM outputFile cmds

let debugPseudoASM = Output.ASM.debugASM