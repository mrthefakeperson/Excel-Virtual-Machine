define(["exports", "fable-core/umd/List", "./Lexer", "./Token", "fable-core/umd/Seq", "fable-core/umd/String", "fable-core/umd/Util", "fable-core/umd/Symbol", "./String Formatting"], function (exports, _List, _Lexer, _Token, _Seq, _String, _Util, _Symbol2, _StringFormatting) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.$7C$Transfer$7C$_$7C$ = exports.$7C$Index$7C$_$7C$ = exports.$7C$Apply$7C$_$7C$ = exports.$7C$Operator$7C$_$7C$ = exports.$7C$Prefix$7C$_$7C$ = exports.$7C$Dot$7C$_$7C$ = exports.$7C$Assignment$7C$_$7C$ = exports.$7C$Return$7C$_$7C$ = exports.$7C$For$7C$_$7C$ = exports.$7C$While$7C$_$7C$ = exports.$7C$If$7C$_$7C$ = exports.$7C$Braces$7C$_$7C$ = exports.$7C$Brackets$7C$_$7C$ = exports.$7C$DatatypeLocal$7C$_$7C$ = exports.$7C$DatatypeNameL$7C$_$7C$ = exports.$7C$CommaFunction$7C$_$7C$ = exports.$7C$DatatypeFunction$7C$_$7C$ = exports.$7C$Struct$7C$_$7C$ = exports.$7C$DatatypeGlobal$7C$_$7C$ = exports.$7C$DatatypeName$7C$_$7C$ = exports.State = exports.listOfDatatypeNames = exports.listOfDatatypeNamesDefault = exports.preprocess = undefined;
  exports.restoreDefault = restoreDefault;
  exports.parse = parse;
  exports.matchString = matchString;
  exports.getStatementBody = getStatementBody;
  exports.postProcess = postProcess;
  exports.parseSyntax = parseSyntax;

  var _List2 = _interopRequireDefault(_List);

  var _Symbol3 = _interopRequireDefault(_Symbol2);

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

  var preprocess = exports.preprocess = function () {
    var mainRules = (0, _List.append)((0, _Lexer.singleLineCommentRules)("#"), (0, _List.append)((0, _Lexer.singleLineCommentRules)("//"), (0, _List.append)((0, _Lexer.delimitedCommentRules)("/*", "*/"), (0, _List.append)((0, _Lexer.createSymbol)("=="), (0, _List.append)((0, _Lexer.createSymbol)("!="), (0, _List.append)((0, _Lexer.createSymbol)("<="), (0, _List.append)((0, _Lexer.createSymbol)(">="), (0, _List.append)((0, _Lexer.createSymbol)("++"), (0, _List.append)((0, _Lexer.createSymbol)("--"), _Lexer.commonRules)))))))));
    return function ($var53) {
      return function (list_4) {
        return (0, _List.reverse)(list_4);
      }(function ($var52) {
        return function () {
          var folder = function folder(acc) {
            return function (e_3) {
              var matchValue_1 = [acc, e_3];
              var $var48 = void 0;

              if (matchValue_1[0].tail != null) {
                var activePatternResult260 = _Token.Token["|T|_|"](matchValue_1[0].head);

                if (activePatternResult260 != null) {
                  if (activePatternResult260 === "{") {
                    if (matchValue_1[0].tail.tail != null) {
                      if (matchValue_1[0].tail.tail.tail != null) {
                        var activePatternResult261 = _Token.Token["|T|_|"](matchValue_1[0].tail.tail.head);

                        if (activePatternResult261 != null) {
                          if (activePatternResult261 === "struct") {
                            var activePatternResult262 = _Token.Token["|T|_|"](matchValue_1[1]);

                            if (activePatternResult262 != null) {
                              if (activePatternResult262 === "}") {
                                $var48 = [0];
                              } else {
                                $var48 = [1];
                              }
                            } else {
                              $var48 = [1];
                            }
                          } else {
                            var activePatternResult263 = _Token.Token["|T|_|"](matchValue_1[0].tail.head);

                            if (activePatternResult263 != null) {
                              if (activePatternResult263 === "struct") {
                                var activePatternResult264 = _Token.Token["|T|_|"](matchValue_1[1]);

                                if (activePatternResult264 != null) {
                                  if (activePatternResult264 === "}") {
                                    $var48 = [0];
                                  } else {
                                    $var48 = [1];
                                  }
                                } else {
                                  $var48 = [1];
                                }
                              } else {
                                $var48 = [1];
                              }
                            } else {
                              $var48 = [1];
                            }
                          }
                        } else {
                          var activePatternResult265 = _Token.Token["|T|_|"](matchValue_1[0].tail.head);

                          if (activePatternResult265 != null) {
                            if (activePatternResult265 === "struct") {
                              var activePatternResult266 = _Token.Token["|T|_|"](matchValue_1[1]);

                              if (activePatternResult266 != null) {
                                if (activePatternResult266 === "}") {
                                  $var48 = [0];
                                } else {
                                  $var48 = [1];
                                }
                              } else {
                                $var48 = [1];
                              }
                            } else {
                              $var48 = [1];
                            }
                          } else {
                            $var48 = [1];
                          }
                        }
                      } else {
                        var activePatternResult267 = _Token.Token["|T|_|"](matchValue_1[0].tail.head);

                        if (activePatternResult267 != null) {
                          if (activePatternResult267 === "struct") {
                            var activePatternResult268 = _Token.Token["|T|_|"](matchValue_1[1]);

                            if (activePatternResult268 != null) {
                              if (activePatternResult268 === "}") {
                                $var48 = [0];
                              } else {
                                $var48 = [1];
                              }
                            } else {
                              $var48 = [1];
                            }
                          } else {
                            $var48 = [1];
                          }
                        } else {
                          $var48 = [1];
                        }
                      }
                    } else {
                      $var48 = [1];
                    }
                  } else {
                    $var48 = [1];
                  }
                } else {
                  $var48 = [1];
                }
              } else {
                $var48 = [1];
              }

              switch ($var48[0]) {
                case 0:
                  return new _List2.default(e_3, acc);

                case 1:
                  var $var49 = void 0;

                  if (matchValue_1[0].tail != null) {
                    var activePatternResult258 = _Token.Token["|T|_|"](matchValue_1[0].head);

                    if (activePatternResult258 != null) {
                      if (activePatternResult258 === "{") {
                        var activePatternResult259 = _Token.Token["|T|_|"](matchValue_1[1]);

                        if (activePatternResult259 != null) {
                          if (activePatternResult259 === "}") {
                            $var49 = [0, matchValue_1[0].tail];
                          } else {
                            $var49 = [1];
                          }
                        } else {
                          $var49 = [1];
                        }
                      } else {
                        $var49 = [1];
                      }
                    } else {
                      $var49 = [1];
                    }
                  } else {
                    $var49 = [1];
                  }

                  switch ($var49[0]) {
                    case 0:
                      return new _List2.default(_Token.Token.Token[".ctor_3"](";"), $var49[1]);

                    case 1:
                      return new _List2.default(e_3, acc);
                  }

              }
            };
          };

          var state = new _List2.default();
          return function (list_3) {
            return (0, _Seq.fold)(function ($var50, $var51) {
              return folder($var50)($var51);
            }, state, list_3);
          };
        }()(function ($var47) {
          return function () {
            var mapping_1 = function mapping_1(e_2) {
              return _Token.Token.Token[".ctor_3"](e_2);
            };

            return function (list_2) {
              return (0, _List.map)(mapping_1, list_2);
            };
          }()(function ($var46) {
            return function () {
              var predicate = function predicate($var45) {
                return function (value_1) {
                  return !value_1;
                }(function () {
                  var classifierA = void 0;
                  var classifierB = void 0;
                  var delim1 = "//";
                  var delim2 = "\n";

                  classifierB = function classifierB(str) {
                    return (0, _Lexer.isDelimitedString)(delim1, delim2, str);
                  };

                  classifierA = function classifierA(e) {
                    return _Lexer.CommonClassifiers.op_GreaterGreaterBarBar(_Lexer.CommonClassifiers.isWhitespace, classifierB, e);
                  };

                  var classifierB_1 = void 0;
                  var delim1_1 = "/*";
                  var delim2_1 = "*/";

                  classifierB_1 = function classifierB_1(str_1) {
                    return (0, _Lexer.isDelimitedString)(delim1_1, delim2_1, str_1);
                  };

                  return function (e_1) {
                    return _Lexer.CommonClassifiers.op_GreaterGreaterBarBar(classifierA, classifierB_1, e_1);
                  };
                }()($var45));
              };

              return function (list_1) {
                return (0, _List.filter)(predicate, list_1);
              };
            }()(function ($var44) {
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
              }()(function ($var43) {
                return function (txt) {
                  return (0, _Lexer.tokenize)(mainRules, txt);
                }(function ($var42) {
                  return (0, _List.map)(function (value) {
                    return value;
                  }, (0, _Seq.toList)($var42));
                }($var43));
              }($var44));
            }($var46));
          }($var47));
        }($var52));
      }($var53));
    };
  }();

  var listOfDatatypeNamesDefault = exports.listOfDatatypeNamesDefault = (0, _Seq.toList)((0, _Seq.sortWith)(function (x, y) {
    return (0, _Util.compare)(function (e) {
      return (0, _String.split)(e, " ").length;
    }(x), function (e) {
      return (0, _String.split)(e, " ").length;
    }(y));
  }, (0, _List.ofArray)(["int", "long long", "long", "bool", "char", "unsigned", "unsigned int", "unsigned long int", "unsigned long long int", "long int", "long long int"])));
  var listOfDatatypeNames = exports.listOfDatatypeNames = {
    contents: listOfDatatypeNamesDefault
  };

  function restoreDefault() {
    listOfDatatypeNames.contents = listOfDatatypeNamesDefault;
  }

  var State = exports.State = function () {
    function State(caseName, fields) {
      _classCallCheck(this, State);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(State, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Parser.C.State",
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

  (0, _Symbol2.setType)("Parser.C.State", State);

  function parse($var134, $var135, $var136, $var137, $var138) {
    parse: while (true) {
      var state = $var134;
      var stop = $var135;
      var fail = $var136;
      var left = $var137;
      var right = $var138;

      if (state.Case === "FunctionArgs") {
        var matchValue = [left, right];

        if (stop(right)) {
          return [_Token.Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
        } else if (fail(right)) {
          return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
            throw new Error(x);
          })([left, right]);
        } else {
          var $var54 = void 0;

          var activePatternResult310 = _DatatypeFunction___(state, stop, fail, matchValue[0], matchValue[1]);

          if (activePatternResult310 != null) {
            $var54 = [0, activePatternResult310];
          } else {
            var activePatternResult311 = _CommaFunction___(state, stop, fail, matchValue[0], matchValue[1]);

            if (activePatternResult311 != null) {
              $var54 = [0, activePatternResult311];
            } else {
              var activePatternResult312 = _Transfer___(state, stop, fail, matchValue[0], matchValue[1]);

              if (activePatternResult312 != null) {
                $var54 = [0, activePatternResult312];
              } else {
                $var54 = [1];
              }
            }
          }

          switch ($var54[0]) {
            case 0:
              return $var54[1];

            case 1:
              return (0, _String.fsFormat)("unknown: %A")(function (x) {
                throw new Error(x);
              })([left, right]);
          }
        }
      } else if (state.Case === "Local") {
        var matchValue_1 = [left, right];

        if (stop(right)) {
          return [_Token.Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
        } else if (fail(right)) {
          return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
            throw new Error(x);
          })([left, right]);
        } else {
          var $var55 = void 0;

          if (matchValue_1[0].tail != null) {
            var activePatternResult341 = _Token.Token["|T|_|"](matchValue_1[0].head);

            if (activePatternResult341 != null) {
              if (activePatternResult341 === ";") {
                if (matchValue_1[1].tail != null) {
                  $var55 = [0, matchValue_1[1].head, matchValue_1[1].tail];
                } else {
                  $var55 = [1];
                }
              } else {
                $var55 = [1];
              }
            } else {
              $var55 = [1];
            }
          } else {
            $var55 = [1];
          }

          switch ($var55[0]) {
            case 0:
              $var134 = new State("Local", []);
              $var135 = stop;
              $var136 = fail;
              $var137 = new _List2.default($var55[1], left);
              $var138 = $var55[2];
              continue parse;

            case 1:
              var $var56 = void 0;

              var activePatternResult327 = _DatatypeLocal___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

              if (activePatternResult327 != null) {
                $var56 = [0, activePatternResult327];
              } else {
                var activePatternResult328 = _Brackets___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                if (activePatternResult328 != null) {
                  $var56 = [0, activePatternResult328];
                } else {
                  var activePatternResult329 = _Braces___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                  if (activePatternResult329 != null) {
                    $var56 = [0, activePatternResult329];
                  } else {
                    var activePatternResult330 = _If___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                    if (activePatternResult330 != null) {
                      $var56 = [0, activePatternResult330];
                    } else {
                      var activePatternResult331 = _While___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                      if (activePatternResult331 != null) {
                        $var56 = [0, activePatternResult331];
                      } else {
                        var activePatternResult332 = _For___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                        if (activePatternResult332 != null) {
                          $var56 = [0, activePatternResult332];
                        } else {
                          var activePatternResult333 = _Return___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                          if (activePatternResult333 != null) {
                            $var56 = [0, activePatternResult333];
                          } else {
                            var activePatternResult334 = _Assignment___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                            if (activePatternResult334 != null) {
                              $var56 = [0, activePatternResult334];
                            } else {
                              var activePatternResult335 = _Dot___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                              if (activePatternResult335 != null) {
                                $var56 = [0, activePatternResult335];
                              } else {
                                var activePatternResult336 = _Prefix___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                if (activePatternResult336 != null) {
                                  $var56 = [0, activePatternResult336];
                                } else {
                                  var activePatternResult337 = _Operator___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                  if (activePatternResult337 != null) {
                                    $var56 = [0, activePatternResult337];
                                  } else {
                                    var activePatternResult338 = _Apply___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                    if (activePatternResult338 != null) {
                                      $var56 = [0, activePatternResult338];
                                    } else {
                                      var activePatternResult339 = _Index___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                      if (activePatternResult339 != null) {
                                        $var56 = [0, activePatternResult339];
                                      } else {
                                        var activePatternResult340 = _Transfer___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                        if (activePatternResult340 != null) {
                                          $var56 = [0, activePatternResult340];
                                        } else {
                                          $var56 = [1];
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

              switch ($var56[0]) {
                case 0:
                  return $var56[1];

                case 1:
                  return (0, _String.fsFormat)("unknown: %A")(function (x) {
                    throw new Error(x);
                  })([left, right]);
              }

          }
        }
      } else if (state.Case === "LocalImd") {
        var matchValue_2 = [left, right];

        if (stop(right)) {
          return [_Token.Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
        } else if (fail(right)) {
          return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
            throw new Error(x);
          })([left, right]);
        } else {
          var $var57 = void 0;

          if (matchValue_2[0].tail != null) {
            var activePatternResult360 = _Token.Token["|T|_|"](matchValue_2[0].head);

            if (activePatternResult360 != null) {
              if (activePatternResult360 === ";") {
                if (matchValue_2[1].tail != null) {
                  $var57 = [0, matchValue_2[1].head, matchValue_2[1].tail];
                } else {
                  $var57 = [1];
                }
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
              $var134 = new State("Local", []);
              $var135 = stop;
              $var136 = fail;
              $var137 = new _List2.default($var57[1], left);
              $var138 = $var57[2];
              continue parse;

            case 1:
              var $var58 = void 0;

              var activePatternResult351 = _Brackets___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

              if (activePatternResult351 != null) {
                $var58 = [0, activePatternResult351];
              } else {
                var activePatternResult352 = _Assignment___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                if (activePatternResult352 != null) {
                  $var58 = [0, activePatternResult352];
                } else {
                  var activePatternResult353 = _Dot___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                  if (activePatternResult353 != null) {
                    $var58 = [0, activePatternResult353];
                  } else {
                    var activePatternResult354 = _Prefix___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                    if (activePatternResult354 != null) {
                      $var58 = [0, activePatternResult354];
                    } else {
                      var activePatternResult355 = _Operator___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                      if (activePatternResult355 != null) {
                        $var58 = [0, activePatternResult355];
                      } else {
                        var activePatternResult356 = _Apply___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                        if (activePatternResult356 != null) {
                          $var58 = [0, activePatternResult356];
                        } else {
                          var activePatternResult357 = _Index___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                          if (activePatternResult357 != null) {
                            $var58 = [0, activePatternResult357];
                          } else {
                            var activePatternResult358 = _CommaFunction___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                            if (activePatternResult358 != null) {
                              $var58 = [0, activePatternResult358];
                            } else {
                              var activePatternResult359 = _Transfer___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                              if (activePatternResult359 != null) {
                                $var58 = [0, activePatternResult359];
                              } else {
                                $var58 = [1];
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }

              switch ($var58[0]) {
                case 0:
                  return $var58[1];

                case 1:
                  return (0, _String.fsFormat)("unknown: %A")(function (x) {
                    throw new Error(x);
                  })([left, right]);
              }

          }
        }
      } else {
        var matchValue_3 = [left, right];

        if (stop(right)) {
          return [_Token.Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
        } else if (fail(right)) {
          return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
            throw new Error(x);
          })([left, right]);
        } else {
          var $var59 = void 0;

          var activePatternResult298 = _DatatypeGlobal___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

          if (activePatternResult298 != null) {
            $var59 = [0, activePatternResult298];
          } else {
            var activePatternResult299 = _Struct___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

            if (activePatternResult299 != null) {
              $var59 = [0, activePatternResult299];
            } else {
              var activePatternResult300 = _Apply___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

              if (activePatternResult300 != null) {
                $var59 = [0, activePatternResult300];
              } else {
                var activePatternResult301 = _Index___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                if (activePatternResult301 != null) {
                  $var59 = [0, activePatternResult301];
                } else {
                  var activePatternResult302 = _Brackets___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                  if (activePatternResult302 != null) {
                    $var59 = [0, activePatternResult302];
                  } else {
                    var activePatternResult303 = _Braces___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                    if (activePatternResult303 != null) {
                      $var59 = [0, activePatternResult303];
                    } else {
                      var activePatternResult304 = _Assignment___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                      if (activePatternResult304 != null) {
                        $var59 = [0, activePatternResult304];
                      } else {
                        var activePatternResult305 = _Operator___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                        if (activePatternResult305 != null) {
                          $var59 = [0, activePatternResult305];
                        } else {
                          var activePatternResult306 = _Transfer___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                          if (activePatternResult306 != null) {
                            $var59 = [0, activePatternResult306];
                          } else {
                            $var59 = [1];
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }

          switch ($var59[0]) {
            case 0:
              return $var59[1];

            case 1:
              return (0, _String.fsFormat)("unknown: %A")(function (x) {
                throw new Error(x);
              })([left, right]);
          }
        }
      }
    }
  }

  function matchString(tokens, stringToStringList, s) {
    var findMatch = function findMatch(_arg1) {
      findMatch: while (true) {
        var $var60 = void 0;

        if (_arg1[0].tail != null) {
          if (_arg1[1].tail != null) {
            (function () {
              var activePatternResult366 = _Token.Token["|T|_|"](_arg1[1].head);

              if (activePatternResult366 != null) {
                if (function () {
                  var restb = _arg1[1].tail;
                  var resta = _arg1[0].tail;
                  var a = _arg1[0].head;
                  return a === activePatternResult366;
                }()) {
                  $var60 = [1, _arg1[0].head, activePatternResult366, _arg1[0].tail, _arg1[1].tail];
                } else {
                  $var60 = [2];
                }
              } else {
                $var60 = [2];
              }
            })();
          } else {
            $var60 = [2];
          }
        } else {
          $var60 = [0, _arg1[1]];
        }

        switch ($var60[0]) {
          case 0:
            return [s, $var60[1]];

          case 1:
            _arg1 = [$var60[3], $var60[4]];
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

    var activePatternResult389 = _DatatypeName___(_arg2[1]);

    if (activePatternResult389 != null) {
      var patternInput = parse(new State("Global", []), function (_arg3) {
        var $var61 = void 0;

        if (_arg3.tail != null) {
          var activePatternResult374 = _Token.Token["|T|_|"](_arg3.head);

          if (activePatternResult374 != null) {
            if (activePatternResult374 === ";") {
              $var61 = [0];
            } else if (activePatternResult374 === "{") {
              $var61 = [0];
            } else if (activePatternResult374 === ",") {
              $var61 = [0];
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
            return true;

          case 1:
            return false;
        }
      }, function (e) {
        return stop(e) ? true : fail(e);
      }, new _List2.default(), activePatternResult389[1]);
      var patternInput_2 = void 0;
      var matchValue = patternInput[0].Clean();

      var activePatternResult385 = _Token.Token["|T|_|"](matchValue);

      if (activePatternResult385 != null) {
        patternInput_2 = [_Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult389[0]), patternInput[0]])), _Token.Token.Token[".ctor_3"]("nothing")])), patternInput[1]];
      } else {
        var $var62 = void 0;

        var activePatternResult383 = _Token.Token["|X|"](matchValue);

        if (activePatternResult383[0] === "assign") {
          if (activePatternResult383[1].tail != null) {
            if (activePatternResult383[1].tail.tail != null) {
              if (activePatternResult383[1].tail.tail.tail == null) {
                $var62 = [0, activePatternResult383[1].head, activePatternResult383[1].tail.head];
              } else {
                $var62 = [3, matchValue];
              }
            } else {
              $var62 = [3, matchValue];
            }
          } else {
            $var62 = [3, matchValue];
          }
        } else if (activePatternResult383[0] === "dot") {
          if (activePatternResult383[1].tail != null) {
            if (activePatternResult383[1].tail.tail != null) {
              var activePatternResult384 = _Token.Token["|X|"](activePatternResult383[1].tail.head);

              if (activePatternResult384[0] === "[]") {
                if (activePatternResult383[1].tail.tail.tail == null) {
                  $var62 = [1, activePatternResult384[1], activePatternResult383[1].head];
                } else {
                  $var62 = [3, matchValue];
                }
              } else {
                $var62 = [3, matchValue];
              }
            } else {
              $var62 = [3, matchValue];
            }
          } else {
            $var62 = [3, matchValue];
          }
        } else if (activePatternResult383[0] === "apply") {
          if (activePatternResult383[1].tail != null) {
            if (activePatternResult383[1].tail.tail != null) {
              if (activePatternResult383[1].tail.tail.tail == null) {
                $var62 = [2, activePatternResult383[1].tail.head, activePatternResult383[1].head];
              } else {
                $var62 = [3, matchValue];
              }
            } else {
              $var62 = [3, matchValue];
            }
          } else {
            $var62 = [3, matchValue];
          }
        } else {
          $var62 = [3, matchValue];
        }

        switch ($var62[0]) {
          case 0:
            patternInput_2 = [_Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult389[0]), $var62[1]])), $var62[2]])), patternInput[1]];
            break;

          case 1:
            patternInput_2 = [_Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult389[0]), $var62[2]])), _Token.Token.Token[".ctor_2"]("array", $var62[1])])), patternInput[1]];
            break;

          case 2:
            var patternInput_1 = parse(new State("Local", []), function (_arg4) {
              var $var63 = void 0;

              if (_arg4.tail != null) {
                var activePatternResult377 = _Token.Token["|X|"](_arg4.head);

                if (activePatternResult377[0] === "{}") {
                  $var63 = [0];
                } else {
                  $var63 = [1];
                }
              } else {
                $var63 = [1];
              }

              switch ($var63[0]) {
                case 0:
                  return true;

                case 1:
                  return false;
              }
            }, function (e_1) {
              return stop(e_1) ? true : fail(e_1);
            }, new _List2.default(), patternInput[1]);
            var $var64 = void 0;

            var activePatternResult380 = _Token.Token["|X|"](patternInput_1[0]);

            if (activePatternResult380[0] === "sequence") {
              if (activePatternResult380[1].tail == null) {
                if (patternInput_1[1].tail != null) {
                  $var64 = [0, patternInput_1[1].head, patternInput_1[1].tail];
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
                patternInput_2 = [_Token.Token.Token[".ctor_2"]("declare function", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult389[0]), $var62[2], $var62[1], $var64[1]])), $var64[2]];
                break;

              case 1:
                throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 144, 14);
                break;
            }

            break;

          case 3:
            patternInput_2 = (0, _String.fsFormat)("expression following data type declaration is invalid %O")(function (x) {
              throw new Error(x);
            })($var62[1]);
            break;
        }
      }

      var restr = void 0;
      var $var65 = void 0;

      if (patternInput_2[1].tail != null) {
        var activePatternResult388 = _Token.Token["|T|_|"](patternInput_2[1].head);

        if (activePatternResult388 != null) {
          if (activePatternResult388 === ",") {
            $var65 = [0, patternInput_2[1].tail];
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
          restr = (0, _List.append)(new _List2.default(_Token.Token.Token[".ctor_3"](";"), (0, _List.ofArray)((0, _String.split)(activePatternResult389[0], " ").map(function (e_2) {
            return _Token.Token.Token[".ctor_3"](e_2);
          }))), $var65[1]);
          break;

        case 1:
          var $var66 = void 0;

          if (patternInput_2[1].tail != null) {
            var activePatternResult387 = _Token.Token["|T|_|"](patternInput_2[1].head);

            if (activePatternResult387 != null) {
              if (activePatternResult387 === ";") {
                $var66 = [0, patternInput_2[1].tail];
              } else {
                $var66 = [0, patternInput_2[1]];
              }
            } else {
              $var66 = [0, patternInput_2[1]];
            }
          } else {
            $var66 = [0, patternInput_2[1]];
          }

          switch ($var66[0]) {
            case 0:
              restr = $var66[1];
              break;
          }

          break;
      }

      return parse(new State("Global", []), stop, fail, _arg2[0], new _List2.default(patternInput_2[0], restr));
    } else {
      return null;
    }
  }

  exports.$7C$DatatypeGlobal$7C$_$7C$ = _DatatypeGlobal___;

  function _Struct___(state, stop, fail, _arg5_0, _arg5_1) {
    var _arg5 = [_arg5_0, _arg5_1];
    var $var67 = void 0;

    if (_arg5[0].tail != null) {
      var activePatternResult402 = _Token.Token["|T|_|"](_arg5[0].head);

      if (activePatternResult402 != null) {
        if (activePatternResult402 === "struct") {
          $var67 = [0, _arg5[0].tail, _arg5[1]];
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
        var patternInput = void 0;
        var $var68 = void 0;

        if ($var67[2].tail != null) {
          var activePatternResult396 = _Token.Token["|T|_|"]($var67[2].head);

          if (activePatternResult396 != null) {
            if (activePatternResult396 === "{") {
              $var68 = [0, $var67[2].tail];
            } else {
              $var68 = [1];
            }
          } else {
            $var68 = [1];
          }
        } else {
          $var68 = [1];
        }

        switch ($var68[0]) {
          case 0:
            patternInput = ["anonymousStruct", $var68[1]];
            break;

          case 1:
            var $var69 = void 0;

            if ($var67[2].tail != null) {
              var activePatternResult394 = _Token.Token["|T|_|"]($var67[2].head);

              if (activePatternResult394 != null) {
                if ($var67[2].tail.tail != null) {
                  var activePatternResult395 = _Token.Token["|T|_|"]($var67[2].tail.head);

                  if (activePatternResult395 != null) {
                    if (activePatternResult395 === "{") {
                      $var69 = [0, $var67[2].tail.tail, activePatternResult394];
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
            } else {
              $var69 = [1];
            }

            switch ($var69[0]) {
              case 0:
                patternInput = [$var69[2], $var69[1]];
                break;

              case 1:
                patternInput = (0, _String.fsFormat)("not a valid struct declaration: %A")(function (x) {
                  throw new Error(x);
                })($var67[2]);
                break;
            }

            break;
        }

        listOfDatatypeNames.contents = new _List2.default("struct " + patternInput[0], listOfDatatypeNames.contents);
        (0, _String.fsFormat)("datatype names: %A")(function (x) {
          console.log(x);
        })(listOfDatatypeNames.contents);
        var patternInput_1 = parse(new State("Local", []), function (_arg6) {
          var $var70 = void 0;

          if (_arg6.tail != null) {
            var activePatternResult397 = _Token.Token["|T|_|"](_arg6.head);

            if (activePatternResult397 != null) {
              if (activePatternResult397 === "}") {
                $var70 = [0];
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
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), patternInput[1]);
        var $var71 = void 0;

        if (patternInput_1[1].tail != null) {
          var activePatternResult401 = _Token.Token["|T|_|"](patternInput_1[1].head);

          if (activePatternResult401 != null) {
            if (activePatternResult401 === "}") {
              $var71 = [0, patternInput_1[0], patternInput_1[1].tail];
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
            var restr = void 0;
            var $var72 = void 0;

            if ($var71[2].tail != null) {
              var activePatternResult400 = _Token.Token["|T|_|"]($var71[2].head);

              if (activePatternResult400 != null) {
                if (activePatternResult400 === ";") {
                  $var72 = [0, $var71[2].tail];
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
                restr = $var72[1];
                break;

              case 1:
                restr = (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("struct"), _Token.Token.Token[".ctor_3"](patternInput[0])], $var71[2]);
                break;
            }

            var parsed = _Token.Token.Token[".ctor_2"]("struct", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](patternInput[0]), $var71[1]]));

            return parse(state, stop, fail, $var67[1], new _List2.default(parsed, restr));

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 163, 10);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$Struct$7C$_$7C$ = _Struct___;

  function _DatatypeFunction___(state, stop, fail, _arg7_0, _arg7_1) {
    var _arg7 = [_arg7_0, _arg7_1];
    var $var73 = void 0;

    var activePatternResult408 = _DatatypeName___(_arg7[1]);

    if (activePatternResult408 != null) {
      if (activePatternResult408[1].tail != null) {
        var activePatternResult409 = _Token.Token["|Pref|_|"](activePatternResult408[1].head);

        if (activePatternResult409 != null) {
          var activePatternResult410 = _Token.Token["|T|_|"](activePatternResult409);

          if (activePatternResult410 != null) {
            if (activePatternResult408[1].tail.tail != null) {
              $var73 = [0, activePatternResult408[0], activePatternResult408[1].tail.head, _arg7[0], activePatternResult408[1].tail.tail, activePatternResult410];
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
        var parsed = _Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var73[1] + $var73[5].slice(1, $var73[5].length)), $var73[2]]));

        return parse(new State("FunctionArgs", []), stop, fail, $var73[3], new _List2.default(parsed, $var73[4]));

      case 1:
        var $var74 = void 0;

        var activePatternResult407 = _DatatypeName___(_arg7[1]);

        if (activePatternResult407 != null) {
          if (activePatternResult407[1].tail != null) {
            $var74 = [0, activePatternResult407[0], activePatternResult407[1].head, _arg7[0], activePatternResult407[1].tail];
          } else {
            $var74 = [1];
          }
        } else {
          $var74 = [1];
        }

        switch ($var74[0]) {
          case 0:
            var parsed_1 = _Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var74[1]), $var74[2]]));

            return parse(new State("FunctionArgs", []), stop, fail, $var74[3], new _List2.default(parsed_1, $var74[4]));

          case 1:
            return null;
        }

    }
  }

  exports.$7C$DatatypeFunction$7C$_$7C$ = _DatatypeFunction___;

  function _CommaFunction___(state, stop, fail, _arg8_0, _arg8_1) {
    var _arg8 = [_arg8_0, _arg8_1];
    var $var75 = void 0;

    if (_arg8[0].tail != null) {
      if (_arg8[1].tail != null) {
        var activePatternResult417 = _Token.Token["|T|_|"](_arg8[1].head);

        if (activePatternResult417 != null) {
          if (activePatternResult417 === ",") {
            $var75 = [0, _arg8[0].head, _arg8[0].tail, _arg8[1].tail];
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
        var patternInput = parse(state, stop, fail, new _List2.default(), $var75[3]);
        var parsed = void 0;
        var $var76 = void 0;

        var activePatternResult415 = _Token.Token["|X|"](patternInput[0]);

        if (activePatternResult415[0] === "sequence") {
          if (activePatternResult415[1].tail != null) {
            var activePatternResult416 = _Token.Token["|X|"](activePatternResult415[1].head);

            if (activePatternResult416[0] === ",") {
              if (activePatternResult415[1].tail.tail == null) {
                $var76 = [0, activePatternResult416[1]];
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
            parsed = _Token.Token.Token[".ctor_2"](",", new _List2.default($var75[1], $var76[1]));
            break;

          case 1:
            parsed = _Token.Token.Token[".ctor_2"](",", (0, _List.ofArray)([$var75[1], patternInput[0]]));
            break;
        }

        return parse(state, stop, fail, $var75[2], new _List2.default(parsed, patternInput[1]));

      case 1:
        return null;
    }
  }

  exports.$7C$CommaFunction$7C$_$7C$ = _CommaFunction___;

  function _DatatypeNameL___(left) {
    return (0, _Seq.tryPick)(function () {
      var stringToStringList = function stringToStringList(e) {
        return (0, _List.reverse)((0, _List.ofArray)((0, _String.split)(e, " ")));
      };

      return function (s) {
        return matchString(left, stringToStringList, s);
      };
    }(), listOfDatatypeNames.contents);
  }

  exports.$7C$DatatypeNameL$7C$_$7C$ = _DatatypeNameL___;

  function _DatatypeLocal___(state, stop, fail, _arg9_0, _arg9_1) {
    var _arg9 = [_arg9_0, _arg9_1];

    var activePatternResult453 = _DatatypeNameL___(_arg9[0]);

    if (activePatternResult453 != null) {
      var patternInput = parse(new State("LocalImd", []), function (_arg10) {
        var $var77 = void 0;

        if (_arg10.tail != null) {
          var activePatternResult424 = _Token.Token["|T|_|"](_arg10.head);

          if (activePatternResult424 != null) {
            if (activePatternResult424 === ";") {
              $var77 = [0];
            } else if (activePatternResult424 === ",") {
              $var77 = [0];
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
            return true;

          case 1:
            return false;
        }
      }, function (e) {
        return stop(e) ? true : fail(e);
      }, new _List2.default(), _arg9[1]);
      var parsed = void 0;
      var pars = void 0;
      var $var78 = void 0;

      var activePatternResult427 = _Token.Token["|X|"](patternInput[0]);

      if (activePatternResult427[0] === "sequence") {
        if (activePatternResult427[1].tail != null) {
          if (activePatternResult427[1].tail.tail == null) {
            $var78 = [0, activePatternResult427[1].head];
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
          pars = $var78[1];
          break;

        case 1:
          throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 196, 25);
          break;
      }

      var $var79 = void 0;

      var activePatternResult450 = _Token.Token["|X|"](pars);

      if (activePatternResult450[0] === "assign") {
        if (activePatternResult450[1].tail != null) {
          var activePatternResult451 = _Token.Token["|T|_|"](activePatternResult450[1].head);

          if (activePatternResult451 != null) {
            if (activePatternResult450[1].tail.tail != null) {
              if (activePatternResult450[1].tail.tail.tail == null) {
                $var79 = [0, activePatternResult451, activePatternResult450[1].head, activePatternResult450[1].tail.head];
              } else {
                $var79 = [1];
              }
            } else {
              $var79 = [1];
            }
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
          parsed = _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult453[0]), $var79[2]])), $var79[3]]));
          break;

        case 1:
          var activePatternResult449 = _Token.Token["|T|_|"](pars);

          if (activePatternResult449 != null) {
            parsed = _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult453[0]), pars])), _Token.Token.Token[".ctor_3"]("nothing")]));
          } else {
            var $var80 = void 0;

            var activePatternResult446 = _Token.Token["|X|"](pars);

            if (activePatternResult446[0] === "dot") {
              if (activePatternResult446[1].tail != null) {
                var activePatternResult447 = _Token.Token["|T|_|"](activePatternResult446[1].head);

                if (activePatternResult447 != null) {
                  if (activePatternResult446[1].tail.tail != null) {
                    var activePatternResult448 = _Token.Token["|X|"](activePatternResult446[1].tail.head);

                    if (activePatternResult448[0] === "[]") {
                      if (activePatternResult446[1].tail.tail.tail == null) {
                        $var80 = [0, activePatternResult448[1], activePatternResult447, activePatternResult446[1].head];
                      } else {
                        $var80 = [1];
                      }
                    } else {
                      $var80 = [1];
                    }
                  } else {
                    $var80 = [1];
                  }
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
                parsed = _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult453[0]), $var80[3]])), _Token.Token.Token[".ctor_2"]("array", $var80[1])]));
                break;

              case 1:
                var $var81 = void 0;

                var activePatternResult441 = _Token.Token["|X|"](pars);

                if (activePatternResult441[0] === "assign") {
                  if (activePatternResult441[1].tail != null) {
                    var activePatternResult442 = _Token.Token["|X|"](activePatternResult441[1].head);

                    if (activePatternResult442[0] === "apply") {
                      if (activePatternResult442[1].tail != null) {
                        var activePatternResult443 = _Token.Token["|Pref'|_|"](activePatternResult442[1].head);

                        if (activePatternResult443 != null) {
                          var activePatternResult444 = _Token.Token["|T|_|"](activePatternResult443);

                          if (activePatternResult444 != null) {
                            if (activePatternResult444 === "~*") {
                              if (activePatternResult442[1].tail.tail != null) {
                                var activePatternResult445 = _Token.Token["|T|_|"](activePatternResult442[1].tail.head);

                                if (activePatternResult445 != null) {
                                  if (activePatternResult442[1].tail.tail.tail == null) {
                                    if (activePatternResult441[1].tail.tail != null) {
                                      if (activePatternResult441[1].tail.tail.tail == null) {
                                        $var81 = [0, activePatternResult442[1].tail.head, activePatternResult441[1].tail.head];
                                      } else {
                                        $var81 = [1];
                                      }
                                    } else {
                                      $var81 = [1];
                                    }
                                  } else {
                                    $var81 = [1];
                                  }
                                } else {
                                  $var81 = [1];
                                }
                              } else {
                                $var81 = [1];
                              }
                            } else {
                              $var81 = [1];
                            }
                          } else {
                            $var81 = [1];
                          }
                        } else {
                          $var81 = [1];
                        }
                      } else {
                        $var81 = [1];
                      }
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
                    parsed = _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult453[0] + "*"), $var81[1]])), $var81[2]]));
                    break;

                  case 1:
                    var $var82 = void 0;

                    var activePatternResult437 = _Token.Token["|X|"](pars);

                    if (activePatternResult437[0] === "apply") {
                      if (activePatternResult437[1].tail != null) {
                        var activePatternResult438 = _Token.Token["|Pref'|_|"](activePatternResult437[1].head);

                        if (activePatternResult438 != null) {
                          var activePatternResult439 = _Token.Token["|T|_|"](activePatternResult438);

                          if (activePatternResult439 != null) {
                            if (activePatternResult439 === "~*") {
                              if (activePatternResult437[1].tail.tail != null) {
                                var activePatternResult440 = _Token.Token["|T|_|"](activePatternResult437[1].tail.head);

                                if (activePatternResult440 != null) {
                                  if (activePatternResult437[1].tail.tail.tail == null) {
                                    $var82 = [0, activePatternResult437[1].tail.head];
                                  } else {
                                    $var82 = [1];
                                  }
                                } else {
                                  $var82 = [1];
                                }
                              } else {
                                $var82 = [1];
                              }
                            } else {
                              $var82 = [1];
                            }
                          } else {
                            $var82 = [1];
                          }
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
                        parsed = _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult453[0] + "*"), $var82[1]])), _Token.Token.Token[".ctor_3"]("nothing")]));
                        break;

                      case 1:
                        var $var83 = void 0;

                        var activePatternResult431 = _Token.Token["|X|"](pars);

                        if (activePatternResult431[0] === "dot") {
                          if (activePatternResult431[1].tail != null) {
                            var activePatternResult432 = _Token.Token["|X|"](activePatternResult431[1].head);

                            if (activePatternResult432[0] === "apply") {
                              if (activePatternResult432[1].tail != null) {
                                var activePatternResult433 = _Token.Token["|Pref'|_|"](activePatternResult432[1].head);

                                if (activePatternResult433 != null) {
                                  var activePatternResult434 = _Token.Token["|T|_|"](activePatternResult433);

                                  if (activePatternResult434 != null) {
                                    if (activePatternResult434 === "~*") {
                                      if (activePatternResult432[1].tail.tail != null) {
                                        var activePatternResult435 = _Token.Token["|T|_|"](activePatternResult432[1].tail.head);

                                        if (activePatternResult435 != null) {
                                          if (activePatternResult432[1].tail.tail.tail == null) {
                                            if (activePatternResult431[1].tail.tail != null) {
                                              var activePatternResult436 = _Token.Token["|X|"](activePatternResult431[1].tail.head);

                                              if (activePatternResult436[0] === "[]") {
                                                if (activePatternResult431[1].tail.tail.tail == null) {
                                                  $var83 = [0, activePatternResult436[1], activePatternResult432[1].tail.head];
                                                } else {
                                                  $var83 = [1];
                                                }
                                              } else {
                                                $var83 = [1];
                                              }
                                            } else {
                                              $var83 = [1];
                                            }
                                          } else {
                                            $var83 = [1];
                                          }
                                        } else {
                                          $var83 = [1];
                                        }
                                      } else {
                                        $var83 = [1];
                                      }
                                    } else {
                                      $var83 = [1];
                                    }
                                  } else {
                                    $var83 = [1];
                                  }
                                } else {
                                  $var83 = [1];
                                }
                              } else {
                                $var83 = [1];
                              }
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
                            parsed = _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult453[0] + "*"), $var83[2]])), _Token.Token.Token[".ctor_2"]("array", $var83[1])]));
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
      var $var84 = void 0;

      if (patternInput[1].tail != null) {
        var activePatternResult452 = _Token.Token["|T|_|"](patternInput[1].head);

        if (activePatternResult452 != null) {
          if (activePatternResult452 === ",") {
            $var84 = [0, patternInput[1].tail];
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
          restr = (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](";"), _Token.Token.Token[".ctor_3"](activePatternResult453[0])], $var84[1]);
          break;

        case 1:
          restr = patternInput[1];
          break;
      }

      return parse(new State("LocalImd", []), stop, fail, activePatternResult453[1], new _List2.default(parsed, restr));
    } else {
      return null;
    }
  }

  exports.$7C$DatatypeLocal$7C$_$7C$ = _DatatypeLocal___;

  function _Brackets___(state, stop, fail, _arg11_0, _arg11_1) {
    var _arg11 = [_arg11_0, _arg11_1];
    var $var85 = void 0;

    if (_arg11[0].tail != null) {
      var activePatternResult462 = _Token.Token["|T|_|"](_arg11[0].head);

      if (activePatternResult462 != null) {
        if (activePatternResult462 === "(") {
          $var85 = [0, _arg11[0].tail, _arg11[1]];
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
        var patternInput = parse(new State("LocalImd", []), function (_arg12) {
          var $var86 = void 0;

          if (_arg12.tail != null) {
            var activePatternResult458 = _Token.Token["|T|_|"](_arg12.head);

            if (activePatternResult458 != null) {
              if (activePatternResult458 === ")") {
                $var86 = [0];
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
              return true;

            case 1:
              return false;
          }
        }, function (_arg1) {
          return false;
        }, new _List2.default(), $var85[2]);
        var $var87 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult461 = _Token.Token["|T|_|"](patternInput[1].head);

          if (activePatternResult461 != null) {
            if (activePatternResult461 === ")") {
              $var87 = [0, patternInput[0], patternInput[1].tail];
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
            var parsed = _Token.Token.Token[".ctor_2"]("()", (0, _List.ofArray)([$var87[1]]));

            return parse(new State("LocalImd", []), stop, fail, $var85[1], new _List2.default(parsed, $var87[2]));

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 220, 10);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$Brackets$7C$_$7C$ = _Brackets___;

  function _Braces___(state, stop, fail, _arg13_0, _arg13_1) {
    var _arg13 = [_arg13_0, _arg13_1];
    var $var88 = void 0;

    if (_arg13[0].tail != null) {
      var activePatternResult471 = _Token.Token["|T|_|"](_arg13[0].head);

      if (activePatternResult471 != null) {
        if (activePatternResult471 === "{") {
          $var88 = [0, _arg13[0].tail, _arg13[1]];
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
        var patternInput = parse(new State("Local", []), function (_arg14) {
          var $var89 = void 0;

          if (_arg14.tail != null) {
            var activePatternResult467 = _Token.Token["|T|_|"](_arg14.head);

            if (activePatternResult467 != null) {
              if (activePatternResult467 === "}") {
                $var89 = [0];
              } else {
                $var89 = [1];
              }
            } else {
              $var89 = [1];
            }
          } else {
            $var89 = [1];
          }

          switch ($var89[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var88[2]);
        var $var90 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult470 = _Token.Token["|T|_|"](patternInput[1].head);

          if (activePatternResult470 != null) {
            if (activePatternResult470 === "}") {
              $var90 = [0, patternInput[0], patternInput[1].tail];
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
            var parsed = _Token.Token.Token[".ctor_2"]("{}", (0, _List.ofArray)([$var90[1]]));

            return parse(new State("Local", []), stop, fail, $var88[1], new _List2.default(parsed, $var90[2]));

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 227, 10);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$Braces$7C$_$7C$ = _Braces___;

  function getStatementBody(stop, fail, right) {
    var $var91 = void 0;

    if (right.tail != null) {
      var activePatternResult484 = _Token.Token["|T|_|"](right.head);

      if (activePatternResult484 != null) {
        if (activePatternResult484 === "{") {
          $var91 = [0];
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
        var patternInput = parse(new State("Local", []), function (_arg15) {
          var $var92 = void 0;

          if (_arg15.tail != null) {
            var activePatternResult476 = _Token.Token["|X|"](_arg15.head);

            if (activePatternResult476[0] === "{}") {
              if (activePatternResult476[1].tail != null) {
                if (activePatternResult476[1].tail.tail == null) {
                  $var92 = [0];
                } else {
                  $var92 = [1];
                }
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
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), right);
        var $var93 = void 0;

        var activePatternResult479 = _Token.Token["|X|"](patternInput[0]);

        if (activePatternResult479[0] === "sequence") {
          if (activePatternResult479[1].tail == null) {
            if (patternInput[1].tail != null) {
              var activePatternResult480 = _Token.Token["|X|"](patternInput[1].head);

              if (activePatternResult480[0] === "{}") {
                if (activePatternResult480[1].tail != null) {
                  if (activePatternResult480[1].tail.tail == null) {
                    $var93 = [0, activePatternResult480[1].head, patternInput[1].tail];
                  } else {
                    $var93 = [1];
                  }
                } else {
                  $var93 = [1];
                }
              } else {
                $var93 = [1];
              }
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
            return [$var93[1], new _List2.default(_Token.Token.Token[".ctor_3"](";"), $var93[2])];

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 235, 10);
        }

      case 1:
        var patternInput_1 = parse(new State("Local", []), function (_arg16) {
          var $var94 = void 0;

          if (_arg16.tail != null) {
            var activePatternResult481 = _Token.Token["|T|_|"](_arg16.head);

            if (activePatternResult481 != null) {
              if (activePatternResult481 === ";") {
                $var94 = [0];
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
              return true;

            case 1:
              return false;
          }
        }, function (e_1) {
          return stop(e_1) ? true : fail(e_1);
        }, new _List2.default(), right);
        return [patternInput_1[0], patternInput_1[1]];
    }
  }

  function _If___(state, stop, fail, _arg17_0, _arg17_1) {
    var _arg17 = [_arg17_0, _arg17_1];
    var $var95 = void 0;

    if (_arg17[0].tail != null) {
      var activePatternResult496 = _Token.Token["|T|_|"](_arg17[0].head);

      if (activePatternResult496 != null) {
        if (activePatternResult496 === "if") {
          if (_arg17[1].tail != null) {
            var activePatternResult497 = _Token.Token["|T|_|"](_arg17[1].head);

            if (activePatternResult497 != null) {
              if (activePatternResult497 === "(") {
                $var95 = [0, _arg17[0].tail, _arg17[1]];
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
        var patternInput = parse(new State("LocalImd", []), function (_arg18) {
          var $var96 = void 0;

          if (_arg18.tail != null) {
            var activePatternResult488 = _Token.Token["|X|"](_arg18.head);

            if (activePatternResult488[0] === "()") {
              $var96 = [0];
            } else {
              $var96 = [1];
            }
          } else {
            $var96 = [1];
          }

          switch ($var96[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var95[2]);
        var $var97 = void 0;

        var activePatternResult494 = _Token.Token["|X|"](patternInput[0]);

        if (activePatternResult494[0] === "sequence") {
          if (activePatternResult494[1].tail == null) {
            if (patternInput[1].tail != null) {
              var activePatternResult495 = _Token.Token["|X|"](patternInput[1].head);

              if (activePatternResult495[0] === "()") {
                if (activePatternResult495[1].tail != null) {
                  if (activePatternResult495[1].tail.tail == null) {
                    $var97 = [0, activePatternResult495[1].head, patternInput[1].tail];
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
            var patternInput_1 = getStatementBody(stop, fail, $var97[2]);
            var patternInput_2 = void 0;
            var matchValue = patternInput_1[1].tail;
            var $var98 = void 0;

            if (matchValue.tail != null) {
              var activePatternResult492 = _Token.Token["|T|_|"](matchValue.head);

              if (activePatternResult492 != null) {
                if (activePatternResult492 === "else") {
                  if (matchValue.tail.tail != null) {
                    var activePatternResult493 = _Token.Token["|T|_|"](matchValue.tail.head);

                    if (activePatternResult493 != null) {
                      if (activePatternResult493 === "if") {
                        $var98 = [0, matchValue.tail.tail];
                      } else {
                        $var98 = [1];
                      }
                    } else {
                      $var98 = [1];
                    }
                  } else {
                    $var98 = [1];
                  }
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
                patternInput_2 = parse(new State("Local", []), stop, fail, new _List2.default(), new _List2.default(_Token.Token.Token[".ctor_3"]("if"), $var98[1]));
                break;

              case 1:
                var $var99 = void 0;

                if (matchValue.tail != null) {
                  var activePatternResult491 = _Token.Token["|T|_|"](matchValue.head);

                  if (activePatternResult491 != null) {
                    if (activePatternResult491 === "else") {
                      $var99 = [0, matchValue.tail];
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
                    patternInput_2 = getStatementBody(stop, fail, $var99[1]);
                    break;

                  case 1:
                    patternInput_2 = [_Token.Token.Token[".ctor_2"]("sequence", new _List2.default()), patternInput_1[1]];
                    break;
                }

                break;
            }

            var parsed = _Token.Token.Token[".ctor_2"]("if", (0, _List.ofArray)([$var97[1], patternInput_1[0], patternInput_2[0]]));

            return parse(new State("Local", []), stop, fail, $var95[1], new _List2.default(parsed, patternInput_2[1]));

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 245, 10);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$If$7C$_$7C$ = _If___;

  function _While___(state, stop, fail, _arg19_0, _arg19_1) {
    var _arg19 = [_arg19_0, _arg19_1];
    var $var100 = void 0;

    if (_arg19[0].tail != null) {
      var activePatternResult507 = _Token.Token["|T|_|"](_arg19[0].head);

      if (activePatternResult507 != null) {
        if (activePatternResult507 === "while") {
          if (_arg19[1].tail != null) {
            var activePatternResult508 = _Token.Token["|T|_|"](_arg19[1].head);

            if (activePatternResult508 != null) {
              if (activePatternResult508 === "(") {
                $var100 = [0, _arg19[0].tail, _arg19[1]];
              } else {
                $var100 = [1];
              }
            } else {
              $var100 = [1];
            }
          } else {
            $var100 = [1];
          }
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
        var patternInput = parse(new State("LocalImd", []), function (_arg20) {
          var $var101 = void 0;

          if (_arg20.tail != null) {
            var activePatternResult502 = _Token.Token["|X|"](_arg20.head);

            if (activePatternResult502[0] === "()") {
              $var101 = [0];
            } else {
              $var101 = [1];
            }
          } else {
            $var101 = [1];
          }

          switch ($var101[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var100[2]);
        var $var102 = void 0;

        var activePatternResult505 = _Token.Token["|X|"](patternInput[0]);

        if (activePatternResult505[0] === "sequence") {
          if (activePatternResult505[1].tail == null) {
            if (patternInput[1].tail != null) {
              var activePatternResult506 = _Token.Token["|X|"](patternInput[1].head);

              if (activePatternResult506[0] === "()") {
                if (activePatternResult506[1].tail != null) {
                  if (activePatternResult506[1].tail.tail == null) {
                    $var102 = [0, activePatternResult506[1].head, patternInput[1].tail];
                  } else {
                    $var102 = [1];
                  }
                } else {
                  $var102 = [1];
                }
              } else {
                $var102 = [1];
              }
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
            var patternInput_1 = getStatementBody(stop, fail, $var102[2]);

            var parsed = _Token.Token.Token[".ctor_2"]("while", (0, _List.ofArray)([$var102[1], patternInput_1[0]]));

            return parse(new State("Local", []), stop, fail, $var100[1], new _List2.default(parsed, patternInput_1[1]));

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 258, 10);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$While$7C$_$7C$ = _While___;

  function _For___(state, stop, fail, _arg21_0, _arg21_1) {
    var _arg21 = [_arg21_0, _arg21_1];
    var $var103 = void 0;

    if (_arg21[0].tail != null) {
      var activePatternResult525 = _Token.Token["|T|_|"](_arg21[0].head);

      if (activePatternResult525 != null) {
        if (activePatternResult525 === "for") {
          if (_arg21[1].tail != null) {
            var activePatternResult526 = _Token.Token["|T|_|"](_arg21[1].head);

            if (activePatternResult526 != null) {
              if (activePatternResult526 === "(") {
                $var103 = [0, _arg21[0].tail, _arg21[1].tail];
              } else {
                $var103 = [1];
              }
            } else {
              $var103 = [1];
            }
          } else {
            $var103 = [1];
          }
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
        var patternInput = parse(new State("Local", []), function (_arg22) {
          var $var104 = void 0;

          if (_arg22.tail != null) {
            var activePatternResult513 = _Token.Token["|T|_|"](_arg22.head);

            if (activePatternResult513 != null) {
              if (activePatternResult513 === ";") {
                $var104 = [0];
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
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var103[2]);
        var $var105 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult524 = _Token.Token["|T|_|"](patternInput[1].head);

          if (activePatternResult524 != null) {
            if (activePatternResult524 === ";") {
              $var105 = [0, patternInput[0], patternInput[1].tail];
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
            var patternInput_1 = parse(new State("Local", []), function (_arg23) {
              var $var106 = void 0;

              if (_arg23.tail != null) {
                var activePatternResult516 = _Token.Token["|T|_|"](_arg23.head);

                if (activePatternResult516 != null) {
                  if (activePatternResult516 === ";") {
                    $var106 = [0];
                  } else {
                    $var106 = [1];
                  }
                } else {
                  $var106 = [1];
                }
              } else {
                $var106 = [1];
              }

              switch ($var106[0]) {
                case 0:
                  return true;

                case 1:
                  return false;
              }
            }, function (e_1) {
              return stop(e_1) ? true : fail(e_1);
            }, new _List2.default(), $var105[2]);
            var $var107 = void 0;

            if (patternInput_1[1].tail != null) {
              var activePatternResult523 = _Token.Token["|T|_|"](patternInput_1[1].head);

              if (activePatternResult523 != null) {
                if (activePatternResult523 === ";") {
                  $var107 = [0, patternInput_1[0], patternInput_1[1].tail];
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
                var patternInput_2 = parse(new State("Local", []), function (_arg24) {
                  var $var108 = void 0;

                  if (_arg24.tail != null) {
                    var activePatternResult519 = _Token.Token["|T|_|"](_arg24.head);

                    if (activePatternResult519 != null) {
                      if (activePatternResult519 === ")") {
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
                      return false;
                  }
                }, function (e_2) {
                  return stop(e_2) ? true : fail(e_2);
                }, new _List2.default(), $var107[2]);
                var $var109 = void 0;

                if (patternInput_2[1].tail != null) {
                  var activePatternResult522 = _Token.Token["|T|_|"](patternInput_2[1].head);

                  if (activePatternResult522 != null) {
                    if (activePatternResult522 === ")") {
                      $var109 = [0, patternInput_2[0], patternInput_2[1].tail];
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
                    var patternInput_3 = getStatementBody(stop, fail, $var109[2]);

                    var parsed = _Token.Token.Token[".ctor_2"]("sequence", (0, _List.ofArray)([$var105[1], _Token.Token.Token[".ctor_2"]("while", (0, _List.ofArray)([$var107[1], _Token.Token.Token[".ctor_2"]("sequence", (0, _List.ofArray)([patternInput_3[0], $var109[1]]))]))]));

                    return parse(new State("Local", []), stop, fail, $var103[1], new _List2.default(parsed, patternInput_3[1]));

                  case 1:
                    throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 271, 10);
                }

              case 1:
                throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 269, 10);
            }

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 266, 10);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$For$7C$_$7C$ = _For___;

  function _Return___(state, stop, fail, _arg25_0, _arg25_1) {
    var _arg25 = [_arg25_0, _arg25_1];
    var $var110 = void 0;

    if (_arg25[0].tail != null) {
      var activePatternResult534 = _Token.Token["|T|_|"](_arg25[0].head);

      if (activePatternResult534 != null) {
        if (activePatternResult534 === "return") {
          $var110 = [0, _arg25[0].tail, _arg25[1]];
        } else {
          $var110 = [1];
        }
      } else {
        $var110 = [1];
      }
    } else {
      $var110 = [1];
    }

    switch ($var110[0]) {
      case 0:
        var patternInput = parse(new State("LocalImd", []), function (_arg26) {
          var $var111 = void 0;

          if (_arg26.tail != null) {
            var activePatternResult531 = _Token.Token["|T|_|"](_arg26.head);

            if (activePatternResult531 != null) {
              if (activePatternResult531 === ";") {
                $var111 = [0];
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
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), $var110[2]);

        var parsed = _Token.Token.Token[".ctor_2"]("return", (0, _List.ofArray)([patternInput[0]]));

        return parse(new State("Local", []), stop, fail, $var110[1], new _List2.default(parsed, patternInput[1]));

      case 1:
        return null;
    }
  }

  exports.$7C$Return$7C$_$7C$ = _Return___;

  function _Assignment___(state, stop, fail, _arg27_0, _arg27_1) {
    var _arg27 = [_arg27_0, _arg27_1];
    var $var112 = void 0;

    if (_arg27[0].tail != null) {
      if (_arg27[1].tail != null) {
        var activePatternResult541 = _Token.Token["|T|_|"](_arg27[1].head);

        if (activePatternResult541 != null) {
          if (activePatternResult541 === "=") {
            $var112 = [0, _arg27[0].head, _arg27[0].tail, _arg27[1].tail];
          } else {
            $var112 = [1];
          }
        } else {
          $var112 = [1];
        }
      } else {
        $var112 = [1];
      }
    } else {
      $var112 = [1];
    }

    switch ($var112[0]) {
      case 0:
        var patternInput = parse(new State("LocalImd", []), function (_arg28) {
          var $var113 = void 0;

          if (_arg28.tail != null) {
            var activePatternResult539 = _Token.Token["|T|_|"](_arg28.head);

            if (activePatternResult539 != null) {
              if (activePatternResult539 === ";") {
                $var113 = [0];
              } else {
                $var113 = [1];
              }
            } else {
              $var113 = [1];
            }
          } else {
            $var113 = [1];
          }

          switch ($var113[0]) {
            case 0:
              return true;

            case 1:
              return stop(_arg28);
          }
        }, fail, new _List2.default(), $var112[3]);

        var parsed = _Token.Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([$var112[1], patternInput[0]]));

        return parse(new State("LocalImd", []), stop, fail, $var112[2], new _List2.default(parsed, patternInput[1]));

      case 1:
        return null;
    }
  }

  exports.$7C$Assignment$7C$_$7C$ = _Assignment___;

  function _Dot___(state, stop, fail, _arg29_0, _arg29_1) {
    var _arg29 = [_arg29_0, _arg29_1];
    var $var114 = void 0;

    if (_arg29[0].tail != null) {
      if (_arg29[1].tail != null) {
        var activePatternResult546 = _Token.Token["|T|_|"](_arg29[1].head);

        if (activePatternResult546 != null) {
          if (activePatternResult546 === ".") {
            if (_arg29[1].tail.tail != null) {
              var activePatternResult547 = _Token.Token["|T|_|"](_arg29[1].tail.head);

              if (activePatternResult547 != null) {
                $var114 = [0, _arg29[0].head, activePatternResult547, _arg29[0].tail, _arg29[1].tail.tail];
              } else {
                $var114 = [1];
              }
            } else {
              $var114 = [1];
            }
          } else {
            $var114 = [1];
          }
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
        var parsed = _Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var114[1], _Token.Token.Token[".ctor_3"]($var114[2])]));

        return parse(state, stop, fail, $var114[3], new _List2.default(parsed, $var114[4]));

      case 1:
        return null;
    }
  }

  exports.$7C$Dot$7C$_$7C$ = _Dot___;

  function _Prefix___(state, stop, fail, _arg30_0, _arg30_1) {
    var _arg30 = [_arg30_0, _arg30_1];
    var $var115 = void 0;

    if (_arg30[0].tail != null) {
      var activePatternResult569 = _Token.Token["|Pref|_|"](_arg30[0].head);

      if (activePatternResult569 != null) {
        $var115 = [0, activePatternResult569, _arg30[0].tail, _arg30[1]];
      } else {
        $var115 = [1];
      }
    } else {
      $var115 = [1];
    }

    switch ($var115[0]) {
      case 0:
        var patternInput_2 = void 0;
        var $var116 = void 0;

        if ($var115[3].tail != null) {
          var activePatternResult567 = _Token.Token["|T|_|"]($var115[3].head);

          if (activePatternResult567 != null) {
            if ($var115[3].tail.tail != null) {
              var activePatternResult568 = _Token.Token["|T|_|"]($var115[3].tail.head);

              if (activePatternResult568 != null) {
                if (activePatternResult568 === "[") {
                  $var116 = [0, $var115[3].tail, activePatternResult567];
                } else {
                  $var116 = [1];
                }
              } else {
                $var116 = [1];
              }
            } else {
              $var116 = [1];
            }
          } else {
            $var116 = [1];
          }
        } else {
          $var116 = [1];
        }

        switch ($var116[0]) {
          case 0:
            var patternInput = parse(state, function (_arg31) {
              var $var117 = void 0;

              if (_arg31.tail != null) {
                var activePatternResult552 = _Token.Token["|T|_|"](_arg31.head);

                if (activePatternResult552 != null) {
                  $var117 = [0];
                } else {
                  $var117 = [1];
                }
              } else {
                $var117 = [1];
              }

              switch ($var117[0]) {
                case 0:
                  return false;

                case 1:
                  return true;
              }
            }, function (e) {
              return stop(e) ? true : fail(e);
            }, new _List2.default(), $var115[3]);
            var $var118 = void 0;

            var activePatternResult556 = _Token.Token["|T|_|"](patternInput[0]);

            if (activePatternResult556 != null) {
              if (activePatternResult556 === "()") {
                if (patternInput[1].tail != null) {
                  $var118 = [0, patternInput[1].head, patternInput[1].tail];
                } else {
                  $var118 = [1];
                }
              } else {
                var activePatternResult557 = _Token.Token["|X|"](patternInput[0]);

                if (activePatternResult557[0] === "sequence") {
                  if (activePatternResult557[1].tail == null) {
                    if (patternInput[1].tail != null) {
                      $var118 = [0, patternInput[1].head, patternInput[1].tail];
                    } else {
                      $var118 = [1];
                    }
                  } else {
                    $var118 = [1];
                  }
                } else {
                  $var118 = [1];
                }
              }
            } else {
              var activePatternResult558 = _Token.Token["|X|"](patternInput[0]);

              if (activePatternResult558[0] === "sequence") {
                if (activePatternResult558[1].tail == null) {
                  if (patternInput[1].tail != null) {
                    $var118 = [0, patternInput[1].head, patternInput[1].tail];
                  } else {
                    $var118 = [1];
                  }
                } else {
                  $var118 = [1];
                }
              } else {
                $var118 = [1];
              }
            }

            switch ($var118[0]) {
              case 0:
                patternInput_2 = [$var118[1], $var118[2]];
                break;

              case 1:
                throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 301, 14);
                break;
            }

            break;

          case 1:
            var $var119 = void 0;

            if ($var115[3].tail != null) {
              var activePatternResult566 = _Token.Token["|T|_|"]($var115[3].head);

              if (activePatternResult566 != null) {
                if (_Lexer.CommonClassifiers.op_GreaterGreaterBarBar(_Lexer.CommonClassifiers.isNumeric, _Lexer.CommonClassifiers.isVariable, activePatternResult566)) {
                  $var119 = [0, $var115[3].head, $var115[3].tail, activePatternResult566];
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
                patternInput_2 = [$var119[1], $var119[2]];
                break;

              case 1:
                var patternInput_1 = parse(state, function (_arg32) {
                  var $var120 = void 0;

                  if (_arg32.tail != null) {
                    var activePatternResult559 = _Token.Token["|T|_|"](_arg32.head);

                    if (activePatternResult559 != null) {
                      $var120 = [0];
                    } else {
                      $var120 = [1];
                    }
                  } else {
                    $var120 = [1];
                  }

                  switch ($var120[0]) {
                    case 0:
                      return false;

                    case 1:
                      return true;
                  }
                }, function (e_1) {
                  return stop(e_1) ? true : fail(e_1);
                }, new _List2.default(), $var115[3]);
                var $var121 = void 0;

                var activePatternResult563 = _Token.Token["|T|_|"](patternInput_1[0]);

                if (activePatternResult563 != null) {
                  if (activePatternResult563 === "()") {
                    if (patternInput_1[1].tail != null) {
                      $var121 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                    } else {
                      $var121 = [1];
                    }
                  } else {
                    var activePatternResult564 = _Token.Token["|X|"](patternInput_1[0]);

                    if (activePatternResult564[0] === "sequence") {
                      if (activePatternResult564[1].tail == null) {
                        if (patternInput_1[1].tail != null) {
                          $var121 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                        } else {
                          $var121 = [1];
                        }
                      } else {
                        $var121 = [1];
                      }
                    } else {
                      $var121 = [1];
                    }
                  }
                } else {
                  var activePatternResult565 = _Token.Token["|X|"](patternInput_1[0]);

                  if (activePatternResult565[0] === "sequence") {
                    if (activePatternResult565[1].tail == null) {
                      if (patternInput_1[1].tail != null) {
                        $var121 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                      } else {
                        $var121 = [1];
                      }
                    } else {
                      $var121 = [1];
                    }
                  } else {
                    $var121 = [1];
                  }
                }

                switch ($var121[0]) {
                  case 0:
                    patternInput_2 = [$var121[1], $var121[2]];
                    break;

                  case 1:
                    throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 306, 14);
                    break;
                }

                break;
            }

            break;
        }

        var parsed = new _Token.Token.Token("apply", $var115[1].Indentation, true, (0, _List.ofArray)([$var115[1], patternInput_2[0]]));
        return parse(state, stop, fail, $var115[2], new _List2.default(parsed, patternInput_2[1]));

      case 1:
        return null;
    }
  }

  exports.$7C$Prefix$7C$_$7C$ = _Prefix___;

  function _Operator___(state, stop, fail, _arg33_0, _arg33_1) {
    var _arg33 = [_arg33_0, _arg33_1];
    var $var122 = void 0;

    if (_arg33[0].tail != null) {
      if (_arg33[1].tail != null) {
        var activePatternResult577 = _Token.Token["|T|_|"](_arg33[1].head);

        if (activePatternResult577 != null) {
          if (function () {
            var restr = _arg33[1].tail;
            var restl = _arg33[0].tail;
            var nfx = _arg33[1].head;
            var a = _arg33[0].head;
            return nfx.Priority !== -1;
          }()) {
            $var122 = [0, _arg33[0].head, _arg33[1].head, _arg33[0].tail, _arg33[1].tail, activePatternResult577];
          } else {
            $var122 = [1];
          }
        } else {
          $var122 = [1];
        }
      } else {
        $var122 = [1];
      }
    } else {
      $var122 = [1];
    }

    switch ($var122[0]) {
      case 0:
        var patternInput = parse(new State("LocalImd", []), function (_arg34) {
          var $var123 = void 0;

          if (_arg34.tail != null) {
            var activePatternResult575 = _Token.Token["|T|_|"](_arg34.head);

            if (activePatternResult575 != null) {
              if (_arg34.head.Priority <= $var122[2].Priority ? _arg34.head.Priority !== -1 : false) {
                $var123 = [0, _arg34.head];
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
              var $var124 = void 0;

              if (_arg34.tail != null) {
                var activePatternResult574 = _Token.Token["|T|_|"](_arg34.head);

                if (activePatternResult574 != null) {
                  if (activePatternResult574 === ";") {
                    $var124 = [0];
                  } else if (activePatternResult574 === ",") {
                    $var124 = [0];
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
                  return true;

                case 1:
                  return stop(_arg34);
              }

          }
        }, fail, new _List2.default(), $var122[4]);

        var parsed = _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var122[5]), $var122[1]])), patternInput[0]]));

        return parse(new State("LocalImd", []), stop, fail, $var122[3], new _List2.default(parsed, patternInput[1]));

      case 1:
        return null;
    }
  }

  exports.$7C$Operator$7C$_$7C$ = _Operator___;

  function _Apply___(state, stop, fail, _arg35_0, _arg35_1) {
    var _arg35 = [_arg35_0, _arg35_1];
    var $var125 = void 0;

    if (_arg35[0].tail != null) {
      if (_arg35[1].tail != null) {
        var activePatternResult590 = _Token.Token["|T|_|"](_arg35[1].head);

        if (activePatternResult590 != null) {
          if (activePatternResult590 === "(") {
            $var125 = [0, _arg35[0].head, _arg35[0].tail, _arg35[1].tail];
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
        var state_ = state.Case === "Global" ? new State("FunctionArgs", []) : new State("LocalImd", []);
        var patternInput = void 0;
        var matchValue = parse(state_, function (_arg36) {
          var $var126 = void 0;

          if (_arg36.tail != null) {
            var activePatternResult582 = _Token.Token["|T|_|"](_arg36.head);

            if (activePatternResult582 != null) {
              if (activePatternResult582 === ")") {
                $var126 = [0];
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
              return true;

            case 1:
              return false;
          }
        }, function (_arg2) {
          return false;
        }, new _List2.default(), $var125[3]);
        var $var127 = void 0;

        var activePatternResult588 = _Token.Token["|X|"](matchValue[0]);

        if (activePatternResult588[0] === "sequence") {
          if (activePatternResult588[1].tail != null) {
            if (activePatternResult588[1].tail.tail == null) {
              if (matchValue[1].tail != null) {
                var activePatternResult589 = _Token.Token["|T|_|"](matchValue[1].head);

                if (activePatternResult589 != null) {
                  if (activePatternResult589 === ")") {
                    $var127 = [0, activePatternResult588[1].head, matchValue[1].tail];
                  } else {
                    $var127 = [1];
                  }
                } else {
                  $var127 = [1];
                }
              } else {
                $var127 = [1];
              }
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
            patternInput = [$var127[1], $var127[2]];
            break;

          case 1:
            var $var128 = void 0;

            var activePatternResult586 = _Token.Token["|X|"](matchValue[0]);

            if (activePatternResult586[0] === "sequence") {
              if (activePatternResult586[1].tail == null) {
                if (matchValue[1].tail != null) {
                  var activePatternResult587 = _Token.Token["|T|_|"](matchValue[1].head);

                  if (activePatternResult587 != null) {
                    if (activePatternResult587 === ")") {
                      $var128 = [0, matchValue[1].tail];
                    } else {
                      $var128 = [1];
                    }
                  } else {
                    $var128 = [1];
                  }
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
                patternInput = [_Token.Token.Token[".ctor_2"]("_", new _List2.default()), $var128[1]];
                break;

              case 1:
                patternInput = (0, _String.fsFormat)("arguments were not formatted correctly %A")(function (x) {
                  throw new Error(x);
                })(matchValue);
                break;
            }

            break;
        }

        var parsed = _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var125[1], patternInput[0]]));

        return parse(new State("LocalImd", []), stop, fail, $var125[2], new _List2.default(parsed, patternInput[1]));

      case 1:
        return null;
    }
  }

  exports.$7C$Apply$7C$_$7C$ = _Apply___;

  function _Index___(state, stop, fail, _arg37_0, _arg37_1) {
    var _arg37 = [_arg37_0, _arg37_1];
    var $var129 = void 0;

    if (_arg37[0].tail != null) {
      if (_arg37[1].tail != null) {
        var activePatternResult599 = _Token.Token["|T|_|"](_arg37[1].head);

        if (activePatternResult599 != null) {
          if (activePatternResult599 === "[") {
            $var129 = [0, _arg37[0].head, _arg37[0].tail, _arg37[1].tail];
          } else {
            $var129 = [1];
          }
        } else {
          $var129 = [1];
        }
      } else {
        $var129 = [1];
      }
    } else {
      $var129 = [1];
    }

    switch ($var129[0]) {
      case 0:
        var patternInput = parse(state, function (_arg38) {
          var $var130 = void 0;

          if (_arg38.tail != null) {
            var activePatternResult595 = _Token.Token["|T|_|"](_arg38.head);

            if (activePatternResult595 != null) {
              if (activePatternResult595 === "]") {
                $var130 = [0];
              } else {
                $var130 = [1];
              }
            } else {
              $var130 = [1];
            }
          } else {
            $var130 = [1];
          }

          switch ($var130[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (_arg3) {
          return false;
        }, new _List2.default(), $var129[3]);
        var $var131 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult598 = _Token.Token["|T|_|"](patternInput[1].head);

          if (activePatternResult598 != null) {
            if (activePatternResult598 === "]") {
              $var131 = [0, patternInput[0], patternInput[1].tail];
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
            var parsed = _Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var129[1], _Token.Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([$var131[1]]))]));

            return parse(state, stop, fail, $var129[2], new _List2.default(parsed, $var131[2]));

          case 1:
            throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 334, 10);
        }

      case 1:
        return null;
    }
  }

  exports.$7C$Index$7C$_$7C$ = _Index___;

  function _Transfer___(state, stop, fail, _arg39_0, _arg39_1) {
    var _arg39 = [_arg39_0, _arg39_1];

    if (_arg39[1].tail != null) {
      var x = _arg39[1].head;
      var restr = _arg39[1].tail;
      return parse(state, stop, fail, new _List2.default(x, _arg39[0]), restr);
    } else {
      return null;
    }
  }

  exports.$7C$Transfer$7C$_$7C$ = _Transfer___;

  function postProcess(_arg1) {
    postProcess: while (true) {
      var $var132 = void 0;

      var activePatternResult617 = _Token.Token["|X|"](_arg1);

      if (activePatternResult617[0] === "declare function") {
        if (activePatternResult617[1].tail != null) {
          if (activePatternResult617[1].tail.tail != null) {
            if (activePatternResult617[1].tail.tail.tail != null) {
              if (activePatternResult617[1].tail.tail.tail.tail != null) {
                if (activePatternResult617[1].tail.tail.tail.tail.tail == null) {
                  $var132 = [0, activePatternResult617[1].tail.tail.head, activePatternResult617[1].head, activePatternResult617[1].tail.head, activePatternResult617[1].tail.tail.tail.head];
                } else {
                  $var132 = [5];
                }
              } else {
                $var132 = [5];
              }
            } else {
              $var132 = [5];
            }
          } else {
            $var132 = [5];
          }
        } else {
          $var132 = [5];
        }
      } else if (activePatternResult617[0] === "{}") {
        $var132 = [1, activePatternResult617[1]];
      } else if (activePatternResult617[0] === "sequence") {
        $var132 = [2, activePatternResult617[1]];
      } else if (activePatternResult617[0] === "==") {
        $var132 = [3, activePatternResult617[1]];
      } else if (activePatternResult617[0] === "apply") {
        if (activePatternResult617[1].tail != null) {
          var activePatternResult618 = _Token.Token["|T|_|"](activePatternResult617[1].head);

          if (activePatternResult618 != null) {
            if (activePatternResult618 === "printf") {
              if (activePatternResult617[1].tail.tail != null) {
                var activePatternResult619 = _Token.Token["|X|"](activePatternResult617[1].tail.head);

                if (activePatternResult619[0] === ",") {
                  if (activePatternResult619[1].tail != null) {
                    if (activePatternResult617[1].tail.tail.tail == null) {
                      $var132 = [4, activePatternResult619[1].tail, activePatternResult619[1].head, activePatternResult617[1].head];
                    } else {
                      $var132 = [5];
                    }
                  } else {
                    $var132 = [5];
                  }
                } else {
                  $var132 = [5];
                }
              } else {
                $var132 = [5];
              }
            } else if (activePatternResult618 === "sprintf") {
              if (activePatternResult617[1].tail.tail != null) {
                var activePatternResult620 = _Token.Token["|X|"](activePatternResult617[1].tail.head);

                if (activePatternResult620[0] === ",") {
                  if (activePatternResult620[1].tail != null) {
                    if (activePatternResult617[1].tail.tail.tail == null) {
                      $var132 = [4, activePatternResult620[1].tail, activePatternResult620[1].head, activePatternResult617[1].head];
                    } else {
                      $var132 = [5];
                    }
                  } else {
                    $var132 = [5];
                  }
                } else {
                  $var132 = [5];
                }
              } else {
                $var132 = [5];
              }
            } else if (activePatternResult618 === "scanf") {
              if (activePatternResult617[1].tail.tail != null) {
                var activePatternResult621 = _Token.Token["|X|"](activePatternResult617[1].tail.head);

                if (activePatternResult621[0] === ",") {
                  if (activePatternResult621[1].tail != null) {
                    if (activePatternResult617[1].tail.tail.tail == null) {
                      $var132 = [4, activePatternResult621[1].tail, activePatternResult621[1].head, activePatternResult617[1].head];
                    } else {
                      $var132 = [5];
                    }
                  } else {
                    $var132 = [5];
                  }
                } else {
                  $var132 = [5];
                }
              } else {
                $var132 = [5];
              }
            } else {
              $var132 = [5];
            }
          } else {
            $var132 = [5];
          }
        } else {
          $var132 = [5];
        }
      } else {
        $var132 = [5];
      }

      switch ($var132[0]) {
        case 0:
          return _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([$var132[3], _Token.Token.Token[".ctor_2"]("fun", (0, _List.ofArray)([postProcess($var132[1]), postProcess($var132[4])]))]));

        case 1:
          _arg1 = _Token.Token.Token[".ctor_2"]("sequence", $var132[1]);
          continue postProcess;

        case 2:
          var xprs_ = (0, _List.filter)(function (_arg2) {
            var $var133 = void 0;

            var activePatternResult611 = _Token.Token["|T|_|"](_arg2);

            if (activePatternResult611 != null) {
              if (activePatternResult611 === ";") {
                $var133 = [0];
              } else {
                $var133 = [1];
              }
            } else {
              $var133 = [1];
            }

            switch ($var133[0]) {
              case 0:
                return false;

              case 1:
                return true;
            }
          }, $var132[1]);
          return _Token.Token.Token[".ctor_2"]("sequence", (0, _List.map)(function (_arg1_1) {
            return postProcess(_arg1_1);
          }, xprs_));

        case 3:
          return _Token.Token.Token[".ctor_2"]("=", (0, _List.map)(function (_arg1_2) {
            return postProcess(_arg1_2);
          }, $var132[1]));

        case 4:
          return (0, _Seq.fold)(function (acc, e) {
            return _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([acc, e]));
          }, _Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var132[3], $var132[2]])), $var132[1]);

        case 5:
          var activePatternResult616 = _Token.Token["|X|"](_arg1);

          return _Token.Token.Token[".ctor_2"](activePatternResult616[0], (0, _List.map)(function (_arg1_3) {
            return postProcess(_arg1_3);
          }, activePatternResult616[1]));
      }
    }
  }

  function parseSyntax(e) {
    restoreDefault();
    return function (e_1) {
      (0, _String.fsFormat)("%A")(function (x) {
        console.log(x);
      })(e_1);
      return e_1;
    }((0, _StringFormatting.processStringFormatting)(function (_arg2) {
      var activePatternResult625 = _Token.Token["|X|"](_arg2);

      if (activePatternResult625[0] === "sequence") {
        return _Token.Token.Token[".ctor_2"]("sequence", (0, _List.append)(activePatternResult625[1], (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("main"), _Token.Token.Token[".ctor_3"]("()")]))])));
      } else {
        throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser_C.fs", 362, 8);
      }
    }(postProcess(parse(new State("Global", []), function (_arg1) {
      return _arg1.tail == null ? true : false;
    }, function (_arg1_1) {
      return false;
    }, new _List2.default(), preprocess(e))[0]))));
  }
});