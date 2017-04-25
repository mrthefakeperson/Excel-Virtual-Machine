module private AST.Compile
open Parser.Definition
open AST.Definition

let nxt' x () =
  incr x
  string !x
let nxt = nxt' (ref 0)
let rec ASTCompile' (capture, captured as cpt) = function
  |X("return", xl) ->
    match xl with
    |[] -> AST.Return (Const "()")
    |[x] -> AST.Return (ASTCompile' cpt x)
    |_ -> failwith "cannot return more than one item"
  |Var s -> if Map.containsKey s captured && captured.[s] <> [] then Apply(Value s, captured.[s]) else Value s   //variables
  |Cnst s -> Const s    //constants, could use some work
  |X(",", tupled) ->
    let name = "$tuple" + nxt()
    let allocate = [Declare(name, New(Const(string(List.length tupled))))]
    let assignAll = List.mapi (fun i e -> Assign(Value name, Const(string i), ASTCompile' cpt e)) tupled
    let returnVal = [Value name]
    Sequence (allocate @ assignAll @ returnVal)
  |X("apply", [a; b]) -> Apply(ASTCompile' cpt a, [ASTCompile' cpt b])
  |X("fun", [x; b]) ->
    let rec unpack arg = function
      |X("declare", [_; T s])
      |T s -> [Declare(s, arg)], [s]
      |X(",", xprs) ->
        let compiled, extractedVars =
          List.mapi (fun i -> unpack (Get(arg, Const(string i)))) xprs
           |> List.unzip
        List.concat compiled, List.concat extractedVars
      |e -> failwithf "could not unpack %A" e
    let argName = "$arg" + nxt()
    let extractCode, extractedVars = unpack (Value argName) x
    let L, K = "L" + nxt(), "K" + nxt()
    let cptr'd = List.map Value capture
    let cpt' = extractedVars @ capture, Map.add L cptr'd captured
    let functionBody = Sequence (extractCode @ [ASTCompile' cpt' b])
    Sequence [
      yield Define(L, argName::capture, functionBody)
      match ASTCompile' cpt' (Token(L, [])) with
      |Apply(a, args) ->
        yield Declare(K, New (Const(string(List.length args + 1))))
        yield Assign(Value K, Const "0", Get(a, Const "0"))
        yield! List.mapi (fun i e ->
          Assign(Value K, Const(string(i + 1)), e)
         ) args
        yield Value K
      |compiled -> yield compiled
     ]
  |X("declare", [datatypeName; a]) -> ASTCompile' cpt a
  |X("let", [a; b]) ->           //todo: handle non-recursive statements by renaming
    match a with
    |X("apply", [aa; ab]) -> ASTCompile' cpt (Token("let", [aa; Token("fun", [ab; b])]))
    |T s | X("declare", [_; T s]) ->
      Declare(s, ASTCompile' cpt b)
    |e -> failwithf "patterns in function arguments not supported yet: %A" e
  |X("let rec", [a; b]) -> ASTCompile' cpt (Token("let", [a; b]))
  |X("array", [sz]) -> New (ASTCompile' cpt sz)
  |X("dot", [a; b]) ->
    match b with
    |X("[]", [i]) -> Get(ASTCompile' cpt a, ASTCompile' cpt i)
    |_ -> failwith "should never happen"
  |X("assign", [a; b]) ->
    match a with
    |X(name, []) -> Mutate(name, ASTCompile' cpt b)
    |X("dot", [a; X("[]", [i])]) ->
      Assign(ASTCompile' cpt a, ASTCompile' cpt i, ASTCompile' cpt b)
    |X("deref", [a]) -> Assign(ASTCompile' cpt a, Const "0", ASTCompile' cpt b)
    |_ -> failwith "todo: unpacking"
  |X("if", [cond; aff; neg]) ->
    If(ASTCompile' cpt cond, ASTCompile' cpt aff, ASTCompile' cpt neg)
  |X("do", [b]) -> Apply(Value "ignore", [ASTCompile' cpt b])
  |X("while", [cond; b]) -> Loop(ASTCompile' cpt cond, ASTCompile' cpt b)
  |X("for", [name; iterable; body]) ->
    let name = match name with X(name, []) -> name | _ -> failwith "todo: unpacking"
    match iterable with
    |X("..", [a; step; b]) ->
      Sequence [    //todo: negative step values
        Declare(name, ASTCompile' cpt a)      //no need to capture, since no part of the code can touch it
        Loop(Apply(Apply(Value "<=", [Value name]), [ASTCompile' cpt b]),
          Sequence
           [ASTCompile' cpt body; Mutate(name, Apply(Apply(Value "+", [Value name]), [ASTCompile' cpt step]))]
         )
       ]
    |_ -> failwith "iterable objects not supported yet"
  |X("deref", [x]) -> Get(ASTCompile' cpt x, Const "0")
  |X("sequence", list) ->
    List.fold (fun (acc, (capt', capd' as cpt')) e ->
      let compiled = ASTCompile' cpt' e
      let cpt' =
        match compiled with
        |Declare(a, _) -> (a::capt', capd')
        |Define(a, _, _) -> (a::capt', Map.add a (List.map Value capt') capd')
        |_ -> cpt'
      (compiled::acc, cpt')
     ) ([], cpt) list
      |> fst |> List.rev
      |> Sequence
  
  |unknown -> failwithf "unknown: %A" unknown
let transformFromToken e = ASTCompile' ([], Map.empty) e

