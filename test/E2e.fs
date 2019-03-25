module E2e
open Fuchu
open System.IO
open TestUtils

let test_stage name (stage: 'a -> 'b) (data: 'a list) =
  printfn "Stage: %s" name
  let results = List.map stage data
  List.iteri (fun i ->
    try approval_test (Path.Combine(name, sprintf "%i.out" i))
    finally printf "."
   ) results
  printfn ""
  results

let file_data =
  Path.Combine(__SOURCE_DIRECTORY__, "../samples")
    |> Directory.GetFiles
    |> Array.sortBy (fun f -> int(Path.GetFileName(f).Replace(".c", "")))
    |> Seq.take 18  // partial test for now - 19
    |> Seq.map File.ReadAllText
    |> List.ofSeq

open Lexer.Token
let lexer_stage: string list -> Token list list = test_stage "lexer" Lexer.Main.tokenize_text

open Parser.AST
let parser_stage: Token list list -> AST list = test_stage "parser" Parser.Main.parse_tokens_to_ast

open Codegen.Hooks
open Codegen.TypeCheck
let ast_process_stage: AST list -> AST list =
  test_stage "typecheck" (
    apply_mapping_hook transform_sizeof_hook
     >> apply_mapping_hook extract_strings_to_global_hook
     >> apply_mapping_hook convert_logic_hook
     >> apply_mapping_hook prototype_hook
     >> check_type (Codegen.Tables.empty_symbol_table()) >> fst
   )
   
open Codegen.PAsm
open Codegen.Interpreter
let interpret_ast_stage: AST list -> (Boxed * string) list = test_stage "interpret-AST" Codegen.Interpreter.preprocess_eval_ast

open Codegen.PAsm.Flat
let codegen_stage: AST list -> Asm list list = test_stage "codegen-PAsm" Codegen.Main.generate_from_ast

//let interpret_pasm_stage: Asm list list -> Codewriters.Interpreter.State list =
//  test_stage "interpret-PAsm" Codewriters.Interpreter.eval

let test() =
  testCase "End to end test" <|
      fun _ ->
        let ast =
          file_data
           |> lexer_stage
           |> parser_stage
        //let _ = ast |> ast_process_stage
        let _ = ast |> interpret_ast_stage
        let _ =
          ast
           |> codegen_stage
        //   |> interpret_pasm_stage
        ()
   |> run
   |> ignore