module Parser.Main
open Parser.Datatype
open Parser.AST
open Parser.Combinators
open Utils.RegexUtils
open System.Security.Claims

module Types = TypeClasses

let _null = (!"null" |/ !"NULL" |/ !"Null") ->/ fun () -> Value(Lit("0", Ptr Void))
let _var = Match RegexUtils.VAR ->/ fun s -> Value(Var(s, Types.any))
let _string = Match RegexUtils.STRING ->/ fun s -> Value(Lit(s, Ptr Byte))
let _int = Match RegexUtils.NUM_INT32 ->/ fun s -> Value(Lit(s, Int))
let _float = Match RegexUtils.NUM_FLOAT ->/ fun s -> Value(Lit(s, Float))
let _char = Match RegexUtils.CHAR ->/ fun s -> Value(Lit(s, Byte))
let _long = Match RegexUtils.NUM_INT64 ->/ fun s -> Value(Lit(s, Int64))


let var_ast t s = Value(Var(s, t))

let prefix: AST Rule =
  OneOf [
    %"-" ->/ fun _ -> Value(Var("-prefix", Types.f_arith_prefix))
    %"*" ->/ fun _ -> var_ast (Types.f_unary Types.ptr Types.any) "*prefix"
    %"&" ->/ fun _ -> var_ast (Types.f_unary Types.any Types.ptr) "&prefix"
    %"!" ->/ var_ast Types.f_arith_prefix
    %"++" ->/ var_ast Types.f_arith_prefix
    %"--" ->/ var_ast Types.f_arith_prefix
   ]
let suffix: AST Rule =
  OneOf [
    %"++" ->/ fun _ -> var_ast Types.f_arith_prefix "++suffix"
    %"--" ->/ fun _ -> var_ast Types.f_arith_prefix "--suffix"
   ]
let access_operator: AST Rule = (%"." |/ %"->") ->/ var_ast Types.any
let math_infix_1: AST Rule = Match "[*/%]" ->/ var_ast Types.f_arith_infix
let math_infix_2: AST Rule = Match "[+-]" ->/ var_ast Types.f_arith_infix
let logic_infix: AST Rule = (%"||" |/ %"&&") ->/ var_ast Types.f_logic_infix
let comparison: AST Rule =
  OneOf [
    Match "[><]"
    %"!="
    %"=="
    %">="
    %"<="
   ] ->/ var_ast Types.f_logic_infix

let datatype: DT Rule =
  OneOf [
    Optional !"unsigned" +/ %"int" ->/ fun _ -> Int  // TEMP - unsigned
    %"char" ->/ fun _ -> Byte
    %"bool" ->/ fun _ -> Byte
    %"long" ->/ fun _ -> Int64
    %"double" ->/ fun _ -> Double
    %"float" ->/ fun _ -> Float
    %"void" ->/ fun _ -> Void
    !"struct" +/ _var ->/ function _, Value(Var(s, _)) | Strict s -> TypeDef (Struct s)
    !"union" +/ _var ->/ function _, Value(Var(u, _)) | Strict u -> TypeDef (Union u)
    _var ->/ function Value(Var(t, _)) | Strict t -> TypeDef (Alias t)
   ]
let assignment: AST Rule = (%"=" |/ Match "[+\-*/&|]=") ->/ var_ast Types.f_arith_infix


