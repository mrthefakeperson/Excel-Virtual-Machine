﻿module Compiler_Definitions
open System.Collections.Generic
open Excel_Language

type AST =
  |Sequence of AST list
  |Declare of a:string*b:AST      //let a = b
  |Define of a:string*args:string list*b:AST    //define a function
                                                //recursive functions should be differentiated by name changes (referencing a from within a calls a jump to a's address, creating the recursion)
  |Value of string
  |Const of string
  |Apply of a:AST*args:AST list              //a(b1, b2, ...)
  |If of a:AST*b:AST*c:AST
  |New of n:AST               //allocate n new heap spaces
  |Get of a:AST*i:AST           //get array a at i
  |Assign of a:AST*i:AST*e:AST  //set array a at i to e
  |Return of a:AST option
  |Loop of a:AST*b:AST
  |Mutate of a:string*b:AST     //a <- b
   with
    override x.ToString() =
      let rec str indent = function
        |Sequence ll -> List.map (str indent) ll |> String.concat "\n"
        |Declare(a, b) -> indent + sprintf "let %s =\n%s" a (str (indent+"  ") b)
        |Define(a, args, b) -> indent + sprintf "define %s(%s):\n%s" a (String.concat ", " args) (str (indent+"  ") b)
        |Value s | Const s -> indent + s
        |Apply(a, args) -> indent + sprintf "%s(%s)" (str "" a) (String.concat ", " (List.map (str "") args))
        |If(a, b, c) -> indent + sprintf "if %s then\n%s\n" (str "" a) (str (indent+"  ") b) + indent + "else\n" + str (indent+"  ") c
        |New n -> indent + sprintf "alloc %s" (str "" n)
        |Get(a, i) -> indent + sprintf "%s[%s]" (str "" a) (str "" i)
        |Assign(a, i, e) -> indent + sprintf "%s[%s] <- %s" (str "" a) (str "" i) (str "" e)
        |Return None -> indent + "return"
        |Return(Some x) -> indent + sprintf "return\n%s" (str (indent+"  ") x)
        |Loop(a, b) -> indent + sprintf "while %s do\n%s\n" (str "" a) (str (indent+"  ") b)
        |Mutate(a, b) -> indent + sprintf "%s <-\n%s" a (str (indent+"  ") b)
      str "" x

[<AbstractClass>]     //combinator class
type comb2(name, symbol) =
  member x.ToStrPair() = name, ""
  abstract member Interpret: string -> string -> string
  abstract member CreateFormula: Formula -> Formula -> Formula
  member x.Name = name
  member x.Symbol = symbol
  override x.ToString() = name

//heap is indexed from 0
//everything else (?) from 1
type PseudoAsm =
  |Push of string
  |PushFwdShift of int     //push the number equal to (the address of this instruction) + (int)
  |Pop
  |Store of string
  |Load of string
  |Popv of string
  |GotoFwdShift of int
  |GotoIfTrueFwdShift of int
  |Call
  |Return
  |NewHeap     //allocate a new spot in heap (update size, make sure to `WriteHeap (size) (value)` before)
  |GetHeap     //let i = topstack; pop stack; push heap value at i to stack
  |WriteHeap   //let v = topstack; pop stack; let i = topstack; pop stack; heap at i <- v
  |InputLine
  |OutputLine of System.Type
  |Combinator_2 of comb2
type C2_Add(name, symbol) =
  inherit comb2(name, symbol)
  override x.Interpret a b = string (int a + int b)
  override x.CreateFormula a b = a +. b
type C2_Equals(name, symbol) =
  inherit comb2(name, symbol)
  override x.Interpret a b = string (a = b)
  override x.CreateFormula a b = a =. b
type C2_LEq(name, symbol) =
  inherit comb2(name, symbol)
  override x.Interpret a b =
    if System.Int32.TryParse(a, ref 0) && System.Int32.TryParse(b, ref 0)
     then string(int a <= int b)
     else string(a <= b)
  override x.CreateFormula a b = a <=. b
type C2_Greater(name, symbol) =
  inherit comb2(name, symbol)
  override x.Interpret a b =
    if System.Int32.TryParse(a, ref 0) && System.Int32.TryParse(b, ref 0)
     then string(int a > int b)
     else string(a > b)
  override x.CreateFormula a b = a >. b
type C2_Mod(name, symbol) =
  inherit comb2(name, symbol)
  override x.Interpret a b = string(int a % int b)
  override x.CreateFormula a b = a %. b
let allCombinators =
  List.map Combinator_2 [
    C2_Add("add", "+"); C2_Equals("equals", "="); C2_LEq("leq", "<="); C2_Greater("greater", ">"); C2_Mod("mod", "%")
   ]
let [Add; Equals; LEq; Greater; Mod] = allCombinators

let cmdToStrPair i = function
  |Push e -> "push", e | PushFwdShift x -> "push", string(i + x) | Pop -> "pop", ""
  |Store e -> "store", e | Load e -> "load", string(alphaToNumber e - 5) | Popv e -> "popv", e
  |GotoFwdShift x -> "goto", string(i + x) | GotoIfTrueFwdShift x -> "gotoiftrue", string(i + x)
  |Call -> "call", "" | Return -> "return", ""
  |GetHeap -> "getheap", "" | NewHeap -> "newheap", "" | WriteHeap -> "writeheap", ""
  |InputLine -> "inputline", "" | OutputLine _ -> "outputline", ""
  |Combinator_2 c -> c.ToStrPair()
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
    |InputLine -> push value (stdin.ReadLine())
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