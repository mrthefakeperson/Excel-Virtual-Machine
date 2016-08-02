module Type_Inference
open System
open AST

type Type=
  |A of string
  |Function of Type*Type
  |Tuple of Type list

let nextUnusedGeneric=
  let nug (acc:string ref) ()=
    if (!acc).Length=0 || (!acc).[(!acc).Length-1]='z'
     then acc:= !acc+"a"
     else acc:= (!acc).[..(!acc).Length-2]+string((!acc).[(!acc).Length-1]+char 1)
    !acc
  nug (ref "")
let (|Generic|_|)=function
  |A s when s.[0]='''->
    Some(Generic s)
  |_ -> None
let inferSingle s=
  match {t=V s; row=0; col=0} with
  |Literal _->
    match s with
    |"()" -> A "unit"
    |_ when Int32.TryParse(s,ref 0) -> A "int"
    |_ when s.[s.Length-1]='L' && Int64.TryParse(s.[..s.Length-2],ref 0L) -> A "int64"
    |_ when s.[0]=''' && s.[s.Length-1]=''' -> A "char"
    |_ when s.[0]='"' && s.[s.Length-1]='"' -> A "string"
    |"true" | "false" -> A "bool"
  |_ -> A (nextUnusedGeneric())
let infer e=
  let inferred=Collections.Generic.Dictionary()
  let rec infer e=
    let rr=
      match e with
      |V s -> inferSingle s
      |Define(s,b) -> Function(inferSingle s,infer b)
      |Bind(s,b)->
        inferred.[V s]<-infer b
        A "()"
      |Apply(Bind(_,_) as a,b)->
        ignore (infer a)
        infer b
      |Apply(a,b)->
        match (infer a,infer b) with
        |Function(Generic a,b),c->
          b    //reinfer b with A 'a <- c
        |Function(a,b),c when a=c -> b
        |_ -> failwith "function application not understood"
      |Condition(a,b,c)->
        if infer a<>A "bool" then failwith "can't branch without boolean"
        match (infer b,infer c) with
        |Generic a,t | t,Generic a->
          t    //reinfer b or c with A 'a <- t
        |b,c when b=c -> b
        |_ -> failwith "branch results should have the same type"
    inferred.[e]<-rr
    rr
  ignore (infer e)
  inferred

//for testing
let inferSimple e=
  let inferred=Collections.Generic.Dictionary()
  inferred.Add(V "(+)",Function(A "type",Function(A "type",A "type")))
  let rec infer e=
    let rr=
      match e with
      |_ when inferred.ContainsKey e -> inferred.[e]
      |V s -> A "type"
      |Define(s,b) -> Function(A "type",infer b)
      |Bind(s,b)->
        inferred.[V s]<-infer b
        A "()"
      |Apply(Bind(_,_) as a,b)->
        ignore (infer a)
        infer b
      |Apply(a,b)->
        match (infer a,infer b) with
        |Function(_,bb),(Function(_,_) as aa)->
          inferred.[a]<-Function(aa,bb)
          bb
        |Function(_,bb),_ -> bb
        |_ -> failwith "not a function, cannot apply"
      |Condition(a,b,c)->
        match (infer b,infer c) with
        |b,c when b=c -> b
        |_ -> failwith "branch results should have the same type"
    inferred.[e]<-rr
    rr
  ignore (infer e)
  inferred