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
///parse F# code for its abstract syntax tree
let parseSyntax (text:string)=
  let collapse (V a) (V b)=V(a+b)
  let merge'=merge collapse
  let tokenized=
    text.ToCharArray()
     |> Array.fold (fun (row,col,acc)->function
          |'\t' -> failwith "no tabs allowed"
          |'\r' -> row,col,acc
          |'\n' -> row+1,1,{t=V "\n"; row=row; col=col}::acc
          |e -> row,col+1,{t=V(string e); row=row; col=col}::acc
         ) (1,1,[])
     |> fun (_,_,e) -> List.rev e
     |> List.fold (fun tokens e->
          match tokens,e with
          |(Literal a | Variable a)::rest,Variable b
          |(Prefix a | Infix a)::rest,Infix b
          |Literal a::rest,Literal b -> merge' a b::rest
          |_,Ignore -> tokens
          |_ -> e::tokens
         ) []
     |> List.rev
     |> List.filter (function (T " " | T "\n") -> false | _ -> true)
  //printfn "%A" tokenized
  let rec parse left right=
    //printfn "%O | %O" left right
    //ignore (System.Console.ReadLine())
    match (left,right) with
    |[],[] -> failwith "nothing to parse"
    //end rule
    |[{t=a}],[] -> a
    //push rules            (InCoMpLeTe: carry the indentation over for multi-token expressions)
    |[],a::restr    //(begin rule)
    |_,(T"(" | T"let" | T"if" | T"while" as a)::restr
    |(T"(" | T"let" | T"if" | T"while")::_,a::restr->
      parse (a::left) restr
    |_::(T"let" as a)::_,(T"=" as b)::restr -> parse (indent a b::left) restr
    |T"="::_::T"let"::_,a::restr->     //incomplete statements not supported
      parse (a::left) restr
    |a::_,(T"fun" as b)::restr -> parse (indent a b::left) restr
    |T"fun"::_,a::restr
    |_::T"fun"::_,(T"->" as a)::restr
    |T"->"::_::T"fun"::_,a::restr->
      parse (a::left) restr
    |_::(T"if" as a)::_,(T"then" as b)::restr
    |T"then"::_::(T"if" as a)::_,b::restr
    |_::T"then"::_::(T"if" as a)::_,(T"else" as b)::restr
    |T"else"::_::T"then"::_::(T"if" as a)::_,b::restr->
      parse (indent a b::left) restr
    |_::T"while"::_,(T"do" as a)::restr
    |T"do"::_::T"while"::_,a::restr->
      parse (a::left) restr
    |_::T"="::_::(T"let" as a)::_,b::restr when a.col=b.col->
      parse (b::left) restr
    //|(Bind(_,_,_) as a)::_,b::restr-> this case should never occur, since Bind requires a finishing state