let rec value() : AST rule =
  let basic_value = OneOf [_null; _var; _string; _int; _float; _char; _long; bracketed]
  let rec value_with_apply_and_index: AST Rule =
    basic_value
     +/ OptionalListOf (
          !"(" +/ arg_list +/ !")" ->/ function ((), ll), () -> Apply(Value.unit, ll)
           |/ square_bracketed ->/ function i -> Index(Value.unit, i)
         )
     ->/ fun (v, ops) ->
           List.fold (fun acc -> function
             |Apply(Value.Unit, ll) -> Apply(acc, ll)
             |Index(Value.Unit, i) -> Index(acc, i)
             |_ -> failwith "should never be reached"
            ) v ops
  let op1 v op = Apply(Value(Var(op, Types.any)), [v; Value(Lit("1", Int))])
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
    let v = suffixed_value
    let v = v +/ OptionalListOf (access_operator +/ _var) ->/ ast_from_infixes
    let v = JoinedListOf v math_infix_1 ->/ ast_from_infixes
    let v = JoinedListOf v math_infix_2 ->/ ast_from_infixes
    let v = JoinedListOf v comparison ->/ ast_from_infixes
    let v = JoinedListOf v logic_infix ->/ ast_from_infixes
    v
  let rec value_with_assignment (): AST rule =
    let assign_transformation ((a, op), b) =
      match op with
      |Value(Var("=", _)) -> Assign(a, b)
      |Value(Var(("+=" | "-=" | "*=" | "/=" | "&=" | "|=" as s), _)) ->
        let xpr = Apply(Value(Var(s.[..0], Types.f_arith_infix)), [a; b])
        Assign(a, xpr)
      |_ -> failwith "should never be reached"
    () |> OneOf [
      value_with_infix +/ assignment +/ value_with_assignment ->/ assign_transformation
       |/ value_with_infix
     ]
  () |> value_with_assignment
and bracketed: AST Rule = !"(" +/ value +/ !")" ->/ function ((), v), () -> v
and square_bracketed: AST Rule = !"[" +/ value +/ !"]" ->/ function ((), v), () -> v
and arg_list: AST list Rule = Optional (JoinedListOf value !",") ->/ function None -> [] | Some(arg1, args) -> arg1::List.map snd args

type DeclBuilder = DT -> AST list -> AST list  // type -> initial values -> complete block
let declarable_value: DeclBuilder Rule =
  let _var =
    let builder name: DeclBuilder = fun dt initials ->
      Declare(name, dt)::List.map (fun v -> Assign(Value(Var(name, Types.any)), v)) initials
    _var ->/ function Value(Var(name, _)) | Strict name -> builder name
  let rec ptr() =
    () |>
      !"*" +/ (_var |/ ptr) ->/ fun ((), builder) dt initials ->
        let (Declare(name, dt)::rest | Strict(name, dt, rest)) = builder dt initials
        Declare(name, Ptr dt)::rest
  let array =
    _var +/ square_bracketed ->/ fun (builder, sz_ast) dt initials ->
      let (Declare(name, dt)::_ | Strict(name, dt)) = builder dt initials
      let assign_index_ast value i e = Assign(Index(value, Value(Lit(string i, Int))), e)
      Declare(name, Ptr dt)
       :: Assign(Value(Var(name, Types.any)), BuiltinASTs.stack_alloc dt sz_ast)
       :: List.mapi (assign_index_ast (Value(Var(name, Types.any)))) initials
  OneOf [
    array
    ptr
    _var
   ]
// TODO: update static and extern
let declare_value: AST Rule =
  let init_list =
    !"{" +/ JoinedListOf value !"," +/ !"}" ->/ fun (((), (v1, vlist)), ()) -> v1::List.map snd vlist
  let decl_with_assign =
    OneOf [
      declarable_value +/ !"=" +/ value ->/ fun ((builder, ()), initial) dt -> builder dt [initial]
      declarable_value +/ !"=" +/ init_list ->/ fun ((builder, ()), init_l) dt -> builder dt init_l
      declarable_value ->/ fun builder dt -> builder dt []
     ]
  let decl_list =
    datatype +/ JoinedListOf decl_with_assign !"," ->/ fun (dtype, (v1, vlist)) ->
      List.collect ((|>) dtype) (v1::List.map snd vlist)
  Optional (%"extern" |/ %"static") +/ decl_list ->/ fun (_, ll) -> DeclareHelper ll
let return_value: AST Rule =
  !"return" +/ Optional value
   ->/ function
       |(), Some v -> Return v
       |(), None -> Return Value.unit


let rec statement() : AST rule =
  () |>
    OneOf [
      _if
      _for
      _while
      Optional(return_value |/ declare_value |/ value) +/ !";"
       ->/ function Some x, () -> x | None, () -> Block []
     ]
