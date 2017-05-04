namespace Project

module Util =
  type CommandLineArguments = Map<string, string>
  let splitFileExtension (s:string) =
    match s.LastIndexOf '.' with
    | -1 -> s, ""
    |i -> s.[..i - 1], s.[i + 1..]

  let definedOperators = [
    "+"; "-"; "*"; "/"; "%"
    "<"; "<="; "="; "<>"; ">"; ">="; "!="
    "&&"; "||"
   ]
  let definedPrefixOperators = ["~&"; "~*"; "~-"; "~!"]
  [<Literal>]
  let PRINT = "printf"
  [<Literal>]
  let SCAN = "scan"