#r "paket:
open Fuchu
open Fake.IO
open Fake.Core
open Fake.Core
nuget Fake.IO.FileSystem
nuget Fake.DotNet.MSBuild
nuget Fuchu
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet

let buildDir = "./build/"
let testDir = "./build/test/"

// Targets
Target.create "Clean" (fun _ ->
  Shell.cleanDir buildDir
 )

Target.create "BuildApp" (fun _ ->
  !!"src/*.fsproj"
   |> MSBuild.runRelease id buildDir "Build"
   |> Trace.logItems "AppBuild-Output: "
 )

Target.create "BuildTest" (fun _ ->
  !!"test/*.fsproj"
   |> MSBuild.runDebug id testDir "Build"
   |> Trace.logItems "TestBuild-Output: "
 )

Target.create "Test" (fun _ ->
  !!"build/test/test.exe"
   |> Seq.map (fun fname ->
        Process.asyncShellExec {ExecParams.Empty with Program = fname}
       )
   |> Async.Parallel
   |> Async.RunSynchronously
   |> Seq.sum
   |> function 0 -> () | _ -> failwith "tests failed"
 )

Target.create "Default" (fun _ ->
  Trace.trace "Hello World from FAKE"
 )

open Fake.Core.TargetOperators
"Clean"
 ==> "BuildApp"
 ==> "BuildTest"
 ==> "Test"
 ==> "Default"

// start build
Target.runOrDefault "Default"