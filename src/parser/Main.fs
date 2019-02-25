module Parser.Main
open Parser.AST
open Parser.Combinators

let _null = (!"null" |/ !"NULL" |/ !"Null") ->/ fun () -> Value(Lit("0", Pointer(Void, Some 0)))
let _var = Match "[a-z A-Z _][a-z 0-9 A-Z _]*" ->/ fun s -> Value(Var(s, t_any))
let _string = Match "\"(\\\"|[^\"])*\"" ->/ fun s -> Value(Lit(s, Pointer(Char, Some(s.Length + 1))))
let _int = Match "-?[0-9]+" ->/ fun s -> Value(Lit(s, Int))
let _float = Match "-?[0-9]+\.[0-9]*\w?" ->/ fun s -> Value(Lit(s, Float))
let _char = Match "'\\\\?.'" ->/ fun s -> Value(Lit(s, Char))
let _long = Match "-?[0-9]+(L|l){1,2}" ->/ fun s -> Value(Lit(s, Long))


let var_ast t s = Value(Var(s, t))

let prefix =
  OneOf [
    %"-" ->/ fun _ -> Value(Var("-prefix", tf_arith_prefix))
    %"*" ->/ fun _ -> var_ast (tf_unary [Pointer(t_any, None)] [t_any]) "*prefix"
    %"&" ->/ fun _ -> var_ast (tf_unary [t_any] [Pointer(t_any, None)]) "&prefix"
    %"!" ->/ var_ast tf_arith_prefix
    %"++" ->/ var_ast tf_arith_prefix
    %"--" ->/ var_ast tf_arith_prefix
   ]
let suffix =
  OneOf [
    %"++" ->/ fun _ -> var_ast tf_arith_prefix "++suffix"
    %"--" ->/ fun _ -> var_ast tf_arith_prefix "--suffix"
   ]
let math_infix_1 = Match "[*/%]" ->/ var_ast tf_arith_infix
let math_infix_2 = Match "[+-]" ->/ var_ast tf_arith_infix
let logic_infix = (%"||" |/ %"&&") ->/ var_ast tf_logic_infix
let comparison =
  OneOf [
    Match "[><]"
    %"!="
    %"=="
    %">="
    %"<="
   ] ->/ var_ast tf_logic_infix

let datatype =
  OneOf [
    %"int" ->/ fun _ -> Int
    %"char" ->/ fun _ -> Char
    %"bool" ->/ fun _ -> Char
    %"long" ->/ fun _ -> Long
    %"double" ->/ fun _ -> Double
    %"float" ->/ fun _ -> Float
    %"void" ->/ fun _ -> Void
   ]
let assignment = (%"=" |/ Match "[+\-*/&|]=") ->/ var_ast tf_arith_infix


