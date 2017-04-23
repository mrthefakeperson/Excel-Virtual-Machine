module private Parser.Lexer
open System

type stringClassifier = string -> bool
module CommonClassifiers =
  // util
  let getOccurances pred str =
    String.collect (fun e -> if pred e then string e else "") str
     |> String.length
  let (>>||) classifierA classifierB e = classifierA e || classifierB e
  let (>>&&) classifierA classifierB e = classifierA e && classifierB e
  let inline negate f = f >> not
  let isPrefix (a:string) (b:string) =  // a is a prefix of b?
    a.Length <= b.Length && a = b.[..a.Length - 1]
  let isSuffix (a:string) (b:string) =  // a is a suffix of b?
    a.Length <= b.Length && a = b.[b.Length - a.Length..]
  // does str start with delim1, end with delim2, and not have the delimiters overlap characters?
  let isDelimitedString (delim1:string) (delim2:string) (str:string) =
    delim1.Length + delim2.Length <= str.Length && isPrefix delim1 str && isSuffix delim2 str
  let entireString = String.forall
  let firstCharOfString isClassified (str:string) = isClassified str.[0]
  let firstSuffixOfString isClassified (str:string) = entireString isClassified str.[1..]
  let firstCharIsXAndSuffixIsY isClassified pred str =
    firstCharOfString isClassified str && pred str.[1..]
  // common classifiers (char)
  let isNumeral = Char.IsDigit     // 0 to 9
  let isAlphabetic = Char.IsLetter // a to z, A to Z, accented and Greek characters
  let isUndersc = (=) '_'
  let isDot = (=) '.'
  let isHyphen = (=) '-'
  let isValidForVariables = isNumeral >>|| isAlphabetic >>|| isUndersc
  let isWhitespaceChar = (=) ' ' >>|| (=) '\n' >>|| (=) '\t' >>|| (=) '\r'
  let isSingleQuote = (=) '''
  let isDoubleQuote = (=) '"'
  // common classifiers (string)
  let isVariableSuffix = entireString isValidForVariables
  let isInteger =   // first character is digit, rest are valid for variables
    firstCharIsXAndSuffixIsY isNumeral isVariableSuffix
  let isVariable =  // all characters are valid for variables, first is not digit
    firstCharIsXAndSuffixIsY (isValidForVariables >>&& negate isNumeral) isVariableSuffix
  let isNumericSuffix = entireString (isDot >>|| isValidForVariables)
  let isNumeric =   // first character is digit, rest are valid for variables or are dot
    firstCharIsXAndSuffixIsY isNumeral isNumericSuffix
  let isFloat = isNumeric >>&& (negate isInteger)  // is numeric, but contains a dot
  // note that isFloatSuffix === isNumericSuffix
  let isWhitespace = entireString isWhitespaceChar
  let isAnything = entireString (fun _ -> true)
  let failedToClassify message _ = failwith message
// rules which use classifiers
open CommonClassifiers
type tokenizeRule =
  |StickRule of (string -> string -> bool)
  |PriorityStickRule of (string -> string -> bool)    // gets checked first
  |SymbolRule of string
let makeRule (c1:stringClassifier) (c2:stringClassifier) =
  StickRule (fun s1 s2 -> c1 s1 && c2 s2)
let makeSymbolRule (symbol:string) = SymbolRule symbol
let makeDelimiterRule (symbol1:string) (symbol2:string) =
  PriorityStickRule (fun s1 s2 ->
    isPrefix symbol1 s1 && not (isDelimitedString symbol1 symbol2 s1)
   )
let tokenize (ruleset:(tokenizeRule*bool) list) (txt:string list) =
  let ruleset =  // put a sentry at the end to catch match failures
    ruleset @ [makeRule (failedToClassify "could not classify") isAnything, false]
  let txtWithSymbols, stickRules, priorityStickRules =
    // go through the rule set
    List.fold (fun (accTxt, accRules, accPriorityRules) -> function
      // stick-rules get filtered into two new lists
      |StickRule r, b -> accTxt, (r, b)::accRules, accPriorityRules
      |PriorityStickRule r, b -> accTxt, accRules, (r, b)::accPriorityRules
      // symbol rules are dealt with immediately: scan for each symbol
      |SymbolRule s, _ ->
        let rec tryGetPrefix = function   // try to match a with some prefix of b and split b
          |[], ll -> Some ([], ll)
          |_, [] -> None
          |a::_, b::_ when a <> b -> None
          |a::tl1, b::tl2 ->
            match tryGetPrefix (tl1, tl2) with
            |None -> None
            |Some (p, s) -> Some (b::p, s)
        let ss = List.ofSeq s |> List.map string
        let rec groupSymbols ll =     // symbols will not overwrite previous symbols, even if they match due to the way the matching list is constructed (one char per index, meaning prefixes with previously formed symbols will never be matched)
          match tryGetPrefix (ss, ll), ll with
          |Some (p, s), _ -> String.concat "" p::groupSymbols s
          |None, [] -> []
          |None, hd::tl -> hd::groupSymbols tl
        (groupSymbols accTxt, accRules, accPriorityRules)
     ) (txt, [], []) ruleset
  // priority rules go first
  let stickRules = List.rev priorityStickRules @ List.rev stickRules
  // make one scanning pass of the whole list of tokens, considering all stick-rules
  List.fold (fun acc nextToken ->
    match acc with
    |[] -> [nextToken]
    |lastToken::tl ->
      // one by one, apply each stick-rule to find the first one that matches
      let _, shouldStick = Seq.find (fun (stick, _) -> stick lastToken nextToken) stickRules
      // the bool for each stick-rule determines if a matching pair should always or never be concatenated
      if shouldStick
       then lastToken + nextToken::tl
       else nextToken::acc
   ) [] txtWithSymbols
   |> List.rev
module SpecialCases =
  let detectEscapeSequenceDoubleQuote =
    StickRule (fun s1 (s2:string) -> isPrefix "\"" s1 && isSuffix "\\\"" s1)
let createSingleLineComment delimiter = [
  makeSymbolRule delimiter, true
  makeDelimiterRule delimiter "\n", true
  makeRule (isDelimitedString delimiter "\n") isAnything, false
 ]
let createDelimitedComment delim1 delim2 = [
  makeSymbolRule delim1, true
  makeSymbolRule delim2, true
  makeDelimiterRule delim1 delim2, true
  makeRule (isDelimitedString delim1 delim2) isAnything, false
 ]
let createSymbol symbol = [
  makeSymbolRule symbol, true
  makeRule ((=) symbol) isAnything, false
  makeRule (fun e -> not (isPrefix e symbol)) (fun e -> isPrefix e symbol), false
 ]
// don't need to check for \" due to the context in which this is used
let createStrings = [
  SpecialCases.detectEscapeSequenceDoubleQuote, true
  makeDelimiterRule "\"" "\"", true
  makeRule (isDelimitedString "\"" "\"") isAnything, false
 ]
let createVariablesAndNumbers = [
  makeRule isNumeric isNumericSuffix, true
  makeRule isVariable isVariableSuffix, true
  makeRule isAnything isAnything, false
 ]
