module Compiler_Definitions
open System.Collections.Generic
open Excel_Language

type AST =
  |Sequence of AST list
  |Declare of a:string*b:AST      //let a = b
  |Define of a:string*args:string list*b:AST    //define a function
                                                //recursive functions should be distinctified by name changes (referencing a from within a calls a jump to a's address, creating the recursion)
  |Value of string
  |Const of string
  |Apply of a:AST*args:AST list              //a(b1, b2, ...)
  |If of a:AST*b:AST*c:AST
  |New of n:AST               //allocate n new heap spaces
  |Get of a:AST*i:AST           //get array a at i
  |Assign of a:AST*i:AST*e:AST  //set array a at i to e
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
      str "" x

[<AbstractClass>]     //combinator class
type comb2(name) =
  member x.ToStrPair() = name, ""
  abstract member Interpret: string -> string -> string
  abstract member CreateFormula: Formula -> Formula -> Formula
  member x.Name = name

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
  |GetHeap     //let i = topstack; pop stack; push heap value at i to stack
  |NewHeap     //allocate a new spot in heap (update size, make sure to `WriteHeap (size) (value)` before)
  |WriteHeap   //let v = topstack; pop stack; let i = topstack; pop stack; heap at i <- v
  |InputLine
  |OutputLine
  |Combinator_2 of comb2
  //arithmetic (consider making this a type)
//  |Add
//  |Equals
//  |Greater
//  |LEq
type C2_Add(name) =
  inherit comb2(name)
  override x.Interpret a b = string (int a + int b)
  override x.CreateFormula a b = a +. b
let Add = Combinator_2 (C2_Add("add"))
type C2_Equals(name) =
  inherit comb2(name)
  override x.Interpret a b = string (a = b)
  override x.CreateFormula a b = a =. b
let Equals = Combinator_2 (C2_Equals("equals"))
type C2_LEq(name) =
  inherit comb2(name)
  override x.Interpret a b =
    if System.Int32.TryParse(a, ref 0) && System.Int32.TryParse(b, ref 0)
     then string(int a <= int b)
     else string(a <= b)
  override x.CreateFormula a b = a <=. b
let LEq = Combinator_2 (C2_LEq("leq"))
type C2_Greater(name) =
  inherit comb2(name)
  override x.Interpret a b = failwith "not done yet"
  override x.CreateFormula a b = failwith "not done yet"
let Greater = Combinator_2 (C2_Greater("greater"))
let allCombinators = [Add; Equals; LEq]

let cmdToStrPair (mapping: IDictionary<string, string>) i = function
  |Push e -> "push", e | PushFwdShift x -> "push", string(i + x) | Pop -> "pop", ""
  |Store e -> "store", mapping.[e] | Load e -> "load", string(alphaToNumber mapping.[e] - 5) | Popv e -> "popv", mapping.[e]
  |GotoFwdShift x -> "goto", string(i + x) | GotoIfTrueFwdShift x -> "gotoiftrue", string(i + x)
  |Call -> "call", "" | Return -> "return", ""
  |GetHeap -> "getheap", "" | NewHeap -> "newheap", "" | WriteHeap -> "writeheap", ""
  |InputLine -> "inputline", "" | OutputLine -> "outputline", ""
  |Combinator_2 c -> c.ToStrPair()
//  |Add -> "add", "" | Equals -> "equals", "" | Greater -> "greater", "" | LEq -> "leq", ""
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
    |OutputLine -> push output (top value); pop value
    |Combinator_2 c ->
      let a = top value in pop value
      let b = top value in pop value
      push value (c.Interpret a b)
(*
    |Add -> let a = top value in pop value; let b = top value in pop value; push value (string(int a + int b))
    |Equals -> let a = top value in pop value; let b = top value in pop value; push value (string(a = b))
    |Greater -> failwith "do later"//let a = top value in pop value; let b = top value in pop value; push value (string(a > b))
    |LEq ->
      let a = top value in pop value; let b = top value in pop value
      if System.Int32.TryParse(a, ref 0) && System.Int32.TryParse(b, ref 0)
       then push value (string(int a <= int b))
       else push value (string(a <= b))
*)
  let debug = false
  push instr "0"
  while int(top instr) < Array.length cmds do
    let i = int(top instr)
    if debug then printfn "%A" stacks.["e"]
    if debug then printfn "%A" cmds.[i]
    interpretCmd i cmds.[i]
    if debug then printfn "stack %A" !stacks.[value]
    if debug then printfn "heap [%s]" (String.concat "; " (Seq.map (sprintf "%A") heap))
    let pt = int(top instr)
    pop instr; push instr (string(pt+1))
    if debug then ignore (stdin.ReadLine())
  !stacks.[value], heap