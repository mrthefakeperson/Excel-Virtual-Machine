module PseudoAsm
open AST
open Type_Inference
open System

//unfinished:
//proper, non-tail recursive recursion
//optimize memory for variables which fall out of scope (careful with the correct types)

[<Literal>]
let MAXSTACK=15
type primitive=Choice<int,string,bool,unit,int[]>
let parsePrimitive=function
  |"()" -> Choice4Of5 ()
  |"true" -> Choice3Of5 true
  |"false" -> Choice3Of5 false
  |"" -> failwith "empty literal?"
  |s when Int32.TryParse(s,ref 0) -> Choice1Of5 (Int32.Parse s)
  |s when s.Length>=2 && s.[0]='"' && s.[s.Length-1]='"' -> Choice2Of5 s.[1..s.Length-2]
  |s -> failwithf "unknown literal %s" s
type AsmST=
  |Push of primitive
  |Pop
  |Load of int
  |Store of int
  |Marker of string
  |Goto of name:string
  |Command of name:string
let nextUnused acc add _=
  add acc
  !acc
let convert e=
  let types=Type_Inference.inferSimple e
  types.Add(V "function-=",A "type")
  let getType e=
    match types.[e] with
    |Function(_,_) -> A "int_pair"
    |_ -> types.[e]
  let nufuncID=nextUnused (ref -1) incr
  let (lvarTypes:Type_Inference.Type ResizeArray),acc=ResizeArray(),ref 1
  let nuloc t=     //0 and 1 are reserved for function application and array creation
    nextUnused acc (fun e ->
      lvarTypes.Add (getType t)
      incr e
     ) ()
  let nucont=nextUnused (ref -1) incr
  let nucond=nextUnused (ref -1) incr
  let rec deabstract localVariables=function
    |V s when Map.containsKey s localVariables -> [Load localVariables.[s]]
    |V s -> [Push (parsePrimitive s)]
    |Define(s,(Define(_,_) as b)) ->
      let loc=nuloc (V s)
      let rec insrt=function    //not working: Goto _, Store 1/do stuff/load 1 order disruption
                                //fix: replace anything_else with Store 1 and the other with anything_else
        |(Store _ | Marker _ as hd)::tl -> hd::insrt tl
        |anything_else -> Store loc::anything_else
      insrt (deabstract (localVariables.Add (s,loc)) b)
    |Define(s,b) as e ->
      let n,loc_s,cont=nufuncID(),nuloc (V s),nucont()
      let loc_self,loc_f=nuloc e,nuloc (V "function-=") 
      [ Goto (sprintf "c%i" cont)
        Marker (sprintf "f%i" n)
        Store loc_s
        Store 1                   ]
       @ deabstract (localVariables.Add (s,loc_s)) b
       @ [ Load 1
           Goto "universal_function"
           Marker (sprintf "c%i" cont)
           Push (Choice1Of5 n)
           Store loc_f
           //Command "make_pair"      DEPRECIATED
           Push (Choice1Of5 2)
           Command "new_array"
           Store loc_self
           Load loc_self
           Push (Choice1Of5 0)
           Command (sprintf "load_address_of %i" loc_f)
           Command "store_index"
           Load loc_self
           Push (Choice1Of5 1)
           Push (Choice1Of5 -1)
           Command "store_index"
           Load loc_self                                ]
    |Bind(s,b) -> failwith "lone bind detected with no proceeding expression"
    |Apply(Bind(s,b),a) ->
      let loc=nuloc (V s)
      let rr=
        match deabstract (localVariables.Add (s,loc)) b with
        |Marker a::_ as r ->
          let cont=nucont()
          [ Push (Choice1Of5 (int a.[1..]))
            Store loc
            Goto (sprintf "c%i" cont)       ]
           @ r @ [Marker (sprintf "f%i" cont)]
        |r -> r @ [Store loc]
      rr @ deabstract (localVariables.Add (s,loc)) a
    |Apply(a,b) as e ->
      let cmd_a=deabstract localVariables a
      let loc_a=
        match Seq.last cmd_a with
        |Load loc_a -> loc_a
        |_ -> failwith "couldn't find location of function to apply"
      let loc_b,loc_self,cont=nuloc b,nuloc a,nufuncID()     //function applications are all pointer pairs regardless of being complete or not
      let apply=
        match types.[e] with
        |Function(_,_) -> [Load loc_self]
        |_ ->
          [ Push (Choice1Of5 cont)
            Load loc_self
            Goto "apply_universal_function"
            Marker (sprintf "f%i" cont)     ]
      //new pair: (pointer to b, pointer to loc_a)
      cmd_a
       @ [ Pop
           //Command "make_pair"
           Push (Choice1Of5 2)
           Command "new_array"
           Store loc_self
           Load loc_self
           Push (Choice1Of5 0) ]
       @ deabstract localVariables b
       @ [ Store loc_b
           Command (sprintf "load_address_of %i" loc_b)
           Command "store_index"
           Load loc_self
           Push (Choice1Of5 1)
           Command (sprintf "load_address_of %i" loc_a)
           Command "store_index"                     ]
       @ apply
    |Condition(a,b,c) ->
      let t,f=nucond(),nucond()
      deabstract localVariables a
       @ [Command (sprintf "goto_if_true %i" t)]
       @ deabstract localVariables c
       @ [Goto (sprintf "t%i" f); Marker (sprintf "t%i" t)]
       @ deabstract localVariables b
       @ [Marker (sprintf "t%i" f)]

  let body=Marker "real_entrypoint"::deabstract Map.empty e
  let head=
    [ Command (sprintf "declare_locals %s"
       ("int_pair|type|" + String.concat "|"
         [for e in lvarTypes -> match e with A e -> e | _ -> "sdfdsf" (*temp*)]))
      Command (sprintf "maxstack %i" MAXSTACK)
      Command "entrypoint"

      Goto "real_entrypoint"
      Marker "apply_universal_function"
      Store 0
      Load 0
      Push (Choice1Of5 0)
      Command "load_index"
      Command "load_address"
      Load 0
      Push (Choice1Of5 1)
      Command "load_index"
      Load 0
      Push (Choice1Of5 1)
      Command "load_index"
      Push (Choice1Of5 -1)
      Command "equals"
      Command "goto_if_true apply2"
      Command "load_address"
      Goto "apply_universal_function"
      Marker "apply2"
      Pop
      Goto "universal_function" ]
     @ (
        let rec check (l,r)=
          if l>r then []
           else
            if l=r then [Marker (sprintf "bound%i_%i" l r); Goto (sprintf "f%i" l)]
             else
              let h=(l+r)/2
              [ Marker (sprintf "bound%i_%i" l r)
                Load 1
                Push (Choice1Of5 h)
                Command "greater_than"
                Command (sprintf "goto_if_true bound%i_%i" (h+1) r)
                Goto (sprintf "bound%i_%i" l h) ]
               @ check (l,h)
               @ check (h+1,r)
        [ Marker "universal_function"
          Store 1                     ]
         @ check (0,nufuncID()-1)
      )
  head @ body @ [Command "print"; Command "ret"]

