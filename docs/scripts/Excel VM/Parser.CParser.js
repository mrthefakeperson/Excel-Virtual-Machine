define(["exports", "fable-core/umd/List", "./Parser.Lexer", "./Parser.Definition", "fable-core/umd/String", "fable-core/umd/Seq", "fable-core/umd/Util", "fable-core/umd/Symbol", "./Parser.StringFormatting"], function (exports, _List, _Parser, _Parser2, _String, _Seq, _Util, _Symbol2, _Parser3) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.$7C$Transfer$7C$_$7C$ = exports.$7C$Index$7C$_$7C$ = exports.$7C$Apply$7C$_$7C$ = exports.$7C$Operator$7C$_$7C$ = exports.$7C$Suffix$7C$_$7C$ = exports.$7C$Prefix$7C$_$7C$ = exports.$7C$Dot$7C$_$7C$ = exports.$7C$Assignment$7C$_$7C$ = exports.$7C$Return$7C$_$7C$ = exports.$7C$For$7C$_$7C$ = exports.$7C$While$7C$_$7C$ = exports.$7C$If$7C$_$7C$ = exports.$7C$Braces$7C$_$7C$ = exports.$7C$Brackets$7C$_$7C$ = exports.$7C$NextBracePair$7C$ = exports.$7C$NextBracketPair$7C$ = exports.$7C$DatatypeLocal$7C$_$7C$ = exports.$7C$DatatypeNameL$7C$_$7C$ = exports.$7C$CommaFunction$7C$_$7C$ = exports.$7C$DatatypeFunction$7C$_$7C$ = exports.$7C$Struct$7C$_$7C$ = exports.$7C$DatatypeGlobal$7C$_$7C$ = exports.$7C$DatatypeName$7C$_$7C$ = exports.listOfDatatypeNames = exports.listOfDatatypeNamesDefault = exports.preprocess = undefined;
  exports.restoreDefault = restoreDefault;
  exports.parse = parse;
  exports.matchString = matchString;
  exports.getNextStatement = getNextStatement;
  exports.postProcess = postProcess;
  exports.parseSyntax = parseSyntax;

  var _List2 = _interopRequireDefault(_List);

  var _Symbol3 = _interopRequireDefault(_Symbol2);

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

  var preprocess = exports.preprocess = function () {
    var mainRules = (0, _List.append)((0, _Parser.createSingleLineComment)("#"), (0, _List.append)((0, _Parser.createSingleLineComment)("//"), (0, _List.append)((0, _Parser.createDelimitedComment)("/*", "*/"), (0, _List.append)(_Parser.createStrings, (0, _List.append)((0, _Parser.createSymbol)("=="), (0, _List.append)((0, _Parser.createSymbol)("!="), (0, _List.append)((0, _Parser.createSymbol)("<="), (0, _List.append)((0, _Parser.createSymbol)(">="), (0, _List.append)((0, _Parser.createSymbol)("+="), (0, _List.append)((0, _Parser.createSymbol)("-="), (0, _List.append)((0, _Parser.createSymbol)("*="), (0, _List.append)((0, _Parser.createSymbol)("/="), (0, _List.append)((0, _Parser.createSymbol)("++"), (0, _List.append)((0, _Parser.createSymbol)("--"), _Parser.createVariablesAndNumbers))))))))))))));
    return function ($var49) {
      return function () {
        var mapping_1 = function mapping_1(e_2) {
          return _Parser2.Token[".ctor_3"](e_2);
        };

        return function (list_2) {
          return (0, _List.map)(mapping_1, list_2);
        };
      }()(function ($var48) {
        return function () {
          var predicate = function predicate($var47) {
            return function (value_1) {
              return !value_1;
            }(function () {
              var classifierA = void 0;
              var classifierB = void 0;
              var delim1 = "//";
              var delim2 = "\n";

              classifierB = function classifierB(str) {
                return _Parser.CommonClassifiers.isDelimitedString(delim1, delim2, str);
              };

              classifierA = function classifierA(e) {
                return _Parser.CommonClassifiers.op_GreaterGreaterBarBar(_Parser.CommonClassifiers.isWhitespace, classifierB, e);
              };

              var classifierB_1 = void 0;
              var delim1_1 = "/*";
              var delim2_1 = "*/";

              classifierB_1 = function classifierB_1(str_1) {
                return _Parser.CommonClassifiers.isDelimitedString(delim1_1, delim2_1, str_1);
              };

              return function (e_1) {
                return _Parser.CommonClassifiers.op_GreaterGreaterBarBar(classifierA, classifierB_1, e_1);
              };
            }()($var47));
          };

          return function (list_1) {
            return (0, _List.filter)(predicate, list_1);
          };
        }()(function ($var46) {
          return function () {
            var mapping = function mapping(_arg1) {
              if (_arg1[0] === "#") {
                var matchValue = (0, _String.replace)(_arg1, " ", "");

                if (matchValue === "#include<stdio.h>\n") {
                  return new _List2.default();
                } else {
                  throw new Error("preprocessor commands are not supported yet");
                }
              } else {
                return (0, _List.ofArray)([_arg1]);
              }
            };

            return function (list) {
              return (0, _List.collect)(mapping, list);
            };
          }()(function ($var45) {
            return function (txt) {
              return (0, _Parser.tokenize)(mainRules, txt);
            }(function ($var44) {
              return (0, _List.map)(function (value) {
                return value;
              }, (0, _Seq.toList)($var44));
            }($var45));
          }($var46));
        }($var48));
      }($var49));
    };
  }();

  var listOfDatatypeNamesDefault = exports.listOfDatatypeNamesDefault = (0, _Seq.toList)((0, _Seq.sortWith)(function (x, y) {
    return (0, _Util.compare)(function (e) {
      return -(0, _String.split)(e, " ").length;
    }(x), function (e) {
      return -(0, _String.split)(e, " ").length;
    }(y));
  }, (0, _List.ofArray)(["int", "long long", "long", "bool", "char", "unsigned", "unsigned int", "unsigned long int", "unsigned long long int", "long int", "long long int"])));
  var listOfDatatypeNames = exports.listOfDatatypeNames = {
    contents: listOfDatatypeNamesDefault
  };

  function restoreDefault() {
    listOfDatatypeNames.contents = listOfDatatypeNamesDefault;
  }

  var State = function () {
    function State(caseName, fields) {
      _classCallCheck(this, State);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(State, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Parser.CParser.State",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            FunctionArgs: [],
            Global: [],
            Local: [],
            LocalImd: []
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
    }]);

    return State;
  }();

  (0, _Symbol2.setType)("Parser.CParser.State", State);

  function parse($var134, $var135, $var136, $var137, $var138) {
    var _loop = function _loop() {
      var state = $var134;
      var stop = $var135;
      var fail = $var136;
      var left = $var137;
      var right = $var138;
      var matchValue = [stop(right), fail(right), state];

      if (matchValue[0]) {
        return {
          v: [_Parser2.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right]
        };
      } else if (matchValue[1]) {
        return {
          v: (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
            throw new Error(x);
          })([left, right])
        };
      } else {
        var continueParsing = function continueParsing(x) {
          return parse(x[0], stop, fail, x[1], x[2]);
        };

        if (state.Case === "FunctionArgs") {
          var matchValue_1 = [left, right];
          var $var50 = void 0;

          var activePatternResult303 = _DatatypeFunction___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

          if (activePatternResult303 != null) {
            $var50 = [0, activePatternResult303];
          } else {
            var activePatternResult304 = _CommaFunction___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

            if (activePatternResult304 != null) {
              $var50 = [0, activePatternResult304];
            } else {
              var activePatternResult305 = _Transfer___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

              if (activePatternResult305 != null) {
                $var50 = [0, activePatternResult305];
              } else {
                $var50 = [1];
              }
            }
          }

          switch ($var50[0]) {
            case 0:
              return {
                v: continueParsing($var50[1])
              };

            case 1:
              return {
                v: (0, _String.fsFormat)("unknown: %A")(function (x) {
                  throw new Error(x);
                })([left, right])
              };
          }
        } else if (state.Case === "Local") {
          var matchValue_2 = [left, right];
          var $var51 = void 0;

          if (matchValue_2[0].tail != null) {
            var activePatternResult336 = (0, _Parser2.$7C$T$7C$_$7C$)(matchValue_2[0].head);

            if (activePatternResult336 != null) {
              if (activePatternResult336 === ";") {
                if (matchValue_2[1].tail != null) {
                  $var51 = [0, matchValue_2[1].head, matchValue_2[1].tail];
                } else {
                  $var51 = [1];
                }
              } else {
                $var51 = [1];
              }
            } else {
              $var51 = [1];
            }
          } else {
            $var51 = [1];
          }

          switch ($var51[0]) {
            case 0:
              $var134 = new State("Local", []);
              $var135 = stop;
              $var136 = fail;
              $var137 = new _List2.default($var51[1], left);
              $var138 = $var51[2];
              return "continue|parse";

            case 1:
              var $var52 = void 0;

              var activePatternResult321 = _DatatypeLocal___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

              if (activePatternResult321 != null) {
                $var52 = [0, activePatternResult321];
              } else {
                var activePatternResult322 = _Brackets___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                if (activePatternResult322 != null) {
                  $var52 = [0, activePatternResult322];
                } else {
                  var activePatternResult323 = _Braces___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                  if (activePatternResult323 != null) {
                    $var52 = [0, activePatternResult323];
                  } else {
                    var activePatternResult324 = _If___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                    if (activePatternResult324 != null) {
                      $var52 = [0, activePatternResult324];
                    } else {
                      var activePatternResult325 = _While___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                      if (activePatternResult325 != null) {
                        $var52 = [0, activePatternResult325];
                      } else {
                        var activePatternResult326 = _For___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                        if (activePatternResult326 != null) {
                          $var52 = [0, activePatternResult326];
                        } else {
                          var activePatternResult327 = _Return___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                          if (activePatternResult327 != null) {
                            $var52 = [0, activePatternResult327];
                          } else {
                            var activePatternResult328 = _Assignment___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                            if (activePatternResult328 != null) {
                              $var52 = [0, activePatternResult328];
                            } else {
                              var activePatternResult329 = _Dot___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                              if (activePatternResult329 != null) {
                                $var52 = [0, activePatternResult329];
                              } else {
                                var activePatternResult330 = _Prefix___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                                if (activePatternResult330 != null) {
                                  $var52 = [0, activePatternResult330];
                                } else {
                                  var activePatternResult331 = _Suffix___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                                  if (activePatternResult331 != null) {
                                    $var52 = [0, activePatternResult331];
                                  } else {
                                    var activePatternResult332 = _Operator___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                                    if (activePatternResult332 != null) {
                                      $var52 = [0, activePatternResult332];
                                    } else {
                                      var activePatternResult333 = _Apply___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                                      if (activePatternResult333 != null) {
                                        $var52 = [0, activePatternResult333];
                                      } else {
                                        var activePatternResult334 = _Index___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                                        if (activePatternResult334 != null) {
                                          $var52 = [0, activePatternResult334];
                                        } else {
                                          var activePatternResult335 = _Transfer___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                                          if (activePatternResult335 != null) {
                                            $var52 = [0, activePatternResult335];
                                          } else {
                                            $var52 = [1];
                                          }
                                        }
                                      }
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }

              switch ($var52[0]) {
                case 0:
                  return {
                    v: continueParsing($var52[1])
                  };

                case 1:
                  return {
                    v: (0, _String.fsFormat)("unknown: %A")(function (x) {
                      throw new Error(x);
                    })([left, right])
                  };
              }

          }
        } else if (state.Case === "LocalImd") {
          var matchValue_3 = [left, right];
          var $var53 = void 0;

          if (matchValue_3[0].tail != null) {
            var activePatternResult357 = (0, _Parser2.$7C$T$7C$_$7C$)(matchValue_3[0].head);

            if (activePatternResult357 != null) {
              if (activePatternResult357 === ";") {
                if (matchValue_3[1].tail != null) {
                  $var53 = [0, matchValue_3[1].head, matchValue_3[1].tail];
                } else {
                  $var53 = [1];
                }
              } else {
                $var53 = [1];
              }
            } else {
              $var53 = [1];
            }
          } else {
            $var53 = [1];
          }

          switch ($var53[0]) {
            case 0:
              $var134 = new State("Local", []);
              $var135 = stop;
              $var136 = fail;
              $var137 = new _List2.default($var53[1], left);
              $var138 = $var53[2];
              return "continue|parse";

            case 1:
              var $var54 = void 0;

              var activePatternResult347 = _Brackets___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

              if (activePatternResult347 != null) {
                $var54 = [0, activePatternResult347];
              } else {
                var activePatternResult348 = _Assignment___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                if (activePatternResult348 != null) {
                  $var54 = [0, activePatternResult348];
                } else {
                  var activePatternResult349 = _Dot___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                  if (activePatternResult349 != null) {
                    $var54 = [0, activePatternResult349];
                  } else {
                    var activePatternResult350 = _Prefix___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                    if (activePatternResult350 != null) {
                      $var54 = [0, activePatternResult350];
                    } else {
                      var activePatternResult351 = _Suffix___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                      if (activePatternResult351 != null) {
                        $var54 = [0, activePatternResult351];
                      } else {
                        var activePatternResult352 = _Operator___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                        if (activePatternResult352 != null) {
                          $var54 = [0, activePatternResult352];
                        } else {
                          var activePatternResult353 = _Apply___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                          if (activePatternResult353 != null) {
                            $var54 = [0, activePatternResult353];
                          } else {
                            var activePatternResult354 = _Index___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                            if (activePatternResult354 != null) {
                              $var54 = [0, activePatternResult354];
                            } else {
                              var activePatternResult355 = _CommaFunction___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                              if (activePatternResult355 != null) {
                                $var54 = [0, activePatternResult355];
                              } else {
                                var activePatternResult356 = _Transfer___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                                if (activePatternResult356 != null) {
                                  $var54 = [0, activePatternResult356];
                                } else {
                                  $var54 = [1];
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }

              switch ($var54[0]) {
                case 0:
                  return {
                    v: continueParsing($var54[1])
                  };

                case 1:
                  return {
                    v: (0, _String.fsFormat)("unknown: %A")(function (x) {
                      throw new Error(x);
                    })([left, right])
                  };
              }

          }
        } else {
          var matchValue_4 = [left, right];
          var $var55 = void 0;

          var activePatternResult291 = _DatatypeGlobal___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

          if (activePatternResult291 != null) {
            $var55 = [0, activePatternResult291];
          } else {
            var activePatternResult292 = _Struct___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

            if (activePatternResult292 != null) {
              $var55 = [0, activePatternResult292];
            } else {
              var activePatternResult293 = _Apply___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

              if (activePatternResult293 != null) {
                $var55 = [0, activePatternResult293];
              } else {
                var activePatternResult294 = _Index___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

                if (activePatternResult294 != null) {
                  $var55 = [0, activePatternResult294];
                } else {
                  var activePatternResult295 = _Brackets___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

                  if (activePatternResult295 != null) {
                    $var55 = [0, activePatternResult295];
                  } else {
                    var activePatternResult296 = _Braces___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

                    if (activePatternResult296 != null) {
                      $var55 = [0, activePatternResult296];
                    } else {
                      var activePatternResult297 = _Assignment___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

                      if (activePatternResult297 != null) {
                        $var55 = [0, activePatternResult297];
                      } else {
                        var activePatternResult298 = _Operator___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

                        if (activePatternResult298 != null) {
                          $var55 = [0, activePatternResult298];
                        } else {
                          var activePatternResult299 = _Transfer___(state, stop, fail, matchValue_4[0], matchValue_4[1]);

                          if (activePatternResult299 != null) {
                            $var55 = [0, activePatternResult299];
                          } else {
                            $var55 = [1];
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }

          switch ($var55[0]) {
            case 0:
              return {
                v: continueParsing($var55[1])
              };

            case 1:
              return {
                v: (0, _String.fsFormat)("unknown: %A")(function (x) {
                  throw new Error(x);
                })([left, right])
              };
          }
        }
      }
    };

    parse: while (true) {
      var _ret = _loop();

      switch (_ret) {
        case "continue|parse":
          continue parse;

        default:
          if ((typeof _ret === "undefined" ? "undefined" : _typeof(_ret)) === "object") return _ret.v;
      }
    }
  }

  function matchString(tokens, stringToStringList, s) {
    var findMatch = function findMatch(_arg1) {
      findMatch: while (true) {
        var $var56 = void 0;

        if (_arg1[0].tail != null) {
          if (_arg1[1].tail != null) {
            (function () {
              var activePatternResult363 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg1[1].head);

              if (activePatternResult363 != null) {
                if (function () {
                  var restb = _arg1[1].tail;
                  var resta = _arg1[0].tail;
                  var a = _arg1[0].head;
                  return a === activePatternResult363;
                }()) {
                  $var56 = [1, _arg1[0].head, activePatternResult363, _arg1[0].tail, _arg1[1].tail];
                } else {
                  $var56 = [2];
                }
              } else {
                $var56 = [2];
              }
            })();
          } else {
            $var56 = [2];
          }
        } else {
          $var56 = [0, _arg1[1]];
        }

        switch ($var56[0]) {
          case 0:
            return [s, $var56[1]];

          case 1:
            _arg1 = [$var56[3], $var56[4]];
            continue findMatch;

          case 2:
            return null;
        }
      }
    };

    return findMatch([stringToStringList(s), tokens]);
  }

  function _DatatypeName___(right) {
    return (0, _Seq.tryPick)(function () {
      var stringToStringList = function stringToStringList(e) {
        return (0, _List.ofArray)((0, _String.split)(e, " "));
      };

      return function (s) {
        return matchString(right, stringToStringList, s);
      };
    }(), listOfDatatypeNames.contents);
  }

  exports.$7C$DatatypeName$7C$_$7C$ = _DatatypeName___;

  function _DatatypeGlobal___(state, stop, fail, _arg2_0, _arg2_1) {
    var _arg2 = [_arg2_0, _arg2_1];

    var activePatternResult386 = _DatatypeName___(_arg2[1]);

    if (activePatternResult386 != null) {
      var patternInput = parse(new State("Global", []), function (_arg3) {
        var $var57 = void 0;

        if (_arg3.tail != null) {
          var activePatternResult371 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg3.head);

          if (activePatternResult371 != null) {
            if (activePatternResult371 === ";") {
              $var57 = [0];
            } else if (activePatternResult371 === "{") {
              $var57 = [0];
            } else if (activePatternResult371 === ",") {
              $var57 = [0];
            } else {
              $var57 = [1];
            }
          } else {
            $var57 = [1];
          }
        } else {
          $var57 = [1];
        }

        switch ($var57[0]) {
          case 0:
            return true;

          case 1:
            return false;
        }
      }, function (e) {
        return stop(e) ? true : fail(e);
      }, new _List2.default(), activePatternResult386[1]);
      var patternInput_2 = void 0;
      var matchValue = patternInput[0].Clean();
      var activePatternResult382 = (0, _Parser2.$7C$T$7C$_$7C$)(matchValue);

      if (activePatternResult382 != null) {
        patternInput_2 = [_Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult386[0]), patternInput[0]])), _Parser2.Token[".ctor_3"]("nothing")])), patternInput[1]];
      } else {
        var $var58 = void 0;
        var activePatternResult380 = (0, _Parser2.$7C$X$7C$)(matchValue);

        if (activePatternResult380[0] === "assign") {
          if (activePatternResult380[1].tail != null) {
            if (activePatternResult380[1].tail.tail != null) {
              if (activePatternResult380[1].tail.tail.tail == null) {
                $var58 = [0, activePatternResult380[1].head, activePatternResult380[1].tail.head];
              } else {
                $var58 = [3, matchValue];
              }
            } else {
              $var58 = [3, matchValue];
            }
          } else {
            $var58 = [3, matchValue];
          }
        } else if (activePatternResult380[0] === "dot") {
          if (activePatternResult380[1].tail != null) {
            if (activePatternResult380[1].tail.tail != null) {
              var activePatternResult381 = (0, _Parser2.$7C$X$7C$)(activePatternResult380[1].tail.head);

              if (activePatternResult381[0] === "[]") {
                if (activePatternResult380[1].tail.tail.tail == null) {
                  $var58 = [1, activePatternResult381[1], activePatternResult380[1].head];
                } else {
                  $var58 = [3, matchValue];
                }
              } else {
                $var58 = [3, matchValue];
              }
            } else {
              $var58 = [3, matchValue];
            }
          } else {
            $var58 = [3, matchValue];
          }
        } else if (activePatternResult380[0] === "apply") {
          if (activePatternResult380[1].tail != null) {
            if (activePatternResult380[1].tail.tail != null) {
              if (activePatternResult380[1].tail.tail.tail == null) {
                $var58 = [2, activePatternResult380[1].tail.head, activePatternResult380[1].head];
              } else {
                $var58 = [3, matchValue];
              }
            } else {
              $var58 = [3, matchValue];
            }
          } else {
            $var58 = [3, matchValue];
          }
        } else {
          $var58 = [3, matchValue];
        }

        switch ($var58[0]) {
          case 0:
            patternInput_2 = [_Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult386[0]), $var58[1]])), $var58[2]])), patternInput[1]];
            break;

          case 1:
            patternInput_2 = [_Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult386[0]), $var58[2]])), _Parser2.Token[".ctor_2"]("array", $var58[1])])), patternInput[1]];
            break;

          case 2:
            var patternInput_1 = parse(new State("Local", []), function (_arg4) {
              var $var59 = void 0;

              if (_arg4.tail != null) {
                var activePatternResult374 = (0, _Parser2.$7C$X$7C$)(_arg4.head);

                if (activePatternResult374[0] === "{}") {
                  $var59 = [0];
                } else {
                  $var59 = [1];
                }
              } else {
                $var59 = [1];
              }

              switch ($var59[0]) {
                case 0:
                  return true;

                case 1:
                  return false;
              }
            }, function (e_1) {
              return stop(e_1) ? true : fail(e_1);
            }, new _List2.default(), patternInput[1]);
            var $var60 = void 0;
            var activePatternResult377 = (0, _Parser2.$7C$X$7C$)(patternInput_1[0]);

            if (activePatternResult377[0] === "sequence") {
              if (activePatternResult377[1].tail == null) {
                if (patternInput_1[1].tail != null) {
                  $var60 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                } else {
                  $var60 = [1];
                }
              } else {
                $var60 = [1];
              }
            } else {
              $var60 = [1];
            }

            switch ($var60[0]) {
              case 0:
                patternInput_2 = [_Parser2.Token[".ctor_2"]("declare function", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult386[0]), $var58[2], $var58[1], $var60[1]])), $var60[2]];
                break;

              case 1:
                throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 128, 12);
                break;
            }

            break;

          case 3:
            patternInput_2 = (0, _String.fsFormat)("expression following data type declaration is invalid %O")(function (x) {
              throw new Error(x);
            })($var58[1]);
            break;
        }
      }

      var restr = void 0;
      var $var61 = void 0;

      if (patternInput_2[1].tail != null) {
        var activePatternResult385 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_2[1].head);

        if (activePatternResult385 != null) {
          if (activePatternResult385 === ",") {
            $var61 = [0, patternInput_2[1].tail];
          } else {
            $var61 = [1];
          }
        } else {
          $var61 = [1];
        }
      } else {
        $var61 = [1];
      }

      switch ($var61[0]) {
        case 0:
          restr = (0, _List.append)(new _List2.default(_Parser2.Token[".ctor_3"](";"), (0, _List.ofArray)((0, _String.split)(activePatternResult386[0], " ").map(function (e_2) {
            return _Parser2.Token[".ctor_3"](e_2);
          }))), $var61[1]);
          break;

        case 1:
          var $var62 = void 0;

          if (patternInput_2[1].tail != null) {
            var activePatternResult384 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_2[1].head);

            if (activePatternResult384 != null) {
              if (activePatternResult384 === ";") {
                $var62 = [0, patternInput_2[1].tail];
              } else {
                $var62 = [0, patternInput_2[1]];
              }
            } else {
              $var62 = [0, patternInput_2[1]];
            }
          } else {
            $var62 = [0, patternInput_2[1]];
          }

          switch ($var62[0]) {
            case 0:
              restr = $var62[1];
              break;
          }

          break;
      }

      return [new State("Global", []), _arg2[0], new _List2.default(patternInput_2[0], restr)];
    } else {
      return null;
    }
  }

  exports.$7C$DatatypeGlobal$7C$_$7C$ = _DatatypeGlobal___;

  function _Struct___(state, stop, fail, _arg5_0, _arg5_1) {
    var _arg5 = [_arg5_0, _arg5_1];
    var $var63 = void 0;

    if (_arg5[0].tail != null) {
      var activePatternResult395 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg5[0].head);

      if (activePatternResult395 != null) {
        if (activePatternResult395 === "struct") {
          $var63 = [0, _arg5[0].tail, _arg5[1]];
        } else {
          $var63 = [1];
        }
      } else {
        $var63 = [1];
      }
    } else {
      $var63 = [1];
    }

    switch ($var63[0]) {
      case 0:
        var patternInput = void 0;
        var $var64 = void 0;

        if ($var63[2].tail != null) {
          var activePatternResult393 = (0, _Parser2.$7C$T$7C$_$7C$)($var63[2].head);

          if (activePatternResult393 != null) {
            if (activePatternResult393 === "{") {
              $var64 = [0, $var63[2].tail];
            } else {
              $var64 = [1];
            }
          } else {
            $var64 = [1];
          }
        } else {
          $var64 = [1];
        }

        switch ($var64[0]) {
          case 0:
            patternInput = ["anonymousStruct", $var64[1]];
            break;

          case 1:
            var $var65 = void 0;

            if ($var63[2].tail != null) {
              var activePatternResult391 = (0, _Parser2.$7C$T$7C$_$7C$)($var63[2].head);

              if (activePatternResult391 != null) {
                if ($var63[2].tail.tail != null) {
                  var activePatternResult392 = (0, _Parser2.$7C$T$7C$_$7C$)($var63[2].tail.head);

                  if (activePatternResult392 != null) {
                    if (activePatternResult392 === "{") {
                      $var65 = [0, $var63[2].tail.tail, activePatternResult391];
                    } else {
                      $var65 = [1];
                    }
                  } else {
                    $var65 = [1];
                  }
                } else {
                  $var65 = [1];
                }
              } else {
                $var65 = [1];
              }
            } else {
              $var65 = [1];
            }

            switch ($var65[0]) {
              case 0:
                patternInput = [$var65[2], $var65[1]];
                break;

              case 1:
                patternInput = (0, _String.fsFormat)("not a valid struct declaration: %A")(function (x) {
                  throw new Error(x);
                })($var63[2]);
                break;
            }

            break;
        }

        listOfDatatypeNames.contents = new _List2.default("struct " + patternInput[0], listOfDatatypeNames.contents);

        var patternInput_1 = _NextBracePair_(new State("Local", []), stop, fail, (0, _List.ofArray)([_Parser2.Token[".ctor_3"]("{")]), patternInput[1]);

        var restr = void 0;
        var $var66 = void 0;

        if (patternInput_1[1].tail != null) {
          var activePatternResult394 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_1[1].head);

          if (activePatternResult394 != null) {
            if (activePatternResult394 === ";") {
              $var66 = [0, patternInput_1[1].tail];
            } else {
              $var66 = [1];
            }
          } else {
            $var66 = [1];
          }
        } else {
          $var66 = [1];
        }

        switch ($var66[0]) {
          case 0:
            restr = $var66[1];
            break;

          case 1:
            restr = (0, _List.ofArray)([_Parser2.Token[".ctor_3"]("struct"), _Parser2.Token[".ctor_3"](patternInput[0])], patternInput_1[1]);
            break;
        }

        var parsed = _Parser2.Token[".ctor_2"]("struct", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](patternInput[0]), patternInput_1[0]]));

        return [state, $var63[1], new _List2.default(parsed, restr)];

      case 1:
        return null;
    }
  }

  exports.$7C$Struct$7C$_$7C$ = _Struct___;

  function _DatatypeFunction___(state, stop, fail, _arg6_0, _arg6_1) {
    var _arg6 = [_arg6_0, _arg6_1];
    var $var67 = void 0;

    var activePatternResult401 = _DatatypeName___(_arg6[1]);

    if (activePatternResult401 != null) {
      if (activePatternResult401[1].tail != null) {
        var activePatternResult402 = (0, _Parser2.$7C$Pref$7C$_$7C$)(activePatternResult401[1].head);

        if (activePatternResult402 != null) {
          var activePatternResult403 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult402);

          if (activePatternResult403 != null) {
            if (activePatternResult401[1].tail.tail != null) {
              $var67 = [0, activePatternResult401[0], activePatternResult401[1].tail.head, _arg6[0], activePatternResult401[1].tail.tail, activePatternResult403];
            } else {
              $var67 = [1];
            }
          } else {
            $var67 = [1];
          }
        } else {
          $var67 = [1];
        }
      } else {
        $var67 = [1];
      }
    } else {
      $var67 = [1];
    }

    switch ($var67[0]) {
      case 0:
        var parsed = _Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"]($var67[1] + $var67[5].slice(1, $var67[5].length)), $var67[2]]));

        return [new State("FunctionArgs", []), $var67[3], new _List2.default(parsed, $var67[4])];

      case 1:
        var $var68 = void 0;

        var activePatternResult400 = _DatatypeName___(_arg6[1]);

        if (activePatternResult400 != null) {
          if (activePatternResult400[1].tail != null) {
            $var68 = [0, activePatternResult400[0], activePatternResult400[1].head, _arg6[0], activePatternResult400[1].tail];
          } else {
            $var68 = [1];
          }
        } else {
          $var68 = [1];
        }

        switch ($var68[0]) {
          case 0:
            var parsed_1 = _Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"]($var68[1]), $var68[2]]));

            return [new State("FunctionArgs", []), $var68[3], new _List2.default(parsed_1, $var68[4])];

          case 1:
            return null;
        }

    }
  }

  exports.$7C$DatatypeFunction$7C$_$7C$ = _DatatypeFunction___;

  function _CommaFunction___(state, stop, fail, _arg7_0, _arg7_1) {
    var _arg7 = [_arg7_0, _arg7_1];
    var $var69 = void 0;

    if (_arg7[0].tail != null) {
      if (_arg7[1].tail != null) {
        var activePatternResult410 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg7[1].head);

        if (activePatternResult410 != null) {
          if (activePatternResult410 === ",") {
            $var69 = [0, _arg7[0].head, _arg7[0].tail, _arg7[1].tail];
          } else {
            $var69 = [1];
          }
        } else {
          $var69 = [1];
        }
      } else {
        $var69 = [1];
      }
    } else {
      $var69 = [1];
    }

    switch ($var69[0]) {
      case 0:
        var patternInput = parse(state, stop, fail, new _List2.default(), $var69[3]);
        var parsed = void 0;
        var $var70 = void 0;
        var activePatternResult408 = (0, _Parser2.$7C$X$7C$)(patternInput[0]);

        if (activePatternResult408[0] === "sequence") {
          if (activePatternResult408[1].tail != null) {
            var activePatternResult409 = (0, _Parser2.$7C$X$7C$)(activePatternResult408[1].head);

            if (activePatternResult409[0] === ",") {
              if (activePatternResult408[1].tail.tail == null) {
                $var70 = [0, activePatternResult409[1]];
              } else {
                $var70 = [1];
              }
            } else {
              $var70 = [1];
            }
          } else {
            $var70 = [1];
          }
        } else {
          $var70 = [1];
        }

        switch ($var70[0]) {
          case 0:
            parsed = _Parser2.Token[".ctor_2"](",", new _List2.default($var69[1], $var70[1]));
            break;

          case 1:
            parsed = _Parser2.Token[".ctor_2"](",", (0, _List.ofArray)([$var69[1], patternInput[0]]));
            break;
        }

        return [state, $var69[2], new _List2.default(parsed, patternInput[1])];

      case 1:
        return null;
    }
  }

  exports.$7C$CommaFunction$7C$_$7C$ = _CommaFunction___;

  function _DatatypeNameL___(_arg8_0, _arg8_1) {
    var _arg8 = [_arg8_0, _arg8_1];

    if (_arg8[0].tail != null) {
      var restl = _arg8[0].tail;
      var hd = _arg8[0].head;
      var matchValue = new _List2.default(hd, _arg8[1]);

      var activePatternResult415 = _DatatypeName___(matchValue);

      if (activePatternResult415 != null) {
        return [restl, activePatternResult415[0], activePatternResult415[1]];
      } else {
        return null;
      }
    } else {
      return null;
    }
  }

  exports.$7C$DatatypeNameL$7C$_$7C$ = _DatatypeNameL___;

  function _DatatypeLocal___(state, stop, fail, _arg9_0, _arg9_1) {
    var _arg9 = [_arg9_0, _arg9_1];

    var activePatternResult446 = _DatatypeNameL___(_arg9[0], _arg9[1]);

    if (activePatternResult446 != null) {
      var patternInput = parse(new State("LocalImd", []), function (_arg10) {
        var $var71 = void 0;

        if (_arg10.tail != null) {
          var activePatternResult417 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg10.head);

          if (activePatternResult417 != null) {
            if (activePatternResult417 === ";") {
              $var71 = [0];
            } else if (activePatternResult417 === ",") {
              $var71 = [0];
            } else {
              $var71 = [1];
            }
          } else {
            $var71 = [1];
          }
        } else {
          $var71 = [1];
        }

        switch ($var71[0]) {
          case 0:
            return true;

          case 1:
            return false;
        }
      }, function (e) {
        return stop(e) ? true : fail(e);
      }, new _List2.default(), activePatternResult446[2]);
      var parsed = void 0;
      var pars = void 0;
      var $var72 = void 0;
      var activePatternResult420 = (0, _Parser2.$7C$X$7C$)(patternInput[0]);

      if (activePatternResult420[0] === "sequence") {
        if (activePatternResult420[1].tail != null) {
          if (activePatternResult420[1].tail.tail == null) {
            $var72 = [0, activePatternResult420[1].head];
          } else {
            $var72 = [1];
          }
        } else {
          $var72 = [1];
        }
      } else {
        $var72 = [1];
      }

      switch ($var72[0]) {
        case 0:
          pars = $var72[1];
          break;

        case 1:
          throw new Error("unexpected");
          break;
      }

      var $var73 = void 0;
      var activePatternResult443 = (0, _Parser2.$7C$X$7C$)(pars);

      if (activePatternResult443[0] === "assign") {
        if (activePatternResult443[1].tail != null) {
          var activePatternResult444 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult443[1].head);

          if (activePatternResult444 != null) {
            if (activePatternResult443[1].tail.tail != null) {
              if (activePatternResult443[1].tail.tail.tail == null) {
                $var73 = [0, activePatternResult444, activePatternResult443[1].head, activePatternResult443[1].tail.head];
              } else {
                $var73 = [1];
              }
            } else {
              $var73 = [1];
            }
          } else {
            $var73 = [1];
          }
        } else {
          $var73 = [1];
        }
      } else {
        $var73 = [1];
      }

      switch ($var73[0]) {
        case 0:
          parsed = _Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult446[1]), $var73[2]])), $var73[3]]));
          break;

        case 1:
          var activePatternResult442 = (0, _Parser2.$7C$T$7C$_$7C$)(pars);

          if (activePatternResult442 != null) {
            parsed = _Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult446[1]), pars])), _Parser2.Token[".ctor_3"]("nothing")]));
          } else {
            var $var74 = void 0;
            var activePatternResult439 = (0, _Parser2.$7C$X$7C$)(pars);

            if (activePatternResult439[0] === "dot") {
              if (activePatternResult439[1].tail != null) {
                var activePatternResult440 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult439[1].head);

                if (activePatternResult440 != null) {
                  if (activePatternResult439[1].tail.tail != null) {
                    var activePatternResult441 = (0, _Parser2.$7C$X$7C$)(activePatternResult439[1].tail.head);

                    if (activePatternResult441[0] === "[]") {
                      if (activePatternResult439[1].tail.tail.tail == null) {
                        $var74 = [0, activePatternResult441[1], activePatternResult440, activePatternResult439[1].head];
                      } else {
                        $var74 = [1];
                      }
                    } else {
                      $var74 = [1];
                    }
                  } else {
                    $var74 = [1];
                  }
                } else {
                  $var74 = [1];
                }
              } else {
                $var74 = [1];
              }
            } else {
              $var74 = [1];
            }

            switch ($var74[0]) {
              case 0:
                parsed = _Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult446[1]), $var74[3]])), _Parser2.Token[".ctor_2"]("array", $var74[1])]));
                break;

              case 1:
                var $var75 = void 0;
                var activePatternResult434 = (0, _Parser2.$7C$X$7C$)(pars);

                if (activePatternResult434[0] === "assign") {
                  if (activePatternResult434[1].tail != null) {
                    var activePatternResult435 = (0, _Parser2.$7C$X$7C$)(activePatternResult434[1].head);

                    if (activePatternResult435[0] === "apply") {
                      if (activePatternResult435[1].tail != null) {
                        var activePatternResult436 = (0, _Parser2.$7C$Pref$27$$7C$_$7C$)(activePatternResult435[1].head);

                        if (activePatternResult436 != null) {
                          var activePatternResult437 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult436);

                          if (activePatternResult437 != null) {
                            if (activePatternResult437 === "~*") {
                              if (activePatternResult435[1].tail.tail != null) {
                                var activePatternResult438 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult435[1].tail.head);

                                if (activePatternResult438 != null) {
                                  if (activePatternResult435[1].tail.tail.tail == null) {
                                    if (activePatternResult434[1].tail.tail != null) {
                                      if (activePatternResult434[1].tail.tail.tail == null) {
                                        $var75 = [0, activePatternResult435[1].tail.head, activePatternResult434[1].tail.head];
                                      } else {
                                        $var75 = [1];
                                      }
                                    } else {
                                      $var75 = [1];
                                    }
                                  } else {
                                    $var75 = [1];
                                  }
                                } else {
                                  $var75 = [1];
                                }
                              } else {
                                $var75 = [1];
                              }
                            } else {
                              $var75 = [1];
                            }
                          } else {
                            $var75 = [1];
                          }
                        } else {
                          $var75 = [1];
                        }
                      } else {
                        $var75 = [1];
                      }
                    } else {
                      $var75 = [1];
                    }
                  } else {
                    $var75 = [1];
                  }
                } else {
                  $var75 = [1];
                }

                switch ($var75[0]) {
                  case 0:
                    parsed = _Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult446[1] + "*"), $var75[1]])), $var75[2]]));
                    break;

                  case 1:
                    var $var76 = void 0;
                    var activePatternResult430 = (0, _Parser2.$7C$X$7C$)(pars);

                    if (activePatternResult430[0] === "apply") {
                      if (activePatternResult430[1].tail != null) {
                        var activePatternResult431 = (0, _Parser2.$7C$Pref$27$$7C$_$7C$)(activePatternResult430[1].head);

                        if (activePatternResult431 != null) {
                          var activePatternResult432 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult431);

                          if (activePatternResult432 != null) {
                            if (activePatternResult432 === "~*") {
                              if (activePatternResult430[1].tail.tail != null) {
                                var activePatternResult433 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult430[1].tail.head);

                                if (activePatternResult433 != null) {
                                  if (activePatternResult430[1].tail.tail.tail == null) {
                                    $var76 = [0, activePatternResult430[1].tail.head];
                                  } else {
                                    $var76 = [1];
                                  }
                                } else {
                                  $var76 = [1];
                                }
                              } else {
                                $var76 = [1];
                              }
                            } else {
                              $var76 = [1];
                            }
                          } else {
                            $var76 = [1];
                          }
                        } else {
                          $var76 = [1];
                        }
                      } else {
                        $var76 = [1];
                      }
                    } else {
                      $var76 = [1];
                    }

                    switch ($var76[0]) {
                      case 0:
                        parsed = _Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult446[1] + "*"), $var76[1]])), _Parser2.Token[".ctor_3"]("nothing")]));
                        break;

                      case 1:
                        var $var77 = void 0;
                        var activePatternResult424 = (0, _Parser2.$7C$X$7C$)(pars);

                        if (activePatternResult424[0] === "dot") {
                          if (activePatternResult424[1].tail != null) {
                            var activePatternResult425 = (0, _Parser2.$7C$X$7C$)(activePatternResult424[1].head);

                            if (activePatternResult425[0] === "apply") {
                              if (activePatternResult425[1].tail != null) {
                                var activePatternResult426 = (0, _Parser2.$7C$Pref$27$$7C$_$7C$)(activePatternResult425[1].head);

                                if (activePatternResult426 != null) {
                                  var activePatternResult427 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult426);

                                  if (activePatternResult427 != null) {
                                    if (activePatternResult427 === "~*") {
                                      if (activePatternResult425[1].tail.tail != null) {
                                        var activePatternResult428 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult425[1].tail.head);

                                        if (activePatternResult428 != null) {
                                          if (activePatternResult425[1].tail.tail.tail == null) {
                                            if (activePatternResult424[1].tail.tail != null) {
                                              var activePatternResult429 = (0, _Parser2.$7C$X$7C$)(activePatternResult424[1].tail.head);

                                              if (activePatternResult429[0] === "[]") {
                                                if (activePatternResult424[1].tail.tail.tail == null) {
                                                  $var77 = [0, activePatternResult429[1], activePatternResult425[1].tail.head];
                                                } else {
                                                  $var77 = [1];
                                                }
                                              } else {
                                                $var77 = [1];
                                              }
                                            } else {
                                              $var77 = [1];
                                            }
                                          } else {
                                            $var77 = [1];
                                          }
                                        } else {
                                          $var77 = [1];
                                        }
                                      } else {
                                        $var77 = [1];
                                      }
                                    } else {
                                      $var77 = [1];
                                    }
                                  } else {
                                    $var77 = [1];
                                  }
                                } else {
                                  $var77 = [1];
                                }
                              } else {
                                $var77 = [1];
                              }
                            } else {
                              $var77 = [1];
                            }
                          } else {
                            $var77 = [1];
                          }
                        } else {
                          $var77 = [1];
                        }

                        switch ($var77[0]) {
                          case 0:
                            parsed = _Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](activePatternResult446[1] + "*"), $var77[2]])), _Parser2.Token[".ctor_2"]("array", $var77[1])]));
                            break;

                          case 1:
                            parsed = (0, _String.fsFormat)("could not recognize declaration: %A")(function (x) {
                              throw new Error(x);
                            })(pars);
                            break;
                        }

                        break;
                    }

                    break;
                }

                break;
            }
          }

          break;
      }

      var restr = void 0;
      var $var78 = void 0;

      if (patternInput[1].tail != null) {
        var activePatternResult445 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput[1].head);

        if (activePatternResult445 != null) {
          if (activePatternResult445 === ",") {
            $var78 = [0, patternInput[1].tail];
          } else {
            $var78 = [1];
          }
        } else {
          $var78 = [1];
        }
      } else {
        $var78 = [1];
      }

      switch ($var78[0]) {
        case 0:
          restr = (0, _List.ofArray)([_Parser2.Token[".ctor_3"](";"), _Parser2.Token[".ctor_3"](activePatternResult446[1])], $var78[1]);
          break;

        case 1:
          restr = patternInput[1];
          break;
      }

      return [new State("LocalImd", []), activePatternResult446[0], new _List2.default(parsed, restr)];
    } else {
      return null;
    }
  }

  exports.$7C$DatatypeLocal$7C$_$7C$ = _DatatypeLocal___;

  function _NextBracketPair_(state, stop, fail, _arg11_0, _arg11_1) {
    var _arg11 = [_arg11_0, _arg11_1];
    var $var79 = void 0;

    if (_arg11[0].tail != null) {
      var activePatternResult454 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg11[0].head);

      if (activePatternResult454 != null) {
        if (activePatternResult454 === "(") {
          $var79 = [0, _arg11[0].tail, _arg11[1]];
        } else {
          $var79 = [1];
        }
      } else {
        $var79 = [1];
      }
    } else {
      $var79 = [1];
    }

    switch ($var79[0]) {
      case 0:
        var patternInput = parse(new State("LocalImd", []), function (_arg12) {
          var $var80 = void 0;

          if (_arg12.tail != null) {
            var activePatternResult451 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg12.head);

            if (activePatternResult451 != null) {
              if (activePatternResult451 === ")") {
                $var80 = [0];
              } else {
                $var80 = [1];
              }
            } else {
              $var80 = [1];
            }
          } else {
            $var80 = [1];
          }

          switch ($var80[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function () {
          var x = new _List2.default();
          return function (y) {
            return x.Equals(y);
          };
        }(), new _List2.default(), $var79[2]);
        var $var81 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult453 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput[1].head);

          if (activePatternResult453 != null) {
            if (activePatternResult453 === ")") {
              $var81 = [0, patternInput[0], patternInput[1].tail];
            } else {
              $var81 = [1];
            }
          } else {
            $var81 = [1];
          }
        } else {
          $var81 = [1];
        }

        switch ($var81[0]) {
          case 0:
            return [$var81[1], $var81[2]];

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 206, 8);
        }

      case 1:
        throw new Error("only use this when a bracket is at the top of the left stack");
    }
  }

  exports.$7C$NextBracketPair$7C$ = _NextBracketPair_;

  function _NextBracePair_(state, stop, fail, _arg13_0, _arg13_1) {
    var _arg13 = [_arg13_0, _arg13_1];
    var $var82 = void 0;

    if (_arg13[0].tail != null) {
      var activePatternResult463 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg13[0].head);

      if (activePatternResult463 != null) {
        if (activePatternResult463 === "{") {
          $var82 = [0, _arg13[0].tail, _arg13[1]];
        } else {
          $var82 = [1];
        }
      } else {
        $var82 = [1];
      }
    } else {
      $var82 = [1];
    }

    switch ($var82[0]) {
      case 0:
        var patternInput = parse(new State("Local", []), function (_arg14) {
          var $var83 = void 0;

          if (_arg14.tail != null) {
            var activePatternResult459 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg14.head);

            if (activePatternResult459 != null) {
              if (activePatternResult459 === "}") {
                $var83 = [0];
              } else {
                $var83 = [1];
              }
            } else {
              $var83 = [1];
            }
          } else {
            $var83 = [1];
          }

          switch ($var83[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var82[2]);
        var $var84 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult462 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput[1].head);

          if (activePatternResult462 != null) {
            if (activePatternResult462 === "}") {
              $var84 = [0, patternInput[0], patternInput[1].tail];
            } else {
              $var84 = [1];
            }
          } else {
            $var84 = [1];
          }
        } else {
          $var84 = [1];
        }

        switch ($var84[0]) {
          case 0:
            return [$var84[1], $var84[2]];

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 212, 8);
        }

      case 1:
        throw new Error("only use this when a brace is at the top of the left stack");
    }
  }

  exports.$7C$NextBracePair$7C$ = _NextBracePair_;

  function _Brackets___(state, stop, fail, _arg15_0, _arg15_1) {
    var _arg15 = [_arg15_0, _arg15_1];
    var $var85 = void 0;

    if (_arg15[0].tail != null) {
      var activePatternResult470 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg15[0].head);

      if (activePatternResult470 != null) {
        if (activePatternResult470 === "(") {
          var activePatternResult471 = _NextBracketPair_(state, stop, fail, _arg15[0], _arg15[1]);

          $var85 = [0, activePatternResult471[0], _arg15[0].tail, activePatternResult471[1]];
        } else {
          $var85 = [1];
        }
      } else {
        $var85 = [1];
      }
    } else {
      $var85 = [1];
    }

    switch ($var85[0]) {
      case 0:
        var parsed = _Parser2.Token[".ctor_2"]("()", (0, _List.ofArray)([$var85[1]]));

        return [state, $var85[2], new _List2.default(parsed, $var85[3])];

      case 1:
        return null;
    }
  }

  exports.$7C$Brackets$7C$_$7C$ = _Brackets___;

  function _Braces___(state, stop, fail, _arg16_0, _arg16_1) {
    var _arg16 = [_arg16_0, _arg16_1];
    var $var86 = void 0;

    if (_arg16[0].tail != null) {
      var activePatternResult478 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg16[0].head);

      if (activePatternResult478 != null) {
        if (activePatternResult478 === "{") {
          var activePatternResult479 = _NextBracePair_(state, stop, fail, _arg16[0], _arg16[1]);

          $var86 = [0, activePatternResult479[0], _arg16[0].tail, activePatternResult479[1], _arg16[1]];
        } else {
          $var86 = [1];
        }
      } else {
        $var86 = [1];
      }
    } else {
      $var86 = [1];
    }

    switch ($var86[0]) {
      case 0:
        var parsed = _Parser2.Token[".ctor_2"]("{}", (0, _List.ofArray)([$var86[1]]));

        return [state, $var86[2], new _List2.default(parsed, $var86[3])];

      case 1:
        return null;
    }
  }

  exports.$7C$Braces$7C$_$7C$ = _Braces___;

  function getNextStatement(state, stop, fail, _arg17) {
    var $var87 = void 0;

    if (_arg17.tail != null) {
      var activePatternResult507 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg17.head);

      if (activePatternResult507 != null) {
        if (activePatternResult507 === ";") {
          $var87 = [0, _arg17.tail];
        } else {
          $var87 = [1];
        }
      } else {
        $var87 = [1];
      }
    } else {
      $var87 = [1];
    }

    switch ($var87[0]) {
      case 0:
        return [_Parser2.Token[".ctor_3"]("()"), $var87[1]];

      case 1:
        if (_arg17.tail == null) {
          throw new Error("invalid end of file");
        } else {
          var patternInput = [(0, _List.ofArray)([_arg17.head]), _arg17.tail];
          var matchValue = [patternInput[0], patternInput[1]];

          if (stop(patternInput[1]) ? true : fail(patternInput[1])) {
            return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
              throw new Error(x);
            })([patternInput[0], patternInput[1]]);
          } else {
            var $var88 = void 0;

            var activePatternResult505 = _Braces___(state, stop, fail, matchValue[0], matchValue[1]);

            if (activePatternResult505 != null) {
              if (activePatternResult505[1].tail == null) {
                if (activePatternResult505[2].tail != null) {
                  var activePatternResult506 = (0, _Parser2.$7C$X$7C$)(activePatternResult505[2].head);

                  if (activePatternResult506[0] === "{}") {
                    if (activePatternResult506[1].tail != null) {
                      if (activePatternResult506[1].tail.tail == null) {
                        $var88 = [0, activePatternResult505[2].tail, activePatternResult506[1].head];
                      } else {
                        $var88 = [1];
                      }
                    } else {
                      $var88 = [1];
                    }
                  } else {
                    $var88 = [1];
                  }
                } else {
                  $var88 = [1];
                }
              } else {
                $var88 = [1];
              }
            } else {
              $var88 = [1];
            }

            switch ($var88[0]) {
              case 0:
                return [$var88[2], $var88[1]];

              case 1:
                var $var89 = void 0;

                var activePatternResult491 = _If___(state, stop, fail, matchValue[0], matchValue[1]);

                if (activePatternResult491 != null) {
                  if (activePatternResult491[1].tail == null) {
                    if (activePatternResult491[2].tail != null) {
                      $var89 = [0, activePatternResult491[2].tail, activePatternResult491[2].head];
                    } else {
                      var activePatternResult492 = _While___(state, stop, fail, matchValue[0], matchValue[1]);

                      if (activePatternResult492 != null) {
                        if (activePatternResult492[1].tail == null) {
                          if (activePatternResult492[2].tail != null) {
                            $var89 = [0, activePatternResult492[2].tail, activePatternResult492[2].head];
                          } else {
                            var activePatternResult493 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                            if (activePatternResult493 != null) {
                              if (activePatternResult493[1].tail == null) {
                                if (activePatternResult493[2].tail != null) {
                                  $var89 = [0, activePatternResult493[2].tail, activePatternResult493[2].head];
                                } else {
                                  $var89 = [1];
                                }
                              } else {
                                $var89 = [1];
                              }
                            } else {
                              $var89 = [1];
                            }
                          }
                        } else {
                          var activePatternResult494 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                          if (activePatternResult494 != null) {
                            if (activePatternResult494[1].tail == null) {
                              if (activePatternResult494[2].tail != null) {
                                $var89 = [0, activePatternResult494[2].tail, activePatternResult494[2].head];
                              } else {
                                $var89 = [1];
                              }
                            } else {
                              $var89 = [1];
                            }
                          } else {
                            $var89 = [1];
                          }
                        }
                      } else {
                        var activePatternResult495 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                        if (activePatternResult495 != null) {
                          if (activePatternResult495[1].tail == null) {
                            if (activePatternResult495[2].tail != null) {
                              $var89 = [0, activePatternResult495[2].tail, activePatternResult495[2].head];
                            } else {
                              $var89 = [1];
                            }
                          } else {
                            $var89 = [1];
                          }
                        } else {
                          $var89 = [1];
                        }
                      }
                    }
                  } else {
                    var activePatternResult496 = _While___(state, stop, fail, matchValue[0], matchValue[1]);

                    if (activePatternResult496 != null) {
                      if (activePatternResult496[1].tail == null) {
                        if (activePatternResult496[2].tail != null) {
                          $var89 = [0, activePatternResult496[2].tail, activePatternResult496[2].head];
                        } else {
                          var activePatternResult497 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                          if (activePatternResult497 != null) {
                            if (activePatternResult497[1].tail == null) {
                              if (activePatternResult497[2].tail != null) {
                                $var89 = [0, activePatternResult497[2].tail, activePatternResult497[2].head];
                              } else {
                                $var89 = [1];
                              }
                            } else {
                              $var89 = [1];
                            }
                          } else {
                            $var89 = [1];
                          }
                        }
                      } else {
                        var activePatternResult498 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                        if (activePatternResult498 != null) {
                          if (activePatternResult498[1].tail == null) {
                            if (activePatternResult498[2].tail != null) {
                              $var89 = [0, activePatternResult498[2].tail, activePatternResult498[2].head];
                            } else {
                              $var89 = [1];
                            }
                          } else {
                            $var89 = [1];
                          }
                        } else {
                          $var89 = [1];
                        }
                      }
                    } else {
                      var activePatternResult499 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                      if (activePatternResult499 != null) {
                        if (activePatternResult499[1].tail == null) {
                          if (activePatternResult499[2].tail != null) {
                            $var89 = [0, activePatternResult499[2].tail, activePatternResult499[2].head];
                          } else {
                            $var89 = [1];
                          }
                        } else {
                          $var89 = [1];
                        }
                      } else {
                        $var89 = [1];
                      }
                    }
                  }
                } else {
                  var activePatternResult500 = _While___(state, stop, fail, matchValue[0], matchValue[1]);

                  if (activePatternResult500 != null) {
                    if (activePatternResult500[1].tail == null) {
                      if (activePatternResult500[2].tail != null) {
                        $var89 = [0, activePatternResult500[2].tail, activePatternResult500[2].head];
                      } else {
                        var activePatternResult501 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                        if (activePatternResult501 != null) {
                          if (activePatternResult501[1].tail == null) {
                            if (activePatternResult501[2].tail != null) {
                              $var89 = [0, activePatternResult501[2].tail, activePatternResult501[2].head];
                            } else {
                              $var89 = [1];
                            }
                          } else {
                            $var89 = [1];
                          }
                        } else {
                          $var89 = [1];
                        }
                      }
                    } else {
                      var activePatternResult502 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                      if (activePatternResult502 != null) {
                        if (activePatternResult502[1].tail == null) {
                          if (activePatternResult502[2].tail != null) {
                            $var89 = [0, activePatternResult502[2].tail, activePatternResult502[2].head];
                          } else {
                            $var89 = [1];
                          }
                        } else {
                          $var89 = [1];
                        }
                      } else {
                        $var89 = [1];
                      }
                    }
                  } else {
                    var activePatternResult503 = _For___(state, stop, fail, matchValue[0], matchValue[1]);

                    if (activePatternResult503 != null) {
                      if (activePatternResult503[1].tail == null) {
                        if (activePatternResult503[2].tail != null) {
                          $var89 = [0, activePatternResult503[2].tail, activePatternResult503[2].head];
                        } else {
                          $var89 = [1];
                        }
                      } else {
                        $var89 = [1];
                      }
                    } else {
                      $var89 = [1];
                    }
                  }
                }

                switch ($var89[0]) {
                  case 0:
                    return [$var89[2], $var89[1]];

                  case 1:
                    var patternInput_1 = parse(state, function (_arg18) {
                      var $var90 = void 0;

                      if (_arg18.tail != null) {
                        var activePatternResult484 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg18.head);

                        if (activePatternResult484 != null) {
                          if (activePatternResult484 === ";") {
                            $var90 = [0];
                          } else {
                            $var90 = [1];
                          }
                        } else {
                          $var90 = [1];
                        }
                      } else {
                        $var90 = [1];
                      }

                      switch ($var90[0]) {
                        case 0:
                          return true;

                        case 1:
                          return false;
                      }
                    }, function (e) {
                      return stop(e) ? true : fail(e);
                    }, patternInput[0], patternInput[1]);
                    var $var91 = void 0;

                    if (patternInput_1[1].tail != null) {
                      var activePatternResult487 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_1[1].head);

                      if (activePatternResult487 != null) {
                        if (activePatternResult487 === ";") {
                          $var91 = [0, patternInput_1[0], patternInput_1[1].tail];
                        } else {
                          $var91 = [1];
                        }
                      } else {
                        $var91 = [1];
                      }
                    } else {
                      $var91 = [1];
                    }

                    switch ($var91[0]) {
                      case 0:
                        return [$var91[1], $var91[2]];

                      case 1:
                        throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 237, 10);
                    }

                }

            }
          }
        }

    }
  }

  function _If___(state, stop, fail, _arg19_0, _arg19_1) {
    var _arg19 = [_arg19_0, _arg19_1];
    var $var92 = void 0;

    if (_arg19[0].tail != null) {
      var activePatternResult514 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg19[0].head);

      if (activePatternResult514 != null) {
        if (activePatternResult514 === "if") {
          $var92 = [0, _arg19[0].tail, _arg19[1]];
        } else {
          $var92 = [1];
        }
      } else {
        $var92 = [1];
      }
    } else {
      $var92 = [1];
    }

    switch ($var92[0]) {
      case 0:
        var patternInput = void 0;
        var $var93 = void 0;

        if ($var92[2].tail != null) {
          var activePatternResult512 = (0, _Parser2.$7C$T$7C$_$7C$)($var92[2].head);

          if (activePatternResult512 != null) {
            if (activePatternResult512 === "(") {
              $var93 = [0, $var92[2].tail];
            } else {
              $var93 = [1];
            }
          } else {
            $var93 = [1];
          }
        } else {
          $var93 = [1];
        }

        switch ($var93[0]) {
          case 0:
            patternInput = _NextBracketPair_(new State("LocalImd", []), stop, fail, (0, _List.ofArray)([_Parser2.Token[".ctor_3"]("(")]), $var93[1]);
            break;

          case 1:
            throw new Error("( expected after if");
            break;
        }

        var patternInput_1 = getNextStatement(state, stop, fail, patternInput[1]);
        var patternInput_2 = void 0;
        var $var94 = void 0;

        if (patternInput_1[1].tail != null) {
          var activePatternResult513 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_1[1].head);

          if (activePatternResult513 != null) {
            if (activePatternResult513 === "else") {
              $var94 = [0, patternInput_1[1].tail];
            } else {
              $var94 = [1];
            }
          } else {
            $var94 = [1];
          }
        } else {
          $var94 = [1];
        }

        switch ($var94[0]) {
          case 0:
            patternInput_2 = getNextStatement(state, stop, fail, $var94[1]);
            break;

          case 1:
            patternInput_2 = [_Parser2.Token[".ctor_3"]("()"), patternInput_1[1]];
            break;
        }

        var parsed = _Parser2.Token[".ctor_2"]("if", (0, _List.ofArray)([patternInput[0], patternInput_1[0], patternInput_2[0]]));

        return [new State("Local", []), $var92[1], new _List2.default(parsed, patternInput_2[1])];

      case 1:
        return null;
    }
  }

  exports.$7C$If$7C$_$7C$ = _If___;

  function _While___(state, stop, fail, _arg20_0, _arg20_1) {
    var _arg20 = [_arg20_0, _arg20_1];
    var $var95 = void 0;

    if (_arg20[0].tail != null) {
      var activePatternResult520 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg20[0].head);

      if (activePatternResult520 != null) {
        if (activePatternResult520 === "while") {
          if (_arg20[1].tail != null) {
            var activePatternResult521 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg20[1].head);

            if (activePatternResult521 != null) {
              if (activePatternResult521 === "(") {
                $var95 = [0, _arg20[0].tail, _arg20[1]];
              } else {
                $var95 = [1];
              }
            } else {
              $var95 = [1];
            }
          } else {
            $var95 = [1];
          }
        } else {
          $var95 = [1];
        }
      } else {
        $var95 = [1];
      }
    } else {
      $var95 = [1];
    }

    switch ($var95[0]) {
      case 0:
        var patternInput = void 0;
        var $var96 = void 0;

        if ($var95[2].tail != null) {
          var activePatternResult519 = (0, _Parser2.$7C$T$7C$_$7C$)($var95[2].head);

          if (activePatternResult519 != null) {
            if (activePatternResult519 === "(") {
              $var96 = [0, $var95[2].tail];
            } else {
              $var96 = [1];
            }
          } else {
            $var96 = [1];
          }
        } else {
          $var96 = [1];
        }

        switch ($var96[0]) {
          case 0:
            patternInput = _NextBracketPair_(new State("LocalImd", []), stop, fail, (0, _List.ofArray)([_Parser2.Token[".ctor_3"]("(")]), $var96[1]);
            break;

          case 1:
            throw new Error("( expected after while");
            break;
        }

        var patternInput_1 = getNextStatement(state, stop, fail, patternInput[1]);

        var parsed = _Parser2.Token[".ctor_2"]("while", (0, _List.ofArray)([patternInput[0], patternInput_1[0]]));

        return [new State("Local", []), $var95[1], new _List2.default(parsed, patternInput_1[1])];

      case 1:
        return null;
    }
  }

  exports.$7C$While$7C$_$7C$ = _While___;

  function _For___(state, stop, fail, _arg21_0, _arg21_1) {
    var _arg21 = [_arg21_0, _arg21_1];
    var $var97 = void 0;

    if (_arg21[0].tail != null) {
      var activePatternResult538 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg21[0].head);

      if (activePatternResult538 != null) {
        if (activePatternResult538 === "for") {
          if (_arg21[1].tail != null) {
            var activePatternResult539 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg21[1].head);

            if (activePatternResult539 != null) {
              if (activePatternResult539 === "(") {
                $var97 = [0, _arg21[0].tail, _arg21[1].tail];
              } else {
                $var97 = [1];
              }
            } else {
              $var97 = [1];
            }
          } else {
            $var97 = [1];
          }
        } else {
          $var97 = [1];
        }
      } else {
        $var97 = [1];
      }
    } else {
      $var97 = [1];
    }

    switch ($var97[0]) {
      case 0:
        var patternInput = parse(new State("Local", []), function (_arg22) {
          var $var98 = void 0;

          if (_arg22.tail != null) {
            var activePatternResult526 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg22.head);

            if (activePatternResult526 != null) {
              if (activePatternResult526 === ";") {
                $var98 = [0];
              } else {
                $var98 = [1];
              }
            } else {
              $var98 = [1];
            }
          } else {
            $var98 = [1];
          }

          switch ($var98[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var97[2]);
        var $var99 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult537 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput[1].head);

          if (activePatternResult537 != null) {
            if (activePatternResult537 === ";") {
              $var99 = [0, patternInput[0], patternInput[1].tail];
            } else {
              $var99 = [1];
            }
          } else {
            $var99 = [1];
          }
        } else {
          $var99 = [1];
        }

        switch ($var99[0]) {
          case 0:
            var patternInput_1 = parse(new State("Local", []), function (_arg23) {
              var $var100 = void 0;

              if (_arg23.tail != null) {
                var activePatternResult529 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg23.head);

                if (activePatternResult529 != null) {
                  if (activePatternResult529 === ";") {
                    $var100 = [0];
                  } else {
                    $var100 = [1];
                  }
                } else {
                  $var100 = [1];
                }
              } else {
                $var100 = [1];
              }

              switch ($var100[0]) {
                case 0:
                  return true;

                case 1:
                  return false;
              }
            }, function (e_1) {
              return stop(e_1) ? true : fail(e_1);
            }, new _List2.default(), $var99[2]);
            var $var101 = void 0;

            if (patternInput_1[1].tail != null) {
              var activePatternResult536 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_1[1].head);

              if (activePatternResult536 != null) {
                if (activePatternResult536 === ";") {
                  $var101 = [0, patternInput_1[0], patternInput_1[1].tail];
                } else {
                  $var101 = [1];
                }
              } else {
                $var101 = [1];
              }
            } else {
              $var101 = [1];
            }

            switch ($var101[0]) {
              case 0:
                var patternInput_2 = parse(new State("Local", []), function (_arg24) {
                  var $var102 = void 0;

                  if (_arg24.tail != null) {
                    var activePatternResult532 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg24.head);

                    if (activePatternResult532 != null) {
                      if (activePatternResult532 === ")") {
                        $var102 = [0];
                      } else {
                        $var102 = [1];
                      }
                    } else {
                      $var102 = [1];
                    }
                  } else {
                    $var102 = [1];
                  }

                  switch ($var102[0]) {
                    case 0:
                      return true;

                    case 1:
                      return false;
                  }
                }, function (e_2) {
                  return stop(e_2) ? true : fail(e_2);
                }, new _List2.default(), $var101[2]);
                var $var103 = void 0;

                if (patternInput_2[1].tail != null) {
                  var activePatternResult535 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_2[1].head);

                  if (activePatternResult535 != null) {
                    if (activePatternResult535 === ")") {
                      $var103 = [0, patternInput_2[0], patternInput_2[1].tail];
                    } else {
                      $var103 = [1];
                    }
                  } else {
                    $var103 = [1];
                  }
                } else {
                  $var103 = [1];
                }

                switch ($var103[0]) {
                  case 0:
                    var patternInput_3 = getNextStatement(state, stop, fail, $var103[2]);

                    var parsed = _Parser2.Token[".ctor_2"]("for", (0, _List.ofArray)([$var99[1], $var101[1], $var103[1], patternInput_3[0]]));

                    return [new State("Local", []), $var97[1], new _List2.default(parsed, patternInput_3[1])];

                  case 1:
                    throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 272, 8);
                }

              case 1:
                throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 270, 8);
            }

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 268, 8);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$For$7C$_$7C$ = _For___;

  function _Return___(state, stop, fail, _arg25_0, _arg25_1) {
    var _arg25 = [_arg25_0, _arg25_1];
    var $var104 = void 0;

    if (_arg25[0].tail != null) {
      var activePatternResult548 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg25[0].head);

      if (activePatternResult548 != null) {
        if (activePatternResult548 === "return") {
          $var104 = [0, _arg25[0].tail, _arg25[1]];
        } else {
          $var104 = [1];
        }
      } else {
        $var104 = [1];
      }
    } else {
      $var104 = [1];
    }

    switch ($var104[0]) {
      case 0:
        var patternInput = parse(new State("LocalImd", []), function (_arg26) {
          var $var105 = void 0;

          if (_arg26.tail != null) {
            var activePatternResult544 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg26.head);

            if (activePatternResult544 != null) {
              if (activePatternResult544 === ";") {
                $var105 = [0];
              } else {
                $var105 = [1];
              }
            } else {
              $var105 = [1];
            }
          } else {
            $var105 = [1];
          }

          switch ($var105[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var104[2]);
        var returnedValue = void 0;
        var $var106 = void 0;
        var activePatternResult547 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput[0]);

        if (activePatternResult547 != null) {
          if (activePatternResult547 === "sequence") {
            $var106 = [0];
          } else {
            $var106 = [1];
          }
        } else {
          $var106 = [1];
        }

        switch ($var106[0]) {
          case 0:
            returnedValue = _Parser2.Token[".ctor_3"]("()");
            break;

          case 1:
            returnedValue = patternInput[0];
            break;
        }

        var parsed = _Parser2.Token[".ctor_2"]("return", (0, _List.ofArray)([returnedValue]));

        return [new State("Local", []), $var104[1], new _List2.default(parsed, patternInput[1])];

      case 1:
        return null;
    }
  }

  exports.$7C$Return$7C$_$7C$ = _Return___;

  function _Assignment___(state, stop, fail, _arg27_0, _arg27_1) {
    var _arg27 = [_arg27_0, _arg27_1];
    var $var107 = void 0;

    if (_arg27[0].tail != null) {
      if (_arg27[1].tail != null) {
        var activePatternResult555 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg27[1].head);

        if (activePatternResult555 != null) {
          if (activePatternResult555 === "=") {
            $var107 = [0, _arg27[0].head, activePatternResult555, _arg27[0].tail, _arg27[1].tail];
          } else if (activePatternResult555 === "+=") {
            $var107 = [0, _arg27[0].head, activePatternResult555, _arg27[0].tail, _arg27[1].tail];
          } else if (activePatternResult555 === "-=") {
            $var107 = [0, _arg27[0].head, activePatternResult555, _arg27[0].tail, _arg27[1].tail];
          } else if (activePatternResult555 === "*=") {
            $var107 = [0, _arg27[0].head, activePatternResult555, _arg27[0].tail, _arg27[1].tail];
          } else if (activePatternResult555 === "/=") {
            $var107 = [0, _arg27[0].head, activePatternResult555, _arg27[0].tail, _arg27[1].tail];
          } else {
            $var107 = [1];
          }
        } else {
          $var107 = [1];
        }
      } else {
        $var107 = [1];
      }
    } else {
      $var107 = [1];
    }

    switch ($var107[0]) {
      case 0:
        var patternInput = parse(new State("LocalImd", []), function (_arg28) {
          var $var108 = void 0;

          if (_arg28.tail != null) {
            var activePatternResult553 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg28.head);

            if (activePatternResult553 != null) {
              if (activePatternResult553 === ";") {
                $var108 = [0];
              } else {
                $var108 = [1];
              }
            } else {
              $var108 = [1];
            }
          } else {
            $var108 = [1];
          }

          switch ($var108[0]) {
            case 0:
              return true;

            case 1:
              return stop(_arg28);
          }
        }, fail, new _List2.default(), $var107[4]);
        var parsed = void 0;

        if ($var107[2] === "=") {
          parsed = _Parser2.Token[".ctor_2"]("assign", (0, _List.ofArray)([$var107[1], patternInput[0]]));
        } else {
          var operation = _Parser2.Token[".ctor_3"]($var107[2].slice(null, 0 + 1));

          parsed = _Parser2.Token[".ctor_2"]("assign", (0, _List.ofArray)([$var107[1], _Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([operation, $var107[1]])), patternInput[0]]))]));
        }

        return [new State("LocalImd", []), $var107[3], new _List2.default(parsed, patternInput[1])];

      case 1:
        return null;
    }
  }

  exports.$7C$Assignment$7C$_$7C$ = _Assignment___;

  function _Dot___(state, stop, fail, _arg29_0, _arg29_1) {
    var _arg29 = [_arg29_0, _arg29_1];
    var $var109 = void 0;

    if (_arg29[0].tail != null) {
      if (_arg29[1].tail != null) {
        var activePatternResult560 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg29[1].head);

        if (activePatternResult560 != null) {
          if (activePatternResult560 === ".") {
            if (_arg29[1].tail.tail != null) {
              var activePatternResult561 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg29[1].tail.head);

              if (activePatternResult561 != null) {
                $var109 = [0, _arg29[0].head, activePatternResult561, _arg29[0].tail, _arg29[1].tail.tail];
              } else {
                $var109 = [1];
              }
            } else {
              $var109 = [1];
            }
          } else {
            $var109 = [1];
          }
        } else {
          $var109 = [1];
        }
      } else {
        $var109 = [1];
      }
    } else {
      $var109 = [1];
    }

    switch ($var109[0]) {
      case 0:
        var parsed = _Parser2.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var109[1], _Parser2.Token[".ctor_3"]($var109[2])]));

        return [state, $var109[3], new _List2.default(parsed, $var109[4])];

      case 1:
        return null;
    }
  }

  exports.$7C$Dot$7C$_$7C$ = _Dot___;

  function _Prefix___(state, stop, fail, _arg30_0, _arg30_1) {
    var _arg30 = [_arg30_0, _arg30_1];
    var $var110 = void 0;

    if (_arg30[0].tail != null) {
      var activePatternResult583 = (0, _Parser2.$7C$Pref$7C$_$7C$)(_arg30[0].head);

      if (activePatternResult583 != null) {
        $var110 = [0, activePatternResult583, _arg30[0].tail, _arg30[1]];
      } else {
        $var110 = [1];
      }
    } else {
      $var110 = [1];
    }

    switch ($var110[0]) {
      case 0:
        var patternInput_2 = void 0;
        var $var111 = void 0;

        if ($var110[3].tail != null) {
          var activePatternResult581 = (0, _Parser2.$7C$T$7C$_$7C$)($var110[3].head);

          if (activePatternResult581 != null) {
            if ($var110[3].tail.tail != null) {
              var activePatternResult582 = (0, _Parser2.$7C$T$7C$_$7C$)($var110[3].tail.head);

              if (activePatternResult582 != null) {
                if (activePatternResult582 === "[") {
                  $var111 = [0, $var110[3].tail, activePatternResult581];
                } else {
                  $var111 = [1];
                }
              } else {
                $var111 = [1];
              }
            } else {
              $var111 = [1];
            }
          } else {
            $var111 = [1];
          }
        } else {
          $var111 = [1];
        }

        switch ($var111[0]) {
          case 0:
            var patternInput = parse(state, function (_arg31) {
              var $var112 = void 0;

              if (_arg31.tail != null) {
                var activePatternResult566 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg31.head);

                if (activePatternResult566 != null) {
                  $var112 = [0];
                } else {
                  $var112 = [1];
                }
              } else {
                $var112 = [1];
              }

              switch ($var112[0]) {
                case 0:
                  return false;

                case 1:
                  return true;
              }
            }, function (e) {
              return stop(e) ? true : fail(e);
            }, new _List2.default(), $var110[3]);
            var $var113 = void 0;
            var activePatternResult570 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput[0]);

            if (activePatternResult570 != null) {
              if (activePatternResult570 === "()") {
                if (patternInput[1].tail != null) {
                  $var113 = [0, patternInput[1].head, patternInput[1].tail];
                } else {
                  $var113 = [1];
                }
              } else {
                var activePatternResult571 = (0, _Parser2.$7C$X$7C$)(patternInput[0]);

                if (activePatternResult571[0] === "sequence") {
                  if (activePatternResult571[1].tail == null) {
                    if (patternInput[1].tail != null) {
                      $var113 = [0, patternInput[1].head, patternInput[1].tail];
                    } else {
                      $var113 = [1];
                    }
                  } else {
                    $var113 = [1];
                  }
                } else {
                  $var113 = [1];
                }
              }
            } else {
              var activePatternResult572 = (0, _Parser2.$7C$X$7C$)(patternInput[0]);

              if (activePatternResult572[0] === "sequence") {
                if (activePatternResult572[1].tail == null) {
                  if (patternInput[1].tail != null) {
                    $var113 = [0, patternInput[1].head, patternInput[1].tail];
                  } else {
                    $var113 = [1];
                  }
                } else {
                  $var113 = [1];
                }
              } else {
                $var113 = [1];
              }
            }

            switch ($var113[0]) {
              case 0:
                patternInput_2 = [$var113[1], $var113[2]];
                break;

              case 1:
                throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 310, 12);
                break;
            }

            break;

          case 1:
            var $var114 = void 0;

            if ($var110[3].tail != null) {
              var activePatternResult580 = (0, _Parser2.$7C$T$7C$_$7C$)($var110[3].head);

              if (activePatternResult580 != null) {
                if (_Parser.CommonClassifiers.op_GreaterGreaterBarBar(_Parser.CommonClassifiers.isNumeric, _Parser.CommonClassifiers.isVariable, activePatternResult580)) {
                  $var114 = [0, $var110[3].head, $var110[3].tail, activePatternResult580];
                } else {
                  $var114 = [1];
                }
              } else {
                $var114 = [1];
              }
            } else {
              $var114 = [1];
            }

            switch ($var114[0]) {
              case 0:
                patternInput_2 = [$var114[1], $var114[2]];
                break;

              case 1:
                var patternInput_1 = parse(state, function (_arg32) {
                  var $var115 = void 0;

                  if (_arg32.tail != null) {
                    var activePatternResult573 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg32.head);

                    if (activePatternResult573 != null) {
                      $var115 = [0];
                    } else {
                      $var115 = [1];
                    }
                  } else {
                    $var115 = [1];
                  }

                  switch ($var115[0]) {
                    case 0:
                      return false;

                    case 1:
                      return true;
                  }
                }, function (e_1) {
                  return stop(e_1) ? true : fail(e_1);
                }, new _List2.default(), $var110[3]);
                var $var116 = void 0;
                var activePatternResult577 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput_1[0]);

                if (activePatternResult577 != null) {
                  if (activePatternResult577 === "()") {
                    if (patternInput_1[1].tail != null) {
                      $var116 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                    } else {
                      $var116 = [1];
                    }
                  } else {
                    var activePatternResult578 = (0, _Parser2.$7C$X$7C$)(patternInput_1[0]);

                    if (activePatternResult578[0] === "sequence") {
                      if (activePatternResult578[1].tail == null) {
                        if (patternInput_1[1].tail != null) {
                          $var116 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                        } else {
                          $var116 = [1];
                        }
                      } else {
                        $var116 = [1];
                      }
                    } else {
                      $var116 = [1];
                    }
                  }
                } else {
                  var activePatternResult579 = (0, _Parser2.$7C$X$7C$)(patternInput_1[0]);

                  if (activePatternResult579[0] === "sequence") {
                    if (activePatternResult579[1].tail == null) {
                      if (patternInput_1[1].tail != null) {
                        $var116 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                      } else {
                        $var116 = [1];
                      }
                    } else {
                      $var116 = [1];
                    }
                  } else {
                    $var116 = [1];
                  }
                }

                switch ($var116[0]) {
                  case 0:
                    patternInput_2 = [$var116[1], $var116[2]];
                    break;

                  case 1:
                    throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 315, 12);
                    break;
                }

                break;
            }

            break;
        }

        var parsed = new _Parser2.Token("apply", $var110[1].Indentation, true, (0, _List.ofArray)([$var110[1], patternInput_2[0]]));
        return [state, $var110[2], new _List2.default(parsed, patternInput_2[1])];

      case 1:
        return null;
    }
  }

  exports.$7C$Prefix$7C$_$7C$ = _Prefix___;

  function _Suffix___(state, stop, fail, _arg33_0, _arg33_1) {
    var _arg33 = [_arg33_0, _arg33_1];
    var $var117 = void 0;

    if (_arg33[0].tail != null) {
      if (_arg33[1].tail != null) {
        var activePatternResult594 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg33[1].head);

        if (activePatternResult594 != null) {
          if (activePatternResult594 === "++") {
            $var117 = [0, _arg33[0].head, _arg33[0].tail, _arg33[1].tail, activePatternResult594];
          } else if (activePatternResult594 === "--") {
            $var117 = [0, _arg33[0].head, _arg33[0].tail, _arg33[1].tail, activePatternResult594];
          } else {
            $var117 = [1];
          }
        } else {
          $var117 = [1];
        }
      } else {
        $var117 = [1];
      }
    } else {
      $var117 = [1];
    }

    switch ($var117[0]) {
      case 0:
        var removeSideEffects = function removeSideEffects(_arg34) {
          removeSideEffects: while (true) {
            var $var118 = void 0;
            var activePatternResult589 = (0, _Parser2.$7C$X$7C$)(_arg34);

            if (activePatternResult589[0] === "apply") {
              if (activePatternResult589[1].tail != null) {
                var activePatternResult590 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult589[1].head);

                if (activePatternResult590 != null) {
                  if (activePatternResult590 === "~++") {
                    if (activePatternResult589[1].tail.tail != null) {
                      if (activePatternResult589[1].tail.tail.tail == null) {
                        $var118 = [0, activePatternResult589[1].tail.head];
                      } else {
                        $var118 = [1];
                      }
                    } else {
                      $var118 = [1];
                    }
                  } else if (activePatternResult590 === "~--") {
                    if (activePatternResult589[1].tail.tail != null) {
                      if (activePatternResult589[1].tail.tail.tail == null) {
                        $var118 = [0, activePatternResult589[1].tail.head];
                      } else {
                        $var118 = [1];
                      }
                    } else {
                      $var118 = [1];
                    }
                  } else {
                    $var118 = [1];
                  }
                } else {
                  $var118 = [1];
                }
              } else {
                $var118 = [1];
              }
            } else {
              $var118 = [1];
            }

            switch ($var118[0]) {
              case 0:
                _arg34 = $var118[1];
                continue removeSideEffects;

              case 1:
                return _arg34;
            }
          }
        };

        var a_ = removeSideEffects($var117[1]);
        var patternInput = void 0;

        switch ($var117[4]) {
          case "++":
            patternInput = ["+", "-"];
            break;

          case "--":
            patternInput = ["-", "+"];
            break;

          default:
            throw new Error("unrecognized");
        }

        var apply = function apply(c) {
          return function (a) {
            return _Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser2.Token[".ctor_3"](c), a])), _Parser2.Token[".ctor_3"]("1")]));
          };
        };

        var parsed = _Parser2.Token[".ctor_2"]("sequence", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("assign", (0, _List.ofArray)([removeSideEffects($var117[1]), apply(patternInput[0])($var117[1])])), apply(patternInput[1])(removeSideEffects($var117[1]))]));

        return [state, $var117[2], new _List2.default(parsed, $var117[3])];

      case 1:
        return null;
    }
  }

  exports.$7C$Suffix$7C$_$7C$ = _Suffix___;

  function _Operator___(state, stop, fail, _arg35_0, _arg35_1) {
    var _arg35 = [_arg35_0, _arg35_1];
    var $var119 = void 0;

    if (_arg35[0].tail != null) {
      if (_arg35[1].tail != null) {
        var activePatternResult602 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg35[1].head);

        if (activePatternResult602 != null) {
          if (function () {
            var restr = _arg35[1].tail;
            var restl = _arg35[0].tail;
            var nfx = _arg35[1].head;
            var a = _arg35[0].head;
            return nfx.Priority !== -1;
          }()) {
            $var119 = [0, _arg35[0].head, _arg35[1].head, _arg35[0].tail, _arg35[1].tail, activePatternResult602];
          } else {
            $var119 = [1];
          }
        } else {
          $var119 = [1];
        }
      } else {
        $var119 = [1];
      }
    } else {
      $var119 = [1];
    }

    switch ($var119[0]) {
      case 0:
        var patternInput = parse(new State("LocalImd", []), function (_arg36) {
          var $var120 = void 0;

          if (_arg36.tail != null) {
            var activePatternResult600 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg36.head);

            if (activePatternResult600 != null) {
              if (_arg36.head.Priority <= $var119[2].Priority ? _arg36.head.Priority !== -1 : false) {
                $var120 = [0, _arg36.head];
              } else {
                $var120 = [1];
              }
            } else {
              $var120 = [1];
            }
          } else {
            $var120 = [1];
          }

          switch ($var120[0]) {
            case 0:
              return true;

            case 1:
              var $var121 = void 0;

              if (_arg36.tail != null) {
                var activePatternResult599 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg36.head);

                if (activePatternResult599 != null) {
                  if (activePatternResult599 === ";") {
                    $var121 = [0];
                  } else if (activePatternResult599 === ",") {
                    $var121 = [0];
                  } else {
                    $var121 = [1];
                  }
                } else {
                  $var121 = [1];
                }
              } else {
                $var121 = [1];
              }

              switch ($var121[0]) {
                case 0:
                  return true;

                case 1:
                  return stop(_arg36);
              }

          }
        }, fail, new _List2.default(), $var119[4]);

        var parsed = _Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser2.Token[".ctor_3"]($var119[5]), $var119[1]])), patternInput[0]]));

        return [new State("LocalImd", []), $var119[3], new _List2.default(parsed, patternInput[1])];

      case 1:
        return null;
    }
  }

  exports.$7C$Operator$7C$_$7C$ = _Operator___;

  function _Apply___(state, stop, fail, _arg37_0, _arg37_1) {
    var _arg37 = [_arg37_0, _arg37_1];
    var $var122 = void 0;

    if (_arg37[0].tail != null) {
      var activePatternResult616 = (0, _Parser2.$7C$X$7C$)(_arg37[0].head);

      if (activePatternResult616[0] === "{}") {
        $var122 = [0];
      } else if (activePatternResult616[0] === "if") {
        $var122 = [0];
      } else if (activePatternResult616[0] === "while") {
        $var122 = [0];
      } else if (activePatternResult616[0] === "for") {
        $var122 = [0];
      } else if (_arg37[1].tail != null) {
        var activePatternResult617 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg37[1].head);

        if (activePatternResult617 != null) {
          if (activePatternResult617 === "(") {
            $var122 = [1, _arg37[0].head, _arg37[0].tail, _arg37[1].tail];
          } else {
            $var122 = [2];
          }
        } else {
          $var122 = [2];
        }
      } else {
        $var122 = [2];
      }
    } else {
      $var122 = [2];
    }

    switch ($var122[0]) {
      case 0:
        return null;

      case 1:
        var state_ = state.Case === "Global" ? new State("FunctionArgs", []) : new State("LocalImd", []);
        var patternInput = void 0;
        var matchValue = parse(state_, function (_arg38) {
          var $var123 = void 0;

          if (_arg38.tail != null) {
            var activePatternResult607 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg38.head);

            if (activePatternResult607 != null) {
              if (activePatternResult607 === ")") {
                $var123 = [0];
              } else {
                $var123 = [1];
              }
            } else {
              $var123 = [1];
            }
          } else {
            $var123 = [1];
          }

          switch ($var123[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (_arg1) {
          return false;
        }, new _List2.default(), $var122[3]);
        var $var124 = void 0;
        var activePatternResult613 = (0, _Parser2.$7C$X$7C$)(matchValue[0]);

        if (activePatternResult613[0] === "sequence") {
          if (activePatternResult613[1].tail != null) {
            if (activePatternResult613[1].tail.tail == null) {
              if (matchValue[1].tail != null) {
                var activePatternResult614 = (0, _Parser2.$7C$T$7C$_$7C$)(matchValue[1].head);

                if (activePatternResult614 != null) {
                  if (activePatternResult614 === ")") {
                    $var124 = [0, activePatternResult613[1].head, matchValue[1].tail];
                  } else {
                    $var124 = [1];
                  }
                } else {
                  $var124 = [1];
                }
              } else {
                $var124 = [1];
              }
            } else {
              $var124 = [1];
            }
          } else {
            $var124 = [1];
          }
        } else {
          $var124 = [1];
        }

        switch ($var124[0]) {
          case 0:
            patternInput = [$var124[1], $var124[2]];
            break;

          case 1:
            var $var125 = void 0;
            var activePatternResult611 = (0, _Parser2.$7C$X$7C$)(matchValue[0]);

            if (activePatternResult611[0] === "sequence") {
              if (activePatternResult611[1].tail == null) {
                if (matchValue[1].tail != null) {
                  var activePatternResult612 = (0, _Parser2.$7C$T$7C$_$7C$)(matchValue[1].head);

                  if (activePatternResult612 != null) {
                    if (activePatternResult612 === ")") {
                      $var125 = [0, matchValue[1].tail];
                    } else {
                      $var125 = [1];
                    }
                  } else {
                    $var125 = [1];
                  }
                } else {
                  $var125 = [1];
                }
              } else {
                $var125 = [1];
              }
            } else {
              $var125 = [1];
            }

            switch ($var125[0]) {
              case 0:
                patternInput = [_Parser2.Token[".ctor_2"]("()", new _List2.default()), $var125[1]];
                break;

              case 1:
                patternInput = (0, _String.fsFormat)("arguments were not formatted correctly %A")(function (x) {
                  throw new Error(x);
                })(matchValue);
                break;
            }

            break;
        }

        var parsed = _Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var122[1], patternInput[0]]));

        return [new State("LocalImd", []), $var122[2], new _List2.default(parsed, patternInput[1])];

      case 2:
        return null;
    }
  }

  exports.$7C$Apply$7C$_$7C$ = _Apply___;

  function _Index___(state, stop, fail, _arg39_0, _arg39_1) {
    var _arg39 = [_arg39_0, _arg39_1];
    var $var126 = void 0;

    if (_arg39[0].tail != null) {
      if (_arg39[1].tail != null) {
        var activePatternResult626 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg39[1].head);

        if (activePatternResult626 != null) {
          if (activePatternResult626 === "[") {
            $var126 = [0, _arg39[0].head, _arg39[0].tail, _arg39[1].tail];
          } else {
            $var126 = [1];
          }
        } else {
          $var126 = [1];
        }
      } else {
        $var126 = [1];
      }
    } else {
      $var126 = [1];
    }

    switch ($var126[0]) {
      case 0:
        var patternInput = parse(state, function (_arg40) {
          var $var127 = void 0;

          if (_arg40.tail != null) {
            var activePatternResult622 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg40.head);

            if (activePatternResult622 != null) {
              if (activePatternResult622 === "]") {
                $var127 = [0];
              } else {
                $var127 = [1];
              }
            } else {
              $var127 = [1];
            }
          } else {
            $var127 = [1];
          }

          switch ($var127[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (_arg2) {
          return false;
        }, new _List2.default(), $var126[3]);
        var $var128 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult625 = (0, _Parser2.$7C$T$7C$_$7C$)(patternInput[1].head);

          if (activePatternResult625 != null) {
            if (activePatternResult625 === "]") {
              $var128 = [0, patternInput[0], patternInput[1].tail];
            } else {
              $var128 = [1];
            }
          } else {
            $var128 = [1];
          }
        } else {
          $var128 = [1];
        }

        switch ($var128[0]) {
          case 0:
            var parsed = _Parser2.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var126[1], _Parser2.Token[".ctor_2"]("[]", (0, _List.ofArray)([$var128[1]]))]));

            return [state, $var126[2], new _List2.default(parsed, $var128[2])];

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 355, 8);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$Index$7C$_$7C$ = _Index___;

  function _Transfer___(state, stop, fail, _arg41_0, _arg41_1) {
    var _arg41 = [_arg41_0, _arg41_1];

    if (_arg41[1].tail != null) {
      var x = _arg41[1].head;
      var restr = _arg41[1].tail;
      return [state, new _List2.default(x, _arg41[0]), restr];
    } else {
      return null;
    }
  }

  exports.$7C$Transfer$7C$_$7C$ = _Transfer___;

  function postProcess(_DynamicRules___) {
    return function (_arg1) {
      var activePatternResult656 = _DynamicRules___(_arg1);

      if (activePatternResult656 != null) {
        return activePatternResult656;
      } else {
        var $var129 = void 0;
        var activePatternResult651 = (0, _Parser2.$7C$X$7C$)(_arg1);

        if (activePatternResult651[0] === "declare function") {
          if (activePatternResult651[1].tail != null) {
            if (activePatternResult651[1].tail.tail != null) {
              if (activePatternResult651[1].tail.tail.tail != null) {
                if (activePatternResult651[1].tail.tail.tail.tail != null) {
                  if (activePatternResult651[1].tail.tail.tail.tail.tail == null) {
                    $var129 = [0, activePatternResult651[1].tail.tail.head, activePatternResult651[1].head, activePatternResult651[1].tail.head, activePatternResult651[1].tail.tail.tail.head];
                  } else {
                    $var129 = [5];
                  }
                } else {
                  $var129 = [5];
                }
              } else {
                $var129 = [5];
              }
            } else {
              $var129 = [5];
            }
          } else {
            $var129 = [5];
          }
        } else if (activePatternResult651[0] === "{}") {
          $var129 = [1, activePatternResult651[1]];
        } else if (activePatternResult651[0] === "sequence") {
          $var129 = [2, activePatternResult651[1]];
        } else if (activePatternResult651[0] === "==") {
          $var129 = [3, activePatternResult651[1]];
        } else if (activePatternResult651[0] === "apply") {
          if (activePatternResult651[1].tail != null) {
            var activePatternResult652 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult651[1].head);

            if (activePatternResult652 != null) {
              if (activePatternResult652 === "printf") {
                if (activePatternResult651[1].tail.tail != null) {
                  var activePatternResult653 = (0, _Parser2.$7C$X$7C$)(activePatternResult651[1].tail.head);

                  if (activePatternResult653[0] === ",") {
                    if (activePatternResult653[1].tail != null) {
                      if (activePatternResult651[1].tail.tail.tail == null) {
                        $var129 = [4, activePatternResult653[1].tail, activePatternResult653[1].head, activePatternResult651[1].head];
                      } else {
                        $var129 = [5];
                      }
                    } else {
                      $var129 = [5];
                    }
                  } else {
                    $var129 = [5];
                  }
                } else {
                  $var129 = [5];
                }
              } else if (activePatternResult652 === "sprintf") {
                if (activePatternResult651[1].tail.tail != null) {
                  var activePatternResult654 = (0, _Parser2.$7C$X$7C$)(activePatternResult651[1].tail.head);

                  if (activePatternResult654[0] === ",") {
                    if (activePatternResult654[1].tail != null) {
                      if (activePatternResult651[1].tail.tail.tail == null) {
                        $var129 = [4, activePatternResult654[1].tail, activePatternResult654[1].head, activePatternResult651[1].head];
                      } else {
                        $var129 = [5];
                      }
                    } else {
                      $var129 = [5];
                    }
                  } else {
                    $var129 = [5];
                  }
                } else {
                  $var129 = [5];
                }
              } else if (activePatternResult652 === "scanf") {
                if (activePatternResult651[1].tail.tail != null) {
                  var activePatternResult655 = (0, _Parser2.$7C$X$7C$)(activePatternResult651[1].tail.head);

                  if (activePatternResult655[0] === ",") {
                    if (activePatternResult655[1].tail != null) {
                      if (activePatternResult651[1].tail.tail.tail == null) {
                        $var129 = [4, activePatternResult655[1].tail, activePatternResult655[1].head, activePatternResult651[1].head];
                      } else {
                        $var129 = [5];
                      }
                    } else {
                      $var129 = [5];
                    }
                  } else {
                    $var129 = [5];
                  }
                } else {
                  $var129 = [5];
                }
              } else {
                $var129 = [5];
              }
            } else {
              $var129 = [5];
            }
          } else {
            $var129 = [5];
          }
        } else {
          $var129 = [5];
        }

        switch ($var129[0]) {
          case 0:
            return _Parser2.Token[".ctor_2"]("let", (0, _List.ofArray)([$var129[3], _Parser2.Token[".ctor_2"]("fun", (0, _List.ofArray)([function (e) {
              return postProcess(_DynamicRules___)(e);
            }($var129[1]), function (e_1) {
              return postProcess(_DynamicRules___)(e_1);
            }($var129[4])]))]));

          case 1:
            return function (e_2) {
              return postProcess(_DynamicRules___)(e_2);
            }(_Parser2.Token[".ctor_2"]("sequence", $var129[1]));

          case 2:
            var xprs_ = (0, _List.filter)(function (_arg2) {
              var $var130 = void 0;
              var activePatternResult640 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg2);

              if (activePatternResult640 != null) {
                if (activePatternResult640 === ";") {
                  $var130 = [0];
                } else {
                  $var130 = [1];
                }
              } else {
                $var130 = [1];
              }

              switch ($var130[0]) {
                case 0:
                  return false;

                case 1:
                  return true;
              }
            }, $var129[1]);
            return _Parser2.Token[".ctor_2"]("sequence", (0, _List.map)(function (e_3) {
              return postProcess(_DynamicRules___)(e_3);
            }, xprs_));

          case 3:
            return _Parser2.Token[".ctor_2"]("=", (0, _List.map)(function (e_4) {
              return postProcess(_DynamicRules___)(e_4);
            }, $var129[1]));

          case 4:
            return (0, _Seq.fold)(function (acc, e_5) {
              return _Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([acc, function (e_6) {
                return postProcess(_DynamicRules___)(e_6);
              }(e_5)]));
            }, _Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var129[3], $var129[2]])), $var129[1]);

          case 5:
            var $var131 = void 0;
            var activePatternResult649 = (0, _Parser2.$7C$X$7C$)(_arg1);

            if (activePatternResult649[0] === "apply") {
              if (activePatternResult649[1].tail != null) {
                var activePatternResult650 = (0, _Parser2.$7C$T$7C$_$7C$)(activePatternResult649[1].head);

                if (activePatternResult650 != null) {
                  if (activePatternResult650 === "~++") {
                    if (activePatternResult649[1].tail.tail != null) {
                      if (activePatternResult649[1].tail.tail.tail == null) {
                        $var131 = [0, activePatternResult649[1].tail.head, activePatternResult650];
                      } else {
                        $var131 = [1];
                      }
                    } else {
                      $var131 = [1];
                    }
                  } else if (activePatternResult650 === "~--") {
                    if (activePatternResult649[1].tail.tail != null) {
                      if (activePatternResult649[1].tail.tail.tail == null) {
                        $var131 = [0, activePatternResult649[1].tail.head, activePatternResult650];
                      } else {
                        $var131 = [1];
                      }
                    } else {
                      $var131 = [1];
                    }
                  } else {
                    $var131 = [1];
                  }
                } else {
                  $var131 = [1];
                }
              } else {
                $var131 = [1];
              }
            } else {
              $var131 = [1];
            }

            switch ($var131[0]) {
              case 0:
                var op = _Parser2.Token[".ctor_3"]($var131[2].slice(1, 1 + 1));

                return _Parser2.Token[".ctor_2"]("assign", (0, _List.ofArray)([$var131[1], _Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([op, function (e_7) {
                  return postProcess(_DynamicRules___)(e_7);
                }($var131[1])])), _Parser2.Token[".ctor_3"]("1")]))]));

              case 1:
                var $var132 = void 0;
                var activePatternResult648 = (0, _Parser2.$7C$X$7C$)(_arg1);

                if (activePatternResult648[0] === "for") {
                  if (activePatternResult648[1].tail != null) {
                    if (activePatternResult648[1].tail.tail != null) {
                      if (activePatternResult648[1].tail.tail.tail != null) {
                        if (activePatternResult648[1].tail.tail.tail.tail != null) {
                          if (activePatternResult648[1].tail.tail.tail.tail.tail == null) {
                            $var132 = [0, activePatternResult648[1].tail.tail.tail.head, activePatternResult648[1].tail.head, activePatternResult648[1].head, activePatternResult648[1].tail.tail.head];
                          } else {
                            $var132 = [1, activePatternResult648[0], activePatternResult648[1]];
                          }
                        } else {
                          $var132 = [1, activePatternResult648[0], activePatternResult648[1]];
                        }
                      } else {
                        $var132 = [1, activePatternResult648[0], activePatternResult648[1]];
                      }
                    } else {
                      $var132 = [1, activePatternResult648[0], activePatternResult648[1]];
                    }
                  } else {
                    $var132 = [1, activePatternResult648[0], activePatternResult648[1]];
                  }
                } else {
                  $var132 = [1, activePatternResult648[0], activePatternResult648[1]];
                }

                switch ($var132[0]) {
                  case 0:
                    var incr_ = function (e_8) {
                      return postProcess(_DynamicRules___)(e_8);
                    }($var132[4]);

                    var replaceCont = function replaceCont(_arg3) {
                      var $var133 = void 0;
                      var activePatternResult645 = (0, _Parser2.$7C$T$7C$_$7C$)(_arg3);

                      if (activePatternResult645 != null) {
                        if (activePatternResult645 === "continue") {
                          $var133 = [0];
                        } else {
                          $var133 = [1];
                        }
                      } else {
                        $var133 = [1];
                      }

                      switch ($var133[0]) {
                        case 0:
                          return _Parser2.Token[".ctor_2"]("sequence", (0, _List.ofArray)([incr_, _Parser2.Token[".ctor_3"]("continue")]));

                        case 1:
                          return _DynamicRules___(_arg3);
                      }
                    };

                    return _Parser2.Token[".ctor_2"]("sequence", (0, _List.ofArray)([function (e_9) {
                      return postProcess(_DynamicRules___)(e_9);
                    }($var132[3]), _Parser2.Token[".ctor_2"]("while", (0, _List.ofArray)([function (e_10) {
                      return postProcess(_DynamicRules___)(e_10);
                    }($var132[2]), _Parser2.Token[".ctor_2"]("sequence", (0, _List.ofArray)([postProcess(replaceCont)($var132[1]), incr_]))]))]));

                  case 1:
                    return _Parser2.Token[".ctor_2"]($var132[1], (0, _List.map)(function (e_11) {
                      return postProcess(_DynamicRules___)(e_11);
                    }, $var132[2]));
                }

            }

        }
      }
    };
  }

  function parseSyntax(e) {
    restoreDefault();
    return (0, _Parser3.processStringFormatting)(function (_arg3) {
      var activePatternResult662 = (0, _Parser2.$7C$X$7C$)(_arg3);

      if (activePatternResult662[0] === "sequence") {
        return _Parser2.Token[".ctor_2"]("sequence", (0, _List.append)(activePatternResult662[1], (0, _List.ofArray)([_Parser2.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Parser2.Token[".ctor_3"]("main"), _Parser2.Token[".ctor_3"]("()")]))])));
      } else {
        throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.CParser.fs", 397, 6);
      }
    }(postProcess(function (_arg2) {
      return null;
    })(parse(new State("Global", []), function (_arg1) {
      return _arg1.tail == null ? true : false;
    }, function (_arg1_1) {
      return false;
    }, new _List2.default(), preprocess(e))[0])));
  }
});