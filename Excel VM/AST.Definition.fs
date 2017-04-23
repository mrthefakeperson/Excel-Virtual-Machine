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
  |Return of a:AST option
  |Loop of a:AST*b:AST
  |Mutate of a:string*b:AST     //a <- b
   with
    override x.ToString() =
      let rec str indent = function
        |Sequence ll -> List.map (str indent) ll |> String.concat "\n"
        |Declare(a, b) -> indent + sprintf "let %s =\n%s" a (str (indent+"  ") b)
        |Define(a, args, b) -> indent + sprintf "define %s(%s):\n%s" a (String.concat ", " args) (str (indent+"  ") b)
        |Value s | Const s -> indent + s
        |Apply(a, args) -> indent + sprintf "%s(%s)" (str "" a) (String.concat ", " (List.map (str "") args))
        |If(a, b, c) -> indent + sprintf "if %s then\n%s\n" (str "" a) (str (indent+"  ") b) + indent + "else\n" + str (indent+"  ") c
        |New n -> indent + sprintf "alloc %s" (str "" n)
        |Get(a, i) -> indent + sprintf "%s[%s]" (str "" a) (str "" i)
        |Assign(a, i, e) -> indent + sprintf "%s[%s] <- %s" (str "" a) (str "" i) (str "" e)
        |Return None -> indent + "return"
        |Return(Some x) -> indent + sprintf "return\n%s" (str (indent+"  ") x)
        |Loop(a, b) -> indent + sprintf "while %s do\n%s\n" (str "" a) (str (indent+"  ") b)
        |Mutate(a, b) -> indent + sprintf "%s <-\n%s" a (str (indent+"  ") b)
      str "" x
let (|Children|) = function
  |Sequence ll -> ll
  |Declare(_, c) | Define(_, _, c) |New c | Mutate(_, c) | Return(Some c) -> [c]
  |Value _ | Const _ | Return None -> []
  |Get(a, b) | Loop(a, b) -> [a; b]
  |Assign(a, b, c) | If(a, b, c) -> [a; b; c]
  |Apply(a, ll) -> a::ll
