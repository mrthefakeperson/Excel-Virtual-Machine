//#load "../Excel VM/Project.fs"
#load "./unused modules/UnusedModule1.fsx"
#load "../Excel VM/Parser.Definition.fs"
#load "../Excel VM/Parser.Lexer.fs"
#load "../Excel VM/Parser.StringFormatting.fs"
#load "./unused modules/UnusedModule2.fsx"
#load "../Excel VM/Parser.CParser.fs"
#load "../Excel VM/Parser.TypeValidation.fs"
#load "../Excel VM/Parser.Implementation.fs"
#load "../Excel VM/AST.Definition.fs"
#load "../Excel VM/AST.Compile.fs"
#load "../Excel VM/AST.Optimize.fs"
#load "../Excel VM/AST.Implementation.fs"
#load "../Excel VM/PseudoASM.Definition.fs"
#load "../Excel VM/PseudoASM.Compile.fs"
#load "../Excel VM/PseudoASM.Implementation.fs"

module Interpreter =
  open System
  open Parser.Definition
  open AST.Definition
  open PseudoASM.Definition
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
  let interpretPAsm act getInput sendOutput cmds =
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
      |GotoIfTrueFwdShift ii -> (if (top value).ToLower() = "true" || top value = "1" then pop instr; push instr (fwd ii)); pop value  //not perfect
      |Call -> let newInstr = string(int(top value) - 1) in pop value; push value (string(int(top instr) + 1)); pop instr; push instr newInstr
      |Return -> pop instr; push instr (string(int(top value) - 1)); pop value
      |GetHeap -> let yld = heap.[int(top value)] in pop value; push value yld
      |NewHeap -> heap.Add ""; push value (string(heap.Count-1))
      |WriteHeap -> let v = top value in pop value; let i = top value in pop value; heap.[int i] <- v
      |Input fmt -> push value (getInput fmt)
      |Output _ -> sendOutput (top value); push output (top value); pop value
      |Combinator_2(_, _) as c ->
        let a = top value in pop value
        let b = top value in pop value
        let op = List.find (fun (e:Op) -> (c.CommandInfo :?> Comb2).Symbol = e.Symbol) allCombinators
        push value (op.Apply a b)
    push instr "0"
    while int(top instr) < Array.length cmds && List.length !stacks.[output] < 50 do
      let i = int(top instr)
      try
        interpretCmd i cmds.[i]
      with
      |ex ->
        printfn "%A" ex
        printfn "stack %A" !stacks.[value]
        printfn "heap [%s]" (String.concat "; " (Seq.map (sprintf "%A") heap))
        printfn "%A" cmds.[i]
        failwith "failed"
      act stacks heap cmds.[i]
      let pt = int(top instr)
      pop instr; push instr (string(pt+1))


let compileAndRun act getInput sendOutput txt =
  Parser.Implementation.fromString (txt, Map ["language", "C"])
   |> AST.Implementation.fromToken
   |> PseudoASM.Implementation.fromAST
   |> fst
   |> Array.ofSeq
   |> Interpreter.interpretPAsm act getInput sendOutput


//compileAndRun (ignore >> stdin.ReadLine) stdout.WriteLine "int main(){printf(\"%i\", 45);}"