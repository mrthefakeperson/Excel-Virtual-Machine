#if !TEST
#load "../.fake/build.fsx/intellisense.fsx"
#endif
#r "./build/Utils.dll"
#r "./build/CompilerDatatypes.dll"
#r "./build/Parser.dll"
#r "./build/Transformers.dll"
#r "./build/Sim.dll"
#r "./build/Codegen.dll"
#r "./build/Codewriters.dll"
#load "./Utils/TestUtils/E2e.fsx"
#load "./.env.fsx"
open Utils
open Utils.TestUtils

module TestParser =
  open CompilerDatatypes.AST
  open Parser
  
  let parser_data = E2e.Samples.sample_data |> Seq.take 33

  E2e.add_test_stage "Lexer" (Lex.tokenize_text >> Preproc.preprocessor_pass) parser_data
  E2e.add_test_stage "Parser" (Main.parse_tokens_to_ast_with Main.parse_global_scope) parser_data
  E2e.add_test_stage "Pprint" (pprint_ast_structure: SyntaxAST.AST -> string) parser_data

  open E2e.Operators
  "Lexer" ==> "Parser" ==> "Pprint"

module TestAST =
  open CompilerDatatypes
  open CompilerDatatypes.AST
  open CompilerDatatypes.Semantics.InterpreterValue

  let data = E2e.Samples.sample_data |> Seq.take 18

  E2e.add_test_stage "Transform" Transformers.Main.transform data
  E2e.add_test_stage "TransformPprint" (Transformers.Main.transform >> pprint_ast_structure) data

  let interpret: SemanticAST.AST -> Boxed * string = Sim.Main.interpret_semantics_ast
  E2e.add_test_stage "Interpret" interpret data

  E2e.add_test_stage "Codegen" Codegen.Main.generate_pasm data
  E2e.add_test_stage "SimulatePAsm" Sim.Main.simulate_pasm
   (Seq.append (Seq.take 6 data) (Seq.skip 7 data))

  open E2e.Operators
  "Parser" ==> "Transform" ==> "Interpret"
  "Parser" ==> "TransformPprint"
  "Transform" ==> "Codegen" ==> "SimulatePAsm"

module TestFullCompile =
  open CompilerDatatypes
  open Parser

  let compile_to_pasm =
    Lex.tokenize_text
     >> Preproc.preprocessor_pass
     >> Main.parse_tokens_to_ast_with Main.parse_global_scope
     >> Transformers.Main.transform
     >> Codegen.Main.generate_pasm

  E2e.add_test_stage "FullCompile" compile_to_pasm TestAST.data
  E2e.add_test_stage "CompileToNasm" Codewriters.Main.write_nasm TestAST.data

  open E2e.Operators
  "FullCompile" ==> "CompileToNasm"

E2e.run_e2e()