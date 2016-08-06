module Excel_Conversion

open System
open PseudoAsm
open Excel_Language
open Excel_Language_Interpreter

//shortcuts
//let rf (Cell(s,_))=Reference s
let name (Reference s) = s
let num n=Literal (Choice1Of3 n)
let str s=Literal (Choice2Of3 s)
let bln b=Literal (Choice3Of3 b)
let [(<.); (<=.); (=.); (<>.); (>.); (>=.)]=List.init 6 (fun e a b -> Compare(a, enum e, b))
let [(+.); (-.); ( *. ); (/.)]=List.init 4 (fun e a b -> Combine(a, enum e, b))
let (^.) a b = Concatenate [|a; b|]
type Group(formulaSequence:Formula[])=
  //take a column
  new(column,start,finish) =
    Group [|for e in start..finish -> Reference(sprintf "%s%i" column e)|]
  member x.Item o = Choose(o, formulaSequence)
let writeExcel (allInstructions:AsmST list)=
  let stackSize, localSize, instructions =
    match allInstructions with
     Command maxstack::Command locals::rest ->
      match maxstack.Split ' ', locals.Split ' ' with
       [|"maxstack"; n|], [|"locals"; nn|] -> (int n, int nn, Command ""::Command ""::rest)
      |_ -> failwith "header wrong"
    |_ -> failwith "header wrong"
  //cell address declarations
  let ``*instIndex`` = Reference "F1"
  let ``*instName`` = Reference "G1"
  let ``*instArgument`` = Reference "H1"
  let ``*topstack`` = Reference "I1"
  let ``*stdin`` = Reference "J1"
  let ``*stdout`` = Reference "K1"
  let ``*stack`` = Group("L", 1, stackSize)
  let ``*local`` = Group("M", 1, localSize)
  let n = instructions.Length
  let ``*cmdColA``, ``*cmdColB``, ``*cmdColC`` = Group("A", 1, n), Group("B", 1, n), Group("C", 1, n)
  //cells
  let instIndex=
    Cell(name ``*instIndex``,
      If(``*instIndex`` =. num 0,
        num 1,
        If(``*instName`` =. str "goto if true",
          If(``*stack``.[``*topstack`` +. num 2],
            ``*stack``.[``*topstack`` +. num 1],
            num 1 +. ``*instIndex``
           ),
          If(``*instName`` =. str "end",
            ``*instIndex``,
            num 1 +. ``*instIndex``
           )
         )
       )
     )
  let instName=
    Cell(name ``*instName``, ``*cmdColA``.[``*instIndex``])
  let instArgument=
    Cell(name ``*instArgument``, ``*cmdColB``.[``*instIndex``])
  let topstack=
    Cell(name ``*topstack``,
      If(``*instIndex`` =. num 2,     //num 2, num 1 or num 0?
        ``*cmdColC``.[``*instIndex``],
        ``*topstack`` +. ``*cmdColC``.[``*instIndex``]
       )
     )
  let stdin=
    Cell(name ``*stdin``, str "")
  let stdout=
    Cell(name ``*stdout``,
      If(``*instIndex`` =. num 1,
        str "",
        If(``*instName`` =. str "print",
          ``*stdout`` ^. ``*stack``.[``*topstack``],
          ``*stdout``
         )
       )
     )
  let stack n=
    let label=sprintf "L%i" n
    Cell(label,
      If(``*instIndex`` =. num 1,      //num 1 or num 0?
        str "",
        If(``*topstack`` <>. num n,
          Reference label,
          If(``*instName`` =. str "push",
            ``*instArgument``,
            If(``*instName`` =. str "load",
              ``*local``.[``*instArgument`` +. num 1],

              Reference label     // nothing else
             )
           )
         )
       )
     )
  let local n=
    let label=sprintf "M%i" n
    Cell(label,
      If(``*instIndex`` =. num 1,     //num 1 or num 0?
        str "",
        If(``*instName`` =. str "store",
          If(``*instArgument`` +. num 1 =. num n,
            ``*stack``.[``*topstack`` +. num 1],
            Reference label
           ),
          Reference label
         )
       )
     )
  let cmdColA =
    List.mapi (fun i e ->
      let x =
        match e with
        |Push _ -> "push"
        |Pop -> "pop"
        |Load _ -> "load"
        |Store _ -> "store"
        |Marker s -> sprintf "%s:" s
        |GotoIfTrue -> "goto if true"
        |Command s -> s
      Cell(sprintf "A%i" (i+1), str x)
     ) instructions
  let cmdColB =
    let markedLines=
      Array.mapi (fun i e -> (e,i)) (Array.ofList instructions)
       |> Array.choose (function Marker e,i -> Some(e,i+1) | _ -> None)
       |> dict
    List.mapi (fun i e ->
      let x =
        match e with
        |Push (Int v) -> num v
        |Push (Str v) -> str v
        |Push (Bln v) -> bln v
        |Push (Goto v) -> num markedLines.[v]
        |Pop | Marker _ | GotoIfTrue | Command _ -> str ""
        |Load i | Store i -> num i
      Cell(sprintf "B%i" (i+1), x)
     ) instructions
  let cmdColC =
    List.mapi (fun i e ->
      let x =
        match e with
        |Push _ -> 1
        |Pop -> -1
        |Load _ -> 1
        |Store _ -> -1
        |Marker _ -> 0
        |GotoIfTrue -> -2
        |Command _ -> 0
      Cell(sprintf "C%i" (i+1), num x)
     ) instructions
  List.zip3 cmdColA cmdColB cmdColC
   |> List.iteri (fun i (Cell(_,a),Cell(_,b),Cell(_,c)) -> printfn "%A: %A, %A, %A" (i+1) a b c)
  seq {
    yield instIndex
    yield instName
    yield instArgument
    yield topstack
    yield stdin
    yield stdout
    yield! Seq.init stackSize ((+) 1 >> stack)
    yield! Seq.init localSize ((+) 1 >> local)
    yield! cmdColA
    yield! cmdColB
    yield! cmdColC
   }

