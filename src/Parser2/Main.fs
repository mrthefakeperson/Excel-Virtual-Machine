module Parser.Main
open ParserCombinators
open CompilerDatatypes.Token
open CompilerDatatypes.AST
open Parser.Parse

let parse_global_scope: AST Expr.Rule =
  let try_parse_decl =
    OneOf [
      Control.declare_function
      Control.declare_expr +/ !";" ->/ fst
      !";" ->/ fun () -> []
      Typedef.parse_typedecl
     ]
  ListOf try_parse_decl +/ (End |/ try_parse_decl ->/ ignore)  // try_parse_decl in parallel with EOF to get correct error messages
   ->/ (fst >> List.concat >> GlobalParse)

let parse_tokens_to_ast_with: AST Expr.Rule -> Token list -> AST = fun rule tokens ->
  run_parser rule {stream = tokens}

let parse_tokens_to_ast: Token list -> AST = parse_tokens_to_ast_with parse_global_scope

let parse_string_to_ast: string -> AST =
  Lex.tokenize_text >> Preproc.preprocessor_pass >> parse_tokens_to_ast