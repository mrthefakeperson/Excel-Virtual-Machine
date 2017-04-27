module private ExcelLanguage.DefineVM
open PseudoASM.Definition
open Definition
open System

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
  let selfName = coordsToS(instrR + 1, instrC)
  let self = Reference selfName
  [ Cell(``instr*``, self)
    Cell(selfName,
      matchInstrWith [
        "Call", currentValue *. Int 2
        "GotoFwdShift", self +. (instr_index 1 *. Int 2)
        "GotoIfTrueFwdShift", If(currentValue, self +. (instr_index 1 *. Int 2), self +. Int 2)
        "Input", If(Reference ``inputMachine*``, self +. Int 2, self)
        "Return", currentValue *. Int 2
       ] (self +. Int 2)
       |> defaultTo (Int 0)
     )
    Cell(coordsToS(instrR + 2, instrC), currentInstruction)
    Cell(coordsToS(instrR + 3, instrC), instr_index 1)
      ] |> Seq.ofList

let valueStack =
  makeVerticalStack LARGE_SIZE ``value*``
   (matchInstrWith
     ([
      "Push", Int 1
      "PushFwdShift", Int 1
      "Pop", Int -1
      "Load", Int 1
      "Store", Int -1
      "NewHeap", Int 1
      "WriteHeap", Int -2
      "Output", Int -1
      "GotoIfTrueFwdShift", Int -1
      "Input", If(Reference ``inputMachine*``, Int 1, Int 0)
      "Return", Int -1
     ]
     @ List.map (fun (comb2:Comb2WithFormula) ->
         comb2.Name, Int -1
        ) allCombinators
     ) (Int 0)
    )
   (fun self ->
      matchInstrWith
       ([
        "Push", instr_index 1
        "PushFwdShift", Reference ``instr*`` /. Int 2 +. instr_index 1
        "Load", Index(Range("F" + string ``var*``, "XFD" + string ``var*``), instr_index 1)
        "NewHeap", Reference(coordsToS (heapR + 1, heapC)) -. Int 2    //size of heap
        "GetHeap", Index(Range(coordsToS (heapR + 2, heapC), "C" + string(heapR + 2 + LARGE_SIZE)), self +. Int 1)
        "Input", If(Reference ``inputMachine*``, Reference ``scannedInput*``, self)  // push input
        "Call", Reference ``instr*`` /. Int 2 +. Int 1
       ]
       @ List.map (fun (comb2:Comb2WithFormula) ->
           comb2.Name, comb2.ToFormula (Index(Range(coordsToS (valueR + 2, valueC), "B" + string(valueR + 2 + LARGE_SIZE)), valueTopstackPt +. Int 1)) self
          ) allCombinators
       ) self
    )

let heap =
  makeVerticalStack LARGE_SIZE ``heap*``
   (matchInstrWith ["NewHeap", Int 1] (Int 0))
   id
   |> Seq.mapi (fun i (Cell(s, e)) ->
        if i < 2 then Cell(s, e)
        else
          Cell(s,
            If((currentInstruction =. Literal "WriteHeap")
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
      If((currentInstruction =. Literal "Input") &&.
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
    If(currentInstruction =. Literal "Output",
      Concatenate [|Reference ``output*``; currentValue|],
      Reference ``output*``
     )
     |> defaultTo (Literal "stdout:\n")
   )

let variableStack row col =
  let selfName = name (row + 1) col
  let self = Reference selfName
  [ Cell(name row col, self)
    Cell(selfName,
      If((currentInstruction =. Literal "Store") &&. (instr_index 1 =. Literal col), currentValue, self)
       |> defaultTo (Literal "-")
    ) ]
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
  let variables = Seq.map (variableStack ``var*``) vars
  let cmds =
    Seq.map (function
      |Store e -> Store (varToColumnName e)
      |Load e -> Load (varToColumnName e)
      |x -> x
     ) cmds             //sequence of pAsm commands
//     |> Seq.mapi cmdToStrPair               //pairs of commands as string values
     |> Seq.map (fun e -> match e.CommandInfo.StringPair with "Load", s -> "Load", string(alphaToNumber s - 5) | x -> x)
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
