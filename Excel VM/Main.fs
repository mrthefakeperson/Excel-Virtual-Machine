open Testing

[<EntryPoint>]
let main argv =
  match argv with
  |[|"test"|] -> runSpecificTest()

  0

