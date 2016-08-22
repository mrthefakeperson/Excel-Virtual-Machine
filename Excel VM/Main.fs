open System.IO
let debug e=printfn "%A" e; e
let interpret_cells e =
  e
   |> Seq.map (function Excel_Language.Cell(a,b) -> (a,b))
   |> Seq.sortBy (fun (e,_) -> (int e.[1..],e.[0]))
   |> (Array.ofSeq >> Array.unzip)
   ||> Excel_Language_Interpreter.interpret 200 0

[<EntryPoint>]
let main argv = 
  let argv = "program.txt program.xlsx Brainfuck".Split ' ' //System.Console.ReadLine().Split ' '     //DEBUG
  match argv with
  |[|fileName_input; fileName_output; fileLanguage|] ->
    let fileName_input = sprintf @"%s\%s" __SOURCE_DIRECTORY__ fileName_input
    let fileName_output = sprintf @"%s\%s" __SOURCE_DIRECTORY__ fileName_output

    ( match fileLanguage with
      |"Brainfuck" -> Brainf____Parser.parseSyntax
      |"F#" -> FSharp_Parser.parseSyntax >> FSharp_Parser.translate

      |_ -> failwith "language not recognized"
     ) (File.ReadAllText fileName_input)
     |> debug

     |> PseudoAsm.convert       |> debug

//     |> PseudoAsm.interpret     |> printfn "%A"

     |> Excel_Conversion.writeExcel

//     |> interpret_cells         |> ignore

     |> Excel_Conversion.ActualWriteExcel.actually_write_the_excel fileName_output

    System.Console.ReadLine() |> ignore

  |_ -> failwith "instructions not understood"

  0