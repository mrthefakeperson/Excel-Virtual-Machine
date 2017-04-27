namespace Testing
open ExcelLanguage.Definition
open PseudoASM.Definition
open System

module Interpreters =
  [<AbstractClass>]
  type Op(x:PseudoASM) =
    inherit Comb2("", (x.CommandInfo :?> Comb2).Symbol)
    abstract member Apply: string -> string -> string
  let implementForType<'T> (cast:string -> 'T) (f:'T -> 'T -> string) a b = f (cast a) (cast b)
  let k f a b = (f a b).ToString()
  let castToIntThen = implementForType<int> int
  let checkIntFirst fint fstr a b =
    if fst (Int32.TryParse a) && fst (Int32.TryParse b)
     then castToIntThen fint a b
     else fstr a b
  let castToBoolThen = implementForType<bool> (fun s ->
    match s.ToLower() with
    |"true" -> true
    |"false" -> false
    |_ -> failwith "not a bool"
   )
  let allCombinators = [  //[Add; Sub; Mul; Div; Mod; Less; LEq; Equals; NotEq; Greater; GEq; And; Or]
    {new Op(Add) with override x.Apply a b = castToIntThen (k (+)) a b}
    {new Op(Sub) with override x.Apply a b = castToIntThen (k (-)) a b}
    {new Op(Mul) with override x.Apply a b = castToIntThen (k (*)) a b}
    {new Op(Div) with override x.Apply a b = castToIntThen (k (/)) a b}
    {new Op(Mod) with override x.Apply a b = castToIntThen (k (%)) a b}
    {new Op(Less) with override x.Apply a b = checkIntFirst (k (<)) (k (<)) a b}
    {new Op(LEq) with override x.Apply a b = checkIntFirst (k (<=)) (k (<=)) a b}
    {new Op(Equals) with override x.Apply a b = checkIntFirst (k (=)) (k (=)) a b}
    {new Op(NotEq) with override x.Apply a b = checkIntFirst (k (<>)) (k (<>)) a b}
    {new Op(Greater) with override x.Apply a b = checkIntFirst (k (>)) (k (>)) a b}
    {new Op(GEq) with override x.Apply a b = checkIntFirst (k (>=)) (k (>=)) a b}
    {new Op(And) with override x.Apply a b = castToBoolThen (k (&&)) a b}
    {new Op(Or) with override x.Apply a b = castToBoolThen (k (||)) a b}
   ]
  let interpretPAsm debug cmds =
    let pushstack stack v = stack := v :: !stack
    let popstack stack = stack := match !stack with _ :: tl -> tl | [] -> []
    let topstack stack = match !stack with hd :: _ -> hd | [] -> failwith "took top of an empty stack"
    let stacks =
      let x =
        Array.append (Array.map string [|'A'..'E'|])
         <| Array.choose (function Store e | Load e -> Some e | _ -> None) cmds
         |> Set.ofArray |> Set.toArray
      printfn "%A" x
      (x, Array.init x.Length (fun _ -> ref ["0"]))
       ||> Array.zip
       |> dict
    let push name = pushstack stacks.[name]
    let pop name = popstack stacks.[name]
    let top name = topstack stacks.[name]
    let instr, value, _, _, output = "A", "B", "C", "D", "E"
    pop instr; pop value; pop output
    let heap = ResizeArray()
    let interpretCmd i =
      let fwd = (+) (i-1) >> string     // i-1 because +1 to instrPt gets appended to the end of every cmd
      let fwd2 = (+) i >> string
      function
      |Push s -> push value s
      |PushFwdShift ii -> push value (fwd2 ii)
      |Pop -> pop value
      |Store var -> pop var; push var (top value); pop value
      |Load var -> push value (top var)
      |GotoFwdShift ii -> pop instr; push instr (fwd ii)
      |GotoIfTrueFwdShift ii -> (if (top value).ToLower() = "true" then pop instr; push instr (fwd ii)); pop value  //not perfect
      |Call -> let newInstr = string(int(top value) - 1) in pop value; push value (string(int(top instr) + 1)); pop instr; push instr newInstr
      |Return -> pop instr; push instr (string(int(top value) - 1)); pop value
      |GetHeap -> let yld = heap.[int(top value)] in pop value; push value yld
      |NewHeap -> heap.Add ""; push value (string(heap.Count-1))
      |WriteHeap -> let v = top value in pop value; let i = top value in pop value; heap.[int i] <- v
      |Input _ -> push value (stdin.ReadLine())
      |Output _ -> push output (top value); pop value
      |Combinator_2(_, _) as c ->
        let a = top value in pop value
        let b = top value in pop value
        let op = List.find (fun (e:Op) -> (c.CommandInfo :?> Comb2).Symbol = e.Symbol) allCombinators
        push value (op.Apply a b)
    push instr "0"
    while int(top instr) < Array.length cmds && List.length !stacks.[output] < 50 do
      let i = int(top instr)
