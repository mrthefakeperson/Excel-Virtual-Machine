module Utils.TestUtils.E2e
open System.IO
open System.Collections.Generic
open Fuchu

let approval_test fname obj =
  let serialized = sprintf "%A" obj
  let fpath = Path.Combine(__SOURCE_DIRECTORY__, "Approval", fname)
  let record =
    try File.ReadAllText fpath
    with :? FileNotFoundException | :? DirectoryNotFoundException ->
      ignore <| Directory.CreateDirectory (Path.GetDirectoryName fpath)
      File.Create(fpath).Close()
      ""
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

let context = {|
  tasks = Dictionary()
  cache = Dictionary()
  testdata = Dictionary()
|}

let add_test_stage name (task: 'i -> 'o) (testdata: string seq) =
  if context.tasks.ContainsKey name then failwithf "duplicate stage name %s" name
  context.tasks.[name] <-
    fun (input: obj) ->
      if not (context.cache.ContainsKey (name, input)) then
        context.cache.[(name, input)] <- input :?> 'i |> task :> obj
      context.cache.[(name, input)]
  context.testdata.[name] <- testdata

module Operators =
  let (==>) dep task =
    context.tasks.[task] <- context.tasks.[dep] >> context.tasks.[task]
    task

module Samples =
  let SAMPLES_DIR = Path.Combine(__SOURCE_DIRECTORY__, "Samples")
  let sample_data: string seq =
    Directory.GetFiles SAMPLES_DIR
     |> Array.sortBy (fun f -> int(Path.GetFileName(f).Replace(".c", "")))
     |> Seq.map File.ReadAllText

let run_stage name =
  printfn "Stage: %s" name
  let (task, data) = (context.tasks.[name], context.testdata.[name])
  Seq.iteri (fun i input ->
    let fname = sprintf "%s/%i.out" name (i + 1)
    let output = task input
    approval_test fname output
    printf "."
   ) data
  printfn ""

let run_e2e() =
  Seq.map
   (fun name -> testCase "End to end test" (fun _ -> run_stage name))
   context.tasks.Keys
   |> Seq.iter (run >> ignore)