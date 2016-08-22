module PseudoAsm
open AST
open System

//unfinished:
//partial application of functions

[<Literal>]
let MAXSTACK = 15
[<Literal>]
let STATIC_MEMORY = 15
[<Literal>]
let MEMORY = 30
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
  |Load
  |Store
  |Marker of string
  |GotoIfTrue
  |Allocate
  |AllocateNext
  |Entrypoint of int*int*int
  |End
  |Print
  |Add
  |Eql
let nextUnused acc add _ =
  add acc
  !acc
let convert e=
  let nuv = nextUnused (ref 0) incr      // 0 reserved for function call results
  let nuf = nextUnused (ref 0) incr
  let nuc = nextUnused (ref 0) incr
  let nuif = nextUnused (ref 0) incr
  let rec deabstract stackVariables scopeVariables e =
    match e with
    |Apply(V "printf",V "\"%A\"") -> [Print]
    |Apply(Apply(V "+",a),b) ->
      deabstract stackVariables scopeVariables a
       @ deabstract stackVariables scopeVariables b
       @ [Add]
    |Apply(Apply(V "=",a),b) ->
      deabstract stackVariables scopeVariables a
       @ deabstract stackVariables scopeVariables b
       @ [Eql]
    |V s when Map.containsKey s scopeVariables -> [Push (Int scopeVariables.[s]); Load]
    |V s -> [Push (parsePrimitive s)]
    |Define(V s,b) ->
      let s_var,funcID,contID=nuv(),sprintf "f%i" (nuf()),sprintf "c%i" (nuc())
      [ Push (Goto contID)
        Push (Bln true)
        GotoIfTrue
        Marker funcID
        Push (Int s_var); Store ]
       @ deabstract (s_var::stackVariables) (scopeVariables.Add(s,s_var)) b
       @ [Push (Int 0); Store; Push (Bln true); GotoIfTrue]    //store result, then go back to the calling code
       @ [Marker contID; Push (Goto funcID)]
    |Bind(V s,b,r) ->
      let s_var = if Map.containsKey s scopeVariables then scopeVariables.[s] else nuv()
      deabstract stackVariables (scopeVariables.Add(s,s_var)) b
       @ [Push (Int s_var); Store]
       @ deabstract stackVariables (scopeVariables.Add(s,s_var)) r
    |Bind(Index(st,i),b,r) ->
      deabstract stackVariables scopeVariables b
       @ deabstract stackVariables scopeVariables st
       @ deabstract stackVariables scopeVariables i
       @ [Add; Store]
       @ deabstract stackVariables scopeVariables r
    |Apply(a,b) ->
      let callingID=sprintf "f%i" (nuf())
      List.collect (fun e -> [Push (Int e); Load]) stackVariables   //load all the variables which could be overwritten by recursion
       @ [Push (Goto callingID)]
       @ deabstract stackVariables scopeVariables b
       @ deabstract stackVariables scopeVariables a
       @ [Push (Bln true); GotoIfTrue; Marker callingID]
       @ List.collect (fun e -> [Push (Int e); Store]) (List.rev stackVariables)  //rewrite them all after the function call
       @ [Push (Int 0); Load]
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
    |Struct a ->
      Allocate::
      List.collect (fun e ->
        deabstract stackVariables scopeVariables e @ [AllocateNext; Store]
       ) (List.ofArray a)
    |Index(st,i) ->
      deabstract stackVariables scopeVariables st
       @ deabstract stackVariables scopeVariables i
       @ [Add; Load]
      
  [Entrypoint (MAXSTACK,STATIC_MEMORY,MEMORY)] @ deabstract [] Map.empty e @ [End]

let interpret commands=
  let commands=Array.ofList commands
  let markedLines=
    Array.mapi (fun i e -> (e,i)) commands
     |> Array.choose (function Marker e,i -> Some(e,i) | _ -> None)
     |> dict
  let setIndex i v a=
    Array.set a i v
    a
  let rec interpret stack locals memPt x=
    //printfn "%A\n%A\n%A" commands.[x] stack locals
    //ignore <| System.Console.ReadLine()
    match commands.[x] with
    |End -> (stack,locals)
    |Push v -> interpret (v::stack) locals memPt (x+1)
    |Pop ->
      match stack with
      |_::tl -> interpret tl locals memPt (x+1)
      |[] -> failwith "could not pop empty stack"
    |Load ->
      match stack with
      |Int i::tl -> interpret (Array.get locals i::tl) locals memPt (x+1)
      |_ -> failwith "could not load a local index"
    |Store ->
      match stack with
      |Int i::hd::tl -> interpret tl (setIndex i hd locals) memPt (x+1)
      |_ -> failwith "could not store a local index"
    |Marker _ -> interpret stack locals memPt (x+1)
    |GotoIfTrue ->
      match stack with
      |Bln true::Goto label::tl -> interpret tl locals memPt (markedLines.[label])
      |Bln false::Goto _::tl -> interpret tl locals memPt (x+1)
      |_ -> failwithf "goto failed: %A" stack
    |Allocate -> interpret (Int memPt::stack) locals memPt (x+1)
    |AllocateNext -> interpret (Int memPt::stack) locals (memPt+1) (x+1)
    |Entrypoint(maxstack,static_mem,memory) ->
      interpret stack (Array.create (int memory) Unit) static_mem (x+1) //assume it won't run out of stack space
    |Print ->
      match stack with
      |hd::tl ->
        printf "%O" hd
        interpret tl locals memPt (x+1)
      |[] -> failwith "could not print from empty stack"
    |Add ->
      match stack with
      |Int a::Int b::tl -> interpret (Int (a+b)::tl) locals memPt (x+1)
      |_ -> failwith "could not add"
    |Eql ->
      match stack with
      |a::b::tl -> interpret (Bln (a=b)::tl) locals memPt (x+1)
      |_ -> failwith "could not equate"

  interpret [] [||] 0 0