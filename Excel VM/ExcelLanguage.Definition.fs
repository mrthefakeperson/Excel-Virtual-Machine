﻿module ExcelLanguage.Definition
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
  |Range of topLeft:string*bottomRight:string
  |If of Formula*Formula*Formula
  |Choose of Formula*Formula[]
  |Index of Formula*Formula
  |IndexStr of Formula*Formula
  |Combine of Formula*Combinator*Formula
  |Concatenate of Formula[]
  |Line_Break
   with
    member x.hasReference()=
      match x with
      |Literal _ -> false
      |Reference _ | Range _ -> true
      |If(a,b,c) -> a.hasReference() || b.hasReference() || c.hasReference()
      |Choose(a,b) -> a.hasReference() || Array.exists (fun (e:Formula) -> e.hasReference()) b
      |Index(a, b)
      |IndexStr(a, b)
      |Combine(a,_,b) -> a.hasReference() || b.hasReference()
      |Concatenate a -> Array.exists (fun (e:Formula) -> e.hasReference()) a
      |Line_Break -> false
and Cell =
  Cell of name:string*Formula
   with
    override x.ToString()=
      match x with
      Cell(_,formula) ->
        let rec pr = function
          |Literal v when fst (Int32.TryParse v) -> sprintf "%s" v
          |Literal v -> sprintf "\"%s\"" v
          |Reference s -> s
          |Range(a, b) -> sprintf "%s:%s" a b
          |If(cond,aff,neg) -> sprintf "IF(%s,%s,%s)" (pr cond) (pr aff) (pr neg)
          |Choose(n,list) ->
            sprintf "CHOOSE(%s,%s)" (pr n) (String.concat "," (Array.map pr list))
          |Index(list, k) -> sprintf "INDEX(%s,%s)" (pr list) (pr k)
          |IndexStr(s, i) -> sprintf "MID(%s,%s,1)" (pr s) (pr i)
          |Combine(a, (Combinator.Mod | Combinator.And | Combinator.Or as u), b) ->
            sprintf "%s(%s,%s)" (combString u) (pr a) (pr b)
          |Combine(a, u, b) -> sprintf "(%s)%s(%s)" (pr a) (combString u) (pr b)
          |Concatenate list -> sprintf "CONCATENATE(%s)" (String.concat "," (Array.map pr list))
          |Line_Break -> sprintf "CHAR(10)"
        sprintf "=%s" (pr formula)
let cmb c a b = Combine(a, c, b)
// helper symbols for creating formulas
let (+.), (-.), ( *. ), (/.), (%.), (<=.), (=.), (<>.), (>.), (&&.), (||.) =
  cmb Combinator.Plus, cmb Combinator.Minus, cmb Combinator.Mul, cmb Combinator.Div, cmb Combinator.Mod,
  cmb Combinator.LessOrEqual, cmb Combinator.Equal, cmb Combinator.Unequal, cmb Combinator.Greater,
  cmb Combinator.And, cmb Combinator.Or
  
open PseudoASM.Definition
// enhancement for 2-combinators with a member to construct formulas
[<AbstractClass>]
type Comb2WithFormula(x:Comb2) =
  inherit Comb2(x.Name, x.Symbol)
  abstract member ToFormula: Formula -> Formula -> Formula
let allCombinators = 
  List.mapi (fun i (e:PseudoASM) ->
    let makeFormula = cmb (unbox<Combinator> i)
    {new Comb2WithFormula(e.CommandInfo :?> Comb2) with override x.ToFormula a b = makeFormula a b}
   ) allCombinators

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

// size of stack
[<Literal>]
let LARGE_SIZE = 1000

// locations of important cells
[<Literal>]
let ``seed*`` = "A1"
let ``allInput*``, ``allOutput*`` = "A2", "B2"
[<Literal>]
let ``instr*`` = "A3"
let instrR, instrC = coordinates ``instr*``
[<Literal>]
let ``value*`` = "B3"
let valueR, valueC = coordinates ``value*``
[<Literal>]
let ``heap*`` = "C3"
let heapR, heapC = coordinates ``heap*``
let ``inputMachine*`` = "D3"
let inputR, inputC = coordinates ``inputMachine*``
let ``scannedInput*`` = coordsToS (inputR + 1, inputC)
let ``output*`` = "E3"
[<Literal>]
let ``var*`` = 3