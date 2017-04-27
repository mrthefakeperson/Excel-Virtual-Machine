module private Output.ASM
open PseudoASM.Definition
open System
open System.IO
open System.Collections.Generic

// compile to asm
let writeASM fileName cmds =
  let getLabel = sprintf "label_%i"
  let convertFwdShift = fun i -> function
    |PushFwdShift n -> Push (getLabel (i + n))
    |GotoFwdShift n -> GotoFwdShift (i + n)
    |GotoIfTrueFwdShift n -> GotoIfTrueFwdShift (i + n)
    |x -> x
  let cmds = Seq.mapi convertFwdShift cmds
  let (|Goto|GotoIfTrue|Unused|) = function
    |GotoFwdShift x -> Goto x
    |GotoIfTrueFwdShift x -> GotoIfTrue x
    |_ -> Unused
  let getLocal e = sprintf "-%i(%sbp)" (e * 4) "%e"
  let varToLocal =
    Seq.choose (function Store s | Load s -> Some s | _ -> None) cmds
     |> Set.ofSeq |> Seq.toArray
     |> Seq.mapi (fun i e -> e, getLocal i)
     |> dict
  let numberOfLocalVariables = varToLocal.Count
  let mapVars = function
    |Store s -> Store varToLocal.[s]
    |Load s -> Load varToLocal.[s]
    |x -> x
  let cmds = Seq.map mapVars cmds
  let GAS =
    let labels = Seq.init (Seq.length cmds) getLabel
    let usedLabels =
      HashSet(
        Seq.choose
         (function Push s -> Some s | Goto x | GotoIfTrue x -> Some (getLabel x) | _ -> None)
         cmds
       )
    let combine2TopFirst name = ["popl %eax"; "popl %ebx"; name + " %eax, %ebx"; "pushl %ebx"]
    let code =
      Seq.map (function
        |Push x ->
          if fst (Int32.TryParse x)
           then [sprintf "pushl $%s" x]
           else failwith "todo: convert strings and other literals"
        |PushFwdShift _ | GotoFwdShift _ | GotoIfTrueFwdShift _ -> failwith "should never happen"
        |Pop -> ["addl $4, %esp"]
        |Load x -> ["leal " + x + ", %eax"; "pushl %eax"] // [sprintf "pushl %s" x]
        |Store x -> ["popl %eax"; "mov %eax, " + x]
        |Goto x -> ["jmp " + getLabel x]
        |GotoIfTrue x -> ["jnz " + getLabel x]
        |Call -> ["call"]
        |Return -> ["ret"]
//        |NewHeap ->
//        |GetHeap ->
//        |WriteHeap ->
//        |Input ->
//        |Output ->
        |Combinator_2(_, "+") -> combine2TopFirst "addl"
        |Combinator_2(_, "-") -> combine2TopFirst "subl"
        |Combinator_2(_, "*") -> combine2TopFirst "mull"
        |Combinator_2(_, "/") -> combine2TopFirst "divl"
        |Combinator_2(_, "%") -> combine2TopFirst "modl"
//        |Combinator_2(_, "<") -> 
//        |Combinator_2(_, "<=") -> 
//        |Combinator_2(_, "=") -> 
//        |Combinator_2(_, "<>") -> 
//        |Combinator_2(_, ">") -> 
//        |Combinator_2(_, ">=") -> 
//        |Combinator_2(_, "&&") -> 
//        |Combinator_2(_, "||") -> 
        |Combinator_2(_, _) as e -> failwithf "unknown command: %A" e
       ) cmds
    Seq.map2 (fun _GASLines label ->
      match _GASLines with
      |hd::tl when usedLabels.Contains label -> label + ":\t" + hd::List.map ((+) "\t") tl
      |_ -> List.map ((+) "\t") _GASLines
     ) code labels
     |> Seq.concat
  File.WriteAllLines(fileName, Array.ofSeq GAS)