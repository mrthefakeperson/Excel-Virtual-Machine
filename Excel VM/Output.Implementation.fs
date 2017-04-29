module Output.Implementation
open Project.Util
open PseudoASM.Definition

let fromPseudoASM (cmds:PseudoASM seq, args:CommandLineArguments): unit =
  if args.ContainsKey "outputExcelFile" then
    let outputFile = if args.ContainsKey "o" then args.["o"] else args.["input_file"] + ".xlsx"
    Output.Excel.writeExcelFile outputFile cmds   // uses ExcelLanguage.Implementation
  else
    let outputFile = if args.ContainsKey "o" then args.["o"] else args.["input_file"] + ".s"
    Output.ASM.writeASM outputFile cmds

let debugPseudoASM = Output.ASM.debugASM