module Type_System
open Token
open System.Collections.Generic

// dict: name_of_member -> (index, initial_value [option])
// used to map member names of objects (as strings) to array indices, with possible initial values
// objects will be compiled to arrays, with each member using its mapped index
type memberMapping = IDictionary<string, int*Token option>
let noObject:memberMapping = dict []
// hashtable: name_of_type -> mapping_for_type_members
let objectTypes = Dictionary<string, memberMapping>()
let compileObjectType = function
  |X("struct", [T name; memberList]) ->
    printfn "%A" memberList
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
      objectTypes.[name] <- mapMembers
    |_ -> failwith "wrong struct format"
  |_ -> failwith "should never happen"
// hashtable: variable_name -> stack of member mappings for the variable
let varTypes = Dictionary<string, memberMapping list>()
let rec compileObjects = function
  |X("struct", _) as x ->
    compileObjectType x
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