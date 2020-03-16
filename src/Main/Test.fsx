#if !TEST
#load "../.fake/build.fsx/intellisense.fsx"
#endif
#r "../build/Main.dll"

open Main.Main
open System.IO

let get_sample_path name =
  let dir = Directory.GetParent __SOURCE_FILE__ |> string
  Path.Combine(dir, "Main", "Samples", name)

let argv = sprintf "%s -p -o %s" (get_sample_path "input.txt") (get_sample_path "output.txt")
main (argv.Split())