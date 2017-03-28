namespace Write_File
open Excel_Language.Definitions
open ASM_Compiler

module Definitions =
  let cmdToStrPair i = function
    |Push e -> "push", e | PushFwdShift x -> "push", string(i + x) | Pop -> "pop", ""
    |Store e -> "store", e | Load e -> "load", string(alphaToNumber e - 5) | Popv e -> "popv", e
    |GotoFwdShift x -> "goto", string(i + x) | GotoIfTrueFwdShift x -> "gotoiftrue", string(i + x)
    |Call -> "call", "" | Return -> "return", ""
    |GetHeap -> "getheap", "" | NewHeap -> "newheap", "" | WriteHeap -> "writeheap", ""
    |Input s -> "input", s | OutputLine _ -> "outputline", ""
    |Combinator_2 c -> c.ToStrPair()