let rec translate e=
  List.map (function
    |Push e ->
      match e with
      |Choice1Of5 q -> sprintf "ldc.i4 %i" q
      |Choice2Of5 q -> sprintf "ldstr %s" q
      |Choice3Of5 true -> sprintf "ldc.i4 1"
      |Choice3Of5 false -> sprintf "ldc.i4 0"
      |Choice4Of5 () -> sprintf "ldc.i4 -1"     //doesn't matter, just load something
      |Choice5Of5 [|a;b|] ->
        translate
          [ //Command "make_pair"
            Push (Choice1Of5 2)
            Command "new_array"
            Store 0
            Load 0
            Push (Choice1Of5 0)
            Push (Choice1Of5 a)
            Command "store_index"
            Load 0
            Push (Choice1Of5 1)
            Push (Choice1Of5 b)
            Command "store_index"
            Load 0                ]
    |Pop -> "pop"
    |Load i -> sprintf "ldloc %i" i
    |Store i -> sprintf "stloc %i" i
    |Marker s -> s + ":"
    |Goto s -> "br " + s
    |Command c ->
      match c.Split ' ' with
      |[|c|] ->
        match c with
        |"entrypoint" -> ".entrypoint"
        |"make_pair" -> translate [Push (Choice1Of5 2); Command "new_array"]
        |"new_array" -> "newarr [mscorlib]System.Int32"
        |"load_index" -> "ldelem.i4"     //only int32 arrays are created right now
        |"store_index" -> "stelem.i4"
        |"load_address" -> "ldind.i4"
        |"equals" -> "ceq"
        |"greater_than" -> "cgt"
        |"print" -> "call void [mscorlib]System.Console::WriteLine(int32)"
        |"ret" -> "ret"
        |_ -> failwithf "unrecognized command %s" c
      |[|"maxstack";n|] -> sprintf ".maxstack %s" n
      |[|"declare_locals";locals|] ->
        locals.Split '|'
         |> Array.map (function "type" -> "int32" | _ -> "int32[]")    //temp
         |> String.concat ","
         |> sprintf ".locals init (%s)"
      |[|"load_address_of";var|] -> sprintf "ldloca %s" var
      |[|"goto_if_true";pl|] -> sprintf "brtrue %s" pl
      |[|"goto_if_greater_than";pl|] -> sprintf "bgt %s" pl     //depreciated
      |_ -> failwithf "unrecognized command %s" c
   ) e
   |> String.concat "\n"

