module Excel_Language_Interpreter
open System
open Excel_Language

type Interpreted=
  |N of int
  |S of string
  |B of bool
  |ERROR
let interpret iterations maxChange cellNames (cells:Formula[])=
  let buffer=Array.create cells.Length (N 0)
  let cellsByName=dict (Array.mapi (fun i e -> (e,i)) cellNames)
  let rec evaluateCell s=function
    |Literal v ->
      match v with
      |Choice1Of3 v -> N v
      |Choice2Of3 v -> S v
      |Choice3Of3 v -> B v
    |Reference s -> buffer.[cellsByName.[s]]
    |If(cond,aff,neg) ->
      match evaluateCell s cond with
      |B true -> evaluateCell s aff
      |B false -> evaluateCell s neg
      |_ -> ERROR
    |Choose(n,choices) ->
      match evaluateCell s n with
      |N i when 1 <= i && i <= choices.Length -> evaluateCell s choices.[i-1]
      |_ -> ERROR
    |Compare(a,comb,b) ->
      match evaluateCell s a,evaluateCell s b with
      |N a,N b -> B (compOp comb a b)
      |S a,S b -> B (compOp comb a b)
      |ERROR,_ | _,ERROR -> ERROR
      |_ ->
        match comb with
        |Comparison.Equal -> B false
        |Comparison.Unequal -> B true
        |_ -> ERROR
    |Combine(a,comb,b) ->
      match evaluateCell s a,evaluateCell s b with
      |N a,N b -> N (combOp comb a b)
      |_ -> ERROR
    |Concatenate a ->
      Array.fold (fun acc e ->
        match acc,evaluateCell s e with
        |S acc,N e -> S (acc+string e)
        |S acc,S e -> S (acc+e)
        |S acc,B e -> S (acc+string e)
        |ERROR,_ | _,ERROR -> ERROR
       ) (S "") a
  let primary=Array.findIndex (fun (e:Formula) -> e.hasReference()) cells
  buffer.[primary] <- evaluateCell "F1" cells.[primary]
  for _ in 1..iterations do
    Array.iter2 (fun (a:string) b -> if a.[0]>'D' then printfn "cell %s: %A" a b) cellNames buffer
    Console.ReadLine() |> ignore
    Array.iteri2
     (fun i s e -> buffer.[i] <- evaluateCell s e)
     cellNames cells
  cellNames,buffer

(*
  if iterations=0
   then
    cellNames,
    Array.map2 ((fun a _ (value:Formula) ->
      match (!a,value.hasReference()) with
      |(true,true) ->
        a:=false
        N 0
      |_ -> ERROR     //literal cells should be fine upon first reevaluation
     ) (ref true)) cellNames cells
   else
    let _,data=interpret (iterations-1) maxChange cellNames cells
    let rec evaluateCell s=function
      |Literal v ->
        match v with
        |Choice1Of3 v -> N v
        |Choice2Of3 v -> S v
        |Choice3Of3 v -> B v
      |Reference s -> data.[Array.findIndex ((=) s) cellNames]     //improve
      |If(cond,aff,neg) ->
        match evaluateCell s cond with
        |B true -> evaluateCell s aff
        |B false -> evaluateCell s neg
        |_ -> ERROR
      |Choose(n,choices) ->
        match evaluateCell s n with
        |N i when 1 <= i && i <= choices.Length -> evaluateCell s choices.[i-1]
        |_ -> ERROR
      |Compare(a,comb,b) ->
        match evaluateCell s a,evaluateCell s b with
        |N a,N b -> B (compOp comb a b)
        |S a,S b -> B (compOp comb a b)
        |ERROR,_ | _,ERROR -> ERROR
        |_ ->
          match comb with
          |Comparison.Equal -> B false
          |Comparison.Unequal -> B true
          |_ -> ERROR
      |Combine(a,comb,b) ->
        match evaluateCell s a,evaluateCell s b with
        |N a,N b -> N (combOp comb a b)
        |_ -> ERROR
      |Concatenate a ->
        Array.fold (fun (S acc) e ->
          match evaluateCell s e with
          |N e -> S (acc+string e) | S e -> S (acc+e) | B e -> S (acc+string e)
          |ERROR -> ERROR
         ) (S "") a
    let rr=Array.map2 evaluateCell cellNames cells
    //Array.iter2 (printfn "cell %s: %A") cellNames rr
    //ignore <| System.Console.ReadLine()
    cellNames,rr
*)