module NUnitLite.Tests
open NUnitLite
open NUnit.Framework
open Parser

open Lexer
let tokens = List.ofSeq >> List.map string

[<Test>]
let LexerTest1() =
  Assert.AreEqual(["sd"; " "; " "; " "; "dfdfdf4"; " "; " "; "_4_e"; " "; "4_4"],
    tokenize createVariablesAndNumbers (tokens "sd   dfdfdf4  _4_e 4_4"))
[<Test>]
let LexerTest2() =
  Assert.AreEqual(["aadf"; " "; "4.33f"; " "; "df8"; " "; "_45"; " "; "__r4"],
    tokenize createVariablesAndNumbers (tokens "aadf 4.33f df8 _45 __r4"))
[<Test>]
let LexerStringTest1() =
  Assert.AreEqual([" "; "\"string with \\\"quote\\\"\""; " "],
    tokenize (createStrings @ createVariablesAndNumbers)
     (tokens " \"string with \\\"quote\\\"\" "))
[<Test>]
let LexerSymbolTest1() =
  Assert.AreEqual(["sdf"; "("; " "; "df"; "->"; "df"; " "; "d"; "->"; " "; " "; "->"; "fdfd"; " "; "->"; "("; "d"; ")"; " "; "*"],
    tokenize (createSymbol "->" @ createVariablesAndNumbers)
     (tokens "sdf( df->df d->  ->fdfd ->(d) *"))
[<Test>]
let LexerStringWithSymbolTest() =
  Assert.AreEqual(["ddf"; " "; "\" fdffdssf-> df334.d 4 \""; "\"\""; " "; "d"; " "; "\""],
    tokenize (createStrings @ createSymbol "->" @ createVariablesAndNumbers)
     (tokens "ddf \" fdffdssf-> df334.d 4 \"\"\" d \""))
[<Test>]
let LexerMultilineCommentWithSymbolTest() =
  Assert.AreEqual(["outside"; " "; "comment"; " "; "(* 7inside -> comment\"\"*)"; " "; "outside"; " "; "comment"],
    tokenize (createDelimitedComment "(*" "*)" @ createSymbol "->" @ createVariablesAndNumbers)
     (tokens "outside comment (* 7inside -> comment\"\"*) outside comment"))
[<Test>]
let LexerFakeDelimitedCommentTest() =
  Assert.AreEqual(["(*) inside comment *)"],
    tokenize (createDelimitedComment "(*" "*)" @ createVariablesAndNumbers)
     (tokens "(*) inside comment *)"))
[<Test>]
let LexerSingleLineCommentTest() =
  Assert.AreEqual(["//fddf\n"; "fdfd"; "//\n"],
    tokenize (createSingleLineComment "//" @ createVariablesAndNumbers)
     (tokens "//fddf\nfdfd//\n"))
[<Test>]
let LexerOverlappingStringAndCommentTest1() =
  Assert.AreEqual(["\"string(*\""; "not"; " "; "comment"; "*)"],
    tokenize (createDelimitedComment "(*" "*)" @ createStrings @ createVariablesAndNumbers)
     (tokens "\"string(*\"not comment*)"))
[<Test>]
let LexerOverlappingStringAndCommentTest2() =
  Assert.AreEqual(["(*comment\"*)"; "not"; " "; "string"; "\""],
    tokenize (createDelimitedComment "(*" "*)" @ createStrings @ createVariablesAndNumbers)
     (tokens "(*comment\"*)not string\""))

open Token
open FSharp
let parseString (str:string) =
  parse Normal ((=) []) (fun _ -> false) []
   (List.map (fun e -> Token e) (List.ofArray (str.Split '^')))
let (|A|_|) = function "apply" -> Some A | _ -> None

[<Test>]
let FsParserTupleTest() =
  match parseString "a^,^b^,^c" with
  |X(",", [T "a"; T "b"; T "c"]), [] -> ()
  |a, _ -> failwithf "failed tuple test: %A" a
[<Test>]
let FsParserInfixTest() =
  match parseString "0^+^1^+^2^*^(^3^+^4^)" with
  |X(A,[X(A,[T"+";X(A,[X(A,[T"+";T"0"]);T"1"])]);X(A,[X(A,[T"*";T"2"]);X("()",[X(A,[X(A,[T"+";T"3"]);T"4"])])])]), [] -> ()
  |a, _ -> failwithf "failed infix test: %A" a
[<Test>]
let FsParserPrefixTest() =
  match parseString "!^min^!^a^b" with
  |X(A, [T "~!"; X(A, [X(A, [T "min"; X(A, [T "~!"; T "a"])]); T "b"])]), [] -> ()
  |a, _ -> failwithf "failed prefix test: %A" a

[<EntryPoint>]
let main argv =
  try AutoRun().Execute argv
  finally System.Console.ReadLine() |> ignore