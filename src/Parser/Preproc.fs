module Parser.Preproc
open System.IO
open Utils
open RegexUtils
open CompilerDatatypes.Token

let (|Directive|_|) (content: string) =
  match content.Trim() with
  |Regex PREPROCESSOR_LINE _ ->
    let split = Seq.findIndex ((=) ' ') content
    let (dir, content) = (content.[..split - 1], content.[split + 1..])
    let content = Array.filter ((<>) "") (content.Trim().Split())
    Some(dir, content)
  |_ -> None

let builtin_files: Map<string, string> =
  Map [
    "stdio.h", ""
    "limits.h", ""
    "string.h", ""
   ]

let get_include: string[] -> SourceFile * string = function
  |[|Regex "^<(.*)>$" mtch|] ->
    let filename = mtch.Groups.[1].Value
    match Map.tryFind filename builtin_files with
    |Some src -> (Builtin filename, src)
    |None -> failwithf "bad include: could not find <%s>" filename
  |[|Regex "^\"(.*)\"$" mtch|] ->
    let filename = mtch.Groups.[1].Value
    try (Local filename, File.ReadAllText filename)
    with :? FileNotFoundException -> failwithf "bad include: could not find %A" filename
  |_ -> failwith "bad include"

let preprocessor_pass: Token list -> Token list =
  let rec preprocess (state: {| macros: Map<string, string> |}) = function
    |[] -> []
    |{value = Lit(Directive("#define", content), _)}::tl ->
      let state =
        match content with
        |[|token1; token2|] -> {|state with macros = Map.add token1 token2 state.macros|}
        |_ -> failwith "macro not supported"
      preprocess state tl
    |{value = Lit(Directive(dir, _), _)}::_ -> failwithf "directive not supported: %s" dir
    |{value = Var(token, dt) | Lit(token, dt)} as hd::tl when Map.containsKey token state.macros ->
      let substitution = state.macros.[token]
      let constructor =
        match substitution with
        |Regex RegexUtils.VAR _ -> Var
        |_ -> Lit
      {hd with value = constructor(substitution, dt)}::preprocess state tl
    |hd::tl -> hd::preprocess state tl
  preprocess {|macros = Map []|}