let rec value() =
  let basic_value = OneOf [_null; _var; _string; _int; _float; _char; _long; bracketed]
  let rec value_with_apply_and_index: AST Rule =
    basic_value
     +/ OptionalListOf (
          !"(" +/ arg_list +/ !")" ->/ function ((), ll), () -> Apply(Value Unit, ll)
           |/ square_bracketed ->/ function i -> Index(Value Unit, i)
         )
     ->/ fun (v, ops) ->
           List.fold (fun acc -> function
             |Apply(Value Unit, ll) -> Apply(acc, ll)
             |Index(Value Unit, i) -> Index(acc, i)
             |_ -> failwith "should never be reached"
            ) v ops
  let op1 v op = Apply(Value(Var(op, t_any)), [v; Value(Lit("1", Int))])
  let rec prefixed_value() =
    () |>
      prefix +/ (value_with_apply_and_index |/ prefixed_value)
       ->/ function
           |Value(Var("++", _)), v -> Assign(v, op1 v "+")
           |Value(Var("--", _)), v -> Assign(v, op1 v "-")
           |pf, v -> Apply(pf, [v])
  let suffixed_value =
    (prefixed_value |/ value_with_apply_and_index) +/ OptionalListOf suffix
     ->/ fun (v, sfs) ->
           List.fold (fun acc -> function
             |Value(Var("++suffix", _)) -> op1 (Assign(v, op1 v "+")) "-"
             |Value(Var("--suffix", _)) -> op1 (Assign(v, op1 v "-")) "+"
             |sf -> Apply(sf, [acc])
            ) v sfs
  let value_with_infix =
    let ast_from_infixes (iv: AST, ops: ((AST * AST) list)) =
      List.fold (fun acc (op, oprnd) -> Apply(op, [acc; oprnd])) iv ops
    JoinedListOf (
      JoinedListOf (
        JoinedListOf (
          JoinedListOf suffixed_value math_infix_1 ->/ ast_from_infixes
         ) math_infix_2 ->/ ast_from_infixes
       ) comparison ->/ ast_from_infixes
     ) logic_infix ->/ ast_from_infixes
  let rec value_with_assignment (): AST rule =
    let assign_transformation ((a, op), b) =
      match op with
      |Value(Var("=", _)) -> Assign(a, b)
      |Value(Var(("+=" | "-=" | "*=" | "/=" | "&=" | "|=" as s), _)) ->
        let xpr = Apply(Value(Var(s.[..0], tf_arith_infix)), [a; b])
        Assign(a, xpr)
      |_ -> failwith "should never be reached"
    () |> OneOf [
      value_with_infix +/ assignment +/ value_with_assignment ->/ assign_transformation
       |/ value_with_infix
     ]
  () |> value_with_assignment
and bracketed: AST Rule = !"(" +/ value +/ !")" ->/ function ((), v), () -> v
and square_bracketed = !"[" +/ value +/ !"]" ->/ function ((), v), () -> v
and arg_list = Optional (JoinedListOf value !",") ->/ function None -> [] | Some(arg1, args) -> arg1::List.map snd args

let declarable_value =
  let _var: (Datatype -> string * Datatype) Rule =
    _var ->/ function Value(Var(name, Unknown [])) -> (fun dtype -> (name, dtype)) | _ -> failwith "should never be reached"
  let rec ptr(): (Datatype -> string * Datatype) rule =
    () |>
      !"*" +/ (_var |/ ptr) ->/ fun ((), v) dtype ->
        let name, t = v dtype
        (name, Pointer(t, Some t.sizeof))
  OneOf [
    _var +/ square_bracketed ->/ fun (name, sz_ast) (dtype: Datatype) ->
      let sz =
        match sz_ast with
        |Value(Lit(s, (Int | Long))) -> Some(dtype.sizeof * int (s.Replace("L", "")))
        |_ -> None
      (fst (name dtype), Pointer(dtype, sz))
    ptr
    _var
   ]
// TODO: update static and extern
let declare_value =
  let init_list =
    !"{" +/ JoinedListOf value !"," +/ !"}" ->/ fun (((), (v1, vlist)), ()) -> v1::List.map snd vlist
  let decl_with_assign =
    OneOf [
      declarable_value +/ !"=" +/ value ->/ fun ((v, ()), initial) t ->
        let name, dtype = v t
        [Declare(name, dtype); Assign(Value(Var(name, dtype)), initial)]
      declarable_value +/ !"=" +/ init_list ->/ fun ((v, ()), init_l) t ->
        let name, dtype = v t
        let assign_index_ast value i e = Assign(Index(value, Value(Lit(string i, Int))), e)
        Declare(name, dtype)::List.mapi (assign_index_ast (Value(Var(name, dtype)))) init_l
      declarable_value ->/ fun v dtype -> [Declare(v dtype)]
     ]
  let decl_list =
    datatype +/ JoinedListOf decl_with_assign !"," ->/ fun (dtype, (v1, vlist)) ->
      List.collect ((|>) dtype) (v1::List.map snd vlist)
  Optional (%"extern" |/ %"static") +/ decl_list ->/ fun (_, ll) -> DeclareHelper ll
