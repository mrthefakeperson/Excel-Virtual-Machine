define(["exports", "fable-core/umd/List", "fable-core/umd/Set", "fable-core/umd/Seq", "fable-core/umd/GenericComparer", "fable-core/umd/Util", "fable-core/umd/String", "../Excel VM/ASM Compiler", "../Excel VM/AST Compiler", "../Excel VM/Type System", "../Excel VM/Parser_C"], function (exports, _List, _Set, _Seq, _GenericComparer, _Util, _String, _ASMCompiler, _ASTCompiler, _TypeSystem, _Parser_C) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.compileAndRun = exports.Interpreter = undefined;

  var _List2 = _interopRequireDefault(_List);

  var _GenericComparer2 = _interopRequireDefault(_GenericComparer);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

  var Interpreter = exports.Interpreter = function (__exports) {
    var interpretPAsm = __exports.interpretPAsm = function (cmds, stdInput) {
      var pushstack = function pushstack(stack) {
        return function (v) {
          stack.contents = new _List2.default(v, stack.contents);
        };
      };

      var popstack = function popstack(stack_1) {
        var matchValue = stack_1.contents;

        if (matchValue.tail == null) {
          stack_1.contents = new _List2.default();
        } else {
          stack_1.contents = matchValue.tail;
        }
      };

      var topstack = function topstack(stack_2) {
        var matchValue_1 = stack_2.contents;

        if (matchValue_1.tail == null) {
          throw new Error("took top of an empty stack");
        } else {
          return matchValue_1.head;
        }
      };

      var stacks = void 0;
      var x = Array.from((0, _Set.create)(Array.from((0, _Seq.rangeChar)("A", "E")).map(function (value) {
        return value;
      }).concat(Array.from((0, _Seq.choose)(function (_arg1) {
        var $var268 = _arg1.Case === "Store" ? [0, _arg1.Fields[0]] : _arg1.Case === "Load" ? [0, _arg1.Fields[0]] : _arg1.Case === "Popv" ? [0, _arg1.Fields[0]] : [1];

        switch ($var268[0]) {
          case 0:
            return $var268[1];

          case 1:
            return null;
        }
      }, cmds))), new _GenericComparer2.default(_Util.compare)));
      stacks = new Map(Array.from((0, _Seq.zip)(x, Array.from((0, _Seq.initialize)(x.length, function (_arg1_1) {
        return {
          contents: new _List2.default()
        };
      })))));

      var push = function push(name) {
        return pushstack(stacks.get(name));
      };

      var pop = function pop(name_1) {
        popstack(stacks.get(name_1));
      };

      var top = function top(name_2) {
        return topstack(stacks.get(name_2));
      };

      var patternInput = ["A", "B", "C", "D", "E"];
      var heap = [];
      (0, _String.split)(stdInput, " ").slice().reverse().forEach(push(patternInput[3]));
      (0, _String.fsFormat)("input: %A")(function (x) {
        console.log(x);
      })(stacks.get(patternInput[3]).contents);

      var interpretCmd = function interpretCmd(i) {
        var fwd = function fwd($var269) {
          return String(i - 1 + $var269);
        };

        var fwd2 = function fwd2($var270) {
          return function (value_1) {
            return String(value_1);
          }(function (y) {
            return i + y;
          }($var270));
        };

        return function (_arg2) {
          if (_arg2.Case === "PushFwdShift") {
            push(patternInput[1])(fwd2(_arg2.Fields[0]));
          } else if (_arg2.Case === "Pop") {
            pop(patternInput[1]);
          } else if (_arg2.Case === "Store") {
            push(_arg2.Fields[0])(top(patternInput[1]));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Load") {
            push(patternInput[1])(top(_arg2.Fields[0]));
          } else if (_arg2.Case === "Popv") {
            pop(_arg2.Fields[0]);
          } else if (_arg2.Case === "GotoFwdShift") {
            pop(patternInput[0]);
            push(patternInput[0])(fwd(_arg2.Fields[0]));
          } else if (_arg2.Case === "GotoIfTrueFwdShift") {
            if (top(patternInput[1]).toLocaleLowerCase() === "true") {
              pop(patternInput[0]);
              push(patternInput[0])(fwd(_arg2.Fields[0]));
            }

            pop(patternInput[1]);
          } else if (_arg2.Case === "Call") {
            push(patternInput[0])(String(Number.parseInt(top(patternInput[1])) - 1));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Return") {
            pop(patternInput[0]);
          } else if (_arg2.Case === "GetHeap") {
            var yld = heap[Number.parseInt(top(patternInput[1]))];
            pop(patternInput[1]);
            push(patternInput[1])(yld);
          } else if (_arg2.Case === "NewHeap") {
            heap.push("");
            push(patternInput[1])(String(heap.length - 1));
          } else if (_arg2.Case === "WriteHeap") {
            var v_1 = top(patternInput[1]);
            pop(patternInput[1]);
            var i_1 = top(patternInput[1]);
            pop(patternInput[1]);
            heap[Number.parseInt(i_1)] = v_1;
          } else if (_arg2.Case === "Input") {
            push(patternInput[1])(top(patternInput[3]));
            pop(patternInput[3]);
          } else if (_arg2.Case === "Output") {
            push(patternInput[4])(top(patternInput[1]));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Combinator_2") {
            var a = top(patternInput[1]);
            pop(patternInput[1]);
            var b = top(patternInput[1]);
            pop(patternInput[1]);
            push(patternInput[1])(function (arg00) {
              return function (arg10) {
                return _arg2.Fields[0].Interpret(arg00, arg10);
              };
            }(a)(b));
          } else {
            push(patternInput[1])(_arg2.Fields[0]);
          }
        };
      };

      push(patternInput[0])("0");

      while (Number.parseInt(top(patternInput[0])) < cmds.length ? stacks.get(patternInput[4]).contents.length < 50 : false) {
        var i_2 = Number.parseInt(top(patternInput[0]));

        try {
          interpretCmd(i_2)(cmds[i_2]);
        } catch (ex) {
          (0, _String.fsFormat)("%A")(function (x) {
            console.log(x);
          })(ex);
          (0, _String.fsFormat)("stack %A")(function (x) {
            console.log(x);
          })(stacks.get(patternInput[1]).contents);
          (0, _String.fsFormat)("heap [%s]")(function (x) {
            console.log(x);
          })((0, _String.join)("; ", (0, _Seq.map)((0, _String.fsFormat)("%A")(function (x) {
            return x;
          }), heap)));
          (0, _String.fsFormat)("%A")(function (x) {
            console.log(x);
          })(cmds[i_2]);
        }

        var pt = Number.parseInt(top(patternInput[0]));
        pop(patternInput[0]);
        push(patternInput[0])(String(pt + 1));
      }

      (0, _String.fsFormat)("%A")(function (x) {
        console.log(x);
      })([stacks.get(patternInput[1]).contents, heap, (0, _List.reverse)(stacks.get(patternInput[4]).contents)]);
      return (0, _String.join)("", (0, _List.reverse)(stacks.get(patternInput[4]).contents));
    };

    return __exports;
  }({});

  var compileAndRun = exports.compileAndRun = function compileAndRun($var276) {
    return function (cmds) {
      return function (stdInput) {
        return Interpreter.interpretPAsm(cmds, stdInput);
      };
    }(function ($var275) {
      return function (list) {
        return Array.from(list);
      }(function ($var274) {
        return function (e_3) {
          return (0, _ASMCompiler.compile)(e_3);
        }(function ($var273) {
          return function (e_2) {
            return (0, _ASTCompiler.ASTCompile)(e_2);
          }(function ($var272) {
            return (0, _TypeSystem.applyTypeSystem)(function ($var271) {
              return function (e_1) {
                return e_1.Clean();
              }(function (e) {
                return (0, _Parser_C.parseSyntax)(e);
              }($var271));
            }($var272));
          }($var273));
        }($var274));
      }($var275));
    }($var276));
  };
});