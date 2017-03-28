namespace Testing
open Excel_Language
open ASM_Compiler
open System

module Interpreters =
  let interpretPAsm cmds =
    let pushstack stack v = stack := v :: !stack
    let popstack stack = stack := match !stack with _ :: tl -> tl | [] -> []
    let topstack stack = match !stack with hd :: _ -> hd | [] -> failwith "took top of an empty stack"
    let stacks =
      let x =
        Array.append (Array.map string [|'A'..'E'|])
         <| Array.choose (function Store e | Load e | Popv e -> Some e | _ -> None) cmds
         |> Set.ofArray |> Set.toArray
      printfn "%A" x
      (x, Array.init x.Length (fun _ -> ref []))
       ||> Array.zip
       |> dict
    let push name = pushstack stacks.[name]
    let pop name = popstack stacks.[name]
    let top name = topstack stacks.[name]
    let instr, value, _, _, output = "A", "B", "C", "D", "E"
    let heap = ResizeArray()
    let interpretCmd i =
      let fwd = (+) (i-1) >> string     // i-1 because +1 to instrPt gets appended to the end of every cmd
      let fwd2 = (+) i >> string
      function
      |Push s -> push value s
      |PushFwdShift ii -> push value (fwd2 ii)
      |Pop -> pop value
      |Store var -> push var (top value); pop value
      |Load var -> push value (top var)
      |Popv var -> pop var
      |GotoFwdShift ii -> pop instr; push instr (fwd ii)
      |GotoIfTrueFwdShift ii -> (if (top value).ToLower() = "true" then pop instr; push instr (fwd ii)); pop value  //not perfect
      |Call -> push instr (string(int(top value) - 1)); pop value
      |Return -> pop instr
      |GetHeap -> let yld = heap.[int(top value)] in pop value; push value yld
      |NewHeap -> heap.Add ""; push value (string(heap.Count-1))
      |WriteHeap -> let v = top value in pop value; let i = top value in pop value; heap.[int i] <- v
      |Input _ -> push value (stdin.ReadLine())
      |OutputLine _ -> push output (top value); pop value
      |Combinator_2 c ->
        let a = top value in pop value
        let b = top value in pop value
        push value (c.Interpret a b)
    let debug = false
    push instr "0"
    while int(top instr) < Array.length cmds && List.length !stacks.[output] < 50 do
      let i = int(top instr)
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
    List.iter (printfn "%A") (List.rev !stacks.[output])
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
