﻿module private Output.ASM
open PseudoASM.Definition
open System
open System.IO
open System.Collections.Generic

// compile to GCC asm
// registers are for operations, (%ebp) is the heap pointer, -4(%ebp) reserved for scanning, division
let generateCommands cmds =

  let convertIdiomaticSymbols = function
    |Push "()" | Push "nothing" -> Push "0"
    |Push "endArr" -> Push "7777777"         // bugs occur around this value
    |Push s when s.ToLower() = "true" -> Push "1"
    |Push s when s.ToLower() = "false" -> Push "0"
    |x -> x
  let cmds = Seq.map convertIdiomaticSymbols cmds

  let getTextAddress = sprintf "defined_%i"
  let convertStrings = fun i -> function
    |Push s when not (fst (Int32.TryParse s)) ->
//      printfn "found string `%s`" ((s.Replace("\n", "\\n")))
      Some (sprintf "%s:\t.ascii \"%s\0\"" (getTextAddress i) (s.Replace("\n", "\\n"))),
       Push ("$" + getTextAddress i)
    |x -> None, x
  let textDefinitions, cmds = Seq.mapi convertStrings cmds |> Seq.toArray |> Array.unzip
  let textDefinitions = Array.choose id textDefinitions

  // number off the commands (in p-asm), convert the forward shifts to reference the appropriate command
  let getLabel = sprintf "label_%i"
  let convertFwdShift = fun i -> function
    |PushFwdShift n -> PushFwdShift (i + n)
    |GotoFwdShift n -> GotoFwdShift (i + n)
    |GotoIfTrueFwdShift n -> GotoIfTrueFwdShift (i + n)
    |x -> x
  let cmds = Seq.mapi convertFwdShift cmds

  // number off the local variables, convert the locals to the appropriate stack addresses
  let getLocal e = "-" + string (e * 4 + 4) + "(%ebp)"
  let varToLocal =
    Seq.choose (function Store s | Load s -> Some s | _ -> None) cmds
     |> Set.ofSeq |> Seq.toArray
     |> Seq.mapi (fun i e -> e, getLocal i)
     |> dict
  let mapVars = function
    |Store s -> Store varToLocal.[s]
    |Load s -> Load varToLocal.[s]
    |x -> x
  let cmds = Seq.map mapVars cmds
  let numberOfLocalVariables = varToLocal.Count + 2  // + 2 reserved variables
  // put at start to allocate space for locals and set heap pointer to 0
  let declareLocals = [|"\tsubl $" + string (4 * numberOfLocalVariables) + ", %esp"; "\tmovl $0, (%ebp)"|]
  // put at end to pop all locals; if it segfaults after this then the stack pointer is being mismanaged
  let leave = [|"\taddl $" + string (4 * numberOfLocalVariables + 4) + ", %esp"|]

  // generate GNU ASM code
  let GAS =
    let labels = Seq.init (Seq.length cmds) getLabel
    let usedLabels =
      HashSet(
        Seq.choose
         (function PushFwdShift x | GotoFwdShift x | GotoIfTrueFwdShift x -> Some (getLabel x) | _ -> None)
         cmds
       )
    let comb2Arith name = ["popl %edx"; "popl %eax"; name + " %eax, %edx"; "pushl %edx"]
    let comb2Divide register = ["popl %ecx"; "movl %ecx, %eax"; "xorl %edx, %edx"; "cltd"; "idivl (%esp)"; "movl " + register + ", (%esp)"]
    let comb2Comp name = ["popl %eax"; "popl %edx"; "cmpl %edx, %eax"; name + " %dl"; "xorl %eax, %eax"; "movb %dl, %al"; "push %eax"]
    let code =
      Seq.map (function
        |Push "%i" -> ["pushl $IntFormat"] | Push "%s" -> ["pushl $StringFormat"]
        |Push x ->
          if x.Length > 0 && x.[0] = '$' then ["pushl " + x]
          elif fst (Int32.TryParse x) then ["pushl $" + x]
          else failwithf "%A  todo: convert strings and other literals" x
        |PushFwdShift x -> ["pushl $" + getLabel x]
        |Pop -> ["addl $4, %esp"]
        |Load x -> ["leal " + x + ", %eax"; "pushl (%eax)"] // [sprintf "pushl %s" x]
        |Store x -> ["popl %eax"; "movl %eax, " + x]
        |GotoFwdShift x -> ["jmp " + getLabel x]
        |GotoIfTrueFwdShift x -> ["popl %eax"; "cmpl $0, %eax"; "jne " + getLabel x]
        |Call -> ["popl %eax"; "call *%eax"]
        |Return -> ["ret"]
        |NewHeap -> ["pushl (%ebp)"; "addl $1, (%ebp)"]
        |GetHeap -> ["popl %eax"; "imull $4, %eax"; "addl $_heap, %eax"; "pushl (%eax)"]
        |WriteHeap -> ["popl %edx"; "popl %eax"; "imull $4, %eax"; "addl $_heap, %eax"; "movl %edx, (%eax)"]
        |Input _ -> ["leal -4(%ebp), %eax"; "pushl %eax"; "pushl $IntFormat"; "call _scanf"; "addl $8, %esp"; "pushl -4(%ebp)"]
        |Output "%i" -> ["pushl $IntFormat"; "call _printf"; "addl $8, %esp"]
        |Output _ -> ["pushl $StringFormat"; "call _printf"; "addl $8, %esp"]  // %s
        |Combinator_2(_, "+") -> comb2Arith "addl"
        |Combinator_2(_, "-") -> comb2Arith "subl"
        |Combinator_2(_, "*") -> comb2Arith "imull"
        |Combinator_2(_, "/") -> comb2Divide "%eax"
        |Combinator_2(_, "%") -> comb2Divide "%edx"
        |Combinator_2(_, "<") -> comb2Comp "setl"
        |Combinator_2(_, "<=") -> comb2Comp "setle"
        |Combinator_2(_, "=") -> comb2Comp "sete"
        |Combinator_2(_, "<>") -> comb2Comp "setne"
        |Combinator_2(_, ">") -> comb2Comp "setg"
        |Combinator_2(_, ">=") -> comb2Comp "setge"
        |Combinator_2(_, "&&") -> comb2Arith "andl"
        |Combinator_2(_, "||") -> comb2Arith "orl"
        |Combinator_2(_, _) as e -> failwithf "unknown command: %A" e
       ) cmds
    Seq.map2 (fun _GASLines label ->
      match _GASLines with
      |hd::tl when usedLabels.Contains label -> label + ":\t" + hd::List.map ((+) "\t") tl
      |_ -> List.map ((+) "\t") _GASLines
     ) code labels
     |> Seq.concat
     |> Array.ofSeq

  textDefinitions, Array.concat [|declareLocals; GAS; leave|]

