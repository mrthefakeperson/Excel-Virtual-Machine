module PseudoASM.Definition

type Cmd(name, ?arg: obj) =
  member x.Name = name
  override x.ToString() = sprintf "%O ( %O )" name arg
  abstract member StringPair: string*string
  default x.StringPair =
    match arg with
    |Some arg when (arg :? string) -> name, arg :?> string
    |Some arg when (arg :? int) -> name, string(arg :?> int)
    |None -> name, ""
    |Some arg -> failwithf "argument type not recognized: %O" arg

type Comb2(name, symbol) =
  inherit Cmd("Combinator_2", name)
  override x.StringPair = name, ""
  member x.Symbol = symbol

type PseudoASM =
  |Push of string
  |PushFwdShift of int     //push the number equal to (the address of this instruction) + (int)
  |Pop
  |Store of string
  |Load of string
  |GotoFwdShift of int
  |GotoIfTrueFwdShift of int
  |Call
  |Return
  |NewHeap     //allocate a new spot in heap (update size, make sure to `WriteHeap (size) (value)` before)
  |GetHeap     //let i = topstack; pop stack; push heap value at i to stack
  |WriteHeap   //let v = topstack; pop stack; let i = topstack; pop stack; heap at i <- v
  |Input of string
  |Output of string
  |Combinator_2 of string*string
   with
    member case.CommandInfo =
      match case with
      |Push x -> Cmd("Push", x)
      |PushFwdShift x -> Cmd("PushFwdShift", x)  //push the number equal to (the address of this instruction) + (int)
      |Pop -> Cmd "Pop"
      |Store x -> Cmd("Store", x)
      |Load x -> Cmd("Load", x)
      |GotoFwdShift x -> Cmd("GotoFwdShift", x)
      |GotoIfTrueFwdShift x -> Cmd("GotoIfTrueFwdShift", x)
      |Call -> Cmd "Call"
      |Return -> Cmd "Return"
      |NewHeap -> Cmd "NewHeap"      //allocate a new spot in heap (update size, make sure to `WriteHeap (size) (value)` before)
      |GetHeap -> Cmd "GetHeap"      //let i = topstack; pop stack; push heap value at i to stack
      |WriteHeap -> Cmd "WriteHeap"  //let v = topstack; pop stack; let i = topstack; pop stack; heap at i <- v
      |Input x -> Cmd("Input", x)
      |Output x -> Cmd("Output", x)
      |Combinator_2(name, symbol) -> Comb2(name, symbol) :> Cmd

let private c2 = Combinator_2
let Add, Sub, Mul, Div, Mod =
  c2("Add", "+"), c2("Sub", "-"), c2("Mul", "*"), c2("Div", "/"), c2("Mod", "%")
let Equals, Greater, Less, NotEq, GEq, LEq =
  c2("Equals", "="), c2("Greater", ">"), c2("Less", "<"),
  c2("NotEq", "<>"), c2("GEq", ">="), c2("LEq", "<=")
let And, Or = c2("And", "&&"), c2("Or", "||")
let allCombinators = [Add; Sub; Mul; Div; Mod; Less; LEq; Equals; NotEq; Greater; GEq; And; Or]

let allFormatSymbols = ["%i"; "%s"]
let stringFormat = "%s"