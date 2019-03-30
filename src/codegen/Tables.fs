module Codegen.Tables
open Parser.Datatype

type VarInfo = Local of register: int * size: int | Global of size: int

type SymbolTable = {
  next_register: int  // registers are 4-byte-aligned, next_register is the next multiple of 4 bytes to alloc from stack
  var_to_register: (string * int) list  // register allocation tracked using a stack
  var_to_type: Map<string, DT>
  struct_type_to_properties: Map<string, (string * int) list>
  globals: Set<string>
  return_type: DT Option  // for typecheck, return type in current function

  // mutables
  mutable max_register: int
  mutable next_label: int
 }
  with
    member x.register_var vname typ =
      match typ with T _ -> failwithf "variable %s has ambiguous type?" vname | _ -> ()
      let next_register = x.next_register + (typ.sizeof + 3) / 4
      x.max_register <- max x.max_register next_register
      { x with
          next_register = next_register
          var_to_type = x.var_to_type.Add(vname, typ)
          var_to_register = (vname, x.next_register)::x.var_to_register }

    // assert correct parsing metadata about vname, get (register, size) or only size if global
    member x.check_var vname dt =
      if not (Map.containsKey vname x.var_to_type) then
        failwithf "variable %s not registered" vname
      //try ignore <| DT.infer_type dt x.var_to_type.[vname]
      //with _ -> failwithf "variable %s (type %A) is not of type %A" vname x.var_to_type.[vname] dt
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
  "+", TypeClasses.f_arith_infix
  "-", TypeClasses.f_arith_infix
  "*", TypeClasses.f_arith_infix
  "/", TypeClasses.f_arith_infix
  "%", TypeClasses.f_arith_infix

  "&prefix", DT.Function([TypeClasses.any' 0], Ptr (TypeClasses.any' 0))
  "*prefix", DT.Function([Ptr (TypeClasses.any' 0)], TypeClasses.any' 0)
  "-prefix", TypeClasses.f_arith_prefix

  "&&", TypeClasses.f_logic_infix
  "||", TypeClasses.f_logic_infix
  "==", TypeClasses.f_logic_infix
  "<", TypeClasses.f_logic_infix
  ">", TypeClasses.f_logic_infix
  "<=", TypeClasses.f_logic_infix
  ">=", TypeClasses.f_logic_infix

  "\stack_alloc", DT.Function([Int], TypeClasses.ptr)

  // TEMP: this probably shouldn't be builtin
  "printf", DT.Function2 Void
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

