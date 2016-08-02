module AST

type Expr=
  |V of string
  |Define of string*Expr
  |Bind of string*Expr
  |Apply of Expr*Expr
  |Condition of Expr*Expr*Expr
type Token=
  { t:Expr
    row:int
    col:int }
 with
  override x.ToString()=
    sprintf "(%A at %A,%A)" x.t x.row x.col
let (|T|_|)=function {t=V s} -> Some(T s) | _ -> None
let merge combine ({t=tA} as a) {t=tB}={a with t=combine tA tB}
let indented ({row=x; col=y} as a) {t=b; row=xx; col=yy} = xx>=x && yy>y
let indent ({row=x; col=y} as a) {t=b; row=xx; col=yy} = {a with t=b}
let finished a = function
  |[] | T(")" | "then" | "else" | "do")::_ -> true     //do shouldn't be here (it should take the indentation of ``while`` and ``for``)
  |b::_ -> not (indented a b)
let (|Ignore|Special|Literal|Variable|Prefix|Infix|) tk=
  let s=match tk with T x -> x | _ -> ""
  match s with
  |"" -> Ignore
  |"(*" | "*)" | "<-" | "\n" | ";" | " " | "(" | ")" -> Special tk
  |_ when
    (s.Length>=3 && s.[0]='(' && s.[s.Length-1]=')') ||
    (('A'<=s.[0] && s.[0]<='_') || ('a'<=s.[0] && s.[0]<='z')) &&
    String.forall (fun e->
      List.exists ((=) e) (['0'..'9'] @ ['A'..'Z'] @ ['a'..'z'] @ ['_'; '\''])
     ) s -> Variable tk
  |_ when s.[0]='~' -> Prefix tk
  |_ when
    String.forall (fun e->
      List.exists ((=) e) (['!'; '#'; '%'; '&'] @ ['*'..'/'] @ [':'] @ ['<'..'@'] @ ['^'])
     ) s -> Infix tk
  |_ -> Literal tk     //negative numbers not supported yet
let priorityOfOperation (T s)=
  match s with
  |"+" | "-" -> 0
  |"*" | "/" | "**" | "%" | "&&" -> 1
  |_ -> 0


