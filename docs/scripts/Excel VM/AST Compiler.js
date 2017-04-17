define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/List", "fable-core/umd/String", "./Token", "fable-core/umd/Map", "fable-core/umd/Seq", "fable-core/umd/GenericComparer"], function (exports, _Symbol2, _Util, _List, _String, _Token, _Map, _Seq, _GenericComparer) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.ASTCompile$27$ = exports.nxt = exports.nxt$27$ = exports.AST = undefined;
  exports.ASTCompile = ASTCompile;

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
          type: "AST_Compiler.AST",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            Apply: [AST, (0, _Util.makeGeneric)(_List2.default, {
              T: AST
            })],
            Assign: [AST, AST, AST],
            Const: ["string"],
            Declare: ["string", AST],
            Define: ["string", (0, _Util.makeGeneric)(_List2.default, {
              T: "string"
            }), AST],
            Get: [AST, AST],
            If: [AST, AST, AST],
            Loop: [AST, AST],
            Mutate: ["string", AST],
            New: [AST],
            Return: [(0, _Util.Option)(AST)],
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
        var str = function str(indent) {
          return function (_arg1) {
            var $var250 = _arg1.Case === "Declare" ? [1, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "Define" ? [2, _arg1.Fields[0], _arg1.Fields[1], _arg1.Fields[2]] : _arg1.Case === "Value" ? [3, _arg1.Fields[0]] : _arg1.Case === "Const" ? [3, _arg1.Fields[0]] : _arg1.Case === "Apply" ? [4, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "If" ? [5, _arg1.Fields[0], _arg1.Fields[1], _arg1.Fields[2]] : _arg1.Case === "New" ? [6, _arg1.Fields[0]] : _arg1.Case === "Get" ? [7, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "Assign" ? [8, _arg1.Fields[0], _arg1.Fields[2], _arg1.Fields[1]] : _arg1.Case === "Return" ? _arg1.Fields[0] != null ? [10, _arg1.Fields[0]] : [9] : _arg1.Case === "Loop" ? [11, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "Mutate" ? [12, _arg1.Fields[0], _arg1.Fields[1]] : [0, _arg1.Fields[0]];

            switch ($var250[0]) {
              case 0:
                return (0, _String.join)("\n", (0, _List.map)(str(indent), $var250[1]));

              case 1:
                return indent + (0, _String.fsFormat)("let %s =\n%s")(function (x) {
                  return x;
                })($var250[1])(str(indent + "  ")($var250[2]));

              case 2:
                return indent + (0, _String.fsFormat)("define %s(%s):\n%s")(function (x) {
                  return x;
                })($var250[1])((0, _String.join)(", ", $var250[2]))(str(indent + "  ")($var250[3]));

              case 3:
                return indent + $var250[1];

              case 4:
                return indent + (0, _String.fsFormat)("%s(%s)")(function (x) {
                  return x;
                })(str("")($var250[1]))((0, _String.join)(", ", (0, _List.map)(str(""), $var250[2])));

              case 5:
                return indent + (0, _String.fsFormat)("if %s then\n%s\n")(function (x) {
                  return x;
                })(str("")($var250[1]))(str(indent + "  ")($var250[2])) + indent + "else\n" + str(indent + "  ")($var250[3]);

              case 6:
                return indent + (0, _String.fsFormat)("alloc %s")(function (x) {
                  return x;
                })(str("")($var250[1]));

              case 7:
                return indent + (0, _String.fsFormat)("%s[%s]")(function (x) {
                  return x;
                })(str("")($var250[1]))(str("")($var250[2]));

              case 8:
                return indent + (0, _String.fsFormat)("%s[%s] <- %s")(function (x) {
                  return x;
                })(str("")($var250[1]))(str("")($var250[3]))(str("")($var250[2]));

              case 9:
                return indent + "return";

              case 10:
                return indent + (0, _String.fsFormat)("return\n%s")(function (x) {
                  return x;
                })(str(indent + "  ")($var250[1]));

              case 11:
                return indent + (0, _String.fsFormat)("while %s do\n%s\n")(function (x) {
                  return x;
                })(str("")($var250[1]))(str(indent + "  ")($var250[2]));

              case 12:
                return indent + (0, _String.fsFormat)("%s <-\n%s")(function (x) {
                  return x;
                })($var250[1])(str(indent + "  ")($var250[2]));
            }
          };
        };

        return str("")(this);
      }
    }]);

    return AST;
  }();

  (0, _Symbol2.setType)("AST_Compiler.AST", AST);

  function nxt_(x) {
    void x.contents++;
    return (0, _Util.toString)(x.contents);
  }

  exports.nxt$27$ = nxt_;

  var nxt = exports.nxt = function () {
    var x = {
      contents: 0
    };
    return function () {
      return nxt_(x, null);
    };
  }();

  function ASTCompile_(_arg1_0, _arg1_1) {
    var _arg1 = [_arg1_0, _arg1_1];
    return function (_arg2) {
      var $var251 = void 0;

      var activePatternResult804 = _Token.Token["|X|"](_arg2);

      if (activePatternResult804[0] === "return") {
        $var251 = [0, activePatternResult804[1]];
      } else if (activePatternResult804[0] === ",") {
        var activePatternResult805 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult805.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult805.Fields[0]];
        } else if (activePatternResult805.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult805.Fields[0]];
        } else {
          $var251 = [3, activePatternResult804[1]];
        }
      } else if (activePatternResult804[0] === "apply") {
        var activePatternResult806 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult806.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult806.Fields[0]];
        } else if (activePatternResult806.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult806.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [4, activePatternResult804[1].head, activePatternResult804[1].tail.head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "fun") {
        var activePatternResult807 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult807.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult807.Fields[0]];
        } else if (activePatternResult807.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult807.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [5, activePatternResult804[1].tail.head, activePatternResult804[1].head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "declare") {
        var activePatternResult808 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult808.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult808.Fields[0]];
        } else if (activePatternResult808.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult808.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [6, activePatternResult804[1].tail.head, activePatternResult804[1].head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "let") {
        var activePatternResult809 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult809.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult809.Fields[0]];
        } else if (activePatternResult809.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult809.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [7, activePatternResult804[1].head, activePatternResult804[1].tail.head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "let rec") {
        var activePatternResult810 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult810.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult810.Fields[0]];
        } else if (activePatternResult810.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult810.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [8, activePatternResult804[1].head, activePatternResult804[1].tail.head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "array") {
        var activePatternResult811 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult811.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult811.Fields[0]];
        } else if (activePatternResult811.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult811.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail == null) {
            $var251 = [9, activePatternResult804[1].head];
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "dot") {
        var activePatternResult812 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult812.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult812.Fields[0]];
        } else if (activePatternResult812.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult812.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [10, activePatternResult804[1].head, activePatternResult804[1].tail.head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "assign") {
        var activePatternResult813 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult813.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult813.Fields[0]];
        } else if (activePatternResult813.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult813.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [11, activePatternResult804[1].head, activePatternResult804[1].tail.head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "if") {
        var activePatternResult814 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult814.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult814.Fields[0]];
        } else if (activePatternResult814.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult814.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail != null) {
              if (activePatternResult804[1].tail.tail.tail.tail == null) {
                $var251 = [12, activePatternResult804[1].tail.head, activePatternResult804[1].head, activePatternResult804[1].tail.tail.head];
              } else {
                $var251 = [18, _arg2];
              }
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "do") {
        var activePatternResult815 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult815.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult815.Fields[0]];
        } else if (activePatternResult815.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult815.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail == null) {
            $var251 = [13, activePatternResult804[1].head];
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "while") {
        var activePatternResult816 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult816.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult816.Fields[0]];
        } else if (activePatternResult816.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult816.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail == null) {
              $var251 = [14, activePatternResult804[1].tail.head, activePatternResult804[1].head];
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "for") {
        var activePatternResult817 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult817.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult817.Fields[0]];
        } else if (activePatternResult817.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult817.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail != null) {
            if (activePatternResult804[1].tail.tail.tail != null) {
              if (activePatternResult804[1].tail.tail.tail.tail == null) {
                $var251 = [15, activePatternResult804[1].tail.tail.head, activePatternResult804[1].tail.head, activePatternResult804[1].head];
              } else {
                $var251 = [18, _arg2];
              }
            } else {
              $var251 = [18, _arg2];
            }
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "deref") {
        var activePatternResult818 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult818.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult818.Fields[0]];
        } else if (activePatternResult818.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult818.Fields[0]];
        } else if (activePatternResult804[1].tail != null) {
          if (activePatternResult804[1].tail.tail == null) {
            $var251 = [16, activePatternResult804[1].head];
          } else {
            $var251 = [18, _arg2];
          }
        } else {
          $var251 = [18, _arg2];
        }
      } else if (activePatternResult804[0] === "sequence") {
        var activePatternResult819 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult819.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult819.Fields[0]];
        } else if (activePatternResult819.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult819.Fields[0]];
        } else {
          $var251 = [17, activePatternResult804[1]];
        }
      } else {
        var activePatternResult820 = _Token.Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult820.Case === "Choice1Of3") {
          $var251 = [1, activePatternResult820.Fields[0]];
        } else if (activePatternResult820.Case === "Choice2Of3") {
          $var251 = [2, activePatternResult820.Fields[0]];
        } else {
          $var251 = [18, _arg2];
        }
      }

      switch ($var251[0]) {
        case 0:
          if ($var251[1].tail != null) {
            if ($var251[1].tail.tail == null) {
              return new AST("Return", [function (tupledArg) {
                return ASTCompile_(tupledArg[0], tupledArg[1]);
              }(_arg1)($var251[1].head)]);
            } else {
              throw new Error("cannot return more than one item");
            }
          } else {
            return new AST("Return", [null]);
          }

        case 1:
          if (_arg1[1].has($var251[1]) ? !_arg1[1].get($var251[1]).Equals(new _List2.default()) : false) {
            return new AST("Apply", [new AST("Value", [$var251[1]]), _arg1[1].get($var251[1])]);
          } else {
            return new AST("Value", [$var251[1]]);
          }

        case 2:
          return new AST("Const", [$var251[1]]);

        case 3:
          var name = "$tuple" + nxt(null);
          var allocate = (0, _List.ofArray)([new AST("Declare", [name, new AST("New", [new AST("Const", [String($var251[1].length)])])])]);
          var assignAll = (0, _List.mapIndexed)(function (i, e) {
            return new AST("Assign", [new AST("Value", [name]), new AST("Const", [String(i)]), function (tupledArg_1) {
              return ASTCompile_(tupledArg_1[0], tupledArg_1[1]);
            }(_arg1)(e)]);
          }, $var251[1]);
          var returnVal = (0, _List.ofArray)([new AST("Value", [name])]);
          return new AST("Sequence", [(0, _List.append)(allocate, (0, _List.append)(assignAll, returnVal))]);

        case 4:
          return new AST("Apply", [function (tupledArg_2) {
            return ASTCompile_(tupledArg_2[0], tupledArg_2[1]);
          }(_arg1)($var251[1]), (0, _List.ofArray)([function (tupledArg_3) {
            return ASTCompile_(tupledArg_3[0], tupledArg_3[1]);
          }(_arg1)($var251[2])])]);

        case 5:
          var unpack = function unpack(arg) {
            return function (_arg3) {
              var $var252 = void 0;

              var activePatternResult770 = _Token.Token["|X|"](_arg3);

              if (activePatternResult770[0] === "declare") {
                if (activePatternResult770[1].tail != null) {
                  if (activePatternResult770[1].tail.tail != null) {
                    var activePatternResult771 = _Token.Token["|T|_|"](activePatternResult770[1].tail.head);

                    if (activePatternResult771 != null) {
                      if (activePatternResult770[1].tail.tail.tail == null) {
                        $var252 = [0, activePatternResult771];
                      } else {
                        var activePatternResult772 = _Token.Token["|T|_|"](_arg3);

                        if (activePatternResult772 != null) {
                          $var252 = [0, activePatternResult772];
                        } else {
                          $var252 = [1];
                        }
                      }
                    } else {
                      var activePatternResult773 = _Token.Token["|T|_|"](_arg3);

                      if (activePatternResult773 != null) {
                        $var252 = [0, activePatternResult773];
                      } else {
                        $var252 = [1];
                      }
                    }
                  } else {
                    var activePatternResult774 = _Token.Token["|T|_|"](_arg3);

                    if (activePatternResult774 != null) {
                      $var252 = [0, activePatternResult774];
                    } else {
                      $var252 = [1];
                    }
                  }
                } else {
                  var activePatternResult775 = _Token.Token["|T|_|"](_arg3);

                  if (activePatternResult775 != null) {
                    $var252 = [0, activePatternResult775];
                  } else {
                    $var252 = [1];
                  }
                }
              } else {
                var activePatternResult776 = _Token.Token["|T|_|"](_arg3);

                if (activePatternResult776 != null) {
                  $var252 = [0, activePatternResult776];
                } else {
                  $var252 = [1];
                }
              }

              switch ($var252[0]) {
                case 0:
                  return [(0, _List.ofArray)([new AST("Declare", [$var252[1], arg])]), (0, _List.ofArray)([$var252[1]])];

                case 1:
                  var activePatternResult769 = _Token.Token["|X|"](_arg3);

                  if (activePatternResult769[0] === ",") {
                    var patternInput = (0, _List.unzip)((0, _List.mapIndexed)(function ($var253, $var254) {
                      return function (i_1) {
                        return unpack(new AST("Get", [arg, new AST("Const", [String(i_1)])]));
                      }($var253)($var254);
                    }, activePatternResult769[1]));
                    return [(0, _List.concat)(patternInput[0]), (0, _List.concat)(patternInput[1])];
                  } else {
                    return (0, _String.fsFormat)("could not unpack %A")(function (x) {
                      throw new Error(x);
                    })(_arg3);
                  }

              }
            };
          };

          var argName = "$arg" + nxt(null);
          var patternInput_1 = unpack(new AST("Value", [argName]))($var251[2]);
          var cptr_d = (0, _List.map)(function (arg0) {
            return new AST("Value", [arg0]);
          }, _arg1[0]);
          var cpt_ = [(0, _List.append)(patternInput_1[1], _arg1[0]), (0, _Map.add)("L", cptr_d, _arg1[1])];
          var functionBody = new AST("Sequence", [(0, _List.append)(patternInput_1[0], (0, _List.ofArray)([function (tupledArg_4) {
            return ASTCompile_(tupledArg_4[0], tupledArg_4[1]);
          }(cpt_)($var251[1])]))]);
          return new AST("Sequence", [(0, _Seq.toList)((0, _Seq.delay)(function () {
            return (0, _Seq.append)((0, _Seq.singleton)(new AST("Define", ["L", new _List2.default(argName, _arg1[0]), functionBody])), (0, _Seq.delay)(function () {
              var matchValue = function (tupledArg_5) {
                return ASTCompile_(tupledArg_5[0], tupledArg_5[1]);
              }(cpt_)(_Token.Token.Token[".ctor_2"]("L", new _List2.default()));

              if (matchValue.Case === "Apply") {
                return (0, _Seq.append)((0, _Seq.singleton)(new AST("Declare", ["K", new AST("New", [new AST("Const", [String(matchValue.Fields[1].length + 1)])])])), (0, _Seq.delay)(function () {
                  return (0, _Seq.append)((0, _Seq.singleton)(new AST("Assign", [new AST("Value", ["K"]), new AST("Const", ["0"]), new AST("Get", [matchValue.Fields[0], new AST("Const", ["0"])])])), (0, _Seq.delay)(function () {
                    return (0, _Seq.append)((0, _List.mapIndexed)(function (i_2, e_1) {
                      return new AST("Assign", [new AST("Value", ["K"]), new AST("Const", [String(i_2 + 1)]), e_1]);
                    }, matchValue.Fields[1]), (0, _Seq.delay)(function () {
                      return (0, _Seq.singleton)(new AST("Value", ["K"]));
                    }));
                  }));
                }));
              } else {
                return (0, _Seq.singleton)(matchValue);
              }
            }));
          }))]);

        case 6:
          return function (tupledArg_6) {
            return ASTCompile_(tupledArg_6[0], tupledArg_6[1]);
          }(_arg1)($var251[1]);

        case 7:
          var $var255 = void 0;

          var activePatternResult788 = _Token.Token["|X|"]($var251[1]);

          if (activePatternResult788[0] === "apply") {
            if (activePatternResult788[1].tail != null) {
              if (activePatternResult788[1].tail.tail != null) {
                if (activePatternResult788[1].tail.tail.tail == null) {
                  $var255 = [0, activePatternResult788[1].head, activePatternResult788[1].tail.head];
                } else {
                  var activePatternResult789 = _Token.Token["|T|_|"]($var251[1]);

                  if (activePatternResult789 != null) {
                    $var255 = [1, activePatternResult789];
                  } else {
                    $var255 = [2];
                  }
                }
              } else {
                var activePatternResult790 = _Token.Token["|T|_|"]($var251[1]);

                if (activePatternResult790 != null) {
                  $var255 = [1, activePatternResult790];
                } else {
                  $var255 = [2];
                }
              }
            } else {
              var activePatternResult791 = _Token.Token["|T|_|"]($var251[1]);

              if (activePatternResult791 != null) {
                $var255 = [1, activePatternResult791];
              } else {
                $var255 = [2];
              }
            }
          } else if (activePatternResult788[0] === "declare") {
            var activePatternResult792 = _Token.Token["|T|_|"]($var251[1]);

            if (activePatternResult792 != null) {
              $var255 = [1, activePatternResult792];
            } else if (activePatternResult788[1].tail != null) {
              if (activePatternResult788[1].tail.tail != null) {
                var activePatternResult793 = _Token.Token["|T|_|"](activePatternResult788[1].tail.head);

                if (activePatternResult793 != null) {
                  if (activePatternResult788[1].tail.tail.tail == null) {
                    $var255 = [1, activePatternResult793];
                  } else {
                    $var255 = [2];
                  }
                } else {
                  $var255 = [2];
                }
              } else {
                $var255 = [2];
              }
            } else {
              $var255 = [2];
            }
          } else {
            var activePatternResult794 = _Token.Token["|T|_|"]($var251[1]);

            if (activePatternResult794 != null) {
              $var255 = [1, activePatternResult794];
            } else {
              $var255 = [2];
            }
          }

          switch ($var255[0]) {
            case 0:
              return function (tupledArg_7) {
                return ASTCompile_(tupledArg_7[0], tupledArg_7[1]);
              }(_arg1)(_Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([$var255[1], _Token.Token.Token[".ctor_2"]("fun", (0, _List.ofArray)([$var255[2], $var251[2]]))])));

            case 1:
              return new AST("Declare", [$var255[1], function (tupledArg_8) {
                return ASTCompile_(tupledArg_8[0], tupledArg_8[1]);
              }(_arg1)($var251[2])]);

            case 2:
              return (0, _String.fsFormat)("patterns in function arguments not supported yet: %A")(function (x) {
                throw new Error(x);
              })($var251[1]);
          }

        case 8:
          return function (tupledArg_9) {
            return ASTCompile_(tupledArg_9[0], tupledArg_9[1]);
          }(_arg1)(_Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([$var251[1], $var251[2]])));

        case 9:
          return new AST("New", [function (tupledArg_10) {
            return ASTCompile_(tupledArg_10[0], tupledArg_10[1]);
          }(_arg1)($var251[1])]);

        case 10:
          var $var256 = void 0;

          var activePatternResult795 = _Token.Token["|X|"]($var251[2]);

          if (activePatternResult795[0] === "[]") {
            if (activePatternResult795[1].tail != null) {
              if (activePatternResult795[1].tail.tail == null) {
                $var256 = [0, activePatternResult795[1].head];
              } else {
                $var256 = [1];
              }
            } else {
              $var256 = [1];
            }
          } else {
            $var256 = [1];
          }

          switch ($var256[0]) {
            case 0:
              return new AST("Get", [function (tupledArg_11) {
                return ASTCompile_(tupledArg_11[0], tupledArg_11[1]);
              }(_arg1)($var251[1]), function (tupledArg_12) {
                return ASTCompile_(tupledArg_12[0], tupledArg_12[1]);
              }(_arg1)($var256[1])]);

            case 1:
              throw new Error("should never happen");
          }

        case 11:
          var $var257 = void 0;

          var activePatternResult796 = _Token.Token["|X|"]($var251[1]);

          if (activePatternResult796[1].tail != null) {
            if (activePatternResult796[1].tail.tail == null) {
              if (activePatternResult796[0] === "deref") {
                $var257 = [2];
              } else {
                $var257 = [3];
              }
            } else {
              var activePatternResult797 = _Token.Token["|X|"](activePatternResult796[1].tail.head);

              if (activePatternResult797[0] === "[]") {
                if (activePatternResult797[1].tail != null) {
                  if (activePatternResult797[1].tail.tail == null) {
                    if (activePatternResult796[1].tail.tail.tail == null) {
                      if (activePatternResult796[0] === "dot") {
                        $var257 = [1, activePatternResult796[1].head, activePatternResult797[1].head];
                      } else {
                        $var257 = [3];
                      }
                    } else {
                      $var257 = [3];
                    }
                  } else {
                    $var257 = [3];
                  }
                } else {
                  $var257 = [3];
                }
              } else {
                $var257 = [3];
              }
            }
          } else {
            $var257 = [0, activePatternResult796[0]];
          }

          switch ($var257[0]) {
            case 0:
              return new AST("Mutate", [$var257[1], function (tupledArg_13) {
                return ASTCompile_(tupledArg_13[0], tupledArg_13[1]);
              }(_arg1)($var251[2])]);

            case 1:
              return new AST("Assign", [function (tupledArg_14) {
                return ASTCompile_(tupledArg_14[0], tupledArg_14[1]);
              }(_arg1)($var257[1]), function (tupledArg_15) {
                return ASTCompile_(tupledArg_15[0], tupledArg_15[1]);
              }(_arg1)($var257[2]), function (tupledArg_16) {
                return ASTCompile_(tupledArg_16[0], tupledArg_16[1]);
              }(_arg1)($var251[2])]);

            case 2:
              return new AST("Assign", [function (tupledArg_17) {
                return ASTCompile_(tupledArg_17[0], tupledArg_17[1]);
              }(_arg1)($var251[1]), new AST("Const", ["0"]), function (tupledArg_18) {
                return ASTCompile_(tupledArg_18[0], tupledArg_18[1]);
              }(_arg1)($var251[2])]);

            case 3:
              throw new Error("todo: unpacking");
          }

        case 12:
          return new AST("If", [function (tupledArg_19) {
            return ASTCompile_(tupledArg_19[0], tupledArg_19[1]);
          }(_arg1)($var251[2]), function (tupledArg_20) {
            return ASTCompile_(tupledArg_20[0], tupledArg_20[1]);
          }(_arg1)($var251[1]), function (tupledArg_21) {
            return ASTCompile_(tupledArg_21[0], tupledArg_21[1]);
          }(_arg1)($var251[3])]);

        case 13:
          return new AST("Apply", [new AST("Value", ["ignore"]), (0, _List.ofArray)([function (tupledArg_22) {
            return ASTCompile_(tupledArg_22[0], tupledArg_22[1]);
          }(_arg1)($var251[1])])]);

        case 14:
          return new AST("Loop", [function (tupledArg_23) {
            return ASTCompile_(tupledArg_23[0], tupledArg_23[1]);
          }(_arg1)($var251[2]), function (tupledArg_24) {
            return ASTCompile_(tupledArg_24[0], tupledArg_24[1]);
          }(_arg1)($var251[1])]);

        case 15:
          var name_1 = void 0;

          var activePatternResult798 = _Token.Token["|X|"]($var251[3]);

          if (activePatternResult798[1].tail == null) {
            name_1 = activePatternResult798[0];
          } else {
            throw new Error("todo: unpacking");
          }

          var $var258 = void 0;

          var activePatternResult799 = _Token.Token["|X|"]($var251[2]);

          if (activePatternResult799[0] === "..") {
            if (activePatternResult799[1].tail != null) {
              if (activePatternResult799[1].tail.tail != null) {
                if (activePatternResult799[1].tail.tail.tail != null) {
                  if (activePatternResult799[1].tail.tail.tail.tail == null) {
                    $var258 = [0, activePatternResult799[1].head, activePatternResult799[1].tail.tail.head, activePatternResult799[1].tail.head];
                  } else {
                    $var258 = [1];
                  }
                } else {
                  $var258 = [1];
                }
              } else {
                $var258 = [1];
              }
            } else {
              $var258 = [1];
            }
          } else {
            $var258 = [1];
          }

          switch ($var258[0]) {
            case 0:
              return new AST("Sequence", [(0, _List.ofArray)([new AST("Declare", [name_1, function (tupledArg_25) {
                return ASTCompile_(tupledArg_25[0], tupledArg_25[1]);
              }(_arg1)($var258[1])]), new AST("Loop", [new AST("Apply", [new AST("Apply", [new AST("Value", ["<="]), (0, _List.ofArray)([new AST("Value", [name_1])])]), (0, _List.ofArray)([function (tupledArg_26) {
                return ASTCompile_(tupledArg_26[0], tupledArg_26[1]);
              }(_arg1)($var258[2])])]), new AST("Sequence", [(0, _List.ofArray)([function (tupledArg_27) {
                return ASTCompile_(tupledArg_27[0], tupledArg_27[1]);
              }(_arg1)($var251[1]), new AST("Mutate", [name_1, new AST("Apply", [new AST("Apply", [new AST("Value", ["+"]), (0, _List.ofArray)([new AST("Value", [name_1])])]), (0, _List.ofArray)([function (tupledArg_28) {
                return ASTCompile_(tupledArg_28[0], tupledArg_28[1]);
              }(_arg1)($var258[3])])])])])])])])]);

            case 1:
              throw new Error("iterable objects not supported yet");
          }

        case 16:
          return new AST("Get", [function (tupledArg_29) {
            return ASTCompile_(tupledArg_29[0], tupledArg_29[1]);
          }(_arg1)($var251[1]), new AST("Const", ["0"])]);

        case 17:
          return new AST("Sequence", [(0, _List.reverse)((0, _Seq.fold)(function (tupledArg_30, e_2) {
            var compiled = function (tupledArg_31) {
              return ASTCompile_(tupledArg_31[0], tupledArg_31[1]);
            }(tupledArg_30[1])(e_2);

            var cpt__1 = compiled.Case === "Declare" ? [new _List2.default(compiled.Fields[0], tupledArg_30[1][0]), tupledArg_30[1][1]] : compiled.Case === "Define" ? [new _List2.default(compiled.Fields[0], tupledArg_30[1][0]), (0, _Map.add)(compiled.Fields[0], (0, _List.map)(function (arg0_1) {
              return new AST("Value", [arg0_1]);
            }, tupledArg_30[1][0]), tupledArg_30[1][1])] : tupledArg_30[1];
            return [new _List2.default(compiled, tupledArg_30[0]), cpt__1];
          }, [new _List2.default(), _arg1], $var251[1])[0])]);

        case 18:
          return (0, _String.fsFormat)("unknown: %A")(function (x) {
            throw new Error(x);
          })($var251[1]);
      }
    };
  }

  exports.ASTCompile$27$ = ASTCompile_;

  function ASTCompile(e) {
    return ASTCompile_(new _List2.default(), (0, _Map.create)(null, new _GenericComparer2.default(_Util.compare)))(e);
  }
});