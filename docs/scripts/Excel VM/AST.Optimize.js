define(["exports", "./AST.Definition", "fable-core/umd/List", "fable-core/umd/Seq"], function (exports, _AST, _List, _Seq) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.TCOPreprocess = undefined;
  exports.all = all;

  var TCOPreprocess = exports.TCOPreprocess = function () {
    var TCOPreprocess_ = function TCOPreprocess_(_arg1) {
      var $var290 = _arg1.Case === "Sequence" ? [0] : _arg1.Case === "If" ? [1] : _arg1.Case === "Return" ? [2, _arg1.Fields[0]] : [2, _arg1];

      switch ($var290[0]) {
        case 0:
          return new _AST.AST("Sequence", [(0, _List.append)((0, _Seq.toList)((0, _Seq.take)(_arg1.Fields[0].length - 1, _arg1.Fields[0])), (0, _List.ofArray)([TCOPreprocess_((0, _Seq.last)(_arg1.Fields[0]))]))]);

        case 1:
          return new _AST.AST("If", [_arg1.Fields[0], TCOPreprocess_(_arg1.Fields[1]), TCOPreprocess_(_arg1.Fields[2])]);

        case 2:
          return new _AST.AST("Return", [$var290[1]]);
      }
    };

    var TCOPreprocess_1 = function TCOPreprocess_1(_arg2) {
      if (_arg2.Case === "Define") {
        return new _AST.AST("Define", [_arg2.Fields[0], _arg2.Fields[1], TCOPreprocess_1(TCOPreprocess_(_arg2.Fields[2]))]);
      } else if (_arg2.Case === "Return") {
        return TCOPreprocess_(_arg2.Fields[0]);
      } else {
        return _AST.AST.map(TCOPreprocess_1)(_arg2);
      }
    };

    return TCOPreprocess_1;
  }();

  function all(ast) {
    return TCOPreprocess(ast);
  }
});