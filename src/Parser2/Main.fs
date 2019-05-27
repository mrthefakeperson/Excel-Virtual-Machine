module Parser.Main
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

let datatype: DT Rule =
  OneOf [
    Optional !"unsigned" +/ %"int" ->/ fun _ -> Int  // TEMP - unsigned
    %"char" ->/ fun _ -> Byte
    %"bool" ->/ fun _ -> Byte
    %"long" ->/ fun _ -> Int64
    %"double" ->/ fun _ -> Double
    %"float" ->/ fun _ -> Float
    %"void" ->/ fun _ -> Void
    !"struct" +/ _var ->/ fun (_, (Var(s, _)) | Strict s) -> TypeDef (Struct s)
    !"union" +/ _var ->/ fun (_, (Var(u, _)) | Strict u) -> TypeDef (Union u)
    _var ->/ fun (Var(t, _) | Strict t) -> TypeDef (Alias t)
   ]
let assignment: Value Rule = (%"=" |/ Match "[+\-*/&|]=") ->/ retype Types.f_arith_infix

let Index(v, i) =
  Apply'.fn("*prefix", Types.f_unary Types.ptr Types.any)
   (Apply'.fn2("+", Types.f_arith_infix) v i)

let rec expr() : AST Rule =
  let apply_and_index_expr = SequenceOf {
    let! v = basic_value
    let! result =
      FoldListOf (fun acc ->
        !"(" +/ arg_list +/ !")" ->/ fun parsed -> Apply(acc, middle parsed)
         |/ square_bracketed ->/ fun i -> Index(acc, i)
       ) (V v)
    return result
   }
  let op1 v op = Apply'.fn2(op, Types.f_arith_infix) v (V(Lit("1", Int)))
  let prefixed_expr =
    FoldBackListOf (fun (Var(s, _) | Strict s as pref) ast ->
      Assign(V pref, op1 ast (match s with "++" -> "+" | "--" -> "-" | Strict x -> x))
     ) prefix apply_and_index_expr
  let suffixed_expr = SequenceOf {
    let! pref = prefixed_expr
    let! result =
      FoldListOf (fun acc ->
        suffix ->/ function
          |Var("++suffix", _) -> op1 (Assign(acc, op1 acc "+")) "-"
          |Var("--suffix", _) -> op1 (Assign(acc, op1 acc "-")) "+"
          |Strict x -> x
       ) pref
    return result
   }
  let infix_expr = SequenceOf {
    let infix_folder oprule opndrule acc = oprule +/ opndrule ->/ fun (op, opnd) -> Apply(V op, [acc; opnd])
    let! res = suffixed_expr
    let rl = FoldListOf (infix_folder access_operator (V <-/ _var)) res
    let! res = rl
    let rl = FoldListOf (infix_folder math_infix_1 rl) res
    let! res = rl
    let rl = FoldListOf (infix_folder math_infix_2 rl) res
    let! res = rl
    let rl = FoldListOf (infix_folder comparison rl) res
    let! res = rl
    let rl = FoldListOf (infix_folder logic_infix rl) res
    let! res = rl
    return res
   }
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

let declarable_expr dt : AST Rule =
  let _var' = _var ->/ fun (Var(s, _) | Strict s) -> Declare(s, dt)
  let wrap_ptr _ (Declare(s, dt) | Strict(s, dt)) = Declare(s, DT.Ptr dt)
  let ptr = FoldBackListOf wrap_ptr !"*" _var'
  // TODO: package bounds into wrap_ptr
  let array = _var' +/ ListOf square_bracketed ->/ fun (v, bounds) -> List.fold wrap_ptr v bounds
  array |/ ptr |/ _var'

// TODO: update static and extern
let declare_expr: AST list Rule =
  let init_list = !"{" +/ JoinedListOf (expr()) !"," +/ !"}" ->/ middle
  let decl_with_assign dt = SequenceOf {
    let! Declare(name, dt) | Strict(name, dt) as decl = declarable_expr dt
    match! Optional !"=" with
    |None -> return [decl]
    |Some () ->
      match! Optional (expr()) with
      |Some x -> return [decl; x]
      |None ->
        let! init_values = init_list
        let assign_index i expr = Assign(Index(V(Var(name, dt)), V(Lit(string i, DT.Int))), expr)
        return decl::List.mapi assign_index init_values
   }
  let decl_list = SequenceOf {
    let! dt = datatype
    let! result = JoinedListOf (decl_with_assign dt) !","
    return List.concat result
   }
  Optional (%"extern" |/ %"static") +/ decl_list ->/ snd

let return_expr: AST Rule =
  !"return" +/ Optional (expr()) ->/ snd ->/ Option.defaultValue Value.unit


let rec statement() : AST list Rule =
  OneOf [_if; _for; _while] ->/ List.singleton
   |/ declare_expr +/ !";" ->/ fst
   |/ !";" ->/ fun _ -> []
   |/ OneOf [return_expr; expr()] +/ !";" ->/ (fst >> List.singleton)
and code_block: AST list Rule =
  !"{" +/ OptionalListOf (statement()) +/ !"}" ->/ (middle >> List.concat)
and code_body: AST list Rule = statement() |/ code_block
and _if: AST Rule = SequenceOf {
  do! !"if"
  let! cond = bracketed
  let! thn = code_body
  match! Optional !"else" with
  |Some () -> let! els = code_body in return If(cond, Block thn, Block els)
  |None -> return If(cond, Block thn, Value.unit)
 }
and _while: AST Rule =
  !"while" +/ bracketed +/ code_body
   ->/ function ((), cond), loop -> While(cond, Block loop)
and _for: AST Rule = SequenceOf {
  do! !"for"
  do! !"("
  let! decl = Optional (declare_expr |/ expr() ->/ List.singleton)
  let decl = Option.defaultValue [] decl
  do! !";"
  let! cond = Optional (expr())
  let cond = Option.defaultValue (V(Lit("'\001'", Byte))) cond
  do! !";"
  let! incr = Optional (expr())
  let incr = Option.defaultValue Value.unit incr
  do! !")"
  let! body = code_body
  let loop_body = Block (body @ [incr])
  return Block (decl @ [While(cond, loop_body)])
 }


let declare_function: AST list Rule =
  let arg_list: (string * DT) list Option Rule =  // TODO: update void args / no args - no args should accept any number of args, void should not accept any args
    let single_arg = SequenceOf {
      let! dt = datatype
      let! (Declare(name, dt) | Strict(name, dt)) = declarable_expr dt
      return (name, dt)
     }
    let csvalues = JoinedListOf single_arg !","
    // f(void) - must be called f(); f() - can be called f(..);
    csvalues ->/ Some |/ Optional !"void" ->/ Option.map (fun () -> [])
  SequenceOf {
    let! _ = Optional (!"static" |/ !"extern")  // TODO: update static/extern
    let! (return_dt, name) = SequenceOf {
      match! Optional (datatype +/ _var) with
      |Some(dt, (Var(name, _) | Strict name)) -> return (dt, name)
      |None -> let! _ = !"main" in return (Int, "main")
     }
    do! !"("
    let! args = arg_list
    do! !")"
    let! body = code_block |/ !";" ->/ fun () -> []
    let (args, func_dt) =
      match args with
      |Some args -> let arg_dts = List.map snd args in (args, DT.Function(arg_dts, return_dt))
      |None -> ([], DT.Function2 return_dt)
    return [
      Declare(name, func_dt)
      Assign(V(Var(name, func_dt)), Function(return_dt, args, Block body))
     ]
   }


let typedef: TypeDef Rule = SequenceOf {
  do! !"typedef"
  let! Var(alias_name, _) | Strict alias_name = _var
  let! dt = datatype
  return DeclAlias(alias_name, dt)
 }

let declare_struct: TypeDef Rule = SequenceOf {  // `struct X? { int a; int b[50]; int c:2; }`  (initial values are parsed later)
  do! !"struct"
  let! name_option = Optional _var
  let Var(name, _) | Strict name = Option.defaultValue (Var("_anon", DT.Void)) name_option
  do! !"{"
  let field = SequenceOf {
    let! dt = datatype
    let! Declare(name, dt) | Strict(name, dt) = declarable_expr dt
    match! Optional !":" with
    |Some () ->
      let! Lit(start_bit, _) | Strict start_bit = %%NUM_INT32
      do! !";"
      return StructField(name, dt, int start_bit, dt.sizeof)
    |None -> let! () = !";" in return StructField(name, dt, -1, dt.sizeof)
   }
  let! fields = OptionalListOf field
  do! !"}"
  let (fields, _) =
    List.mapFold (fun bit -> function
      |StructField(name, dt, -1, sz) -> (StructField(name, dt, bit, sz), bit + sz * 8)
      |field -> (field, bit)
     ) 0 fields
  return DeclStruct(name, fields)
 }

let declare_union: TypeDef Rule = SequenceOf {
  do! !"union"
  let! name_option = Optional _var
  let Var(name, _) | Strict name = Option.defaultValue (Var("_anon", DT.Void)) name_option
  do! !"{"
  let field = SequenceOf {
    let! dt = datatype
    let! Declare(name, dt) | Strict(name, dt) = declarable_expr dt
    do! !";"
    return StructField(name, dt, 0, dt.sizeof)
   }
  let! fields = OptionalListOf field
  do! !"}"
  return DeclUnion(name, fields)
 }

let parse_typedecl: AST list Rule =
  let placeholder_decl dt = Declare("?", dt)  // generated to contain new type when no immediate variable is assigned to it
  let declare_struct_or_union = SequenceOf {
    let! dt = DT.TypeDef <-/ (declare_struct |/ declare_union)
    let! decls = Optional (JoinedListOf (declarable_expr dt) !",")
    let decls = Option.defaultValue [placeholder_decl dt] decls
    return decls
   }
  let typedef_alias_struct_or_union = SequenceOf {
    do! !"typedef"
    let! typedef = declare_struct |/ declare_union
    let typedef_reference =
      match typedef with
      |DeclStruct(name, _) -> TypeDef (Struct name)
      |DeclUnion(name, _) -> TypeDef (Union name)
      |Strict x -> x
    let! type_names = Optional (JoinedListOf (_var ->/ fun (Var(s, _) | Strict s) -> s) !",")
    let type_names = Option.defaultValue [] type_names
    let make_alias alias_name = Declare("?", TypeDef (DeclAlias(alias_name, typedef_reference)))
    return placeholder_decl (TypeDef typedef) :: List.map make_alias type_names
   }
  OneOf [
    declare_struct_or_union
    typedef_alias_struct_or_union
    typedef ->/ fun typedef -> [placeholder_decl (TypeDef typedef)]
   ] +/ !";" ->/ fst


let parse_global_scope: AST Rule =
  let try_parse_decl =
    OneOf [
      declare_function
      declare_expr +/ !";" ->/ fst
      !";" ->/ fun () -> []
      parse_typedecl
     ]
  ListOf try_parse_decl +/ (End |/ try_parse_decl ->/ ignore)  // try_parse_decl in parallel with EOF to get correct error messages
   ->/ (fst >> List.concat >> GlobalParse)

let parse_tokens_to_ast_with: AST Rule -> Token list -> AST = fun rule tokens ->
  run_parser rule {stream = tokens}

let parse_tokens_to_ast: Token list -> AST = parse_tokens_to_ast_with parse_global_scope