//      let debug = cmds.[i] = Store "*TCO" || cmds.[i] = Load "*TCO"
      if debug then printfn "%A" cmds.[i]
      try
        interpretCmd i cmds.[i]
      with
      |ex ->
        printfn "%A" ex
        printfn "stack %A" !stacks.[value]
        printfn "heap [%s]" (String.concat "; " (Seq.map (sprintf "%A") heap))
        printfn "%A" cmds.[i]
        failwith "failed"
      if debug then
        printfn "stack %A" !stacks.[value]
        printfn "heap [%s]" (String.concat "; " (Seq.map (sprintf "%A") heap))
        printfn "instruction %s" (top instr)
      let pt = int(top instr)
      pop instr; push instr (string(pt+1))
      if debug then ignore (stdin.ReadLine())
    List.iter (printf "%s") (List.rev !stacks.[output])
    !stacks.[value], heap, List.rev !stacks.[output]

  type Interpreted =                //dynamic typing, except for errors
    |S of string
    |ERROR
  let (|V|N|B|Error|) = function
    |S s ->
      match s.ToLower() with
      |"" -> N 0.
      |"true" -> B true | "false" -> B false
      |s when Double.TryParse(s, ref 0.) -> N (float s)
      |_ -> V (s.ToUpper())
    |ERROR -> Error
  let error x message =
  //printfn "%s: %A" message x
  //ignore (stdin.ReadLine())
    ERROR
  let combOp (x:Combinator) =
    let _N op a b =
      match a,b with
      |N a, N b -> S (string(op a b))
      |x -> error x "int cast"
    let _comp (op:System.IComparable->System.IComparable->bool) a b =
      match a,b with
      |N a, N b -> S (string(op a b))
      |B a, B b -> S (string(op a b))    //true > false is how this is evaluated
      |S a, S b -> S (string(op a b))  //default to comparing strings
      |x -> error x "string cast"
    let _B op a b =
      match a,b with
      |B a, B b -> S (string(op a b))
      |x -> error x "bool cast"
    let a = [|
      _N (+); _N (-); _N (*); _N (/)
      _comp (<); _comp (<=); _comp (=); _comp (<>); _comp (>); _comp (>=)
      _B (&&); _B (||)
     |]
    a.[int x]
  let inline ret e = S ((string e).ToUpper())
  let getRange = function
    |Range(a, b) ->
      let (r1, c1), (r2, c2) = coordinates a, coordinates b
      [|for r in r1..r2 do for c in c1..c2 -> Reference(coordsToS (r, c))|]
    |_ -> failwith "couldn't get range"

  let interpret iterations maxChange cellNames (cells:Formula[]) =
    let buffer = Array.create cells.Length ERROR //(ret 0)
    buffer.[0] <- ret 0
    let cellsByName = dict (Array.mapi (fun i e -> (e,i)) cellNames)
    let getCell e = try buffer.[cellsByName.[e]] with x -> failwithf "getCell %A: %A" e x
    let rec evaluateCell = function
      |Literal v -> ret v
      |Reference s -> getCell s
      |Range _ -> failwith "standalone range object not supported"
      |If(cond,aff,neg) ->
        match evaluateCell cond with
        |B true -> evaluateCell aff
        |B false -> evaluateCell neg
        |x -> error x "conditional expression"
      |Choose(n,choices) ->
        match evaluateCell n with
        |N i when 1 <= int i && int i <= choices.Length -> evaluateCell choices.[int i-1]
        |x -> error x "choice"
      |Index(range, k) ->
        match range with
        |Range(a, b) ->
          match coordinates a, coordinates b, evaluateCell k with
          |(r1, c1), (r2, c2), N i when int i < 1 || int i > (r2-r1+1)*(c2-c1+1) -> error i "index out of bounds"
          |(r1, c1), (r2, c2), N i when c1 = c2 -> evaluateCell (Reference(coordsToS (r1+int i-1, c1)))
          |(r1, c1), (r2, c2), N i when r1 = r2 -> evaluateCell (Reference(coordsToS (r1, c1+int i-1)))
          |_ -> evaluateCell (Choose(k, getRange range))
        |_ -> evaluateCell (Choose(k, getRange range))
      |IndexStr(s, i) ->
        match evaluateCell s, evaluateCell i with
        |S s, N i -> S (string s.[int i])     // indexing with float is not the best
        |_ -> failwith "index string"
      |Combine(a,comb,b) -> combOp comb (evaluateCell a) (evaluateCell b)
      |Concatenate a ->
        Array.fold (fun acc e ->
          match acc, evaluateCell e with
          |S a, S b -> S (a + b)
          |x -> error x "concatenation"
         ) (S "") a
      |Line_Break -> S "\n"
    let variables, constants =
      Array.mapi (fun i e -> i, e) cells
       |> Array.partition (fun (i, e:Formula) -> e.hasReference())
    Array.iter (fun (i, e) -> buffer.[i] <- evaluateCell e) constants
    match variables with
    |[||] -> ()
    |_ ->
      let i, primary = variables.[0]         //don't remember whether the first cell gets evaluated once or twice
      buffer.[i] <- evaluateCell primary     //it seems to be evaluated once
      for e in 1..iterations do
        Array.iter (fun (i, e) ->
          buffer.[i] <- evaluateCell e
         ) variables
    Array.zip cellNames buffer
