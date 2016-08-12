open System.IO
let debug e=printfn "%A" e; e
[<EntryPoint>]
let main argv = 
  let argv = "program.txt program.xlsx F#".Split ' ' //System.Console.ReadLine().Split ' '     //DEBUG
  match argv with
  |[|fileName_input; fileName_output; fileLanguage|] ->
    let fileName_input = sprintf @"%s\%s" __SOURCE_DIRECTORY__ fileName_input
    let fileName_output = sprintf @"%s\%s" __SOURCE_DIRECTORY__ fileName_output
    ( match fileLanguage with
      |"F#" -> FSharp_Parser.parseSyntax (File.ReadAllText fileName_input)

      |_ -> failwith "language not recognized"
     )
     |> FSharp_Parser.rewrite FSharp_Parser.Light
     |> printfn "%O"
//     |> debug
//     |> PseudoAsm.convert
//     |> debug
(*
     |> PseudoAsm.interpret
     |> printfn "%A"
*)
//     |> Excel_Conversion.writeExcel
(*
     |> Seq.map (function Excel_Language.Cell(a,b) -> (a,b))
     |> Seq.sortBy (fun (e,_) -> (int e.[1..],e.[0]))
     |> (Array.ofSeq >> Array.unzip)
     ||> Excel_Language_Interpreter.interpret 200 0
     |> ignore
*)
//     |> Excel_Conversion.ActualWriteExcel.actually_write_the_excel fileName_output

    System.Console.ReadLine() |> ignore

  |_ -> failwith "instructions not understood"

  0