let getType=function
  |Choice1Of5 _ -> "int"
  |Choice2Of5 _ -> "string"
  |Choice3Of5 _ -> "bool"
  |Choice4Of5 _ -> "unit"
  |Choice5Of5 _ -> "int_pair"
let getVal_int=function
  |Choice1Of5 a -> a
  |_ -> failwith "wrong type"
let interpret e=
  let rec getMarkers=function
    |[] -> Map.empty
    |Marker s::tl -> Map.add s tl (getMarkers tl)
    |_::tl -> getMarkers tl
  let markers=getMarkers e
  //address of: add 100000
  let rec interpret (stack,locals:primitive[]) e=
    try
      match e with
        |[] -> ()
        |hd::tl ->
          //printfn "%A\n  %A\n\t%A" hd stack locals
          //ignore <| Console.ReadLine()
          match hd with
          |Push e -> interpret (e::stack,locals) tl
          |Pop -> interpret (stack.Tail,locals) tl
          |Load i -> interpret (locals.[i]::stack,locals) tl
          |Store i ->
            if stack=[] then
              failwith "tried to store topstack, but stack was empty"
            if getType stack.Head <> getType locals.[i] then
              failwithf "tried to store %s as %s at %i" (getType stack.Head) (getType locals.[i]) i
            locals.[i]<-stack.Head
            interpret (stack.Tail,locals) tl
          |Marker s -> interpret (stack,locals) tl
          |Goto s ->
            if not (Map.containsKey s markers)
             then failwithf "tried to goto %s; it did not exist" s
             else interpret (stack,locals) markers.[s]
          |Command c ->
            match c.Split ' ' with
            |[|c|] ->
              match c with
              |"entrypoint" -> interpret (stack,locals) tl
              |"make_pair" -> interpret (Choice5Of5 [|0;0|]::stack,locals) tl
              |"new_array" ->
                match stack with
                |Choice1Of5 hd::ttl -> interpret (Choice5Of5 (Array.create hd 0)::ttl,locals) tl
                |_ -> failwith "stack did not immediately contain an integer (array creation parameter)"
              |"load_index" ->
                match stack with
                |Choice1Of5 i::Choice5Of5 p::rest -> interpret (Choice1Of5 p.[i]::rest,locals) tl
                |_ -> failwith "couldn't load index"
              |"store_index" ->
                match stack with
                |Choice1Of5 v::Choice1Of5 i::Choice5Of5 p::rest ->
                  p.[i]<-v
                  interpret (rest,locals) tl
                |_ -> failwith "couldn't store index"
              |"load_address" -> interpret (locals.[getVal_int stack.Head-100000]::stack.Tail,locals) tl
              |"equals" ->
                match stack with
                |[] | [_] -> failwith "stack does not contain two items"
                |a::b::rest ->
                  if getType a <> getType b then failwith "compared different types"
                  interpret (Choice1Of5 (if a=b then 1 else 0)::rest,locals) tl
              |"greater_than" ->
                match stack with
                |[] | [_] -> failwith "stack does not contain two items"
                |a::b::rest ->
                  if getType a <> getType b then failwith "compared different types"
                  interpret (Choice1Of5 (if b>a then 1 else 0)::rest,locals) tl
              |"print" ->
                printfn "%O" (getVal_int stack.Head)
                interpret (stack.Tail,locals) tl
              |"ret" ->
                if stack<>[] then failwith "did not return void"
                printfn "ended with\n%A" (stack,locals)
                interpret (stack,locals) []
              |_ -> failwithf "unrecognized command %s" c
            |[|"maxstack";n|] -> interpret (stack,locals) tl
            |[|"declare_locals";locals|] ->
              let locals'=
                locals.Split '|'
                 |> Array.map (function "type" -> Choice1Of5 0 | _ -> Choice5Of5 [|0;0|])    //temp
              interpret (stack,locals') tl
            |[|"load_address_of";var|] -> interpret (Choice1Of5 (int var+100000)::stack,locals) tl
            |[|"goto_if_true";pl|] ->
              match stack with
              |[] -> failwith "empty stack"
              |Choice1Of5 1::rest -> interpret (rest,locals) (Goto pl::tl)
              |_::rest -> interpret (rest,locals) tl
            |[|"goto_if_greater_than";pl|] ->
              match stack with
              |[] | [_] -> failwith "two items not in the stack"
              |Choice1Of5 b::Choice1Of5 a::rest ->
                interpret
                 (Choice1Of5 (if a>b then 1 else 0)::rest,locals)
                 (Command (sprintf "goto_if_true %s" pl)::tl)
              |_ -> failwith "compared two things which aren't ints"
            |_ -> failwithf "unrecognized command %s" c
    with
      |ex ->
        printfn "%A\n%A\n%A" stack locals ex.Message
        ignore <| Console.ReadLine()
        failwith "error encountered"
  interpret ([],[||]) e