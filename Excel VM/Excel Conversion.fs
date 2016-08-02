module Excel_Conversion
open System
open PseudoAsm
open Excel_Language
open Excel_Language_Interpreter

let rf (Cell(s,_))=Reference s
let num n=Literal (Choice1Of3 n)
let str s=Literal (Choice2Of3 s)
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

  let commandColumns=
    let markers=
      List.fold (fun (acc,i) -> function
        |Marker s -> (Map.add s i acc,i+1)
        |_ -> (acc,i+1)
       ) (Map.empty,1) instructions
       |> fst
    let cA,cB,cC,cD=
      (+) 1 >> sprintf "A%i",(+) 1 >> sprintf "B%i",(+) 1 >> sprintf "C%i",(+) 1 >> sprintf "D%i"
    let yld i=function
      [a; b; c; d] -> [Cell(cA i, a); Cell(cB i, b); Cell(cC i, c); Cell(cD i, d)]
      |_ -> failwith "cannot do this"
    let empty=str ""
    List.mapi (fun i ->
      let i=i+1
      function
      |Push v ->
        let vv=
          match v with
          |Choice1Of5 v -> num v
          |Choice2Of5 v -> str v
          |_ -> failwith "not implemented yet"
        [str "push"; num (i+1); vv; num 1]
      |Pop -> [str "pop"; num (i+1); empty; num -1]
      |Load ind -> [str "load"; num (i+1); num ind; num 1]
      |Store ind -> [str "store"; num (i+1); num ind; num -1]
      |Marker s -> [str (s+":"); num (i+1); empty; num 0]
      |Goto s -> [str ("goto:"+s); num markers.[s]; empty; num 0]
      |Command c ->
        match c.Split ' ' with
        |[|c|] ->
          match c with
          |"load_address" -> [str "load address"; num (i+1); empty; num -1]
          |"new_array" -> [str "new array"; num (i+1); empty; num 0]
          |"load_index" -> [str "load index"; num (i+1); empty; num -1]
          |"store_index" -> [str "store index"; num (i+1); empty; num -3]
          |"equals" -> [str "equals"; num (i+1); empty; num -1]
          |"greater_than" -> [str "greater than"; num (i+1); empty; num -1]
          |"print" -> [str "print"; num (i+1); empty; num -1]     //not done
          |"ret" -> [str "end"; num i; empty; num 0]

          |"" -> [str ""; num (i+1); empty; num 0]
          |_ -> failwith "unknown command"
        |[|"load_address_of"; n|] -> [str "load address of"; num (i+1); num (int n); num 1]
        |[|"goto_if_true"; pl|] -> [str "goto if true"; num (i+1); num (markers.[pl]); num -1]

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
  |Command locals_declaration::Command stacksize_declaration::Command "entrypoint"::restInstructions ->
    match locals_declaration.Split ' ', stacksize_declaration.Split ' ' with
    |[|_; locals|], [|_; maxstack|] ->
      writeExcel' (int maxstack) (locals.Split '|').Length (Command ""::restInstructions)
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
