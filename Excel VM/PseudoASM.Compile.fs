module private PseudoASM.Compile
open AST.Definition
open Definition

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
// todo: change to dfs order passing for better performance
// todo: all names should be unique, including closure args
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
      List.mapi (fun i e -> [Load e; Store (string i + "-arg")]) xs |> List.concat
    let storeArgumentsOnStackFromLocal =
      List.map (fun i -> Load (string i + "-arg")) [0..List.length xs - 1]
    // stack: return value :: previous argument values in reverse order @ calling address :: ...
    let restorePrevArgsAndReturnTopstackValue =
      storeReturnValue @ getAllLocalsFromStack @ storeCallingAddress
       @ loadReturnValue @ loadCallingAddress @ [Return]
    let rec redefWithEarlyReturn = function
      |AST.Return (Apply(a, b's)) ->   // tailcall optimization
        let pushB'sThenA = List.collect (compileASM redef) b's @ (compileASM redef) a
        let storeB'sAndA = List.map (fun i -> Store (string i + "-arg")) [b's.Length.. -1..0]
        let loadB'sAndA = List.map (fun i -> Load (string i + "-arg")) [0..b's.Length]
        let handleCall =
          pushB'sThenA @ storeB'sAndA
           @ getAllLocalsFromStack @ storeCallingAddress @ loadB'sAndA
        // state: a (address) :: args @ ... with calling address stored
        let unboxA =  // a :: ... -> unboxed a @ ...
          let pushSuffixArgs =   // x :: ... -> (args in reverse order) @ ...
            let loopBody =  // x :: ... -> x + 1 :: *x :: ...
              [stTemp; ldTemp; GetHeap; ldTemp; Push "1"; Add]
            let loopCond =  // x :: ... -> (*x = terminator)? :: x :: ...
              pushAgain @ [GetHeap; Push "endArr"; Equals]
            let skipBody = List.length loopBody + 2
            let loopAgain = -(List.length loopCond + 1 + List.length loopBody)
            [Push "1"; Add] @ loopCond @ [GotoIfTrueFwdShift skipBody] @ loopBody @ [GotoFwdShift loopAgain; Pop]
          [stTemp2; ldTemp2] @ pushSuffixArgs @ [ldTemp2; GetHeap]
        handleCall @ unboxA @ [stTemp] @ loadCallingAddress @ [ldTemp; Return]
         |> Some
      |AST.Return a -> Some (compileASM redefWithEarlyReturn a @ restorePrevArgsAndReturnTopstackValue)
      |x -> redef x
    let functionBlock =
      storeCallingAddress @ storeArgumentsLocally @ getArgumentsFromStack
       @ loadCallingAddress @ storeArgumentsOnStackFromLocal
       @ compileASM redefWithEarlyReturn b
//       @ restorePrevArgsAndReturnTopstackValue    // no longer necessary with TCO preprocessing
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
  |AST.Return _ | Break | Continue -> failwith "invalid: found return/break/continue in a wrong place"
  |Sequence ll ->
    match List.map compileASM' ll with
    |[] -> [Push "()"]
    |firstCompiledStatement::rest ->
      List.concat (firstCompiledStatement::List.map (fun e -> Pop::e) rest)
  |Value s -> [Load s]
let predefined = function
  // consider: let f = printf "%i" in f 1
  |Apply(Value "printf", [Const fmt]) when List.exists ((=) fmt) allFormatSymbols ->
    Some (compileASM (fun _ -> None) (Value ("printf " + fmt)))
  |Value "printf" -> Some (compileASM (fun _ -> None) (Value ("printf %s")))
  |Apply(Value "scan", [Const fmt | Value fmt]) -> Some [Input fmt]
  |Value "nothing" -> Some [Push "nothing"]     // void; change this later so that assigning `nothing` does nothing
  |_ -> None
let defineBinaryOperator (op:Comb2) =
  let functionBlock2Args =  // calling address :: b :: a :: ... -> return to calling address, op a b :: ...
    [stTemp; Combinator_2(op.Name, op.Symbol); ldTemp; Return]
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
let CompileToASM ast =
  List.collect (fun (c2:PseudoASM) -> defineBinaryOperator (c2.CommandInfo :?> Comb2)) allCombinators
   @
  List.collect ((+) "printf " >> definePrint) allFormatSymbols
   @
//  defineBinaryOperator (allCombinators.Head.CommandInfo :?> Comb2) @
  compileASM predefined ast
   |> Seq.ofList