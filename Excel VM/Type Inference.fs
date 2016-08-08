module Type_Inference
open System
open AST

type Type=
  |A of string
  |Function of Type*Type
  |Tuple of Type list
  |Generic of int

let nextUnused acc ()=
  incr acc
  !acc
let nextUnusedGeneric=nextUnused (ref 0) >> Generic
let nextUnusedVarN=nextUnused (ref 0)
let nextUnusedVar a=sprintf "%s$%i" a (nextUnusedVarN())
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
  |_ -> nextUnusedGeneric()
let infer=
  let rec adjustNaming changes=function
    |V s when Map.containsKey s changes -> V changes.[s]
    |V s -> V s
    |Define(s,b) ->
      let s'=if Map.containsKey s changes then nextUnusedVar s else s
      Define(s',adjustNaming (changes.Add (s,s')) b)
    |Bind(s,b,r) ->
      let s'=if Map.containsKey s changes then nextUnusedVar s else s
      Bind(s',adjustNaming (changes.Add (s,s')) b,adjustNaming (changes.Add (s,s')) r)
    |Apply(a,b) ->
      Apply(adjustNaming changes a,adjustNaming changes b)
    |Condition(a,b,c) ->
      Condition(adjustNaming changes a,adjustNaming changes b,adjustNaming changes c)
  let map_merge (a:Map<'a,'b>) (b:Map<'a,'b>)=
    if a.Count>b.Count
     then Map.fold (fun acc key value -> Map.add key value acc) a b
     else Map.fold (fun acc key value -> Map.add key value acc) b a
  let rec mergeTypes a b=
    match a,b with
    |Function(a,b),Function(c,d) -> Function(mergeTypes a c,mergeTypes b d)
    |Generic a,Generic b -> Generic(min a b)     //maybe?
    |x,Generic _ | Generic _,x -> x
    |x,y when x=y -> x
    |x,y -> failwithf "type conflict: expected %A, got %A" x y
  let mergeTypesOption a=function
    |Some b -> mergeTypes a b
    |None -> a
  //knownTypes: map of assumed types of AST nodes captured within the current scope
  //return value: map of AST nodes' types as inferred (only the ones under this node)
  let rec infer knownTypes e=
    match e with
    |V s -> Map [V s,mergeTypesOption (inferSingle s) (Map.tryFind (V s) knownTypes)]
    |Define(s,b) ->
      let s_declared,b_declared=
        match Map.tryFind e knownTypes with
        |Some(Function(ts,tb)) -> Some ts,Some tb
        |Some(Generic _) | None -> None,None
        |Some _ -> failwith "function doesn't make sense in this context"
      let s_type=mergeTypesOption (nextUnusedGeneric()) s_declared
      let b_types=infer (Map.add (V s) s_type knownTypes) b
      let s_type'=mergeTypesOption s_type (Map.tryFind (V s) b_types)
      b_types.Add(e,Function(s_type',b_types.[b])).Add(V s,s_type')
    |Bind(s,b,r) ->
      let s_type=mergeTypesOption (nextUnusedGeneric()) (Map.tryFind (V s) knownTypes)
      let b_types=infer (Map.add (V s) s_type knownTypes) b
      if b_types.ContainsKey (V s) && b_types.[V s]<>b_types.[b] then
        failwith "infinite type"
      let r_types=infer (Map.add (V s) b_types.[b] knownTypes) r
      map_merge b_types r_types
       |> Map.add (V s) b_types.[b]
       |> Map.add e r_types.[r]
    |Apply(a,b) ->
      let b_type=mergeTypesOption (nextUnusedGeneric()) (Map.tryFind b knownTypes)
      let a_type=
        match Map.tryFind e knownTypes with
        |Some z -> mergeTypesOption (Function(b_type,z)) (Map.tryFind a knownTypes)
        |None -> mergeTypesOption (Function(b_type,nextUnusedGeneric())) (Map.tryFind a knownTypes)
      let a_types,b_types=infer (knownTypes.Add(a,a_type)) a,infer (knownTypes.Add(b,b_type)) b
      match a_types.[a],b_types.[b] with
      |Function(x,xx),y ->
        let r_types=
          map_merge
           (infer (knownTypes.Add(a,Function(mergeTypes x y,xx))) a)
           (infer (knownTypes.Add(b,mergeTypes x y)) b)
        match r_types.[a] with Function(_,xx) -> r_types.Add(e,xx)
      |_ -> failwith "function application type union failed"
    |Condition(a,b,c) ->
      let a_types=infer (knownTypes.Add(a,A "bool")) a
      let b_types=infer knownTypes b
      let c_types=infer knownTypes c
      let res_type=mergeTypesOption (mergeTypes b_types.[b] c_types.[c]) (Map.tryFind e knownTypes)
      map_merge
       (infer (knownTypes.Add(b,res_type)) b)
       (infer (knownTypes.Add(c,res_type)) c)
       |> map_merge a_types
       |> Map.add e res_type
  adjustNaming Map.empty >> infer Map.empty
 