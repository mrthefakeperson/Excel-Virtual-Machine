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
  |Output of string
  |Combinator_2 of comb2
                     // todo: try object expressions here instead
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
    if fst (System.Int32.TryParse a) && fst (System.Int32.TryParse b)
     then string(int a <= int b)
     else string(a <= b)
  override x.CreateFormula a b = a <=. b
type C2_Greater(name, symbol) =
  inherit comb2(name, symbol)
  override x.Interpret a b =
    if fst (System.Int32.TryParse a) && fst (System.Int32.TryParse b)
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
//   will this have consequences?
//let allTypes = [System.Type.GetType "System.Int32"; System.Type.GetType "System.String"]
let allTypes = ["%i"; "%s"]
let [type_int32; type_string] = allTypes
let rec operationsPrefix =
  allComb2Sections
   @ List.collect (fun e -> [Output e; Push "()"; Return]) allTypes
let _PrintAddress t =
  match List.tryFindIndex ((=) t) allTypes with
  |Some i -> List.length allComb2Sections + 1 + 3 * i
  |None -> failwithf "can't output type: %A" t
//let rec (|Inline|_|) = function
//  |Value nfx when List.exists (function Combinator_2 e -> e.Symbol = nfx | _ -> false) allCombinators ->
//    Some [
//      NewHeap; Store x; Load x; Push (string(getSectionAddressFromInfix nfx)); WriteHeap  // store address
//      NewHeap; Push "endArr"; WriteHeap; Load x; Popv x      // end the array
//     ]
//  |Value "ignore" -> Some [Push "not implemented"]
//  |Apply(Value "printf", [Const "%i"]) ->
//    Some [NewHeap; Store x; Load x; Push (string (_PrintAddress type_int32)); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
//  |Apply(Value "printf", [Const "%s"])
//  |Value "printf" ->
//    Some [NewHeap; Store x; Load x; Push (string (_PrintAddress type_string)); WriteHeap; NewHeap; Push "endArr"; WriteHeap; Load x; Popv x]
//  |Apply(Value "scan", [Const s | Value s]) -> Some [Input s]
//  |Value "nothing" -> Some [Push "nothing"]     // void; change this later so that assigning `nothing` does nothing
//  |_ -> None
//and compile' inScope = function
//  |Inline ll -> ll
//  |Sequence ll ->
//    let x = (List.collect (fun e -> Pop :: List.rev (compile' inScope e)) (List.rev ll)).Tail
//    List.fold (fun (acc, scope) e ->
//      let acc' = Pop :: List.rev (compile' (scope @ inScope) e) @ acc //assuming no Returns within Declares
//      match e with
//      |Declare(a, _) -> (acc', a::scope)
//      |_ -> (acc', scope)
//     ) ([], []) ll
//     |> function
//          |(Pop::revCmds, scopedVars) ->
//            //printfn "\n%A\n%A" revCmds x
//            //assert (revCmds = x)
//            List.rev revCmds @ List.map Popv scopedVars
//          |_ -> failwith "this should never happen unless the sequence was empty"
//  |Declare(a, b) -> compile' (a::inScope) b @ [Store a; Push "()"]
//  |Define(a, args, b) ->       //all function values are arrays: [|&f; arg1; arg2; ... |]
//    let functionBody =
//      List.map Store (List.rev args) @ compile' args b @ List.map Popv args @ [Return]
//    let len = List.length functionBody
//    [GotoFwdShift (len + 1)] @ functionBody
//     @ [NewHeap; Store a; Load a; PushFwdShift (-len - 3); WriteHeap; Load a]
//     @ [NewHeap; Push "endArr"; WriteHeap]
//  |Value a -> [Load a]
//  |Const v -> [Push v]
//  |Apply(a, args) ->     //maybe put in prefix
//    let functionArray = compile' inScope a @ [Store x; Load x]
//    let loop = [   //start with address of array (representing function value)
//      Push "1"; Add; Store x   //x <- f[1] (arg 1)
//      Load x; GetHeap; Push "endArr"; Equals; GotoIfTrueFwdShift 9; //while not (f[x] = terminator)
//        Load x; GetHeap;       //push the currently indexed value onto the stack
//        Load x; Push "1"; Add; Popv x; Store x;
//      GotoFwdShift -12
//      Popv x
//     ]
//    let call = [Load x; GetHeap; Call; Popv x]
//    List.collect (compile' inScope) args @ functionArray @ loop @ call
//  |If(a, b, c) ->
//    let cond, aff, neg = compile' inScope a, compile' inScope b, compile' inScope c
//    cond
//     @ [GotoIfTrueFwdShift (List.length neg + 2)] @ neg
//     @ [GotoFwdShift (List.length aff + 1)] @ aff
//  |New n ->              //maybe put in prefix
//    let loop = [
//      Store x    //any variable name which exists
//      NewHeap    //store the first spot allocated
//      Load x; Push "1"; Equals; GotoIfTrueFwdShift 9;   //while not (x = 1) ...
//        NewHeap; Pop;
//        Load x; Push "-1"; Add; Popv x; Store x   //x <- x - 1
//      GotoFwdShift -11
//      Popv x
//      NewHeap; Push "endArr"; WriteHeap    //put a terminator at the end
//     ]
//    compile' inScope n @ loop
//  |Get(a, i) -> compile' inScope a @ compile' inScope i @ [Add; GetHeap]
//  |Assign(a, i, e) ->
//    compile' inScope a @ compile' inScope i @ [Add] @ compile' inScope e @ [WriteHeap; Push "()"]
//  |AST.Return a ->
//    let exitScope = List.map Popv inScope @ [Return]
//    match a with
//    |None -> exitScope
//    |Some x -> compile' inScope x @ exitScope
//  |Loop(a, b) ->
//    let cond, body = compile' inScope a, compile' inScope b
//    let a = cond @ [GotoIfTrueFwdShift 2; GotoFwdShift(List.length body + 3)] @ body @ [Pop]
//    a @ [GotoFwdShift(-(List.length a)); Push "()"]
//  |Mutate(a, b) -> compile' inScope b @ [Popv a; Store a; Push "()"]
//let compile e = [GotoFwdShift (List.length operationsPrefix + 1)] @ operationsPrefix @ compile' [] e


