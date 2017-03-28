//todo: investigate the one variable which doesn't get popped completely by program end
module ASM_Compiler
open Excel_Language.Definitions
open AST_Compiler

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
  |Input of string
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
let rec (|Inline|_|) = function
  |Value nfx when List.exists (function Combinator_2 e -> e.Symbol = nfx | _ -> false) allCombinators ->
    Some [
      NewHeap; Store x; Load x; Push (string(getSectionAddressFromInfix nfx)); WriteHeap  // store address
      NewHeap; Push "endArr"; WriteHeap; Load x; Popv x      // end the array
     ]
  |Value "ignore" -> Some [Push "not implemented"]
  |Apply(Value "printfn", [Const "%i"]) ->
    Some [NewHeap; Store x; Load x; Push (string (_PrintAddress type_int32)); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |Apply(Value "printfn", [Const "%s"])
  |Value "printfn" ->
    Some [NewHeap; Store x; Load x; Push (string (_PrintAddress type_string)); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
  |Apply(Value "scan", [Const s | Value s]) ->
    Some [NewHeap; Store x; Load x; Input s; WriteHeap; Load x; Popv x]
  |Value "nothing" -> Some [Push "nothing"]     // void; change this later so that assigning `nothing` does nothing
  |_ -> None
and compile' inScope = function
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
    a @ [GotoFwdShift(-(List.length a)); Push "()"]
  |Mutate(a, b) -> compile' inScope b @ [Popv a; Store a; Push "()"]
let compile e = [GotoFwdShift (List.length operationsPrefix + 1)] @ operationsPrefix @ compile' [] e