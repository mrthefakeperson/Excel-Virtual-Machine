module FSharp_Parser
open AST

(*
  rest :: Bind(name,body) :: restr -> Apply(Bind(name,body),rest) as a special case
    -``rest`` ends when a finisher is detected and the finisher has a strictly lower priority than the Bind
    let name =
      body
    rest
  finished

  if true then 5 else 6
   + 4
    -``+`` aligns to ``true`` or ``6``

  1 + id
   1
  evaluates to 2
*)
let (|Ignore|Special|Literal|Variable|Prefix|Infix|) s =
  match s with
  |"" -> Ignore
  |"(*" | "*)" | "<-" | "\n" | ";" | " " | "(" | ")" | "->" -> Special s
  |_ when
    (s.Length>=3 && s.[0]='(' && s.[s.Length-1]=')') ||
    (('A'<=s.[0] && s.[0]<='_') || ('a'<=s.[0] && s.[0]<='z')) &&
    String.forall (fun e->
      List.exists ((=) e) (['0'..'9'] @ ['A'..'Z'] @ ['a'..'z'] @ ['_'; '\''; '$'])
     ) s -> Variable s
  |_ when s.[0]='~' -> Prefix s
  |_ when
    String.forall (fun e->
      List.exists ((=) e) (['!'; '#'; '%'; '&'] @ ['*'..'/'] @ [':'] @ ['<'..'@'] @ ['^'])
     ) s ->
    match s with
    |"*" | "/" | "**" | "%" | "&&" -> Infix (s,1)
    |"|>" | "<|" | ">>" | "<<" -> Infix (s,-1)
    |_ -> Infix (s,0)
  |_ -> Literal s     //negative numbers not supported yet

type Construct =
  |Single of string
  |Apply of Construct*Construct
  |Fun of Construct*Construct
  |Let of Construct*Construct*Construct
  |While of Construct*Construct
  |If of Construct*Construct*Construct
type Token =
  { t:Construct
    row:int
    col:int  }
let (|T|_|) = function {t=Single s} -> Some s | _ -> None
///parse F# code for its abstract syntax tree
let parseSyntax (text:string)=
  let tokenized=
    let isUnfinishedString = function
      |"\"" -> true
      |s -> s.[0] = '\"' && s.[s.Length-1] <> '\"'
    text.ToCharArray()
     |> Array.fold (fun (row,col,acc)->function
          |'\t' -> failwith "no tabs allowed"
          |'\r' -> row,col,acc
          |'\n' -> row+1,1,{t=Single "\n"; row=row; col=col}::acc
          |e -> row,col+1,{t=Single(string e); row=row; col=col}::acc
         ) (1,1,[])
     |> fun (_,_,e) -> List.rev e
     |> List.fold (fun tokens e->
          match tokens,e with
          //escape sequences don't work
          |(T s as a)::rest,T b when isUnfinishedString s -> {a with t=Single(s+b)}::rest
          |T(Variable a)::rest,T(Variable b)
          |T(Prefix a | Infix (a,_))::rest,T(Infix (b,_))
          |T(Literal a)::rest,T(Literal b) -> {tokens.Head with t=Single(a+b)}::rest
          |_,T Ignore -> tokens
          |_ -> e::tokens
         ) []
     |> List.rev
     |> List.filter (function (T " " | T "\n") -> false | _ -> true)

  let rec parse left right=
    let (|EndRule|_|) = function [{t=a}],[] -> Some a | _ -> None
    let (|BeginRule|_|) = function [],a::restr -> Some BeginRule | _ -> None
    let (|Context1|_|) = function
      |T("(" | "let" | "if" | "while" | "fun" | Infix _)::_ -> Some Context1
      |_ -> None
    let (|Context2|_|) =
      let ctx = function "let","=" | "while","do" | "if","then" | "fun","->" -> true | _ -> false
      function
      |T b::_::(T a as x)::_,_
      |_::(T a as x)::_,T b::_ when ctx (a,b) -> Some x
      |_ -> None
    let (|Context3|_|) =
      let ctx = function "if","then","else" -> true | _ -> false
      function
      |T c::_::T b::_::(T a as x)::_,_
      |_::T b::_::(T a as x)::_,T c::_ when ctx (a,b,c) -> Some x
      |_ -> None
    let (|DelimN1|_|) =
      let lims = function "(",")" -> true | _ -> false
      function
      |(T a as x)::_,(T b as y)::_ when lims (a,b) -> Some {x with t=Single (a+b)}
      |_ -> None
    let (|DelimN2|_|) =
      function
      |x::T"("::restl,T")"::restr -> Some (restl,x::restr)
      |_ -> None
    let (|DelimX|_|) (l,r) =
      let finished a = function
        |[] | T(")" | "do" | "then" | "else")::_ -> true
        |b::_ -> b.row>a.row && b.col<=a.col
      match l,r with
      |_::T"="::_::(T"let" as a)::_,b::r when a.col=b.col ->
        Some ((b::l),r)
      |rs::d::T"="::s::(T"let" as a)::l,_ when finished {a with col=a.col-1} r ->
        Some (l,{a with t=Let(s.t,d.t,rs.t)}::r)
      |e::T"do"::c::(T"while" as a)::l,_ when finished a r ->
        Some (l,{a with t=While(c.t,e.t)}::r)
      |t::T"then"::c::(T"if" as a)::l,_ when finished a r ->
        Some (l,{a with t=If(c.t,t.t,Single "()")}::r)
      |e::T"else"::t::T"then"::c::(T"if" as a)::l,_ when finished a r ->
        Some (l,{a with t=If(c.t,t.t,e.t)}::r)
      |x::T"->"::e::(T"fun" as a)::l,_ when finished a r ->
        Some (l,{a with t=Fun(e.t,x.t)}::r)
      |c::(T(Infix (_,p)) as b)::a::l,T(Infix (_,p2))::_ when p>=p2 ->
        Some (l,{a with t=Apply(Apply(b.t,a.t),c.t)}::r)
      |c::(T(Infix (_,p)) as b)::a::l,_ when finished a r ->
        Some (l,{a with t=Apply(Apply(b.t,a.t),c.t)}::r)

      |_ -> None
    //printfn "%A | %A" (List.map (fun {t=t} -> t) left) (List.map (fun {t=t} -> t) right)
    //ignore (System.Console.ReadLine())
    let indent a b = {a with t=b.t}
    match (left,right) with
    |[],[] -> failwith "nothing to parse"
    |EndRule a -> a
    //delimiter rules (incl and excl)
    |DelimN1 x ->
      parse left.Tail (x::right.Tail)
    |DelimN2 (l,r) -> parse l r
    |DelimX (l,r) -> parse l r
    //push rules
    |BeginRule
    |_,Context1
    |Context1,_::_ ->
      parse (right.Head::left) right.Tail
    |(T"fun" as a)::b::_,_ -> parse ({b with t=a.t}::left.Tail) right
    |Context2 x
    |Context3 x ->
      parse (indent x right.Head::left) right.Tail
    //application rules, including infixes
    |a::restl,b::restr -> parse restl ({a with t=Apply(a.t,b.t)}::restr)