let stTemp, ldTemp, stTemp2, ldTemp2 = Store "!temp", Load "!temp", Store "!temp2", Load "!temp2"
let pushAgain = [stTemp; ldTemp; ldTemp]
let getLocalVariables = function
  |Define(start, _, _) as e ->
    let yld = ref []
    let rec getLocalVariables = function
      |Define(s, locals, children) when s = start ->
        yld := locals @ !yld
        getLocalVariables children
      |Define(_, _, _) -> ()
      |Declare(local, children) | Mutate(local, children) ->
        yld := local :: !yld
        getLocalVariables children
      |Children children -> List.iter getLocalVariables children
    getLocalVariables e
    Seq.toList (System.Collections.Generic.HashSet !yld)
  |_ -> failwith "local variables are for functions only"
// new assembly compilation function, with a single stack
// todo: change to list comprehension with yield!s for better performance
// todo: all names should be unique 
let rec compileASM redef =
  let inline compileASM' x = compileASM redef x
  let (|RedefDetected|_|) construct = redef construct
  function
  |RedefDetected yld -> yld
  |Apply(a, b) ->   // a is an array, a.[0]: function, a.[1..]: suffix of args
    let pushSuffixArgs =   // x :: ... -> (args in reverse order) @ ...
      let loopBody =  // x :: ... -> x + 1 :: *x :: ...
        [stTemp; ldTemp; GetHeap; ldTemp; Push "1"; Add]
      let loopCond =  // x :: ... -> (*x = terminator)? :: x :: ...
        pushAgain @ [GetHeap; Push "endArr"; Equals]
      let skipBody = List.length loopBody + 2
      let loopAgain = -(List.length loopCond + 1 + List.length loopBody)
      [Push "1"; Add] @ loopCond @ [GotoIfTrueFwdShift skipBody] @ loopBody @ [GotoFwdShift loopAgain; Pop]
    List.collect compileASM' b @ compileASM' a @ [stTemp2; ldTemp2] @ pushSuffixArgs @ [ldTemp2; GetHeap; Call]
  |Assign(a, i, e) -> compileASM' a @ compileASM' i @ [Add] @ compileASM' e @ [WriteHeap; Push "()"]
  |Const c -> [Push c]
  |Declare(a, b) -> compileASM' b @ [Store a; Push "()"]
  |Define(f, xs, b) as ast ->   // returns an array address, since functions are aliases for arrays
    // on stack: calling address :: args in reverse order @ ...
    let storeCallingAddress, loadCallingAddress = [Store "*call_addr"], [Load "*call_addr"]
    let getArgumentsFromStack = List.map Store (List.rev xs)
    let storeReturnValue, loadReturnValue = [Store "!yield"], [Load "!yield"]
    // save arguments and local variables (todo: only do this if the function is recursive)
    let xs = getLocalVariables ast
    let getAllLocalsFromStack = List.map Store (List.rev xs)
    let storeArgumentsLocally =
      List.mapi (fun i e -> [Load e; Store (string i + "-arg")]) xs
       |> List.concat
    let storeArgumentsOnStackFromLocal =
      List.map (fun i -> Load (string i + "-arg")) [0..List.length xs - 1]
    // stack: return value :: previous argument values in reverse order @ calling address :: ...
    let restorePrevArgsAndReturnTopstackValue =
      storeReturnValue @ getAllLocalsFromStack @ storeCallingAddress
       @ loadReturnValue @ loadCallingAddress @ [Return]
    let redefWithEarlyReturn = function
      |AST.Return (Some a) -> Some (compileASM' a @ restorePrevArgsAndReturnTopstackValue)
      |AST.Return None -> Some ([Push "()"] @ restorePrevArgsAndReturnTopstackValue)
      |x -> redef x
    let functionBlock =
      storeCallingAddress @ storeArgumentsLocally @ getArgumentsFromStack
       @ loadCallingAddress @ storeArgumentsOnStackFromLocal
       @ compileASM redefWithEarlyReturn b
       @ restorePrevArgsAndReturnTopstackValue
    let skipFunctionBlock, pushFunctionBlock = List.length functionBlock + 1, - List.length functionBlock
    let assignFunction =  // x :: ... -> newheap :: ... where newheap <- [|x; terminator|]
      [ stTemp; NewHeap; Store f;  // temp <- x, f <- newheap
        Load f; ldTemp; WriteHeap;  // newheap[0] <- x
        NewHeap; Push "endArr"; WriteHeap;  // newheap[1] <- endArr
        Load f ]
    [GotoFwdShift skipFunctionBlock] @ functionBlock @ [PushFwdShift pushFunctionBlock] @ assignFunction
  |Get(a, i) -> compileASM' a @ compileASM' i @ [Add; GetHeap]
  |If(a, b, c) ->
    let compiledAff, compiledNeg = compileASM' b, compileASM' c
    let skipAffBlock, skipNegBlock = List.length compiledAff + 1, List.length compiledNeg + 2
    compileASM' a @ [GotoIfTrueFwdShift skipNegBlock] @ compiledNeg
     @ [GotoFwdShift skipAffBlock] @ compiledAff
  |Loop(a, b) ->
    let compiledCond, compiledBody = compileASM' a, compileASM' b @ [Pop]
    let skipBody = List.length compiledBody + 2
    let loopAgain = -(List.length compiledCond + 2 + List.length compiledBody)
    compiledCond @ [GotoIfTrueFwdShift 2; GotoFwdShift skipBody]
     @ compiledBody @ [GotoFwdShift loopAgain; Push "()"]
  |Mutate(a, b) -> compileASM' (Declare(a, b))
  |New a ->
    let allocInALoop =  // a :: ... -> ... where a spots have been created
      let cond =  // a :: ... -> (a > 1)? :: a :: ...
        pushAgain @ [Push "1"; LEq]
      let body =  // a :: ... -> a - 1 :: ... where 1 spot has been created
        [NewHeap; Pop; Push "-1"; Add]
      let skipBody = List.length body + 2
      let loopAgain = -(List.length cond + 2 + List.length body)
      cond @ [GotoIfTrueFwdShift 2; GotoFwdShift skipBody] @ body @ [GotoFwdShift loopAgain; Pop]
    [NewHeap] @ pushAgain @ compileASM' a @ pushAgain @ allocInALoop    // stack: a :: newheap :: newheap :: ...
     @ [Add; Push "endArr"; WriteHeap]   // write terminator
  |AST.Return a -> failwith "invalid: found return which was not in a function"
  |Sequence ll ->
    match List.map compileASM' ll with
    |[] -> [Push "()"]
    |firstCompiledStatement::rest ->
      List.concat (firstCompiledStatement::List.map (fun e -> Pop::e) rest)
  |Value s -> [Load s]
let predefined = function
  // consider: let f = printf "%i" in f 1
  |Apply(Value "printf", [Const fmt]) when List.exists ((=) fmt) allTypes ->
    Some (compileASM (fun _ -> None) (Value ("printf " + fmt)))
  |Value "printf" -> Some (compileASM (fun _ -> None) (Value ("printf %s")))
  |Apply(Value "scan", [Const fmt | Value fmt]) -> Some [Input fmt]
  |Value "nothing" -> Some [Push "nothing"]     // void; change this later so that assigning `nothing` does nothing
  |_ -> None
let defineBinaryOperator op =
  let functionBlock2Args =  // calling address :: b :: a :: ... -> return to calling address, op a b :: ...
    [stTemp; Combinator_2 op; ldTemp; Return]
  let skipFunctionBlock2, pushFunctionBlock2 =
    List.length functionBlock2Args + 1, - List.length functionBlock2Args - 6
  let functionBlock1Arg =  // calling address :: a :: ... -> return, newheap :: ... where newheap = [|f2args; a|]
    [ stTemp; Store (op.Symbol + "-1"); NewHeap; stTemp2;  // temp <- calling address, +-1 <- a, temp2 <- newheap
      ldTemp2; PushFwdShift pushFunctionBlock2; WriteHeap;  // newheap[0] <- +
      NewHeap; Load (op.Symbol + "-1"); WriteHeap;  // newheap[1] <- a
      NewHeap; Push "endArr"; WriteHeap;  // newheap[2] <- endArr
      ldTemp2; ldTemp; Return ]
  let skipFunctionBlock1, pushFunctionBlock1 =
    List.length functionBlock1Arg + 1, - List.length functionBlock1Arg
  let assignFunction =  // x :: ... -> newheap :: ... where newheap <- [|x; terminator|]
    let f = op.Symbol
    [ stTemp; NewHeap; Store f;  // temp <- x, f <- newheap
      Load f; ldTemp; WriteHeap;  // newheap[0] <- x
      NewHeap; Push "endArr"; WriteHeap;  // newheap[1] <- endArr
     ]    //Load f ]
  [GotoFwdShift skipFunctionBlock2] @ functionBlock2Args
   @ [GotoFwdShift skipFunctionBlock1] @ functionBlock1Arg
   @ [PushFwdShift pushFunctionBlock1] @ assignFunction
let definePrint (formattedName:string) =
  let functionBlock =  // calling address :: a :: ... -> return to calling address, () :: ... where a has been printed
    [stTemp; Output (formattedName.Split(' ').[1]); Push "()"; ldTemp; Return]
  let skipFunctionBlock, pushFunctionBlock = List.length functionBlock + 1, - List.length functionBlock
  let assignFunction =  // x :: ... -> newheap :: ... where newheap <- [|x; terminator|]
    let f = formattedName
    [ stTemp; NewHeap; Store f;  // temp <- x, f <- newheap
      Load f; ldTemp; WriteHeap;  // newheap[0] <- x
      NewHeap; Push "endArr"; WriteHeap;  // newheap[1] <- endArr
     ]    //Load f ]
  [GotoFwdShift skipFunctionBlock] @ functionBlock @ [PushFwdShift pushFunctionBlock] @ assignFunction
let compileToASM ast =
  List.collect (function Combinator_2 x -> defineBinaryOperator x | _ -> failwith"") allCombinators
   @ List.collect ((+) "printf " >> definePrint) allTypes
   @ compileASM predefined ast