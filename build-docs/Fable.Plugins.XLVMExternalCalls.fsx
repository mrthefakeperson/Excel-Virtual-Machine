namespace Fable.Plugins
#r "../docs/fable-core/Fable.Core.dll"
open Fable
open Fable.AST

type PatchedMethods() =
  interface IReplacePlugin with
    member x.TryReplace com (info:Fable.ApplyInfo) =
      match info.ownerFullName, info.methodName, info.args with
      |"System.Char", "IsDigit", [s] ->       // char => string after compilation
        let emitExpr = Fable.Value (Fable.Emit "!isNaN(parseInt($0))")
        Some (Fable.Apply(emitExpr, [s], Fable.ApplyMeth, info.returnType, info.range))
      |"System.Char", "IsLetter", [s] ->      // only doing a-z and A-Z
        let emitExpr = Fable.Value (Fable.Emit @"/^[a-zA-Z]$/.test($0)")
        Some (Fable.Apply(emitExpr, [s], Fable.ApplyMeth, info.returnType, info.range))
      |_ -> None
