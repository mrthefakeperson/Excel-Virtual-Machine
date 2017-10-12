#r "lex.dll"
#load "framework.fs"
open Framework

let text = """
a: {
  b: ["cdefg", 1],
  y: 3
}
"""

let tokens = Lexer.tokenize text |> List.ofSeq

let _var = lazy Rule().matches("[a-z A-Z][a-z 0-9 A-Z _]*")
let _string = lazy Rule().matches("\"(\\\"|[^\"])*\"")
let _int = lazy Rule().matches("-?[0-9]+")
let rec value =
  lazy Rule().isOneOf(
    lazy Rule("object").isSequenceOf(lazy Rule().is "{", lazy optional (assignmentSeq.Force()), lazy Rule().is "}"),
    lazy Rule("list").isSequenceOf(lazy Rule().is "[", lazy optional (valueSeq.Force()), lazy Rule().is "]"),
    lazy Rule().isOneOf(_string, _int)
   )
and valueSeq: Lazy<rule> =
  lazy Rule().isSequenceOf(value, lazy optional (Rule().isManyOf(lazy Rule().isSequenceOf(lazy Rule().is ",", value))))
and assignment = lazy Rule("assignment").isSequenceOf(_var, lazy Rule().is ":", value)
and assignmentSeq: Lazy<rule> =
  lazy Rule().isSequenceOf(assignment, lazy optional (Rule().isManyOf(lazy Rule().isSequenceOf(lazy Rule().is ",", assignment))))

// let anything: Lazy<rule> = lazy function hd::rest -> Some(T(hd, []), rest) | _ -> None
// let var = lazy Rule().is("[xyz]")
// let prop = lazy Rule("prop").isSequenceOf(var, lazy Rule().is "=", anything)
// let openTag =  // note: only the last one of `isSequenceOf` should be `isOneOf`, or otherwise it won't work (fail on one branch -> doesn't check others)
//   lazy Rule("open tag").isSequenceOf(lazy Rule().is "<", var, lazy optional (Rule().isManyOf prop), lazy Rule().is ">")
// let closeTag = lazy Rule("close tag").isSequenceOf(lazy Rule().is "<", lazy Rule().is "/", var, lazy Rule().is ">")
// let containedTag = lazy Rule("open tag").isSequenceOf(lazy Rule().is "<", var, lazy optional (Rule().isManyOf prop), lazy Rule().is "/", lazy Rule().is ">")
// let rec value = lazy Rule().isOneOf(containedTag, lazy Rule().isSequenceOf(openTag, lazy optional (Rule().isManyOf value), closeTag))

// value.Force() tokens