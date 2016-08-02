module Excel_Language

type Comparison=
  |Less             = 0
  |LessOrEqual      = 1
  |Equal            = 2
  |Unequal          = 3
  |Greater          = 4
  |GreaterOrEqual   = 5
let compString (x:Comparison)=
  let a=[|"<"; "<="; "="; "<>"; ">"; ">="|]
  a.[int x]
let compOp (x:Comparison)=
  let a=[|(<); (<=); (=); (<>); (>); (>=)|]
  a.[int x]
type Combinator=
  |Plus     = 0
  |Minus    = 1
  |Mul      = 2
  |Div      = 3
let combString (x:Combinator)=
  let a=[|"+"; "-"; "*"; "/"|]
  a.[int x]
let combOp (x:Combinator)=
  let a=[|(+); (-); (*); (/)|]
  a.[int x]
type Formula=
  |Literal of Choice<int,string,bool>
  |Reference of cellName:string
  |If of Formula*Formula*Formula
  |Choose of Formula*Formula[]
  |Compare of Formula*Comparison*Formula
  |Combine of Formula*Combinator*Formula
  |Concatenate of Formula[]
   with
    member x.hasReference()=
      match x with
      |Literal _ -> false
      |Reference _ -> true
      |If(a,b,c) -> a.hasReference() || b.hasReference() || c.hasReference()
      |Choose(a,b) -> a.hasReference() || Array.exists (fun (e:Formula) -> e.hasReference()) b
      |Compare(a,_,b) | Combine(a,_,b) -> a.hasReference() || b.hasReference()
      |Concatenate a -> Array.exists (fun (e:Formula) -> e.hasReference()) a
and Cell=
  Cell of name:string*Formula
   with
    override x.ToString()=
      match x with
      Cell(_,formula) ->
        let rec pr=function
          |Literal v ->
            match v with
            |Choice1Of3 v -> string v
            |Choice2Of3 v -> sprintf "\"%s\"" v
            |Choice3Of3 v -> (string v).ToUpper()
          |Reference s -> s
          |If(cond,aff,neg) -> sprintf "IF(%s,%s,%s)" (pr cond) (pr aff) (pr neg)
          |Choose(n,list) ->
            sprintf "CHOOSE(%s,%s)" (pr n) (String.concat "," (Array.map pr list))
          |Compare(a,u,b) -> sprintf "(%s)%s(%s)" (pr a) (compString u) (pr b)
          |Combine(a,u,b) -> sprintf "(%s)%s(%s)" (pr a) (combString u) (pr b)
          |Concatenate list -> sprintf "CONCATENATE(%s)" (String.concat "," (Array.map pr list))
        sprintf "=%s" (pr formula)