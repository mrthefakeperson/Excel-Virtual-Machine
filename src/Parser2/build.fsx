#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fake.Core.Target //"
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
  !! "Parser.fsproj"
   |> MSBuild.runDebug id build_dir "Build"
   |> Trace.logItems "Target 'Source' output: "
 )

open Fake.Core.TargetOperators
"CleanParser" ==> "Parser"

Target.runOrDefault "Parser"