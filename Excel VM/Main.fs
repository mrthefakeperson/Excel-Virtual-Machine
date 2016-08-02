open System.IO
[<EntryPoint>]
let main argv = 
  let argv = System.Console.ReadLine().Split ' '     //DEBUG
  match argv with
  |[|fileName_input; fileName_output; fileLanguage|] ->
    ( match fileLanguage with
      |"F#" -> FSharp_Parser.parseSyntax (File.ReadAllText fileName_input)

      |_ -> failwith "language not recognized"
     )
     |> PseudoAsm.convert
     |> Excel_Conversion.writeExcel
     |> Excel_Conversion.ActualWriteExcel.actually_write_the_excel fileName_output

  |_ -> failwith "instructions not understood"

  0