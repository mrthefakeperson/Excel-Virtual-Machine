define(["exports", "../Excel VM/PseudoASM.Definition", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/List", "fable-core/umd/Set", "fable-core/umd/Seq", "fable-core/umd/GenericComparer", "fable-core/umd/String", "../Excel VM/PseudoASM.Implementation", "../Excel VM/AST.Implementation", "../Excel VM/Parser.Implementation", "fable-core/umd/Map"], function (exports, _PseudoASM, _Symbol2, _Util, _List, _Set, _Seq, _GenericComparer, _String, _PseudoASM2, _AST, _Parser, _Map) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.Interpreter = undefined;
  exports.compileAndRun = compileAndRun;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  var _List2 = _interopRequireDefault(_List);

  var _GenericComparer2 = _interopRequireDefault(_GenericComparer);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  function _possibleConstructorReturn(self, call) {
    if (!self) {
      throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
    }

    return call && (typeof call === "object" || typeof call === "function") ? call : self;
  }

  var _createClass = function () {
    function defineProperties(target, props) {
      for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) descriptor.writable = true;
        Object.defineProperty(target, descriptor.key, descriptor);
      }
    }

    return function (Constructor, protoProps, staticProps) {
      if (protoProps) defineProperties(Constructor.prototype, protoProps);
      if (staticProps) defineProperties(Constructor, staticProps);
      return Constructor;
    };
  }();

  function _inherits(subClass, superClass) {
    if (typeof superClass !== "function" && superClass !== null) {
      throw new TypeError("Super expression must either be null or a function, not " + typeof superClass);
    }

    subClass.prototype = Object.create(superClass && superClass.prototype, {
      constructor: {
        value: subClass,
        enumerable: false,
        writable: true,
        configurable: true
      }
    });
    if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass;
  }

  var Interpreter = exports.Interpreter = function (__exports) {
    var Op = __exports.Op = function (_Comb) {
      _inherits(Op, _Comb);

      _createClass(Op, [{
        key: _Symbol3.default.reflection,
        value: function () {
          return (0, _Util.extendInfo)(Op, {
            type: "CompileAndRun.Interpreter.Op",
            interfaces: [],
            properties: {}
          });
        }
      }]);

      function Op(x) {
        _classCallCheck(this, Op);

        var _this = _possibleConstructorReturn(this, (Op.__proto__ || Object.getPrototypeOf(Op)).call(this, "", x.CommandInfo.Symbol));

        return _this;
      }

      return Op;
    }(_PseudoASM.Comb2);

    (0, _Symbol2.setType)("CompileAndRun.Interpreter.Op", Op);

    var implementForType = __exports.implementForType = function (cast, f, a, b) {
      return f(cast(a))(cast(b));
    };

    var k = __exports.k = function (f, a, b) {
      var copyOfStruct = f(a)(b);
      return (0, _Util.toString)(copyOfStruct);
    };

    var castToIntThen = __exports.castToIntThen = function () {
      var cast = function cast(value) {
        return Number.parseInt(value);
      };

      return function (f) {
        return function (a) {
          return function (b) {
            return implementForType(cast, f, a, b);
          };
        };
      };
    }();

    var checkIntFirst = __exports.checkIntFirst = function (fint, fstr, a, b) {
      if (function () {
        var $var301 = Number.parseInt(a, 10);
        return isNaN($var301) ? [false, 0] : [true, $var301];
      }()[0] ? function () {
        var $var302 = Number.parseInt(b, 10);
        return isNaN($var302) ? [false, 0] : [true, $var302];
      }()[0] : false) {
        return castToIntThen(fint)(a)(b);
      } else {
        return fstr(a)(b);
      }
    };

    var castToBoolThen = __exports.castToBoolThen = function () {
      var cast = function cast(s) {
        var matchValue = s.toLocaleLowerCase();

        switch (matchValue) {
          case "true":
            return true;

          case "false":
            return false;

          default:
            throw new Error("not a bool");
        }
      };

      return function (f) {
        return function (a) {
          return function (b) {
            return implementForType(cast, f, a, b);
          };
        };
      };
    }();

    var allCombinators = __exports.allCombinators = (0, _List.ofArray)([new (function (_Op) {
      _inherits(_class, _Op);

      function _class() {
        var _this2, _ret;

        _classCallCheck(this, _class);

        return _ret = (_this2 = _possibleConstructorReturn(this, (_class.__proto__ || Object.getPrototypeOf(_class)).call(this, _PseudoASM.Add)), _this2), _possibleConstructorReturn(_this2, _ret);
      }

      _createClass(_class, [{
        key: "Apply",
        value: function (a, b) {
          return castToIntThen(function () {
            var f = function f(x) {
              return function (y) {
                return x + y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }())(a)(b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class;
    }(Op))(), new (function (_Op2) {
      _inherits(_class2, _Op2);

      function _class2() {
        var _this3, _ret2;

        _classCallCheck(this, _class2);

        return _ret2 = (_this3 = _possibleConstructorReturn(this, (_class2.__proto__ || Object.getPrototypeOf(_class2)).call(this, _PseudoASM.Sub)), _this3), _possibleConstructorReturn(_this3, _ret2);
      }

      _createClass(_class2, [{
        key: "Apply",
        value: function (a, b) {
          return castToIntThen(function () {
            var f = function f(x) {
              return function (y) {
                return x - y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }())(a)(b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class2;
    }(Op))(), new (function (_Op3) {
      _inherits(_class3, _Op3);

      function _class3() {
        var _this4, _ret3;

        _classCallCheck(this, _class3);

        return _ret3 = (_this4 = _possibleConstructorReturn(this, (_class3.__proto__ || Object.getPrototypeOf(_class3)).call(this, _PseudoASM.Mul)), _this4), _possibleConstructorReturn(_this4, _ret3);
      }

      _createClass(_class3, [{
        key: "Apply",
        value: function (a, b) {
          return castToIntThen(function () {
            var f = function f(x) {
              return function (y) {
                return x * y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }())(a)(b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class3;
    }(Op))(), new (function (_Op4) {
      _inherits(_class4, _Op4);

      function _class4() {
        var _this5, _ret4;

        _classCallCheck(this, _class4);

        return _ret4 = (_this5 = _possibleConstructorReturn(this, (_class4.__proto__ || Object.getPrototypeOf(_class4)).call(this, _PseudoASM.Div)), _this5), _possibleConstructorReturn(_this5, _ret4);
      }

      _createClass(_class4, [{
        key: "Apply",
        value: function (a, b) {
          return castToIntThen(function () {
            var f = function f(x) {
              return function (y) {
                return ~~(x / y);
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }())(a)(b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class4;
    }(Op))(), new (function (_Op5) {
      _inherits(_class5, _Op5);

      function _class5() {
        var _this6, _ret5;

        _classCallCheck(this, _class5);

        return _ret5 = (_this6 = _possibleConstructorReturn(this, (_class5.__proto__ || Object.getPrototypeOf(_class5)).call(this, _PseudoASM.Mod)), _this6), _possibleConstructorReturn(_this6, _ret5);
      }

      _createClass(_class5, [{
        key: "Apply",
        value: function (a, b) {
          return castToIntThen(function () {
            var f = function f(x) {
              return function (y) {
                return x % y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }())(a)(b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class5;
    }(Op))(), new (function (_Op6) {
      _inherits(_class6, _Op6);

      function _class6() {
        var _this7, _ret6;

        _classCallCheck(this, _class6);

        return _ret6 = (_this7 = _possibleConstructorReturn(this, (_class6.__proto__ || Object.getPrototypeOf(_class6)).call(this, _PseudoASM.Less)), _this7), _possibleConstructorReturn(_this7, _ret6);
      }

      _createClass(_class6, [{
        key: "Apply",
        value: function (a, b) {
          return checkIntFirst(function () {
            var f = function f(x) {
              return function (y) {
                return x < y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }(), function () {
            var f_1 = function f_1(x_1) {
              return function (y_1) {
                return x_1 < y_1;
              };
            };

            return function (a_2) {
              return function (b_2) {
                return k(f_1, a_2, b_2);
              };
            };
          }(), a, b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class6;
    }(Op))(), new (function (_Op7) {
      _inherits(_class7, _Op7);

      function _class7() {
        var _this8, _ret7;

        _classCallCheck(this, _class7);

        return _ret7 = (_this8 = _possibleConstructorReturn(this, (_class7.__proto__ || Object.getPrototypeOf(_class7)).call(this, _PseudoASM.LEq)), _this8), _possibleConstructorReturn(_this8, _ret7);
      }

      _createClass(_class7, [{
        key: "Apply",
        value: function (a, b) {
          return checkIntFirst(function () {
            var f = function f(x) {
              return function (y) {
                return x <= y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }(), function () {
            var f_1 = function f_1(x_1) {
              return function (y_1) {
                return x_1 <= y_1;
              };
            };

            return function (a_2) {
              return function (b_2) {
                return k(f_1, a_2, b_2);
              };
            };
          }(), a, b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class7;
    }(Op))(), new (function (_Op8) {
      _inherits(_class8, _Op8);

      function _class8() {
        var _this9, _ret8;

        _classCallCheck(this, _class8);

        return _ret8 = (_this9 = _possibleConstructorReturn(this, (_class8.__proto__ || Object.getPrototypeOf(_class8)).call(this, _PseudoASM.Equals)), _this9), _possibleConstructorReturn(_this9, _ret8);
      }

      _createClass(_class8, [{
        key: "Apply",
        value: function (a, b) {
          return checkIntFirst(function () {
            var f = function f(x) {
              return function (y) {
                return x === y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }(), function () {
            var f_1 = function f_1(x_1) {
              return function (y_1) {
                return x_1 === y_1;
              };
            };

            return function (a_2) {
              return function (b_2) {
                return k(f_1, a_2, b_2);
              };
            };
          }(), a, b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class8;
    }(Op))(), new (function (_Op9) {
      _inherits(_class9, _Op9);

      function _class9() {
        var _this10, _ret9;

        _classCallCheck(this, _class9);

        return _ret9 = (_this10 = _possibleConstructorReturn(this, (_class9.__proto__ || Object.getPrototypeOf(_class9)).call(this, _PseudoASM.NotEq)), _this10), _possibleConstructorReturn(_this10, _ret9);
      }

      _createClass(_class9, [{
        key: "Apply",
        value: function (a, b) {
          return checkIntFirst(function () {
            var f = function f(x) {
              return function (y) {
                return x !== y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }(), function () {
            var f_1 = function f_1(x_1) {
              return function (y_1) {
                return x_1 !== y_1;
              };
            };

            return function (a_2) {
              return function (b_2) {
                return k(f_1, a_2, b_2);
              };
            };
          }(), a, b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class9;
    }(Op))(), new (function (_Op10) {
      _inherits(_class10, _Op10);

      function _class10() {
        var _this11, _ret10;

        _classCallCheck(this, _class10);

        return _ret10 = (_this11 = _possibleConstructorReturn(this, (_class10.__proto__ || Object.getPrototypeOf(_class10)).call(this, _PseudoASM.Greater)), _this11), _possibleConstructorReturn(_this11, _ret10);
      }

      _createClass(_class10, [{
        key: "Apply",
        value: function (a, b) {
          return checkIntFirst(function () {
            var f = function f(x) {
              return function (y) {
                return x > y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }(), function () {
            var f_1 = function f_1(x_1) {
              return function (y_1) {
                return x_1 > y_1;
              };
            };

            return function (a_2) {
              return function (b_2) {
                return k(f_1, a_2, b_2);
              };
            };
          }(), a, b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class10;
    }(Op))(), new (function (_Op11) {
      _inherits(_class11, _Op11);

      function _class11() {
        var _this12, _ret11;

        _classCallCheck(this, _class11);

        return _ret11 = (_this12 = _possibleConstructorReturn(this, (_class11.__proto__ || Object.getPrototypeOf(_class11)).call(this, _PseudoASM.GEq)), _this12), _possibleConstructorReturn(_this12, _ret11);
      }

      _createClass(_class11, [{
        key: "Apply",
        value: function (a, b) {
          return checkIntFirst(function () {
            var f = function f(x) {
              return function (y) {
                return x >= y;
              };
            };

            return function (a_1) {
              return function (b_1) {
                return k(f, a_1, b_1);
              };
            };
          }(), function () {
            var f_1 = function f_1(x_1) {
              return function (y_1) {
                return x_1 >= y_1;
              };
            };

            return function (a_2) {
              return function (b_2) {
                return k(f_1, a_2, b_2);
              };
            };
          }(), a, b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class11;
    }(Op))(), new (function (_Op12) {
      _inherits(_class12, _Op12);

      function _class12() {
        var _this13, _ret12;

        _classCallCheck(this, _class12);

        return _ret12 = (_this13 = _possibleConstructorReturn(this, (_class12.__proto__ || Object.getPrototypeOf(_class12)).call(this, _PseudoASM.And)), _this13), _possibleConstructorReturn(_this13, _ret12);
      }

      _createClass(_class12, [{
        key: "Apply",
        value: function (a, b) {
          return castToBoolThen(function (a_1) {
            return function (b_1) {
              return k(function (e1) {
                return function (e2) {
                  return e1 && e2;
                };
              }, a_1, b_1);
            };
          })(a)(b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class12;
    }(Op))(), new (function (_Op13) {
      _inherits(_class13, _Op13);

      function _class13() {
        var _this14, _ret13;

        _classCallCheck(this, _class13);

        return _ret13 = (_this14 = _possibleConstructorReturn(this, (_class13.__proto__ || Object.getPrototypeOf(_class13)).call(this, _PseudoASM.Or)), _this14), _possibleConstructorReturn(_this14, _ret13);
      }

      _createClass(_class13, [{
        key: "Apply",
        value: function (a, b) {
          return castToBoolThen(function (a_1) {
            return function (b_1) {
              return k(function (e1) {
                return function (e2) {
                  return e1 || e2;
                };
              }, a_1, b_1);
            };
          })(a)(b);
        }
      }, {
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            interfaces: ["CompileAndRun.Interpreter.Op"]
          };
        }
      }]);

      return _class13;
    }(Op))()]);

    var interpretPAsm = __exports.interpretPAsm = function (act, getInput, sendOutput, cmds) {
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
        var $var303 = _arg1.Case === "Store" ? [0, _arg1.Fields[0]] : _arg1.Case === "Load" ? [0, _arg1.Fields[0]] : [1];

        switch ($var303[0]) {
          case 0:
            return $var303[1];

          case 1:
            return null;
        }
      }, cmds))), new _GenericComparer2.default(_Util.compare)));
      (0, _String.fsFormat)("%A")(function (x) {
        console.log(x);
      })(x);
      stacks = new Map(Array.from((0, _Seq.zip)(x, Array.from((0, _Seq.initialize)(x.length, function (_arg1_1) {
        return {
          contents: (0, _List.ofArray)(["0"])
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
      pop(patternInput[0]);
      pop(patternInput[1]);
      pop(patternInput[4]);
      var heap = [];

      var interpretCmd = function interpretCmd(i) {
        var fwd = function fwd($var304) {
          return String(i - 1 + $var304);
        };

        var fwd2 = function fwd2($var305) {
          return function (value_1) {
            return String(value_1);
          }(function (y) {
            return i + y;
          }($var305));
        };

        return function (_arg2) {
          if (_arg2.Case === "PushFwdShift") {
            push(patternInput[1])(fwd2(_arg2.Fields[0]));
          } else if (_arg2.Case === "Pop") {
            pop(patternInput[1]);
          } else if (_arg2.Case === "Store") {
            pop(_arg2.Fields[0]);
            push(_arg2.Fields[0])(top(patternInput[1]));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Load") {
            push(patternInput[1])(top(_arg2.Fields[0]));
          } else if (_arg2.Case === "GotoFwdShift") {
            pop(patternInput[0]);
            push(patternInput[0])(fwd(_arg2.Fields[0]));
          } else if (_arg2.Case === "GotoIfTrueFwdShift") {
            if (top(patternInput[1]).toLocaleLowerCase() === "true" ? true : top(patternInput[1]) === "1") {
              pop(patternInput[0]);
              push(patternInput[0])(fwd(_arg2.Fields[0]));
            }

            pop(patternInput[1]);
          } else if (_arg2.Case === "Call") {
            var newInstr = String(Number.parseInt(top(patternInput[1])) - 1);
            pop(patternInput[1]);
            push(patternInput[1])(String(Number.parseInt(top(patternInput[0])) + 1));
            pop(patternInput[0]);
            push(patternInput[0])(newInstr);
          } else if (_arg2.Case === "Return") {
            pop(patternInput[0]);
            push(patternInput[0])(String(Number.parseInt(top(patternInput[1])) - 1));
            pop(patternInput[1]);
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
            push(patternInput[1])(getInput(_arg2.Fields[0]));
          } else if (_arg2.Case === "Output") {
            sendOutput(top(patternInput[1]));
            push(patternInput[4])(top(patternInput[1]));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Combinator_2") {
            var a = top(patternInput[1]);
            pop(patternInput[1]);
            var b = top(patternInput[1]);
            pop(patternInput[1]);
            var op = (0, _Seq.find)(function (e) {
              return _arg2.CommandInfo.Symbol === e.Symbol;
            }, allCombinators);
            push(patternInput[1])(function (arg00) {
              return function (arg10) {
                return op.Apply(arg00, arg10);
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
          throw new Error("failed");
        }

        act(stacks)(heap)(cmds[i_2]);
        var pt = Number.parseInt(top(patternInput[0]));
        pop(patternInput[0]);
        push(patternInput[0])(String(pt + 1));
      }
    };

    return __exports;
  }({});

  function compileAndRun(act, getInput, sendOutput, txt) {
    (function (cmds) {
      Interpreter.interpretPAsm(act, getInput, sendOutput, cmds);
    })(Array.from(function (tupledArg) {
      return (0, _PseudoASM2.fromAST)(tupledArg[0], tupledArg[1]);
    }(function (tupledArg_1) {
      return (0, _AST.fromToken)(tupledArg_1[0], tupledArg_1[1]);
    }((0, _Parser.fromString)(txt, (0, _Map.create)((0, _List.ofArray)([["language", "C"]]), new _GenericComparer2.default(_Util.compare)))))[0]));
  }
});