(*
    |_::T"="::_::(T"let" as a)::_,b::restr when a.col=b.col->
      parse (b::left) restr
    //brackets
    |Infix(T s as a)::T"("::restl,T")"::restr -> parse ({a with t=V("("+s+")")}::restl) restr
     -> parse restl (a::restr)
    //let statements
    |T"let"::_,T"rec"::restr -> parse left restr
    //prefix operators
    |Infix _::_,(Infix(T fA) as a)::restr -> parse ({a with t=V("~"+fA)}::left) restr
    //infix operators
    |a::_,Infix b::restr -> parse (indent a b::left) restr
    |Infix d::c::Infix(T s as a)::b::restl,_ when priorityOfOperation a>=priorityOfOperation d->
      parse restl ({a with t=Apply(Apply(V("("+s+")"),b.t),c.t)}::d::right)
    |c::Infix(T s as a)::b::restl,_ when finished {b with col=b.col-1} right->
      parse restl ({a with t=Apply(Apply(V("("+s+")"),b.t),c.t)}::right)
    |Infix _::_,a::restr -> parse (a::left) restr
    |a::restl,(Variable b | Literal b)::_ when a.col=b.col->
      parse (a::{a with t=V"="}::{a with t=V"_"}::{a with t=V"let"}::restl) right
    //function application
    |({t=fA} as a)::restl,({t=fB} as b)::restr when indented a b->
      parse restl ({a with t=Apply(fA,fB)}::restr)
*)
    |_->
      printfn "unknown expression"
      List.iter (printfn "%O") left
      List.iter (printfn "%O") right
      ignore (System.Console.ReadLine())
      failwith "unknown expression"

  parse [] tokenized

///represents different syntax modes/styles
type rewriteMode=
  |Brief
  |Light
  //|Verbose
///write F# code to evaluate an abstract syntax tree construct
let rec rewrite = function
  |Brief -> function
    |Single s -> s
    |Apply(a,b) -> sprintf "(%s %s)" (rewrite Brief a) (rewrite Brief b)
    |Let(a,b,c) -> sprintf "let %s = %s in %s" (rewrite Brief a) (rewrite Brief b) (rewrite Brief c)
    |If(a,b,c) -> sprintf "(if %s then %s else %s)" (rewrite Brief a) (rewrite Brief b) (rewrite Brief c)
    |While(a,b) -> sprintf "(while %s do %s);" (rewrite Brief a) (rewrite Brief b)
    |Fun(a,b) -> sprintf "(fun %s -> %s)" (rewrite Brief a) (rewrite Brief b)
  |Light->
    let rec rewrite' indent=function
      |Single s -> s
      |Fun(a,b) ->
        sprintf "(fun %s ->\n%s  %s\n%s )" (rewrite' (indent+"  ") a) indent (rewrite' (indent+"  ") b) indent
      |Let(a,b,c) ->
        sprintf "let %s =\n%s  %s\n%s%s"
         (rewrite' (indent+"  ") a) indent (rewrite' (indent+"  ") b) indent (rewrite' indent c)
      |Apply(a,b) -> sprintf "(%s %s)" (rewrite' indent a) (rewrite' indent b)
      |If(a,b,c) ->
        sprintf "(\n%sif %s\n%s then\n%s  %s\n%s else\n%s  %s\n%s)"
         indent (rewrite' indent a) indent indent
         (rewrite' (indent+"  ") b) indent indent
         (rewrite' (indent+"  ") c) indent
      |While(a,b) ->
        sprintf "(while %s do\n%s  %s);" (rewrite' (indent+"  ") a) indent (rewrite' (indent+"  ") b)
    rewrite' ""
