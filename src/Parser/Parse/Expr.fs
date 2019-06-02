﻿module Parser.Parse.Expr
open Utils
open RegexUtils
open ParserCombinators
open CompilerDatatypes.DT
open CompilerDatatypes.Token
open CompilerDatatypes.AST

type Tokens = {stream : Token list}
  with
    member x.get_atomic_equal() = fun (token: string) ->
      match x.stream with
      |{value = (Var(s, _) | Lit(s, _) as v)}::tl when s = token -> Some({stream = tl}, v)
      |_ -> None
    member x.get_atomic_match() = fun (rgx: string) ->
      let rgx' = "^" + rgx + "$"
      match x.stream with
      |{value = (Var(Regex rgx' _, _) | Lit(Regex rgx' _, _) as v)}::tl -> Some({stream = tl}, v)
      |_ -> None
    member x.get_length() = List.length x.stream
    static member empty = {stream = []}

module Types = TypeClasses
type 't Rule = Rule<Tokens, 't>
let middle ((_, x), _) = x

let _var = %%VAR
let basic_value: Value Rule =
  OneOf [_var; %%STRING; %%NUM_INT32; %%NUM_FLOAT; %%CHAR; %%NUM_INT64]

let rename s (Var(_, t) | Strict t) = Var(s, t)
let retype t (Var(name, _) | Strict name) = Var(name, t)

let prefix: Value Rule =
  OneOf [
    %"-" ->/ (rename "-prefix" >> retype Types.f_arith_prefix)
    %"*" ->/ (rename "*prefix" >> retype (Types.f_unary Types.ptr Types.any))
    %"&" ->/ (rename "&prefix" >> retype (Types.f_unary Types.any Types.ptr))
    %"!" ->/ retype Types.f_arith_prefix
    %"++" ->/ retype Types.f_arith_prefix
    %"--" ->/ retype Types.f_arith_prefix
   ]
let suffix: Value Rule =
  OneOf [
    %"++" ->/ (rename "++suffix" >> retype Types.f_arith_prefix)
    %"--" ->/ (rename "--suffix" >> retype Types.f_arith_prefix)
   ]
let access_operator: Value Rule = (%"." |/ %"->") ->/ retype Types.any
let math_infix_1: Value Rule = Match "[*/%]" ->/ retype Types.f_arith_infix
let math_infix_2: Value Rule = Match "[+-]" ->/ retype Types.f_arith_infix
let logic_infix: Value Rule = (%"||" |/ %"&&") ->/ retype Types.f_logic_infix
let comparison: Value Rule =
  OneOf [
    Match "[><]"
    %"!="
    %"=="
    %">="
    %"<="
   ] ->/ retype Types.f_logic_infix

let assignment: Value Rule = (%"=" |/ Match "[+\-*/&|]=") ->/ retype Types.f_arith_infix

let Index(v, i) =
  Apply'.fn("*prefix", Types.f_unary Types.ptr Types.any)
   (Apply'.fn2("+", Types.f_arith_infix) v i)

let rec expr() : AST Rule =
  let apply_and_index_expr = SequenceOf {
    let! v = V <-/ basic_value |/ bracketed
    let! result =
      FoldListOf (fun acc ->
        !"(" +/ arg_list +/ !")" ->/ fun parsed -> Apply(acc, middle parsed)
         |/ square_bracketed ->/ fun i -> Index(acc, i)
       ) v
    return result
   }
  let op1 v op = Apply'.fn2(op, Types.f_arith_infix) v (V(Lit("1", Int)))
  let prefixed_expr =
    FoldBackListOf (fun (Var(s, _) | Strict s as pref) ast ->
      match s with
      |"++" -> Assign(V pref, op1 ast "+")
      |"--" -> Assign(V pref, op1 ast "-")
      |"-prefix" -> Apply'.fn2 "*" ast (V(Lit("-1", Int)))
      |Strict x -> x
     ) prefix apply_and_index_expr
  let suffixed_expr = SequenceOf {
    let! pref = prefixed_expr
    let! result =
      FoldListOf (fun acc ->
        // TODO: fix bug - acc is generated twice, and assignments / calls occur twice
        suffix ->/ function
          |Var("++suffix", _) -> op1 (Assign(acc, op1 acc "+")) "-"
          |Var("--suffix", _) -> op1 (Assign(acc, op1 acc "-")) "+"
          |Strict x -> x
       ) pref
    return result
   }
  let infix_expr =
    let wrap_infix_rule oprule opndrule = SequenceOf {
      let! opnd = opndrule
      let! applys = OptionalListOf (oprule +/ opndrule)
      return List.fold (fun acc (op, opnd) -> Apply(V op, [acc; opnd])) opnd applys
     }
    wrap_infix_rule access_operator suffixed_expr
     |> wrap_infix_rule math_infix_1
     |> wrap_infix_rule math_infix_2
     |> wrap_infix_rule comparison
     |> wrap_infix_rule logic_infix
  let assignment_expr =
    FoldBackListOf (fun (left, assign) right ->
      match assign with
      |Var("=", _) -> Assign(left, right)
      |Var(Regex "^(\+=|-=|\*=|/=|&=|\|=)$" _ as s, _) ->
        Assign(left, Apply'.fn2(s.[..0], Types.f_arith_infix) left right)
      |Strict x -> x
     ) (infix_expr +/ assignment) infix_expr
  assignment_expr
and bracketed: AST Rule = !"(" +/ expr() +/ !")" ->/ middle
and square_bracketed: AST Rule = !"[" +/ expr() +/ !"]" ->/ middle
and arg_list: AST list Rule = Option.defaultValue [] <-/ Optional (JoinedListOf (expr()) !",")