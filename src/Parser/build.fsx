#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.Core.Target //
nuget Fuchu"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet

let build_dir = "./build/"

Target.create "CleanParser" (fun _ ->
  Shell.rm_rf build_dir
  Shell.rm_rf "./bin/"
 )
 
Target.create "Parser" (fun _ ->
  !!"Parser.fsproj"
   |> MSBuild.runDebug id build_dir "Build"
   |> Trace.logItems "Target 'Source' output: "
 )

Target.create "TestParser" (fun _ ->
  match Shell.Exec("fsi", "Test.fsx") with
  |0 -> ()
  |_ -> failwith "tests failed"
 )

open Fake.Core.TargetOperators
"CleanParser" ==> "Parser" ==> "TestParser"

Target.runOrDefault "TestParser"