#r "lex.dll"
#load "framework.fs"
open Framework

let text = """
<x>
  <y z=123/>
  <z x=1.2 y=1.3> </z>
</x>
"""

let tokens = Lexer.tokenize text |> List.ofSeq

let anything: Lazy<rule> = lazy function hd::rest -> Some(T(hd, []), rest) | _ -> None
let var = lazy Rule().isOneOf("x", "y", "z")
let prop = lazy Rule("prop").isSequenceOf(var, lazy Rule().is "=", anything)
let openTag =  // note: only the last one of `isSequenceOf` should be `isOneOf`, or otherwise it won't work (fail on one branch -> doesn't check others)
  lazy Rule("open tag").isSequenceOf(lazy Rule().is "<", var, lazy optional (Rule().isManyOf prop), lazy Rule().is ">")
let closeTag = lazy Rule("close tag").isSequenceOf(lazy Rule().is "<", lazy Rule().is "/", var, lazy Rule().is ">")
let containedTag = lazy Rule("open tag").isSequenceOf(lazy Rule().is "<", var, lazy optional (Rule().isManyOf prop), lazy Rule().is "/", lazy Rule().is ">")
let rec value = lazy Rule().isOneOf(containedTag, lazy Rule().isSequenceOf(openTag, lazy optional (Rule().isManyOf value), closeTag))

value.Force() tokens