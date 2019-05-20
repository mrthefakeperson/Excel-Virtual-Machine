module Lexer.Main
open System
open Utils
open RegexUtils
open ParserCombinators
open ParserCombinators.Atoms.String
open CompilerDatatypes.DT
open CompilerDatatypes.Token
open Lexer.Preprocessor

let comment = %%COMMENT_LINE |/ %%COMMENT_BLOCK
let garbage =
  %%WHITESPACE +/ comment +/ %%WHITESPACE ->/ fun ((a, b), c) -> a.Length + b.Length + c.Length
let lexeme tkn (rule: Rule<string, Value>) : Rule<string, Token> = fun input ->
  match (garbage +/ rule) input with
  |Ok(rest, (spaces, res)) -> Ok(rest, {tkn with value = res; col = -input.Length + spaces})
  |Error msg -> Error msg

let token_num = SequenceOf {
  let! radix =
    OneOf [
      !"0x" ->/ fun _ -> 16
      !"0" ->/ fun _ -> 8
      !"" ->/ fun _ -> 10
     ]
  let! integer = %%"[0-9]+"
  let! has_dot = Optional !"." ->/ Option.isSome
  let! decimal = %%"[0-9]*"
  let! type_suffix = %%"[A-Za-z]*"
  return
    match (radix, has_dot, type_suffix.ToLower()) with
    |(_, false, "") -> Lit(string (Convert.ToInt32(integer, radix)), DT.Int)
    |(_, false, ("l" | "ll")) -> Lit(string (Convert.ToInt64(integer, radix)), DT.Int64)
    |(10, true, "f") -> Lit(string (Convert.ToSingle(integer + "." + decimal)), DT.Float)
    |(10, true, ("" | "lf" | "llf")) ->
      Lit(string (Convert.ToDouble(integer + "." + decimal)), DT.Double)
    |_ -> failwith "unknown integer literal"
 }

let token_var = %%"[A-Za-z_]\w*" ->/ fun name -> Var(name, DT.Void)

let escape = SequenceOf {
  do! !"\\"
  let! c = %%"."
  return
    match c with
    |"n" -> '\n' | "t" -> '\t' | "r" -> '\r' | "f" -> '\f' | "\\" -> '\\' | "0" -> '\000'
    |_ -> failwithf "unknown escape character: \\%s" c
 }

let token_string = SequenceOf {
  do! !"\""
  let! chars = OptionalListOf (escape ->/ string |/ %%"[^\"\\\\]")
  do! !"\""
  return Lit("\"" + String.concat "" chars + "\"", DT.Ptr DT.Byte)
 }

let token_char = SequenceOf {
  do! !"'"
  let! c = escape
  do! !"'"
  return Lit(string (byte c), DT.Byte)
 }

let token_symbol =
  (%%"(--|\+\+|>=|<=|==|!=|&&|\|\||&|\+=|-=|\*=|/=|&=|\|=|->)" |/ %%"[{}();,=+\-*/%<>\[\].:]")
   ->/ fun name -> Lit(name, DT.Void)

let preprocess = %%PREPROCESSOR_LINE ->/ fun name -> Lit(name, DT.Void)

let parse_tokens tkn : Rule<String, Token list> =
  let parse_token =
    OneOf [token_num; token_var; token_string; token_char; token_symbol; preprocess]
  OptionalListOf (lexeme tkn parse_token) +/ (garbage +/ End) ->/ fst

let rec tokenize filename (s: string) : Token list =
  let rec get_rc i =
    if i = 0 then (0, 0)
    elif s.[i - 1] = '\n' then let (line, _) = array_rc.[i - 1] in (line + 1, 0)
    else let (line, col) = array_rc.[i - 1] in (line, col + 1)
  and array_rc: (int * int)[] = Array.init s.Length get_rc
  run_parser (parse_tokens {deftkn with file = filename}) s
   |> List.map (fun tkn ->
        let pos = s.Length + tkn.col
        let (line, col) = array_rc.[pos]
        {tkn with line = line; col = col}
       )
   |> List.collect (function
        |{value = Lit(Directive("#include", incl), _)} ->  // handle this as a special case for recursion
          let (file, src) = get_include incl
          tokenize file src
        |tkn -> [tkn]
       )
   |> preprocessor_pass

let tokenize_text = tokenize (Builtin "source")
