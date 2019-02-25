module Codegen.Tables
open Parser.AST

type VarInfo = Local of register: int * size: int | Global of size: int

type SymbolTable = {
  next_register: int  // registers are 4-byte-aligned, next_register is the next multiple of 4 bytes to alloc from stack
  var_to_register: (string * int) list  // register allocation tracked using a stack
  var_to_type: Map<string, Datatype>
  struct_type_to_properties: Map<string, (string * int) list>
  globals: Set<string>
  return_type: Datatype Option  // for typecheck, return type in current function

  // mutables
  mutable max_register: int
  mutable next_label: int
 }
  with
    member x.register_var vname typ =
      match typ with Unknown _ -> failwithf "variable %s has ambiguous type?" vname | _ -> ()
      let next_register = x.next_register + (typ.sizeof + 3) / 4
      x.max_register <- max x.max_register next_register
      { x with
          next_register = next_register
          var_to_type = x.var_to_type.Add(vname, typ)
          var_to_register = (vname, x.next_register)::x.var_to_register }

    // assert correct parsing metadata about vname, get (register, size) or only size if global
    member x.check_var vname (typ: Datatype) =
      if not (Map.containsKey vname x.var_to_type) then
        failwithf "variable %s not registered" vname
      if not (typ.is_superset_of x.var_to_type.[vname]) then
        failwithf "variable %s (type %A) is not of type %A" vname x.var_to_type.[vname] typ
      match List.tryFind (fst >> (=) vname) x.var_to_register with
      |Some(_, register) ->
        let size = x.var_to_type.[vname].sizeof
        Local(register, size)
      |None when Set.contains vname x.globals ->
        let size = x.var_to_type.[vname].sizeof
        Global size
      |None -> Global 0  // builtin

    member x.register_global_var vname t =
      { x with
          globals = Set.add vname x.globals
          var_to_type = x.var_to_type.Add(vname, t) }

    member x.check_global_var = x.globals.Contains

    member x.get_label s =
      try sprintf "%s_%i" s x.next_label
      finally x.next_label <- x.next_label + 1

let builtins = [
  "+", tf_arith_infix
  "-", tf_arith_infix
  "*", tf_arith_infix
  "/", tf_arith_infix
  "%", tf_arith_infix
  // TODO: named type parameters (eg. Function([Unknown([], name = "t")], Ptr("t", "t".sizeof)))
  //                               or Function([Unknown([], name = "t"); "t"], "t")
  // the following are temporary
  "&prefix", Unknown [for t in TypeClasses.ptr_compatible -> Datatype.Function([t], Pointer(t, None))]  //Unknown [for t in [Int; Char; Long; Void; Float; Double; Pointer(Unknown [], None)] -> Datatype.Function([t], Pointer(t, None))]
  "*prefix", Unknown [for t in TypeClasses.ptr_compatible -> Datatype.Function([Pointer(t, None)], t)] //Unknown [for t in [Int; Char; Long; Void; Float; Double; Pointer(Unknown [], None)] -> Datatype.Function([Pointer(t, None)], t)]

  "&&", tf_logic_infix
  "||", tf_logic_infix
  "==", tf_logic_infix
  "<", tf_logic_infix
  ">", tf_logic_infix
  "<=", tf_logic_infix
  ">=", tf_logic_infix

  // TEMP: this probably shouldn't be builtin
  "printf", Datatype.Function([t_any], Void)
 ]

let empty_symbol_table() = {
  next_register = 1
  var_to_register = []
  var_to_type = Map builtins
  struct_type_to_properties = Map.empty
  globals = Set []
  return_type = None

  max_register = 0
  next_label = 1
 }