let return_value =
  !"return" +/ Optional value
   ->/ function
       |(), Some v -> Return v
       |(), None -> Return (Value Unit)


let rec statement() =
  () |>
    OneOf [
      _if
      _for
      _while
      Optional(declare_value |/ return_value |/ value) +/ !";"
       ->/ function Some x, () -> x | None, () -> Block []
     ]
and code_block = !"{" +/ OptionalListOf statement +/ !"}" ->/ function ((), stmts), () -> Block stmts
and code_body = statement |/ code_block
and _if =
  !"if" +/ bracketed +/ code_body +/ Optional(!"else" +/ code_body)
   ->/ function
       |(((), cond_body), then_body), optional_else ->
         let else_body = match optional_else with Some ((), Block xprs) -> Block xprs | Some ((), xpr) -> Block [xpr] | None -> Block []
         let then_body = match then_body with Block xprs -> Block xprs | xpr -> Block [xpr]
         If(cond_body, then_body, else_body)
and _while =
  !"while" +/ bracketed +/ code_body
   ->/ function ((), cond_body), loop_body -> While(cond_body, loop_body)
and _for =
  !"for" +/ !"("
   +/ Optional(declare_value |/ value) +/ !";"
   +/ Optional value +/ !";"
   +/ Optional value +/ !")"
   +/ code_body
   ->/ fun (((((((((), ()), decl), ()), cond), ()), incr), ()), loop_body) ->
         let extract_for_clause = function Some x -> x | None -> Value(Lit("1", Char))
         let loop_body' =
           match loop_body with
           |Block xprs -> Block (xprs @ [extract_for_clause incr])
           |xpr -> Block [xpr; extract_for_clause incr]
         Block [
           extract_for_clause decl
           While(extract_for_clause cond, loop_body')
          ]


let declare_function =
  let arg_list =  // TODO: update void args / no args - no args should accept any number of args, void should not accept any args
    let csvalues = JoinedListOf (datatype +/ declarable_value ->/ fun (dtype, v) -> v dtype) !","
    csvalues ->/ fun (a1, args) -> a1::List.map snd args |> List.map Declare
     |/ Optional !"void" ->/ function Some() -> [] | None -> [Declare(".", t_any)]
  Optional (!"static" |/ !"extern")  // TODO: update static/extern
   +/ (datatype +/ _var |/ !"main" ->/ fun () -> Int, Value(Var("main", t_any)))  // int main() and main() are both valid
   +/ !"(" +/ arg_list +/ !")" +/ (code_block |/ !";" ->/ fun () -> Block [])
   ->/ fun (((((_, (dtype, name)), ()), args), ()), func_body) ->
         let name = match name with Value(Var(s, Unknown [])) -> s | _ -> failwith "should never be reached"
         let args, arg_types =
           List.map (function Declare(name, t) -> (name, t), t | _ -> failwith "invalid function definition") args
            |> List.unzip
         let func_dtype = Datatype.Function(arg_types, dtype)
         DeclareHelper [
           Declare(name, func_dtype)
           Assign(Value(Var(name, func_dtype)), Function(args, func_body))
          ]


let parse_global_scope: AST Rule =
  let try_parse_decl =
    OneOf [
      declare_function
      declare_value +/ !";" ->/ fst
      !";" ->/ fun () -> Block []
     ]
  ListOf try_parse_decl +/ (EOF |/ try_parse_decl ->/ fun _ -> ())  // try_parse_decl in parallel with EOF to get correct error messages
   ->/ (fst >> GlobalParse)


let parse_tokens_to_ast_with rule tokens =
  let result = rule () tokens
  match result with
  |Yes(parsed, []) -> parsed
  |Yes(_, fail::_) -> failwithf "unexpected token: %A" fail
  |Error(err, rest) -> failwithf "error: %s\n%A" err rest

let parse_tokens_to_ast = parse_tokens_to_ast_with parse_global_scope

let parse_string_to_ast_with rule string = parse_tokens_to_ast_with rule (Lexer.Main.tokenize_text string)
  
  