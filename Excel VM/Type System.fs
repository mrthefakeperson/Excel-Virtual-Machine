// todo: structs get deepcopied, pointers don't
// todo: recursive structs

module Type_System
open Parser.Token
open System.Collections.Generic

// dict: name_of_member -> (index, initial_value [option])
// used to map member names of objects (as strings) to array indices, with possible initial values
// objects will be compiled to arrays, with each member using its mapped index
type memberMapping = IDictionary<string, int*Token option>
let noObject:memberMapping = dict []
// hashtable: name_of_type -> mapping_for_type_members
let objectTypes = Dictionary<string, memberMapping>()
let compileObjectDeclarations = function
  |X("struct", [T name; memberList]) ->
    match memberList with
    |X("sequence", members) ->
      let mapMembers =
        List.mapi (fun i e ->
          match e with
          |X("let", [(T memberName | X("declare", [_; T memberName])); T "nothing"]) ->
            (memberName, (i, None))
          |X("let", [(T memberName | X("declare", [_; T memberName])); initialValue]) ->
            (memberName, (i, Some initialValue))
          |_ -> failwith "wrong member format"
         ) members
         |> dict
      objectTypes.["struct " + name] <- mapMembers
    |_ -> failwith "wrong struct format"
  |_ -> failwith "should never happen"
// hashtable: variable_name -> stack of member mappings for the variable
let varTypes = Dictionary<string, memberMapping list>()
let rec compileObjects = function
  |X("struct", _) as x ->
    compileObjectDeclarations x
    Token "()"
  |X("dot", [T a; T b]) ->  //  consider: (new object(?)).k;      does (T b) come in a sequence?
    Token("dot", [Token a; Token("[]", [Token(string(fst varTypes.[a].Head.[b]))])])
  |X("sequence", xprs) ->
    let yld =
      List.collect (function
        |X("let", [X("declare", [T typeName; T a]); b]) as e ->
          let mapping =
            if objectTypes.ContainsKey typeName
             then objectTypes.[typeName]
             else noObject
          if not (varTypes.ContainsKey a) then varTypes.[a] <- []
          varTypes.[a] <- mapping::varTypes.[a]
          match mapping.Count, b with
          |n, T "nothing" when n > 0 ->
            let allocate =  // let (a:'typeName) = array of size n
              Token("let", [Token("declare", [Token typeName; Token a]); Token("array", [Token(string n)])])
            let initializeMembers =
              Seq.collect (function
                |(i, Some v) ->    // a.[i] <- v
                  [Token("assign", [Token("dot", [Token a; Token("[]", [Token (string i)])]); v])]
                |(i, None) -> []
               ) mapping.Values
               |> Seq.toList
            allocate::initializeMembers
          |_ -> [compileObjects e]
        |x -> [compileObjects x]
       ) xprs
       |> fun e -> Token("sequence", e)
    // end of scope: pop all mappings
    List.iter (function
      |X("let", [X("declare", [_; T a]); b]) -> varTypes.[a] <- varTypes.[a].Tail
      |_ -> ()
     ) xprs
    yld
  |X(s, dep) -> Token(s, List.map compileObjects dep)

let restoreDefault() =
  objectTypes.Clear()
  varTypes.Clear()
let compileObjectsToArrays x =
  restoreDefault()
  compileObjects x


type LabelledToken = LT of string*LabelledToken list*int
let compilePointersToArrays x =
  let acc = ref -1
  let nxt() =
    incr acc
    !acc
  // give each variable within its own scope a unique ID (int)
  let rec labelTokens labels = function
    |T s when Map.containsKey s labels -> LT(s, [], labels.[s])
    |X("sequence", nodes) ->
      let nodes' =
        List.fold (fun (accLabels, acc) -> function
          |X("let", [(T varName | X("declare", [_; T varName]) as a); b]) ->
            let x = nxt()
            let newLabels = Map.add varName x accLabels
            let newNode = LT("let", [labelTokens newLabels a; labelTokens newLabels b], -1)
            (newLabels, newNode::acc)
          |x -> (accLabels, labelTokens accLabels x::acc)
         ) (labels, []) nodes
         |> snd
         |> List.rev
      LT("sequence", nodes', -1)
    |X(s, dep) -> LT(s, List.map (labelTokens labels) dep, -1)
  let k = labelTokens Map.empty x
  //printfn "labelled: %A" k
  // find all the var IDs which get their reference taken
  let hasReference = Array.create (nxt()) false
  let rec findAllDerefs = function
    |LT("apply", [LT("~&", [], _); LT(s, [], a)], _) when a <> -1 ->
      hasReference.[a] <- true
    |LT(_, ll, _) -> List.iter findAllDerefs ll
  findAllDerefs k
  //printfn "hasDerefs: %A" hasDereference
  // map referenced vars to arrays
  let (|IsDeref|_|) = function
    |LT(s, [], x) -> if x <> -1 && hasReference.[x] then Some s else None
    |_ -> None
  let rec mapDerefs = function
    |IsDeref a -> Token("dot", [Token a; Token("[]", [Token "0"])])   // a -> a.[0]
    |LT("apply", [LT("~&", [], _); IsDeref a], _) -> Token a   // &a -> a
    |LT("sequence", nodes, _) ->
      let nodes' =
        List.collect (function
          |LT("let", [(IsDeref a | LT("declare", [_; IsDeref a], _)); b], _) ->
            [ Token("let", [Token a; Token("array", [Token "1"])])
              Token("assign", [Token("dot", [Token a; Token("[]", [Token "0"])]); mapDerefs b]) ]
          |x -> [mapDerefs x]
         ) nodes
      Token("sequence", nodes')
    |LT(s, dep, _) -> Token(s, List.map mapDerefs dep)
  let yld = mapDerefs k
  //printfn "yield: %A" yld
  yld

let rec processDerefs = function
  |X("apply", [T "~*"; x]) -> Token("deref", [x])
  |X(s, dep) -> Token(s, List.map processDerefs dep)


let applyTypeSystem:Token -> Token =
  compileObjectsToArrays
   >> compilePointersToArrays
   >> processDerefs