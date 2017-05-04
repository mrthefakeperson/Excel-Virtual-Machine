define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/List", "fable-core/umd/Seq", "fable-core/umd/String"], function (exports, _Symbol2, _Util, _List, _Seq, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.$7C$Children$7C$ = exports.AST = undefined;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  var _List2 = _interopRequireDefault(_List);

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

  var AST = exports.AST = function () {
    function AST(caseName, fields) {
      _classCallCheck(this, AST);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(AST, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "AST.Definition.AST",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            Apply: [AST, (0, _Util.makeGeneric)(_List2.default, {
              T: AST
            })],
            Assign: [AST, AST, AST],
            Break: [],
            Const: ["string"],
            Continue: [],
            Declare: ["string", AST],
            Define: ["string", (0, _Util.makeGeneric)(_List2.default, {
              T: "string"
            }), AST],
            Get: [AST, AST],
            If: [AST, AST, AST],
            Loop: [AST, AST],
            Mutate: ["string", AST],
            New: [AST],
            Return: [AST],
            Sequence: [(0, _Util.makeGeneric)(_List2.default, {
              T: AST
            })],
            Value: ["string"]
          }
        };
      }
    }, {
      key: "Equals",
      value: function (other) {
        return (0, _Util.equalsUnions)(this, other);
      }
    }, {
      key: "CompareTo",
      value: function (other) {
        return (0, _Util.compareUnions)(this, other);
      }
    }, {
      key: "ToString",
      value: function () {
        var _Children_ = AST["( |Children| )"];
        var isInline = new Map();

        var push = function push(ast) {
          return function (x) {
            isInline.set(ast, x);
            return x;
          };
        };

        var isInline_ = function isInline_(e) {
          return function (_arg2) {
            var $var271 = _arg2.Case === "Value" ? [0] : _arg2.Case === "Const" ? [0] : _arg2.Case === "Break" ? [0] : _arg2.Case === "Continue" ? [0] : _arg2.Case === "Sequence" ? [1] : [2];

            switch ($var271[0]) {
              case 0:
                return push(e)(true);

              case 1:
                (0, _List.map)(isInline_, _arg2.Fields[0]);
                return push(e)(false);

              case 2:
                var activePatternResult837 = _Children_(_arg2);

                return push(e)(function (list) {
                  return (0, _Seq.reduce)(function (e1, e2) {
                    return e1 && e2;
                  }, list);
                }((0, _List.map)(isInline_, activePatternResult837)));
            }
          }(e);
        };

        isInline_(this);

        var _Inline___ = function _Inline___(ast_1) {
          if (isInline.get(ast_1)) {
            return ast_1;
          } else {
            return null;
          }
        };

        var AorB = function AorB(ast_2) {
          return function (a) {
            return function (b) {
              if (isInline.get(ast_2)) {
                return a;
              } else {
                return b;
              }
            };
          };
        };

        var str = function str(indent) {
          return function (e_1) {
            var ind_ = indent + "  ";
            var a_1 = void 0;
            var $var272 = void 0;

            if (e_1.Case === "Sequence") {
              $var272 = [0, e_1.Fields[0]];
            } else if (e_1.Case === "Loop") {
              var activePatternResult851 = _Inline___(e_1.Fields[0]);

              if (activePatternResult851 != null) {
                var activePatternResult852 = _Inline___(e_1.Fields[1]);

                if (activePatternResult852 != null) {
                  $var272 = [1, activePatternResult851, activePatternResult852];
                } else {
                  $var272 = [2];
                }
              } else {
                $var272 = [2];
              }
            } else {
              $var272 = [2];
            }

            switch ($var272[0]) {
              case 0:
                a_1 = (0, _String.join)("\n", (0, _List.map)(str(indent), $var272[1])).slice(indent.length, (0, _String.join)("\n", (0, _List.map)(str(indent), $var272[1])).length);
                break;

              case 1:
                a_1 = (0, _String.fsFormat)("while %s do %s")(function (x) {
                  return x;
                })(str("")($var272[1]))(str("")($var272[2]));
                break;

              case 2:
                var $var273 = void 0;

                if (e_1.Case === "Loop") {
                  var activePatternResult850 = _Inline___(e_1.Fields[0]);

                  if (activePatternResult850 != null) {
                    $var273 = [0, activePatternResult850, e_1.Fields[1]];
                  } else {
                    $var273 = [1];
                  }
                } else {
                  $var273 = [1];
                }

                switch ($var273[0]) {
                  case 0:
                    a_1 = (0, _String.fsFormat)("while %s do\n%s")(function (x) {
                      return x;
                    })(str("")($var273[1]))(str(ind_)($var273[2]));
                    break;

                  case 1:
                    var $var274 = void 0;

                    if (e_1.Case === "Loop") {
                      var activePatternResult849 = _Inline___(e_1.Fields[1]);

                      if (activePatternResult849 != null) {
                        $var274 = [0, e_1.Fields[0], activePatternResult849];
                      } else {
                        $var274 = [1];
                      }
                    } else {
                      $var274 = [1];
                    }

                    switch ($var274[0]) {
                      case 0:
                        a_1 = (0, _String.fsFormat)("while\n%s\n do %s")(function (x) {
                          return x;
                        })(str(ind_)($var274[1]))(str("")($var274[2]));
                        break;

                      case 1:
                        var $var275 = void 0;

                        if (e_1.Case === "Loop") {
                          $var275 = [0, e_1.Fields[0], e_1.Fields[1]];
                        } else if (e_1.Case === "Value") {
                          $var275 = [1, e_1.Fields[0]];
                        } else if (e_1.Case === "Const") {
                          $var275 = [1, e_1.Fields[0]];
                        } else if (e_1.Case === "Apply") {
                          $var275 = [2, e_1.Fields[0], e_1.Fields[1]];
                        } else if (e_1.Case === "Declare") {
                          var activePatternResult848 = _Inline___(e_1.Fields[1]);

                          if (activePatternResult848 != null) {
                            $var275 = [3, e_1.Fields[0], activePatternResult848];
                          } else {
                            $var275 = [4];
                          }
                        } else {
                          $var275 = [4];
                        }

                        switch ($var275[0]) {
                          case 0:
                            a_1 = (0, _String.fsFormat)("while\n%s\n do %s")(function (x) {
                              return x;
                            })(str(ind_)($var275[1]))(str(ind_)($var275[2]));
                            break;

                          case 1:
                            a_1 = $var275[1];
                            break;

                          case 2:
                            a_1 = (0, _String.fsFormat)("%s%s")(function (x) {
                              return x;
                            })(AorB($var275[1])(str("")($var275[1]))((0, _String.fsFormat)("(\n%s\n%s )")(function (x) {
                              return x;
                            })(str(ind_)($var275[1]))(indent)))((0, _String.fsFormat)("(%s)")(function (x) {
                              return x;
                            })((0, _String.join)(", ", (0, _List.map)(function (e_2) {
                              return AorB(e_2)(str("")(e_2))((0, _String.fsFormat)("\n%s")(function (x) {
                                return x;
                              })(str(ind_)(e_2)));
                            }, $var275[2]))));
                            break;

                          case 3:
                            a_1 = (0, _String.fsFormat)("let %s = %s")(function (x) {
                              return x;
                            })($var275[1])(str("")($var275[2]));
                            break;

                          case 4:
                            var $var276 = void 0;

                            if (e_1.Case === "Declare") {
                              $var276 = [0, e_1.Fields[0], e_1.Fields[1]];
                            } else if (e_1.Case === "Define") {
                              var activePatternResult847 = _Inline___(e_1.Fields[2]);

                              if (activePatternResult847 != null) {
                                $var276 = [1, e_1.Fields[0], e_1.Fields[1], activePatternResult847];
                              } else {
                                $var276 = [2];
                              }
                            } else {
                              $var276 = [2];
                            }

                            switch ($var276[0]) {
                              case 0:
                                a_1 = (0, _String.fsFormat)("let %s =\n%s")(function (x) {
                                  return x;
                                })($var276[1])(str(ind_)($var276[2]));
                                break;

                              case 1:
                                a_1 = (0, _String.fsFormat)("define %s(%s): %s")(function (x) {
                                  return x;
                                })($var276[1])((0, _String.join)(", ", $var276[2]))(str("")($var276[3]));
                                break;

                              case 2:
                                var $var277 = void 0;

                                if (e_1.Case === "Define") {
                                  $var277 = [0, e_1.Fields[0], e_1.Fields[1], e_1.Fields[2]];
                                } else if (e_1.Case === "If") {
                                  $var277 = [1, e_1.Fields[0], e_1.Fields[1], e_1.Fields[2]];
                                } else if (e_1.Case === "New") {
                                  $var277 = [2, e_1.Fields[0]];
                                } else if (e_1.Case === "Get") {
                                  $var277 = [3, e_1.Fields[0], e_1.Fields[1]];
                                } else if (e_1.Case === "Assign") {
                                  $var277 = [4, e_1.Fields[0], e_1.Fields[1], e_1.Fields[2]];
                                } else if (e_1.Case === "Return") {
                                  $var277 = [5, e_1.Fields[0]];
                                } else if (e_1.Case === "Break") {
                                  $var277 = [6];
                                } else if (e_1.Case === "Continue") {
                                  $var277 = [7];
                                } else if (e_1.Case === "Mutate") {
                                  var activePatternResult846 = _Inline___(e_1.Fields[1]);

                                  if (activePatternResult846 != null) {
                                    $var277 = [8, e_1.Fields[0], activePatternResult846];
                                  } else {
                                    $var277 = [9];
                                  }
                                } else {
                                  $var277 = [9];
                                }

                                switch ($var277[0]) {
                                  case 0:
                                    a_1 = (0, _String.fsFormat)("define %s(%s):\n%s")(function (x) {
                                      return x;
                                    })($var277[1])((0, _String.join)(", ", $var277[2]))(str(ind_)($var277[3]));
                                    break;

                                  case 1:
                                    a_1 = (0, _String.fsFormat)("if%sthen%selse%s")(function (x) {
                                      return x;
                                    })(AorB($var277[1])((0, _String.fsFormat)(" %s ")(function (x) {
                                      return x;
                                    })(str("")($var277[1])))((0, _String.fsFormat)("\n%s\n%s ")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[1]))(indent)))(AorB($var277[2])((0, _String.fsFormat)(" %s ")(function (x) {
                                      return x;
                                    })(str("")($var277[2])))((0, _String.fsFormat)("\n%s\n%s ")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[2]))(indent)))(AorB($var277[3])((0, _String.fsFormat)(" %s")(function (x) {
                                      return x;
                                    })(str("")($var277[3])))((0, _String.fsFormat)("\n%s")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[3]))));
                                    break;

                                  case 2:
                                    a_1 = (0, _String.fsFormat)("alloc%s")(function (x) {
                                      return x;
                                    })(AorB($var277[1])((0, _String.fsFormat)(" %s")(function (x) {
                                      return x;
                                    })(str("")($var277[1])))((0, _String.fsFormat)("\n%s")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[1]))));
                                    break;

                                  case 3:
                                    a_1 = (0, _String.fsFormat)("%s%s")(function (x) {
                                      return x;
                                    })(AorB($var277[1])(str("")($var277[1]))((0, _String.fsFormat)("(\n%s\n%s )")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[1]))(indent)))(AorB($var277[2])((0, _String.fsFormat)("[%s]")(function (x) {
                                      return x;
                                    })(str("")($var277[2])))((0, _String.fsFormat)("[\n%s\n%s ]")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[2]))(indent)));
                                    break;

                                  case 4:
                                    a_1 = (0, _String.fsFormat)("%s%s <- %s")(function (x) {
                                      return x;
                                    })(AorB($var277[1])(str("")($var277[1]))((0, _String.fsFormat)("(\n%s\n%s )")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[1]))(indent)))(AorB($var277[2])((0, _String.fsFormat)("[%s]")(function (x) {
                                      return x;
                                    })(str("")($var277[2])))((0, _String.fsFormat)("[\n%s\n%s ]")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[2]))(indent)))(AorB($var277[3])(str("")($var277[3]))((0, _String.fsFormat)("\n%s")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[3]))));
                                    break;

                                  case 5:
                                    a_1 = (0, _String.fsFormat)("return%s")(function (x) {
                                      return x;
                                    })(AorB($var277[1])((0, _String.fsFormat)(" %s")(function (x) {
                                      return x;
                                    })(str("")($var277[1])))((0, _String.fsFormat)("\n%s")(function (x) {
                                      return x;
                                    })(str(ind_)($var277[1]))));
                                    break;

                                  case 6:
                                    a_1 = "Break";
                                    break;

                                  case 7:
                                    a_1 = "Continue";
                                    break;

                                  case 8:
                                    a_1 = (0, _String.fsFormat)("%s <- %s")(function (x) {
                                      return x;
                                    })($var277[1])(str("")($var277[2]));
                                    break;

                                  case 9:
                                    if (e_1.Case === "Mutate") {
                                      a_1 = (0, _String.fsFormat)("%s <-\n%s")(function (x) {
                                        return x;
                                      })(e_1.Fields[0])(str(ind_)(e_1.Fields[1]));
                                    } else {
                                      throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/AST.Definition.fs", 64, 16);
                                    }

                                    break;
                                }

                                break;
                            }

                            break;
                        }

                        break;
                    }

                    break;
                }

                break;
            }

            return indent + a_1;
          };
        };

        return str("")(this);
      }
    }], [{
      key: "map",
      value: function (f) {
        return function (_arg1) {
          return _arg1.Case === "Assign" ? new AST("Assign", [f(_arg1.Fields[0]), f(_arg1.Fields[1]), f(_arg1.Fields[2])]) : _arg1.Case === "Break" ? new AST("Break", []) : _arg1.Case === "Const" ? new AST("Const", [_arg1.Fields[0]]) : _arg1.Case === "Continue" ? new AST("Continue", []) : _arg1.Case === "Declare" ? new AST("Declare", [_arg1.Fields[0], f(_arg1.Fields[1])]) : _arg1.Case === "Define" ? new AST("Define", [_arg1.Fields[0], _arg1.Fields[1], f(_arg1.Fields[2])]) : _arg1.Case === "Get" ? new AST("Get", [f(_arg1.Fields[0]), f(_arg1.Fields[1])]) : _arg1.Case === "If" ? new AST("If", [f(_arg1.Fields[0]), f(_arg1.Fields[1]), f(_arg1.Fields[2])]) : _arg1.Case === "Loop" ? new AST("Loop", [f(_arg1.Fields[0]), f(_arg1.Fields[1])]) : _arg1.Case === "Mutate" ? new AST("Mutate", [_arg1.Fields[0], f(_arg1.Fields[1])]) : _arg1.Case === "New" ? new AST("New", [f(_arg1.Fields[0])]) : _arg1.Case === "Return" ? new AST("Return", [f(_arg1.Fields[0])]) : _arg1.Case === "Sequence" ? new AST("Sequence", [(0, _List.map)(f, _arg1.Fields[0])]) : _arg1.Case === "Value" ? new AST("Value", [_arg1.Fields[0]]) : new AST("Apply", [f(_arg1.Fields[0]), (0, _List.map)(f, _arg1.Fields[1])]);
        };
      }
    }, {
      key: "iter",
      value: function (f) {
        return function ($var270) {
          return function (value) {
            value;
          }(AST.map(function (e) {
            f(e);
            return e;
          })($var270));
        };
      }
    }, {
      key: "( |Children| )",
      get: function () {
        return function (e) {
          var yld = {
            contents: new _List2.default()
          };
          AST.iter(function (e_1) {
            yld.contents = new _List2.default(e_1, yld.contents);
          })(e);
          return (0, _List.reverse)(yld.contents);
        };
      }
    }]);

    return AST;
  }();

  (0, _Symbol2.setType)("AST.Definition.AST", AST);
  var _Children_ = AST["( |Children| )"];
  exports.$7C$Children$7C$ = _Children_;
});