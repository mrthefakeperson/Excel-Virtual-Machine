module AST.Definition

type AST =
  |Sequence of AST list
  |Declare of a:string*b:AST      //let a = b
  |Define of a:string*args:string list*b:AST    //define a function
                                                //recursive functions should be differentiated by name changes (referencing a from within a calls a jump to a's address, creating the recursion)
  |Value of string
  |Const of string
  |Apply of a:AST*args:AST list              //a(b1, b2, ...)
  |If of a:AST*b:AST*c:AST
  |New of n:AST               //allocate n new heap spaces
  |Get of a:AST*i:AST           //get array a at i
  |Assign of a:AST*i:AST*e:AST  //set array a at i to e
  |Return of a:AST
  |Break
  |Continue
  |Loop of a:AST*b:AST
  |Mutate of a:string*b:AST     //a <- b
   with
    static member map f = function
      |Apply(a, b) -> Apply(f a, List.map f b)
      |Assign(a, b, c) -> Assign(f a, f b, f c)
      |Break -> Break
      |Const a -> Const a
      |Continue -> Continue
      |Declare(a, b) -> Declare(a, f b)
      |Define(a, bs, c) -> Define(a, bs, f c)
      |Get(a, b) -> Get(f a, f b)
      |If(a, b, c) -> If(f a, f b, f c)
      |Loop(a, b) -> Loop(f a, f b)
      |Mutate(a, b) -> Mutate(a, f b)
      |New a -> New(f a)
      |Return a -> Return(f a)
      |Sequence ll -> Sequence(List.map f ll)
      |Value a -> Value a
    static member iter f = AST.map (fun e -> f e; e) >> ignore
    static member (|Children|) = fun e ->
      let yld = ref []
      AST.iter (fun e -> yld := e:: !yld) e
      List.rev !yld
    override x.ToString() =
      let (|Children|) = AST.``|Children|``
      let isInline = System.Collections.Generic.Dictionary()
      let push ast x =
        isInline.[ast] <- x
        x
      let rec isInline' e = e |> function
        |Value _ | Const _ | Break | Continue -> push e true
        |Sequence ll ->
          ignore (List.map isInline' ll)
          push e false
        |Children ll -> push e (List.map isInline' ll |> List.reduce (&&))
      isInline' x |> ignore
      let (|Inline|_|) ast = if isInline.[ast] then Some ast else None
      let AorB ast a b = if isInline.[ast] then a else b
      let rec str indent e =
        let ind' = indent + "  "
        let a =
          match e with
          |Sequence ll -> (List.map (str indent) ll |> String.concat "\n").[indent.Length..]
          |Loop(Inline a, Inline b) -> sprintf "while %s do %s" (str""a) (str""b)
          |Loop(Inline a, b) -> sprintf "while %s do\n%s" (str""a) (str ind' b)
          |Loop(a, Inline b) -> sprintf "while\n%s\n do %s" (str ind' a) (str""b)
          |Loop(a, b) -> sprintf "while\n%s\n do %s" (str ind' a) (str ind' b)
          |Value s | Const s -> s
          |Apply(a, b) ->
            sprintf "%s%s"
             (AorB a (str""a) (sprintf "(\n%s\n%s )" (str ind' a) indent))
             (sprintf "(%s)"
               (List.map (fun e ->
                  AorB e (str""e) (sprintf "\n%s" (str ind' e))
                 ) b |> String.concat ", "))
          |Declare(a, Inline b) -> sprintf "let %s = %s" a (str""b)
          |Declare(a, b) -> sprintf "let %s =\n%s" a (str ind' b)
          |Define(a, args, Inline b) -> sprintf "define %s(%s): %s" a (String.concat", "args) (str""b)
          |Define(a, args, b) -> sprintf "define %s(%s):\n%s" a (String.concat", "args) (str ind' b)
          |If(a, b, c) ->
            sprintf "if%sthen%selse%s"
             (AorB a (sprintf " %s " (str""a)) (sprintf "\n%s\n%s " (str ind' a) indent))
             (AorB b (sprintf " %s " (str""b)) (sprintf "\n%s\n%s " (str ind' b) indent))
             (AorB c (sprintf " %s" (str""c)) (sprintf "\n%s" (str ind' c)))
          |New a -> sprintf "alloc%s" (AorB a (sprintf " %s" (str""a)) (sprintf "\n%s" (str ind' a)))
          |Get(a, b) ->
            sprintf "%s%s"
             (AorB a (str""a) (sprintf "(\n%s\n%s )" (str ind' a) indent))
             (AorB b (sprintf "[%s]" (str""b)) (sprintf "[\n%s\n%s ]" (str ind' b) indent))
          |Assign(a, b, c) ->
            sprintf "%s%s <- %s"
             (AorB a (str""a) (sprintf "(\n%s\n%s )" (str ind' a) indent))
             (AorB b (sprintf "[%s]" (str""b)) (sprintf "[\n%s\n%s ]" (str ind' b) indent))
             (AorB c (str""c) (sprintf "\n%s" (str ind' c)))
          |Return a -> sprintf "return%s" (AorB a (sprintf " %s" (str""a)) (sprintf "\n%s" (str ind' a)))
          |Break -> "break"
          |Continue -> "continue"
          |Mutate(a, Inline b) -> sprintf "%s <- %s" a (str""b)
          |Mutate(a, b) -> sprintf "%s <-\n%s" a (str ind' b)
        indent + a
      str "" x
let (|Children|) = AST.``|Children|``
