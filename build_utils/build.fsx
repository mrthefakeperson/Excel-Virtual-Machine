#r "paket:
nuget FSharp.Core
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.Core.Target //
nuget Fuchu"
// #load "./.fake/build.fsx/intellisense.fsx"
#load "generate.fsx"

open System.IO
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.Core.TargetOperators

type path = string
type target_name = string

let copy_data_target_name = "CopyData"

let copy_data (build_dir : path) (data_dir : path) : unit =
  Target.create copy_data_target_name (fun _ ->
    Shell.mkdir build_dir
    Directory.GetFiles data_dir
     |> Array.iter (fun data_file -> Shell.cp data_file build_dir)
   )

let create_module
  (build_dir : path)
  (bin_dir : path)
  (fsproj_path : path)
  (target_name : target_name)
  (dependencies : target_name list)
  : unit
  =
  if not (File.Exists fsproj_path) then
    failwithf "fsbuild error: target %s not found (%s)" target_name fsproj_path
  let clean_target_name = "Clean" + target_name
  Target.create clean_target_name (fun _ ->
    Shell.rm_rf build_dir
    Shell.rm_rf bin_dir
   )
  Target.create target_name (fun _ ->
    !!fsproj_path
     |> MSBuild.runDebug id build_dir "Build"
     |> Trace.logItems "Output:"
   )
  ignore (clean_target_name ==> copy_data_target_name ==> target_name)
  List.map (fun dep_name -> dep_name ==> target_name) dependencies |> ignore

let create_script
  (main_script_path : path)
  (target_name : target_name)
  (packages : string list)
  (dependencies : target_name list)
  (fsi : string)
  : unit
  =
  if not (File.Exists main_script_path) then
    failwithf "fsbuild error: target %s not found (%s)" target_name main_script_path
  Target.create target_name (fun _ ->
    let references = List.map ((+) "--reference:") packages
    let fsi_args = String.concat " " ([fsi; "--define:FAKE"] @ references @ [main_script_path])
    let test_process = Shell.Exec("dotnet", fsi_args)
    if test_process <> 0 then failwith "script failed"
   )
  List.map (fun dep_name -> dep_name ==> target_name) dependencies |> ignore

let create_test
  (script_path : path)
  (original_target : target_name)
  packages
  fsi
  =
  let target_name = "Test" + original_target
  let dependencies = [original_target]
  create_script script_path target_name packages dependencies fsi

let create_aggregate (target_name : target_name) (dependencies : target_name list) =
  Target.create target_name (fun _ -> printfn "build multiple targets")
  List.map (fun dep_name -> dep_name ==> target_name) dependencies |> ignore

open Generate

let build_project_targets (root_path : path) : unit =
  let project = generate_project_targets root_path
  copy_data project.build_dir project.data_dir
  List.iter (fun target ->
    match target.target_path with
    | Script main ->
      create_script main target.name project.script_packages target.dependencies project.fsi_command
    | Module fsproj ->
      create_module project.build_dir project.bin_dir fsproj target.name target.dependencies
   ) project.targets
  create_aggregate "All" (List.map (fun target -> target.name) project.targets)

  let test_names =
    List.choose (fun target ->
      Option.map (function
        | Script main ->
          create_test main target.name project.script_packages project.fsi_command
          "Test" + target.name
        | Module _ -> failwith "compiled tests are not currently supported"
       ) target.test_target_path
     ) project.targets
  create_aggregate "TestAll" test_names

  Target.runOrList()

build_project_targets System.Environment.CurrentDirectory