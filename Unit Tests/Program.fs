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
   (List.mapi (fun i e -> Token(e, (0, i))) (List.ofArray (str.Split ' ')))
let (|A|_|) = function "apply" -> Some A | _ -> None

[<Test>]
let FsParserTupleTest() =
  match parseString "a , b , c" with
  |X(",", [T "a"; T "b"; T "c"]), [] -> ()
  |a, _ -> failwithf "failed tuple test: %A" a
[<Test>]
let FsParserInfixTest() =
  match parseString "0 + 1 + 2 * ( 3 + 4 )" with
  |X(A,[X(A,[T"+";X(A,[X(A,[T"+";T"0"]);T"1"])]);X(A,[X(A,[T"*";T"2"]);X("()",[X(A,[X(A,[T"+";T"3"]);T"4"])])])]), [] -> ()
  |a, _ -> failwithf "failed infix test: %A" a
[<Test>][<Ignore "">]
let FsParserUnaryTest() =
  match parseString "! ! ( a + & b )" with
  |X(A, [T "~!"; X(A, [T "~!"; X("()", [X(A, [X(A, [T "+"; T "a"]); X(A, [T "~&"; T "b"])])])])]), [] -> ()
  |a, _ -> failwithf "failed unary test: %A" a
[<Test>][<Ignore "">]    // annoying
let FsParserUnaryNegationTest() =
  match parseString "- min - a b" with
  |X(A, [T "~-"; X(A, [X(A, [T "min"; X(A, [T "~-"; T "a"])]); T "b"])]), [] -> ()
  |a, _ -> failwithf "failed unary negation test: %A" a
[<Test>]
let FsParserBracketTest() =
  match parseString "( ( ( ) ) )" with
  |X("()", [X("()", [X("()", [T "()"])])]), [] -> ()
  |a, _ -> failwithf "failed bracket test: %A" a
[<Test>]
let FsParserIfTest() =
  match parseString "if a then b elif c then ( if a2 then b2 ) else d" with
  |X("if", [T "a"; T "b"; X("if", [T "c"; X("()", [X("if", [T "a2"; T "b2"; T "()"])]); T "d"])]), [] -> ()
  |a, _ -> failwithf "failed if test: %A" a
[<Test>][<Ignore "">]
let FsParserLetTest() =
  match parseString "let a x = let b = 1 in b" with
  |X("let", [X(A, [T "a"; T "x"]); X("sequence", [X("let", [T "b"; T "1"]); T "b"])]), [] -> ()
  |a, _ -> failwithf "failed let test: %A" a
[<Test>]
let FsParserFunTest() =
  match parseString "fun a b -> fun c -> ()" with
  |X("fun", [X(A, [T "a"; T "b"]); X("fun", [T "c"; T "()"])]), [] -> ()
  |a, _ -> failwithf "failed fun test: %A" a

open C
let parseStringC state (str:string) =
  parse state ((=) []) (fun _ -> false) []
   (List.mapi (fun i e -> Token(e, (0, i))) (List.ofArray (str.Split ' ')))
   |> fun (e, rest) -> e.Clean(), rest

[<Test>]
let CParserBracesTest() =
  match parseStringC Local "{ a + b }" with
  |X("{}", [X(A, [X(A, [T "+"; T "a"]); T "b"])]), [] -> ()
  |a, _ -> failwithf "failed C-braces test: %A" a
[<Test>]
let CParserIfWithoutBracesTest() =
  match parseStringC Local "if ( a ) return b ;" with
  |X("if", [T "a"; X("return", [T "b"]); T "()"]), [] -> ()
  |a, _ -> failwithf "failed C-if without braces test: %A" a
[<Test>]
let CParserIfNestedBracesTest() =
  match parseStringC Local "if ( a ) { b ; { c ; } d ; }" with
  |X("if", [T "a"; X("sequence", [T "b"; T ";"; X("{}", [X("sequence", [T "c"; T ";"])]); T "d"; T ";"]); T "()"]), [] -> ()
  |a, _ -> failwithf "failed C-if with nested braces test: %A" a
[<Test>]
let CParserIfNestedParensTest() =
  match parseStringC Local "if ( ( a ) ) b ;" with
  |X("if", [T "a"; T "b"; T "()"]), [] -> ()
  |a, x -> failwithf "failed C-if with nested parentheses test: %A" a
[<Test>]
let CParserIfElseTest() =
  match parseStringC Local "if ( a ) b ; else c ;" with
  |X("if", [T "a"; T "b"; T "c"]), [] -> ()
  |a, x -> failwithf "failed C-if with else test: %A" a
[<Test>]
let CParserInnerBracesTest() =
  match parseStringC Local "while ( 1 ) if ( 0 ) { x ; }" with
  |X("while", [T "1"; X("if", [T "0"; X("sequence", [T "x"; T ";"]); T "()"])]), [] -> ()
  |a, _ -> failwithf "failed C-outer braceless, inner braces test: %A" a
[<Test>]
let CParserReturnVoidTest() =
  match parseStringC Local "return ;" with
  |X("sequence", [X("return", [T "()"]); T ";"]), [] -> ()
  |a, _ -> failwithf "failed C-return void test: %A" a
[<Test>]
let CParserScopeThenBracketTest() =
  match parseStringC Local "{ a ; } ( b ) ;" with
  |X("sequence", [X("{}", [X("sequence", [T "a"; T ";"])]); T "b"; T ";"]), [] -> ()
  |a, _ -> failwithf "failed braces then brackets test: %A" a
[<Test>]
let CParserEmptyStatementTest() =
  match parseStringC Local "if ( a ) ;" with
  |X("if", [T "a"; T "()"; T "()"]), [] -> ()
  |a, _ -> failwithf "failed empty statement test: %A" a

[<EntryPoint>]
let main argv =
  try AutoRun().Execute argv
  finally System.Console.ReadLine() |> ignore