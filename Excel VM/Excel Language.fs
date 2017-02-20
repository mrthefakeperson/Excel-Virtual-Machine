//undo dynamic typing: integers are actually a thing, as it turns out
//`not`
//error for circular references
//more detailed error states

module Excel_Language
open System

type Combinator=
  |Plus     = 0
  |Minus    = 1
  |Mul      = 2
  |Div      = 3
  |Mod      = 4
  |Less             = 5
  |LessOrEqual      = 6
  |Equal            = 7
  |Unequal          = 8
  |Greater          = 9
  |GreaterOrEqual   = 10
  |And      = 11
  |Or       = 12
let combString (x:Combinator)=
  [|"+"; "-"; "*"; "/"; "MOD"; "<"; "<="; "="; "<>"; ">"; ">="; "AND"; "OR";|]
   .[int x]
type Formula=
  |Literal of string
  |Reference of cellName:string
  |Range of topLeft:string*bottomRight:string       //////add this
  |If of Formula*Formula*Formula
  |Choose of Formula*Formula[]               /////////using Index(range, k) is better (Choose(,) doesn't accept ranges, or rather treats them as single objects)
  |Index of Formula*Formula
  |Combine of Formula*Combinator*Formula
  |Concatenate of Formula[]
   with
    member x.hasReference()=
      match x with
      |Literal _ -> false
      |Reference _ | Range _ -> true
      |If(a,b,c) -> a.hasReference() || b.hasReference() || c.hasReference()
      |Choose(a,b) -> a.hasReference() || Array.exists (fun (e:Formula) -> e.hasReference()) b
      |Index(a, b) -> a.hasReference() || b.hasReference()
      |Combine(a,_,b) -> a.hasReference() || b.hasReference()
      |Concatenate a -> Array.exists (fun (e:Formula) -> e.hasReference()) a
and Cell =
  Cell of name:string*Formula
   with
    override x.ToString()=
      match x with
      Cell(_,formula) ->
        let rec pr = function
          |Literal v when Int32.TryParse(v, ref 0) -> sprintf "%s" v
          |Literal v -> sprintf "\"%s\"" v
          |Reference s -> s
          |Range(a, b) -> sprintf "%s:%s" a b
          |If(cond,aff,neg) -> sprintf "IF(%s,%s,%s)" (pr cond) (pr aff) (pr neg)
          |Choose(n,list) ->
            sprintf "CHOOSE(%s,%s)" (pr n) (String.concat "," (Array.map pr list))
          |Index(list, k) -> sprintf "INDEX(%s,%s)" (pr list) (pr k)
          |Combine(a, (Combinator.Mod | Combinator.And | Combinator.Or as u), b) ->
            sprintf "%s(%s,%s)" (combString u) (pr a) (pr b)
          |Combine(a, u, b) -> sprintf "(%s)%s(%s)" (pr a) (combString u) (pr b)
          |Concatenate list -> sprintf "CONCATENATE(%s)" (String.concat "," (Array.map pr list))
        sprintf "=%s" (pr formula)
let cmb c a b = Combine(a, c, b)
let (+.), (-.), ( *. ), (/.), (%.), (<=.), (=.), (>.), (&&.), (||.) =
  cmb Combinator.Plus, cmb Combinator.Minus, cmb Combinator.Mul, cmb Combinator.Div, cmb Combinator.Mod,
  cmb Combinator.LessOrEqual, cmb Combinator.Equal, cmb Combinator.Greater,
  cmb Combinator.And, cmb Combinator.Or

let alphaList =
  let nextAlpha (s:string) =
    if s.Replace("Z", "") = ""
     then String.init (s.Length + 1) (fun _ -> "A")
     else
      let a = s.ToCharArray()
      let rec incr i =
        if a.[i] = 'Z'
         then
          a.[i] <- 'A'
          incr (i-1)
         else
          a.[i] <- a.[i] + char 1
      incr (s.Length - 1)
      String a
  Array.init 50000 ((fun acc e ->
    let yld = !acc
    acc := nextAlpha !acc
    yld
   ) (ref ""))
let arcAlphaList = dict (Array.mapi (fun i e -> (e, i)) alphaList)

let numberToAlpha e = alphaList.[e]
let alphaToNumber e = arcAlphaList.[e]
let separate s =
  let rec separate s i =
    if i >= String.length s then failwith "could not separate"
    if '0' <= s.[i] && s.[i] <= '9' then (s.[..i-1], s.[i..]) else separate s (i+1)
  separate s 0