let writeASM fileName cmds =
  let constants, program = generateCommands cmds
  let definitions =
    """
.text
IntFormat:  .ascii "%i\0"
StringFormat:   .ascii "%s\0"
    """.Split '\n' |> fun e -> Array.append e constants
  let start =
    """
.globl _main
_main:
	pushl %ebp
	movl %esp, %ebp
    """.Split '\n'
  let exit =
    """
	popl %ebp
	ret
	.def	_printf;	.scl	2;	.type	32;	.endef
	.def	_scanf;	.scl	2;	.type	32;	.endef
	.comm	_heap, 4000
    """.Split '\n'
  File.WriteAllLines(fileName, Array.concat [|definitions; start; program; exit|])

let debugASM fileName arrangeCmds actCmds =
  let definitions =
    """
.text
IntFormat:  .ascii "%i\0"
StringFormat:   .ascii "%s\0"
Debugging:
	.ascii "register a: %i\nregister b: %i\nregister c: %i\nregister d: %i\nstack pointer: %i\nheap pointer: %i\n\0"
.globl _main
_main:
	pushl %ebp
	movl %esp, %ebp
    """.Trim('\n').Split '\n'
  let definitions2 =
    """
	jmp begin
printStuff:
	movl $5, %edx
	xorl %ebx, %ebx
	pushl (%ebp)
	pushl %esp
	pushl %edx
	pushl %ecx
	pushl %ebx
	pushl %eax
	pushl $Debugging
	call _printf
	addl $28, %esp
	ret
begin:
	call printStuff
    """.Trim('\n').Split '\n'
  let exit =
    """
	call printStuff
	leave
	ret
	.def	_printf;	.scl	2;	.type	32;	.endef
	.def	_scanf;	.scl	2;	.type	32;	.endef
	.comm	_heap, 4000
    """.Trim('\n').Split '\n'
  let (_a, a), (_b, b) = generateCommands arrangeCmds, generateCommands actCmds  // strings currently not in debug mode
  File.WriteAllLines(fileName,
    Array.concat [|definitions; a.[..a.Length - 2]; definitions2; b.[2..b.Length - 2]; exit|])