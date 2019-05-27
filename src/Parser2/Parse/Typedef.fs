module Parser.Parse.Typedef
open Utils
open RegexUtils
open ParserCombinators
open CompilerDatatypes.DT
open CompilerDatatypes.Token
open CompilerDatatypes.AST
open Parser.Parse.Expr
open Parser.Parse.Control

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