module PseudoAsm
open AST
open Type_Inference
open System

//unfinished:
//proper, non-tail recursive recursion
//optimize memory for variables which fall out of scope (careful with the correct types)

[<Literal>]
let MAXSTACK=15
type primitive=
  |Int of int
  |Str of string
  |Bln of bool
  |Unit
  |Goto of string
let parsePrimitive=function
  |"()" -> Unit
  |"true" -> Bln true
  |"false" -> Bln false
  |"" -> failwith "empty literal?"
  |s when Int32.TryParse(s,ref 0) -> Int (int s)
  |s when s.Length>=2 && s.[0]='"' && s.[s.Length-1]='"' -> Str s.[1..s.Length-2]
  |s -> failwithf "unknown literal %s" s
type AsmST=
  |Push of primitive
  |Pop
  |Load of int
  |Store of int
  |Marker of string
  |GotoIfTrue
  |Command of name:string
let nextUnused acc add _=
  add acc
  !acc
let convert e=
  let nuv=nextUnused (ref 0) incr      // 0 reserved for function call results
  let nuf=nextUnused (ref 0) incr
  let nuc=nextUnused (ref 0) incr
  let nuif=nextUnused (ref 0) incr
  let rec deabstract stackVariables scopeVariables e=
    match e with
    |V s when Map.containsKey s scopeVariables -> [Load scopeVariables.[s]]
    |V s -> [Push (parsePrimitive s)]
    |Define(s,b) ->
      let s_var,funcID,contID=nuv(),sprintf "f%i" (nuf()),sprintf "c%i" (nuc())
      [ Push (Goto contID)
        Push (Bln true)
        GotoIfTrue
        Marker funcID
        Store s_var        ]
       @ deabstract (s_var::stackVariables) (scopeVariables.Add(s,s_var)) b
       @ [Store 0; Push (Bln true); GotoIfTrue]    //store result, then go back to the calling code
       @ [Marker contID; Push (Goto funcID)]
    |Bind(s,b,r) ->
      let s_var=nuv()
      deabstract stackVariables (scopeVariables.Add(s,s_var)) b
       @ [Store s_var]
       @ deabstract stackVariables (scopeVariables.Add(s,s_var)) r
    |Apply(V "printfn",V "\"%A\"") -> [Command "print"]
    |Apply(a,b) ->
      let callingID=sprintf "f%i" (nuf())
      List.map Load stackVariables      //load all the variables which could be overwritten by recursion
       @ [Push (Goto callingID)]
       @ deabstract stackVariables scopeVariables b
       @ deabstract stackVariables scopeVariables a
       @ [Push (Bln true); GotoIfTrue; Marker callingID]
       @ List.map Store (List.rev stackVariables)     //rewrite them all after the function call
       @ [Load 0]
    |Condition(a,b,c) ->
      let aff,neg,rest=sprintf "if%i" (nuif()),sprintf "if%i" (nuif()),sprintf "if%i" (nuif())
      [Push (Goto neg); Push (Goto aff)]
       @ deabstract stackVariables scopeVariables a
       @ [ GotoIfTrue
           Push (Bln true)
           GotoIfTrue      ]
       @ [Marker aff] @ deabstract stackVariables scopeVariables b
       @ [Push (Goto rest); Push (Bln true); GotoIfTrue]
       @ [Marker neg] @ deabstract stackVariables scopeVariables c
       @ [Marker rest]
  [Command (sprintf "maxstack %i" MAXSTACK); Command "locals 10"] @ deabstract [] Map.empty e @ [Command "end"]

let interpret commands=
  let commands=Array.ofList commands
  let markedLines=
    Array.mapi (fun i e -> (e,i)) commands
     |> Array.choose (function Marker e,i -> Some(e,i) | _ -> None)
     |> dict
  let setIndex i v a=
    Array.set a i v
    a
  let rec interpret stack locals x=
    printfn "%A\n%A\n%A" commands.[x] stack locals
    ignore <| System.Console.ReadLine()
    match commands.[x] with
    |Command "end" -> (stack,locals)
    |Push v -> interpret (v::stack) locals (x+1)
    |Pop ->
      match stack with
      |_::tl -> interpret tl locals (x+1)
      |[] -> failwith "could not pop empty stack"
    |Load i -> interpret (Array.get locals i::stack) locals (x+1)
    |Store i ->
      match stack with
      |hd::tl -> interpret tl (setIndex i hd locals) (x+1)
      |[] -> failwith "could not store from empty stack"
    |Marker _ -> interpret stack locals (x+1)
    |GotoIfTrue ->
      match stack with
      |Bln true::Goto label::tl -> interpret tl locals (markedLines.[label])
      |Bln false::Goto _::tl -> interpret tl locals (x+1)
      |_ -> failwithf "goto failed: %A" stack
    |Command s ->
      match s.Split ' ' with
      |[|"maxstack"; n|] -> interpret stack locals (x+1)   //assume it won't run out of stack space
      |[|"locals"; n|] -> interpret stack (Array.create (int n) Unit) (x+1)
      |[|"print"|] ->
        match stack with
        |hd::tl ->
          printf "%O" hd
          interpret tl locals (x+1)
        |[] -> failwith "could not print from empty stack"
      |_ -> failwith "unknown command"

  interpret [] [||] 0
