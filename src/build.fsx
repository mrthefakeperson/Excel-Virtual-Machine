#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.Core.Target //
nuget Fuchu"
#load "./.fake/build.fsx/intellisense.fsx"
#load "./.env.fsx"

open System.IO
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.Core.TargetOperators

let build_dir = "./build/"

let create_target name =
  if not (Directory.Exists name) then
    failwithf "Target %s not found" name
  let clean = "Clean" + name
  Target.create clean (fun _ ->
    Shell.rm_rf build_dir
    Shell.rm_rf "./bin/"
   )
  Target.create name (fun _ ->
    !!Path.Combine(name, name + ".fsproj")
     |> MSBuild.runDebug id build_dir "Build"
     |> Trace.logItems "Output:"
   )
  clean ==> name

let create_test path name =
  let test_script = match path with Some p -> p | None -> Path.combine name "Test.fsx"
  if File.exists test_script then
    let test = "Test" + name
    Target.create test (fun _ ->
      let references = List.map ((+) "--reference:") Env.packages
      let test_process = Shell.Exec("dotnet", String.concat " " ([Env.fsi; "--define:TEST"] @ references @ [test_script]))
      match test_process with
      |0 -> ()
      |_ -> failwith "tests failed"
     )
    if Option.isNone path then Some (name ==> test) else None
  else
    None

let targets = [
  "Utils"
  "CompilerDatatypes"
  "Parser"
  "Transformers"
  "Sim"
  "Codegen"
  "Codewriters"
  "Main"
 ]

let tests = List.choose (create_target >> create_test None) targets
[for (a, b) in List.pairwise targets -> a ==> b]
Target.create "All" (fun _ ->
  printfn "Build everything"
 )
[for a in targets -> a ==> "All"]
create_test (Some "./TestSamples.fsx") "Samples"
Target.create "TestAll" (fun _ -> printfn "Test everything")
"All" ==> "TestAll"
[for a in tests -> a ==> "TestAll"]

// fake build -t `TargetName`
Target.runOrList()