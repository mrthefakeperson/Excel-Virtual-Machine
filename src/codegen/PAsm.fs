module Codegen.PAsm

let NUM_REAL_REGS = 4
let STACK_START = 100000
let CODE_START = 200000

type Register =
  |R of int | RX
  |BP | SP
  |PSR_EQ | PSR_LT | PSR_GT

type Memory =
  |Lbl of string
  |Indirect of Register

// contains reference to a var whose reference cannot be directly expressed
type Handle =
  |HandleLbl of string
  |HandleReg of int

type 'a Asm =
  |Data of 'a[]
  |Label of name: string
  |Push of Register
  |PushC of 'a
  |PushRealRs
  |Pop of Register
  |PopRealRs
  |ShiftStackDown of offset: int * length: int  // tail recursion shortcut: shift {length} items below SP starting at {offset} spaces above SP
  |MovRR of Register * Register
  |MovRM of Register * Memory
  |MovMR of Memory * Register
  |MovRC of Register * 'a
  |MovRHandle of Register * Handle  // move the address value directly into a register; useful when address is not available at time of codegen
  |Cmp of Register * Register  // sets comparison flags (a - b, (> 0)? (= 0)? (< 0)?)
  |CmpC of Register * 'a
  |Br of label: string  // unconditional branch
  |Br0 of label: string  // branch if EQ flag is 0
  |BrT of label: string  // branch if EQ flag is not 0
  |BrLT of label: string
  |BrGT of label: string
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


