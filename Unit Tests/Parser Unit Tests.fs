module Parser.Parser_Unit_Tests
open NUnitLite
open NUnit.Framework
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