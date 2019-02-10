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
    |> Seq.take 19  // partial test for now
    |> Seq.map File.ReadAllText
    |> List.ofSeq

open Lexer.Token
let lexer_stage: string list -> Token list list = test_stage "lexer" Lexer.Main.tokenize_text

open Parser.AST
let parser_stage: Token list list -> AST list = test_stage "parser" Parser.Main.parse_tokens_to_ast

open Codegen.Interpreter
let interpret_ast_stage: AST list -> Boxed list =
  test_stage "interpret AST" (
    Codegen.TypeCheck.check_type Codegen.Tables.empty_symbol_table >> fst
     >> Codegen.Interpreter.eval_ast (default_memory())
   )

open Codegen.PAsm
let codegen_stage: AST list -> Boxed Asm list list = test_stage "codegen (PAsm)" Codegen.Main.generate_from_ast

let interpret_pasm_stage: Boxed Asm list list -> Codegen.Interpreter.AsmMemory list =
  test_stage "interpret PAsm" Codegen.Interpreter.eval_pasm

let test() =
  testCase "End to end test" <|
      fun _ ->
        let ast =
          file_data
           |> lexer_stage
           |> parser_stage
        //let _ = ast |> interpret_ast_stage
        //let _ =
        //  ast
        //   |> codegen_stage
        //   |> interpret_pasm_stage
        ()
   |> run
   |> ignore