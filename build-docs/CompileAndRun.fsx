#load "../Excel VM/Token.fs"
#load "../Excel VM/Lexer.fs"
#load "../Excel VM/String Formatting.fs"
#load "../Excel VM/Parser_C.fs"
#load "../Excel VM/Type System.fs"
#load "../Excel VM/AST Compiler.fs"
#load "./Excel Language.fs"     // gutted this because it takes too long to load
#load "../Excel VM/ASM Compiler.fs"

module Interpreter =
  open Parser
  open AST_Compiler
  open ASM_Compiler
  let interpretPAsm cmds (stdInput:string) =
    let pushstack stack v = stack := v :: !stack
    let popstack stack = stack := match !stack with _ :: tl -> tl | [] -> []
    let topstack stack = match !stack with hd :: _ -> hd | [] -> failwith "took top of an empty stack"
    let stacks =
      let x =
        Array.append (Array.map string [|'A'..'E'|])
         <| Array.choose (function Store e | Load e | Popv e -> Some e | _ -> None) cmds
         |> Set.ofArray |> Set.toArray
      (x, Array.init x.Length (fun _ -> ref []))
       ||> Array.zip
       |> dict
    let push name = pushstack stacks.[name]
    let pop name = popstack stacks.[name]
    let top name = topstack stacks.[name]
    let instr, value, _, input, output = "A", "B", "C", "D", "E"
    let heap = ResizeArray()
    Array.iter (push input) (Array.rev (stdInput.Split ' '))
    printfn "input: %A" !stacks.[input]
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
      |Input _ -> push value (top input); pop input
      |Output _ -> push output (top value); pop value
      |Combinator_2 c ->
        let a = top value in pop value
        let b = top value in pop value
        push value (c.Interpret a b)
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
      let pt = int(top instr)
      pop instr; push instr (string(pt+1))
    printfn "%A" (!stacks.[value], heap, List.rev !stacks.[output])
    String.concat "" (List.rev !stacks.[output]) //|> printfn "%s"

let compileAndRun =
  Parser.C.parseSyntax
   >> fun (e:Parser.Token.Token) -> e.Clean()
   >> Type_System.applyTypeSystem
   >> AST_Compiler.ASTCompile
   >> ASM_Compiler.compile
   >> Array.ofList
   >> Interpreter.interpretPAsm