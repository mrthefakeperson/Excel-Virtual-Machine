module Parser.Parse.Control
open Utils
open ParserCombinators
open CompilerDatatypes.DT
open CompilerDatatypes.Token
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SyntaxAST
open Parser.Parse.Expr

let declarable_expr dt : AST list Rule =
  let _var' = _var ->/ fun (Var(s, _) | Strict s) -> Declare(s, dt)
  let wrap_ptr _ (Declare(s, dt) | Strict(s, dt)) = Declare(s, Ptr dt)
  let ptr = FoldBackListOf wrap_ptr !"*" _var'
  // TODO: package bounds into wrap_ptr
  let array = SequenceOf {
    let! Declare(s, dt) | Strict(s, dt) = _var'
    match! ListOf square_bracketed with
    |[] -> return [Declare(s, dt)]
    |array_dims ->
      let alloc_array = BuiltinASTs.stack_alloc dt array_dims
      let dt = TypeDef (Array(List.length array_dims, dt))
      return [Declare(s, dt); Assign(V (Var(s, dt)), alloc_array)]
   }
  array |/ (ptr |/ _var') ->/ List.singleton

let declare_expr: AST list Rule =
  let init_list = !"{" +/ JoinedListOf (expr()) !"," +/ !"}" ->/ middle
  let decl_with_assign dt = SequenceOf {
    let! Declare(name, dt)::_ | Strict(name, dt) as decl = declarable_expr dt
    match! Optional !"=" with
    |None -> return decl
    |Some () ->
      match! Optional (expr()) with
      |Some x -> return decl @ [Assign(V (Var(name, dt)), x)]
      |None ->
        let! init_values = init_list
        let assign_index i expr =
          let i_ast = V (Lit(string i, Int))
          Assign(BuiltinASTs.index (V (Var(name, dt))) i_ast, expr)
        return decl @ List.mapi assign_index init_values
   }
  SequenceOf {
    let! is_static = Optional %"static" ->/ Option.isSome
    let apply_static =
      if is_static then function
        |Declare(name, dt)::rest ->
          let static_assign = BuiltinASTs.get_static (V (Var(name, dt))) rest
          [Declare(name, dt); static_assign]
        |Strict x -> x
      else id
    let! is_extern = Optional %"extern" ->/ Option.isSome
    let apply_extern = id  // TODO: update
    let! dt = datatype
    let! result = JoinedListOf (decl_with_assign dt) !","
    return List.collect (apply_static >> apply_extern) result
   }

let return_expr: AST Rule =
  !"return" +/ Optional (expr()) ->/ (snd >> Option.defaultValue Value.unit >> Return)


let rec statement() : AST list Rule =
  let _if: AST Rule = SequenceOf {
    do! !"if"
    let! cond = bracketed
    let! thn = code_body
    match! Optional !"else" with
    |Some () -> let! els = code_body in return If(cond, Block thn, Block els)
    |None -> return If(cond, Block thn, Value.unit)
   }
  let _while: AST Rule = SequenceOf {
    do! !"while"
    let! cond = bracketed
    let! loop = code_body
    return While(cond, Block loop)
   }
  let _for: AST Rule = SequenceOf {
    do! !"for"
    do! !"("
    let! decl = Optional (declare_expr |/ expr() ->/ List.singleton)
    let decl = Option.defaultValue [] decl
    do! !";"
    let! cond = Optional (expr())
    let cond = Option.defaultValue (V(Lit("1", Byte))) cond
    do! !";"
    let! incr = Optional (expr())
    let incr = Option.defaultValue Value.unit incr
    do! !")"
    let! body = code_body
    let loop_body = Block (body @ [incr])
    return Block (decl @ [While(cond, loop_body)])
   }
  OneOf [
    OneOf [_if; _for; _while] ->/ List.singleton
    return_expr +/ !";" ->/ (fst >> List.singleton)  // keywords (eg. if, while, return) before declare_expr
    declare_expr +/ !";" ->/ fst
    expr() +/ !";" ->/ (fst >> List.singleton)  // a * b gets incorrectly parsed as decl (b: a*)
    !";" ->/ fun _ -> []
   ]
and code_block: AST list Rule =
  !"{" +/ OptionalListOf (statement()) +/ !"}" ->/ (middle >> List.concat)
and code_body: AST list Rule = statement() |/ code_block


let declare_function: AST list Rule =
  let arg_list: (string * DT) list Option Rule =
    let single_arg = SequenceOf {
      let! dt = datatype
      match! declarable_expr dt with
      |[Declare(name, dt)] | [Declare(name, (TypeDef (Array _) as dt)); _] -> return (name, dt)
      |Strict x -> return x
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
    let (args, func_dt) =
      match args with
      |Some args -> let arg_dts = List.map snd args in (args, DT.Function(arg_dts, return_dt))
      |None -> ([], DT.Function2 return_dt)
    match! code_block ->/ Some |/ !";" ->/ fun _ -> None with
    |Some body -> return [
      Declare(name, func_dt)
      Assign(V(Var(name, func_dt)), Function(return_dt, args, Block body))
     ]
    |None -> return [Declare(name, func_dt)]
   }