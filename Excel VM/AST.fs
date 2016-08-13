module AST
(*
type Expr=
  |V of string
  |Define of string*Expr
  |Bind of string*Expr*Expr
  |Apply of Expr*Expr
  |Condition of Expr*Expr*Expr
*)
type Expr=
  |V of string
  |Define of Expr list*Expr
  |Bind of Expr*Expr*Expr
  |Apply of Expr*Expr list
  |Condition of Expr*Expr*Expr
  |Struct of Expr[]
  |Index of Expr*int
