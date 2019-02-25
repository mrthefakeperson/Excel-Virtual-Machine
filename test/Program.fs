open Fuchu

//let simpleTest =
//    testCase "A simple test" <| 
//        fun _ -> Assert.Equal("2+2", 4, 2+2)

[<EntryPoint>]
let main _ =
  TestParser.run_all()

  TestInterpreter.test_declare()
  
  TestCodegen.TestHooks.run_all()
  TestCodegen.TestTypeCheck.run_all()
  TestCodegen.TestMain.run_all()

  for _ in 1..5 do E2e.test()

  ignore <| System.Console.ReadLine()
    
  0