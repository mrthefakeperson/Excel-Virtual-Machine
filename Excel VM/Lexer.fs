namespace Parser
open System

module Lexer =
  type stringClassifier = string -> bool
  module CommonClassifiers =
    // util
    let getOccurances charClassifier str =
      String.collect (fun e -> if charClassifier e then string e else "") str
       |> String.length
    let (>>||) classifierA classifierB e = classifierA e || classifierB e
    let (>>&&) classifierA classifierB e = classifierA e && classifierB e
    let inline negate f = f >> not
    let classifierApplies classifier str = String.forall classifier str
    let classifierAppliesFstChar classifier (str:string) = classifier str.[0]
    let classifierAppliesFstSuffix classifier (str:string) =
      String.forall classifier str.[1..]
    let classifiersApplyCons hdCharClassifier tlStrClassifier str =
      classifierAppliesFstChar hdCharClassifier str
       && tlStrClassifier str.[1..]
    // common classifiers (char)
    module Chr =
      let isNumeric = Char.IsDigit     // 0 to 9
      let isAlphabetic = Char.IsLetter // a to z, A to Z, accented and Greek characters
      let isUndersc = (=) '_'
      let isDot = (=) '.'
      let isHyphen = (=) '-'
      let isValidForVariables = isNumeric >>|| isAlphabetic >>|| isUndersc
      let isWhitespace = (=) ' ' >>|| (=) '\n' >>|| (=) '\t' >>|| (=) '\r'
      let isSingleQuote = (=) '''
      let isDoubleQuote = (=) '"'
    // common classifiers (string)
    let isVariableSuffix = classifierApplies Chr.isValidForVariables
    let isInteger =   // first character is digit, rest are valid for variables
      classifiersApplyCons Chr.isNumeric isVariableSuffix
    let isVariable =  // all characters are valid for variables, first is not digit
      classifiersApplyCons (Chr.isValidForVariables >>&& negate Chr.isNumeric) isVariableSuffix
    let isNumericSuffix = classifierApplies (Chr.isDot >>|| Chr.isValidForVariables)
    let isNumeric =   // first character is digit, rest are valid for variables or are dot
      classifiersApplyCons Chr.isNumeric isNumericSuffix
    let isFloat = isNumeric >>&& (negate isInteger)  // is numeric, but contains a dot
    // note that isFloatSuffix === isNumericSuffix
    let isWhitespace = classifierApplies Chr.isWhitespace
    let isAnything = classifierApplies (fun _ -> true)
    let failedToClassify message _ = failwith message
  // rules which use classifiers
  open CommonClassifiers
  type tokenizeRule =
    |StickRule of (string -> string -> bool)
    |SymbolRule of string
  let makeRule (c1:stringClassifier) (c2:stringClassifier) =
    StickRule (fun s1 s2 -> c1 s1 && c2 s2)
  let isPrefix (a:string) (b:string) =  // a is a prefix of b?
    a.Length <= b.Length && a = b.[..a.Length - 1]
  let isSuffix (a:string) (b:string) =  // a is a suffix of b?
    a.Length <= b.Length && a = b.[b.Length - a.Length..]
  // does str start with delim1, end with delim2, and not have the delimiters overlap characters?
  let isDelimitedString (delim1:string) (delim2:string) (str:string) =
    delim1.Length + delim2.Length <= str.Length && isPrefix delim1 str && isSuffix delim2 str
  let makeSymbolRule (symbol:string) = SymbolRule symbol
  let makeDelimiterRule (symbol1:string) (symbol2:string) =
    StickRule (fun s1 s2 ->
      isPrefix symbol1 s1 && not (isDelimitedString symbol1 symbol2 s1)
     )
  let tokenize (ruleset:(tokenizeRule*bool) list) (txt:string list) =
    let ruleset =  // put a sentry at the end automatically
      ruleset @ [makeRule (failedToClassify "could not classify") isAnything, false]
    let txtWithSymbols, stickRules =
      List.fold (fun (accTxt, accRules) -> function
        |SymbolRule s, _ ->
          let rec tryGetPrefix = function   // try to match a with a prefix of b and split b
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
          (groupSymbols accTxt, accRules)
        |StickRule r, b -> (accTxt, (r, b)::accRules)
       ) (txt, []) ruleset
    let stickRules = List.rev stickRules
    txtWithSymbols
     |> List.fold (fun acc next ->
          match acc with
          |[] -> [next]
          |hd::tl ->
            let _, shouldStick = Seq.find (fun (stick, _) -> stick hd next) stickRules
            if shouldStick
             then hd + next::tl
             else next::acc
         ) []
     |> List.rev
  module SpecialCases =
    let detectEscapeSequenceDoubleQuote =
      StickRule (fun s1 (s2:string) -> isPrefix "\"" s1 && isSuffix "\\\"" s1)
  let singleLineCommentRules delimiter = [
    makeSymbolRule delimiter, true
    makeDelimiterRule delimiter "\n", true
    makeRule (isDelimitedString delimiter "\n") isAnything, false
   ]
  let delimitedCommentRules delim1 delim2 = [
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
  let commonRules = [
    SpecialCases.detectEscapeSequenceDoubleQuote, true
    makeDelimiterRule "\"" "\"", true
    makeRule (isDelimitedString "\"" "\"") isAnything, false
    makeRule isAnything isWhitespace, false
    makeRule isNumeric isNumericSuffix, true
    makeRule isVariable isVariableSuffix, true
    makeRule isAnything isAnything, false
   ]

// "aadf 4.33f df8 _45 __r4" -> ["aadf"; " "; "4.33f"; " "; "df8"; " "; "_45"; " "; "__r4"]
// "sd   dfdfdf4  _4_e 4_4" -> ["sd"; " "; " "; " "; "dfdfdf4"; " "; " "; "_4_e"; " "; "4_4"]
// "sdf( df->df d->  ->fdfd ->(d) *" -> ["sdf"; "("; " "; "df"; "->"; "df"; " "; "d"; "->"; " "; " "; "->"; "fdfd"; " "; "->"; "("; "d"; ")"; " "; "*"]
// "ddf \" fdffdssf-> df334.d 4 \"\"\" d \"" -> ["ddf"; " "; "" fdffdssf-> df334.d 4 ""; """"; " "; "d"; " "; """]
// "outside comment (* 7inside -> comment\"\"*) outside comment" -> ["outside"; " "; "comment"; " "; "(* 7inside -> comment""*)"; " "; "outside"; " "; "comment"]
// " \"string with \\\"quote\\\"\" " -> [" "; ""string with \"quote\"""; " "]
// "(*)"
// "//fddf\nfdfd//\n"

module StringExpressions =
  let buildString builder = String (List.toArray (List.rev builder))
  let escapeSequences (s:string) =
    let charList = List.ofArray (s.ToCharArray())
    let rec parseCharList builder = function
      |[] -> buildString builder
      |'\\'::c::tl ->
        let k =
          match c with
          |'\\' -> '\\'
          |'"' -> '"'
          |_ -> failwith "escape sequence not recognized"
        parseCharList (k::builder) tl
      |c::tl -> parseCharList (c::builder) tl
    parseCharList [] charList

  let separateFormatSymbols (s:string) =
    let charList = List.ofArray (s.ToCharArray())
    // todo: more complex format rules, eg. "%5.4[1-9]"
    let rec parseCharList builder = function
      |[] -> List.rev (List.map buildString (List.filter ((<>) []) builder))
      |'%'::c::tl ->
        let formatStringBuilder = [c; '%']
        parseCharList ([]::formatStringBuilder::builder) tl
      |['%'] -> failwith "incomplete format"
      |c::tl -> parseCharList ((c::builder.Head)::builder.Tail) tl
    parseCharList [[]] charList
