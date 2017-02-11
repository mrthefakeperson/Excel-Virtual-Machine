module Compiler
open Token
open System.Collections.Generic

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

let nxt' x () =
  incr x
  string !x
let nxt = nxt' (ref 0)
let (|X|) (t:Token) = X(t.Name, t.Dependants)
let (|Var|Cnst|Other|) = function               //todo: non-numeric constants (eg. chars)
  |T ("true" | "false" as s) -> Cnst s
  |T s -> if s <> "" && '0' <= s.[0] && s.[0] <= '9' then Cnst s else Var s
  |_ -> Other
let rec ASTCompile' (capture, captured as cpt) = function
  |Var s -> if Map.containsKey s captured && captured.[s] <> [] then Apply(Value s, captured.[s]) else Value s   //variables
  |Cnst s -> Const s    //constants, could use some work
  |X("apply", [a; b]) -> Apply(ASTCompile' cpt a, [ASTCompile' cpt b])
  |X("fun", [x; b]) ->
    let unpacked = match x with T s -> s | _ -> failwith "argument unpacking not done"
    let cptr'd = List.map Value capture
    let cpt' = unpacked::capture, Map.add "L" cptr'd captured
    Sequence [
      yield Define("L", unpacked::capture, ASTCompile' cpt' b)
      match ASTCompile' cpt' (Token("L", [])) with
      |Apply(a, args) ->
        yield Declare("K", New (Const(string(List.length args + 1))))
        yield Assign(Value "K", Const "0", Get(a, Const "0"))
        yield! List.mapi (fun i e ->
          Assign(Value "K", Const(string(i + 1)), e)
         ) args
        yield Value "K"
      |compiled -> yield compiled
     ]
  |X("let", [a; b]) ->
    match a with
    |X("apply", [aa; ab]) -> ASTCompile' cpt (Token("let", [aa; Token("fun", [ab; b])]))
    |T s ->
      match capture with
      |[] -> Declare(s, ASTCompile' cpt b)
      |ll -> Define(s, capture, ASTCompile' (capture, Map.add s (List.map Value capture) captured) b)
    |_ -> failwith "patterns in function arguments not supported yet"
  |X("if", [cond; aff; neg]) ->
    If(ASTCompile' cpt cond, ASTCompile' cpt aff, ASTCompile' cpt neg)
  |X("do", [b]) -> Apply(Value "ignore", [ASTCompile' cpt b])
  |X("while", [cond; b]) ->
    let loop = "$loop" + nxt()
    Sequence [
      Define(loop, ["()"],
        If(ASTCompile' cpt cond,
          Sequence [ASTCompile' cpt b; Apply(Value loop, [Const "()"])],
          Const "()"     //Apply(Value "ignore", [Const "()"])
         ) )
      Apply(Value loop, [Const "()"])
     ]
  |X("for", [name; iterable; body]) ->
    let loop = "$loop" + nxt()
    //todo: flatten name pattern, change call to loop function accordingly
    let name = match name with X(name, []) -> name | _ -> failwith "patterns not supported yet"
    match iterable with
    |X("..", [a; step; b]) ->
      Sequence [
        Define(loop, [name],
          If(Apply(Apply(Value "<=", [Value name]), [ASTCompile' cpt b]),          //todo: negative step values
            Sequence [ASTCompile' cpt body; Apply(Value loop, [Apply(Apply(Value "+", [Value name]), [ASTCompile' cpt step])])],
            Const "()"
           ) )
        Apply(Value loop, [ASTCompile' cpt a])
       ]
    |_ -> failwith "iterable objects not supported yet"
  |X("sequence", list) -> //Sequence (List.map (ASTCompile' capture) list)
    List.fold (fun (acc, (capt', capd' as cpt')) e ->
      let compiled = ASTCompile' cpt' e
      let cpt' =
        match compiled with
        |Declare(a, _) | Define(a, _, _) -> (a::capt', Map.add a (List.map Value capt') capd')
        |_ -> cpt'
      (compiled::acc, cpt')
     ) ([], cpt) list
      |> fst |> List.rev
      |> Sequence
  
  |unknown -> failwithf "unknown: %A" unknown
let ASTCompile e = ASTCompile' ([], Map.empty) e

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
  //arithmetic
  |Add
  |Equals
  |Greater
  |LEq
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
    |Add -> let a = top value in pop value; let b = top value in pop value; push value (string(int a + int b))
    |Equals -> let a = top value in pop value; let b = top value in pop value; push value (string(a = b))
    |Greater -> failwith "do later"//let a = top value in pop value; let b = top value in pop value; push value (string(a > b))
    |LEq ->
      let a = top value in pop value; let b = top value in pop value
      if System.Int32.TryParse(a, ref 0) && System.Int32.TryParse(b, ref 0)
       then push value (string(int a <= int b))
       else push value (string(a <= b))

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
  
let x = "F"    //just a place to store values
let rec operationsPrefix = [
  Add
  Return
  Store x;   //(+): 3   (one instruction goes before all of this)
    NewHeap; Store x; Load x; PushFwdShift -6; WriteHeap; Load x; Popv x;
    NewHeap; Load x; WriteHeap; Popv x; NewHeap; Push "endArr"; WriteHeap
  Return

  OutputLine    //print: 19
  Push "()"
  Return

  Equals
  Return
  Store x;        //(=): 24
    NewHeap; Store x; Load x; PushFwdShift -6; WriteHeap; Load x; Popv x;
    NewHeap; Load x; WriteHeap; Popv x; NewHeap; Push "endArr"; WriteHeap
  Return

  LEq
  Return
  Store x;        //(<=): 42
    NewHeap; Store x; Load x; PushFwdShift -6; WriteHeap; Load x; Popv x;
    NewHeap; Load x; WriteHeap; Popv x; NewHeap; Push "endArr"; WriteHeap
  Return
 ]
let (|Inline|_|) = function
  |Value "+" -> Some [NewHeap; Store x; Load x; Push "3"; WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |Value "=" -> Some [NewHeap; Store x; Load x; Push "24"; WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |Value "<=" -> Some [NewHeap; Store x; Load x; Push "42"; WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |Value "ignore" -> Some [Push "not implemented"]
  |Apply(Value "printfn", [Value "\"%A\""]) -> Some [NewHeap; Store x; Load x; Push "19"; WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |_ -> None
let rec compile' = function
  |Inline ll -> ll
  |Sequence ll ->               //make sure they all return a value (or this will screw up stuff)
    match List.collect (fun e -> Pop :: List.rev (compile' e)) (List.rev ll) with
    |_::revCmds -> List.rev revCmds
    |[] -> failwith "this should never happen unless the sequence was empty"
  |Declare(a, b) -> compile' b @ [Store a; Push "()"]
  |Define(a, args, b) ->       //all function values are arrays: [|&f; arg1; arg2; ... |]
    let functionBody =
      List.map Store (List.rev args) @ compile' b @ List.map Popv args @ [Return]   //double check `List.rev args`
    let len = List.length functionBody
    [GotoFwdShift (len + 1)] @ functionBody
     @ [NewHeap; Store a; Load a; PushFwdShift (-len - 3); WriteHeap; Load a]
     @ [NewHeap; Push "endArr"; WriteHeap]
  |Value a -> [Load a]
  |Const v -> [Push v]
  |Apply(a, args) ->     //maybe put in prefix
    let functionArray = compile' a @ [Store x; Load x]
    let loop = [   //start with address of array (representing function value)
      Push "1"; Add; Store x   //x <- f[1] (arg 1)
      Load x; GetHeap; Push "endArr"; Equals; GotoIfTrueFwdShift 9; //while not (f[x] = terminator)
        Load x; GetHeap;       //push the currently indexed value onto the stack
        Load x; Push "1"; Add; Popv x; Store x;
      GotoFwdShift -12
      Popv x
     ]
    let call = [Load x; GetHeap; Call; Popv x]
    List.collect compile' args @ functionArray @ loop @ call
  |If(a, b, c) ->
    let cond, aff, neg = compile' a, compile' b, compile' c
    cond
     @ [GotoIfTrueFwdShift (List.length neg + 2)] @ neg
     @ [GotoFwdShift (List.length aff + 1)] @ aff
  |New n ->              //maybe put in prefix
    let loop = [
      Store x    //any variable name which exists
      NewHeap    //store the first spot allocated
      Load x; Push "1"; Equals; GotoIfTrueFwdShift 9;   //while not (x = 1) ...
        NewHeap; Pop;
        Load x; Push "-1"; Add; Popv x; Store x   //x <- x - 1
      GotoFwdShift -11
      Popv x
      NewHeap; Push "endArr"; WriteHeap    //put a terminator at the end
     ]
    compile' n @ loop
  |Get(a, i) -> compile' a @ compile' i @ [Add; GetHeap]
  |Assign(a, i, e) -> compile' a @ compile' i @ [Add] @ compile' e @ [WriteHeap; Push "()"]
let compile e = [GotoFwdShift (List.length operationsPrefix + 1)] @ operationsPrefix @ compile' e