namespace Project

module Util =
  type CommandLineArguments = Map<string, string>
  let definedOperators = [
    "+"; "-"; "*"; "/"; "%"
    "<"; "<="; "="; "<>"; ">"; ">="
    "&&"; "||"
   ]
  let definedPrefixOperators = ["~&"; "~*"; "~-"; "~!"]
  [<Literal>]
  let PRINT = "printf"
  [<Literal>]
  let SCAN = "scan"

module Input =
  open System.IO
  module Implementation =

    let fromTestFile fileName: string*Util.CommandLineArguments =
      let txt = File.ReadAllLines fileName
      match txt.[0] with
      |("C" | "F#" as inferredLanguage) ->
        String.concat "\n" txt.[1..], Map ["language", inferredLanguage]
      |_ -> String.concat "\n" txt, Map ["language", "F#"]

    // format: Excel_VM (output file can appear anywhere) -(param name) (argument to param)
    let fromCommandLine (argv:string[]): string*Util.CommandLineArguments =
      printfn "argv: %A" argv
      // parameterizedArgs: map of parameter -> argument pairs
      // fileName: the first non-parameter non-argument string
      let parameterizedArgs, fileName =
        let (|ParamName|_|) (s:string) =
          if s.Length >= 2 && s.[0] = '-' then Some s.[1..] else None
        let rec searchOrderedParamList = function
          |ParamName s::((ParamName _::_ | []) as tl) ->
            let foundArgs, foundFileName = searchOrderedParamList tl
            Map.add s "" foundArgs, foundFileName
          |ParamName s::arg::tl ->
            let foundArgs, foundFileName = searchOrderedParamList tl
            Map.add s arg foundArgs, foundFileName
          |possibleFileName::tl ->
            let foundArgs, foundFileName = searchOrderedParamList tl
            foundArgs, lazy possibleFileName
          |[] -> Map.empty, lazy failwith "error: no input file given"
        searchOrderedParamList (List.ofArray argv)
      // check for no file given error
      let fileName = fileName.Force()
      // add file name to args
      let parameterizedArgs = parameterizedArgs.Add("input_file", fileName)
      printfn "args: %A" parameterizedArgs
      let txt = String.concat "\n" (File.ReadAllLines fileName)
      if parameterizedArgs.ContainsKey "language" then txt, parameterizedArgs
      else
        let fileExtension = fileName.[fileName.LastIndexOf '.' + 1..]
        match fileExtension with
        |"fs" | "fsx" -> txt, parameterizedArgs.Add("language", "F#")
        |_ -> txt, parameterizedArgs.Add("language", "C")