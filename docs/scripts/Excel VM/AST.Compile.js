define(["exports", "fable-core/umd/Util", "./Parser.Definition", "./AST.Definition", "fable-core/umd/List", "fable-core/umd/String", "fable-core/umd/Map", "fable-core/umd/Seq", "fable-core/umd/GenericComparer"], function (exports, _Util, _Parser, _AST, _List, _String, _Map, _Seq, _GenericComparer) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.ASTCompile$27$ = exports.nxt = exports.nxt$27$ = undefined;
  exports.transformFromToken = transformFromToken;

  var _List2 = _interopRequireDefault(_List);

  var _GenericComparer2 = _interopRequireDefault(_GenericComparer);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

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
      var $var280 = void 0;
      var activePatternResult934 = (0, _Parser.$7C$X$7C$)(_arg2);

      if (activePatternResult934[0] === "return") {
        $var280 = [0, activePatternResult934[1]];
      } else {
        var activePatternResult935 = (0, _Parser.$7C$T$7C$_$7C$)(_arg2);

        if (activePatternResult935 != null) {
          if (activePatternResult935 === "break") {
            $var280 = [1];
          } else {
            $var280 = [2];
          }
        } else {
          $var280 = [2];
        }
      }

      switch ($var280[0]) {
        case 0:
          if ($var280[1].tail != null) {
            if ($var280[1].tail.tail == null) {
              return new _AST.AST("Return", [function (tupledArg) {
                return ASTCompile_(tupledArg[0], tupledArg[1]);
              }(_arg1)($var280[1].head)]);
            } else {
              throw new Error("cannot return more than one item");
            }
          } else {
            return new _AST.AST("Return", [new _AST.AST("Const", ["()"])]);
          }

        case 1:
          return new _AST.AST("Break", []);

        case 2:
          var $var281 = void 0;
          var activePatternResult933 = (0, _Parser.$7C$T$7C$_$7C$)(_arg2);

          if (activePatternResult933 != null) {
            if (activePatternResult933 === "continue") {
              $var281 = [0];
            } else {
              $var281 = [1];
            }
          } else {
            $var281 = [1];
          }

          switch ($var281[0]) {
            case 0:
              return new _AST.AST("Continue", []);

            case 1:
              var $var282 = void 0;
              var activePatternResult931 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

              if (activePatternResult931.Case === "Choice1Of3") {
                $var282 = [0, activePatternResult931.Fields[0]];
              } else if (activePatternResult931.Case === "Choice2Of3") {
                $var282 = [1, activePatternResult931.Fields[0]];
              } else {
                var activePatternResult932 = (0, _Parser.$7C$X$7C$)(_arg2);

                if (activePatternResult932[0] === ",") {
                  $var282 = [2, activePatternResult932[1]];
                } else if (activePatternResult932[0] === "apply") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [3, activePatternResult932[1].head, activePatternResult932[1].tail.head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "fun") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [4, activePatternResult932[1].tail.head, activePatternResult932[1].head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "declare") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [5, activePatternResult932[1].tail.head, activePatternResult932[1].head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "let") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [6, activePatternResult932[1].head, activePatternResult932[1].tail.head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "let rec") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [7, activePatternResult932[1].head, activePatternResult932[1].tail.head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "array") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail == null) {
                      $var282 = [8, activePatternResult932[1].head];
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "dot") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [9, activePatternResult932[1].head, activePatternResult932[1].tail.head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "assign") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [10, activePatternResult932[1].head, activePatternResult932[1].tail.head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "if") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail != null) {
                        if (activePatternResult932[1].tail.tail.tail.tail == null) {
                          $var282 = [11, activePatternResult932[1].tail.head, activePatternResult932[1].head, activePatternResult932[1].tail.tail.head];
                        } else {
                          $var282 = [17, _arg2];
                        }
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "do") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail == null) {
                      $var282 = [12, activePatternResult932[1].head];
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "while") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail == null) {
                        $var282 = [13, activePatternResult932[1].tail.head, activePatternResult932[1].head];
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "for") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail != null) {
                      if (activePatternResult932[1].tail.tail.tail != null) {
                        if (activePatternResult932[1].tail.tail.tail.tail == null) {
                          $var282 = [14, activePatternResult932[1].tail.tail.head, activePatternResult932[1].tail.head, activePatternResult932[1].head];
                        } else {
                          $var282 = [17, _arg2];
                        }
                      } else {
                        $var282 = [17, _arg2];
                      }
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "deref") {
                  if (activePatternResult932[1].tail != null) {
                    if (activePatternResult932[1].tail.tail == null) {
                      $var282 = [15, activePatternResult932[1].head];
                    } else {
                      $var282 = [17, _arg2];
                    }
                  } else {
                    $var282 = [17, _arg2];
                  }
                } else if (activePatternResult932[0] === "sequence") {
                  $var282 = [16, activePatternResult932[1]];
                } else {
                  $var282 = [17, _arg2];
                }
              }

              switch ($var282[0]) {
                case 0:
                  if (_arg1[1].has($var282[1]) ? !_arg1[1].get($var282[1]).Equals(new _List2.default()) : false) {
                    return new _AST.AST("Apply", [new _AST.AST("Value", [$var282[1]]), _arg1[1].get($var282[1])]);
                  } else {
                    return new _AST.AST("Value", [$var282[1]]);
                  }

                case 1:
                  return new _AST.AST("Const", [$var282[1]]);

                case 2:
                  var name = "$tuple" + nxt(null);
                  var allocate = (0, _List.ofArray)([new _AST.AST("Declare", [name, new _AST.AST("New", [new _AST.AST("Const", [String($var282[1].length)])])])]);
                  var assignAll = (0, _List.mapIndexed)(function (i, e) {
                    return new _AST.AST("Assign", [new _AST.AST("Value", [name]), new _AST.AST("Const", [String(i)]), function (tupledArg_1) {
                      return ASTCompile_(tupledArg_1[0], tupledArg_1[1]);
                    }(_arg1)(e)]);
                  }, $var282[1]);
                  var returnVal = (0, _List.ofArray)([new _AST.AST("Value", [name])]);
                  return new _AST.AST("Sequence", [(0, _List.append)(allocate, (0, _List.append)(assignAll, returnVal))]);

                case 3:
                  return new _AST.AST("Apply", [function (tupledArg_2) {
                    return ASTCompile_(tupledArg_2[0], tupledArg_2[1]);
                  }(_arg1)($var282[1]), (0, _List.ofArray)([function (tupledArg_3) {
                    return ASTCompile_(tupledArg_3[0], tupledArg_3[1]);
                  }(_arg1)($var282[2])])]);

                case 4:
                  var unpack = function unpack(arg) {
                    return function (_arg3) {
                      var $var283 = void 0;
                      var activePatternResult880 = (0, _Parser.$7C$X$7C$)(_arg3);

                      if (activePatternResult880[0] === "declare") {
                        if (activePatternResult880[1].tail != null) {
                          if (activePatternResult880[1].tail.tail != null) {
                            var activePatternResult881 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult880[1].tail.head);

                            if (activePatternResult881 != null) {
                              if (activePatternResult880[1].tail.tail.tail == null) {
                                $var283 = [0, activePatternResult881];
                              } else {
                                var activePatternResult882 = (0, _Parser.$7C$T$7C$_$7C$)(_arg3);

                                if (activePatternResult882 != null) {
                                  $var283 = [0, activePatternResult882];
                                } else {
                                  $var283 = [1];
                                }
                              }
                            } else {
                              var activePatternResult883 = (0, _Parser.$7C$T$7C$_$7C$)(_arg3);

                              if (activePatternResult883 != null) {
                                $var283 = [0, activePatternResult883];
                              } else {
                                $var283 = [1];
                              }
                            }
                          } else {
                            var activePatternResult884 = (0, _Parser.$7C$T$7C$_$7C$)(_arg3);

                            if (activePatternResult884 != null) {
                              $var283 = [0, activePatternResult884];
                            } else {
                              $var283 = [1];
                            }
                          }
                        } else {
                          var activePatternResult885 = (0, _Parser.$7C$T$7C$_$7C$)(_arg3);

                          if (activePatternResult885 != null) {
                            $var283 = [0, activePatternResult885];
                          } else {
                            $var283 = [1];
                          }
                        }
                      } else {
                        var activePatternResult886 = (0, _Parser.$7C$T$7C$_$7C$)(_arg3);

                        if (activePatternResult886 != null) {
                          $var283 = [0, activePatternResult886];
                        } else {
                          $var283 = [1];
                        }
                      }

                      switch ($var283[0]) {
                        case 0:
                          return [(0, _List.ofArray)([new _AST.AST("Declare", [$var283[1], arg])]), (0, _List.ofArray)([$var283[1]])];

                        case 1:
                          var activePatternResult879 = (0, _Parser.$7C$X$7C$)(_arg3);

                          if (activePatternResult879[0] === ",") {
                            var patternInput = (0, _List.unzip)((0, _List.mapIndexed)(function ($var284, $var285) {
                              return function (i_1) {
                                return unpack(new _AST.AST("Get", [arg, new _AST.AST("Const", [String(i_1)])]));
                              }($var284)($var285);
                            }, activePatternResult879[1]));
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
                  var patternInput_1 = unpack(new _AST.AST("Value", [argName]))($var282[2]);
                  var patternInput_2 = ["L" + nxt(null), "K" + nxt(null)];
                  var cptr_d = (0, _List.map)(function (arg0) {
                    return new _AST.AST("Value", [arg0]);
                  }, _arg1[0]);
                  var cpt_ = [(0, _List.append)(patternInput_1[1], _arg1[0]), (0, _Map.add)(patternInput_2[0], cptr_d, _arg1[1])];
                  var functionBody = new _AST.AST("Sequence", [(0, _List.append)(patternInput_1[0], (0, _List.ofArray)([function (tupledArg_4) {
                    return ASTCompile_(tupledArg_4[0], tupledArg_4[1]);
                  }(cpt_)($var282[1])]))]);
                  return new _AST.AST("Sequence", [(0, _Seq.toList)((0, _Seq.delay)(function () {
                    return (0, _Seq.append)((0, _Seq.singleton)(new _AST.AST("Define", [patternInput_2[0], new _List2.default(argName, _arg1[0]), functionBody])), (0, _Seq.delay)(function () {
                      var matchValue = function (tupledArg_5) {
                        return ASTCompile_(tupledArg_5[0], tupledArg_5[1]);
                      }(cpt_)(_Parser.Token[".ctor_2"](patternInput_2[0], new _List2.default()));

                      if (matchValue.Case === "Apply") {
                        return (0, _Seq.append)((0, _Seq.singleton)(new _AST.AST("Declare", [patternInput_2[1], new _AST.AST("New", [new _AST.AST("Const", [String(matchValue.Fields[1].length + 1)])])])), (0, _Seq.delay)(function () {
                          return (0, _Seq.append)((0, _Seq.singleton)(new _AST.AST("Assign", [new _AST.AST("Value", [patternInput_2[1]]), new _AST.AST("Const", ["0"]), new _AST.AST("Get", [matchValue.Fields[0], new _AST.AST("Const", ["0"])])])), (0, _Seq.delay)(function () {
                            return (0, _Seq.append)((0, _List.mapIndexed)(function (i_2, e_1) {
                              return new _AST.AST("Assign", [new _AST.AST("Value", [patternInput_2[1]]), new _AST.AST("Const", [String(i_2 + 1)]), e_1]);
                            }, matchValue.Fields[1]), (0, _Seq.delay)(function () {
                              return (0, _Seq.singleton)(new _AST.AST("Value", [patternInput_2[1]]));
                            }));
                          }));
                        }));
                      } else {
                        return (0, _Seq.singleton)(matchValue);
                      }
                    }));
                  }))]);

                case 5:
                  return function (tupledArg_6) {
                    return ASTCompile_(tupledArg_6[0], tupledArg_6[1]);
                  }(_arg1)($var282[1]);

                case 6:
                  var $var286 = void 0;
                  var activePatternResult898 = (0, _Parser.$7C$X$7C$)($var282[1]);

                  if (activePatternResult898[0] === "apply") {
                    if (activePatternResult898[1].tail != null) {
                      if (activePatternResult898[1].tail.tail != null) {
                        if (activePatternResult898[1].tail.tail.tail == null) {
                          $var286 = [0, activePatternResult898[1].head, activePatternResult898[1].tail.head];
                        } else {
                          var activePatternResult899 = (0, _Parser.$7C$T$7C$_$7C$)($var282[1]);

                          if (activePatternResult899 != null) {
                            $var286 = [1, activePatternResult899];
                          } else {
                            $var286 = [2];
                          }
                        }
                      } else {
                        var activePatternResult900 = (0, _Parser.$7C$T$7C$_$7C$)($var282[1]);

                        if (activePatternResult900 != null) {
                          $var286 = [1, activePatternResult900];
                        } else {
                          $var286 = [2];
                        }
                      }
                    } else {
                      var activePatternResult901 = (0, _Parser.$7C$T$7C$_$7C$)($var282[1]);

                      if (activePatternResult901 != null) {
                        $var286 = [1, activePatternResult901];
                      } else {
                        $var286 = [2];
                      }
                    }
                  } else if (activePatternResult898[0] === "declare") {
                    var activePatternResult902 = (0, _Parser.$7C$T$7C$_$7C$)($var282[1]);

                    if (activePatternResult902 != null) {
                      $var286 = [1, activePatternResult902];
                    } else if (activePatternResult898[1].tail != null) {
                      if (activePatternResult898[1].tail.tail != null) {
                        var activePatternResult903 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult898[1].tail.head);

                        if (activePatternResult903 != null) {
                          if (activePatternResult898[1].tail.tail.tail == null) {
                            $var286 = [1, activePatternResult903];
                          } else {
                            $var286 = [2];
                          }
                        } else {
                          $var286 = [2];
                        }
                      } else {
                        $var286 = [2];
                      }
                    } else {
                      $var286 = [2];
                    }
                  } else {
                    var activePatternResult904 = (0, _Parser.$7C$T$7C$_$7C$)($var282[1]);

                    if (activePatternResult904 != null) {
                      $var286 = [1, activePatternResult904];
                    } else {
                      $var286 = [2];
                    }
                  }

                  switch ($var286[0]) {
                    case 0:
                      return function (tupledArg_7) {
                        return ASTCompile_(tupledArg_7[0], tupledArg_7[1]);
                      }(_arg1)(_Parser.Token[".ctor_2"]("let", (0, _List.ofArray)([$var286[1], _Parser.Token[".ctor_2"]("fun", (0, _List.ofArray)([$var286[2], $var282[2]]))])));

                    case 1:
                      return new _AST.AST("Declare", [$var286[1], function (tupledArg_8) {
                        return ASTCompile_(tupledArg_8[0], tupledArg_8[1]);
                      }(_arg1)($var282[2])]);

                    case 2:
                      return (0, _String.fsFormat)("patterns in function arguments not supported yet: %A")(function (x) {
                        throw new Error(x);
                      })($var282[1]);
                  }

                case 7:
                  return function (tupledArg_9) {
                    return ASTCompile_(tupledArg_9[0], tupledArg_9[1]);
                  }(_arg1)(_Parser.Token[".ctor_2"]("let", (0, _List.ofArray)([$var282[1], $var282[2]])));

                case 8:
                  return new _AST.AST("New", [function (tupledArg_10) {
                    return ASTCompile_(tupledArg_10[0], tupledArg_10[1]);
                  }(_arg1)($var282[1])]);

                case 9:
                  var $var287 = void 0;
                  var activePatternResult905 = (0, _Parser.$7C$X$7C$)($var282[2]);

                  if (activePatternResult905[0] === "[]") {
                    if (activePatternResult905[1].tail != null) {
                      if (activePatternResult905[1].tail.tail == null) {
                        $var287 = [0, activePatternResult905[1].head];
                      } else {
                        $var287 = [1];
                      }
                    } else {
                      $var287 = [1];
                    }
                  } else {
                    $var287 = [1];
                  }

                  switch ($var287[0]) {
                    case 0:
                      return new _AST.AST("Get", [function (tupledArg_11) {
                        return ASTCompile_(tupledArg_11[0], tupledArg_11[1]);
                      }(_arg1)($var282[1]), function (tupledArg_12) {
                        return ASTCompile_(tupledArg_12[0], tupledArg_12[1]);
                      }(_arg1)($var287[1])]);

                    case 1:
                      throw new Error("should never happen");
                  }

                case 10:
                  var $var288 = void 0;
                  var activePatternResult906 = (0, _Parser.$7C$X$7C$)($var282[1]);

                  if (activePatternResult906[1].tail != null) {
                    if (activePatternResult906[1].tail.tail == null) {
                      if (activePatternResult906[0] === "deref") {
                        $var288 = [2, activePatternResult906[1].head];
                      } else {
                        $var288 = [3];
                      }
                    } else {
                      var activePatternResult907 = (0, _Parser.$7C$X$7C$)(activePatternResult906[1].tail.head);

                      if (activePatternResult907[0] === "[]") {
                        if (activePatternResult907[1].tail != null) {
                          if (activePatternResult907[1].tail.tail == null) {
                            if (activePatternResult906[1].tail.tail.tail == null) {
                              if (activePatternResult906[0] === "dot") {
                                $var288 = [1, activePatternResult906[1].head, activePatternResult907[1].head];
                              } else {
                                $var288 = [3];
                              }
                            } else {
                              $var288 = [3];
                            }
                          } else {
                            $var288 = [3];
                          }
                        } else {
                          $var288 = [3];
                        }
                      } else {
                        $var288 = [3];
                      }
                    }
                  } else {
                    $var288 = [0, activePatternResult906[0]];
                  }

                  switch ($var288[0]) {
                    case 0:
                      return new _AST.AST("Mutate", [$var288[1], function (tupledArg_13) {
                        return ASTCompile_(tupledArg_13[0], tupledArg_13[1]);
                      }(_arg1)($var282[2])]);

                    case 1:
                      return new _AST.AST("Assign", [function (tupledArg_14) {
                        return ASTCompile_(tupledArg_14[0], tupledArg_14[1]);
                      }(_arg1)($var288[1]), function (tupledArg_15) {
                        return ASTCompile_(tupledArg_15[0], tupledArg_15[1]);
                      }(_arg1)($var288[2]), function (tupledArg_16) {
                        return ASTCompile_(tupledArg_16[0], tupledArg_16[1]);
                      }(_arg1)($var282[2])]);

                    case 2:
                      return new _AST.AST("Assign", [function (tupledArg_17) {
                        return ASTCompile_(tupledArg_17[0], tupledArg_17[1]);
                      }(_arg1)($var288[1]), new _AST.AST("Const", ["0"]), function (tupledArg_18) {
                        return ASTCompile_(tupledArg_18[0], tupledArg_18[1]);
                      }(_arg1)($var282[2])]);

                    case 3:
                      throw new Error("todo: unpacking");
                  }

                case 11:
                  return new _AST.AST("If", [function (tupledArg_19) {
                    return ASTCompile_(tupledArg_19[0], tupledArg_19[1]);
                  }(_arg1)($var282[2]), function (tupledArg_20) {
                    return ASTCompile_(tupledArg_20[0], tupledArg_20[1]);
                  }(_arg1)($var282[1]), function (tupledArg_21) {
                    return ASTCompile_(tupledArg_21[0], tupledArg_21[1]);
                  }(_arg1)($var282[3])]);

                case 12:
                  return new _AST.AST("Apply", [new _AST.AST("Value", ["ignore"]), (0, _List.ofArray)([function (tupledArg_22) {
                    return ASTCompile_(tupledArg_22[0], tupledArg_22[1]);
                  }(_arg1)($var282[1])])]);

                case 13:
                  return new _AST.AST("Loop", [function (tupledArg_23) {
                    return ASTCompile_(tupledArg_23[0], tupledArg_23[1]);
                  }(_arg1)($var282[2]), function (tupledArg_24) {
                    return ASTCompile_(tupledArg_24[0], tupledArg_24[1]);
                  }(_arg1)($var282[1])]);

                case 14:
                  var name_1 = void 0;
                  var activePatternResult908 = (0, _Parser.$7C$X$7C$)($var282[3]);

                  if (activePatternResult908[1].tail == null) {
                    name_1 = activePatternResult908[0];
                  } else {
                    throw new Error("todo: unpacking");
                  }

                  var $var289 = void 0;
                  var activePatternResult909 = (0, _Parser.$7C$X$7C$)($var282[2]);

                  if (activePatternResult909[0] === "..") {
                    if (activePatternResult909[1].tail != null) {
                      if (activePatternResult909[1].tail.tail != null) {
                        if (activePatternResult909[1].tail.tail.tail != null) {
                          if (activePatternResult909[1].tail.tail.tail.tail == null) {
                            $var289 = [0, activePatternResult909[1].head, activePatternResult909[1].tail.tail.head, activePatternResult909[1].tail.head];
                          } else {
                            $var289 = [1];
                          }
                        } else {
                          $var289 = [1];
                        }
                      } else {
                        $var289 = [1];
                      }
                    } else {
                      $var289 = [1];
                    }
                  } else {
                    $var289 = [1];
                  }

                  switch ($var289[0]) {
                    case 0:
                      return new _AST.AST("Sequence", [(0, _List.ofArray)([new _AST.AST("Declare", [name_1, function (tupledArg_25) {
                        return ASTCompile_(tupledArg_25[0], tupledArg_25[1]);
                      }(_arg1)($var289[1])]), new _AST.AST("Loop", [new _AST.AST("Apply", [new _AST.AST("Apply", [new _AST.AST("Value", ["<="]), (0, _List.ofArray)([new _AST.AST("Value", [name_1])])]), (0, _List.ofArray)([function (tupledArg_26) {
                        return ASTCompile_(tupledArg_26[0], tupledArg_26[1]);
                      }(_arg1)($var289[2])])]), new _AST.AST("Sequence", [(0, _List.ofArray)([function (tupledArg_27) {
                        return ASTCompile_(tupledArg_27[0], tupledArg_27[1]);
                      }(_arg1)($var282[1]), new _AST.AST("Mutate", [name_1, new _AST.AST("Apply", [new _AST.AST("Apply", [new _AST.AST("Value", ["+"]), (0, _List.ofArray)([new _AST.AST("Value", [name_1])])]), (0, _List.ofArray)([function (tupledArg_28) {
                        return ASTCompile_(tupledArg_28[0], tupledArg_28[1]);
                      }(_arg1)($var289[3])])])])])])])])]);

                    case 1:
                      throw new Error("iterable objects not supported yet");
                  }

                case 15:
                  return new _AST.AST("Get", [function (tupledArg_29) {
                    return ASTCompile_(tupledArg_29[0], tupledArg_29[1]);
                  }(_arg1)($var282[1]), new _AST.AST("Const", ["0"])]);

                case 16:
                  return new _AST.AST("Sequence", [(0, _List.reverse)((0, _Seq.fold)(function (tupledArg_30, e_2) {
                    var compiled = function (tupledArg_31) {
                      return ASTCompile_(tupledArg_31[0], tupledArg_31[1]);
                    }(tupledArg_30[1])(e_2);

                    var cpt__1 = compiled.Case === "Declare" ? [new _List2.default(compiled.Fields[0], tupledArg_30[1][0]), tupledArg_30[1][1]] : compiled.Case === "Define" ? [new _List2.default(compiled.Fields[0], tupledArg_30[1][0]), (0, _Map.add)(compiled.Fields[0], (0, _List.map)(function (arg0_1) {
                      return new _AST.AST("Value", [arg0_1]);
                    }, tupledArg_30[1][0]), tupledArg_30[1][1])] : tupledArg_30[1];
                    return [new _List2.default(compiled, tupledArg_30[0]), cpt__1];
                  }, [new _List2.default(), _arg1], $var282[1])[0])]);

                case 17:
                  return (0, _String.fsFormat)("unknown: %A")(function (x) {
                    throw new Error(x);
                  })($var282[1]);
              }

          }

      }
    };
  }

  exports.ASTCompile$27$ = ASTCompile_;

  function transformFromToken(e) {
    return ASTCompile_(new _List2.default(), (0, _Map.create)(null, new _GenericComparer2.default(_Util.compare)))(e);
  }
});