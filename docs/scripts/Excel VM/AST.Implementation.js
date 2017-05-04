define(["exports", "./AST.Compile", "./AST.Optimize"], function (exports, _AST, _AST2) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.fromToken = fromToken;

  function fromToken(parseTree, args) {
    var transform = (args.has("optimizations") ? args.get("optimizations") === "off" : false) ? function (e) {
      return (0, _AST.transformFromToken)(e);
    } : function ($var291) {
      return (0, _AST2.all)((0, _AST.transformFromToken)($var291));
    };
    return [transform(parseTree), args];
  }
});