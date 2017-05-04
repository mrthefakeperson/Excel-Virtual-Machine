define(["exports", "fable-core/umd/List"], function (exports, _List) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.SCAN = exports.PRINT = exports.definedPrefixOperators = exports.definedOperators = undefined;
  exports.splitFileExtension = splitFileExtension;

  function splitFileExtension(s) {
    var matchValue = s.lastIndexOf(".");

    if (matchValue === -1) {
      return [s, ""];
    } else {
      return [s.slice(null, matchValue - 1 + 1), s.slice(matchValue + 1, s.length)];
    }
  }

  var definedOperators = exports.definedOperators = (0, _List.ofArray)(["+", "-", "*", "/", "%", "<", "<=", "=", "<>", ">", ">=", "!=", "&&", "||"]);
  var definedPrefixOperators = exports.definedPrefixOperators = (0, _List.ofArray)(["~&", "~*", "~-", "~!"]);
  var PRINT = exports.PRINT = "printf";
  var SCAN = exports.SCAN = "scan";
});