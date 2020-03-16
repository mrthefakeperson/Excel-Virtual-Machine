module Parser.Main
open ParserCombinators
open CompilerDatatypes.Token
open CompilerDatatypes.AST
open CompilerDatatypes.AST.SyntaxAST
open Parser.Parse

let parse_global_scope: AST Expr.Rule =
  let try_parse_decl =
    OneOf [
      !";" ->/ fun () -> []
      Control.declare_function
      Control.declare_expr +/ !";" ->/ fst
      Typedef.parse_typedecl
     ]
  ListOf try_parse_decl +/ (lazy End |/ try_parse_decl ->/ ignore)  // try_parse_decl in parallel with EOF to get correct error messages
   ->/ (fst >> List.concat >> GlobalParse)

let parse_tokens_to_ast_with: AST Expr.Rule -> Token list -> AST = fun rule tokens ->
  run_parser rule {stream = tokens}

let parse_string_to_ast: string -> AST =
  Lex.tokenize_text
   >> Preproc.preprocessor_pass
   >> parse_tokens_to_ast_with parse_global_scope

let parse_string_to_expr: string -> AST =
  Lex.tokenize_text >> parse_tokens_to_ast_with (Expr.expr())

let parse_string_to_control: string -> AST =
  Lex.tokenize_text >> parse_tokens_to_ast_with (Block <-/ Control.code_body)

open Utils
[<EntryPoint>]
let main argv =
  let flags = CLI.get_cli_flags argv
  let inline interact repl = CLI.interact "Parser CLI" repl
  match () with
  |_ when flags.ContainsKey "-l" || flags.ContainsKey "--lex" ->
    interact Parser.Lex.tokenize_text
  |_ when flags.ContainsKey "-x" || flags.ContainsKey "--expr" ->
    interact (parse_string_to_expr >> pprint_ast_structure)
  |_ when flags.ContainsKey "-c" || flags.ContainsKey "--control" ->
    interact (parse_string_to_control >> pprint_ast_structure)
  |_ -> interact (parse_string_to_ast >> pprint_ast_structure)
  0