module ParserCombinators
open Utils

type ParseResult<'input, 'output> = ('input * 'output) option
type ParseError = {
  message: string Lazy
  priority: int  // lowest priority value error is propagated
 }
type Rule<'input, 'output> = Lazy<ParseError ref -> 'input -> ParseResult<'input, 'output>>

let combine_errors (a: ParseError) (b: ParseError) =
  if a.priority < b.priority then a else b

let Error (err: ParseError ref) (err': ParseError) =
  err := combine_errors err.Value err'
  None

// use the following for cleaner recursive parsers
// type Rule1<'i, 'o_, 'o> =
//   |Match of recognizer: ('i -> ParseResult<'i, 'o>)
//   |Map of ('o_ -> 'o) * Rule2<'i, 'o_>
//   |Series of Rule2<'i, 'o_> * Rule2<'i, 'o>  // returns ('output_inner, 'output)
//   |Parallel of Rule2<'i, 'o> * Rule2<'i, 'o>
// and Rule2<'i, 'o> = Rule1<'i, obj, 'o>

// let Series (r1: Rule2<'i, 'o1>) (r2: Rule2<'i, 'o2>) : Rule2<'i, 'o1 * 'o2> =
//   Match (fun input -> run_parser r1 +/ run_parser r2)

let ParseResultMap (binding: 'o1 -> 'o2) : ParseResult<'i, 'o1> -> ParseResult<'i, 'o2> =
  Option.map (fun (i, o) -> (i, binding o))
  
let Map (binding: 'o1 -> 'o2) (rule: Rule<'i, 'o1>) : Rule<'i, 'o2> =
  lazy
    fun err -> rule.Force() err >> ParseResultMap binding
let (<-/) = Map
let (->/) r f = Map f r

let Series (rule1: Rule<'i, 'o1>) (rule2: Rule<'i, 'o2>) : Rule<'i, 'o1 * 'o2> =
  lazy
    fun err input ->
      rule1.Force() err input
       |> Option.bind (fun (input', res1) ->
            rule2.Force() err input'
             |> ParseResultMap (mkpair res1)
           )
let (+/) = Series
type SequenceBuilder = SequenceOf
  with
    member __.Bind(rule: Rule<'i, 'o1>, cont: 'o1 -> Rule<'i, 'o2>) : Rule<'i, 'o2> =
      lazy
        fun err input ->
          match rule.Force() err input with
          |Some(input', x) -> (cont x).Force() err input'
          |None -> None
    member __.Return(x: 'o) : Rule<'i, 'o> =
      lazy fun err input -> Some(input, x)

let Parallel (rule1: Rule<'i, 'o>) (rule2: Rule<'i, 'o>) : Rule<'i, 'o> =
  lazy
    fun err input ->
      match rule1.Force() err input with
      |Some res -> Some res
      |None -> rule2.Force() err input
let (|/) = Parallel
let OneOf (rules: Rule<'i, 'o> list) : Rule<'i, 'o> = List.reduce (|/) rules

let Optional (rule: Rule<'i, 'o>) : Rule<'i, 'o option> =
  lazy
    fun err input ->
      match rule.Force() err input with
      |Some(input', res) -> Some(input', Some res)
      |None -> Some(input, None)

let FoldListOf (f: 'o -> Rule<'i, 'o>) (z: 'o) : Rule<'i, 'o> =
  let rec parse_list acc err input =
    match (f acc).Force() err input with
    |Some(input', res) -> parse_list res err input'
    |None -> Some(input, acc)
  lazy parse_list z

let FoldBackListOf (f: 'o1 -> 'o -> 'o) (rule: Rule<'i, 'o1>) (z: Rule<'i, 'o>) : Rule<'i, 'o> =
  let rec parse_list err input =
    match rule.Force() err input with
    |Some(input', res) -> ParseResultMap (f res) (parse_list err input')
    |None -> z.Force() err input
  lazy parse_list

let OptionalListOf rule : Rule<'i, 'o list> =
  FoldBackListOf (fun a b -> a::b) rule (lazy fun err i -> Some(i, []))

let ListOf rule : Rule<'i, 'o list> = rule +/ OptionalListOf rule ->/ List.Cons

let JoinedListOf (rule: Rule<'i, 'o>) (sep: Rule<'i, unit>) : Rule<'i, 'o list> =
  rule +/ (OptionalListOf (sep +/ rule ->/ snd)) ->/ List.Cons

let LookAhead (rule: Rule<'i, 'o>) : Rule<'i, 'o> =
  lazy
    fun err input ->
      Option.map (fun (i, o) -> (input, o)) (rule.Force() err input)
let (&/) rule1 rule2 = LookAhead rule1 +/ rule2 ->/ snd

let inline Equal (token: string) : Rule< ^I, 'o> =
  lazy
    fun err input ->
      match (^I: (member atomic_equal: (string -> Option< ^I * 'o>)) input) token with
      |None ->
        Error err {
          message = lazy sprintf "expected token %A" token
          priority = (^I: (member length: int) input)
         }
      |some -> some
let inline (~%) x = Equal x
let inline (!) x = Equal x ->/ ignore

let inline Match (rgx: string) : Rule< ^I, 'o> =
  lazy
    fun err input ->
      match (^I: (member atomic_match: (string -> Option< ^I * 'o>)) input) rgx with
      |None ->
        Error err {
          message = lazy sprintf "expected regex %A" rgx
          priority = (^I: (member length: int) input)
         }
      |some -> some
let inline (~%%) rgx = Match rgx
let inline (!!) rgx = Match rgx ->/ ignore

let inline End () : Rule< ^I, unit> =
  lazy
    fun err (input: ^I) ->
      let empty = (^I: (static member empty: ^I) ())
      if input = empty
       then Some(empty, ())
       else
        Error err {
          message = lazy "expected end of stream"
          priority = (^I: (member length: int) input)
         }

let inline run_parser (parse_rule: Rule< ^I, 'o>) (input: ^I) : 'o =
  let error = ref {message = lazy ""; priority = 99999999}
  match parse_rule.Force() error input with
  |Some(rest, result) ->
    if rest = (^I: (static member empty: ^I) ())
     then result
     else failwithf "parser did not reach end: result = %A, remaining = %A" result rest
  |None ->
    failwithf "parser failed with %i remaining elements:\n%s"
     error.Value.priority
     (error.Value.message.Force())