let number = separate >> snd >> int     //extract row
let alpha = separate >> fst >> alphaToNumber   //extract col
let coordinates s = number s, alpha s   //(row, col)
let coordsToS (r, c) = numberToAlpha c + string r

type Interpreted =                //dynamic typing, except for errors
  |S of string
  |ERROR
let (|V|N|B|Error|) = function
  |S s ->
    match s.ToLower() with
    |"" -> N 0.
    |"true" -> B true | "false" -> B false
    |s when Double.TryParse(s, ref 0.) -> N (float s)
    |_ -> V (s.ToUpper())
  |ERROR -> Error
let error x message =
  //printfn "%s: %A" message x
  //ignore (stdin.ReadLine())
  ERROR
let combOp (x:Combinator) =
  let _N op a b =
    match a,b with
    |N a, N b -> S (string(op a b))
    |x -> error x "int cast"
  let _comp (op:System.IComparable->System.IComparable->bool) a b =
    match a,b with
    |N a, N b -> S (string(op a b))
    |B a, B b -> S (string(op a b))    //true > false is how this is evaluated
    |S a, S b -> S (string(op a b))  //default to comparing strings
    |x -> error x "string cast"
  let _B op a b =
    match a,b with
    |B a, B b -> S (string(op a b))
    |x -> error x "bool cast"
  let a = [|
    _N (+); _N (-); _N (*); _N (/)
    _comp (<); _comp (<=); _comp (=); _comp (<>); _comp (>); _comp (>=)
    _B (&&); _B (||)
   |]
  a.[int x]
let inline ret e = S ((string e).ToUpper())
let getRange = function
  |Range(a, b) ->
    let (r1, c1), (r2, c2) = coordinates a, coordinates b
    [|for r in r1..r2 do for c in c1..c2 -> Reference(coordsToS (r, c))|]
  |_ -> failwith "couldn't get range"

let interpret iterations maxChange cellNames (cells:Formula[]) =
  let buffer = Array.create cells.Length ERROR //(ret 0)
  buffer.[0] <- ret 0
  let cellsByName = dict (Array.mapi (fun i e -> (e,i)) cellNames)
  let getCell e = try buffer.[cellsByName.[e]] with x -> failwithf "getCell %A: %A" e x
  let rec evaluateCell = function
    |Literal v -> ret v
    |Reference s -> getCell s
    |Range _ -> failwith "standalone range object not supported"
    |If(cond,aff,neg) ->
      match evaluateCell cond with
      |B true -> evaluateCell aff
      |B false -> evaluateCell neg
      |x -> error x "conditional expression"
    |Choose(n,choices) ->
      match evaluateCell n with
      |N i when 1 <= int i && int i <= choices.Length -> evaluateCell choices.[int i-1]
      |x -> error x "choice"
    |Index(range, k) ->
      match range with
      |Range(a, b) ->
        match coordinates a, coordinates b, evaluateCell k with
        |(r1, c1), (r2, c2), N i when int i < 1 || int i > (r2-r1+1)*(c2-c1+1) -> error i "index out of bounds"
        |(r1, c1), (r2, c2), N i when c1 = c2 -> evaluateCell (Reference(coordsToS (r1+int i-1, c1)))
        |(r1, c1), (r2, c2), N i when r1 = r2 -> evaluateCell (Reference(coordsToS (r1, c1+int i-1)))
        |_ -> evaluateCell (Choose(k, getRange range))
      |_ -> evaluateCell (Choose(k, getRange range))
    |Combine(a,comb,b) -> combOp comb (evaluateCell a) (evaluateCell b)
    |Concatenate a ->
      Array.fold (fun acc e ->
        match acc, evaluateCell e with
        |S a, S b -> S (a + b)
        |x -> error x "concatenation"
       ) (S "") a
  let variables, constants =
    Array.mapi (fun i e -> i, e) cells
     |> Array.partition (fun (i, e:Formula) -> e.hasReference())
  Array.iter (fun (i, e) -> buffer.[i] <- evaluateCell e) constants
  match variables with
  |[||] -> ()
  |_ ->
    let i, primary = variables.[0]         //don't remember whether the first cell gets evaluated once or twice
    buffer.[i] <- evaluateCell primary     //it seems to be evaluated once
    for e in 1..iterations do
      Array.iter (fun (i, e) ->
        buffer.[i] <- evaluateCell e
       ) variables
  Array.zip cellNames buffer
