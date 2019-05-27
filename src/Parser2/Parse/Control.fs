module Parser.Parse.Control
open Utils
open ParserCombinators
open CompilerDatatypes.DT
open CompilerDatatypes.Token
open CompilerDatatypes.AST
open Parser.Parse.Expr

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