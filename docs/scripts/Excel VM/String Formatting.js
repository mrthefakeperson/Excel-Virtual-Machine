define(["exports", "fable-core/umd/List", "./Token", "fable-core/umd/String"], function (exports, _List, _Token, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.processStringFormatting = exports.$7C$StringContents$7C$_$7C$ = undefined;
  exports.buildString = buildString;
  exports.escapeSequences = escapeSequences;
  exports.isFormatString = isFormatString;
  exports.separateFormatSymbols = separateFormatSymbols;
  exports.buildFormatFunction = buildFormatFunction;
  exports.mapFormatting = mapFormatting;
  exports.processScan = processScan;
  exports.processEscapeSequences = processEscapeSequences;

  var _List2 = _interopRequireDefault(_List);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

  var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
    return typeof obj;
  } : function (obj) {
    return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
  };

  function buildString(builder) {
    return Array.from((0, _List.reverse)(builder)).join('');
  }

  function escapeSequences(s) {
    var charList = (0, _List.ofArray)(s.split(""));

    var parseCharList = function parseCharList(builder) {
      return function (_arg1) {
        parseCharList: while (true) {
          var $var31 = _arg1.tail != null ? _arg1.head === "\\" ? _arg1.tail.tail != null ? [1, _arg1.tail.head, _arg1.tail.tail] : [2, _arg1.head, _arg1.tail] : [2, _arg1.head, _arg1.tail] : [0];

          switch ($var31[0]) {
            case 0:
              return buildString(builder);

            case 1:
              var k = void 0;
              var $var32 = $var31[1] === "\"" ? [0] : $var31[1] === "\\" ? [0] : $var31[1] === "n" ? [1] : $var31[1] === "t" ? [2] : [3];

              switch ($var32[0]) {
                case 0:
                  k = $var31[1];
                  break;

                case 1:
                  k = "\n";
                  break;

                case 2:
                  k = "\t";
                  break;

                case 3:
                  throw new Error("escape sequence not recognized");
                  break;
              }

              builder = new _List2.default(k, builder);
              _arg1 = $var31[2];
              continue parseCharList;

            case 2:
              builder = new _List2.default($var31[1], builder);
              _arg1 = $var31[2];
              continue parseCharList;
          }
        }
      };
    };

    return parseCharList(new _List2.default())(charList);
  }

  function isFormatString(s) {
    if (s[0] === "%") {
      return s.length > 1;
    } else {
      return false;
    }
  }

  function _StringContents___(s) {
    if ((s.length >= 2 ? s[0] === "\"" : false) ? s[s.length - 1] === "\"" : false) {
      return s.slice(1, s.length - 2 + 1);
    } else {
      return null;
    }
  }

  exports.$7C$StringContents$7C$_$7C$ = _StringContents___;

  function separateFormatSymbols(s) {
    var charList = (0, _List.ofArray)(s.split(""));

    var parseCharList = function parseCharList(builder) {
      return function (_arg1) {
        parseCharList: while (true) {
          if (_arg1.tail != null) {
            if (_arg1.head === "%") {
              if (_arg1.tail.tail == null) {
                throw new Error("incomplete format");
              } else {
                var formatStringBuilder = (0, _List.ofArray)([_arg1.tail.head, "%"]);
                builder = (0, _List.ofArray)([new _List2.default(), formatStringBuilder], builder);
                _arg1 = _arg1.tail.tail;
                continue parseCharList;
              }
            } else {
              builder = new _List2.default(new _List2.default(_arg1.head, builder.head), builder.tail);
              _arg1 = _arg1.tail;
              continue parseCharList;
            }
          } else {
            return (0, _List.reverse)((0, _List.map)(function (builder_1) {
              return buildString(builder_1);
            }, (0, _List.filter)(function () {
              var x = new _List2.default();
              return function (y) {
                return !x.Equals(y);
              };
            }(), builder)));
          }
        }
      };
    };

    return parseCharList((0, _List.ofArray)([new _List2.default()]))(charList);
  }

  function buildFormatFunction(makeFormat, nonFormat) {
    var buildFormatFunction_1 = function buildFormatFunction_1(arg) {
      return function (ret) {
        return function (_arg1) {
          buildFormatFunction_1: while (true) {
            var $var33 = _arg1.tail != null ? isFormatString(_arg1.head) ? [0, _arg1.head, _arg1.tail] : [1] : [1];

            switch ($var33[0]) {
              case 0:
                var argName = _Token.Token.Token[".ctor_3"]((0, _String.fsFormat)("_%i")(function (x) {
                  return x;
                })(arg));

                return _Token.Token.Token[".ctor_2"]("fun", (0, _List.ofArray)([argName, buildFormatFunction_1(arg + 1)(new _List2.default(makeFormat((0, _String.fsFormat)("\"%s\"")(function (x) {
                  return x;
                })($var33[1]))(argName), ret))($var33[2])]));

              case 1:
                if (_arg1.tail == null) {
                  return _Token.Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(ret));
                } else {
                  arg = arg;
                  ret = new _List2.default(nonFormat((0, _String.fsFormat)("\"%s\"")(function (x) {
                    return x;
                  })(_arg1.head)), ret);
                  _arg1 = _arg1.tail;
                  continue buildFormatFunction_1;
                }

            }
          }
        };
      };
    };

    return buildFormatFunction_1(1)(new _List2.default());
  }

  function mapFormatting(_arg1) {
    mapFormatting: while (true) {
      var $var34 = void 0;

      var activePatternResult234 = _Token.Token["|X|"](_arg1);

      if (activePatternResult234[0] === "apply") {
        if (activePatternResult234[1].tail != null) {
          var activePatternResult235 = _Token.Token["|T|_|"](activePatternResult234[1].head);

          if (activePatternResult235 != null) {
            if (activePatternResult235 === "printfn") {
              if (activePatternResult234[1].tail.tail != null) {
                var activePatternResult236 = _Token.Token["|T|_|"](activePatternResult234[1].tail.head);

                if (activePatternResult236 != null) {
                  var activePatternResult237 = _StringContents___(activePatternResult236);

                  if (activePatternResult237 != null) {
                    if (activePatternResult234[1].tail.tail.tail == null) {
                      $var34 = [0, activePatternResult237];
                    } else {
                      $var34 = [1];
                    }
                  } else {
                    $var34 = [1];
                  }
                } else {
                  $var34 = [1];
                }
              } else {
                $var34 = [1];
              }
            } else {
              $var34 = [1];
            }
          } else {
            $var34 = [1];
          }
        } else {
          $var34 = [1];
        }
      } else {
        $var34 = [1];
      }

      var _ret = function () {
        switch ($var34[0]) {
          case 0:
            _arg1 = _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("printf"), _Token.Token.Token[".ctor_3"]((0, _String.fsFormat)("\"%s\\n\"")(function (x) {
              return x;
            })($var34[1]))]));
            return "continue|mapFormatting";

          case 1:
            var $var35 = void 0;

            var activePatternResult228 = _Token.Token["|X|"](_arg1);

            if (activePatternResult228[0] === "apply") {
              if (activePatternResult228[1].tail != null) {
                var activePatternResult229 = _Token.Token["|T|_|"](activePatternResult228[1].head);

                if (activePatternResult229 != null) {
                  if (activePatternResult229 === "printf") {
                    if (activePatternResult228[1].tail.tail != null) {
                      var activePatternResult230 = _Token.Token["|T|_|"](activePatternResult228[1].tail.head);

                      if (activePatternResult230 != null) {
                        var activePatternResult231 = _StringContents___(activePatternResult230);

                        if (activePatternResult231 != null) {
                          if (activePatternResult228[1].tail.tail.tail == null) {
                            $var35 = [0, activePatternResult231, activePatternResult228[1].head];
                          } else {
                            $var35 = [1];
                          }
                        } else {
                          $var35 = [1];
                        }
                      } else {
                        $var35 = [1];
                      }
                    } else {
                      $var35 = [1];
                    }
                  } else if (activePatternResult229 === "scanf") {
                    if (activePatternResult228[1].tail.tail != null) {
                      var activePatternResult232 = _Token.Token["|T|_|"](activePatternResult228[1].tail.head);

                      if (activePatternResult232 != null) {
                        var activePatternResult233 = _StringContents___(activePatternResult232);

                        if (activePatternResult233 != null) {
                          if (activePatternResult228[1].tail.tail.tail == null) {
                            $var35 = [0, activePatternResult233, activePatternResult228[1].head];
                          } else {
                            $var35 = [1];
                          }
                        } else {
                          $var35 = [1];
                        }
                      } else {
                        $var35 = [1];
                      }
                    } else {
                      $var35 = [1];
                    }
                  } else {
                    $var35 = [1];
                  }
                } else {
                  $var35 = [1];
                }
              } else {
                $var35 = [1];
              }
            } else {
              $var35 = [1];
            }

            switch ($var35[0]) {
              case 0:
                return {
                  v: buildFormatFunction(function (fmt) {
                    return function (v) {
                      return _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var35[2], _Token.Token.Token[".ctor_3"](fmt)])), v]));
                    };
                  }, function (s) {
                    return _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var35[2], _Token.Token.Token[".ctor_3"](s)]));
                  })(separateFormatSymbols($var35[1]))
                };

              case 1:
                var activePatternResult227 = _Token.Token["|X|"](_arg1);

                return {
                  v: _Token.Token.Token[".ctor_2"](activePatternResult227[0], (0, _List.map)(function (_arg1_1) {
                    return mapFormatting(_arg1_1);
                  }, activePatternResult227[1]))
                };
            }

        }
      }();

      switch (_ret) {
        case "continue|mapFormatting":
          continue mapFormatting;

        default:
          if ((typeof _ret === "undefined" ? "undefined" : _typeof(_ret)) === "object") return _ret.v;
      }
    }
  }

  function processScan(_arg1) {
    var $var36 = void 0;

    var activePatternResult246 = _Token.Token["|X|"](_arg1);

    if (activePatternResult246[0] === "apply") {
      if (activePatternResult246[1].tail != null) {
        var activePatternResult247 = _Token.Token["|X|"](activePatternResult246[1].head);

        if (activePatternResult247[0] === "apply") {
          if (activePatternResult247[1].tail != null) {
            var activePatternResult248 = _Token.Token["|T|_|"](activePatternResult247[1].head);

            if (activePatternResult248 != null) {
              if (activePatternResult248 === "scanf") {
                if (activePatternResult247[1].tail.tail != null) {
                  var activePatternResult249 = _Token.Token["|T|_|"](activePatternResult247[1].tail.head);

                  if (activePatternResult249 != null) {
                    if (activePatternResult249 === "\"%i\"") {
                      if (activePatternResult247[1].tail.tail.tail == null) {
                        if (activePatternResult246[1].tail.tail != null) {
                          if (activePatternResult246[1].tail.tail.tail == null) {
                            $var36 = [0, activePatternResult246[1].tail.head];
                          } else {
                            $var36 = [1];
                          }
                        } else {
                          $var36 = [1];
                        }
                      } else {
                        $var36 = [1];
                      }
                    } else {
                      $var36 = [1];
                    }
                  } else {
                    $var36 = [1];
                  }
                } else {
                  $var36 = [1];
                }
              } else {
                $var36 = [1];
              }
            } else {
              $var36 = [1];
            }
          } else {
            $var36 = [1];
          }
        } else {
          $var36 = [1];
        }
      } else {
        $var36 = [1];
      }
    } else {
      $var36 = [1];
    }

    switch ($var36[0]) {
      case 0:
        return _Token.Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var36[1], _Token.Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("0")]))])), _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("scan"), _Token.Token.Token[".ctor_3"]("%i")]))]));

      case 1:
        var $var37 = void 0;

        var activePatternResult242 = _Token.Token["|X|"](_arg1);

        if (activePatternResult242[0] === "apply") {
          if (activePatternResult242[1].tail != null) {
            var activePatternResult243 = _Token.Token["|X|"](activePatternResult242[1].head);

            if (activePatternResult243[0] === "apply") {
              if (activePatternResult243[1].tail != null) {
                var activePatternResult244 = _Token.Token["|T|_|"](activePatternResult243[1].head);

                if (activePatternResult244 != null) {
                  if (activePatternResult244 === "scanf") {
                    if (activePatternResult243[1].tail.tail != null) {
                      var activePatternResult245 = _Token.Token["|T|_|"](activePatternResult243[1].tail.head);

                      if (activePatternResult245 != null) {
                        if (activePatternResult245 === "\"%s\"") {
                          if (activePatternResult243[1].tail.tail.tail == null) {
                            if (activePatternResult242[1].tail.tail != null) {
                              if (activePatternResult242[1].tail.tail.tail == null) {
                                $var37 = [0, activePatternResult242[1].tail.head];
                              } else {
                                $var37 = [1];
                              }
                            } else {
                              $var37 = [1];
                            }
                          } else {
                            $var37 = [1];
                          }
                        } else {
                          $var37 = [1];
                        }
                      } else {
                        $var37 = [1];
                      }
                    } else {
                      $var37 = [1];
                    }
                  } else {
                    $var37 = [1];
                  }
                } else {
                  $var37 = [1];
                }
              } else {
                $var37 = [1];
              }
            } else {
              $var37 = [1];
            }
          } else {
            $var37 = [1];
          }
        } else {
          $var37 = [1];
        }

        switch ($var37[0]) {
          case 0:
            return _Token.Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var37[1], _Token.Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("0")]))])), _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("scan"), _Token.Token.Token[".ctor_3"]("%s")]))]));

          case 1:
            var activePatternResult241 = _Token.Token["|X|"](_arg1);

            return _Token.Token.Token[".ctor_2"](activePatternResult241[0], (0, _List.map)(function (_arg1_1) {
              return processScan(_arg1_1);
            }, activePatternResult241[1]));
        }

    }
  }

  function processEscapeSequences(_arg1) {
    var activePatternResult253 = _Token.Token["|X|"](_arg1);

    var activePatternResult254 = _StringContents___(activePatternResult253[0]);

    if (activePatternResult254 != null) {
      var s_ = escapeSequences(activePatternResult254);
      return _Token.Token.Token[".ctor_2"]((0, _String.fsFormat)("\"%s\"")(function (x) {
        return x;
      })(s_), (0, _List.map)(function (_arg1_1) {
        return processEscapeSequences(_arg1_1);
      }, activePatternResult253[1]));
    } else {
      var activePatternResult252 = _Token.Token["|X|"](_arg1);

      return _Token.Token.Token[".ctor_2"](activePatternResult252[0], (0, _List.map)(function (_arg1_2) {
        return processEscapeSequences(_arg1_2);
      }, activePatternResult252[1]));
    }
  }

  var processStringFormatting = exports.processStringFormatting = function processStringFormatting($var39) {
    return function (_arg1) {
      return processEscapeSequences(_arg1);
    }(function ($var38) {
      return processScan(mapFormatting($var38));
    }($var39));
  };
});