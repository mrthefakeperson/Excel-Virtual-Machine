define(["exports", "./PseudoASM.Compile"], function (exports, _PseudoASM) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.fromAST = fromAST;

  function fromAST(ast, args) {
    return [(0, _PseudoASM.CompileToASM)(ast), args];
  }
});