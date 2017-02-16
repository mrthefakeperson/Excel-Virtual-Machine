module Compiler
open Token
open System.Collections.Generic
open Compiler_Definitions

let nxt' x () =
  incr x
  string !x
let nxt = nxt' (ref 0)
let (|X|) (t:Token) = X(t.Name, t.Dependants)
let (|Inner|_|) c = function
  |T "\"\"" -> Some ""
  |T s when s.Length >= 2 && s.[0] = c && s.[s.Length-1] = c -> Some s.[1..s.Length-2]
  |_ -> None
let (|Var|Cnst|Other|) = function               //todo: non-numeric constants
  |Inner '"' s -> Cnst s        //cases like "\" should not have made it through the lexer
  |Inner ''' s -> Cnst s        //cases like 'dd' are the lexer's job
  |T "()" -> Cnst "()"
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
  
let x = "F"    //just a place to store values
let createComb2Section cmd = [
  cmd
  Return
  Store x;   //cmd: 2 + number of instructions before
    NewHeap; Store x; Load x; PushFwdShift -6; WriteHeap; Load x; Popv x;
    NewHeap; Load x; WriteHeap; Popv x; NewHeap; Push "endArr"; WriteHeap
  Return
 ]
let allComb2Sections = List.collect createComb2Section allCombinators
let getSectionAddress i = 18 * i + 3
let getSectionAddressFromCmd cmd = List.findIndex ((=) cmd) allCombinators |> getSectionAddress
let getSectionAddressFromInfix =
  (function "+" -> Add | "=" -> Equals | "<=" -> LEq | "%" -> Mod | _ -> failwith "???")
   >> getSectionAddressFromCmd
let rec operationsPrefix =
  allComb2Sections
   @ [OutputLine; Push "()"; Return]
let _PrintAddress = List.length allComb2Sections + 1
let (|Inline|_|) = function
  |Value ("+" | "=" | "<=" | "%" as nfx) ->
    Some [
      NewHeap; Store x; Load x; Push (string(getSectionAddressFromInfix nfx)); WriteHeap  // store address
      NewHeap; Push "endArr"; WriteHeap; Load x; Popv x      // end the array
     ]
  |Value "ignore" -> Some [Push "not implemented"]
  |Apply(Value "printfn", [Const "%A"]) -> Some [NewHeap; Store x; Load x; Push (string _PrintAddress); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
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