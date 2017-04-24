module Parser.Implementation
open Project.Definitions
open Definition

let fromStringRunUntilParsed (txt:string, args:CommandLineArguments): Token =
  let languageParser =
    match args.["language"] with
    |"F#" -> FSharpParser.parseSyntax
    |"C" -> CParser.parseSyntax
    |_ -> failwith "error: defined language does not exist"
  (languageParser txt).Clean()

let fromString (txt:string, args:CommandLineArguments): Token*CommandLineArguments =
  fromStringRunUntilParsed (txt, args)
   |> TypeValidation.validateTypes
   |> fun e -> e.Clean(), args