and code_block: AST Rule = !"{" +/ OptionalListOf statement +/ !"}" ->/ function ((), stmts), () -> Block stmts
and code_body: AST Rule = statement |/ code_block
and _if: AST Rule =
  !"if" +/ bracketed +/ code_body +/ Optional(!"else" +/ code_body)
   ->/ function
       |(((), cond_body), then_body), optional_else ->
         let else_body = match optional_else with Some ((), Block xprs) -> Block xprs | Some ((), xpr) -> Block [xpr] | None -> Block []
         let then_body = match then_body with Block xprs -> Block xprs | xpr -> Block [xpr]
         If(cond_body, then_body, else_body)
and _while: AST Rule =
  !"while" +/ bracketed +/ code_body
   ->/ function ((), cond_body), loop_body -> While(cond_body, loop_body)
and _for: AST Rule =
  !"for" +/ !"("
   +/ Optional(declare_value |/ value) +/ !";"
   +/ Optional value +/ !";"
   +/ Optional value +/ !")"
   +/ code_body
   ->/ fun (((((((((), ()), decl), ()), cond), ()), incr), ()), loop_body) ->
         let extract_for_clause = function Some x -> x | None -> Value(Lit("'1'", Byte))
         let loop_body' =
           match loop_body with
           |Block xprs -> Block (xprs @ [extract_for_clause incr])
           |xpr -> Block [xpr; extract_for_clause incr]
         Block [
           extract_for_clause decl
           While(extract_for_clause cond, loop_body')
          ]


let declare_function: AST Rule =
  let arg_list: AST list Option Rule =  // TODO: update void args / no args - no args should accept any number of args, void should not accept any args
    let csvalues = JoinedListOf (datatype +/ declarable_value ->/ fun (dt, builder) -> builder dt []) !","
    csvalues ->/ fun (a1, args) -> a1::List.map snd args |> List.concat |> Some
     |/ Optional !"void" ->/ Option.map (fun () -> [])
  Optional (!"static" |/ !"extern")  // TODO: update static/extern
   +/ (datatype +/ _var |/ !"main" ->/ fun () -> Int, Value(Var("main", Types.any)))  // int main() and main() are both valid
   +/ !"(" +/ arg_list +/ !")" +/ (code_block |/ !";" ->/ fun () -> Block [])
   ->/ fun (((((_, (ret_dt, name)), ()), args), ()), func_body) ->
         let (Value(Var(name, _)) | Strict name) = name
         let args, func_dtype =
           // not looking for init lists, so all elements should be Declare
           Option.map (List.map (function Declare(name, t) -> (name, t), t | _ -> failwith "invalid function definition") >> List.unzip) args
            |> Option.map (fun (args, arg_dts) -> (args, Datatype.Function(arg_dts, ret_dt)))
            |> Option.defaultValue ([], Datatype.Function2 ret_dt)
         DeclareHelper [
           Declare(name, func_dtype)
           Assign(Value(Var(name, func_dtype)), Function(args, func_body))
          ]


let typedef: TypeDef Rule =
  !"typedef" +/ _var +/ datatype ->/ fun (((), Value(Var(t', _)) | Strict t'), dt) -> DeclAlias(t', dt)

let declare_struct: TypeDef Rule =  // `struct X? { int a; int b[50]; int c:2; }`  (initial values are parsed later)
  !"struct"
   +/ (Optional _var ->/ (Option.map (fun (Value(Var(s, _)) | Strict s) -> s) >> Option.defaultValue "_anon"))
   +/ (!"{" +/ OptionalListOf (datatype +/ declarable_value +/ Optional (!":" +/ %%NUM_INT32 ->/ (snd >> int)) +/ !";") +/ !"}")
   ->/ fun ((_, struct_name), ((_, fields), _)) ->
         let (_, all_fields) =
           List.fold (fun (bit, acc) (((dt, fld_builder), ``start_bit?``), _) ->
             match fld_builder dt [], ``start_bit?`` with
             |[Declare(fld_name, dt')], Some start_bit -> (bit, StructField(fld_name, dt', start_bit, dt'.sizeof)::acc)
             |[Declare(fld_name, dt')], None -> (bit + dt'.sizeof * 8, StructField(fld_name, dt', bit, dt'.sizeof)::acc)
             |[Declare(fld_name, (Ptr dt'' as dt')); Assign(_, Apply(Value(Var("\stack_alloc", _)), [Value(Lit(n, Int))]))], Some start_bit ->
               (bit, StructField(fld_name, dt', start_bit, dt'.sizeof + dt''.sizeof * int n)::acc)
             |[Declare(fld_name, (Ptr dt'' as dt')); Assign(_, Apply(Value(Var("\stack_alloc", _)), [Value(Lit(n, Int))]))], None ->
               let sz = dt'.sizeof + dt''.sizeof * int n
               (bit + sz * 8, StructField(fld_name, dt', bit, sz)::acc)
             |Strict x -> x
            ) (0, []) fields
         DeclStruct(struct_name, List.rev all_fields)

let declare_union: TypeDef Rule =
  !"union"
   +/ (Optional _var ->/ (Option.map (fun (Value(Var(s, _)) | Strict s) -> s) >> Option.defaultValue "_anon"))
   +/ (!"{" +/ OptionalListOf (datatype +/ declarable_value +/ !";") +/ !"}")
   ->/ fun ((_, union_name), ((_, fields), _)) ->
         let all_fields =
           List.map (fun ((dt, fld_builder), _) ->
             match fld_builder dt [] with
             |[Declare(fld_name, dt')] -> StructField(fld_name, dt', 0, dt'.sizeof)
             |[Declare(fld_name, (Ptr dt'' as dt')); Assign(_, Apply(Value(Var("\stack_alloc", _)), [Value(Lit(n, Int))]))] ->
               StructField(fld_name, dt', 0, dt'.sizeof + dt''.sizeof * int n)
             |Strict x -> x
            ) fields
         DeclUnion(union_name, all_fields)

let parse_typedecl: AST list Rule =
  let struct_or_union =
    (declare_struct |/ declare_union) +/ Optional (JoinedListOf declarable_value !",")
     ->/ fun (typedef, decls) ->
           let decls =
             Option.map (fun (_1, _z2) -> _1::List.map snd _z2) decls
              |> Option.defaultValue [fun dt _ -> [Declare("?", dt)]]
           List.collect (fun builder -> builder (TypeDef typedef) []) decls
  let typedef_struct_or_union =
    !"typedef" +/ (declare_struct |/ declare_union) +/ Optional (JoinedListOf _var !",")
     ->/ fun ((_, typedef), typenames) ->
           let typenames =
             Option.map (fun (_1, _z2) -> _1::List.map snd _z2) typenames
              |> Option.defaultValue []
              |> List.map (fun (Value(Var(s, _)) | Strict s) -> s)
           let typelink =
             match typedef with
             |DeclStruct(name, _) -> Struct name
             |DeclUnion(name, _) -> Union name
             |Strict x -> x
           Declare("?", TypeDef typedef)
            :: List.map (fun name -> Declare("?", TypeDef (DeclAlias(name, TypeDef typelink)))) typenames
  OneOf [
    struct_or_union
    typedef_struct_or_union
    typedef ->/ fun typedef -> [Declare("?", TypeDef typedef)]
   ] +/ !";" ->/ fst


let parse_global_scope: AST Rule =
  let try_parse_decl =
    OneOf [
      declare_function
      declare_value +/ !";" ->/ fst
      !";" ->/ fun () -> Block []
     ] ->/ List.singleton
     |/ parse_typedecl
  ListOf try_parse_decl +/ (EOF |/ try_parse_decl ->/ fun _ -> ())  // try_parse_decl in parallel with EOF to get correct error messages
   ->/ (fst >> List.concat >> GlobalParse)


let parse_tokens_to_ast_with: AST Rule -> Lexer.Main.Token list -> AST = fun rule tokens ->
  let result = rule () tokens
  match result with
  |Yes(parsed, []) -> parsed
  |Yes(_, fail::_) -> failwithf "unexpected token: %A" fail
  |Error(err, rest) -> failwithf "error: %s\n%A" err rest

let parse_tokens_to_ast: Lexer.Main.Token list -> AST = parse_tokens_to_ast_with parse_global_scope

let parse_string_to_ast_with: AST Rule -> string -> AST = fun rule string ->
  parse_tokens_to_ast_with rule (Lexer.Main.tokenize_text string)
  
  