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

Target.create "CleanTransformers" (fun _ ->
  Shell.rm_rf build_dir
  Shell.rm_rf "./bin/"
 )
 
Target.create "Transformers" (fun _ ->
  !!"Transformers.fsproj"
   |> MSBuild.runDebug id build_dir "Build"
   |> Trace.logItems "Target 'Source' output: "
 )

open Fake.Core.TargetOperators
"CleanTransformers" ==> "Transformers"

Target.runOrDefault "Transformers"