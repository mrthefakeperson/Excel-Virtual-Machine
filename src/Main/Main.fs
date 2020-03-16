module Main.Main
open Utils
module Parser = Parser.Main
module Transformers = Transformers.Main
module Sim = Sim.Main
module Codegen = Codegen.Main
module Codewriters = Codewriters.Main
open System.IO

[<EntryPoint>]
let main argv =
  let flags = CLI.get_cli_flags argv
  let input_file =
    if flags.ContainsKey "-unnamed" then
      List.tryExactlyOne flags.["-unnamed"]
       |> Option.defaultWith (fun () -> failwith "exactly one input file required")
    else failwith "input file required"
  let output_file =
    if flags.ContainsKey "-o" then
      List.tryExactlyOne flags.["-o"]
       |> Option.defaultWith (fun () -> failwith "exactly one output file required")
    else "out"
  let unknown_args = [
    for key in flags.Keys do
      if key <> "-unnamed" && key <> "-o" then
        yield! flags.[key]
   ]
  if not (List.isEmpty unknown_args) then
    failwithf "unknown args: %s" (String.concat ", " unknown_args)
  
  let input = File.ReadAllText input_file
  let syntax_ast = lazy Parser.parse_string_to_ast input
  let sem_ast = lazy Transformers.transform (syntax_ast.Force())
  let sim_result = lazy Sim.interpret_semantics_ast (sem_ast.Force())
  let pasm = lazy Codegen.generate_pasm (sem_ast.Force())
  let nasm = lazy Codewriters.Nasm.write_nasm (pasm.Force())
//   let excel = lazy Codewriters.Xlsx
  
  let result =
    match () with
    |_ when flags.ContainsKey "-p" -> syntax_ast.Force().ToString()
    |_ when flags.ContainsKey "-i" -> sim_result.Force().ToString()
    |_ when flags.ContainsKey "-asm" -> pasm.Force().ToString()
    |Strict x -> x
  File.WriteAllText(output_file, result)
  0
