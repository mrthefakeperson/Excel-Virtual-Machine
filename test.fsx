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

let _var = !!"[a-z A-Z][a-z 0-9 A-Z _]*"
let _string = !!"\"(\\\"|[^\"])*\""
let _int = !!"-?[0-9]+"
let rec value() = () |> (
  !"{" +/ ~~assignmentSeq +/ !"}" ->/ "object"
   |/ !"[" +/ ~~valueSeq +/ !"]" ->/ "list"
   |/ _string |/ _int
 )
and valueSeq() = () |> value +/ ~~(+(!"," +/ value))
and assignment() = () |> _var +/ !":" +/ value ->/ "assignment"
and assignmentSeq() = () |> !!!(assignment +/ ~~(+(!"," +/ assignment)))

// let _var = lazy Rule().matches("[a-z A-Z][a-z 0-9 A-Z _]*")
// let _string = lazy Rule().matches("\"(\\\"|[^\"])*\"")
// let _int = lazy Rule().matches("-?[0-9]+")
// let rec value =
//   lazy Rule().isOneOf(
//     lazy Rule("object").isSequenceOf(lazy Rule().is "{", lazy optional (assignmentSeq.Force()), lazy Rule().is "}"),
//     lazy Rule("list").isSequenceOf(lazy Rule().is "[", lazy optional (valueSeq.Force()), lazy Rule().is "]"),
//     lazy Rule().isOneOf(_string, _int)
//    )
// and valueSeq: Lazy<rule> =
//   lazy Rule().isSequenceOf(value, lazy optional (Rule().isManyOf(lazy Rule().isSequenceOf(lazy Rule().is ",", value))))
// and assignment = lazy Rule("assignment").isSequenceOf(_var, lazy Rule().is ":", value)
// and assignmentSeq: Lazy<rule> =
//   lazy Rule().isSequenceOf(assignment, lazy optional (Rule().isManyOf(lazy Rule().isSequenceOf(lazy Rule().is ",", assignment))))
