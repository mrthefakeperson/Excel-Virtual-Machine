//fix heap indexing before anything else
//more arithmetic
//test arithmetic

module Excel_Conversion
open Excel_Language
open System

let name row col = sprintf "%s%i" col row
let Int a = Literal (string a)
let defaultTo x e = If(Reference "A1" =. Literal "2", x, e)

[<Literal>]
let SZ = 60
let makeVerticalStack _name pushCondition pushValue =
  let column, row = separate _name
  let row = int row
  let _name = name (row+1) column
  seq {
    yield  //topstack value
      Cell(name row column,
        Index(Range(name (row+2) column, name (row+SZ+1) column), Reference _name)
       )
    //stack size (pushCondition is the change in size)
    yield Cell(_name, (Reference _name +. pushCondition) |> defaultTo (Int 1))
    //stack cells (pushValue is the new value)
    for e in row+1+1..row+1+SZ ->
      Cell(name e column,
        If(Int (e-row-1) =. Reference _name,
          pushValue (Reference (name e column)),
          Reference (name e column)
         )
         |> defaultTo (Int 1) )
   }

//where each topstack is located
let ``instr*`` = "A2"
let ``value*``, ``heap*`` = "B2", "C2"
let ``input*``, ``output*`` = "D2", "E2"

let instr_index i = Index(Range("B1", "XFD1"), Reference ``instr*`` +. Int i)     //16383 spaces in total, to XFD1
let currentInstruction = instr_index 0
let currentValue, valueTopstackPt = Reference ``value*``, Reference "B3"

let rec conditionTable defaultVal = function
  |(condFormula, action)::tl ->
    If(condFormula, action, conditionTable defaultVal tl)
  |[] -> defaultVal
let matchTable defaultVal =   //match the current instruction
  (List.map (fun (s, a) -> Literal s =. currentInstruction, a)) >> (conditionTable defaultVal)

let instructionStack =
  makeVerticalStack ``instr*``
   ([ "call", Int 1
      "return", Int -1
     ]
     |> matchTable (Int 0) )
   (fun self ->
    [ "call", currentValue *. Int 2 +. Int 1           // ? | call | ?
      "goto", instr_index 1 *. Int 2 +. Int 1          // ? | goto | arg1 | ?
      "gotoiftrue", If(currentValue, instr_index 1 *. Int 2 +. Int 1, self +. Int 2)
     ]
     |> matchTable (self +. Int 2) )

let valueStack =
  makeVerticalStack ``value*``
   ([ "push", Int 1
      "pop", Int -1
      "load", Int 1
      "store", Int -1
      "call", Int -1
      "newheap", Int 1
      "writeheap", Int -2
      "inputline", Int 1
      "outputline", Int -1
      "gotoiftrue", Int -1
      "add", Int -1
      "equals", Int -1
      "leq", Int -1
     ]
     |> matchTable (Int 0) )
   (fun self ->
    [ "push", instr_index 1
      "load", Index(Range("F2", "XFD2"), instr_index 1)
      "newheap", Reference "C3" -. Int 2    //size of heap
      "getheap", Index(Range("C4", "C54"), self +. Int 1)     //change C14 when changing stack constant
      "add", Index(Range("B4", "B54"), valueTopstackPt +. Int 1) +. self
      "equals", Index(Range("B4", "B54"), valueTopstackPt +. Int 1) =. self
      "leq", Index(Range("B4", "B54"), valueTopstackPt +. Int 1) <=. self
     ]
     |> matchTable self )

let heap =
  makeVerticalStack ``heap*``
   (matchTable (Int 0) ["newheap", Int 1])
   id
   |> Seq.mapi (fun i (Cell(s, e)) ->
        if i < 2 then Cell(s, e)
        else
          Cell(s,
            If((currentInstruction =. Literal "writeheap")
                &&. (Index(Range("B4", "B54"), valueTopstackPt +. Int 1) =. Int (i-2)),    //remember to change this when changing stack size constant
             Index(Range("B4", "B54"), valueTopstackPt +. Int 2),
             e)
             |> defaultTo (Int 1)
           )
       )

let input =
  makeVerticalStack ``input*``
   (matchTable (Int 0) ["inputline", Int 1])
   id
let output =
  makeVerticalStack ``output*``
   (matchTable (Int 0) ["outputline", Int 1])
   (fun self -> matchTable self ["outputline", currentValue])

let variableStack row col =
  makeVerticalStack (name row col)
   (conditionTable (Int 0) [
      If(currentInstruction =. Literal "store", instr_index 1 =. Literal col, Literal "false"),
       Int 1     //`store` pushes the value to the top of the stack, remembering previous values for recursion
      If(currentInstruction =. Literal "popv", instr_index 1 =. Literal col, Literal "false"),
       Int -1    //`popv` pops this variable
     ])
   (fun self ->
    conditionTable self [
      // ? | store | at | ?
      If(currentInstruction =. Literal "store", instr_index 1 =. Literal col, Literal "false"),
       Reference "B2"    //top value of valueStack
     ])

let seed = Cell("A1", Reference("A1") +. Literal "1")

open Compiler
open System.Collections.Generic
let cmdToStrPair (mapping: IDictionary<string, string>) i = function
  |Push e -> "push", e | PushFwdShift x -> "push", string(i + x) | Pop -> "pop", ""
  |Store e -> "store", mapping.[e] | Load e -> "load", string(alphaToNumber mapping.[e] - 5) | Popv e -> "popv", mapping.[e]
  |GotoFwdShift x -> "goto", string(i + x) | GotoIfTrueFwdShift x -> "gotoiftrue", string(i + x)
  |Call -> "call", "" | Return -> "return", ""
  |GetHeap -> "getheap", "" | NewHeap -> "newheap", "" | WriteHeap -> "writeheap", ""
  |InputLine -> "inputline", "" | OutputLine -> "outputline", ""
  |Add -> "add", "" | Equals -> "equals", "" | Greater -> "greater", "" | LEq -> "leq", ""
let packageProgram instructions vars =
  let variables = Seq.map (numberToAlpha >> (variableStack 2)) vars
  let cells =
    Seq.concat [
      Seq.singleton seed
      instructions
      instructionStack
      valueStack
      heap
      input
      output
      Seq.concat variables
     ]
     |> Array.ofSeq
  Array.sortBy (function Cell(e, _) -> coordinates e) cells
let makeProgram cmds =
  let cmds = Array.append cmds [|GotoFwdShift 0|]
  let p = Array.map string [|'A'..'E'|]
  let mapping =
    Array.append (Array.zip p p) (
      let k =
        Array.choose (function Store e | Load e | Popv e -> Some e | _ -> None) cmds
         |> Set.ofArray |> Set.toArray
      Array.zip k (Array.map string [|'F'..'E' + char k.Length|])
     )
     |> dict
  let instructions =
    Array.mapi (cmdToStrPair mapping) cmds
     |> Array.collect (fun (a, b) -> [|a; b|])
     |> Array.map2 (fun col cmd -> Cell(col + "1", Literal cmd))
         (Array.map numberToAlpha [|2..1 + cmds.Length * 2|])
  let variables = {alphaToNumber "F"..mapping.Count}
  packageProgram instructions variables