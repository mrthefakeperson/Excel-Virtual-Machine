define(["exports", "fable-core/umd/List", "fable-core/umd/Seq", "../build-docs/unused modules/UnusedModule1", "./Parser.Definition", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/Map", "fable-core/umd/GenericComparer"], function (exports, _List, _Seq, _UnusedModule, _Parser, _Symbol2, _Util, _Map, _GenericComparer) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.validateTypes = exports.varTypes = exports.objectTypes = exports.noObject = undefined;
  exports.changeNames = changeNames;
  exports.compileObjectDeclarations = compileObjectDeclarations;
  exports.compileObjects = compileObjects;
  exports.restoreDefault = restoreDefault;
  exports.compileObjectsToArrays = compileObjectsToArrays;
  exports.compilePointersToArrays = compilePointersToArrays;
  exports.processDerefs = processDerefs;

  var _List2 = _interopRequireDefault(_List);

  var _Symbol3 = _interopRequireDefault(_Symbol2);

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

  var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) {
    return typeof obj;
  } : function (obj) {
    return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
  };

  function changeNames(e) {
    var nameMappings = new Map();
    var latestNameMappings = new Map();

    var getName = function getName(s) {
      return nameMappings.get(s).head;
    };

    var addName = function addName(s_1) {
      var s_ = latestNameMappings.has(s_1) ? latestNameMappings.get(s_1) + "`" : s_1;
      latestNameMappings.set(s_1, s_);

      if (nameMappings.has(s_1) ? !nameMappings.get(s_1).Equals(new _List2.default()) : false) {
        nameMappings.set(s_1, new _List2.default(s_, nameMappings.get(s_1)));
      } else {
        nameMappings.set(s_1, (0, _List.ofArray)([s_]));
      }
    };

    var popName = function popName(s_2) {
      nameMappings.set(s_2, nameMappings.get(s_2).tail);
    };

    (0, _Seq.iterate)(addName, (0, _List.append)(_UnusedModule.definedOperators, _UnusedModule.definedPrefixOperators));
    addName("printf");
    addName("scan");
    addName("break");
    addName("continue");

    var mapAllNames = function mapAllNames(action) {
      return function (_arg1) {
        var activePatternResult707 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg1);

        if (activePatternResult707.Case === "Choice1Of3") {
          return _Parser.Token[".ctor_3"](action(activePatternResult707.Fields[0]));
        } else {
          var activePatternResult708 = (0, _Parser.$7C$X$7C$)(_arg1);
          return _Parser.Token[".ctor_2"](activePatternResult708[0], (0, _List.map)(mapAllNames(action), activePatternResult708[1]));
        }
      };
    };

    var toAllNames = function toAllNames(action_1) {
      return function ($var247) {
        return function (value) {
          value;
        }(mapAllNames(function (e_1) {
          action_1(e_1);
          return e_1;
        })($var247));
      };
    };

    var getNameExclude = function getNameExclude(s_3) {
      return function (e_2) {
        if (e_2 === s_3) {
          return e_2;
        } else {
          return getName(e_2);
        }
      };
    };

    var changeNames_1 = function changeNames_1(_arg2) {
      changeNames_1: while (true) {
        var $var248 = void 0;
        var activePatternResult724 = (0, _Parser.$7C$X$7C$)(_arg2);

        if (activePatternResult724[0] === "sequence") {
          $var248 = [0, activePatternResult724[1]];
        } else if (activePatternResult724[0] === "let") {
          $var248 = [1, _arg2];
        } else if (activePatternResult724[0] === "let rec") {
          $var248 = [1, _arg2];
        } else if (activePatternResult724[0] === "fun") {
          if (activePatternResult724[1].tail != null) {
            if (activePatternResult724[1].tail.tail != null) {
              if (activePatternResult724[1].tail.tail.tail == null) {
                $var248 = [2, activePatternResult724[1].head, activePatternResult724[1].tail.head];
              } else {
                var activePatternResult725 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

                if (activePatternResult725.Case === "Choice1Of3") {
                  $var248 = [5, activePatternResult725.Fields[0]];
                } else {
                  $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
                }
              }
            } else {
              var activePatternResult726 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

              if (activePatternResult726.Case === "Choice1Of3") {
                $var248 = [5, activePatternResult726.Fields[0]];
              } else {
                $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
              }
            }
          } else {
            var activePatternResult727 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

            if (activePatternResult727.Case === "Choice1Of3") {
              $var248 = [5, activePatternResult727.Fields[0]];
            } else {
              $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
            }
          }
        } else if (activePatternResult724[0] === "for") {
          if (activePatternResult724[1].tail != null) {
            if (activePatternResult724[1].tail.tail != null) {
              if (activePatternResult724[1].tail.tail.tail != null) {
                if (activePatternResult724[1].tail.tail.tail.tail == null) {
                  $var248 = [3, activePatternResult724[1].tail.head, activePatternResult724[1].head, activePatternResult724[1].tail.tail.head];
                } else {
                  var activePatternResult728 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

                  if (activePatternResult728.Case === "Choice1Of3") {
                    $var248 = [5, activePatternResult728.Fields[0]];
                  } else {
                    $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
                  }
                }
              } else {
                var activePatternResult729 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

                if (activePatternResult729.Case === "Choice1Of3") {
                  $var248 = [5, activePatternResult729.Fields[0]];
                } else {
                  $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
                }
              }
            } else {
              var activePatternResult730 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

              if (activePatternResult730.Case === "Choice1Of3") {
                $var248 = [5, activePatternResult730.Fields[0]];
              } else {
                $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
              }
            }
          } else {
            var activePatternResult731 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

            if (activePatternResult731.Case === "Choice1Of3") {
              $var248 = [5, activePatternResult731.Fields[0]];
            } else {
              $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
            }
          }
        } else if (activePatternResult724[0] === "struct") {
          if (activePatternResult724[1].tail != null) {
            if (activePatternResult724[1].tail.tail != null) {
              if (activePatternResult724[1].tail.tail.tail == null) {
                $var248 = [4, activePatternResult724[1].tail.head, activePatternResult724[1].head];
              } else {
                var activePatternResult732 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

                if (activePatternResult732.Case === "Choice1Of3") {
                  $var248 = [5, activePatternResult732.Fields[0]];
                } else {
                  $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
                }
              }
            } else {
              var activePatternResult733 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

              if (activePatternResult733.Case === "Choice1Of3") {
                $var248 = [5, activePatternResult733.Fields[0]];
              } else {
                $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
              }
            }
          } else {
            var activePatternResult734 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

            if (activePatternResult734.Case === "Choice1Of3") {
              $var248 = [5, activePatternResult734.Fields[0]];
            } else {
              $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
            }
          }
        } else {
          var activePatternResult735 = (0, _Parser.$7C$Var$7C$Cnst$7C$Other$7C$)(_arg2);

          if (activePatternResult735.Case === "Choice1Of3") {
            $var248 = [5, activePatternResult735.Fields[0]];
          } else {
            $var248 = [6, activePatternResult724[1], activePatternResult724[0]];
          }
        }

        var _ret = function () {
          switch ($var248[0]) {
            case 0:
              var scan = function scan(_arg3) {
                var $var249 = void 0;

                if (_arg3.tail == null) {
                  $var249 = [2];
                } else {
                  var activePatternResult717 = (0, _Parser.$7C$X$7C$)(_arg3.head);

                  if (activePatternResult717[0] === "let") {
                    if (activePatternResult717[1].tail != null) {
                      if (activePatternResult717[1].tail.tail != null) {
                        if (activePatternResult717[1].tail.tail.tail == null) {
                          $var249 = [0, activePatternResult717[0], activePatternResult717[1].head, _arg3.tail, activePatternResult717[1].tail.head];
                        } else {
                          $var249 = [1, _arg3.head, _arg3.tail];
                        }
                      } else {
                        $var249 = [1, _arg3.head, _arg3.tail];
                      }
                    } else {
                      $var249 = [1, _arg3.head, _arg3.tail];
                    }
                  } else if (activePatternResult717[0] === "let rec") {
                    if (activePatternResult717[1].tail != null) {
                      if (activePatternResult717[1].tail.tail != null) {
                        if (activePatternResult717[1].tail.tail.tail == null) {
                          $var249 = [0, activePatternResult717[0], activePatternResult717[1].head, _arg3.tail, activePatternResult717[1].tail.head];
                        } else {
                          $var249 = [1, _arg3.head, _arg3.tail];
                        }
                      } else {
                        $var249 = [1, _arg3.head, _arg3.tail];
                      }
                    } else {
                      $var249 = [1, _arg3.head, _arg3.tail];
                    }
                  } else {
                    $var249 = [1, _arg3.head, _arg3.tail];
                  }
                }

                switch ($var249[0]) {
                  case 0:
                    toAllNames(addName)($var249[2]);

                    try {
                      return new _List2.default(_Parser.Token[".ctor_2"]("let", (0, _List.ofArray)([mapAllNames(getNameExclude("declare"))($var249[2]), $var249[1] === "let" ? changeNames_1($var249[4]) : $var249[4]])), scan($var249[3]));
                    } finally {
                      toAllNames(popName)($var249[2]);
                    }

                  case 1:
                    return new _List2.default(function (arg00) {
                      return changeNames_1(arg00);
                    }($var249[1]), scan($var249[2]));

                  case 2:
                    return new _List2.default();
                }
              };

              return {
                v: _Parser.Token[".ctor_2"]("sequence", scan($var248[1]))
              };

            case 1:
              _arg2 = _Parser.Token[".ctor_2"]("sequence", (0, _List.ofArray)([$var248[1]]));
              return "continue|changeNames_1";

            case 2:
              toAllNames(addName)($var248[1]);

              try {
                return {
                  v: _Parser.Token[".ctor_2"]("fun", (0, _List.ofArray)([mapAllNames(getNameExclude("declare"))($var248[1]), changeNames_1($var248[2])]))
                };
              } finally {
                toAllNames(popName)($var248[1]);
              }

            case 3:
              toAllNames(addName)($var248[2]);

              try {
                return {
                  v: _Parser.Token[".ctor_2"]("for", (0, _List.ofArray)([mapAllNames(getNameExclude("declare"))($var248[2]), $var248[1], changeNames_1($var248[3])]))
                };
              } finally {
                toAllNames(popName)($var248[2]);
              }

            case 4:
              return {
                v: _Parser.Token[".ctor_2"]("struct", (0, _List.ofArray)([$var248[2], changeNames_1($var248[1])]))
              };

            case 5:
              return {
                v: _Parser.Token[".ctor_3"](getName($var248[1]))
              };

            case 6:
              return {
                v: _Parser.Token[".ctor_2"]($var248[2], (0, _List.map)(changeNames_1, $var248[1]))
              };
          }
        }();

        switch (_ret) {
          case "continue|changeNames_1":
            continue changeNames_1;

          default:
            if ((typeof _ret === "undefined" ? "undefined" : _typeof(_ret)) === "object") return _ret.v;
        }
      }
    };

    return changeNames_1(e);
  }

  var noObject = exports.noObject = new Map(new _List2.default());
  var objectTypes = exports.objectTypes = new Map();

  function compileObjectDeclarations(_arg1) {
    var $var250 = void 0;
    var activePatternResult754 = (0, _Parser.$7C$X$7C$)(_arg1);

    if (activePatternResult754[0] === "struct") {
      if (activePatternResult754[1].tail != null) {
        var activePatternResult755 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult754[1].head);

        if (activePatternResult755 != null) {
          if (activePatternResult754[1].tail.tail != null) {
            if (activePatternResult754[1].tail.tail.tail == null) {
              $var250 = [0, activePatternResult754[1].tail.head, activePatternResult755];
            } else {
              $var250 = [1];
            }
          } else {
            $var250 = [1];
          }
        } else {
          $var250 = [1];
        }
      } else {
        $var250 = [1];
      }
    } else {
      $var250 = [1];
    }

    switch ($var250[0]) {
      case 0:
        var activePatternResult752 = (0, _Parser.$7C$X$7C$)($var250[1]);

        if (activePatternResult752[0] === "sequence") {
          var mapMembers = new Map((0, _List.mapIndexed)(function (i, e) {
            var $var251 = void 0;
            var activePatternResult744 = (0, _Parser.$7C$X$7C$)(e);

            if (activePatternResult744[0] === "let") {
              if (activePatternResult744[1].tail != null) {
                var activePatternResult745 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult744[1].head);

                if (activePatternResult745 != null) {
                  if (activePatternResult744[1].tail.tail != null) {
                    var activePatternResult746 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult744[1].tail.head);

                    if (activePatternResult746 != null) {
                      if (activePatternResult746 === "nothing") {
                        if (activePatternResult744[1].tail.tail.tail == null) {
                          $var251 = [0, activePatternResult745];
                        } else {
                          $var251 = [1];
                        }
                      } else {
                        $var251 = [1];
                      }
                    } else {
                      $var251 = [1];
                    }
                  } else {
                    $var251 = [1];
                  }
                } else {
                  var activePatternResult747 = (0, _Parser.$7C$X$7C$)(activePatternResult744[1].head);

                  if (activePatternResult747[0] === "declare") {
                    if (activePatternResult747[1].tail != null) {
                      if (activePatternResult747[1].tail.tail != null) {
                        var activePatternResult748 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult747[1].tail.head);

                        if (activePatternResult748 != null) {
                          if (activePatternResult747[1].tail.tail.tail == null) {
                            if (activePatternResult744[1].tail.tail != null) {
                              var activePatternResult749 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult744[1].tail.head);

                              if (activePatternResult749 != null) {
                                if (activePatternResult749 === "nothing") {
                                  if (activePatternResult744[1].tail.tail.tail == null) {
                                    $var251 = [0, activePatternResult748];
                                  } else {
                                    $var251 = [1];
                                  }
                                } else {
                                  $var251 = [1];
                                }
                              } else {
                                $var251 = [1];
                              }
                            } else {
                              $var251 = [1];
                            }
                          } else {
                            $var251 = [1];
                          }
                        } else {
                          $var251 = [1];
                        }
                      } else {
                        $var251 = [1];
                      }
                    } else {
                      $var251 = [1];
                    }
                  } else {
                    $var251 = [1];
                  }
                }
              } else {
                $var251 = [1];
              }
            } else {
              $var251 = [1];
            }

            switch ($var251[0]) {
              case 0:
                return [$var251[1], [i, null]];

              case 1:
                var $var252 = void 0;
                var activePatternResult740 = (0, _Parser.$7C$X$7C$)(e);

                if (activePatternResult740[0] === "let") {
                  if (activePatternResult740[1].tail != null) {
                    var activePatternResult741 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult740[1].head);

                    if (activePatternResult741 != null) {
                      if (activePatternResult740[1].tail.tail != null) {
                        if (activePatternResult740[1].tail.tail.tail == null) {
                          $var252 = [0, activePatternResult740[1].tail.head, activePatternResult741];
                        } else {
                          $var252 = [1];
                        }
                      } else {
                        $var252 = [1];
                      }
                    } else {
                      var activePatternResult742 = (0, _Parser.$7C$X$7C$)(activePatternResult740[1].head);

                      if (activePatternResult742[0] === "declare") {
                        if (activePatternResult742[1].tail != null) {
                          if (activePatternResult742[1].tail.tail != null) {
                            var activePatternResult743 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult742[1].tail.head);

                            if (activePatternResult743 != null) {
                              if (activePatternResult742[1].tail.tail.tail == null) {
                                if (activePatternResult740[1].tail.tail != null) {
                                  if (activePatternResult740[1].tail.tail.tail == null) {
                                    $var252 = [0, activePatternResult740[1].tail.head, activePatternResult743];
                                  } else {
                                    $var252 = [1];
                                  }
                                } else {
                                  $var252 = [1];
                                }
                              } else {
                                $var252 = [1];
                              }
                            } else {
                              $var252 = [1];
                            }
                          } else {
                            $var252 = [1];
                          }
                        } else {
                          $var252 = [1];
                        }
                      } else {
                        $var252 = [1];
                      }
                    }
                  } else {
                    $var252 = [1];
                  }
                } else {
                  $var252 = [1];
                }

                switch ($var252[0]) {
                  case 0:
                    return [$var252[2], [i, $var252[1]]];

                  case 1:
                    throw new Error("wrong member format");
                }

            }
          }, activePatternResult752[1]));
          objectTypes.set("struct " + $var250[2], mapMembers);
        } else {
          throw new Error("wrong struct format");
        }

        break;

      case 1:
        throw new Error("should never happen");
        break;
    }
  }

  var varTypes = exports.varTypes = new Map();

  function compileObjects(_arg1) {
    var $var253 = void 0;
    var activePatternResult775 = (0, _Parser.$7C$X$7C$)(_arg1);

    if (activePatternResult775[0] === "struct") {
      $var253 = [0, _arg1];
    } else if (activePatternResult775[0] === "dot") {
      if (activePatternResult775[1].tail != null) {
        var activePatternResult776 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult775[1].head);

        if (activePatternResult776 != null) {
          if (activePatternResult775[1].tail.tail != null) {
            var activePatternResult777 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult775[1].tail.head);

            if (activePatternResult777 != null) {
              if (activePatternResult775[1].tail.tail.tail == null) {
                $var253 = [1, activePatternResult776, activePatternResult777];
              } else {
                $var253 = [2];
              }
            } else {
              $var253 = [2];
            }
          } else {
            $var253 = [2];
          }
        } else {
          $var253 = [2];
        }
      } else {
        $var253 = [2];
      }
    } else {
      $var253 = [2];
    }

    switch ($var253[0]) {
      case 0:
        compileObjectDeclarations($var253[1]);
        return _Parser.Token[".ctor_3"]("()");

      case 1:
        return _Parser.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Parser.Token[".ctor_3"]($var253[1]), _Parser.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Parser.Token[".ctor_3"](String(varTypes.get($var253[1]).head.get($var253[2])[0]))]))]));

      case 2:
        var activePatternResult774 = (0, _Parser.$7C$X$7C$)(_arg1);

        if (activePatternResult774[0] === "sequence") {
          var yld = function (e) {
            return _Parser.Token[".ctor_2"]("sequence", e);
          }((0, _List.collect)(function (_arg2) {
            var $var254 = void 0;
            var activePatternResult761 = (0, _Parser.$7C$X$7C$)(_arg2);

            if (activePatternResult761[0] === "let") {
              if (activePatternResult761[1].tail != null) {
                var activePatternResult762 = (0, _Parser.$7C$X$7C$)(activePatternResult761[1].head);

                if (activePatternResult762[0] === "declare") {
                  if (activePatternResult762[1].tail != null) {
                    var activePatternResult763 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult762[1].head);

                    if (activePatternResult763 != null) {
                      if (activePatternResult762[1].tail.tail != null) {
                        var activePatternResult764 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult762[1].tail.head);

                        if (activePatternResult764 != null) {
                          if (activePatternResult762[1].tail.tail.tail == null) {
                            if (activePatternResult761[1].tail.tail != null) {
                              if (activePatternResult761[1].tail.tail.tail == null) {
                                $var254 = [0, activePatternResult764, activePatternResult761[1].tail.head, _arg2, activePatternResult763];
                              } else {
                                $var254 = [1];
                              }
                            } else {
                              $var254 = [1];
                            }
                          } else {
                            $var254 = [1];
                          }
                        } else {
                          $var254 = [1];
                        }
                      } else {
                        $var254 = [1];
                      }
                    } else {
                      $var254 = [1];
                    }
                  } else {
                    $var254 = [1];
                  }
                } else {
                  $var254 = [1];
                }
              } else {
                $var254 = [1];
              }
            } else {
              $var254 = [1];
            }

            switch ($var254[0]) {
              case 0:
                var mapping = objectTypes.has($var254[4]) ? objectTypes.get($var254[4]) : noObject;

                if (!varTypes.has($var254[1])) {
                  varTypes.set($var254[1], new _List2.default());
                }

                varTypes.set($var254[1], new _List2.default(mapping, varTypes.get($var254[1])));
                var matchValue = [(0, _Seq.count)(mapping), $var254[2]];
                var $var255 = void 0;
                var activePatternResult758 = (0, _Parser.$7C$T$7C$_$7C$)(matchValue[1]);

                if (activePatternResult758 != null) {
                  if (activePatternResult758 === "nothing") {
                    if (matchValue[0] > 0) {
                      $var255 = [0, matchValue[0]];
                    } else {
                      $var255 = [1];
                    }
                  } else {
                    $var255 = [1];
                  }
                } else {
                  $var255 = [1];
                }

                switch ($var255[0]) {
                  case 0:
                    var allocate = _Parser.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Parser.Token[".ctor_3"]($var254[4]), _Parser.Token[".ctor_3"]($var254[1])])), _Parser.Token[".ctor_2"]("array", (0, _List.ofArray)([_Parser.Token[".ctor_3"](String($var255[1]))]))]));

                    var initializeMembers = (0, _Seq.toList)((0, _Seq.collect)(function (_arg3) {
                      if (_arg3[1] == null) {
                        return new _List2.default();
                      } else {
                        var v = _arg3[1];
                        return (0, _List.ofArray)([_Parser.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Parser.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Parser.Token[".ctor_3"]($var254[1]), _Parser.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Parser.Token[".ctor_3"](String(_arg3[0]))]))])), v]))]);
                      }
                    }, mapping.values()));
                    return new _List2.default(allocate, initializeMembers);

                  case 1:
                    return (0, _List.ofArray)([compileObjects($var254[3])]);
                }

              case 1:
                return (0, _List.ofArray)([compileObjects(_arg2)]);
            }
          }, activePatternResult774[1]));

          (0, _Seq.iterate)(function (_arg4) {
            var $var256 = void 0;
            var activePatternResult769 = (0, _Parser.$7C$X$7C$)(_arg4);

            if (activePatternResult769[0] === "let") {
              if (activePatternResult769[1].tail != null) {
                var activePatternResult770 = (0, _Parser.$7C$X$7C$)(activePatternResult769[1].head);

                if (activePatternResult770[0] === "declare") {
                  if (activePatternResult770[1].tail != null) {
                    if (activePatternResult770[1].tail.tail != null) {
                      var activePatternResult771 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult770[1].tail.head);

                      if (activePatternResult771 != null) {
                        if (activePatternResult770[1].tail.tail.tail == null) {
                          if (activePatternResult769[1].tail.tail != null) {
                            if (activePatternResult769[1].tail.tail.tail == null) {
                              $var256 = [0, activePatternResult771, activePatternResult769[1].tail.head];
                            } else {
                              $var256 = [1];
                            }
                          } else {
                            $var256 = [1];
                          }
                        } else {
                          $var256 = [1];
                        }
                      } else {
                        $var256 = [1];
                      }
                    } else {
                      $var256 = [1];
                    }
                  } else {
                    $var256 = [1];
                  }
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
                varTypes.set($var256[1], varTypes.get($var256[1]).tail);
                break;

              case 1:
                break;
            }
          }, activePatternResult774[1]);
          return yld;
        } else {
          return _Parser.Token[".ctor_2"](activePatternResult774[0], (0, _List.map)(function (_arg1_1) {
            return compileObjects(_arg1_1);
          }, activePatternResult774[1]));
        }

    }
  }

  function restoreDefault() {
    objectTypes.clear();
    varTypes.clear();
  }

  function compileObjectsToArrays(x) {
    restoreDefault();
    return compileObjects(x);
  }

  var LabelledToken = function () {
    function LabelledToken(caseName, fields) {
      _classCallCheck(this, LabelledToken);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(LabelledToken, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Parser.TypeValidation.LabelledToken",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            LT: ["string", (0, _Util.makeGeneric)(_List2.default, {
              T: LabelledToken
            }), "number"]
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

    return LabelledToken;
  }();

  (0, _Symbol2.setType)("Parser.TypeValidation.LabelledToken", LabelledToken);

  function compilePointersToArrays(x) {
    var acc = {
      contents: -1
    };

    var nxt = function nxt() {
      void acc.contents++;
      return acc.contents;
    };

    var labelTokens = function labelTokens(labels) {
      return function (_arg1) {
        var $var257 = void 0;
        var activePatternResult808 = (0, _Parser.$7C$T$7C$_$7C$)(_arg1);

        if (activePatternResult808 != null) {
          if (labels.has(activePatternResult808)) {
            $var257 = [0, activePatternResult808];
          } else {
            $var257 = [1];
          }
        } else {
          $var257 = [1];
        }

        switch ($var257[0]) {
          case 0:
            return new LabelledToken("LT", [$var257[1], new _List2.default(), labels.get($var257[1])]);

          case 1:
            var activePatternResult807 = (0, _Parser.$7C$X$7C$)(_arg1);

            if (activePatternResult807[0] === "sequence") {
              var nodes_ = (0, _List.reverse)((0, _Seq.fold)(function (tupledArg, _arg2) {
                var $var258 = void 0;
                var activePatternResult800 = (0, _Parser.$7C$X$7C$)(_arg2);

                if (activePatternResult800[0] === "let") {
                  if (activePatternResult800[1].tail != null) {
                    var activePatternResult801 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult800[1].head);

                    if (activePatternResult801 != null) {
                      if (activePatternResult800[1].tail.tail != null) {
                        if (activePatternResult800[1].tail.tail.tail == null) {
                          $var258 = [0, activePatternResult800[1].head, activePatternResult800[1].tail.head, activePatternResult801];
                        } else {
                          $var258 = [1];
                        }
                      } else {
                        $var258 = [1];
                      }
                    } else {
                      var activePatternResult802 = (0, _Parser.$7C$X$7C$)(activePatternResult800[1].head);

                      if (activePatternResult802[0] === "declare") {
                        if (activePatternResult802[1].tail != null) {
                          if (activePatternResult802[1].tail.tail != null) {
                            var activePatternResult803 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult802[1].tail.head);

                            if (activePatternResult803 != null) {
                              if (activePatternResult802[1].tail.tail.tail == null) {
                                if (activePatternResult800[1].tail.tail != null) {
                                  if (activePatternResult800[1].tail.tail.tail == null) {
                                    $var258 = [0, activePatternResult800[1].head, activePatternResult800[1].tail.head, activePatternResult803];
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
                        } else {
                          $var258 = [1];
                        }
                      } else {
                        $var258 = [1];
                      }
                    }
                  } else {
                    $var258 = [1];
                  }
                } else {
                  $var258 = [1];
                }

                switch ($var258[0]) {
                  case 0:
                    var x_1 = nxt(null);
                    var newLabels = (0, _Map.add)($var258[3], x_1, tupledArg[0]);
                    var newNode = new LabelledToken("LT", ["let", (0, _List.ofArray)([labelTokens(newLabels)($var258[1]), labelTokens(newLabels)($var258[2])]), -1]);
                    return [newLabels, new _List2.default(newNode, tupledArg[1])];

                  case 1:
                    return [tupledArg[0], new _List2.default(labelTokens(tupledArg[0])(_arg2), tupledArg[1])];
                }
              }, [labels, new _List2.default()], activePatternResult807[1])[1]);
              return new LabelledToken("LT", ["sequence", nodes_, -1]);
            } else {
              return new LabelledToken("LT", [activePatternResult807[0], (0, _List.map)(labelTokens(labels), activePatternResult807[1]), -1]);
            }

        }
      };
    };

    var k = labelTokens((0, _Map.create)(null, new _GenericComparer2.default(_Util.compare)))(x);
    var hasReference = Array.from((0, _Seq.replicate)(nxt(null), false));

    var findAllDerefs = function findAllDerefs(_arg3) {
      var $var259 = _arg3.Fields[0] === "apply" ? _arg3.Fields[1].tail != null ? _arg3.Fields[1].head.Fields[0] === "~&" ? _arg3.Fields[1].head.Fields[1].tail == null ? _arg3.Fields[1].tail.tail != null ? _arg3.Fields[1].tail.head.Fields[1].tail == null ? _arg3.Fields[1].tail.tail.tail == null ? function () {
        var s = _arg3.Fields[1].tail.head.Fields[0];
        var a = _arg3.Fields[1].tail.head.Fields[2];
        return a !== -1;
      }() ? [0, _arg3.Fields[1].tail.head.Fields[2], _arg3.Fields[1].tail.head.Fields[0]] : [1] : [1] : [1] : [1] : [1] : [1] : [1] : [1];

      switch ($var259[0]) {
        case 0:
          hasReference[$var259[1]] = true;
          break;

        case 1:
          (0, _Seq.iterate)(findAllDerefs, _arg3.Fields[1]);
          break;
      }
    };

    findAllDerefs(k);

    var _IsDeref___ = function _IsDeref___(_arg4) {
      if (_arg4.Fields[1].tail == null) {
        if (_arg4.Fields[2] !== -1 ? hasReference[_arg4.Fields[2]] : false) {
          return _arg4.Fields[0];
        } else {
          return null;
        }
      } else {
        return null;
      }
    };

    var mapDerefs = function mapDerefs(_arg5) {
      var activePatternResult817 = _IsDeref___(_arg5);

      if (activePatternResult817 != null) {
        return _Parser.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Parser.Token[".ctor_3"](activePatternResult817), _Parser.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("0")]))]));
      } else {
        var $var260 = void 0;

        if (_arg5.Fields[0] === "apply") {
          if (_arg5.Fields[1].tail != null) {
            if (_arg5.Fields[1].head.Fields[0] === "~&") {
              if (_arg5.Fields[1].head.Fields[1].tail == null) {
                if (_arg5.Fields[1].tail.tail != null) {
                  var activePatternResult816 = _IsDeref___(_arg5.Fields[1].tail.head);

                  if (activePatternResult816 != null) {
                    if (_arg5.Fields[1].tail.tail.tail == null) {
                      $var260 = [0, activePatternResult816];
                    } else {
                      $var260 = [1];
                    }
                  } else {
                    $var260 = [1];
                  }
                } else {
                  $var260 = [1];
                }
              } else {
                $var260 = [1];
              }
            } else {
              $var260 = [1];
            }
          } else {
            $var260 = [1];
          }
        } else {
          $var260 = [1];
        }

        switch ($var260[0]) {
          case 0:
            return _Parser.Token[".ctor_3"]($var260[1]);

          case 1:
            if (_arg5.Fields[0] === "sequence") {
              var nodes__1 = (0, _List.collect)(function (_arg6) {
                var $var261 = void 0;

                if (_arg6.Fields[0] === "let") {
                  if (_arg6.Fields[1].tail != null) {
                    var activePatternResult813 = _IsDeref___(_arg6.Fields[1].head);

                    if (activePatternResult813 != null) {
                      if (_arg6.Fields[1].tail.tail != null) {
                        if (_arg6.Fields[1].tail.tail.tail == null) {
                          $var261 = [0, activePatternResult813, _arg6.Fields[1].tail.head];
                        } else {
                          $var261 = [1];
                        }
                      } else {
                        $var261 = [1];
                      }
                    } else if (_arg6.Fields[1].head.Fields[0] === "declare") {
                      if (_arg6.Fields[1].head.Fields[1].tail != null) {
                        if (_arg6.Fields[1].head.Fields[1].tail.tail != null) {
                          var activePatternResult814 = _IsDeref___(_arg6.Fields[1].head.Fields[1].tail.head);

                          if (activePatternResult814 != null) {
                            if (_arg6.Fields[1].head.Fields[1].tail.tail.tail == null) {
                              if (_arg6.Fields[1].tail.tail != null) {
                                if (_arg6.Fields[1].tail.tail.tail == null) {
                                  $var261 = [0, activePatternResult814, _arg6.Fields[1].tail.head];
                                } else {
                                  $var261 = [1];
                                }
                              } else {
                                $var261 = [1];
                              }
                            } else {
                              $var261 = [1];
                            }
                          } else {
                            $var261 = [1];
                          }
                        } else {
                          $var261 = [1];
                        }
                      } else {
                        $var261 = [1];
                      }
                    } else {
                      $var261 = [1];
                    }
                  } else {
                    $var261 = [1];
                  }
                } else {
                  $var261 = [1];
                }

                switch ($var261[0]) {
                  case 0:
                    return (0, _List.ofArray)([_Parser.Token[".ctor_2"]("let", (0, _List.ofArray)([_Parser.Token[".ctor_3"]($var261[1]), _Parser.Token[".ctor_2"]("array", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("1")]))])), _Parser.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Parser.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Parser.Token[".ctor_3"]($var261[1]), _Parser.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("0")]))])), mapDerefs($var261[2])]))]);

                  case 1:
                    return (0, _List.ofArray)([mapDerefs(_arg6)]);
                }
              }, _arg5.Fields[1]);
              return _Parser.Token[".ctor_2"]("sequence", nodes__1);
            } else {
              return _Parser.Token[".ctor_2"](_arg5.Fields[0], (0, _List.map)(mapDerefs, _arg5.Fields[1]));
            }

        }
      }
    };

    var yld = mapDerefs(k);
    return yld;
  }

  function processDerefs(_arg1) {
    var $var262 = void 0;
    var activePatternResult822 = (0, _Parser.$7C$X$7C$)(_arg1);

    if (activePatternResult822[0] === "apply") {
      if (activePatternResult822[1].tail != null) {
        var activePatternResult823 = (0, _Parser.$7C$T$7C$_$7C$)(activePatternResult822[1].head);

        if (activePatternResult823 != null) {
          if (activePatternResult823 === "~*") {
            if (activePatternResult822[1].tail.tail != null) {
              if (activePatternResult822[1].tail.tail.tail == null) {
                $var262 = [0, activePatternResult822[1].tail.head];
              } else {
                $var262 = [1];
              }
            } else {
              $var262 = [1];
            }
          } else {
            $var262 = [1];
          }
        } else {
          $var262 = [1];
        }
      } else {
        $var262 = [1];
      }
    } else {
      $var262 = [1];
    }

    switch ($var262[0]) {
      case 0:
        return _Parser.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var262[1], _Parser.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Parser.Token[".ctor_3"]("0")]))]));

      case 1:
        var activePatternResult821 = (0, _Parser.$7C$X$7C$)(_arg1);
        return _Parser.Token[".ctor_2"](activePatternResult821[0], (0, _List.map)(function (_arg1_1) {
          return processDerefs(_arg1_1);
        }, activePatternResult821[1]));
    }
  }

  var validateTypes = exports.validateTypes = function validateTypes($var265) {
    return function (_arg1) {
      return processDerefs(_arg1);
    }(function ($var264) {
      return function (x) {
        return compilePointersToArrays(x);
      }(function ($var263) {
        return changeNames(compileObjectsToArrays($var263));
      }($var264));
    }($var265));
  };
});