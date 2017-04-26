module private AST.Optimize
open Definition
open System.Collections.Generic

let rec TCOPreprocess =
  let rec TCOPreprocess' = function
    |Sequence ll ->
      Sequence ((Seq.take (ll.Length - 1) ll |> List.ofSeq) @ [TCOPreprocess' (Seq.last ll)])
    |If(a, b, c) -> If(a, TCOPreprocess' b, TCOPreprocess' c)
    |Return x | x -> Return x
  let rec TCOPreprocess = function
    |Define(f, xs, b) -> Define(f, xs, TCOPreprocess (TCOPreprocess' b))
    |Return x -> TCOPreprocess' x
    |x -> AST.map TCOPreprocess x
  TCOPreprocess
//let rec findCircularReferences =
  



let all ast =
  TCOPreprocess ast