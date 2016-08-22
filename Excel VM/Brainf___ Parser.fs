module Brainf____Parser
open AST

let parseSyntax (syntax:string) =
  let nu acc () =
    incr acc
    !acc
  let nuM = nu (ref 0) >> sprintf "m%i"
  let commands = Array.toList <| syntax.ToCharArray()
  let rec build markers = function
    |[] -> V "()"
    |e::rest ->
      let pt = Index(V "memory",V "pointer")
      match e with
      |'+' -> Bind(pt,Apply(Apply(V "+",pt),V "1"),build markers rest)
      |'-' -> Bind(pt,Apply(Apply(V "+",pt),V "-1"),build markers rest)
      |'>' -> Bind(V "pointer",Apply(Apply(V "+",V "pointer"),V "1"),build markers rest)
      |'<' -> Bind(V "pointer",Apply(Apply(V "+",V "pointer"),V "-1"),build markers rest)
      //|',' ->
      |'.' -> Bind(V "_",Apply(Apply(V "printf",V "\"%A\""),pt),build markers rest)
      |'[' ->
        let mark = nuM()
        Bind(V mark,Define(V "()",build (mark::markers) rest),Apply(V mark,V "()"))
      |']' ->
        Condition(Apply(Apply(V "=",pt),V "0"),Apply(V markers.Head,V "()"),build markers.Tail rest)
  Bind(V "memory",Struct(Array.init 10 (fun _ -> V "0")),
   Bind(V "pointer",V "0",
    build [] commands ))