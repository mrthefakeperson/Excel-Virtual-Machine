#if FAKE
printfn "running script within FAKE"
#else

open System
open System.IO
open System.Diagnostics
open System.Threading
open System.Text.RegularExpressions

let build_file = Path.Combine(__SOURCE_DIRECTORY__, "build.fsx")

type config =
  { delay_ms : int
  ; log_dir : string option
  ; max_logs : int option
  ; rebuild_on_file_update : bool
  ; status_regexes : Regex list
  }

let read_config (file : string) : config =
  let lines =
    try File.ReadAllLines file
    with
    | :? DirectoryNotFoundException | :? FileNotFoundException ->
      failwith "config file not found"
  let config =
    Array.choose (fun (line : string) ->
      match line.Split([|'='|], 2) with
      | [| key; value |] -> Some (key.Trim(), value.Trim())
      | _ -> None
     ) lines
  let status_regexes =
    Array.toList config
     |> List.choose (function ("status_regex", regex) -> Some (Regex(regex)) | _ -> None)
  let config = Map.ofArray config
  { delay_ms = try int config.["delay_ms"] with _ -> 3000
  ; log_dir = try Some (Path.Combine(__SOURCE_DIRECTORY__, config.["log_dir"])) with _ -> None
  ; max_logs = try Some (int config.["max_logs"]) with :? ArgumentException -> None
  ; rebuild_on_file_update = try config.["rebuild_on_file_update"] = "true" with _ -> true
  ; status_regexes = status_regexes
  }

let persistent_run (dir : string) (config : config) (target : string) : unit =
  let process_name = "fake"
  let process_args = String.concat " " ["run"; build_file; "-t"; target]
  let start_process datetime =
    let process' = new Process()
    process'.StartInfo <- ProcessStartInfo(process_name, process_args)
    process'.StartInfo.UseShellExecute <- false
    process'.StartInfo.RedirectStandardOutput <- true
    process'.StartInfo.RedirectStandardError <- true
    let _delete_old_logs : unit =
      match config.log_dir, config.max_logs with
      | Some log_dir, Some max_logs ->
        let existing_logs = Directory.GetFiles(log_dir, "build-*.txt")
        let logs_to_remove = Array.length existing_logs - max_logs + 1
        if logs_to_remove > 0 then
          Seq.sortBy File.GetLastWriteTime existing_logs
           |> Seq.take logs_to_remove
           |> Seq.iter File.Delete
      | _ -> ()
    let log_file =
      Option.map (fun log_dir ->
        let file_name = (sprintf "build-%O.txt" datetime).Replace(" ", "_").Replace(":", ".")
        let log_file = Path.Combine(log_dir, file_name)
        printfn "%A" log_file
        File.AppendText log_file
       ) config.log_dir
    let write_to_log (message : string) =
      Option.iter (fun (log_file : StreamWriter) -> log_file.WriteLine message) log_file
    let matches_status message =
      List.exists (fun (regex : Regex) -> regex.Match(message).Success) config.status_regexes
    process'.OutputDataReceived.Add (fun args ->
      if args.Data <> null then
        write_to_log args.Data
        if matches_status args.Data then printfn "%s" args.Data
     )
    process'.ErrorDataReceived.Add (fun args ->
      if args.Data <> null then
        write_to_log args.Data
        printfn "error: %s" args.Data
     )
    ignore (process'.Start())
    (process', log_file)

  let rec get_last_modified dir =
    // look for *.fsproject files and directories containing *.fsbuild files
    let files = Directory.GetFiles dir
    [ for file in files do
        if Path.GetExtension file = ".fsproject" then
          yield File.GetLastWriteTime file
        elif Path.GetExtension file = ".fsbuild" then
          yield Directory.GetLastWriteTime dir
          yield!
            Seq.filter (fun file -> Path.GetExtension file <> ".fsproj") files
             |> Seq.map File.GetLastWriteTime
      for subdir in Directory.GetDirectories dir do
        yield! get_last_modified subdir
    ]
  
  while true do
    let last_modified = get_last_modified dir
    let process', log_file = start_process DateTime.Now
    Console.Clear()
    printfn "Starting build..."
    process'.BeginOutputReadLine()
    process'.BeginErrorReadLine()
    process'.WaitForExit()
    process'.Dispose()
    Option.iter (fun (log_file : StreamWriter) ->
      log_file.Flush()
      log_file.Close()
      log_file.Dispose()
     ) log_file
    if config.rebuild_on_file_update then
      while last_modified = get_last_modified dir do
        Thread.Sleep config.delay_ms
    Thread.Sleep config.delay_ms

let main (argv : string []) : unit =
  match argv with
  | [| "-t"; target; "-c"; config_file |]
  | [| "-c"; config_file; "-t"; target |] ->
    let config = read_config config_file
    persistent_run Environment.CurrentDirectory config target
  | _ -> failwith "usage: fsi main.fsx -t [target] -c [config_file]"

main (Array.tail fsi.CommandLineArgs)

#endif

