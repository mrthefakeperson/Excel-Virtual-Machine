module CompilerDatatypes.Semantics
open CompilerDatatypes.DT

module RegisterAlloc =
  type VarName = string
  type VarInfo = Local of register: int * DT | Global of DT

module InterpreterValue =
  type Boxed =
    |Int of int
    |Int64 of int64
    |Byte of byte
    |Float of float32
    |Double of float
    |Void
    |Ptr of addr: int * DT
    with
      member x.datatype =
        match x with
        |Int _ -> DT.Int
        |Int64 _ -> DT.Int64
        |Byte _ -> DT.Byte
        |Float _ -> DT.Float
        |Double _ -> DT.Double
        |Void -> DT.Void
        |Ptr(_, dt) -> DT.Ptr dt
      static member check_type (dt: DT) (x: Boxed) =
        let dt' = (Boxed.default_value dt).datatype
        if x.datatype <> dt' then failwithf "type check failed: %A <> %A (from %A)" x dt' dt

      member x.to_int64 =
        match x with
        |Int x -> Some (int64 x)
        |Int64 x -> Some x
        |Byte x -> Some (int64 x)
        |Ptr(x, _) -> Some (int64 x)
        |_ -> None
      member x.to_double =
        match x.to_int64, x with
        |Some x, _ -> Some (float x)
        |_, Float x -> Some (float x)
        |_, Double x -> Some x
        |_ -> None
      static member inline from_t dt (x: 'a) =
        try
          match dt with
          |DT.Void -> Some <| Void
          |DT.Int -> Some <| Int (int x)
          |DT.Int64 -> Some <| Int64 (int64 x)
          |DT.Byte -> Some <| Byte (byte x)
          |DT.Float -> Some <| Float (float32 x)
          |DT.Double -> Some <| Double (float x)
          |DT.Ptr dt -> Some <| Ptr(int x, dt)
          |DT.Function _ | DT.Function2 _ -> Some <| Ptr(int x, DT.Void)
          |_ -> None
        with
          _ -> None
      static member from_int64 dt (x: int64) = Boxed.from_t dt x
      static member from_double dt (x: double) = Boxed.from_t dt x
      static member from_string dt (x: string) =
        try
          match dt with
          |DT.Byte when x.Length = 3 && x.[0] = ''' && x.[2] = ''' ->
            let c = byte x.[1]
            Some <| Byte c
          |DT.Int64 -> Boxed.from_t dt (x.TrimEnd 'L')
          |_ -> Boxed.from_t dt x
        with :? System.FormatException -> None
      static member default_value dt : Boxed =
        match Boxed.from_int64 dt 0L, dt with
        |Some x, _ -> x
        |None, DT.Void -> Void
        |None, unknown -> failwithf "no default value for %A" unknown
      static member cast dt (x: Boxed) =
        let fail () = failwithf "can't cast %A to %A" x dt
        Option.defaultWith fail (Boxed.from_double dt (Option.defaultWith fail x.to_double))

      static member make_binary_op f_int f_dbl (a: Boxed) (b: Boxed) =
        if a.datatype <> b.datatype
         then failwithf "can't combine %A: datatypes don't match" (a, b)
         else
          match a.to_int64, b.to_int64 with
          |Some a', Some b' -> Choice1Of2 (f_int a' b')
          |_ ->
            match a.to_double, b.to_double with
            |Some a'', Some b'' -> Choice2Of2 (f_dbl a'' b'')
            |_ -> failwithf "can't combine %A: couldn't normalize one of the operands" (a, b)
      static member make_arith_op f_int f_dbl a b =
        match Boxed.make_binary_op f_int f_dbl a b with
        |Choice1Of2 i64 -> Option.get <| Boxed.from_int64 a.datatype i64
        |Choice2Of2 dbl -> Option.get <| Boxed.from_double b.datatype dbl
      static member (+) (a, b) = Boxed.make_arith_op (+) (+) a b
      static member (-) (a, b) = Boxed.make_arith_op (-) (-) a b
      static member ( * ) (a, b) = Boxed.make_arith_op ( * ) ( * ) a b
      static member (/) (a, b) = Boxed.make_arith_op (/) (/) a b
      static member (%) (a, b) = Boxed.make_arith_op (%) (%) a b

      static member make_cmp_op (=><) a b =
        let cmp_i x y = if double x =>< double y then 1L else 0L
        let cmp_d x y = if x =>< y then 1. else 0.
        match Boxed.make_binary_op cmp_i cmp_d a b with
        |Choice1Of2 0L | Choice2Of2 0. -> Byte 0uy
        |_ -> Byte 1uy
      member x.equals y = Boxed.make_cmp_op (=) x y
      member x.greater_than y = Boxed.make_cmp_op (>) x y
      member x.less_than y = Boxed.make_cmp_op (<) x y