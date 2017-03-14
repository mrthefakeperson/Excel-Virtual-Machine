﻿//more arithmetic
//test arithmetic

module Excel_Conversion
open Compiler_Definitions
open Excel_Language
open System

// size of stacks (two categories)
[<Literal>]
let LARGE_SIZE = 400
[<Literal>]
let SMALL_SIZE = 20

// locations of important cells
[<Literal>]
let ``seed*`` = "A1"
let ``allInput*``, ``allOutput*`` = "A2", "B2"
[<Literal>]
let ``instr*`` = "A3"
let instrR, instrC = coordinates ``instr*``
[<Literal>]
let ``value*`` = "B3"
let valueR, valueC = coordinates ``value*``
[<Literal>]
let ``heap*`` = "C3"
let heapR, heapC = coordinates ``heap*``
let ``input*``, ``output*`` = "D3", "E3"
[<Literal>]
let ``var*`` = 3

let name row col = sprintf "%s%i" col row
let Int a = Literal (string a)
let defaultTo x e = If(Reference ``seed*`` =. Int 2, x, e)

let makeVerticalStack sz _name pushCondition pushValue =
  let column, row = separate _name
  let row = int row
  let _name = name (row+1) column
  let blankIsZero f = If(f =. Literal "", Int 0, f)
  seq {
    yield  //topstack value
      Cell(name row column,
        blankIsZero(Index(Range(name (row+2) column, name (row+sz+1) column), Reference _name))
       )
    //stack size (pushCondition is the change in size)
    yield Cell(_name, (Reference _name +. pushCondition) |> defaultTo (Int 1))
    //stack cells (pushValue is the new value)
    for e in row+1+1..row+1+sz ->
      Cell(name e column,
        If(Int (e-row-1) =. Reference _name,
          pushValue (Reference (name e column)),
          Reference (name e column)
         )
         |> defaultTo (Literal "") )
   }

let instr_index i = Index(Range("B1", "XFD1"), Reference ``instr*`` +. Int 1 +. Int i)     //16383 spaces in total, to XFD1
let currentInstruction = instr_index 0
let currentValue, valueTopstackPt = Reference ``value*``, Reference(coordsToS (valueR + 1, valueC))

let rec conditionTable defaultVal = function
  |(condFormula, action)::tl ->
    If(condFormula, action, conditionTable defaultVal tl)
  |[] -> defaultVal
let matchTable defaultVal =   //match the current instruction
  (List.map (fun (s, a) -> Literal s =. currentInstruction, a)) >> (conditionTable defaultVal)

let instructionStack =
  makeVerticalStack LARGE_SIZE ``instr*``
   ([ "call", Int 1
      "return", Int -1
     ]
     |> matchTable (Int 0) )
   (fun self ->
    [ "call", currentValue *. Int 2           // ? | call | ?
      "goto", instr_index 1 *. Int 2          // ? | goto | arg1 | ?
      "gotoiftrue", If(currentValue, instr_index 1 *. Int 2, self +. Int 2)
     ]
     |> matchTable (self +. Int 2) )

let valueStack =
  makeVerticalStack LARGE_SIZE ``value*``
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
     ]
     @ List.map (function
         |Combinator_2 c -> c.Name, Int -1
         |_ -> failwith "non-combinator found in the combinator list"
        ) allCombinators
     |> matchTable (Int 0) )
   (fun self ->
    [ "push", instr_index 1
      "load", Index(Range("F" + string ``var*``, "XFD" + string ``var*``), instr_index 1)
      "newheap", Reference(coordsToS (heapR + 1, heapC)) -. Int 2    //size of heap
      "getheap", Index(Range(coordsToS (heapR + 2, heapC), "C" + string(heapR + 2 + LARGE_SIZE)), self +. Int 1)
     ]
     @ List.map (function
         |Combinator_2 c ->
           c.Name, c.CreateFormula (Index(Range(coordsToS (valueR + 2, valueC), "B" + string(valueR + 2 + LARGE_SIZE)), valueTopstackPt +. Int 1)) self
         |_ -> failwith "non-combinator found in the combinator list"
        ) allCombinators
     |> matchTable self )

let heap =
  makeVerticalStack LARGE_SIZE ``heap*``
   (matchTable (Int 0) ["newheap", Int 1])
   id
   |> Seq.mapi (fun i (Cell(s, e)) ->
        if i < 2 then Cell(s, e)
        else
          Cell(s,
            If((currentInstruction =. Literal "writeheap")
                &&. (Index(Range(coordsToS(valueR + 2, valueC), "B" + string(valueR + 2 + LARGE_SIZE)), valueTopstackPt +. Int 1) =. Int (i-2)),
             Index(Range(coordsToS(valueR + 2, valueC), "B" + string(valueR + 2 + LARGE_SIZE)), valueTopstackPt +. Int 2),
             e)
             |> defaultTo (Int 1)
           )
       )

let input =                           //todo: change this
  makeVerticalStack SMALL_SIZE ``input*``
   (matchTable (Int 0) ["inputline", Int 1])
   id
let output =
  Cell (``output*``,
    If(currentInstruction =. Literal "outputline",
      Concatenate [|Reference ``output*``; Line_Break; currentValue|],
      Reference ``output*``
     )
     |> defaultTo (Literal "stdout:")
   )

let variableStack sz row col =
  makeVerticalStack sz (name row col)
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
       Reference ``value*``
     ])

let seed = Cell(``seed*``, Reference ``seed*`` +. Int 1)
let allOutput = Cell(``allOutput*``, Reference ``output*``)

open Compiler_Definitions
open System.Collections.Generic
let packageProgram instructions vars =
  let variables = Seq.map (numberToAlpha >> (variableStack LARGE_SIZE ``var*``)) vars
  let cells =
    Seq.concat [
      Seq.singleton seed
      Seq.singleton allOutput
      instructions
      instructionStack
      valueStack
      heap
      input
      Seq.singleton output
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