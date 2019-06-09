module Parser.Preproc
open System.IO
open Utils
open RegexUtils
open CompilerDatatypes.Token

let (|Directive|_|) (content: string) =
  match content.Trim() with
  |Regex PREPROCESSOR_LINE _ ->
    let split = Seq.findIndex ((<>) ' ') content
    let (dir, content) = (content.[..split - 1], content.[split + 1..])
    Some(dir, content.Trim().Split())
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

type PreprocessState = {
  macros: Map<string, string Option>
 }

let preprocessor_pass: Token list -> Token list =
  let rec preprocess state = function
    |[] -> []
    |{value = Lit(Directive("#define", content), _)}::_ ->
      match content with
      |_ -> failwith "macros not supported"
    |{value = Lit(Directive(dir, _), _)}::_ -> failwithf "directive not supported: %s" dir
    |hd::tl -> hd::preprocess state tl
  preprocess {macros = Map.empty}
