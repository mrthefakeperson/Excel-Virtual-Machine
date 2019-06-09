module Transformers.Tables
open CompilerDatatypes.DT

type VarName = string
type VarInfo = Local of register: int * DT | Global of DT

type SymbolTable = {
  next_register: int  // 4-byte-aligned, next_register is the address to alloc from stack
  var_lookup: Map<VarName, VarInfo>
  return_type: DT Option  // return type in current function for typecheck

  mutable max_register: int
  // mutable next_label: int
 }
  with
    member x.register_local_var vname dt =
      match dt with T _ -> failwithf "variable %s has ambiguous type?" vname | _ -> ()
      let next_register = x.next_register + (dt.sizeof + 3) / 4
      x.max_register <- max x.max_register next_register
      { x with
          next_register = next_register
          var_lookup = Map.add vname (Local(x.next_register, dt)) x.var_lookup }

    member x.register_global_var vname dt =
      { x with var_lookup = Map.add vname (Global dt) x.var_lookup }

    member x.get_var vname =
      Map.tryFind vname x.var_lookup
       |> Option.defaultWith (fun () -> failwithf "variable %s not registered" vname)

    // member x.get_label s =
    //   try sprintf "%s_%i" s x.next_label
    //   finally x.next_label <- x.next_label + 1

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
  var_lookup = Map (List.map (fun (name, dt) -> (name, Global dt)) builtins)
  return_type = None
  max_register = 0
 }