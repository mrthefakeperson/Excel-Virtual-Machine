module NUnitLite.Tests
open NUnitLite
open NUnit.Framework

open AST.Definition

open PseudoASM.Implementation
let runAST f = fromAST >> Array.ofSeq >> Testing.Interpreters.interpretPAsm false >> f
let runASTDebug f = fromAST >> Array.ofSeq >> Testing.Interpreters.interpretPAsm true >> f
let stack (a, _, _) = a
let heap (_, a, _) = a
let output (_, _, a) = a

[<Test>]
let ASMComb2Test1() =
  Assert.AreEqual(runAST stack (Apply(Apply(Value "+", [Const "5"]), [Const "-7"])), ["-2"])
[<Test>]
let ASMComb2Test2() =
  Assert.AreEqual(runAST stack (Apply(Apply(Value ">", [Const "5"]), [Const "7"])), ["False"])
[<Test>]
let ASMComb2Test3() =
  Assert.AreEqual(runAST stack (Apply(Apply(Value "<=", [Const "5"]), [Const "7"])), ["True"])
[<Test>]
let ASMComb2Test4() =
  Assert.AreEqual(runAST stack (Apply(Apply(Value "=", [Const "5"]), [Const "7"])), ["False"])
[<Test>]
let ASMComb2Test5() =
  Assert.AreEqual(runAST stack (Apply(Apply(Value "%", [Const "13"]), [Const "7"])), ["6"])
[<Test>]
let ASMAllocTest() =
  Assert.AreEqual((runAST heap (New (Const "3"))).ToArray().[30..], [|""; ""; ""; "endArr"|])
  Assert.AreEqual(runAST stack (New (Const "3")), ["30"])
[<Test>]
let ASMAllocTest2() =
  Assert.AreEqual((runAST heap (New (Const "1"))).ToArray().[30..], [|""; "endArr"|])
  Assert.AreEqual(runAST stack (New (Const "1")), ["30"])
[<Test>]
let ASMLoopTest() =
  Sequence [
    Declare("a", Const "5")
    Loop(Apply(Apply(Value ">", [Value "a"]), [Const "0"]),
      Sequence [New (Const "1"); Mutate("a", Apply(Apply(Value "+", [Value "a"]), [Const "-1"]))]
     )
   ]
   |> fun e -> Assert.AreEqual((runAST heap e).ToArray().[14..] |> Array.filter ((=) ""), [|""; ""; ""; ""; ""|])
[<Test>]
let ASMLoopTest2() =
  Sequence [
    Declare("a", Const "5")
    Loop(Apply(Apply(Value ">", [Value "a"]), [Const "0"]),
      Sequence [Mutate("a", Apply(Apply(Value "+", [Value "a"]), [Const "-1"])); Const "4"]
     )
   ]
   |> fun e -> Assert.AreEqual(runAST stack e, ["()"])
[<Test>]
let ASMLocalVariableTest() =
  Sequence [
    Declare("ff",
     Define("f", ["ee"],
      Sequence [
        Declare("e", Value "ee")
        If(Apply(Apply(Value "=", [Value "e"]), [Const "9"]),
          AST.Return(Some(Const "7")),
          AST.Return(Some <|
            Apply(Apply(Value "+", [Value "e"]),
              [Apply(Value "f", [Apply(Apply(Value "+", [Value "e"]), [Const "1"])])]) ) )
       ]
      ) )
    Define("g", ["()"], Apply(Value "ff", [Const "7"]))
    Apply(Value "g", [Const "()"])
   ]
   |> fun e -> Assert.AreEqual(runAST stack e, ["22"])

[<EntryPoint>]
let main argv =
  try AutoRun().Execute argv
  finally System.Console.ReadLine() |> ignore