module ActualWriteExcel=
  open Microsoft.Office.Core
  open Microsoft.Office.Interop.Excel
  open System.Reflection
  open System.IO
  let actually_write_the_excel fileName cells=
    if File.Exists fileName then File.Delete fileName
    let back=ApplicationClass()
    let sheet=back.Workbooks.Add().Worksheets.[1] :?> _Worksheet
    back.Calculation<-XlCalculation.xlCalculationManual
    back.CalculateBeforeSave<-false
    back.Iteration<-true
    back.MaxIterations<-1
    back.MaxChange<-0.
    back.Visible<-true
    let set cellname txt=
      sheet.Range(cellname).Value(Missing.Value) <- txt
    Seq.iter (fun (Cell(s,_) as f) -> set s (f.ToString())) cells
    sheet.SaveAs fileName
    back.Quit()



(*
type DecisionBuilder(name,?defaultValue)=
  new() = DecisionBuilder ""
  member x.Bind((cond,aff),rest) = If(cond,aff,rest(cond,aff))
  member x.Return xpr = xpr
  member x.Run xpr =
    let defaultTo n rest=
      If(Compare(Reference "F1", Comparison.Equal, num 0), n, rest)
    match defaultValue with
    |Some v -> Cell(name,defaultTo v xpr)
    |None -> Cell(name,xpr)
  static member Yield = function Cell(_,formula) -> formula

// A: instruction; B: next instruction; C: instruction argument; D: stack size change

//          new way: A -> command; B -> argument; C -> stack size change

//still need stdin

[<Literal>]
let ARRAY_SIZE = 0
let writeExcel' stackSize localSize (instructions:AsmST list)=
  let numInst=instructions.Length
  let column s n=
    Array.init n ((+) 1 >> sprintf "%s%i" s >> Reference)
  let instructionPointer=
    (DecisionBuilder("F1",num 1))
     {
      let! _ =   //goto if true
        Compare(Reference "G1", Comparison.Equal, str "goto if true"),
         DecisionBuilder.Yield (
           (DecisionBuilder())
            {
             let! _ =    //check for TRUE at topstack
               Choose(Reference "J1", column "N" stackSize), Reference "H1"
             return Choose(Reference "F1", column "B" numInst)
            }
          )
      return Choose(Reference "F1", column "B" numInst)    //next instruction
     }
  let currentInstruction=
    (DecisionBuilder("G1",str ""))
     { return Choose(rf instructionPointer, column "A" numInst) }
  let instructionArgument=
    (DecisionBuilder("H1",str ""))
     { return Choose(rf instructionPointer, column "C" numInst) }
  let stackSizeChange=
    (DecisionBuilder("I1",num 0))
     { return Choose(rf instructionPointer, column "D" numInst) }
  let topstack=
    (DecisionBuilder("J1",num 0))
     { return Combine(Reference "J1", Combinator.Plus, rf stackSizeChange) }
  let arrayMemoryPt=
    (DecisionBuilder("K1",num (localSize+1)))
     {
      let! _ =   //check command
        Compare(rf currentInstruction, Comparison.Unequal, str "new array"), Reference "K1"
      return     //allocate space for array
        Combine(Reference "K1", Combinator.Plus, Choose(rf topstack, column "N" stackSize))
     }
  let stack n=
    let n=n+1
    let name=sprintf "N%i" n
    (DecisionBuilder(name,str ""))
     {
      let! _ =   //compare to new topstack
        Compare(rf topstack, Comparison.Unequal, num n),
         DecisionBuilder.Yield (
           (DecisionBuilder())
            {
             //let! _ =   //delete when out of stack
               //Compare(rf topstack, Comparison.Less, num n),
                //str ""
             return Reference name
            }
          )
      //other command functionalities
      let! _ =   //push
        Compare(rf currentInstruction, Comparison.Equal, str "push"), rf instructionArgument
      let! _ =   //load
        Compare(rf currentInstruction, Comparison.Equal, str "load"),
         Choose(Combine(rf instructionArgument, Combinator.Plus, num 1), column "O" localSize)
      let! _ =   //load address
        Compare(rf currentInstruction, Comparison.Equal, str "load address"),
         Choose(Combine(Reference name, Combinator.Minus, num 100000), column "O" localSize)
      let! _ =   //load address of
        Compare(rf currentInstruction, Comparison.Equal, str "load address of"),
         Combine(rf instructionArgument, Combinator.Plus, num 100000)
      let! _ =   //new array
        Compare(rf currentInstruction, Comparison.Equal, str "new array"), rf arrayMemoryPt
      let! _ =   //load index
        Compare(rf currentInstruction, Comparison.Equal, str "load index"),
         Choose(
           Combine(Reference name, Combinator.Plus, Reference (sprintf "N%i" (n+1))),
           column "O" (localSize+ARRAY_SIZE)
          )
      let! _ =   //equals
        Compare(rf currentInstruction, Comparison.Equal, str "equals"),
         Compare(Reference name, Comparison.Equal, Reference (sprintf "N%i" (n+1)))
      let! _ =   //greater than
        Compare(rf currentInstruction, Comparison.Equal, str "greater than"),
         Compare(Reference name, Comparison.Greater, Reference (sprintf "N%i" (n+1)))
         //all else fails, don't change the value
      return Reference name
     }
  let local n=
    let n=n+1
    let name=sprintf "O%i" n
    (DecisionBuilder(name,num 0))      //num 0 is temporary until data types are implemented
     {
      let! _ =     //check for store instruction
        Compare(rf currentInstruction, Comparison.Equal, str "store"),
         DecisionBuilder.Yield (
           (DecisionBuilder())
            {
             let! _ =     //check for this cell being the argument
               Compare(rf instructionArgument, Comparison.Unequal, num (n-1)), Reference name
             return       //return by storing the topstack value here (popping is handled by stack pointer)
               Choose(Combine(rf topstack, Combinator.Plus, num 1), column "N" stackSize)
            }
          )
      let! _ =   //store index
        Compare(rf currentInstruction, Comparison.Equal, str "store index"),
         DecisionBuilder.Yield (
           (DecisionBuilder())
            {
             let! _ =    //argument provided
               Compare(
                 Combine(    //pointer arithmetic
                   Choose(Combine(rf topstack, Combinator.Plus, num 1), column "N" stackSize),
                   Combinator.Plus,
                   Choose(Combine(rf topstack, Combinator.Plus, num 2), column "N" stackSize)
                  ), Comparison.Equal, num n
                ), Choose(Combine(rf topstack, Combinator.Plus, num 3), column "N" stackSize)     //get the new value
             return Reference name     //if not the specified location, no change
            }
          )
      return Reference name
     }
  let stdin=
    Cell("L1", Reference "L1")     //not done yet, still need stdin/stdout to be implemented
  let stdout=
    DecisionBuilder("M1",str "")
     {
      let! _ =   //print command
        Compare(rf currentInstruction, Comparison.Unequal, str "print"), Reference "M1"
      return
        Concatenate
         [|Reference "M1"
           Choose(Combine(rf topstack, Combinator.Plus, num 1), column "N" stackSize)|]
     }

  //new way: A -> command; B -> argument; C -> stack size change
  let commandColumns=
    let markedLines=
      Array.mapi (fun i e -> (e,i)) (Array.ofList instructions)
       |> Array.choose (function Marker e,i -> Some(e,i) | _ -> None)
       |> dict
    let cA,cB,cC = (+) 1 >> sprintf "A%i",(+) 1 >> sprintf "B%i",(+) 1 >> sprintf "C%i"
    let yld i=function
      [a; b; c] -> [Cell(cA i, a); Cell(cB i, b); Cell(cC i, c)]
      |_ -> failwith "cannot do this"
    let empty=str ""
    List.mapi (fun i ->
      let i=i+1
      function
      |Command "end" -> [str "end"; empty; num 0]
      |Push v ->
        let vv=
          match v with
          |Int v -> num v
          |Str v -> str v
          |Bln v -> bln v
          |Unit -> num 0     //maybe temp
          |Goto v -> num (markedLines.[v])
        [str "push"; vv; num 1]
      |Pop -> [str "pop"; empty; num -1]
      |Load ind -> [str "load"; num ind; num 1]
      |Store ind -> [str "store"; num ind; num -1]
      |Marker s -> [str (s+":"); empty; num 0]
      |GotoIfTrue -> [str "goto if true"; empty; num -2]
      |Command c ->
        match c.Split ' ' with
        |[|"print"|] -> [str "print"; empty; num -1]

        |_ -> failwith "unknown command"
     ) instructions
     |> List.mapi yld
     |> List.concat
     
  seq {
    yield instructionPointer
    yield currentInstruction
    yield instructionArgument
    yield stackSizeChange
    yield topstack
    yield arrayMemoryPt
    yield stdin
    yield stdout
    yield! Seq.init stackSize stack
    yield! Seq.init (localSize+ARRAY_SIZE) local
    yield! commandColumns
   }
let writeExcel=function
  |Command stacksize_declaration::Command locals_declaration::restInstructions ->
    match stacksize_declaration.Split ' ', locals_declaration.Split ' ' with
    |[|_; maxstack|], [|_; locals|] ->
      writeExcel' (int maxstack) (int locals) (Command ""::restInstructions)
    |_ -> failwith "wrong commands"
  |_ -> failwith "wrong commands"

module ActualWriteExcel=
  open Microsoft.Office.Core
  open Microsoft.Office.Interop.Excel
  open System.Reflection
  open System.IO
  let actually_write_the_excel fileName cells=
    if File.Exists fileName then File.Delete fileName
    let back=ApplicationClass()
    let sheet=back.Workbooks.Add().Worksheets.[1] :?> _Worksheet
    back.Calculation<-XlCalculation.xlCalculationManual
    back.CalculateBeforeSave<-false
    back.Iteration<-true
    back.MaxIterations<-1
    back.MaxChange<-0.
    back.Visible<-true
    let set cellname txt=
      sheet.Range(cellname).Value(Missing.Value) <- txt
    Seq.iter (fun (Cell(s,_) as f) -> set s (f.ToString())) cells
    sheet.SaveAs fileName
    back.Quit()

*)