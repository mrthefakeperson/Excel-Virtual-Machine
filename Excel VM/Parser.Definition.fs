﻿module Parser.Definition

module Symbols =
  // symbol table for infix operators
  // lower is evaluated earlier
  let infixOrder = [|
    ["*"; "/"; "%"]
    ["+"; "-"]
    ["="; "<>"; ">"; ">="; "<"; "<="] @ ["=="; "!="]      // in the future, this should depend on language
    ["&&"]
    ["||"]
   |]
  // symbol table for prefix operators
  let prefixes = ["-"; "!"; "&"; "*"; "++"; "--"]     // languages really need individualized versions of this

open Symbols
// class definition for tokens
type Token(name:string, row_col, functionApplication:bool, dependants:Token list) =
  let row, col = row_col
  let priority =
    match Array.tryFindIndex (fun e -> List.exists ((=) name) e) infixOrder with
    |Some x -> x
    |None -> -1
  new(name, (row, col)) = Token(name, (row, col), true, [])
  new(name, (row, col), dependants) = Token(name, (row, col), false, dependants)
  new(name, dependants) = Token(name, (0, 0), false, dependants)
  new(name) = Token(name, (0, 0), false, [])
  member x.Name with get() = name
  member x.Dependants with get() = dependants
  member x.CanApply with get() = functionApplication
  member x.Indentation with get() = (row, col)
  member a.IndentedLess (b:Token) =
    let ar,ac = a.Indentation
    let br,bc = b.Indentation
    if b.Name = "fun"
     then ar = br || ac < bc
     else ac < bc && ar <= br
  member a.Priority with get() = priority
  member a.EvaluatedFirst (b:Token) = a.Priority <= b.Priority
  member x.Single with get() = dependants = []
  override x.ToString() =
    //sprintf "%s[%i,%i](%s)" name row col
    // (String.concat "," (List.map (sprintf "%O") dependants))
    sprintf "%s(%s)" name
     (String.concat "," (List.map (sprintf "%O") dependants))
  member x.ToStringExpr() =
    match name, dependants with
    |"apply", [a; b] -> sprintf "(%s) (%s)" (a.ToStringExpr()) (b.ToStringExpr())
    |"if", [a; b; c] -> sprintf "(if %s then %s else %s)" (a.ToStringExpr()) (b.ToStringExpr()) (c.ToStringExpr())
    |"sequence", _ -> String.concat "; " (List.map (fun (e:Token) -> e.ToStringExpr()) dependants)
    |"()", [a] -> sprintf "(%s)" (a.ToStringExpr())
    |"[]", [a] -> sprintf "[%s]" (a.ToStringExpr())
    |"dot", [a; b] -> sprintf "%s.%s" (a.ToStringExpr()) (b.ToStringExpr())
    |"while", [a; b] -> sprintf "(while %s do %s)" (a.ToStringExpr()) (b.ToStringExpr())
    |"for", [a; b; c] -> sprintf "for %s in %s do (%s)" (a.ToStringExpr()) (b.ToStringExpr()) (c.ToStringExpr())
    |"let", [a; b] -> sprintf "let %s = (%s)" (a.ToStringExpr()) (b.ToStringExpr())
    |"let rec", [a; b] -> sprintf "let rec %s = (%s)" (a.ToStringExpr()) (b.ToStringExpr())
    |"fun", [a; b] -> sprintf "fun %s -> (%s)" (a.ToStringExpr()) (b.ToStringExpr())
    |"pattern", [a; b] -> sprintf "| %s -> %s" (a.ToStringExpr()) (b.ToStringExpr())
    |",", members -> List.map (fun (e:Token) -> e.ToStringExpr()) members |> String.concat ", "
    |"struct", [a; m] -> sprintf "struct %s = {%s}" (a.ToStringExpr()) (m.ToStringExpr())
    |_, [] -> name
    |_ -> name + "(" + (String.concat " " (List.map (fun (e:Token) -> e.ToStringExpr()) dependants)) + ")"
  member x.Clean() =
    match name, dependants with
    |("sequence" | "()"), [a] -> a.Clean()
    |_, dependants -> Token(name, row_col, functionApplication, List.map (fun (e:Token) -> e.Clean()) dependants)
 // accessor patterns
let (|T|_|) (x:Token) =
  if x.Single
   then Some x.Name
   else None
let (|A|_|) (x:Token) = if x.CanApply then Some A else None
let (|X|) (t:Token) = X(t.Name, t.Dependants)
 // prefix classifier patterns
let (|Pref|_|) = function     // is a prefix -> Pref (symbol with ~ prepended)
  |T s as t when List.exists ((=) s) prefixes -> Some(Token ("~" + s, t.Indentation, true, []))
  |_ -> None
let (|Pref'|_|) = function     // is a prefix with ~ prepended -> Pref' (itself)
  |T s as t when s.Length > 1 && s.[0] = '~' && List.exists ((=) s.[1..]) prefixes -> Some t
  |_ -> None
 // datatype / variable classifier patterns
let (|Inner|_|) c = function
  |T "\"\"" -> Some ""
  |T s when s.Length >= 2 && s.[0] = c && s.[s.Length-1] = c -> Some s.[1..s.Length-2]
  |_ -> None
let (|Var|Cnst|Other|) = function               //todo: non-numeric constants
  |Inner '"' s -> Cnst s        //cases like "\" should not have made it through the lexer
  |Inner ''' s -> Cnst (string (int s.[0]))        //cases like 'dd' are the lexer's job
  |T "()" -> Cnst "()"
  |T "nothing" -> Cnst "nothing"    // change in future? maybe...
  |T "true" -> Cnst "1" | T "false" -> Cnst "0"
  |T ("true" | "false" as s) -> Cnst s
  |T ("return" | "break") -> Other
  |T s -> if s <> "" && '0' <= s.[0] && s.[0] <= '9' then Cnst s else Var s
  |_ -> Other