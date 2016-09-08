module Python_2_Parser
open AST

type Construct =
  |Single of string
  |Tuple of Construct list
  |Assign of Construct*Construct
  |Apply of Construct*Construct list
  |If of Construct*Construct list*else_optional:Construct list
  |Else of Construct list
  |While of Construct*Construct list
  |Def of Construct*Construct list

let parseSyntax (text:string) =
  let getLeadingSpace (line:string) =
    line.ToCharArray()
     |> Array.fold (fun acc e ->
          if acc>=0
           then acc
           else
            match e with
            |' ' -> acc+1
            |'\t' -> acc+4
            |_ -> acc+1000000
         ) -1000000      //1000000 is used as a flag for if a non-whitespace character has been found
  let (|Special|Literal|Variable|Infix|) = function
    |" " | ";" | "\"" | "'" | "(" | ")" -> Special
    |s when
      (('A'<=s.[0] && s.[0]<='_') || ('a'<=s.[0] && s.[0]<='z')) &&
      String.forall (fun e->
        List.exists ((=) e) (['0'..'9'] @ ['A'..'Z'] @ ['a'..'z'] @ ['_'; '\''; '$'])
       ) s -> Variable
    |s when
      String.forall (fun e->
        List.exists ((=) e) (['!'; '#'; '%'; '&'] @ ['*'..'/'] @ [':'] @ ['<'..'@'] @ ['^'])
       ) s ->
      match s with
      |"*" | "/" | "**" | "%" -> Infix 1
      |_ -> Infix 0
    |_ -> Literal     //negative numbers not supported yet
  let scan (text:string) =
    text.ToCharArray()
     |> Array.fold (fun acc e ->
          match acc,string e with
          |[],_ -> [string e]
          |hd::tl,e ->
            match hd,e with
            |_,"\r" -> acc
            |Literal,Literal | Variable,Variable | Infix _,Infix _ -> hd+e::tl
            |_ -> e::acc
         ) []
     |> List.rev
     |> List.fold (fun (delim,acc) e ->
          match delim,acc with
          |_,[] -> None,[e]
          |_,esc::tl when esc.[esc.Length-1]='\\' && (esc="\\" || not(esc.[esc.Length-2]='\\')) ->
            delim,esc+string e::tl     // \ \n
          |Some d,hd::tl when e=d -> None,sprintf "\"%s\"" hd.[1..]::tl
          |None,_ when e="\"" || e="'" -> Some e,string e::acc
          |Some _,hd::tl -> delim,hd+string e::tl
          |_ -> delim,string e::acc
         ) (None,[])
     |> (snd >> List.rev)
     |> List.fold (fun (layers,acc) e ->
          match layers,acc,e with
          |_,[],_ -> layers,e::acc
          |_,_,"(" -> layers+1,e::acc
          |0,_,")" -> failwith "invalid closing bracket"
          |_,_,")" -> layers-1,e::acc
          |x,_,"\n" when x>0 -> layers,acc
          |_ -> layers,e::acc
         ) (0,[])
     |> (snd >> List.rev)
  let rec parse left right =
    let (|T|_|) = function Single s -> Some s | _ -> None
    let (|Statement|_|) = function T("if" | "else" | "while" | "def" | "return" | "print")::_ -> Some Statement | _ -> None
    let (|Finished|_|) = function
      |[] | T(")" | ":" | "," | "if" | "else")::_ -> Some Finished
      |_ -> None
    let (|EndRule|_|) = function [a],[] -> Some a | _ -> None
    let (|BeginRule|_|) = function [],x::tl -> Some BeginRule | _ -> None
    let (|Context1|_|) =
      let (|Statement'|_|) = function
        |_,Statement | Statement,_ -> Some Statement'
        |_ -> None
      let (|EndStatement|_|) = function
        |[If(_,[],[]) | Else [If(_,[],[])] | Else [] | While(_,[]) | Def(_,[])],_::_ -> Some EndStatement
        |_ -> None
      let (|Ctx|_|) = function
        |[_],T"if"::_ | [T"if"; _],_ | [_; T"if"; _],T"else"::_ | [T"else"; _; T"if"; _],_
        |_,T("(" | "=" | ",")::_ | T("(" | "=" | ",")::_,_ -> Some Ctx
        |_ -> None
      function
      |Statement'
      |EndStatement
      |Ctx -> Some Context1
      |a::T(Infix m)::b::_,T(Infix n)::_ when n>m -> Some Context1
      |_ -> None
    let (|Delim1|_|) =
      let (|Statement|_|) = function
        |x::(Statement & st::_),T":"::_ ->
          Some (
            match st with
            |T "if" -> If(x, [], [])
            |T "elif" -> Else [If(x, [], [])]
            |T "while" -> While(x, [])
            |T "def" -> Def(x, [])
           )
        |(Statement & st::_),T":"::_ ->
          Some (
            match st with
            |T "else" -> Else []
           )
        |(Statement & T("return" | "print" as st)::_),[a] -> Some (Apply(Single st,[a]))
        |_ -> None
      let (|EndStatement|_|) = function
        |[a; If(st,[],[])],[] -> Some(If(st,[a],[]))
        |[a; Else [If(st,[],[])]],[] -> Some(Else [If(st,[a],[])])
        |[a; Else []],[] -> Some(Else [a])
        |[a; While(st,[])],[] -> Some(While(st,[a]))
        |[a; Def(st,[])],[] -> Some(Def(st,[a]))
        |_ -> None
      let (|EndCtx|_|) = function
        |b::T","::Tuple a::restl,(Finished as right) -> Some(restl,Tuple(b::a)::right)
        |b::T","::a::restl,(Finished as right) -> Some(restl,Tuple [b; a]::right)
        |Tuple x::T"("::y::restl,T")"::restr -> Some(restl,Apply(y,x)::restr)
        |x::T"("::y::restl,T")"::restr -> Some(restl,Apply(y,[x])::restr)    //x should only ever be a Single
        |b::T"="::a::restl,(Finished as right) -> Some(restl,Assign(a,b)::right)
        |T"("::restl,T")"::restr -> Some(restl,Single"()"::restr)
        |x::T"("::restl,T")"::restr -> Some(restl,x::restr)
        |neg::T"else"::cond::T"if"::aff::restl,(Finished as right) -> Some(restl,If(cond,[aff],[neg])::right)
        |_ -> None
      function
      |(Statement x & (_,_::tl)) -> Some([],x::tl)
      |EndStatement x -> Some([],[x])
      |EndCtx x -> Some x
      |_ -> None
    let (|InfixOperator|_|) = function
      |a::T(Infix m)::b::_,Finished -> Some InfixOperator
      |a::T(Infix m)::b::_,T(Infix n)::_ when m>=n -> Some InfixOperator
      |_ -> None

    match left,right with
    |[],[] -> failwith "nothing to parse"
    |EndRule a -> a
    |Delim1(l,r) -> parse l r
    |BeginRule
    |Context1 -> parse (right.Head::left) right.Tail
    |(InfixOperator & (a::c::b::restl,_)) -> parse restl (Apply(c,[b; a])::right)
    |_ -> failwithf "not recognized:\n\t%A\n\t\t%A" left right
    
  (String.concat "" (scan text)).Split '\n'
   |> Array.filter (fun e -> e.Trim() <> "")
   |> Array.collect (fun e ->
        e.Split ';'
         |> Array.map (fun ee -> List.choose (function " " -> None | e -> Some(Single e)) (scan ee),getLeadingSpace e)
       )
   |> fun e -> Array.append e [|[Single "0"],0|]
   |> Array.fold (fun acc (e,indent) ->
        let xpr = parse [] e
        let rec doStuff acc (xpr,indent) =
          let acc =
            match acc with
            |(Else neg::If(cond,aff,[])::rest,n)::tl -> (If(cond,aff,neg)::rest,n)::tl
            |_ -> acc
          match acc with
          |(something,indentPrev)::tl when indent>indentPrev -> ([xpr],indent)::acc
          |(something,indentPrev)::tl when indent=indentPrev -> (xpr::something,indent)::tl
          |(something,indentPrev)::tl when indent<indentPrev ->
            let lower =
              match tl with
              |(If(cond,[],[])::rest,n)::ttl -> (If(cond,something,[])::rest,n)::ttl
              |(If(cond,aff,[])::rest,n)::ttl -> (If(cond,aff,something)::rest,n)::ttl
              |(While(cond,[])::rest,n)::ttl -> (While(cond,something)::rest,n)::ttl
              |(Def(x,[])::rest,n)::ttl -> (Def(x,something)::rest,n)::ttl
            doStuff lower (xpr,indent)
        doStuff acc (xpr,indent)
       ) [([],0)]
   |> List.map fst
   |> List.head
   |> List.rev

let rec translate =
  let replace =
    dict [
      Single "True",V "true"
      Single "true",V "True"
      Single "False",V "false"
      Single "false",V "False"
      Single "print",AST.Apply(V "printf",V "\"%A\"")
     ]
  function
  |[] -> V "()"
  |hd::tl ->
    let translated =
      match hd with
      |x when replace.ContainsKey x -> replace.[x]
      |Single s -> V s
      |Tuple xprs -> Struct(Array.ofList (List.rev (List.map (fun e -> translate [e]) xprs)))
      |Assign(a,b) -> Bind(translate [a],translate [b],translate tl)
      |Apply(a,b) ->
        match b with
        |[arg] -> AST.Apply(translate [a],translate [arg])
        |hdd::ttl -> AST.Apply(translate [Apply(a,ttl)],translate [hdd])
      |If(cond,aff,neg) -> Condition(translate [cond],translate (List.rev aff),translate (List.rev neg))
      |While(cond,exprs) ->
        Bind(V "loop$",
          Define(V "()",
            Bind(V "_",translate (List.rev exprs),V "()")
           ),
          AST.Apply(V "loop$",V "()")
         )
      |Def(Apply(f,args),exprs) ->
        let rec doStuff exprs = function
          |hdd::ttl -> Define(translate [hdd],doStuff exprs ttl)
          |[] -> exprs
        let rec doMoreStuff = function
          |AST.Apply(V "return",x) -> x
          |Bind(a,b,c) -> Bind(a,b,doMoreStuff c)
          |Condition(a,b,c) -> Condition(a,doMoreStuff b,doMoreStuff c)
          |els -> Bind(V "_",els,V "()")
        Bind(translate [f],doStuff (doMoreStuff (translate exprs)) (List.rev args),translate tl)

    match tl,translated with
    |_,Bind(_,_,_)
    |[],_ -> translated
    |_ -> Bind(V "_",translated,translate tl)