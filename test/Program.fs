open Fuchu

let simpleTest = 
    testCase "A simple test" <| 
        fun _ -> Assert.Equal("2+2", 4, 2+2)

[<EntryPoint>]
let main _ =
  run simpleTest