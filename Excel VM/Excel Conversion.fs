//more arithmetic
namespace Excel_Language
open Definitions
open ASM_Compiler
open Write_File.Definitions
open System

module Define_VM =
  // size of stacks (two categories)
  [<Literal>]   //400
  let LARGE_SIZE = 400
  [<Literal>]   //20
  let SMALL_SIZE = 75

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
  let ``inputMachine*`` = "D3"
  let inputR, inputC = coordinates ``inputMachine*``
  let ``scannedInput*`` = coordsToS (inputR + 1, inputC)
  let ``output*`` = "E3"
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

  let rec conditionTable conditionBindings defaultVal =
    match conditionBindings with
    |(condFormula, action)::tl ->
      If(condFormula, action, conditionTable tl defaultVal)
    |[] -> defaultVal
  let matchInstrWith bindings defaultVal =   //match the current instruction
    conditionTable
     (List.map (fun (s, a) -> Literal s =. currentInstruction, a) bindings)
     defaultVal

  let instructionStack =
    makeVerticalStack SMALL_SIZE ``instr*``
     (matchInstrWith [
        "call", Int 1
        "return", Int -1
       ] (Int 0)
      )
     (fun self ->
        matchInstrWith [
          "call", currentValue *. Int 2           // ? | call | ?
          "goto", instr_index 1 *. Int 2          // ? | goto | arg1 | ?
          "gotoiftrue", If(currentValue, instr_index 1 *. Int 2, self +. Int 2)
          "input", If(Reference ``inputMachine*``, self +. Int 2, self +. Int 0)
         ] (self +. Int 2)
      )

  let valueStack =
    makeVerticalStack SMALL_SIZE ``value*``
     (matchInstrWith
       ([
        "push", Int 1
        "pop", Int -1
        "load", Int 1
        "store", Int -1
        "call", Int -1
        "newheap", Int 1
        "writeheap", Int -2
        "inputline", Int 1
        "outputline", Int -1
        "gotoiftrue", Int -1
        "input", If(Reference ``inputMachine*``, Int 1, Int 0)
       ]
       @ List.map (function
           |Combinator_2 c -> c.Name, Int -1
           |_ -> failwith "non-combinator found in the combinator list"
          ) allCombinators
       ) (Int 0)
      )
     (fun self ->
        matchInstrWith
         ([
          "push", instr_index 1
          "load", Index(Range("F" + string ``var*``, "XFD" + string ``var*``), instr_index 1)
          "newheap", Reference(coordsToS (heapR + 1, heapC)) -. Int 2    //size of heap
          "getheap", Index(Range(coordsToS (heapR + 2, heapC), "C" + string(heapR + 2 + LARGE_SIZE)), self +. Int 1)
          "input", If(Reference ``inputMachine*``, Reference ``scannedInput*``, self)  // push input
         ]
         @ List.map (function
             |Combinator_2 c ->
               c.Name, c.CreateFormula (Index(Range(coordsToS (valueR + 2, valueC), "B" + string(valueR + 2 + LARGE_SIZE)), valueTopstackPt +. Int 1)) self
             |_ -> failwith "non-combinator found in the combinator list"
            ) allCombinators
         ) self
      )

  let heap =
    makeVerticalStack LARGE_SIZE ``heap*``
     (matchInstrWith ["newheap", Int 1] (Int 0))
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

  let input =
    let ix, iy = coordinates ``inputMachine*``
    let ``done``, ``dup``, ``position``, ``output`` =
      ``inputMachine*``, coordsToS(ix + 1, iy), coordsToS (ix + 3, iy), coordsToS (ix + 2, iy)
    let c = IndexStr(Concatenate [|Reference ``allInput*``; Line_Break|], Reference ``position``)
    let doneStateCell =
      Cell(``done``,
        If((currentInstruction =. Literal "input") &&.
          conditionTable [
            // out of bounds issues: (if) ``position`` >. len(``allInput``), (->) Literal "TRUE"
            // just scan anything delimited by whitespace for now
            instr_index 1 =. Literal "%i",
             Reference ``output`` =. Literal "" ||. (c <>. Literal " " &&. (c <>. Line_Break))
            instr_index 1 =. Literal "%s",
             Reference ``output`` =. Literal "" ||. (c <>. Literal " " &&. (c <>. Line_Break))
           ] (Literal "FALSE"),
          Literal "FALSE", Literal "TRUE")
       )
    let positionCell =
      Cell(``position``,
        If(Reference ``done``, Reference ``position``, Reference ``position`` +. Int 1)
         |> defaultTo (Int 1)
       )
    let outputCell =
      Cell(``output``,
        If(Reference ``done``, Literal "",
          Concatenate [|
            Reference ``output``;
            conditionTable [
              (instr_index 1 =. Literal "%i") &&. (c <>. Literal " ") &&. (c <>. Line_Break), c
              (instr_index 1 =. Literal "%s") &&. (c <>. Literal " ") &&. (c <>. Line_Break), c
             ] (Literal "")
           |]
         )
       )
    let dupCell = Cell(``dup``, Reference ``output``)
    Seq.ofList [doneStateCell; dupCell; positionCell; outputCell]

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
     (conditionTable [
        If(currentInstruction =. Literal "store", instr_index 1 =. Literal col, Literal "false"),
         Int 1     //`store` pushes the value to the top of the stack, remembering previous values for recursion
        If(currentInstruction =. Literal "popv", instr_index 1 =. Literal col, Literal "false"),
         Int -1    //`popv` pops this variable
       ] (Int 0))
     (fun self ->
      conditionTable [
        // ? | store | at | ?
        If(currentInstruction =. Literal "store", instr_index 1 =. Literal col, Literal "false"),
         Reference ``value*``
       ] self)

  let seed = Cell(``seed*``, Reference ``seed*`` +. Int 1)
  let allOutput = Cell(``allOutput*``, Reference ``output*``)

  let makeProgram cmds =
    let vars =
      Array.choose (function Store s -> Some s | _ -> None) cmds
       |> Set.ofArray |> Set.toSeq
    let varToColumnName =
      let v2c =
        Seq.mapi (fun i e -> (e, numberToAlpha(i + alphaToNumber "F"))) vars
         |> dict
      fun e -> v2c.[e]
    let vars = Seq.map varToColumnName vars
    let variables = Seq.map (variableStack SMALL_SIZE ``var*``) vars
    let cmds =
      Seq.map (function
        |Store e -> Store (varToColumnName e)
        |Load e -> Load (varToColumnName e)
        |Popv e -> Popv (varToColumnName e)
        |x -> x
       ) cmds             //sequence of pAsm commands
       |> Seq.mapi cmdToStrPair               //pairs of commands as string values
       |> Seq.collect (fun (a, b) -> [a; b])  //final sequence of commands as string values (as they will be written)
       |> Seq.mapi (fun i e ->
            let cellCoords = coordsToS (1, 2 + i)
            let cellContents = Literal e
            Cell(cellCoords, cellContents)            //final sequence of commands as cells
           )
    let cells =
      Seq.concat [
        Seq.singleton seed
        Seq.singleton allOutput
        cmds
        instructionStack
        valueStack
        heap
        input
        Seq.singleton output
        Seq.concat variables
       ]
       |> Array.ofSeq
    Array.sortBy (function Cell(e, _) -> coordinates e) cells
