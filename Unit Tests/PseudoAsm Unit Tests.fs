module PseudoASM.UnitTests
open NUnitLite
open NUnit.Framework
open AST.Definition
open PseudoASM.Implementation

let runAST'<'T> debug (f: string list*string ResizeArray*string list -> 'T) ast: 'T =
  (ast, Map.empty) |> fromAST |> fst |> Array.ofSeq |> Testing.Interpreters.interpretPAsm debug |> f
let runAST<'a> = runAST'<'a> false
let runASTDebug<'a> = runAST'<'a> true
let stack (a, _, _) = a
let heap (_, a, _): string ResizeArray = a
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
      Sequence [
        Define("f", ["ee"],
          Sequence [
            Declare("e", Value "ee")
            If(Apply(Apply(Value "=", [Value "e"]), [Const "9"]),
              AST.Return(Const "7"),
              AST.Return(
                Apply(Apply(Value "+", [Value "e"]),
                 [Apply(Value "f", [Apply(Apply(Value "+", [Value "e"]), [Const "1"])])]) ) )
           ]
         )
        Value "f" ]
     )
    Define("g", ["()"], AST.Return(Apply(Value "ff", [Const "7"])))
    Apply(Value "g", [Const "()"])
   ]
   |> fun e -> printfn "%A" e; e
   |> fun e -> Assert.AreEqual(runAST stack e, ["22"])
[<Test>]
let ASMBreakTest() =
  Loop(Const "true", Break) |> runAST stack
   |> fun e -> Assert.AreEqual(e, ["()"])