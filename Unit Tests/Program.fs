module NUnitLite.Tests
open NUnitLite
open NUnit.Framework

[<EntryPoint>]
let main argv =
  try AutoRun().Execute argv
  finally System.Console.ReadLine() |> ignore