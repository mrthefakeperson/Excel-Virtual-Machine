define(["exports", "fable-core/umd/List", "./Token", "fable-core/umd/Seq", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/Map", "fable-core/umd/GenericComparer"], function (exports, _List, _Token, _Seq, _Symbol2, _Util, _Map, _GenericComparer) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.applyTypeSystem = exports.LabelledToken = exports.varTypes = exports.objectTypes = exports.noObject = undefined;
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

  var noObject = exports.noObject = new Map(new _List2.default());
  var objectTypes = exports.objectTypes = new Map();

  function compileObjectDeclarations(_arg1) {
    var $var235 = void 0;

    var activePatternResult672 = _Token.Token["|X|"](_arg1);

    if (activePatternResult672[0] === "struct") {
      if (activePatternResult672[1].tail != null) {
        var activePatternResult673 = _Token.Token["|T|_|"](activePatternResult672[1].head);

        if (activePatternResult673 != null) {
          if (activePatternResult672[1].tail.tail != null) {
            if (activePatternResult672[1].tail.tail.tail == null) {
              $var235 = [0, activePatternResult672[1].tail.head, activePatternResult673];
            } else {
              $var235 = [1];
            }
          } else {
            $var235 = [1];
          }
        } else {
          $var235 = [1];
        }
      } else {
        $var235 = [1];
      }
    } else {
      $var235 = [1];
    }

    switch ($var235[0]) {
      case 0:
        var activePatternResult670 = _Token.Token["|X|"]($var235[1]);

        if (activePatternResult670[0] === "sequence") {
          var mapMembers = new Map((0, _List.mapIndexed)(function (i, e) {
            var $var236 = void 0;

            var activePatternResult662 = _Token.Token["|X|"](e);

            if (activePatternResult662[0] === "let") {
              if (activePatternResult662[1].tail != null) {
                var activePatternResult663 = _Token.Token["|T|_|"](activePatternResult662[1].head);

                if (activePatternResult663 != null) {
                  if (activePatternResult662[1].tail.tail != null) {
                    var activePatternResult664 = _Token.Token["|T|_|"](activePatternResult662[1].tail.head);

                    if (activePatternResult664 != null) {
                      if (activePatternResult664 === "nothing") {
                        if (activePatternResult662[1].tail.tail.tail == null) {
                          $var236 = [0, activePatternResult663];
                        } else {
                          $var236 = [1];
                        }
                      } else {
                        $var236 = [1];
                      }
                    } else {
                      $var236 = [1];
                    }
                  } else {
                    $var236 = [1];
                  }
                } else {
                  var activePatternResult665 = _Token.Token["|X|"](activePatternResult662[1].head);

                  if (activePatternResult665[0] === "declare") {
                    if (activePatternResult665[1].tail != null) {
                      if (activePatternResult665[1].tail.tail != null) {
                        var activePatternResult666 = _Token.Token["|T|_|"](activePatternResult665[1].tail.head);

                        if (activePatternResult666 != null) {
                          if (activePatternResult665[1].tail.tail.tail == null) {
                            if (activePatternResult662[1].tail.tail != null) {
                              var activePatternResult667 = _Token.Token["|T|_|"](activePatternResult662[1].tail.head);

                              if (activePatternResult667 != null) {
                                if (activePatternResult667 === "nothing") {
                                  if (activePatternResult662[1].tail.tail.tail == null) {
                                    $var236 = [0, activePatternResult666];
                                  } else {
                                    $var236 = [1];
                                  }
                                } else {
                                  $var236 = [1];
                                }
                              } else {
                                $var236 = [1];
                              }
                            } else {
                              $var236 = [1];
                            }
                          } else {
                            $var236 = [1];
                          }
                        } else {
                          $var236 = [1];
                        }
                      } else {
                        $var236 = [1];
                      }
                    } else {
                      $var236 = [1];
                    }
                  } else {
                    $var236 = [1];
                  }
                }
              } else {
                $var236 = [1];
              }
            } else {
              $var236 = [1];
            }

            switch ($var236[0]) {
              case 0:
                return [$var236[1], [i, null]];

              case 1:
                var $var237 = void 0;

                var activePatternResult658 = _Token.Token["|X|"](e);

                if (activePatternResult658[0] === "let") {
                  if (activePatternResult658[1].tail != null) {
                    var activePatternResult659 = _Token.Token["|T|_|"](activePatternResult658[1].head);

                    if (activePatternResult659 != null) {
                      if (activePatternResult658[1].tail.tail != null) {
                        if (activePatternResult658[1].tail.tail.tail == null) {
                          $var237 = [0, activePatternResult658[1].tail.head, activePatternResult659];
                        } else {
                          $var237 = [1];
                        }
                      } else {
                        $var237 = [1];
                      }
                    } else {
                      var activePatternResult660 = _Token.Token["|X|"](activePatternResult658[1].head);

                      if (activePatternResult660[0] === "declare") {
                        if (activePatternResult660[1].tail != null) {
                          if (activePatternResult660[1].tail.tail != null) {
                            var activePatternResult661 = _Token.Token["|T|_|"](activePatternResult660[1].tail.head);

                            if (activePatternResult661 != null) {
                              if (activePatternResult660[1].tail.tail.tail == null) {
                                if (activePatternResult658[1].tail.tail != null) {
                                  if (activePatternResult658[1].tail.tail.tail == null) {
                                    $var237 = [0, activePatternResult658[1].tail.head, activePatternResult661];
                                  } else {
                                    $var237 = [1];
                                  }
                                } else {
                                  $var237 = [1];
                                }
                              } else {
                                $var237 = [1];
                              }
                            } else {
                              $var237 = [1];
                            }
                          } else {
                            $var237 = [1];
                          }
                        } else {
                          $var237 = [1];
                        }
                      } else {
                        $var237 = [1];
                      }
                    }
                  } else {
                    $var237 = [1];
                  }
                } else {
                  $var237 = [1];
                }

                switch ($var237[0]) {
                  case 0:
                    return [$var237[2], [i, $var237[1]]];

                  case 1:
                    throw new Error("wrong member format");
                }

            }
          }, activePatternResult670[1]));
          objectTypes.set("struct " + $var235[2], mapMembers);
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
    var $var238 = void 0;

    var activePatternResult693 = _Token.Token["|X|"](_arg1);

    if (activePatternResult693[0] === "struct") {
      $var238 = [0, _arg1];
    } else if (activePatternResult693[0] === "dot") {
      if (activePatternResult693[1].tail != null) {
        var activePatternResult694 = _Token.Token["|T|_|"](activePatternResult693[1].head);

        if (activePatternResult694 != null) {
          if (activePatternResult693[1].tail.tail != null) {
            var activePatternResult695 = _Token.Token["|T|_|"](activePatternResult693[1].tail.head);

            if (activePatternResult695 != null) {
              if (activePatternResult693[1].tail.tail.tail == null) {
                $var238 = [1, activePatternResult694, activePatternResult695];
              } else {
                $var238 = [2];
              }
            } else {
              $var238 = [2];
            }
          } else {
            $var238 = [2];
          }
        } else {
          $var238 = [2];
        }
      } else {
        $var238 = [2];
      }
    } else {
      $var238 = [2];
    }

    switch ($var238[0]) {
      case 0:
        compileObjectDeclarations($var238[1]);
        return _Token.Token.Token[".ctor_3"]("()");

      case 1:
        return _Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var238[1]), _Token.Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](String(varTypes.get($var238[1]).head.get($var238[2])[0]))]))]));

      case 2:
        var activePatternResult692 = _Token.Token["|X|"](_arg1);

        if (activePatternResult692[0] === "sequence") {
          var yld = function (e) {
            return _Token.Token.Token[".ctor_2"]("sequence", e);
          }((0, _List.collect)(function (_arg2) {
            var $var239 = void 0;

            var activePatternResult679 = _Token.Token["|X|"](_arg2);

            if (activePatternResult679[0] === "let") {
              if (activePatternResult679[1].tail != null) {
                var activePatternResult680 = _Token.Token["|X|"](activePatternResult679[1].head);

                if (activePatternResult680[0] === "declare") {
                  if (activePatternResult680[1].tail != null) {
                    var activePatternResult681 = _Token.Token["|T|_|"](activePatternResult680[1].head);

                    if (activePatternResult681 != null) {
                      if (activePatternResult680[1].tail.tail != null) {
                        var activePatternResult682 = _Token.Token["|T|_|"](activePatternResult680[1].tail.head);

                        if (activePatternResult682 != null) {
                          if (activePatternResult680[1].tail.tail.tail == null) {
                            if (activePatternResult679[1].tail.tail != null) {
                              if (activePatternResult679[1].tail.tail.tail == null) {
                                $var239 = [0, activePatternResult682, activePatternResult679[1].tail.head, _arg2, activePatternResult681];
                              } else {
                                $var239 = [1];
                              }
                            } else {
                              $var239 = [1];
                            }
                          } else {
                            $var239 = [1];
                          }
                        } else {
                          $var239 = [1];
                        }
                      } else {
                        $var239 = [1];
                      }
                    } else {
                      $var239 = [1];
                    }
                  } else {
                    $var239 = [1];
                  }
                } else {
                  $var239 = [1];
                }
              } else {
                $var239 = [1];
              }
            } else {
              $var239 = [1];
            }

            switch ($var239[0]) {
              case 0:
                var mapping = objectTypes.has($var239[4]) ? objectTypes.get($var239[4]) : noObject;

                if (!varTypes.has($var239[1])) {
                  varTypes.set($var239[1], new _List2.default());
                }

                varTypes.set($var239[1], new _List2.default(mapping, varTypes.get($var239[1])));
                var matchValue = [(0, _Seq.count)(mapping), $var239[2]];
                var $var240 = void 0;

                var activePatternResult676 = _Token.Token["|T|_|"](matchValue[1]);

                if (activePatternResult676 != null) {
                  if (activePatternResult676 === "nothing") {
                    if (matchValue[0] > 0) {
                      $var240 = [0, matchValue[0]];
                    } else {
                      $var240 = [1];
                    }
                  } else {
                    $var240 = [1];
                  }
                } else {
                  $var240 = [1];
                }

                switch ($var240[0]) {
                  case 0:
                    var allocate = _Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var239[4]), _Token.Token.Token[".ctor_3"]($var239[1])])), _Token.Token.Token[".ctor_2"]("array", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](String($var240[1]))]))]));

                    var initializeMembers = (0, _Seq.toList)((0, _Seq.collect)(function (_arg3) {
                      if (_arg3[1] == null) {
                        return new _List2.default();
                      } else {
                        var v = _arg3[1];
                        return (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var239[1]), _Token.Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](String(_arg3[0]))]))])), v]))]);
                      }
                    }, mapping.values()));
                    return new _List2.default(allocate, initializeMembers);

                  case 1:
                    return (0, _List.ofArray)([compileObjects($var239[3])]);
                }

              case 1:
                return (0, _List.ofArray)([compileObjects(_arg2)]);
            }
          }, activePatternResult692[1]));

          (0, _Seq.iterate)(function (_arg4) {
            var $var241 = void 0;

            var activePatternResult687 = _Token.Token["|X|"](_arg4);

            if (activePatternResult687[0] === "let") {
              if (activePatternResult687[1].tail != null) {
                var activePatternResult688 = _Token.Token["|X|"](activePatternResult687[1].head);

                if (activePatternResult688[0] === "declare") {
                  if (activePatternResult688[1].tail != null) {
                    if (activePatternResult688[1].tail.tail != null) {
                      var activePatternResult689 = _Token.Token["|T|_|"](activePatternResult688[1].tail.head);

                      if (activePatternResult689 != null) {
                        if (activePatternResult688[1].tail.tail.tail == null) {
                          if (activePatternResult687[1].tail.tail != null) {
                            if (activePatternResult687[1].tail.tail.tail == null) {
                              $var241 = [0, activePatternResult689, activePatternResult687[1].tail.head];
                            } else {
                              $var241 = [1];
                            }
                          } else {
                            $var241 = [1];
                          }
                        } else {
                          $var241 = [1];
                        }
                      } else {
                        $var241 = [1];
                      }
                    } else {
                      $var241 = [1];
                    }
                  } else {
                    $var241 = [1];
                  }
                } else {
                  $var241 = [1];
                }
              } else {
                $var241 = [1];
              }
            } else {
              $var241 = [1];
            }

            switch ($var241[0]) {
              case 0:
                varTypes.set($var241[1], varTypes.get($var241[1]).tail);
                break;

              case 1:
                break;
            }
          }, activePatternResult692[1]);
          return yld;
        } else {
          return _Token.Token.Token[".ctor_2"](activePatternResult692[0], (0, _List.map)(function (_arg1_1) {
            return compileObjects(_arg1_1);
          }, activePatternResult692[1]));
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

  var LabelledToken = exports.LabelledToken = function () {
    function LabelledToken(caseName, fields) {
      _classCallCheck(this, LabelledToken);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(LabelledToken, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Type_System.LabelledToken",
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

  (0, _Symbol2.setType)("Type_System.LabelledToken", LabelledToken);

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
        var $var242 = void 0;

        var activePatternResult726 = _Token.Token["|T|_|"](_arg1);

        if (activePatternResult726 != null) {
          if (labels.has(activePatternResult726)) {
            $var242 = [0, activePatternResult726];
          } else {
            $var242 = [1];
          }
        } else {
          $var242 = [1];
        }

        switch ($var242[0]) {
          case 0:
            return new LabelledToken("LT", [$var242[1], new _List2.default(), labels.get($var242[1])]);

          case 1:
            var activePatternResult725 = _Token.Token["|X|"](_arg1);

            if (activePatternResult725[0] === "sequence") {
              var nodes_ = (0, _List.reverse)((0, _Seq.fold)(function (tupledArg, _arg2) {
                var $var243 = void 0;

                var activePatternResult718 = _Token.Token["|X|"](_arg2);

                if (activePatternResult718[0] === "let") {
                  if (activePatternResult718[1].tail != null) {
                    var activePatternResult719 = _Token.Token["|T|_|"](activePatternResult718[1].head);

                    if (activePatternResult719 != null) {
                      if (activePatternResult718[1].tail.tail != null) {
                        if (activePatternResult718[1].tail.tail.tail == null) {
                          $var243 = [0, activePatternResult718[1].head, activePatternResult718[1].tail.head, activePatternResult719];
                        } else {
                          $var243 = [1];
                        }
                      } else {
                        $var243 = [1];
                      }
                    } else {
                      var activePatternResult720 = _Token.Token["|X|"](activePatternResult718[1].head);

                      if (activePatternResult720[0] === "declare") {
                        if (activePatternResult720[1].tail != null) {
                          if (activePatternResult720[1].tail.tail != null) {
                            var activePatternResult721 = _Token.Token["|T|_|"](activePatternResult720[1].tail.head);

                            if (activePatternResult721 != null) {
                              if (activePatternResult720[1].tail.tail.tail == null) {
                                if (activePatternResult718[1].tail.tail != null) {
                                  if (activePatternResult718[1].tail.tail.tail == null) {
                                    $var243 = [0, activePatternResult718[1].head, activePatternResult718[1].tail.head, activePatternResult721];
                                  } else {
                                    $var243 = [1];
                                  }
                                } else {
                                  $var243 = [1];
                                }
                              } else {
                                $var243 = [1];
                              }
                            } else {
                              $var243 = [1];
                            }
                          } else {
                            $var243 = [1];
                          }
                        } else {
                          $var243 = [1];
                        }
                      } else {
                        $var243 = [1];
                      }
                    }
                  } else {
                    $var243 = [1];
                  }
                } else {
                  $var243 = [1];
                }

                switch ($var243[0]) {
                  case 0:
                    var x_1 = nxt(null);
                    var newLabels = (0, _Map.add)($var243[3], x_1, tupledArg[0]);
                    var newNode = new LabelledToken("LT", ["let", (0, _List.ofArray)([labelTokens(newLabels)($var243[1]), labelTokens(newLabels)($var243[2])]), -1]);
                    return [newLabels, new _List2.default(newNode, tupledArg[1])];

                  case 1:
                    return [tupledArg[0], new _List2.default(labelTokens(tupledArg[0])(_arg2), tupledArg[1])];
                }
              }, [labels, new _List2.default()], activePatternResult725[1])[1]);
              return new LabelledToken("LT", ["sequence", nodes_, -1]);
            } else {
              return new LabelledToken("LT", [activePatternResult725[0], (0, _List.map)(labelTokens(labels), activePatternResult725[1]), -1]);
            }

        }
      };
    };

    var k = labelTokens((0, _Map.create)(null, new _GenericComparer2.default(_Util.compare)))(x);
    var hasReference = Array.from((0, _Seq.replicate)(nxt(null), false));

    var findAllDerefs = function findAllDerefs(_arg3) {
      var $var244 = _arg3.Fields[0] === "apply" ? _arg3.Fields[1].tail != null ? _arg3.Fields[1].head.Fields[0] === "~&" ? _arg3.Fields[1].head.Fields[1].tail == null ? _arg3.Fields[1].tail.tail != null ? _arg3.Fields[1].tail.head.Fields[1].tail == null ? _arg3.Fields[1].tail.tail.tail == null ? function () {
        var s = _arg3.Fields[1].tail.head.Fields[0];
        var a = _arg3.Fields[1].tail.head.Fields[2];
        return a !== -1;
      }() ? [0, _arg3.Fields[1].tail.head.Fields[2], _arg3.Fields[1].tail.head.Fields[0]] : [1] : [1] : [1] : [1] : [1] : [1] : [1] : [1];

      switch ($var244[0]) {
        case 0:
          hasReference[$var244[1]] = true;
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
      var activePatternResult735 = _IsDeref___(_arg5);

      if (activePatternResult735 != null) {
        return _Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"](activePatternResult735), _Token.Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("0")]))]));
      } else {
        var $var245 = void 0;

        if (_arg5.Fields[0] === "apply") {
          if (_arg5.Fields[1].tail != null) {
            if (_arg5.Fields[1].head.Fields[0] === "~&") {
              if (_arg5.Fields[1].head.Fields[1].tail == null) {
                if (_arg5.Fields[1].tail.tail != null) {
                  var activePatternResult734 = _IsDeref___(_arg5.Fields[1].tail.head);

                  if (activePatternResult734 != null) {
                    if (_arg5.Fields[1].tail.tail.tail == null) {
                      $var245 = [0, activePatternResult734];
                    } else {
                      $var245 = [1];
                    }
                  } else {
                    $var245 = [1];
                  }
                } else {
                  $var245 = [1];
                }
              } else {
                $var245 = [1];
              }
            } else {
              $var245 = [1];
            }
          } else {
            $var245 = [1];
          }
        } else {
          $var245 = [1];
        }

        switch ($var245[0]) {
          case 0:
            return _Token.Token.Token[".ctor_3"]($var245[1]);

          case 1:
            if (_arg5.Fields[0] === "sequence") {
              var nodes__1 = (0, _List.collect)(function (_arg6) {
                var $var246 = void 0;

                if (_arg6.Fields[0] === "let") {
                  if (_arg6.Fields[1].tail != null) {
                    var activePatternResult731 = _IsDeref___(_arg6.Fields[1].head);

                    if (activePatternResult731 != null) {
                      if (_arg6.Fields[1].tail.tail != null) {
                        if (_arg6.Fields[1].tail.tail.tail == null) {
                          $var246 = [0, activePatternResult731, _arg6.Fields[1].tail.head];
                        } else {
                          $var246 = [1];
                        }
                      } else {
                        $var246 = [1];
                      }
                    } else if (_arg6.Fields[1].head.Fields[0] === "declare") {
                      if (_arg6.Fields[1].head.Fields[1].tail != null) {
                        if (_arg6.Fields[1].head.Fields[1].tail.tail != null) {
                          var activePatternResult732 = _IsDeref___(_arg6.Fields[1].head.Fields[1].tail.head);

                          if (activePatternResult732 != null) {
                            if (_arg6.Fields[1].head.Fields[1].tail.tail.tail == null) {
                              if (_arg6.Fields[1].tail.tail != null) {
                                if (_arg6.Fields[1].tail.tail.tail == null) {
                                  $var246 = [0, activePatternResult732, _arg6.Fields[1].tail.head];
                                } else {
                                  $var246 = [1];
                                }
                              } else {
                                $var246 = [1];
                              }
                            } else {
                              $var246 = [1];
                            }
                          } else {
                            $var246 = [1];
                          }
                        } else {
                          $var246 = [1];
                        }
                      } else {
                        $var246 = [1];
                      }
                    } else {
                      $var246 = [1];
                    }
                  } else {
                    $var246 = [1];
                  }
                } else {
                  $var246 = [1];
                }

                switch ($var246[0]) {
                  case 0:
                    return (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("let", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var246[1]), _Token.Token.Token[".ctor_2"]("array", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("1")]))])), _Token.Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([_Token.Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]($var246[1]), _Token.Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([_Token.Token.Token[".ctor_3"]("0")]))])), mapDerefs($var246[2])]))]);

                  case 1:
                    return (0, _List.ofArray)([mapDerefs(_arg6)]);
                }
              }, _arg5.Fields[1]);
              return _Token.Token.Token[".ctor_2"]("sequence", nodes__1);
            } else {
              return _Token.Token.Token[".ctor_2"](_arg5.Fields[0], (0, _List.map)(mapDerefs, _arg5.Fields[1]));
            }

        }
      }
    };

    var yld = mapDerefs(k);
    return yld;
  }

  function processDerefs(_arg1) {
    var $var247 = void 0;

    var activePatternResult740 = _Token.Token["|X|"](_arg1);

    if (activePatternResult740[0] === "apply") {
      if (activePatternResult740[1].tail != null) {
        var activePatternResult741 = _Token.Token["|T|_|"](activePatternResult740[1].head);

        if (activePatternResult741 != null) {
          if (activePatternResult741 === "~*") {
            if (activePatternResult740[1].tail.tail != null) {
              if (activePatternResult740[1].tail.tail.tail == null) {
                $var247 = [0, activePatternResult740[1].tail.head];
              } else {
                $var247 = [1];
              }
            } else {
              $var247 = [1];
            }
          } else {
            $var247 = [1];
          }
        } else {
          $var247 = [1];
        }
      } else {
        $var247 = [1];
      }
    } else {
      $var247 = [1];
    }

    switch ($var247[0]) {
      case 0:
        return _Token.Token.Token[".ctor_2"]("deref", (0, _List.ofArray)([$var247[1]]));

      case 1:
        var activePatternResult739 = _Token.Token["|X|"](_arg1);

        return _Token.Token.Token[".ctor_2"](activePatternResult739[0], (0, _List.map)(function (_arg1_1) {
          return processDerefs(_arg1_1);
        }, activePatternResult739[1]));
    }
  }

  var applyTypeSystem = exports.applyTypeSystem = function applyTypeSystem($var249) {
    return function (_arg1) {
      return processDerefs(_arg1);
    }(function ($var248) {
      return compilePointersToArrays(compileObjectsToArrays($var248));
    }($var249));
  };
});