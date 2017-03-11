//todo: variables leaving scope should be popped
module Compiler
open Token
open System.Collections.Generic
open Compiler_Definitions

let nxt' x () =
  incr x
  string !x
let nxt = nxt' (ref 0)
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
  |X("return", xl) ->
    match xl with
    |[] -> AST.Return None
    |[x] -> AST.Return (Some (ASTCompile' cpt x))
    |_ -> failwith "cannot return more than one item"
  |Var s -> if Map.containsKey s captured && captured.[s] <> [] then Apply(Value s, captured.[s]) else Value s   //variables
  |Cnst s -> Const s    //constants, could use some work
  |X(",", tupled) ->
    let name = "$tuple" + nxt()
    let allocate = [Declare(name, New(Const(string(List.length tupled))))]
    let assignAll = List.mapi (fun i e -> Assign(Value name, Const(string i), ASTCompile' cpt e)) tupled
    let returnVal = [Value name]
    Sequence (allocate @ assignAll @ returnVal)
  |X("apply", [a; b]) -> Apply(ASTCompile' cpt a, [ASTCompile' cpt b])
  |X("fun", [x; b]) ->
    let rec unpack arg = function
      |X("declare", [_; T s])
      |T s -> [Declare(s, arg)], [s]
      |X(",", xprs) ->
        let compiled, extractedVars =
          List.mapi (fun i -> unpack (Get(arg, Const(string i)))) xprs
           |> List.unzip
        List.concat compiled, List.concat extractedVars
      |e -> failwithf "could not unpack %A" e
    let argName = "$arg" + nxt()
    let extractCode, extractedVars = unpack (Value argName) x
    let cptr'd = List.map Value capture
    let cpt' = extractedVars @ capture, Map.add "L" cptr'd captured
    let functionBody = Sequence (extractCode @ [ASTCompile' cpt' b])
    Sequence [
      yield Define("L", argName::capture, functionBody)
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
  |X("declare", [datatypeName; a]) -> ASTCompile' cpt a
  |X("let", [a; b]) ->           //todo: handle non-recursive statements by renaming
    match a with
    |X("apply", [aa; ab]) -> ASTCompile' cpt (Token("let", [aa; Token("fun", [ab; b])]))
    |T s | X("declare", [_; T s]) ->
      Declare(s, ASTCompile' cpt b)
    |e -> failwithf "patterns in function arguments not supported yet: %A" e
  |X("let rec", [a; b]) -> ASTCompile' cpt (Token("let", [a; b]))
  |X("array", [sz]) -> New (ASTCompile' cpt sz)
  |X("dot", [a; b]) ->
    match b with
    |X("[]", [i]) -> Get(ASTCompile' cpt a, ASTCompile' cpt i)
    |_ -> failwith "todo: objects"
  |X("assign", [a; b]) ->
    match a with
    |X(name, []) -> Mutate(name, ASTCompile' cpt b)
    |X("dot", [a; X("[]", [i])]) ->
      Assign(ASTCompile' cpt a, ASTCompile' cpt i, ASTCompile' cpt b)
    |_ -> failwith "todo: unpacking"
  |X("if", [cond; aff; neg]) ->
    If(ASTCompile' cpt cond, ASTCompile' cpt aff, ASTCompile' cpt neg)
  |X("do", [b]) -> Apply(Value "ignore", [ASTCompile' cpt b])
  |X("while", [cond; b]) -> Loop(ASTCompile' cpt cond, ASTCompile' cpt b)
  |X("for", [name; iterable; body]) ->
    let name = match name with X(name, []) -> name | _ -> failwith "todo: unpacking"
    match iterable with
    |X("..", [a; step; b]) ->
      Sequence [    //todo: negative step values
        Declare(name, ASTCompile' cpt a)      //no need to capture, since no part of the code can touch it
        Loop(Apply(Apply(Value "<=", [Value name]), [ASTCompile' cpt b]),
          Sequence
           [ASTCompile' cpt body; Mutate(name, Apply(Apply(Value "+", [Value name]), [ASTCompile' cpt step]))]
         )
       ]
    |_ -> failwith "iterable objects not supported yet"
//    let loop = "$loop" + nxt()
//    //todo: flatten name pattern, change call to loop function accordingly
//    let name = match name with X(name, []) -> name | _ -> failwith "patterns not supported yet"
//    match iterable with
//    |X("..", [a; step; b]) ->
//      Sequence [
//        Define(loop, [name],
//          If(Apply(Apply(Value "<=", [Value name]), [ASTCompile' cpt b]),          //todo: negative step values
//            Sequence [ASTCompile' cpt body; Apply(Value loop, [Apply(Apply(Value "+", [Value name]), [ASTCompile' cpt step])])],
//            Const "()"
//           ) )
//        Apply(Value loop, [ASTCompile' cpt a])
//       ]
//    |_ -> failwith "iterable objects not supported yet"
  |X("sequence", list) -> //Sequence (List.map (ASTCompile' capture) list)
    List.fold (fun (acc, (capt', capd' as cpt')) e ->
      let compiled = ASTCompile' cpt' e
      let cpt' =
        match compiled with
        |Declare(a, _) -> (a::capt', capd')
        |Define(a, _, _) -> (a::capt', Map.add a (List.map Value capt') capd')
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
let getSectionAddressFromInfix nfx =
  List.find (function Combinator_2 e -> e.Symbol = nfx | _ -> false) allCombinators
   |> getSectionAddressFromCmd
let allTypes = [System.Type.GetType "System.Int32"; System.Type.GetType "System.String"]
let [type_int32; type_string] = allTypes
let rec operationsPrefix =
  allComb2Sections
   @ List.collect (fun e -> [OutputLine e; Push "()"; Return]) allTypes
let _PrintAddress t = //List.length allComb2Sections + 1
  match List.tryFindIndex ((=) t) allTypes with
  |Some i -> List.length allComb2Sections + 1 + 3 * i
  |None -> failwithf "can't output type: %A" t
let (|Inline|_|) = function
  |Value nfx when List.exists (function Combinator_2 e -> e.Symbol = nfx | _ -> false) allCombinators ->
    Some [
      NewHeap; Store x; Load x; Push (string(getSectionAddressFromInfix nfx)); WriteHeap  // store address
      NewHeap; Push "endArr"; WriteHeap; Load x; Popv x      // end the array
     ]
  |Value "ignore" -> Some [Push "not implemented"]
//  |Apply(Value "printfn", [Const "%A"]) -> Some [NewHeap; Store x; Load x; Push (string _PrintAddress); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |Apply(Value "printfn", [Const "%i"]) ->
    Some [NewHeap; Store x; Load x; Push (string (_PrintAddress type_int32)); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |Apply(Value "printfn", [Const "%s"])
  |Value "printfn" ->
    Some [NewHeap; Store x; Load x; Push (string (_PrintAddress type_string)); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |_ -> None
let rec compile' inScope = function
  |Inline ll -> ll
  |Sequence ll ->
    let x = (List.collect (fun e -> Pop :: List.rev (compile' inScope e)) (List.rev ll)).Tail
    List.fold (fun (acc, scope) e ->
      let acc' = Pop :: List.rev (compile' (scope @ inScope) e) @ acc //assuming no Returns within Declares
      match e with
      |Declare(a, _) -> (acc', a::scope)
      |_ -> (acc', scope)
     ) ([], []) ll
     |> function
          |(Pop::revCmds, scopedVars) ->
            //printfn "\n%A\n%A" revCmds x
            //assert (revCmds = x)
            List.rev revCmds @ List.map Popv scopedVars
          |_ -> failwith "this should never happen unless the sequence was empty"
  |Declare(a, b) -> compile' (a::inScope) b @ [Store a; Push "()"]
  |Define(a, args, b) ->       //all function values are arrays: [|&f; arg1; arg2; ... |]
    let functionBody =
      List.map Store (List.rev args) @ compile' args b @ List.map Popv args @ [Return]
    let len = List.length functionBody
    [GotoFwdShift (len + 1)] @ functionBody
     @ [NewHeap; Store a; Load a; PushFwdShift (-len - 3); WriteHeap; Load a]
     @ [NewHeap; Push "endArr"; WriteHeap]
  |Value a -> [Load a]
  |Const v -> [Push v]
  |Apply(a, args) ->     //maybe put in prefix
    let functionArray = compile' inScope a @ [Store x; Load x]
    let loop = [   //start with address of array (representing function value)
      Push "1"; Add; Store x   //x <- f[1] (arg 1)
      Load x; GetHeap; Push "endArr"; Equals; GotoIfTrueFwdShift 9; //while not (f[x] = terminator)
        Load x; GetHeap;       //push the currently indexed value onto the stack
        Load x; Push "1"; Add; Popv x; Store x;
      GotoFwdShift -12
      Popv x
     ]
    let call = [Load x; GetHeap; Call; Popv x]
    List.collect (compile' inScope) args @ functionArray @ loop @ call
  |If(a, b, c) ->
    let cond, aff, neg = compile' inScope a, compile' inScope b, compile' inScope c
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
    compile' inScope n @ loop
  |Get(a, i) -> compile' inScope a @ compile' inScope i @ [Add; GetHeap]
  |Assign(a, i, e) ->
    compile' inScope a @ compile' inScope i @ [Add] @ compile' inScope e @ [WriteHeap; Push "()"]
  |AST.Return a ->
    let exitScope = List.map Popv inScope @ [Return]
    match a with
    |None -> exitScope
    |Some x -> compile' inScope x @ exitScope
  |Loop(a, b) ->
    let cond, body = compile' inScope a, compile' inScope b
    let a = cond @ [GotoIfTrueFwdShift 2; GotoFwdShift(List.length body + 3)] @ body @ [Pop]
    //let a = cond @ [Push "False"; Equals; GotoIfTrueFwdShift(List.length body + 3)] @ body @ [Pop]
    a @ [GotoFwdShift(-(List.length a)); Push "()"]
  |Mutate(a, b) -> compile' inScope b @ [Popv a; Store a; Push "()"]
let compile e = [GotoFwdShift (List.length operationsPrefix + 1)] @ operationsPrefix @ compile' [] e