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
  let stackSize, memPt, localSize, instructions =
    match allInstructions with
    Entrypoint(maxstack,mem,locals)::_ -> maxstack, mem, locals, Entrypoint(0,0,0)::allInstructions
    |_ -> failwith "header wrong"
  //cell address declarations
  let ``*instIndex`` = Reference "F1"
  let ``*instName`` = Reference "G1"
  let ``*instArgument`` = Reference "H1"
  let ``*topstack`` = Reference "I1"
  let ``*memPt`` = Reference "J1"
  let ``*stdin`` = Reference "K1"
  let ``*stdout`` = Reference "L1"
  let ``*stack`` = Group("M", 1, stackSize)
  let ``*local`` = Group("N", 1, localSize)
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
      If(``*instIndex`` =. num 2,
        ``*cmdColC``.[``*instIndex``],
        ``*topstack`` +. ``*cmdColC``.[``*instIndex``]
       )
     )
  let memoryPt =
    Cell(name ``*memPt``,
      If(``*instIndex`` =. num 2,
        num memPt,
        ``*memPt``
       )
       +. If(``*instName`` =. str "allocate next", num 1, num 0)
     )
  let stdin=
    Cell(name ``*stdin``, str "")
  let stdout=
    Cell(name ``*stdout``,
      If(``*instIndex`` =. num 1,
        str "",
        If(``*instName`` =. str "print",
          ``*stdout`` ^. ``*stack``.[``*topstack`` +. num 1],
          ``*stdout``
         )
       )
     )
  let stack n=
    let label=sprintf "M%i" n
    Cell(label,
      If(``*instIndex`` =. num 1,
        str "",
        If(``*topstack`` <>. num n,
          Reference label,
          If(``*instName`` =. str "push",
            ``*instArgument``,
            If(``*instName`` =. str "load",
              ``*local``.[Reference label +. num 1],
              If(``*instName`` =. str "add",
                Reference label +. Reference (sprintf "M%i" (n+1)),
                If(``*instName`` =. str "eql",
                  Reference label =. Reference (sprintf "M%i" (n+1)),
                  If(``*instName`` =. str "allocate",
                    ``*memPt``,
                    If(``*instName`` =. str "allocate next",
                      ``*memPt`` -. num 1,

                      Reference label
                     )
                   )
                 )
               )
             )
           )
         )
       )
     )
  let local n=
    let label=sprintf "N%i" n
    Cell(label,
      If(``*instIndex`` =. num 1,
        str "",
        If(``*instName`` =. str "store",
          If(``*stack``.[``*topstack`` +. num 2] +. num 1 =. num n,
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
        |Load -> "load"
        |Store -> "store"
        |Marker s -> sprintf "%s:" s
        |GotoIfTrue -> "goto if true"
        |Allocate -> "allocate"
        |AllocateNext -> "allocate next"
        |Print -> "print"
        |End -> "end"
        |Entrypoint _ -> ""
        |Add -> "add"
        |Eql -> "eql"
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
        |Push Unit -> num 0
        |_ -> str ""
      Cell(sprintf "B%i" (i+1), x)
     ) instructions
  let cmdColC =
    List.mapi (fun i e ->
      let x =
        match e with
        |Push _ -> 1
        |Pop -> -1
        |Load -> 0
        |Store -> -2
        |Marker _ -> 0
        |GotoIfTrue -> -2
        |Allocate -> 1
        |AllocateNext -> 1
        |Print -> -1
        |Entrypoint _ -> 0
        |End -> 0
        |Add -> -1
        |Eql -> -1
      Cell(sprintf "C%i" (i+1), num x)
     ) instructions
  List.zip3 cmdColA cmdColB cmdColC
   |> List.iteri (fun i (Cell(_,a),Cell(_,b),Cell(_,c)) -> printfn "%A: %A, %A, %A" (i+1) a b c)
  seq {
    yield instIndex
    yield instName
    yield instArgument
    yield topstack
    yield memoryPt
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

