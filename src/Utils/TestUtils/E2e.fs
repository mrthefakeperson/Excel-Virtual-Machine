module Utils.TestUtils.E2e
open System
open System.IO
open Fuchu

let approval_test fname obj =
  let serialized = sprintf "%A" obj
  let fpath = Path.Combine(__SOURCE_DIRECTORY__, "approval", fname)
  let record = try File.ReadAllText fpath with :? FileNotFoundException -> ""
  if serialized.Split('\n') <> record.Split('\n') then
    printf "Approval test failed on %s:\nExisting:\n%s\n-----\n\nNew:\n%s\n-----"
     fname record serialized
    let rec confirm() =
      printf "\nOverwrite? (y/n) "
      match System.Console.ReadLine() with
      |"y" -> true
      |"n" -> false
      |_ -> confirm()
    if confirm()
     then File.WriteAllText(fpath, serialized)
     else failwith "Approval test failed"

type 'i TestStage = {
  name: string
  task: 'i -> obj
  result_type: System.Type
  dependencies: obj TestStage list
 }

let test_stage name (stage: 'a TestStage) (data: 'a list) : 'b list =
  printfn "Stage: %s" name
  let results = List.map (fun e -> Convert.ChangeType(stage.task e, stage.result_type)) data
  List.iteri (fun i ->
    try approval_test (Path.Combine(name, sprintf "%i.out" (i + 1)))
    finally printf "."
   ) results
  printfn ""
  results

let SAMPLES_DIR = Path.Combine(__SOURCE_DIRECTORY__, "../../../samples")

let INCLUDED_SAMPLES = 27

let samples_data: string seq =
  Directory.GetFiles SAMPLES_DIR
   |> Array.sortBy (fun f -> int(Path.GetFileName(f).Replace(".c", "")))
   |> Seq.take INCLUDED_SAMPLES
   |> Seq.map File.ReadAllText

// let run_e2e (stage_graph: TestStage) =
//   let rec run_stages data ({dependencies = deps} as stg) _ =
//     let results = test_stage stg.name stg data
    
//   testCase "End to end test" <|

// let test() =
//   testCase "End to end test" <|
//       fun _ ->
//         let ast =
//           file_data
//            |> lexer_stage
//            |> parser_stage
//         //let _ = ast |> ast_process_stage
//         //let _ = ast |> interpret_ast_stage
//         //let _ =
//         //  ast
//         //   |> codegen_stage
//         //   |> interpret_pasm_stage
//         ()
//    |> run
//    |> ignore