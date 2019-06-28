module CompilerDatatypes.DT

type TypeDef =
  |DeclAlias of string * DT | DeclStruct of StructDef | DeclUnion of StructDef  // define a new type and declare a variable using it
  |Alias of string | Struct of string | Union of string  // declare a variable of a struct/union type already defined (replace in preprocessing)
  |Array of dim: int * DT
and StructDef = string * StructFieldDef list
and StructFieldDef = StructField of name: string * DT * start_bit: int * size_bytes: int

and DT =
  |Int | Byte | Int64 | Void | Float | Double
  |Ptr of DT
  |Function of args: DT list * ret: DT
  |Function2 of ret: DT  // non-explicit args; symbolizes any function when used in type parameter
  |T of identifier: int * valid_types: Lazy<DT list>  // type parameter
  |TypeDef of TypeDef
  with
    member x.sizeof =
      match x with
      |Int -> 4
      |Byte -> 1
      |Int64 -> 8
      |Void -> 0
      |Float -> 4
      |Double -> 8
      |Ptr _ -> 4
      |Function _ -> 4
      |Function2 _ -> 4
      |T _ -> failwith "cannot get size of T _"
      |TypeDef (DeclStruct(_, fields) | DeclUnion(_, fields)) ->
        0::List.map (fun (StructField(_, _, start, sz)) -> start + sz) fields
         |> List.max
         |> fun sz_bits -> (sz_bits + 7) / 8
      |TypeDef _ -> failwith "unexpected typedef - should be removed at this point"

    static member ptr_equivalent = function
      |Ptr Byte | TypeDef (DeclStruct _ | DeclUnion _ | Array _) -> true
      |_ -> false

    static member can_promote from to_ =
      let hierarchy = [Byte; Int; Int64; Float; Double]
      if from = to_ then true
      else
        match List.tryFindIndex ((=) from) hierarchy, List.tryFindIndex ((=) to_) hierarchy with
        |Some ai, Some bi when ai < bi -> true
        |Some _, None when DT.ptr_equivalent to_ -> true
        |Some _, None -> match to_ with Ptr _ -> true | _ -> false
        |_ -> match to_ with Ptr Byte -> DT.ptr_equivalent from | _ -> false
    static member supertype a b =  // automatic casting: when some type wants to be both a and b, choose one of them
      if DT.can_promote a b then b
      elif DT.can_promote b a then a
      else failwithf "cannot decide between type %A and %A" a b

    static member valid_types_map = function
      |T(x, ts) -> Map [(x, ts)]
      |Ptr t -> DT.valid_types_map t
      |Function(args, ret) ->
        let combine = Map.fold (fun acc k v -> Map.add k v acc)
        ret::args |> List.map DT.valid_types_map |> List.reduce combine
      |Function2 ret -> DT.valid_types_map ret
      |_ -> Map.empty

    static member inference_map model concrete : Map<int, DT list> =  // identifier -> inferred types
      match model, concrete with
      |T(x, _), T _ -> Map [(x, [])]  // put an abstract type in concrete to allow other inferences to take priority (eg. unknown return value)
      //|_, T _ -> failwith "inferring from non-concrete type"
      |T(x, _), _ -> Map [(x, [concrete])]
      |Ptr t, Ptr t' -> DT.inference_map t t'
      |Function(args, ret), Function(args', ret') ->
        let combine =
          Map.fold (fun acc k v ->
            let v' = Option.defaultValue [] (Map.tryFind k acc) @ v
            Map.add k v' acc
           )
        List.map2 DT.inference_map (ret::args) (ret'::args') |> List.reduce combine
      |Function2 ret, (Function2 ret' | Function(_, ret')) -> DT.inference_map ret ret'
      |_ -> Map.empty

    static member infer_type' model concrete : DT =
      let valid_types = DT.valid_types_map model
      let inferences =
        DT.inference_map model concrete
         |> Map.map (fun identifier types ->
              if types = [] then failwithf "type %A has no concrete candidates" identifier
              let inferred = List.reduce DT.supertype types
              let inferred = if DT.ptr_equivalent inferred then Ptr Byte else inferred
              if List.exists (fun t -> try ignore (DT.infer_type' t inferred); true with _ -> false) (valid_types.[identifier].Force())
               then inferred
               else failwithf "type %A cannot be constrained to %A" identifier inferred
             )
      let rec replace = function  // fill in all T parameters
        |T(x, _) -> inferences.[x]
        |Ptr t -> Ptr (replace t)
        |Function(args, ret) -> Function(List.map replace args, replace ret)
        |Function2 ret -> Function2 (replace ret)
        |t -> t
      let rec cast = function  // promote types if possible
        |T _, _ -> failwith "should never be reached"
        |t, (T _ as t') -> DT.infer_type' t' t  // now t should be the concrete type, so infer t' into it
        |(Ptr _ as model), (Ptr _ | Int | Byte | Int64 | Void | Float | Double)
        |(Int | Byte | Int64 | Float | Double as model), Ptr _ -> model
        |Function(args, ret), Function(args', ret') -> Function(List.map cast (List.zip args args'), cast (ret, ret'))
        |Function2 ret, Function2 ret' -> Function2 (cast (ret, ret'))
        |Function2 ret, Function(args', ret') -> Function(args', cast (ret, ret'))
        |Ptr Byte, target when DT.ptr_equivalent target -> Ptr Byte
        |model, target when DT.can_promote target model -> model
        |x -> failwithf "cannot resolve cast: %A" x
      cast (replace model, concrete)

    static member infer_type model concrete : DT =
      let rec cast = function  // demote types in concrete if possible
        |T _, t -> t
        |Ptr t, Ptr t' -> Ptr (cast (t, t'))
        |Function(args, ret), Function(args', ret') -> Function(List.map cast (List.zip args args'), cast (ret, ret'))
        |Function2 ret, Function2 ret' -> Function2 (cast (ret, ret'))
        |Function2 ret, Function(args', ret') -> Function(args', cast (ret, ret'))
        |model, target when DT.can_promote model target -> model
        |_, t -> t
      DT.infer_type' model (cast (model, concrete))

module TypeClasses =
  let integral n = T(n, lazy [Byte; Int; Int64])
  let rec any' n = T(n, lazy [Byte; Int; Int64; Float; Double; Void; Ptr (any' (n + 1)); Function2 (any' (n + 1))])
  let any = any' 0
  let ptr = Ptr any
  let real n = T(n, lazy [Byte; Int; Int64; Float; Double; ptr])
  let f_unary x1 r = Function([x1], r)
  let f_arith_prefix = f_unary (real 0) (real 0)
  let f_logic_prefix = f_unary (real 0) Byte
  let f_binary x1 x2 r = Function([x1; x2], r)
  let f_arith_infix = f_binary (real 0) (real 0) (real 0)
  let f_logic_infix = f_binary (real 0) (real 0) Byte