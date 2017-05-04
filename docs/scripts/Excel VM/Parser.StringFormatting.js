define(["exports", "fable-core/umd/List", "./Parser.Definition", "fable-core/umd/String"], function (exports, _List, _Parser, _String) {
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
          var $var33 = _arg1.tail != null ? _arg1.head === "\\" ? _arg1.tail.tail != null ? [1, _arg1.tail.head, _arg1.tail.tail] : [2, _arg1.head, _arg1.tail] : [2, _arg1.head, _arg1.tail] : [0];

          switch ($var33[0]) {
            case 0:
              return buildString(builder);

            case 1:
              var k = void 0;
              var $var34 = $var33[1] === "\"" ? [0] : $var33[1] === "\\" ? [0] : $var33[1] === "n" ? [1] : $var33[1] === "t" ? [2] : [3];

              switch ($var34[0]) {
                case 0:
                  k = $var33[1];
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
              _arg1 = $var33[2];
              continue parseCharList;

            case 2:
              builder = new _List2.default($var33[1], builder);
              _arg1 = $var33[2];
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
            var $var35 = _arg1.tail != null ? isFormatString(_arg1.head) ? [0, _arg1.head, _arg1.tail] : [1] : [1];

            switch ($var35[0]) {
              case 0:
                var argName = _Parser.Token[".ctor_3"]((0, _String.fsFormat)("_%i")(function (x) {
                  return x;
                })(arg));

                return _Parser.Token[".ctor_2"]("fun", (0, _List.ofArray)([argName, buildFormatFunction_1(arg + 1)(new _List2.default(makeFormat((0, _String.fsFormat)("\"%s\"")(function (x) {
                  return x;
                })($var35[1]))(argName), ret))($var35[2])]));

              case 1:
                if (_arg1.tail == null) {
                  return _Parser.Token[".ctor_2"]("sequence", (0, _List.reverse)(ret));
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
      var $var36 = void 0;
      var activePatternResult237 = (0, _Parser.$7C$X$7C$)(_arg1);

      if (activePatternResult237[0] === "apply") {
        if (activePatternResult237[1].tail != null) {
          var activePatternResult238 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult237[1].head);

          if (activePatternResult238 != null) {
            if (activePatternResult238 === "printfn") {
              if (activePatternResult237[1].tail.tail != null) {
                var activePatternResult239 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult237[1].tail.head);

                if (activePatternResult239 != null) {
                  var activePatternResult240 = _StringContents___(activePatternResult239);

                  if (activePatternResult240 != null) {
                    if (activePatternResult237[1].tail.tail.tail == null) {
                      $var36 = [0, activePatternResult240];
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

      var _ret = function () {
        switch ($var36[0]) {
          case 0:
            _arg1 = _Parser.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("printf"), _Parser.Token[".ctor_3"]((0, _String.fsFormat)("\"%s\\n\"")(function (x) {
              return x;
            })($var36[1]))]));
            return "continue|mapFormatting";

          case 1:
            var $var37 = void 0;
            var activePatternResult231 = (0, _Parser.$7C$X$7C$)(_arg1);

            if (activePatternResult231[0] === "apply") {
              if (activePatternResult231[1].tail != null) {
                var activePatternResult232 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult231[1].head);

                if (activePatternResult232 != null) {
                  if (activePatternResult232 === "printf") {
                    if (activePatternResult231[1].tail.tail != null) {
                      var activePatternResult233 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult231[1].tail.head);

                      if (activePatternResult233 != null) {
                        var activePatternResult234 = _StringContents___(activePatternResult233);

                        if (activePatternResult234 != null) {
                          if (activePatternResult231[1].tail.tail.tail == null) {
                            $var37 = [0, activePatternResult234, activePatternResult231[1].head];
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
                  } else if (activePatternResult232 === "scanf") {
                    if (activePatternResult231[1].tail.tail != null) {
                      var activePatternResult235 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult231[1].tail.head);

                      if (activePatternResult235 != null) {
                        var activePatternResult236 = _StringContents___(activePatternResult235);

                        if (activePatternResult236 != null) {
                          if (activePatternResult231[1].tail.tail.tail == null) {
                            $var37 = [0, activePatternResult236, activePatternResult231[1].head];
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
                return {
                  v: buildFormatFunction(function (fmt) {
                    return function (v) {
                      return _Parser.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var37[2], _Parser.Token[".ctor_3"](fmt)])), v]));
                    };
                  }, function (s) {
                    return _Parser.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var37[2], _Parser.Token[".ctor_3"](s)]));
                  })(separateFormatSymbols($var37[1]))
                };

              case 1:
                var activePatternResult230 = (0, _Parser.$7C$X$7C$)(_arg1);
                return {
                  v: _Parser.Token[".ctor_2"](activePatternResult230[0], (0, _List.map)(function (_arg1_1) {
                    return mapFormatting(_arg1_1);
                  }, activePatternResult230[1]))
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
    var $var38 = void 0;
    var activePatternResult249 = (0, _Parser.$7C$X$7C$)(_arg1);

    if (activePatternResult249[0] === "apply") {
      if (activePatternResult249[1].tail != null) {
        var activePatternResult250 = (0, _Parser.$7C$X$7C$)(activePatternResult249[1].head);

        if (activePatternResult250[0] === "apply") {
          if (activePatternResult250[1].tail != null) {
            var activePatternResult251 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult250[1].head);

            if (activePatternResult251 != null) {
              if (activePatternResult251 === "scanf") {
                if (activePatternResult250[1].tail.tail != null) {
                  var activePatternResult252 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult250[1].tail.head);

                  if (activePatternResult252 != null) {
                    if (activePatternResult252 === "\"%i\"") {
                      if (activePatternResult250[1].tail.tail.tail == null) {
                        if (activePatternResult249[1].tail.tail != null) {
                          if (activePatternResult249[1].tail.tail.tail == null) {
                            $var38 = [0, activePatternResult249[1].tail.head];
                          } else {
                            $var38 = [1];
                          }
                        } else {
                          $var38 = [1];
                        }
                      } else {
                        $var38 = [1];
                      }
                    } else {
                      $var38 = [1];
                    }
                  } else {
                    $var38 = [1];
                  }
                } else {
                  $var38 = [1];
                }
              } else {
                $var38 = [1];
              }
            } else {
              $var38 = [1];
            }
          } else {
            $var38 = [1];
          }
        } else {
          $var38 = [1];
        }
      } else {
        $var38 = [1];
      }
    } else {
      $var38 = [1];
    }

    switch ($var38[0]) {
      case 0:
        return _Parser.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Parser.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var38[1], _Parser.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("0")]))])), _Parser.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("scan"), _Parser.Token[".ctor_3"]("\"%i\"")]))]));

      case 1:
        var $var39 = void 0;
        var activePatternResult245 = (0, _Parser.$7C$X$7C$)(_arg1);

        if (activePatternResult245[0] === "apply") {
          if (activePatternResult245[1].tail != null) {
            var activePatternResult246 = (0, _Parser.$7C$X$7C$)(activePatternResult245[1].head);

            if (activePatternResult246[0] === "apply") {
              if (activePatternResult246[1].tail != null) {
                var activePatternResult247 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult246[1].head);

                if (activePatternResult247 != null) {
                  if (activePatternResult247 === "scanf") {
                    if (activePatternResult246[1].tail.tail != null) {
                      var activePatternResult248 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult246[1].tail.head);

                      if (activePatternResult248 != null) {
                        if (activePatternResult248 === "\"%s\"") {
                          if (activePatternResult246[1].tail.tail.tail == null) {
                            if (activePatternResult245[1].tail.tail != null) {
                              if (activePatternResult245[1].tail.tail.tail == null) {
                                $var39 = [0, activePatternResult245[1].tail.head];
                              } else {
                                $var39 = [1];
                              }
                            } else {
                              $var39 = [1];
                            }
                          } else {
                            $var39 = [1];
                          }
                        } else {
                          $var39 = [1];
                        }
                      } else {
                        $var39 = [1];
                      }
                    } else {
                      $var39 = [1];
                    }
                  } else {
                    $var39 = [1];
                  }
                } else {
                  $var39 = [1];
                }
              } else {
                $var39 = [1];
              }
            } else {
              $var39 = [1];
            }
          } else {
            $var39 = [1];
          }
        } else {
          $var39 = [1];
        }

        switch ($var39[0]) {
          case 0:
            return _Parser.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Parser.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var39[1], _Parser.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("0")]))])), _Parser.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("scan"), _Parser.Token[".ctor_3"]("\"%s\"")]))]));

          case 1:
            var activePatternResult244 = (0, _Parser.$7C$X$7C$)(_arg1);
            return _Parser.Token[".ctor_2"](activePatternResult244[0], (0, _List.map)(function (_arg1_1) {
              return processScan(_arg1_1);
            }, activePatternResult244[1]));
        }

    }
  }

  function processEscapeSequences(_arg1) {
    var activePatternResult256 = (0, _Parser.$7C$X$7C$)(_arg1);

    var activePatternResult257 = _StringContents___(activePatternResult256[0]);

    if (activePatternResult257 != null) {
      var s_ = escapeSequences(activePatternResult257);
      return _Parser.Token[".ctor_2"]((0, _String.fsFormat)("\"%s\"")(function (x) {
        return x;
      })(s_), (0, _List.map)(function (_arg1_1) {
        return processEscapeSequences(_arg1_1);
      }, activePatternResult256[1]));
    } else {
      var activePatternResult255 = (0, _Parser.$7C$X$7C$)(_arg1);
      return _Parser.Token[".ctor_2"](activePatternResult255[0], (0, _List.map)(function (_arg1_2) {
        return processEscapeSequences(_arg1_2);
      }, activePatternResult255[1]));
    }
  }

  var processStringFormatting = exports.processStringFormatting = function processStringFormatting($var41) {
    return function (_arg1) {
      return processEscapeSequences(_arg1);
    }(function ($var40) {
      return processScan(mapFormatting($var40));
    }($var41));
  };
});