//    |({t=Bind(_,_)} as a)::_,b::restr when a.col=b.col -> parse (b::left) restr
    //brackets
    |(T")" as a)::(T"(" as b)::restl,_->
      parse restl (merge' b a::right)
    |Infix(T s as a)::T"("::restl,T")"::restr -> parse ({a with t=V("("+s+")")}::restl) restr
    |a::T"("::restl,T")"::restr -> parse restl (a::restr)
    //let statements
    |T"let"::_,T"rec"::restr -> parse left restr
    |r::b::T"="::T s::(T"let" as a)::restl,_ when finished {a with col=a.col-1} right->
      parse restl ({a with t=Bind(s,b.t,r.t)}::right)
//    |{t=c}::T"="::T s::(T"let" as a)::restl,b::_ when not(indented a b)->
//      parse restl ({a with t=Bind(s,c)}::right)
    //anonymous functions
    |{t=c}::T"->"::T s::(T"fun" as a)::restl,_ when finished a right->
      parse restl ({a with t=Define(s,c)}::right)
    //ifs
    |{t=c}::T"else"::{t=d}::T"then"::{t=e}::(T"if" as a)::restl,_ when finished a right->
      parse restl ({a with t=Condition(e,d,c)}::right)
    |{t=c}::T"then"::{t=d}::(T"if" as a)::restl,_ when finished a right->
      parse restl ({a with t=Condition(d,c,V"()")}::right)
    //whiles
    |{t=c}::T"do"::{t=d}::(T"while" as a)::restl,_ when finished a right->
      Bind("loop$",
        Define("()",Bind("_",c,Condition(d,Apply(V "loop$",V "()"),V "()"))),
        Apply(V "loop$",V "()")
       )
//      Apply(
//        Bind("loop-=",     //when detecting type of variable, this will appear as Literal
//          Define("()",Apply(Bind("_",c),Condition(d,Apply(V "loop-=",V "()"),V "()")))
//         ),
//        Apply(V "loop-=",V "()")
//       )
    //prefix operators
    |Infix _::_,(Infix(T fA) as a)::restr -> parse ({a with t=V("~"+fA)}::left) restr
    //infix operators
    |a::_,Infix b::restr -> parse (indent a b::left) restr
    |Infix d::c::Infix(T s as a)::b::restl,_ when priorityOfOperation a>=priorityOfOperation d->
      parse restl ({a with t=Apply(Apply(V("("+s+")"),b.t),c.t)}::d::right)
    |c::Infix(T s as a)::b::restl,_ when finished {b with col=b.col-1} right->
      parse restl ({a with t=Apply(Apply(V("("+s+")"),b.t),c.t)}::right)
    |Infix _::_,a::restr -> parse (a::left) restr
    //expression separation
//    |a::restl,T";"::restr -> parse restl ({a with t=Bind("_",a.t)}::restr)
//             ^ no replacement for the above yet
//    |a::({t=Bind(_,_)} as b)::restl,_ when finished a right->  // && a.col<>right.Head.col is implicit
//             ^ shouldn't happen anymore
//      parse restl ({b with t=Apply(b.t,a.t)}::right)
    |a::restl,(Variable b | Literal b)::_ when a.col=b.col->
      parse (a::{a with t=V"="}::{a with t=V"_"}::{a with t=V"let"}::restl) right
//      parse restl ({a with t=Bind("_",a.t)}::right)
    //function application
    |({t=fA} as a)::restl,({t=fB} as b)::restr when indented a b->
      parse restl ({a with t=Apply(fA,fB)}::restr)

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
let rec rewrite=function
  |Brief->function
    |V e -> e
    |Define(s,a) -> sprintf "(fun %s -> %s)" s (rewrite Brief a)
    |Bind(s,b,r) -> sprintf "let %s = %s in %s" s (rewrite Brief b) (rewrite Brief r)
    |Apply(a,b) -> sprintf "(%s %s)" (rewrite Brief a) (rewrite Brief b)
    |Condition(a,b,c)->
      sprintf "(if %s then %s else %s)" (rewrite Brief a) (rewrite Brief b) (rewrite Brief c)
  |Light->
    let rec rewrite' indent=function
      |V e -> e
      |Define(s,a) -> sprintf "(fun %s ->\n%s  %s\n%s )" s indent (rewrite' (indent+"  ") a) indent
      |Bind(s,b,r) -> sprintf "let %s =\n%s  %s\n%s%s" s indent (rewrite' (indent+"  ") b) indent (rewrite' indent r)
//      |Apply(Bind(_,_) as a,b) -> sprintf "%s\n%s%s" (rewrite' indent a) indent (rewrite' indent b)
      |Apply(a,b) -> sprintf "(%s %s)" (rewrite' indent a) (rewrite' indent b)
      |Condition(a,b,c)->
        sprintf "(\n%sif %s\n%s then\n%s  %s\n%s else\n%s  %s\n%s)"
         indent (rewrite' indent a) indent indent
         (rewrite' (indent+"  ") b) indent indent
         (rewrite' (indent+"  ") c) indent
    rewrite' ""
