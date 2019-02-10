module Codegen.PAsm
open Parser.AST

type Register =
  |R of int
  |BP
  |SP
  |RX

type Memory =
  |Lbl of string
  |Indirect of Register

type 'a Asm =
  |Data of 'a[]
  |Label of name: string
  |Push of Register
  |PushC of 'a
  |PushRealRs
  |Pop of Register
  |PopRealRs
  |MovRR of Register * Register
  |MovRM of Register * Memory
  |MovMR of Memory * Register
  |MovRC of Register * 'a
  |Br of label: string  // unconditional branch
  |Br0 of label: string  // branch if R0 is 0
  |BrT of label: string  // branch if R0 is not 0
  |Call of label: string
  |Ret
  // operations
  |Alloc of bytes: int  // alloc an address into R0
  |Add of Register * Register
  |AddC of Register * 'a
  |Sub of Register * Register
  |SubC of Register * 'a
  |Mul of Register * Register
  |MulC of Register * 'a
  |DivMod of Register * Register
  |DivModC of Register * 'a
  |Cmp of Register * Register

