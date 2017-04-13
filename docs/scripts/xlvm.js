define(["exports", "fable-core/umd/List", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/Seq", "fable-core/umd/String", "fable-core/umd/Choice", "fable-core/umd/Map", "fable-core/umd/GenericComparer", "fable-core/umd/Set"], function (exports, _List, _Symbol2, _Util, _Seq, _String, _Choice, _Map, _GenericComparer, _Set) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.compileAndRun = exports.Interpreters = exports.createASM = exports.compile$27$ = exports.$7C$Inline$7C$_$7C$ = exports.operationsPrefix = exports.type_int32 = exports.type_string = exports.allTypes = exports.allComb2Sections = exports.x = exports.Add = exports.Equals = exports.Greater = exports.LEq = exports.Mod = exports.allCombinators = exports.C2_Mod = exports.C2_Greater = exports.C2_LEq = exports.C2_Equals = exports.C2_Add = exports.PseudoAsm = exports.comb2 = exports.createAST = exports.ASTCompile$27$ = exports.nxt = exports.nxt$27$ = exports.AST = exports.applyTypeSystem = exports.LabelledToken = exports.varTypes = exports.objectTypes = exports.noObject = exports.C = exports.Lexer = exports.String_Formatting = exports.Token = exports.Symbols = undefined;
  exports.compileObjectType = compileObjectType;
  exports.compileObjects = compileObjects;
  exports.restoreDefault = restoreDefault;
  exports.compileObjectsToArrays = compileObjectsToArrays;
  exports.compilePointersToArrays = compilePointersToArrays;
  exports.processDerefs = processDerefs;
  exports.ASTCompile = ASTCompile;
  exports.isInt = isInt;
  exports.createComb2Section = createComb2Section;
  exports.getSectionAddress = getSectionAddress;
  exports.getSectionAddressFromCmd = getSectionAddressFromCmd;
  exports.getSectionAddressFromInfix = getSectionAddressFromInfix;
  exports._PrintAddress = _PrintAddress;
  exports.compile = compile;

  var _List2 = _interopRequireDefault(_List);

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  var _Choice2 = _interopRequireDefault(_Choice);

  var _GenericComparer2 = _interopRequireDefault(_GenericComparer);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

  function _possibleConstructorReturn(self, call) {
    if (!self) {
      throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
    }

    return call && (typeof call === "object" || typeof call === "function") ? call : self;
  }

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

  var Symbols = exports.Symbols = function (__exports) {
    var infixOrder = __exports.infixOrder = [(0, _List.ofArray)(["*", "/", "%"]), (0, _List.ofArray)(["+", "-"]), (0, _List.append)((0, _List.ofArray)(["=", "<>", ">", ">=", "<", "<="]), (0, _List.ofArray)(["==", "!="])), (0, _List.ofArray)(["&&"]), (0, _List.ofArray)(["||"])];
    var prefixes = __exports.prefixes = (0, _List.ofArray)(["-", "!", "&", "*", "++", "--"]);
    return __exports;
  }({});

  var Token = exports.Token = function (__exports) {
    var Token = __exports.Token = function () {
      _createClass(Token, [{
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            type: "Xlvm.Token.Token",
            properties: {
              CanApply: "boolean",
              Dependants: (0, _Util.makeGeneric)(_List2.default, {
                T: Token
              }),
              Indentation: (0, _Util.Tuple)(["number", "number"]),
              Name: "string",
              Priority: "number",
              Single: "boolean"
            }
          };
        }
      }]);

      function Token(name, row_col, functionApplication, dependants) {
        var _this = this;

        _classCallCheck(this, Token);

        this.name = name;
        this.row_col = row_col;
        this.functionApplication = functionApplication;
        this.dependants = dependants;
        var patternInput = this.row_col;
        this.row = patternInput[0];
        this.col = patternInput[1];
        var matchValue = (0, _Seq.tryFindIndex)(function (e) {
          return (0, _Seq.exists)(function () {
            var x = _this.name;
            return function (y) {
              return x === y;
            };
          }(), e);
        }, Symbols.infixOrder);

        if (matchValue == null) {
          this.priority = -1;
        } else {
          this.priority = matchValue;
        }
      }

      _createClass(Token, [{
        key: "IndentedLess",
        value: function (b) {
          var patternInput = this.Indentation;
          var patternInput_1 = b.Indentation;

          if (b.Name === "fun") {
            if (patternInput[0] === patternInput_1[0]) {
              return true;
            } else {
              return patternInput[1] < patternInput_1[1];
            }
          } else if (patternInput[1] < patternInput_1[1]) {
            return patternInput[0] <= patternInput_1[0];
          } else {
            return false;
          }
        }
      }, {
        key: "EvaluatedFirst",
        value: function (b) {
          return this.Priority <= b.Priority;
        }
      }, {
        key: "ToString",
        value: function () {
          return (0, _String.fsFormat)("%s(%s)")(function (x) {
            return x;
          })(this.name)((0, _String.join)(",", (0, _List.map)((0, _String.fsFormat)("%O")(function (x) {
            return x;
          }), this.dependants)));
        }
      }, {
        key: "ToStringExpr",
        value: function () {
          var matchValue = [this.name, this.dependants];
          var $var1 = matchValue[0] === "apply" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [0, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[0] === "if" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail != null ? matchValue[1].tail.tail.tail.tail == null ? [1, matchValue[1].head, matchValue[1].tail.head, matchValue[1].tail.tail.head] : [15] : [15] : [15] : matchValue[0] === "sequence" ? [2] : matchValue[0] === "()" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail == null ? [3, matchValue[1].head] : [15] : matchValue[0] === "[]" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail == null ? [4, matchValue[1].head] : [15] : matchValue[0] === "dot" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [5, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[0] === "while" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [6, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[0] === "for" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail != null ? matchValue[1].tail.tail.tail.tail == null ? [7, matchValue[1].head, matchValue[1].tail.head, matchValue[1].tail.tail.head] : [15] : [15] : [15] : matchValue[0] === "let" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [8, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[0] === "let rec" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [9, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[0] === "fun" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [10, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[0] === "pattern" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [11, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[0] === "," ? [12, matchValue[1]] : matchValue[0] === "struct" ? matchValue[1].tail == null ? [14] : matchValue[1].tail.tail != null ? matchValue[1].tail.tail.tail == null ? [13, matchValue[1].head, matchValue[1].tail.head] : [15] : [15] : matchValue[1].tail == null ? [14] : [15];

          switch ($var1[0]) {
            case 0:
              return (0, _String.fsFormat)("(%s) (%s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 1:
              return (0, _String.fsFormat)("(if %s then %s else %s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr())($var1[3].ToStringExpr());

            case 2:
              return (0, _String.join)("; ", (0, _List.map)(function (e) {
                return e.ToStringExpr();
              }, this.dependants));

            case 3:
              return (0, _String.fsFormat)("(%s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr());

            case 4:
              return (0, _String.fsFormat)("[%s]")(function (x) {
                return x;
              })($var1[1].ToStringExpr());

            case 5:
              return (0, _String.fsFormat)("%s.%s")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 6:
              return (0, _String.fsFormat)("(while %s do %s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 7:
              return (0, _String.fsFormat)("for %s in %s do (%s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr())($var1[3].ToStringExpr());

            case 8:
              return (0, _String.fsFormat)("let %s = (%s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 9:
              return (0, _String.fsFormat)("let rec %s = (%s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 10:
              return (0, _String.fsFormat)("fun %s -> (%s)")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 11:
              return (0, _String.fsFormat)("| %s -> %s")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 12:
              return (0, _String.join)(", ", (0, _List.map)(function (e_1) {
                return e_1.ToStringExpr();
              }, $var1[1]));

            case 13:
              return (0, _String.fsFormat)("struct %s = {%s}")(function (x) {
                return x;
              })($var1[1].ToStringExpr())($var1[2].ToStringExpr());

            case 14:
              return this.name;

            case 15:
              return this.name + "(" + (0, _String.join)(" ", (0, _List.map)(function (e_2) {
                return e_2.ToStringExpr();
              }, this.dependants)) + ")";
          }
        }
      }, {
        key: "Clean",
        value: function () {
          var matchValue = [this.name, this.dependants];
          var $var2 = matchValue[0] === "sequence" ? matchValue[1].tail != null ? matchValue[1].tail.tail == null ? [0, matchValue[1].head] : [1, matchValue[1]] : [1, matchValue[1]] : matchValue[0] === "()" ? matchValue[1].tail != null ? matchValue[1].tail.tail == null ? [0, matchValue[1].head] : [1, matchValue[1]] : [1, matchValue[1]] : [1, matchValue[1]];

          switch ($var2[0]) {
            case 0:
              return $var2[1].Clean();

            case 1:
              return new Token(this.name, this.row_col, this.functionApplication, (0, _List.map)(function (e) {
                return e.Clean();
              }, $var2[1]));
          }
        }
      }, {
        key: "Name",
        get: function () {
          return this.name;
        }
      }, {
        key: "Dependants",
        get: function () {
          return this.dependants;
        }
      }, {
        key: "CanApply",
        get: function () {
          return this.functionApplication;
        }
      }, {
        key: "Indentation",
        get: function () {
          return [this.row, this.col];
        }
      }, {
        key: "Priority",
        get: function () {
          return this.priority;
        }
      }, {
        key: "Single",
        get: function () {
          return this.dependants.Equals(new _List2.default());
        }
      }], [{
        key: ".ctor_0",
        value: function (name, _arg1) {
          return new Token(name, [_arg1[0], _arg1[1]], true, new _List2.default());
        }
      }, {
        key: ".ctor_1",
        value: function (name, _arg2, dependants) {
          return new Token(name, [_arg2[0], _arg2[1]], false, dependants);
        }
      }, {
        key: ".ctor_2",
        value: function (name, dependants) {
          return new Token(name, [0, 0], false, dependants);
        }
      }, {
        key: ".ctor_3",
        value: function (name) {
          return new Token(name, [0, 0], false, new _List2.default());
        }
      }]);

      return Token;
    }();

    (0, _Symbol2.setType)("Xlvm.Token.Token", Token);

    var _T___ = __exports["|T|_|"] = function (x) {
      if (x.Single) {
        return x.Name;
      } else {
        return null;
      }
    };

    var _A___ = __exports["|A|_|"] = function (x) {
      if (x.CanApply) {
        return {};
      } else {
        return null;
      }
    };

    var _X_ = __exports["|X|"] = function (t) {
      return [t.Name, t.Dependants];
    };

    var _Pref___ = __exports["|Pref|_|"] = function (_arg1) {
      var $var3 = void 0;

      var activePatternResult129 = _T___(_arg1);

      if (activePatternResult129 != null) {
        if ((0, _Seq.exists)(function (y) {
          return activePatternResult129 === y;
        }, Symbols.prefixes)) {
          $var3 = [0, activePatternResult129, _arg1];
        } else {
          $var3 = [1];
        }
      } else {
        $var3 = [1];
      }

      switch ($var3[0]) {
        case 0:
          return new Token("~" + $var3[1], $var3[2].Indentation, true, new _List2.default());

        case 1:
          return null;
      }
    };

    var _Pref____ = __exports["|Pref'|_|"] = function (_arg1) {
      var $var4 = void 0;

      var activePatternResult131 = _T___(_arg1);

      if (activePatternResult131 != null) {
        if ((activePatternResult131.length > 1 ? activePatternResult131[0] === "~" : false) ? (0, _Seq.exists)(function () {
          var x = activePatternResult131.slice(1, activePatternResult131.length);
          return function (y) {
            return x === y;
          };
        }(), Symbols.prefixes) : false) {
          $var4 = [0, activePatternResult131, _arg1];
        } else {
          $var4 = [1];
        }
      } else {
        $var4 = [1];
      }

      switch ($var4[0]) {
        case 0:
          return $var4[2];

        case 1:
          return null;
      }
    };

    var _Inner___ = __exports["|Inner|_|"] = function (c, _arg1) {
      var $var5 = void 0;

      var activePatternResult135 = _T___(_arg1);

      if (activePatternResult135 != null) {
        if (activePatternResult135 === "\"\"") {
          $var5 = [0];
        } else {
          $var5 = [1];
        }
      } else {
        $var5 = [1];
      }

      switch ($var5[0]) {
        case 0:
          return "";

        case 1:
          var $var6 = void 0;

          var activePatternResult134 = _T___(_arg1);

          if (activePatternResult134 != null) {
            if ((activePatternResult134.length >= 2 ? activePatternResult134[0] === c : false) ? activePatternResult134[activePatternResult134.length - 1] === c : false) {
              $var6 = [0, activePatternResult134];
            } else {
              $var6 = [1];
            }
          } else {
            $var6 = [1];
          }

          switch ($var6[0]) {
            case 0:
              return $var6[1].slice(1, $var6[1].length - 2 + 1);

            case 1:
              return null;
          }

      }
    };

    var _Var_Cnst_Other_ = __exports["|Var|Cnst|Other|"] = function (_arg1) {
      var activePatternResult144 = function () {
        var c = "\"";
        return function (_arg1_1) {
          return _Inner___(c, _arg1_1);
        };
      }()(_arg1);

      if (activePatternResult144 != null) {
        return new _Choice2.default("Choice2Of3", [activePatternResult144]);
      } else {
        var activePatternResult142 = function () {
          var c_1 = "'";
          return function (_arg1_2) {
            return _Inner___(c_1, _arg1_2);
          };
        }()(_arg1);

        if (activePatternResult142 != null) {
          return new _Choice2.default("Choice2Of3", [activePatternResult142]);
        } else {
          var $var7 = void 0;

          var activePatternResult140 = _T___(_arg1);

          if (activePatternResult140 != null) {
            if (activePatternResult140 === "()") {
              $var7 = [0];
            } else {
              $var7 = [1];
            }
          } else {
            $var7 = [1];
          }

          switch ($var7[0]) {
            case 0:
              return new _Choice2.default("Choice2Of3", ["()"]);

            case 1:
              var $var8 = void 0;

              var activePatternResult139 = _T___(_arg1);

              if (activePatternResult139 != null) {
                if (activePatternResult139 === "true") {
                  $var8 = [0, activePatternResult139];
                } else if (activePatternResult139 === "false") {
                  $var8 = [0, activePatternResult139];
                } else {
                  $var8 = [1];
                }
              } else {
                $var8 = [1];
              }

              switch ($var8[0]) {
                case 0:
                  return new _Choice2.default("Choice2Of3", [$var8[1]]);

                case 1:
                  var activePatternResult138 = _T___(_arg1);

                  if (activePatternResult138 != null) {
                    if ((activePatternResult138 !== "" ? "0" <= activePatternResult138[0] : false) ? activePatternResult138[0] <= "9" : false) {
                      return new _Choice2.default("Choice2Of3", [activePatternResult138]);
                    } else {
                      return new _Choice2.default("Choice1Of3", [activePatternResult138]);
                    }
                  } else {
                    return new _Choice2.default("Choice3Of3", [null]);
                  }

              }

          }
        }
      }
    };

    return __exports;
  }({});

  var String_Formatting = exports.String_Formatting = function (__exports) {
    var buildString = __exports.buildString = function (builder) {
      return Array.from((0, _List.reverse)(builder)).join('');
    };

    var escapeSequences = __exports.escapeSequences = function (s) {
      var charList = (0, _List.ofArray)(s.split(""));

      var parseCharList = function parseCharList(builder) {
        return function (_arg1) {
          parseCharList: while (true) {
            var $var9 = _arg1.tail != null ? _arg1.head === "\\" ? _arg1.tail.tail != null ? [1, _arg1.tail.head, _arg1.tail.tail] : [2, _arg1.head, _arg1.tail] : [2, _arg1.head, _arg1.tail] : [0];

            switch ($var9[0]) {
              case 0:
                return buildString(builder);

              case 1:
                var k = void 0;
                var $var10 = $var9[1] === "\"" ? [0] : $var9[1] === "\\" ? [0] : $var9[1] === "n" ? [1] : $var9[1] === "t" ? [2] : [3];

                switch ($var10[0]) {
                  case 0:
                    k = $var9[1];
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
                _arg1 = $var9[2];
                continue parseCharList;

              case 2:
                builder = new _List2.default($var9[1], builder);
                _arg1 = $var9[2];
                continue parseCharList;
            }
          }
        };
      };

      return parseCharList(new _List2.default())(charList);
    };

    var isFormatString = __exports.isFormatString = function (s) {
      if (s[0] === "%") {
        return s.length > 1;
      } else {
        return false;
      }
    };

    var _StringContents___ = __exports["|StringContents|_|"] = function (s) {
      if ((s.length >= 2 ? s[0] === "\"" : false) ? s[s.length - 1] === "\"" : false) {
        return s.slice(1, s.length - 2 + 1);
      } else {
        return null;
      }
    };

    var separateFormatSymbols = __exports.separateFormatSymbols = function (s) {
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
    };

    var buildFormatFunction = __exports.buildFormatFunction = function (makeFormat, nonFormat) {
      var buildFormatFunction_1 = function buildFormatFunction_1(arg) {
        return function (ret) {
          return function (_arg1) {
            buildFormatFunction_1: while (true) {
              var $var11 = _arg1.tail != null ? isFormatString(_arg1.head) ? [0, _arg1.head, _arg1.tail] : [1] : [1];

              switch ($var11[0]) {
                case 0:
                  var argName = Token.Token[".ctor_3"]((0, _String.fsFormat)("_%i")(function (x) {
                    return x;
                  })(arg));
                  return Token.Token[".ctor_2"]("fun", (0, _List.ofArray)([argName, buildFormatFunction_1(arg + 1)(new _List2.default(makeFormat((0, _String.fsFormat)("\"%s\"")(function (x) {
                    return x;
                  })($var11[1]))(argName), ret))($var11[2])]));

                case 1:
                  if (_arg1.tail == null) {
                    return Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(ret));
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
    };

    var mapFormatting = __exports.mapFormatting = function (_arg1) {
      mapFormatting: while (true) {
        var $var12 = void 0;
        var activePatternResult171 = Token["|X|"](_arg1);

        if (activePatternResult171[0] === "apply") {
          if (activePatternResult171[1].tail != null) {
            var activePatternResult172 = Token["|T|_|"](activePatternResult171[1].head);

            if (activePatternResult172 != null) {
              if (activePatternResult172 === "printfn") {
                if (activePatternResult171[1].tail.tail != null) {
                  var activePatternResult173 = Token["|T|_|"](activePatternResult171[1].tail.head);

                  if (activePatternResult173 != null) {
                    var activePatternResult174 = _StringContents___(activePatternResult173);

                    if (activePatternResult174 != null) {
                      if (activePatternResult171[1].tail.tail.tail == null) {
                        $var12 = [0, activePatternResult174];
                      } else {
                        $var12 = [1];
                      }
                    } else {
                      $var12 = [1];
                    }
                  } else {
                    $var12 = [1];
                  }
                } else {
                  $var12 = [1];
                }
              } else {
                $var12 = [1];
              }
            } else {
              $var12 = [1];
            }
          } else {
            $var12 = [1];
          }
        } else {
          $var12 = [1];
        }

        var _ret = function () {
          switch ($var12[0]) {
            case 0:
              _arg1 = Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([Token.Token[".ctor_3"]("printf"), Token.Token[".ctor_3"]((0, _String.fsFormat)("\"%s\\n\"")(function (x) {
                return x;
              })($var12[1]))]));
              return "continue|mapFormatting";

            case 1:
              var $var13 = void 0;
              var activePatternResult165 = Token["|X|"](_arg1);

              if (activePatternResult165[0] === "apply") {
                if (activePatternResult165[1].tail != null) {
                  var activePatternResult166 = Token["|T|_|"](activePatternResult165[1].head);

                  if (activePatternResult166 != null) {
                    if (activePatternResult166 === "printf") {
                      if (activePatternResult165[1].tail.tail != null) {
                        var activePatternResult167 = Token["|T|_|"](activePatternResult165[1].tail.head);

                        if (activePatternResult167 != null) {
                          var activePatternResult168 = _StringContents___(activePatternResult167);

                          if (activePatternResult168 != null) {
                            if (activePatternResult165[1].tail.tail.tail == null) {
                              $var13 = [0, activePatternResult168, activePatternResult165[1].head];
                            } else {
                              $var13 = [1];
                            }
                          } else {
                            $var13 = [1];
                          }
                        } else {
                          $var13 = [1];
                        }
                      } else {
                        $var13 = [1];
                      }
                    } else if (activePatternResult166 === "scanf") {
                      if (activePatternResult165[1].tail.tail != null) {
                        var activePatternResult169 = Token["|T|_|"](activePatternResult165[1].tail.head);

                        if (activePatternResult169 != null) {
                          var activePatternResult170 = _StringContents___(activePatternResult169);

                          if (activePatternResult170 != null) {
                            if (activePatternResult165[1].tail.tail.tail == null) {
                              $var13 = [0, activePatternResult170, activePatternResult165[1].head];
                            } else {
                              $var13 = [1];
                            }
                          } else {
                            $var13 = [1];
                          }
                        } else {
                          $var13 = [1];
                        }
                      } else {
                        $var13 = [1];
                      }
                    } else {
                      $var13 = [1];
                    }
                  } else {
                    $var13 = [1];
                  }
                } else {
                  $var13 = [1];
                }
              } else {
                $var13 = [1];
              }

              switch ($var13[0]) {
                case 0:
                  return {
                    v: buildFormatFunction(function (fmt) {
                      return function (v) {
                        return Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var13[2], Token.Token[".ctor_3"](fmt)])), v]));
                      };
                    }, function (s) {
                      return Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var13[2], Token.Token[".ctor_3"](s)]));
                    })(separateFormatSymbols($var13[1]))
                  };

                case 1:
                  var activePatternResult164 = Token["|X|"](_arg1);
                  return {
                    v: Token.Token[".ctor_2"](activePatternResult164[0], (0, _List.map)(function (_arg1_1) {
                      return mapFormatting(_arg1_1);
                    }, activePatternResult164[1]))
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
    };

    var processScan = __exports.processScan = function (_arg1) {
      var $var14 = void 0;
      var activePatternResult183 = Token["|X|"](_arg1);

      if (activePatternResult183[0] === "apply") {
        if (activePatternResult183[1].tail != null) {
          var activePatternResult184 = Token["|X|"](activePatternResult183[1].head);

          if (activePatternResult184[0] === "apply") {
            if (activePatternResult184[1].tail != null) {
              var activePatternResult185 = Token["|T|_|"](activePatternResult184[1].head);

              if (activePatternResult185 != null) {
                if (activePatternResult185 === "scanf") {
                  if (activePatternResult184[1].tail.tail != null) {
                    var activePatternResult186 = Token["|T|_|"](activePatternResult184[1].tail.head);

                    if (activePatternResult186 != null) {
                      if (activePatternResult186 === "\"%i\"") {
                        if (activePatternResult184[1].tail.tail.tail == null) {
                          if (activePatternResult183[1].tail.tail != null) {
                            if (activePatternResult183[1].tail.tail.tail == null) {
                              $var14 = [0, activePatternResult183[1].tail.head];
                            } else {
                              $var14 = [1];
                            }
                          } else {
                            $var14 = [1];
                          }
                        } else {
                          $var14 = [1];
                        }
                      } else {
                        $var14 = [1];
                      }
                    } else {
                      $var14 = [1];
                    }
                  } else {
                    $var14 = [1];
                  }
                } else {
                  $var14 = [1];
                }
              } else {
                $var14 = [1];
              }
            } else {
              $var14 = [1];
            }
          } else {
            $var14 = [1];
          }
        } else {
          $var14 = [1];
        }
      } else {
        $var14 = [1];
      }

      switch ($var14[0]) {
        case 0:
          return Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var14[1], Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([Token.Token[".ctor_3"]("0")]))])), Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([Token.Token[".ctor_3"]("scan"), Token.Token[".ctor_3"]("%i")]))]));

        case 1:
          var $var15 = void 0;
          var activePatternResult179 = Token["|X|"](_arg1);

          if (activePatternResult179[0] === "apply") {
            if (activePatternResult179[1].tail != null) {
              var activePatternResult180 = Token["|X|"](activePatternResult179[1].head);

              if (activePatternResult180[0] === "apply") {
                if (activePatternResult180[1].tail != null) {
                  var activePatternResult181 = Token["|T|_|"](activePatternResult180[1].head);

                  if (activePatternResult181 != null) {
                    if (activePatternResult181 === "scanf") {
                      if (activePatternResult180[1].tail.tail != null) {
                        var activePatternResult182 = Token["|T|_|"](activePatternResult180[1].tail.head);

                        if (activePatternResult182 != null) {
                          if (activePatternResult182 === "\"%s\"") {
                            if (activePatternResult180[1].tail.tail.tail == null) {
                              if (activePatternResult179[1].tail.tail != null) {
                                if (activePatternResult179[1].tail.tail.tail == null) {
                                  $var15 = [0, activePatternResult179[1].tail.head];
                                } else {
                                  $var15 = [1];
                                }
                              } else {
                                $var15 = [1];
                              }
                            } else {
                              $var15 = [1];
                            }
                          } else {
                            $var15 = [1];
                          }
                        } else {
                          $var15 = [1];
                        }
                      } else {
                        $var15 = [1];
                      }
                    } else {
                      $var15 = [1];
                    }
                  } else {
                    $var15 = [1];
                  }
                } else {
                  $var15 = [1];
                }
              } else {
                $var15 = [1];
              }
            } else {
              $var15 = [1];
            }
          } else {
            $var15 = [1];
          }

          switch ($var15[0]) {
            case 0:
              return Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var15[1], Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([Token.Token[".ctor_3"]("0")]))])), Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([Token.Token[".ctor_3"]("scan"), Token.Token[".ctor_3"]("%s")]))]));

            case 1:
              var activePatternResult178 = Token["|X|"](_arg1);
              return Token.Token[".ctor_2"](activePatternResult178[0], (0, _List.map)(function (_arg1_1) {
                return processScan(_arg1_1);
              }, activePatternResult178[1]));
          }

      }
    };

    var processEscapeSequences = __exports.processEscapeSequences = function (_arg1) {
      var activePatternResult190 = Token["|X|"](_arg1);

      var activePatternResult191 = _StringContents___(activePatternResult190[0]);

      if (activePatternResult191 != null) {
        var s_ = escapeSequences(activePatternResult191);
        return Token.Token[".ctor_2"]((0, _String.fsFormat)("\"%s\"")(function (x) {
          return x;
        })(s_), (0, _List.map)(function (_arg1_1) {
          return processEscapeSequences(_arg1_1);
        }, activePatternResult190[1]));
      } else {
        var activePatternResult189 = Token["|X|"](_arg1);
        return Token.Token[".ctor_2"](activePatternResult189[0], (0, _List.map)(function (_arg1_2) {
          return processEscapeSequences(_arg1_2);
        }, activePatternResult189[1]));
      }
    };

    var processStringFormatting = __exports.processStringFormatting = function ($var17) {
      return function (_arg1) {
        return processEscapeSequences(_arg1);
      }(function ($var16) {
        return processScan(mapFormatting($var16));
      }($var17));
    };

    return __exports;
  }({});

  var Lexer = exports.Lexer = function (__exports) {
    var CommonClassifiers = __exports.CommonClassifiers = function (__exports) {
      var getOccurances = __exports.getOccurances = function (charClassifier, str) {
        return Array.from((0, _Seq.collect)(function (e) {
          return charClassifier(e) ? e : "";
        }, str)).join('').length;
      };

      var op_GreaterGreaterBarBar = __exports.op_GreaterGreaterBarBar = function (classifierA, classifierB, e) {
        if (classifierA(e)) {
          return true;
        } else {
          return classifierB(e);
        }
      };

      var op_GreaterGreaterAmpAmp = __exports.op_GreaterGreaterAmpAmp = function (classifierA, classifierB, e) {
        if (classifierA(e)) {
          return classifierB(e);
        } else {
          return false;
        }
      };

      var classifierApplies = __exports.classifierApplies = function (classifier, str) {
        return (0, _Seq.forAll)(classifier, str);
      };

      var classifierAppliesFstChar = __exports.classifierAppliesFstChar = function (classifier, str) {
        return classifier(str[0]);
      };

      var classifierAppliesFstSuffix = __exports.classifierAppliesFstSuffix = function (classifier, str) {
        return (0, _Seq.forAll)(classifier, str.slice(1, str.length));
      };

      var classifiersApplyCons = __exports.classifiersApplyCons = function (hdCharClassifier, tlStrClassifier, str) {
        if (classifierAppliesFstChar(hdCharClassifier, str)) {
          return tlStrClassifier(str.slice(1, str.length));
        } else {
          return false;
        }
      };

      var Chr = __exports.Chr = function (__exports) {
        var isNumeric = __exports.isNumeric = function (e) {
          if ("0" <= e) {
            return e <= "9";
          } else {
            return false;
          }
        };

        var isAlphabetic = __exports.isAlphabetic = function (e) {
          if ("a" <= e ? e <= "z" : false) {
            return true;
          } else if ("A" <= e) {
            return e <= "Z";
          } else {
            return false;
          }
        };

        var isUndersc = __exports.isUndersc = function () {
          var x = "_";
          return function (y) {
            return x === y;
          };
        }();

        var isDot = __exports.isDot = function () {
          var x = ".";
          return function (y) {
            return x === y;
          };
        }();

        var isHyphen = __exports.isHyphen = function () {
          var x = "-";
          return function (y) {
            return x === y;
          };
        }();

        var isValidForVariables = __exports.isValidForVariables = function () {
          var classifierA = function classifierA(e) {
            return op_GreaterGreaterBarBar(function (e_1) {
              return isNumeric(e_1);
            }, function (e_2) {
              return isAlphabetic(e_2);
            }, e);
          };

          return function (e_3) {
            return op_GreaterGreaterBarBar(classifierA, isUndersc, e_3);
          };
        }();

        var isWhitespace = __exports.isWhitespace = function () {
          var classifierA_2 = void 0;
          var classifierA_1 = void 0;
          var classifierA = void 0;
          var x = " ";

          classifierA = function classifierA(y) {
            return x === y;
          };

          var classifierB = void 0;
          var x_1 = "\n";

          classifierB = function classifierB(y_1) {
            return x_1 === y_1;
          };

          classifierA_1 = function classifierA_1(e) {
            return op_GreaterGreaterBarBar(classifierA, classifierB, e);
          };

          var classifierB_1 = void 0;
          var x_2 = "\t";

          classifierB_1 = function classifierB_1(y_2) {
            return x_2 === y_2;
          };

          classifierA_2 = function classifierA_2(e_1) {
            return op_GreaterGreaterBarBar(classifierA_1, classifierB_1, e_1);
          };

          var classifierB_2 = void 0;
          var x_3 = "\r";

          classifierB_2 = function classifierB_2(y_3) {
            return x_3 === y_3;
          };

          return function (e_2) {
            return op_GreaterGreaterBarBar(classifierA_2, classifierB_2, e_2);
          };
        }();

        var isSingleQuote = __exports.isSingleQuote = function () {
          var x = "'";
          return function (y) {
            return x === y;
          };
        }();

        var isDoubleQuote = __exports.isDoubleQuote = function () {
          var x = "\"";
          return function (y) {
            return x === y;
          };
        }();

        return __exports;
      }({});

      var isVariableSuffix = __exports.isVariableSuffix = function (str) {
        return classifierApplies(Chr.isValidForVariables, str);
      };

      var isInteger = __exports.isInteger = function (str) {
        return classifiersApplyCons(function (e) {
          return Chr.isNumeric(e);
        }, isVariableSuffix, str);
      };

      var isVariable = __exports.isVariable = function () {
        var hdCharClassifier = void 0;

        var classifierB = function classifierB($var18) {
          return function (value) {
            return !value;
          }(function (e) {
            return Chr.isNumeric(e);
          }($var18));
        };

        hdCharClassifier = function hdCharClassifier(e_1) {
          return op_GreaterGreaterAmpAmp(Chr.isValidForVariables, classifierB, e_1);
        };

        return function (str) {
          return classifiersApplyCons(hdCharClassifier, isVariableSuffix, str);
        };
      }();

      var isNumericSuffix = __exports.isNumericSuffix = function () {
        var classifier = function classifier(e) {
          return op_GreaterGreaterBarBar(Chr.isDot, Chr.isValidForVariables, e);
        };

        return function (str) {
          return classifierApplies(classifier, str);
        };
      }();

      var isNumeric = __exports.isNumeric = function (str) {
        return classifiersApplyCons(function (e) {
          return Chr.isNumeric(e);
        }, isNumericSuffix, str);
      };

      var isFloat = __exports.isFloat = function () {
        var classifierB = function classifierB($var19) {
          return function (value) {
            return !value;
          }(isInteger($var19));
        };

        return function (e) {
          return op_GreaterGreaterAmpAmp(isNumeric, classifierB, e);
        };
      }();

      var isWhitespace = __exports.isWhitespace = function (str) {
        return classifierApplies(Chr.isWhitespace, str);
      };

      var isAnything = __exports.isAnything = function () {
        var classifier = function classifier(_arg1) {
          return true;
        };

        return function (str) {
          return classifierApplies(classifier, str);
        };
      }();

      var failedToClassify = __exports.failedToClassify = function (message, _arg1) {
        throw new Error(message);
      };

      return __exports;
    }({});

    var tokenizeRule = __exports.tokenizeRule = function () {
      function tokenizeRule(caseName, fields) {
        _classCallCheck(this, tokenizeRule);

        this.Case = caseName;
        this.Fields = fields;
      }

      _createClass(tokenizeRule, [{
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            type: "Xlvm.Lexer.tokenizeRule",
            interfaces: ["FSharpUnion"],
            cases: {
              StickRule: ["function"],
              SymbolRule: ["string"]
            }
          };
        }
      }]);

      return tokenizeRule;
    }();

    (0, _Symbol2.setType)("Xlvm.Lexer.tokenizeRule", tokenizeRule);

    var makeRule = __exports.makeRule = function (c1, c2) {
      return new tokenizeRule("StickRule", [function (s1) {
        return function (s2) {
          return c1(s1) ? c2(s2) : false;
        };
      }]);
    };

    var isPrefix = __exports.isPrefix = function (a, b) {
      if (a.length <= b.length) {
        return a === b.slice(null, a.length - 1 + 1);
      } else {
        return false;
      }
    };

    var isSuffix = __exports.isSuffix = function (a, b) {
      if (a.length <= b.length) {
        return a === b.slice(b.length - a.length, b.length);
      } else {
        return false;
      }
    };

    var isDelimitedString = __exports.isDelimitedString = function (delim1, delim2, str) {
      if (delim1.length + delim2.length <= str.length ? isPrefix(delim1, str) : false) {
        return isSuffix(delim2, str);
      } else {
        return false;
      }
    };

    var makeSymbolRule = __exports.makeSymbolRule = function (symbol) {
      return new tokenizeRule("SymbolRule", [symbol]);
    };

    var makeDelimiterRule = __exports.makeDelimiterRule = function (symbol1, symbol2) {
      return new tokenizeRule("StickRule", [function (s1) {
        return function (s2) {
          return isPrefix(symbol1, s1) ? !isDelimitedString(symbol1, symbol2, s1) : false;
        };
      }]);
    };

    var tokenize = __exports.tokenize = function (ruleset, txt) {
      var ruleset_1 = (0, _List.append)(ruleset, (0, _List.ofArray)([[makeRule(function () {
        var message = "could not classify";
        return function (arg10_) {
          return CommonClassifiers.failedToClassify(message, arg10_);
        };
      }(), CommonClassifiers.isAnything), false]]));
      var patternInput = (0, _Seq.fold)(function (tupledArg, _arg1) {
        if (_arg1[0].Case === "StickRule") {
          var r = _arg1[0].Fields[0];
          return [tupledArg[0], new _List2.default([r, _arg1[1]], tupledArg[1])];
        } else {
          var s = _arg1[0].Fields[0];

          var tryGetPrefix = function tryGetPrefix(_arg2) {
            if (_arg2[0].tail != null) {
              if (_arg2[1].tail != null) {
                if (function () {
                  var b = _arg2[1].head;
                  var a = _arg2[0].head;
                  return !(0, _Util.equals)(a, b);
                }()) {
                  return null;
                } else {
                  var $var20 = _arg2[0].tail != null ? _arg2[1].tail != null ? [0, _arg2[0].head, _arg2[1].head, _arg2[0].tail, _arg2[1].tail] : [1] : [1];

                  switch ($var20[0]) {
                    case 0:
                      var matchValue = tryGetPrefix([$var20[3], $var20[4]]);

                      if (matchValue != null) {
                        var s_1 = matchValue[1];
                        var p = matchValue[0];
                        return [new _List2.default($var20[2], p), s_1];
                      } else {
                        return null;
                      }

                    case 1:
                      throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 240, 33);
                  }
                }
              } else {
                return null;
              }
            } else {
              return [new _List2.default(), _arg2[1]];
            }
          };

          var ss = (0, _List.map)(function (value) {
            return value;
          }, (0, _Seq.toList)(s));

          var groupSymbols = function groupSymbols(ll) {
            var matchValue_1 = [tryGetPrefix([ss, ll]), ll];

            if (matchValue_1[0] == null) {
              if (matchValue_1[1].tail != null) {
                var tl = matchValue_1[1].tail;
                var hd = matchValue_1[1].head;
                return new _List2.default(hd, groupSymbols(tl));
              } else {
                return new _List2.default();
              }
            } else {
              var s_2 = matchValue_1[0][1];
              var p_1 = matchValue_1[0][0];
              return new _List2.default((0, _String.join)("", p_1), groupSymbols(s_2));
            }
          };

          return [groupSymbols(tupledArg[0]), tupledArg[1]];
        }
      }, [txt, new _List2.default()], ruleset_1);
      var stickRules = (0, _List.reverse)(patternInput[1]);
      return (0, _List.reverse)((0, _Seq.fold)(function (acc, next) {
        if (acc.tail != null) {
          var patternInput_1 = (0, _Seq.find)(function (tupledArg_1) {
            return tupledArg_1[0](acc.head)(next);
          }, stickRules);

          if (patternInput_1[1]) {
            return new _List2.default(acc.head + next, acc.tail);
          } else {
            return new _List2.default(next, acc);
          }
        } else {
          return (0, _List.ofArray)([next]);
        }
      }, new _List2.default(), patternInput[0]));
    };

    var SpecialCases = __exports.SpecialCases = function (__exports) {
      var detectEscapeSequenceDoubleQuote = __exports.detectEscapeSequenceDoubleQuote = new tokenizeRule("StickRule", [function (s1) {
        return function (s2) {
          return isPrefix("\"", s1) ? isSuffix("\\\"", s1) : false;
        };
      }]);
      return __exports;
    }({});

    var singleLineCommentRules = __exports.singleLineCommentRules = function (delimiter) {
      return (0, _List.ofArray)([[makeSymbolRule(delimiter), true], [makeDelimiterRule(delimiter, "\n"), true], [makeRule(function () {
        var delim2 = "\n";
        return function (str) {
          return isDelimitedString(delimiter, delim2, str);
        };
      }(), CommonClassifiers.isAnything), false]]);
    };

    var delimitedCommentRules = __exports.delimitedCommentRules = function (delim1, delim2) {
      return (0, _List.ofArray)([[makeSymbolRule(delim1), true], [makeSymbolRule(delim2), true], [makeDelimiterRule(delim1, delim2), true], [makeRule(function (str) {
        return isDelimitedString(delim1, delim2, str);
      }, CommonClassifiers.isAnything), false]]);
    };

    var createSymbol = __exports.createSymbol = function (symbol) {
      return (0, _List.ofArray)([[makeSymbolRule(symbol), true], [makeRule(function (y) {
        return symbol === y;
      }, CommonClassifiers.isAnything), false], [makeRule(function (e) {
        return !isPrefix(e, symbol);
      }, function (e_1) {
        return isPrefix(e_1, symbol);
      }), false]]);
    };

    var commonRules = __exports.commonRules = (0, _List.ofArray)([[SpecialCases.detectEscapeSequenceDoubleQuote, true], [makeDelimiterRule("\"", "\""), true], [makeRule(function () {
      var delim1 = "\"";
      var delim2 = "\"";
      return function (str) {
        return isDelimitedString(delim1, delim2, str);
      };
    }(), CommonClassifiers.isAnything), false], [makeRule(CommonClassifiers.isAnything, CommonClassifiers.isWhitespace), false], [makeRule(CommonClassifiers.isNumeric, CommonClassifiers.isNumericSuffix), true], [makeRule(CommonClassifiers.isVariable, CommonClassifiers.isVariableSuffix), true], [makeRule(CommonClassifiers.isAnything, CommonClassifiers.isAnything), false]]);
    return __exports;
  }({});

  var C = exports.C = function (__exports) {
    var listOfDatatypeNamesDefault = __exports.listOfDatatypeNamesDefault = (0, _List.ofArray)(["int", "long long", "long", "bool", "char", "unsigned", "unsigned int", "unsigned long int", "unsigned long long int", "long int", "long long int"]);
    var listOfDatatypeNames = __exports.listOfDatatypeNames = {
      contents: listOfDatatypeNamesDefault
    };

    var restoreDefault = __exports.restoreDefault = function () {
      listOfDatatypeNames.contents = listOfDatatypeNamesDefault;
    };

    var _BrokenDatatypeName___ = __exports["|BrokenDatatypeName|_|"] = function (ll) {
      var matchString = function matchString(s) {
        var matching = (0, _Seq.toList)((0, _String.split)(s, " "));

        var findMatch = function findMatch(_arg1) {
          findMatch: while (true) {
            var $var21 = _arg1[0].tail != null ? _arg1[1].tail != null ? function () {
              var restb = _arg1[1].tail;
              var resta = _arg1[0].tail;
              var b = _arg1[1].head;
              var a = _arg1[0].head;
              return (0, _Util.equals)(a, b);
            }() ? [1, _arg1[0].head, _arg1[1].head, _arg1[0].tail, _arg1[1].tail] : [2] : [2] : [0, _arg1[1]];

            switch ($var21[0]) {
              case 0:
                return [s, $var21[1]];

              case 1:
                _arg1 = [$var21[3], $var21[4]];
                continue findMatch;

              case 2:
                return null;
            }
          }
        };

        return findMatch([matching, ll]);
      };

      return function (_arg3) {
        return _arg3 == null ? null : _arg3;
      }((0, _Seq.tryFind)(function (_arg2) {
        return _arg2 == null ? false : true;
      }, (0, _List.map)(matchString, listOfDatatypeNames.contents)));
    };

    var tokenizeDatatypes = __exports.tokenizeDatatypes = function (_arg1) {
      var $var22 = void 0;

      if (_arg1.tail != null) {
        var activePatternResult263 = _BrokenDatatypeName___(_arg1);

        if (activePatternResult263 != null) {
          $var22 = [1, activePatternResult263[0], activePatternResult263[1]];
        } else {
          $var22 = [1, _arg1.head, _arg1.tail];
        }
      } else {
        $var22 = [0];
      }

      switch ($var22[0]) {
        case 0:
          return new _List2.default();

        case 1:
          return new _List2.default($var22[1], tokenizeDatatypes($var22[2]));
      }
    };

    var preprocess = __exports.preprocess = function () {
      var isComment = function isComment(s) {
        if ((Lexer.isPrefix("//", s) ? Lexer.isSuffix("\n", s) : false) ? true : (s.length >= 4 ? Lexer.isPrefix("(*", s) : false) ? Lexer.isSuffix("*)", s) : false) {
          return true;
        } else if (Lexer.isPrefix("#", s)) {
          return Lexer.isSuffix("\n", s);
        } else {
          return false;
        }
      };

      var mainRules = (0, _List.append)(Lexer.singleLineCommentRules("#"), (0, _List.append)(Lexer.singleLineCommentRules("//"), (0, _List.append)(Lexer.delimitedCommentRules("/*", "*/"), (0, _List.append)(Lexer.createSymbol("=="), (0, _List.append)(Lexer.createSymbol("!="), (0, _List.append)(Lexer.createSymbol("<="), (0, _List.append)(Lexer.createSymbol(">="), (0, _List.append)(Lexer.createSymbol("++"), (0, _List.append)(Lexer.createSymbol("--"), Lexer.commonRules)))))))));
      return function ($var35) {
        return function (list_4) {
          return (0, _List.reverse)(list_4);
        }(function ($var34) {
          return function () {
            var folder = function folder(acc) {
              return function (e_3) {
                var matchValue_1 = [acc, e_3];
                var $var30 = void 0;

                if (matchValue_1[0].tail != null) {
                  var activePatternResult270 = Token["|T|_|"](matchValue_1[0].head);

                  if (activePatternResult270 != null) {
                    if (activePatternResult270 === "{") {
                      if (matchValue_1[0].tail.tail != null) {
                        if (matchValue_1[0].tail.tail.tail != null) {
                          var activePatternResult271 = Token["|T|_|"](matchValue_1[0].tail.tail.head);

                          if (activePatternResult271 != null) {
                            if (activePatternResult271 === "struct") {
                              var activePatternResult272 = Token["|T|_|"](matchValue_1[1]);

                              if (activePatternResult272 != null) {
                                if (activePatternResult272 === "}") {
                                  $var30 = [0];
                                } else {
                                  $var30 = [1];
                                }
                              } else {
                                $var30 = [1];
                              }
                            } else {
                              var activePatternResult273 = Token["|T|_|"](matchValue_1[0].tail.head);

                              if (activePatternResult273 != null) {
                                if (activePatternResult273 === "struct") {
                                  var activePatternResult274 = Token["|T|_|"](matchValue_1[1]);

                                  if (activePatternResult274 != null) {
                                    if (activePatternResult274 === "}") {
                                      $var30 = [0];
                                    } else {
                                      $var30 = [1];
                                    }
                                  } else {
                                    $var30 = [1];
                                  }
                                } else {
                                  $var30 = [1];
                                }
                              } else {
                                $var30 = [1];
                              }
                            }
                          } else {
                            var activePatternResult275 = Token["|T|_|"](matchValue_1[0].tail.head);

                            if (activePatternResult275 != null) {
                              if (activePatternResult275 === "struct") {
                                var activePatternResult276 = Token["|T|_|"](matchValue_1[1]);

                                if (activePatternResult276 != null) {
                                  if (activePatternResult276 === "}") {
                                    $var30 = [0];
                                  } else {
                                    $var30 = [1];
                                  }
                                } else {
                                  $var30 = [1];
                                }
                              } else {
                                $var30 = [1];
                              }
                            } else {
                              $var30 = [1];
                            }
                          }
                        } else {
                          var activePatternResult277 = Token["|T|_|"](matchValue_1[0].tail.head);

                          if (activePatternResult277 != null) {
                            if (activePatternResult277 === "struct") {
                              var activePatternResult278 = Token["|T|_|"](matchValue_1[1]);

                              if (activePatternResult278 != null) {
                                if (activePatternResult278 === "}") {
                                  $var30 = [0];
                                } else {
                                  $var30 = [1];
                                }
                              } else {
                                $var30 = [1];
                              }
                            } else {
                              $var30 = [1];
                            }
                          } else {
                            $var30 = [1];
                          }
                        }
                      } else {
                        $var30 = [1];
                      }
                    } else {
                      $var30 = [1];
                    }
                  } else {
                    $var30 = [1];
                  }
                } else {
                  $var30 = [1];
                }

                switch ($var30[0]) {
                  case 0:
                    return new _List2.default(e_3, acc);

                  case 1:
                    var $var31 = void 0;

                    if (matchValue_1[0].tail != null) {
                      var activePatternResult268 = Token["|T|_|"](matchValue_1[0].head);

                      if (activePatternResult268 != null) {
                        if (activePatternResult268 === "{") {
                          var activePatternResult269 = Token["|T|_|"](matchValue_1[1]);

                          if (activePatternResult269 != null) {
                            if (activePatternResult269 === "}") {
                              $var31 = [0, matchValue_1[0].tail];
                            } else {
                              $var31 = [1];
                            }
                          } else {
                            $var31 = [1];
                          }
                        } else {
                          $var31 = [1];
                        }
                      } else {
                        $var31 = [1];
                      }
                    } else {
                      $var31 = [1];
                    }

                    switch ($var31[0]) {
                      case 0:
                        return new _List2.default(Token.Token[".ctor_3"](";"), $var31[1]);

                      case 1:
                        return new _List2.default(e_3, acc);
                    }

                }
              };
            };

            var state = new _List2.default();
            return function (list_3) {
              return (0, _Seq.fold)(function ($var32, $var33) {
                return folder($var32)($var33);
              }, state, list_3);
            };
          }()(function ($var29) {
            return function () {
              var mapping_1 = function mapping_1(e_2) {
                return Token.Token[".ctor_3"](e_2);
              };

              return function (list_2) {
                return (0, _List.map)(mapping_1, list_2);
              };
            }()(function ($var28) {
              return function (_arg1_1) {
                return tokenizeDatatypes(_arg1_1);
              }(function ($var27) {
                return function () {
                  var predicate = function predicate($var26) {
                    return function (value_1) {
                      return !value_1;
                    }(function () {
                      var classifierA = void 0;
                      var classifierB = void 0;
                      var delim1 = "//";
                      var delim2 = "\n";

                      classifierB = function classifierB(str) {
                        return Lexer.isDelimitedString(delim1, delim2, str);
                      };

                      classifierA = function classifierA(e) {
                        return Lexer.CommonClassifiers.op_GreaterGreaterBarBar(Lexer.CommonClassifiers.isWhitespace, classifierB, e);
                      };

                      var classifierB_1 = void 0;
                      var delim1_1 = "/*";
                      var delim2_1 = "*/";

                      classifierB_1 = function classifierB_1(str_1) {
                        return Lexer.isDelimitedString(delim1_1, delim2_1, str_1);
                      };

                      return function (e_1) {
                        return Lexer.CommonClassifiers.op_GreaterGreaterBarBar(classifierA, classifierB_1, e_1);
                      };
                    }()($var26));
                  };

                  return function (list_1) {
                    return (0, _List.filter)(predicate, list_1);
                  };
                }()(function ($var25) {
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
                  }()(function ($var24) {
                    return function (txt) {
                      return Lexer.tokenize(mainRules, txt);
                    }(function ($var23) {
                      return (0, _List.map)(function (value) {
                        return value;
                      }, (0, _Seq.toList)($var23));
                    }($var24));
                  }($var25));
                }($var27));
              }($var28));
            }($var29));
          }($var34));
        }($var35));
      };
    }();

    var _DatatypeName___ = __exports["|DatatypeName|_|"] = function (_arg1) {
      var $var36 = void 0;

      if (_arg1.tail != null) {
        var activePatternResult281 = Token["|T|_|"](_arg1.head);

        if (activePatternResult281 != null) {
          if ((0, _Seq.exists)(function (y) {
            return activePatternResult281 === y;
          }, listOfDatatypeNames.contents)) {
            $var36 = [0, _arg1.tail, activePatternResult281];
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
          return [$var36[2], $var36[1]];

        case 1:
          return null;
      }
    };

    var State = __exports.State = function () {
      function State(caseName, fields) {
        _classCallCheck(this, State);

        this.Case = caseName;
        this.Fields = fields;
      }

      _createClass(State, [{
        key: _Symbol3.default.reflection,
        value: function () {
          return {
            type: "Xlvm.C.State",
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

    (0, _Symbol2.setType)("Xlvm.C.State", State);

    var parse = __exports.parse = function ($var175, $var176, $var177, $var178, $var179) {
      parse: while (true) {
        var state = $var175;
        var stop = $var176;
        var fail = $var177;
        var left = $var178;
        var right = $var179;

        if (state.Case === "FunctionArgs") {
          var matchValue = [left, right];

          if (stop(right)) {
            return [Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
          } else if (fail(right)) {
            return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
              throw new Error(x);
            })([left, right]);
          } else {
            var $var37 = void 0;

            var activePatternResult321 = _DatatypeFunction___(state, stop, fail, matchValue[0], matchValue[1]);

            if (activePatternResult321 != null) {
              $var37 = [0, activePatternResult321];
            } else {
              var activePatternResult322 = _CommaFunction___(state, stop, fail, matchValue[0], matchValue[1]);

              if (activePatternResult322 != null) {
                $var37 = [0, activePatternResult322];
              } else {
                var activePatternResult323 = _Transfer___(state, stop, fail, matchValue[0], matchValue[1]);

                if (activePatternResult323 != null) {
                  $var37 = [0, activePatternResult323];
                } else {
                  $var37 = [1];
                }
              }
            }

            switch ($var37[0]) {
              case 0:
                return $var37[1];

              case 1:
                return (0, _String.fsFormat)("unknown: %A")(function (x) {
                  throw new Error(x);
                })([left, right]);
            }
          }
        } else if (state.Case === "Local") {
          var matchValue_1 = [left, right];

          if (stop(right)) {
            return [Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
          } else if (fail(right)) {
            return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
              throw new Error(x);
            })([left, right]);
          } else {
            var $var38 = void 0;

            if (matchValue_1[0].tail != null) {
              var activePatternResult352 = Token["|T|_|"](matchValue_1[0].head);

              if (activePatternResult352 != null) {
                if (activePatternResult352 === ";") {
                  if (matchValue_1[1].tail != null) {
                    $var38 = [0, matchValue_1[1].head, matchValue_1[1].tail];
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
                $var175 = new State("Local", []);
                $var176 = stop;
                $var177 = fail;
                $var178 = new _List2.default($var38[1], left);
                $var179 = $var38[2];
                continue parse;

              case 1:
                var $var39 = void 0;

                var activePatternResult338 = _DatatypeLocal___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                if (activePatternResult338 != null) {
                  $var39 = [0, activePatternResult338];
                } else {
                  var activePatternResult339 = _Brackets___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                  if (activePatternResult339 != null) {
                    $var39 = [0, activePatternResult339];
                  } else {
                    var activePatternResult340 = _Braces___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                    if (activePatternResult340 != null) {
                      $var39 = [0, activePatternResult340];
                    } else {
                      var activePatternResult341 = _If___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                      if (activePatternResult341 != null) {
                        $var39 = [0, activePatternResult341];
                      } else {
                        var activePatternResult342 = _While___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                        if (activePatternResult342 != null) {
                          $var39 = [0, activePatternResult342];
                        } else {
                          var activePatternResult343 = _For___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                          if (activePatternResult343 != null) {
                            $var39 = [0, activePatternResult343];
                          } else {
                            var activePatternResult344 = _Return___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                            if (activePatternResult344 != null) {
                              $var39 = [0, activePatternResult344];
                            } else {
                              var activePatternResult345 = _Assignment___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                              if (activePatternResult345 != null) {
                                $var39 = [0, activePatternResult345];
                              } else {
                                var activePatternResult346 = _Dot___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                if (activePatternResult346 != null) {
                                  $var39 = [0, activePatternResult346];
                                } else {
                                  var activePatternResult347 = _Prefix___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                  if (activePatternResult347 != null) {
                                    $var39 = [0, activePatternResult347];
                                  } else {
                                    var activePatternResult348 = _Operator___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                    if (activePatternResult348 != null) {
                                      $var39 = [0, activePatternResult348];
                                    } else {
                                      var activePatternResult349 = _Apply___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                      if (activePatternResult349 != null) {
                                        $var39 = [0, activePatternResult349];
                                      } else {
                                        var activePatternResult350 = _Index___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                        if (activePatternResult350 != null) {
                                          $var39 = [0, activePatternResult350];
                                        } else {
                                          var activePatternResult351 = _Transfer___(state, stop, fail, matchValue_1[0], matchValue_1[1]);

                                          if (activePatternResult351 != null) {
                                            $var39 = [0, activePatternResult351];
                                          } else {
                                            $var39 = [1];
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

                switch ($var39[0]) {
                  case 0:
                    return $var39[1];

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
            return [Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
          } else if (fail(right)) {
            return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
              throw new Error(x);
            })([left, right]);
          } else {
            var $var40 = void 0;

            if (matchValue_2[0].tail != null) {
              var activePatternResult371 = Token["|T|_|"](matchValue_2[0].head);

              if (activePatternResult371 != null) {
                if (activePatternResult371 === ";") {
                  if (matchValue_2[1].tail != null) {
                    $var40 = [0, matchValue_2[1].head, matchValue_2[1].tail];
                  } else {
                    $var40 = [1];
                  }
                } else {
                  $var40 = [1];
                }
              } else {
                $var40 = [1];
              }
            } else {
              $var40 = [1];
            }

            switch ($var40[0]) {
              case 0:
                $var175 = new State("Local", []);
                $var176 = stop;
                $var177 = fail;
                $var178 = new _List2.default($var40[1], left);
                $var179 = $var40[2];
                continue parse;

              case 1:
                var $var41 = void 0;

                var activePatternResult362 = _Brackets___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                if (activePatternResult362 != null) {
                  $var41 = [0, activePatternResult362];
                } else {
                  var activePatternResult363 = _Assignment___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                  if (activePatternResult363 != null) {
                    $var41 = [0, activePatternResult363];
                  } else {
                    var activePatternResult364 = _Dot___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                    if (activePatternResult364 != null) {
                      $var41 = [0, activePatternResult364];
                    } else {
                      var activePatternResult365 = _Prefix___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                      if (activePatternResult365 != null) {
                        $var41 = [0, activePatternResult365];
                      } else {
                        var activePatternResult366 = _Operator___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                        if (activePatternResult366 != null) {
                          $var41 = [0, activePatternResult366];
                        } else {
                          var activePatternResult367 = _Apply___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                          if (activePatternResult367 != null) {
                            $var41 = [0, activePatternResult367];
                          } else {
                            var activePatternResult368 = _Index___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                            if (activePatternResult368 != null) {
                              $var41 = [0, activePatternResult368];
                            } else {
                              var activePatternResult369 = _CommaFunction___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                              if (activePatternResult369 != null) {
                                $var41 = [0, activePatternResult369];
                              } else {
                                var activePatternResult370 = _Transfer___(state, stop, fail, matchValue_2[0], matchValue_2[1]);

                                if (activePatternResult370 != null) {
                                  $var41 = [0, activePatternResult370];
                                } else {
                                  $var41 = [1];
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }

                switch ($var41[0]) {
                  case 0:
                    return $var41[1];

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
            return [Token.Token[".ctor_2"]("sequence", (0, _List.reverse)(left)), right];
          } else if (fail(right)) {
            return (0, _String.fsFormat)("tokens are incomplete: %A")(function (x) {
              throw new Error(x);
            })([left, right]);
          } else {
            var $var42 = void 0;

            var activePatternResult309 = _DatatypeGlobal___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

            if (activePatternResult309 != null) {
              $var42 = [0, activePatternResult309];
            } else {
              var activePatternResult310 = _Struct___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

              if (activePatternResult310 != null) {
                $var42 = [0, activePatternResult310];
              } else {
                var activePatternResult311 = _Apply___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                if (activePatternResult311 != null) {
                  $var42 = [0, activePatternResult311];
                } else {
                  var activePatternResult312 = _Index___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                  if (activePatternResult312 != null) {
                    $var42 = [0, activePatternResult312];
                  } else {
                    var activePatternResult313 = _Brackets___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                    if (activePatternResult313 != null) {
                      $var42 = [0, activePatternResult313];
                    } else {
                      var activePatternResult314 = _Braces___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                      if (activePatternResult314 != null) {
                        $var42 = [0, activePatternResult314];
                      } else {
                        var activePatternResult315 = _Assignment___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                        if (activePatternResult315 != null) {
                          $var42 = [0, activePatternResult315];
                        } else {
                          var activePatternResult316 = _Operator___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                          if (activePatternResult316 != null) {
                            $var42 = [0, activePatternResult316];
                          } else {
                            var activePatternResult317 = _Transfer___(state, stop, fail, matchValue_3[0], matchValue_3[1]);

                            if (activePatternResult317 != null) {
                              $var42 = [0, activePatternResult317];
                            } else {
                              $var42 = [1];
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }

            switch ($var42[0]) {
              case 0:
                return $var42[1];

              case 1:
                return (0, _String.fsFormat)("unknown: %A")(function (x) {
                  throw new Error(x);
                })([left, right]);
            }
          }
        }
      }
    };

    var _DatatypeGlobal___ = __exports["|DatatypeGlobal|_|"] = function (state, stop, fail, _arg1_0, _arg1_1) {
      var _arg1 = [_arg1_0, _arg1_1];

      var activePatternResult391 = _DatatypeName___(_arg1[1]);

      if (activePatternResult391 != null) {
        var patternInput = parse(new State("Global", []), function (_arg2) {
          var $var43 = void 0;

          if (_arg2.tail != null) {
            var activePatternResult377 = Token["|T|_|"](_arg2.head);

            if (activePatternResult377 != null) {
              if (activePatternResult377 === ";") {
                $var43 = [0];
              } else if (activePatternResult377 === "{") {
                $var43 = [0];
              } else if (activePatternResult377 === ",") {
                $var43 = [0];
              } else {
                $var43 = [1];
              }
            } else {
              $var43 = [1];
            }
          } else {
            $var43 = [1];
          }

          switch ($var43[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), activePatternResult391[1]);
        var patternInput_2 = void 0;
        var matchValue = patternInput[0].Clean();
        var activePatternResult388 = Token["|T|_|"](matchValue);

        if (activePatternResult388 != null) {
          patternInput_2 = [Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult391[0]), patternInput[0]])), Token.Token[".ctor_3"]("nothing")])), patternInput[1]];
        } else {
          var $var44 = void 0;
          var activePatternResult386 = Token["|X|"](matchValue);

          if (activePatternResult386[0] === "assign") {
            if (activePatternResult386[1].tail != null) {
              if (activePatternResult386[1].tail.tail != null) {
                if (activePatternResult386[1].tail.tail.tail == null) {
                  $var44 = [0, activePatternResult386[1].head, activePatternResult386[1].tail.head];
                } else {
                  $var44 = [3, matchValue];
                }
              } else {
                $var44 = [3, matchValue];
              }
            } else {
              $var44 = [3, matchValue];
            }
          } else if (activePatternResult386[0] === "dot") {
            if (activePatternResult386[1].tail != null) {
              if (activePatternResult386[1].tail.tail != null) {
                var activePatternResult387 = Token["|X|"](activePatternResult386[1].tail.head);

                if (activePatternResult387[0] === "[]") {
                  if (activePatternResult386[1].tail.tail.tail == null) {
                    $var44 = [1, activePatternResult387[1], activePatternResult386[1].head];
                  } else {
                    $var44 = [3, matchValue];
                  }
                } else {
                  $var44 = [3, matchValue];
                }
              } else {
                $var44 = [3, matchValue];
              }
            } else {
              $var44 = [3, matchValue];
            }
          } else if (activePatternResult386[0] === "apply") {
            if (activePatternResult386[1].tail != null) {
              if (activePatternResult386[1].tail.tail != null) {
                if (activePatternResult386[1].tail.tail.tail == null) {
                  $var44 = [2, activePatternResult386[1].tail.head, activePatternResult386[1].head];
                } else {
                  $var44 = [3, matchValue];
                }
              } else {
                $var44 = [3, matchValue];
              }
            } else {
              $var44 = [3, matchValue];
            }
          } else {
            $var44 = [3, matchValue];
          }

          switch ($var44[0]) {
            case 0:
              patternInput_2 = [Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult391[0]), $var44[1]])), $var44[2]])), patternInput[1]];
              break;

            case 1:
              patternInput_2 = [Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult391[0]), $var44[2]])), Token.Token[".ctor_2"]("array", $var44[1])])), patternInput[1]];
              break;

            case 2:
              var patternInput_1 = parse(new State("Local", []), function (_arg3) {
                var $var45 = void 0;

                if (_arg3.tail != null) {
                  var activePatternResult380 = Token["|X|"](_arg3.head);

                  if (activePatternResult380[0] === "{}") {
                    $var45 = [0];
                  } else {
                    $var45 = [1];
                  }
                } else {
                  $var45 = [1];
                }

                switch ($var45[0]) {
                  case 0:
                    return true;

                  case 1:
                    return false;
                }
              }, function (e_1) {
                return stop(e_1) ? true : fail(e_1);
              }, new _List2.default(), patternInput[1]);
              var $var46 = void 0;
              var activePatternResult383 = Token["|X|"](patternInput_1[0]);

              if (activePatternResult383[0] === "sequence") {
                if (activePatternResult383[1].tail == null) {
                  if (patternInput_1[1].tail != null) {
                    $var46 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                  } else {
                    $var46 = [1];
                  }
                } else {
                  $var46 = [1];
                }
              } else {
                $var46 = [1];
              }

              switch ($var46[0]) {
                case 0:
                  patternInput_2 = [Token.Token[".ctor_2"]("declare function", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult391[0]), $var44[2], $var44[1], $var46[1]])), $var46[2]];
                  break;

                case 1:
                  throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 453, 14);
                  break;
              }

              break;

            case 3:
              patternInput_2 = (0, _String.fsFormat)("expression following data type declaration is invalid %O")(function (x) {
                throw new Error(x);
              })($var44[1]);
              break;
          }
        }

        var restr = void 0;
        var $var47 = void 0;

        if (patternInput_2[1].tail != null) {
          var activePatternResult390 = Token["|T|_|"](patternInput_2[1].head);

          if (activePatternResult390 != null) {
            if (activePatternResult390 === ",") {
              $var47 = [0, patternInput_2[1].tail];
            } else {
              $var47 = [1];
            }
          } else {
            $var47 = [1];
          }
        } else {
          $var47 = [1];
        }

        switch ($var47[0]) {
          case 0:
            restr = (0, _List.ofArray)([Token.Token[".ctor_3"](";"), Token.Token[".ctor_3"](activePatternResult391[0])], $var47[1]);
            break;

          case 1:
            var $var48 = void 0;

            if (patternInput_2[1].tail != null) {
              var activePatternResult389 = Token["|T|_|"](patternInput_2[1].head);

              if (activePatternResult389 != null) {
                if (activePatternResult389 === ";") {
                  $var48 = [0, patternInput_2[1].tail];
                } else {
                  $var48 = [0, patternInput_2[1]];
                }
              } else {
                $var48 = [0, patternInput_2[1]];
              }
            } else {
              $var48 = [0, patternInput_2[1]];
            }

            switch ($var48[0]) {
              case 0:
                restr = $var48[1];
                break;
            }

            break;
        }

        return parse(new State("Global", []), stop, fail, _arg1[0], new _List2.default(patternInput_2[0], restr));
      } else {
        return null;
      }
    };

    var _Struct___ = __exports["|Struct|_|"] = function (state, stop, fail, _arg4_0, _arg4_1) {
      var _arg4 = [_arg4_0, _arg4_1];
      var $var49 = void 0;

      if (_arg4[0].tail != null) {
        var activePatternResult404 = Token["|T|_|"](_arg4[0].head);

        if (activePatternResult404 != null) {
          if (activePatternResult404 === "struct") {
            $var49 = [0, _arg4[0].tail, _arg4[1]];
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
          var patternInput = void 0;
          var $var50 = void 0;

          if ($var49[2].tail != null) {
            var activePatternResult398 = Token["|T|_|"]($var49[2].head);

            if (activePatternResult398 != null) {
              if (activePatternResult398 === "{") {
                $var50 = [0, $var49[2].tail];
              } else {
                $var50 = [1];
              }
            } else {
              $var50 = [1];
            }
          } else {
            $var50 = [1];
          }

          switch ($var50[0]) {
            case 0:
              patternInput = ["anonymous structure", $var50[1]];
              break;

            case 1:
              var $var51 = void 0;

              if ($var49[2].tail != null) {
                var activePatternResult396 = Token["|T|_|"]($var49[2].head);

                if (activePatternResult396 != null) {
                  if ($var49[2].tail.tail != null) {
                    var activePatternResult397 = Token["|T|_|"]($var49[2].tail.head);

                    if (activePatternResult397 != null) {
                      if (activePatternResult397 === "{") {
                        $var51 = [0, $var49[2].tail.tail, activePatternResult396];
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
              } else {
                $var51 = [1];
              }

              switch ($var51[0]) {
                case 0:
                  patternInput = [$var51[2], $var51[1]];
                  break;

                case 1:
                  patternInput = (0, _String.fsFormat)("not a valid struct declaration: %A")(function (x) {
                    throw new Error(x);
                  })($var49[2]);
                  break;
              }

              break;
          }

          listOfDatatypeNames.contents = new _List2.default(patternInput[0], listOfDatatypeNames.contents);
          var patternInput_1 = parse(new State("Local", []), function (_arg5) {
            var $var52 = void 0;

            if (_arg5.tail != null) {
              var activePatternResult399 = Token["|T|_|"](_arg5.head);

              if (activePatternResult399 != null) {
                if (activePatternResult399 === "}") {
                  $var52 = [0];
                } else {
                  $var52 = [1];
                }
              } else {
                $var52 = [1];
              }
            } else {
              $var52 = [1];
            }

            switch ($var52[0]) {
              case 0:
                return true;

              case 1:
                return false;
            }
          }, function (e) {
            return stop(e) ? true : fail(e);
          }, new _List2.default(), patternInput[1]);
          var $var53 = void 0;

          if (patternInput_1[1].tail != null) {
            var activePatternResult403 = Token["|T|_|"](patternInput_1[1].head);

            if (activePatternResult403 != null) {
              if (activePatternResult403 === "}") {
                $var53 = [0, patternInput_1[0], patternInput_1[1].tail];
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
              var restr = void 0;
              var $var54 = void 0;

              if ($var53[2].tail != null) {
                var activePatternResult402 = Token["|T|_|"]($var53[2].head);

                if (activePatternResult402 != null) {
                  if (activePatternResult402 === ";") {
                    $var54 = [0, $var53[2].tail];
                  } else {
                    $var54 = [1];
                  }
                } else {
                  $var54 = [1];
                }
              } else {
                $var54 = [1];
              }

              switch ($var54[0]) {
                case 0:
                  restr = $var54[1];
                  break;

                case 1:
                  restr = new _List2.default(Token.Token[".ctor_3"](patternInput[0]), $var53[2]);
                  break;
              }

              var parsed = Token.Token[".ctor_2"]("struct", (0, _List.ofArray)([Token.Token[".ctor_3"](patternInput[0]), $var53[1]]));
              return parse(state, stop, fail, $var49[1], new _List2.default(parsed, restr));

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 471, 10);
          }

        case 1:
          return null;
      }
    };

    var _DatatypeFunction___ = __exports["|DatatypeFunction|_|"] = function (state, stop, fail, _arg6_0, _arg6_1) {
      var _arg6 = [_arg6_0, _arg6_1];
      var $var55 = void 0;

      var activePatternResult410 = _DatatypeName___(_arg6[1]);

      if (activePatternResult410 != null) {
        if (activePatternResult410[1].tail != null) {
          var activePatternResult411 = Token["|Pref|_|"](activePatternResult410[1].head);

          if (activePatternResult411 != null) {
            var activePatternResult412 = Token["|T|_|"](activePatternResult411);

            if (activePatternResult412 != null) {
              if (activePatternResult410[1].tail.tail != null) {
                $var55 = [0, activePatternResult410[0], activePatternResult410[1].tail.head, _arg6[0], activePatternResult410[1].tail.tail, activePatternResult412];
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
      } else {
        $var55 = [1];
      }

      switch ($var55[0]) {
        case 0:
          var parsed = Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"]($var55[1] + $var55[5].slice(1, $var55[5].length)), $var55[2]]));
          return parse(new State("FunctionArgs", []), stop, fail, $var55[3], new _List2.default(parsed, $var55[4]));

        case 1:
          var $var56 = void 0;

          var activePatternResult409 = _DatatypeName___(_arg6[1]);

          if (activePatternResult409 != null) {
            if (activePatternResult409[1].tail != null) {
              $var56 = [0, activePatternResult409[0], activePatternResult409[1].head, _arg6[0], activePatternResult409[1].tail];
            } else {
              $var56 = [1];
            }
          } else {
            $var56 = [1];
          }

          switch ($var56[0]) {
            case 0:
              var parsed_1 = Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"]($var56[1]), $var56[2]]));
              return parse(new State("FunctionArgs", []), stop, fail, $var56[3], new _List2.default(parsed_1, $var56[4]));

            case 1:
              return null;
          }

      }
    };

    var _CommaFunction___ = __exports["|CommaFunction|_|"] = function (state, stop, fail, _arg7_0, _arg7_1) {
      var _arg7 = [_arg7_0, _arg7_1];
      var $var57 = void 0;

      if (_arg7[0].tail != null) {
        if (_arg7[1].tail != null) {
          var activePatternResult418 = Token["|T|_|"](_arg7[1].head);

          if (activePatternResult418 != null) {
            if (activePatternResult418 === ",") {
              $var57 = [0, _arg7[0].head, _arg7[0].tail, _arg7[1].tail];
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
          var patternInput = parse(state, stop, fail, new _List2.default(), $var57[3]);
          var parsed = void 0;
          var matchValue = patternInput[0].Clean();
          var activePatternResult417 = Token["|X|"](matchValue);

          if (activePatternResult417[0] === ",") {
            parsed = Token.Token[".ctor_2"](",", new _List2.default($var57[1], activePatternResult417[1]));
          } else {
            parsed = Token.Token[".ctor_2"](",", (0, _List.ofArray)([$var57[1], patternInput[0]]));
          }

          return parse(state, stop, fail, $var57[2], new _List2.default(parsed, patternInput[1]));

        case 1:
          return null;
      }
    };

    var _DatatypeLocal___ = __exports["|DatatypeLocal|_|"] = function (state, stop, fail, _arg8_0, _arg8_1) {
      var _arg8 = [_arg8_0, _arg8_1];

      var activePatternResult451 = _DatatypeName___(_arg8[0]);

      if (activePatternResult451 != null) {
        var patternInput = parse(new State("LocalImd", []), function (_arg9) {
          var $var58 = void 0;

          if (_arg9.tail != null) {
            var activePatternResult423 = Token["|T|_|"](_arg9.head);

            if (activePatternResult423 != null) {
              if (activePatternResult423 === ";") {
                $var58 = [0];
              } else if (activePatternResult423 === ",") {
                $var58 = [0];
              } else {
                $var58 = [1];
              }
            } else {
              $var58 = [1];
            }
          } else {
            $var58 = [1];
          }

          switch ($var58[0]) {
            case 0:
              return true;

            case 1:
              return false;
          }
        }, function (e) {
          return stop(e) ? true : fail(e);
        }, new _List2.default(), _arg8[1]);
        var parsed = void 0;
        var matchValue = patternInput[0].Clean();
        var $var59 = void 0;
        var activePatternResult448 = Token["|X|"](matchValue);

        if (activePatternResult448[0] === "assign") {
          if (activePatternResult448[1].tail != null) {
            var activePatternResult449 = Token["|T|_|"](activePatternResult448[1].head);

            if (activePatternResult449 != null) {
              if (activePatternResult448[1].tail.tail != null) {
                if (activePatternResult448[1].tail.tail.tail == null) {
                  $var59 = [0, activePatternResult449, activePatternResult448[1].head, activePatternResult448[1].tail.head];
                } else {
                  $var59 = [1];
                }
              } else {
                $var59 = [1];
              }
            } else {
              $var59 = [1];
            }
          } else {
            $var59 = [1];
          }
        } else {
          $var59 = [1];
        }

        switch ($var59[0]) {
          case 0:
            parsed = Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult451[0]), $var59[2]])), $var59[3]]));
            break;

          case 1:
            var activePatternResult447 = Token["|T|_|"](matchValue);

            if (activePatternResult447 != null) {
              parsed = Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult451[0]), matchValue])), Token.Token[".ctor_3"]("nothing")]));
            } else {
              var $var60 = void 0;
              var activePatternResult444 = Token["|X|"](matchValue);

              if (activePatternResult444[0] === "dot") {
                if (activePatternResult444[1].tail != null) {
                  var activePatternResult445 = Token["|T|_|"](activePatternResult444[1].head);

                  if (activePatternResult445 != null) {
                    if (activePatternResult444[1].tail.tail != null) {
                      var activePatternResult446 = Token["|X|"](activePatternResult444[1].tail.head);

                      if (activePatternResult446[0] === "[]") {
                        if (activePatternResult444[1].tail.tail.tail == null) {
                          $var60 = [0, activePatternResult446[1], activePatternResult445, activePatternResult444[1].head];
                        } else {
                          $var60 = [1];
                        }
                      } else {
                        $var60 = [1];
                      }
                    } else {
                      $var60 = [1];
                    }
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
                  parsed = Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult451[0]), $var60[3]])), Token.Token[".ctor_2"]("array", $var60[1])]));
                  break;

                case 1:
                  var $var61 = void 0;
                  var activePatternResult439 = Token["|X|"](matchValue);

                  if (activePatternResult439[0] === "assign") {
                    if (activePatternResult439[1].tail != null) {
                      var activePatternResult440 = Token["|X|"](activePatternResult439[1].head);

                      if (activePatternResult440[0] === "apply") {
                        if (activePatternResult440[1].tail != null) {
                          var activePatternResult441 = Token["|Pref'|_|"](activePatternResult440[1].head);

                          if (activePatternResult441 != null) {
                            var activePatternResult442 = Token["|T|_|"](activePatternResult441);

                            if (activePatternResult442 != null) {
                              if (activePatternResult442 === "~*") {
                                if (activePatternResult440[1].tail.tail != null) {
                                  var activePatternResult443 = Token["|T|_|"](activePatternResult440[1].tail.head);

                                  if (activePatternResult443 != null) {
                                    if (activePatternResult440[1].tail.tail.tail == null) {
                                      if (activePatternResult439[1].tail.tail != null) {
                                        if (activePatternResult439[1].tail.tail.tail == null) {
                                          $var61 = [0, activePatternResult440[1].tail.head, activePatternResult439[1].tail.head];
                                        } else {
                                          $var61 = [1];
                                        }
                                      } else {
                                        $var61 = [1];
                                      }
                                    } else {
                                      $var61 = [1];
                                    }
                                  } else {
                                    $var61 = [1];
                                  }
                                } else {
                                  $var61 = [1];
                                }
                              } else {
                                $var61 = [1];
                              }
                            } else {
                              $var61 = [1];
                            }
                          } else {
                            $var61 = [1];
                          }
                        } else {
                          $var61 = [1];
                        }
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
                      parsed = Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult451[0] + "*"), $var61[1]])), $var61[2]]));
                      break;

                    case 1:
                      var $var62 = void 0;
                      var activePatternResult435 = Token["|X|"](matchValue);

                      if (activePatternResult435[0] === "apply") {
                        if (activePatternResult435[1].tail != null) {
                          var activePatternResult436 = Token["|Pref'|_|"](activePatternResult435[1].head);

                          if (activePatternResult436 != null) {
                            var activePatternResult437 = Token["|T|_|"](activePatternResult436);

                            if (activePatternResult437 != null) {
                              if (activePatternResult437 === "~*") {
                                if (activePatternResult435[1].tail.tail != null) {
                                  var activePatternResult438 = Token["|T|_|"](activePatternResult435[1].tail.head);

                                  if (activePatternResult438 != null) {
                                    if (activePatternResult435[1].tail.tail.tail == null) {
                                      $var62 = [0, activePatternResult435[1].tail.head];
                                    } else {
                                      $var62 = [1];
                                    }
                                  } else {
                                    $var62 = [1];
                                  }
                                } else {
                                  $var62 = [1];
                                }
                              } else {
                                $var62 = [1];
                              }
                            } else {
                              $var62 = [1];
                            }
                          } else {
                            $var62 = [1];
                          }
                        } else {
                          $var62 = [1];
                        }
                      } else {
                        $var62 = [1];
                      }

                      switch ($var62[0]) {
                        case 0:
                          parsed = Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult451[0] + "*"), $var62[1]])), Token.Token[".ctor_3"]("nothing")]));
                          break;

                        case 1:
                          var $var63 = void 0;
                          var activePatternResult429 = Token["|X|"](matchValue);

                          if (activePatternResult429[0] === "dot") {
                            if (activePatternResult429[1].tail != null) {
                              var activePatternResult430 = Token["|X|"](activePatternResult429[1].head);

                              if (activePatternResult430[0] === "apply") {
                                if (activePatternResult430[1].tail != null) {
                                  var activePatternResult431 = Token["|Pref'|_|"](activePatternResult430[1].head);

                                  if (activePatternResult431 != null) {
                                    var activePatternResult432 = Token["|T|_|"](activePatternResult431);

                                    if (activePatternResult432 != null) {
                                      if (activePatternResult432 === "~*") {
                                        if (activePatternResult430[1].tail.tail != null) {
                                          var activePatternResult433 = Token["|T|_|"](activePatternResult430[1].tail.head);

                                          if (activePatternResult433 != null) {
                                            if (activePatternResult430[1].tail.tail.tail == null) {
                                              if (activePatternResult429[1].tail.tail != null) {
                                                var activePatternResult434 = Token["|X|"](activePatternResult429[1].tail.head);

                                                if (activePatternResult434[0] === "[]") {
                                                  if (activePatternResult429[1].tail.tail.tail == null) {
                                                    $var63 = [0, activePatternResult434[1], activePatternResult430[1].tail.head];
                                                  } else {
                                                    $var63 = [1];
                                                  }
                                                } else {
                                                  $var63 = [1];
                                                }
                                              } else {
                                                $var63 = [1];
                                              }
                                            } else {
                                              $var63 = [1];
                                            }
                                          } else {
                                            $var63 = [1];
                                          }
                                        } else {
                                          $var63 = [1];
                                        }
                                      } else {
                                        $var63 = [1];
                                      }
                                    } else {
                                      $var63 = [1];
                                    }
                                  } else {
                                    $var63 = [1];
                                  }
                                } else {
                                  $var63 = [1];
                                }
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
                              parsed = Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult451[0] + "*"), $var63[2]])), Token.Token[".ctor_2"]("array", $var63[1])]));
                              break;

                            case 1:
                              parsed = (0, _String.fsFormat)("could not recognize declaration: %A")(function (x) {
                                throw new Error(x);
                              })(matchValue);
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
        var $var64 = void 0;

        if (patternInput[1].tail != null) {
          var activePatternResult450 = Token["|T|_|"](patternInput[1].head);

          if (activePatternResult450 != null) {
            if (activePatternResult450 === ",") {
              $var64 = [0, patternInput[1].tail];
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
            restr = (0, _List.ofArray)([Token.Token[".ctor_3"](";"), Token.Token[".ctor_3"](activePatternResult451[0])], $var64[1]);
            break;

          case 1:
            restr = patternInput[1];
            break;
        }

        return parse(new State("LocalImd", []), stop, fail, activePatternResult451[1], new _List2.default(parsed, restr));
      } else {
        return null;
      }
    };

    var _Brackets___ = __exports["|Brackets|_|"] = function (state, stop, fail, _arg10_0, _arg10_1) {
      var _arg10 = [_arg10_0, _arg10_1];
      var $var65 = void 0;

      if (_arg10[0].tail != null) {
        var activePatternResult460 = Token["|T|_|"](_arg10[0].head);

        if (activePatternResult460 != null) {
          if (activePatternResult460 === "(") {
            $var65 = [0, _arg10[0].tail, _arg10[1]];
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
          var patternInput = parse(new State("LocalImd", []), function (_arg11) {
            var $var66 = void 0;

            if (_arg11.tail != null) {
              var activePatternResult456 = Token["|T|_|"](_arg11.head);

              if (activePatternResult456 != null) {
                if (activePatternResult456 === ")") {
                  $var66 = [0];
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
                return true;

              case 1:
                return false;
            }
          }, function (_arg1) {
            return false;
          }, new _List2.default(), $var65[2]);
          var $var67 = void 0;

          if (patternInput[1].tail != null) {
            var activePatternResult459 = Token["|T|_|"](patternInput[1].head);

            if (activePatternResult459 != null) {
              if (activePatternResult459 === ")") {
                $var67 = [0, patternInput[0], patternInput[1].tail];
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
              var parsed = Token.Token[".ctor_2"]("()", (0, _List.ofArray)([$var67[1]]));
              return parse(new State("LocalImd", []), stop, fail, $var65[1], new _List2.default(parsed, $var67[2]));

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 524, 10);
          }

        case 1:
          return null;
      }
    };

    var _Braces___ = __exports["|Braces|_|"] = function (state, stop, fail, _arg12_0, _arg12_1) {
      var _arg12 = [_arg12_0, _arg12_1];
      var $var68 = void 0;

      if (_arg12[0].tail != null) {
        var activePatternResult469 = Token["|T|_|"](_arg12[0].head);

        if (activePatternResult469 != null) {
          if (activePatternResult469 === "{") {
            $var68 = [0, _arg12[0].tail, _arg12[1]];
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
          var patternInput = parse(new State("Local", []), function (_arg13) {
            var $var69 = void 0;

            if (_arg13.tail != null) {
              var activePatternResult465 = Token["|T|_|"](_arg13.head);

              if (activePatternResult465 != null) {
                if (activePatternResult465 === "}") {
                  $var69 = [0];
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
                return true;

              case 1:
                return false;
            }
          }, function (e) {
            return stop(e) ? true : fail(e);
          }, new _List2.default(), $var68[2]);
          var $var70 = void 0;

          if (patternInput[1].tail != null) {
            var activePatternResult468 = Token["|T|_|"](patternInput[1].head);

            if (activePatternResult468 != null) {
              if (activePatternResult468 === "}") {
                $var70 = [0, patternInput[0], patternInput[1].tail];
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
              var parsed = Token.Token[".ctor_2"]("{}", (0, _List.ofArray)([$var70[1]]));
              return parse(new State("Local", []), stop, fail, $var68[1], new _List2.default(parsed, $var70[2]));

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 531, 10);
          }

        case 1:
          return null;
      }
    };

    var getStatementBody = __exports.getStatementBody = function (stop, fail, right) {
      var $var71 = void 0;

      if (right.tail != null) {
        var activePatternResult482 = Token["|T|_|"](right.head);

        if (activePatternResult482 != null) {
          if (activePatternResult482 === "{") {
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
          var patternInput = parse(new State("Local", []), function (_arg14) {
            var $var72 = void 0;

            if (_arg14.tail != null) {
              var activePatternResult474 = Token["|X|"](_arg14.head);

              if (activePatternResult474[0] === "{}") {
                if (activePatternResult474[1].tail != null) {
                  if (activePatternResult474[1].tail.tail == null) {
                    $var72 = [0];
                  } else {
                    $var72 = [1];
                  }
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
                return true;

              case 1:
                return false;
            }
          }, function (e) {
            return stop(e) ? true : fail(e);
          }, new _List2.default(), right);
          var $var73 = void 0;
          var activePatternResult477 = Token["|X|"](patternInput[0]);

          if (activePatternResult477[0] === "sequence") {
            if (activePatternResult477[1].tail == null) {
              if (patternInput[1].tail != null) {
                var activePatternResult478 = Token["|X|"](patternInput[1].head);

                if (activePatternResult478[0] === "{}") {
                  if (activePatternResult478[1].tail != null) {
                    if (activePatternResult478[1].tail.tail == null) {
                      $var73 = [0, activePatternResult478[1].head, patternInput[1].tail];
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
          } else {
            $var73 = [1];
          }

          switch ($var73[0]) {
            case 0:
              return [$var73[1], new _List2.default(Token.Token[".ctor_3"](";"), $var73[2])];

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 539, 10);
          }

        case 1:
          var patternInput_1 = parse(new State("Local", []), function (_arg15) {
            var $var74 = void 0;

            if (_arg15.tail != null) {
              var activePatternResult479 = Token["|T|_|"](_arg15.head);

              if (activePatternResult479 != null) {
                if (activePatternResult479 === ";") {
                  $var74 = [0];
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
                return true;

              case 1:
                return false;
            }
          }, function (e_1) {
            return stop(e_1) ? true : fail(e_1);
          }, new _List2.default(), right);
          return [patternInput_1[0], patternInput_1[1]];
      }
    };

    var _If___ = __exports["|If|_|"] = function (state, stop, fail, _arg16_0, _arg16_1) {
      var _arg16 = [_arg16_0, _arg16_1];
      var $var75 = void 0;

      if (_arg16[0].tail != null) {
        var activePatternResult494 = Token["|T|_|"](_arg16[0].head);

        if (activePatternResult494 != null) {
          if (activePatternResult494 === "if") {
            if (_arg16[1].tail != null) {
              var activePatternResult495 = Token["|T|_|"](_arg16[1].head);

              if (activePatternResult495 != null) {
                if (activePatternResult495 === "(") {
                  $var75 = [0, _arg16[0].tail, _arg16[1]];
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
          var patternInput = parse(new State("LocalImd", []), function (_arg17) {
            var $var76 = void 0;

            if (_arg17.tail != null) {
              var activePatternResult486 = Token["|X|"](_arg17.head);

              if (activePatternResult486[0] === "()") {
                $var76 = [0];
              } else {
                $var76 = [1];
              }
            } else {
              $var76 = [1];
            }

            switch ($var76[0]) {
              case 0:
                return true;

              case 1:
                return false;
            }
          }, function (e) {
            return stop(e) ? true : fail(e);
          }, new _List2.default(), $var75[2]);
          var $var77 = void 0;
          var activePatternResult492 = Token["|X|"](patternInput[0]);

          if (activePatternResult492[0] === "sequence") {
            if (activePatternResult492[1].tail == null) {
              if (patternInput[1].tail != null) {
                var activePatternResult493 = Token["|X|"](patternInput[1].head);

                if (activePatternResult493[0] === "()") {
                  if (activePatternResult493[1].tail != null) {
                    if (activePatternResult493[1].tail.tail == null) {
                      $var77 = [0, activePatternResult493[1].head, patternInput[1].tail];
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
              var patternInput_1 = getStatementBody(stop, fail, $var77[2]);
              var patternInput_2 = void 0;
              var matchValue = patternInput_1[1].tail;
              var $var78 = void 0;

              if (matchValue.tail != null) {
                var activePatternResult490 = Token["|T|_|"](matchValue.head);

                if (activePatternResult490 != null) {
                  if (activePatternResult490 === "else") {
                    if (matchValue.tail.tail != null) {
                      var activePatternResult491 = Token["|T|_|"](matchValue.tail.head);

                      if (activePatternResult491 != null) {
                        if (activePatternResult491 === "if") {
                          $var78 = [0, matchValue.tail.tail];
                        } else {
                          $var78 = [1];
                        }
                      } else {
                        $var78 = [1];
                      }
                    } else {
                      $var78 = [1];
                    }
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
                  patternInput_2 = parse(new State("Local", []), stop, fail, new _List2.default(), new _List2.default(Token.Token[".ctor_3"]("if"), $var78[1]));
                  break;

                case 1:
                  var $var79 = void 0;

                  if (matchValue.tail != null) {
                    var activePatternResult489 = Token["|T|_|"](matchValue.head);

                    if (activePatternResult489 != null) {
                      if (activePatternResult489 === "else") {
                        $var79 = [0, matchValue.tail];
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
                      patternInput_2 = getStatementBody(stop, fail, $var79[1]);
                      break;

                    case 1:
                      patternInput_2 = [Token.Token[".ctor_2"]("sequence", new _List2.default()), patternInput_1[1]];
                      break;
                  }

                  break;
              }

              var parsed = Token.Token[".ctor_2"]("if", (0, _List.ofArray)([$var77[1], patternInput_1[0], patternInput_2[0]]));
              return parse(new State("Local", []), stop, fail, $var75[1], new _List2.default(parsed, patternInput_2[1]));

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 549, 10);
          }

        case 1:
          return null;
      }
    };

    var _While___ = __exports["|While|_|"] = function (state, stop, fail, _arg18_0, _arg18_1) {
      var _arg18 = [_arg18_0, _arg18_1];
      var $var80 = void 0;

      if (_arg18[0].tail != null) {
        var activePatternResult505 = Token["|T|_|"](_arg18[0].head);

        if (activePatternResult505 != null) {
          if (activePatternResult505 === "while") {
            if (_arg18[1].tail != null) {
              var activePatternResult506 = Token["|T|_|"](_arg18[1].head);

              if (activePatternResult506 != null) {
                if (activePatternResult506 === "(") {
                  $var80 = [0, _arg18[0].tail, _arg18[1]];
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
          var patternInput = parse(new State("LocalImd", []), function (_arg19) {
            var $var81 = void 0;

            if (_arg19.tail != null) {
              var activePatternResult500 = Token["|X|"](_arg19.head);

              if (activePatternResult500[0] === "()") {
                $var81 = [0];
              } else {
                $var81 = [1];
              }
            } else {
              $var81 = [1];
            }

            switch ($var81[0]) {
              case 0:
                return true;

              case 1:
                return false;
            }
          }, function (e) {
            return stop(e) ? true : fail(e);
          }, new _List2.default(), $var80[2]);
          var $var82 = void 0;
          var activePatternResult503 = Token["|X|"](patternInput[0]);

          if (activePatternResult503[0] === "sequence") {
            if (activePatternResult503[1].tail == null) {
              if (patternInput[1].tail != null) {
                var activePatternResult504 = Token["|X|"](patternInput[1].head);

                if (activePatternResult504[0] === "()") {
                  if (activePatternResult504[1].tail != null) {
                    if (activePatternResult504[1].tail.tail == null) {
                      $var82 = [0, activePatternResult504[1].head, patternInput[1].tail];
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
              var patternInput_1 = getStatementBody(stop, fail, $var82[2]);
              var parsed = Token.Token[".ctor_2"]("while", (0, _List.ofArray)([$var82[1], patternInput_1[0]]));
              return parse(new State("Local", []), stop, fail, $var80[1], new _List2.default(parsed, patternInput_1[1]));

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 562, 10);
          }

        case 1:
          return null;
      }
    };

    var _For___ = __exports["|For|_|"] = function (state, stop, fail, _arg20_0, _arg20_1) {
      var _arg20 = [_arg20_0, _arg20_1];
      var $var83 = void 0;

      if (_arg20[0].tail != null) {
        var activePatternResult523 = Token["|T|_|"](_arg20[0].head);

        if (activePatternResult523 != null) {
          if (activePatternResult523 === "for") {
            if (_arg20[1].tail != null) {
              var activePatternResult524 = Token["|T|_|"](_arg20[1].head);

              if (activePatternResult524 != null) {
                if (activePatternResult524 === "(") {
                  $var83 = [0, _arg20[0].tail, _arg20[1].tail];
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
          var patternInput = parse(new State("Local", []), function (_arg21) {
            var $var84 = void 0;

            if (_arg21.tail != null) {
              var activePatternResult511 = Token["|T|_|"](_arg21.head);

              if (activePatternResult511 != null) {
                if (activePatternResult511 === ";") {
                  $var84 = [0];
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
                return true;

              case 1:
                return false;
            }
          }, function (e) {
            return stop(e) ? true : fail(e);
          }, new _List2.default(), $var83[2]);
          var $var85 = void 0;

          if (patternInput[1].tail != null) {
            var activePatternResult522 = Token["|T|_|"](patternInput[1].head);

            if (activePatternResult522 != null) {
              if (activePatternResult522 === ";") {
                $var85 = [0, patternInput[0], patternInput[1].tail];
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
              var patternInput_1 = parse(new State("Local", []), function (_arg22) {
                var $var86 = void 0;

                if (_arg22.tail != null) {
                  var activePatternResult514 = Token["|T|_|"](_arg22.head);

                  if (activePatternResult514 != null) {
                    if (activePatternResult514 === ";") {
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
              }, function (e_1) {
                return stop(e_1) ? true : fail(e_1);
              }, new _List2.default(), $var85[2]);
              var $var87 = void 0;

              if (patternInput_1[1].tail != null) {
                var activePatternResult521 = Token["|T|_|"](patternInput_1[1].head);

                if (activePatternResult521 != null) {
                  if (activePatternResult521 === ";") {
                    $var87 = [0, patternInput_1[0], patternInput_1[1].tail];
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
                  var patternInput_2 = parse(new State("Local", []), function (_arg23) {
                    var $var88 = void 0;

                    if (_arg23.tail != null) {
                      var activePatternResult517 = Token["|T|_|"](_arg23.head);

                      if (activePatternResult517 != null) {
                        if (activePatternResult517 === ")") {
                          $var88 = [0];
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
                        return true;

                      case 1:
                        return false;
                    }
                  }, function (e_2) {
                    return stop(e_2) ? true : fail(e_2);
                  }, new _List2.default(), $var87[2]);
                  var $var89 = void 0;

                  if (patternInput_2[1].tail != null) {
                    var activePatternResult520 = Token["|T|_|"](patternInput_2[1].head);

                    if (activePatternResult520 != null) {
                      if (activePatternResult520 === ")") {
                        $var89 = [0, patternInput_2[0], patternInput_2[1].tail];
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
                      var patternInput_3 = getStatementBody(stop, fail, $var89[2]);
                      var parsed = Token.Token[".ctor_2"]("sequence", (0, _List.ofArray)([$var85[1], Token.Token[".ctor_2"]("while", (0, _List.ofArray)([$var87[1], Token.Token[".ctor_2"]("sequence", (0, _List.ofArray)([patternInput_3[0], $var89[1]]))]))]));
                      return parse(new State("Local", []), stop, fail, $var83[1], new _List2.default(parsed, patternInput_3[1]));

                    case 1:
                      throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 575, 10);
                  }

                case 1:
                  throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 573, 10);
              }

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 570, 10);
          }

        case 1:
          return null;
      }
    };

    var _Return___ = __exports["|Return|_|"] = function (state, stop, fail, _arg24_0, _arg24_1) {
      var _arg24 = [_arg24_0, _arg24_1];
      var $var90 = void 0;

      if (_arg24[0].tail != null) {
        var activePatternResult532 = Token["|T|_|"](_arg24[0].head);

        if (activePatternResult532 != null) {
          if (activePatternResult532 === "return") {
            $var90 = [0, _arg24[0].tail, _arg24[1]];
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
          var patternInput = parse(new State("LocalImd", []), function (_arg25) {
            var $var91 = void 0;

            if (_arg25.tail != null) {
              var activePatternResult529 = Token["|T|_|"](_arg25.head);

              if (activePatternResult529 != null) {
                if (activePatternResult529 === ";") {
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
                return true;

              case 1:
                return false;
            }
          }, function (e) {
            return stop(e) ? true : fail(e);
          }, new _List2.default(), $var90[2]);
          var parsed = Token.Token[".ctor_2"]("return", (0, _List.ofArray)([patternInput[0]]));
          return parse(new State("Local", []), stop, fail, $var90[1], new _List2.default(parsed, patternInput[1]));

        case 1:
          return null;
      }
    };

    var _Assignment___ = __exports["|Assignment|_|"] = function (state, stop, fail, _arg26_0, _arg26_1) {
      var _arg26 = [_arg26_0, _arg26_1];
      var $var92 = void 0;

      if (_arg26[0].tail != null) {
        if (_arg26[1].tail != null) {
          var activePatternResult539 = Token["|T|_|"](_arg26[1].head);

          if (activePatternResult539 != null) {
            if (activePatternResult539 === "=") {
              $var92 = [0, _arg26[0].head, _arg26[0].tail, _arg26[1].tail];
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
          var patternInput = parse(new State("LocalImd", []), function (_arg27) {
            var $var93 = void 0;

            if (_arg27.tail != null) {
              var activePatternResult537 = Token["|T|_|"](_arg27.head);

              if (activePatternResult537 != null) {
                if (activePatternResult537 === ";") {
                  $var93 = [0];
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
                return true;

              case 1:
                return stop(_arg27);
            }
          }, fail, new _List2.default(), $var92[3]);
          var parsed = Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([$var92[1], patternInput[0]]));
          return parse(new State("LocalImd", []), stop, fail, $var92[2], new _List2.default(parsed, patternInput[1]));

        case 1:
          return null;
      }
    };

    var _Dot___ = __exports["|Dot|_|"] = function (state, stop, fail, _arg28_0, _arg28_1) {
      var _arg28 = [_arg28_0, _arg28_1];
      var $var94 = void 0;

      if (_arg28[0].tail != null) {
        if (_arg28[1].tail != null) {
          var activePatternResult544 = Token["|T|_|"](_arg28[1].head);

          if (activePatternResult544 != null) {
            if (activePatternResult544 === ".") {
              if (_arg28[1].tail.tail != null) {
                var activePatternResult545 = Token["|T|_|"](_arg28[1].tail.head);

                if (activePatternResult545 != null) {
                  $var94 = [0, _arg28[0].head, activePatternResult545, _arg28[0].tail, _arg28[1].tail.tail];
                } else {
                  $var94 = [1];
                }
              } else {
                $var94 = [1];
              }
            } else {
              $var94 = [1];
            }
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
          var parsed = Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var94[1], Token.Token[".ctor_3"]($var94[2])]));
          return parse(state, stop, fail, $var94[3], new _List2.default(parsed, $var94[4]));

        case 1:
          return null;
      }
    };

    var _Prefix___ = __exports["|Prefix|_|"] = function (state, stop, fail, _arg29_0, _arg29_1) {
      var _arg29 = [_arg29_0, _arg29_1];
      var $var95 = void 0;

      if (_arg29[0].tail != null) {
        var activePatternResult567 = Token["|Pref|_|"](_arg29[0].head);

        if (activePatternResult567 != null) {
          $var95 = [0, activePatternResult567, _arg29[0].tail, _arg29[1]];
        } else {
          $var95 = [1];
        }
      } else {
        $var95 = [1];
      }

      switch ($var95[0]) {
        case 0:
          var patternInput_2 = void 0;
          var $var96 = void 0;

          if ($var95[3].tail != null) {
            var activePatternResult565 = Token["|T|_|"]($var95[3].head);

            if (activePatternResult565 != null) {
              if ($var95[3].tail.tail != null) {
                var activePatternResult566 = Token["|T|_|"]($var95[3].tail.head);

                if (activePatternResult566 != null) {
                  if (activePatternResult566 === "[") {
                    $var96 = [0, $var95[3].tail, activePatternResult565];
                  } else {
                    $var96 = [1];
                  }
                } else {
                  $var96 = [1];
                }
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
              var patternInput = parse(state, function (_arg30) {
                var $var97 = void 0;

                if (_arg30.tail != null) {
                  var activePatternResult550 = Token["|T|_|"](_arg30.head);

                  if (activePatternResult550 != null) {
                    $var97 = [0];
                  } else {
                    $var97 = [1];
                  }
                } else {
                  $var97 = [1];
                }

                switch ($var97[0]) {
                  case 0:
                    return false;

                  case 1:
                    return true;
                }
              }, function (e) {
                return stop(e) ? true : fail(e);
              }, new _List2.default(), $var95[3]);
              var $var98 = void 0;
              var activePatternResult554 = Token["|T|_|"](patternInput[0]);

              if (activePatternResult554 != null) {
                if (activePatternResult554 === "()") {
                  if (patternInput[1].tail != null) {
                    $var98 = [0, patternInput[1].head, patternInput[1].tail];
                  } else {
                    $var98 = [1];
                  }
                } else {
                  var activePatternResult555 = Token["|X|"](patternInput[0]);

                  if (activePatternResult555[0] === "sequence") {
                    if (activePatternResult555[1].tail == null) {
                      if (patternInput[1].tail != null) {
                        $var98 = [0, patternInput[1].head, patternInput[1].tail];
                      } else {
                        $var98 = [1];
                      }
                    } else {
                      $var98 = [1];
                    }
                  } else {
                    $var98 = [1];
                  }
                }
              } else {
                var activePatternResult556 = Token["|X|"](patternInput[0]);

                if (activePatternResult556[0] === "sequence") {
                  if (activePatternResult556[1].tail == null) {
                    if (patternInput[1].tail != null) {
                      $var98 = [0, patternInput[1].head, patternInput[1].tail];
                    } else {
                      $var98 = [1];
                    }
                  } else {
                    $var98 = [1];
                  }
                } else {
                  $var98 = [1];
                }
              }

              switch ($var98[0]) {
                case 0:
                  patternInput_2 = [$var98[1], $var98[2]];
                  break;

                case 1:
                  throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 605, 14);
                  break;
              }

              break;

            case 1:
              var $var99 = void 0;

              if ($var95[3].tail != null) {
                var activePatternResult564 = Token["|T|_|"]($var95[3].head);

                if (activePatternResult564 != null) {
                  if (Lexer.CommonClassifiers.op_GreaterGreaterBarBar(Lexer.CommonClassifiers.isNumeric, Lexer.CommonClassifiers.isVariable, activePatternResult564)) {
                    $var99 = [0, $var95[3].head, $var95[3].tail, activePatternResult564];
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
                  patternInput_2 = [$var99[1], $var99[2]];
                  break;

                case 1:
                  var patternInput_1 = parse(state, function (_arg31) {
                    var $var100 = void 0;

                    if (_arg31.tail != null) {
                      var activePatternResult557 = Token["|T|_|"](_arg31.head);

                      if (activePatternResult557 != null) {
                        $var100 = [0];
                      } else {
                        $var100 = [1];
                      }
                    } else {
                      $var100 = [1];
                    }

                    switch ($var100[0]) {
                      case 0:
                        return false;

                      case 1:
                        return true;
                    }
                  }, function (e_1) {
                    return stop(e_1) ? true : fail(e_1);
                  }, new _List2.default(), $var95[3]);
                  var $var101 = void 0;
                  var activePatternResult561 = Token["|T|_|"](patternInput_1[0]);

                  if (activePatternResult561 != null) {
                    if (activePatternResult561 === "()") {
                      if (patternInput_1[1].tail != null) {
                        $var101 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                      } else {
                        $var101 = [1];
                      }
                    } else {
                      var activePatternResult562 = Token["|X|"](patternInput_1[0]);

                      if (activePatternResult562[0] === "sequence") {
                        if (activePatternResult562[1].tail == null) {
                          if (patternInput_1[1].tail != null) {
                            $var101 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                          } else {
                            $var101 = [1];
                          }
                        } else {
                          $var101 = [1];
                        }
                      } else {
                        $var101 = [1];
                      }
                    }
                  } else {
                    var activePatternResult563 = Token["|X|"](patternInput_1[0]);

                    if (activePatternResult563[0] === "sequence") {
                      if (activePatternResult563[1].tail == null) {
                        if (patternInput_1[1].tail != null) {
                          $var101 = [0, patternInput_1[1].head, patternInput_1[1].tail];
                        } else {
                          $var101 = [1];
                        }
                      } else {
                        $var101 = [1];
                      }
                    } else {
                      $var101 = [1];
                    }
                  }

                  switch ($var101[0]) {
                    case 0:
                      patternInput_2 = [$var101[1], $var101[2]];
                      break;

                    case 1:
                      throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 610, 14);
                      break;
                  }

                  break;
              }

              break;
          }

          var parsed = new Token.Token("apply", $var95[1].Indentation, true, (0, _List.ofArray)([$var95[1], patternInput_2[0]]));
          return parse(state, stop, fail, $var95[2], new _List2.default(parsed, patternInput_2[1]));

        case 1:
          return null;
      }
    };

    var _Operator___ = __exports["|Operator|_|"] = function (state, stop, fail, _arg32_0, _arg32_1) {
      var _arg32 = [_arg32_0, _arg32_1];
      var $var102 = void 0;

      if (_arg32[0].tail != null) {
        if (_arg32[1].tail != null) {
          var activePatternResult575 = Token["|T|_|"](_arg32[1].head);

          if (activePatternResult575 != null) {
            if (function () {
              var restr = _arg32[1].tail;
              var restl = _arg32[0].tail;
              var nfx = _arg32[1].head;
              var a = _arg32[0].head;
              return nfx.Priority !== -1;
            }()) {
              $var102 = [0, _arg32[0].head, _arg32[1].head, _arg32[0].tail, _arg32[1].tail, activePatternResult575];
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
          var patternInput = parse(new State("LocalImd", []), function (_arg33) {
            var $var103 = void 0;

            if (_arg33.tail != null) {
              var activePatternResult573 = Token["|T|_|"](_arg33.head);

              if (activePatternResult573 != null) {
                if (_arg33.head.Priority <= $var102[2].Priority ? _arg33.head.Priority !== -1 : false) {
                  $var103 = [0, _arg33.head];
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
                return true;

              case 1:
                var $var104 = void 0;

                if (_arg33.tail != null) {
                  var activePatternResult572 = Token["|T|_|"](_arg33.head);

                  if (activePatternResult572 != null) {
                    if (activePatternResult572 === ";") {
                      $var104 = [0];
                    } else if (activePatternResult572 === ",") {
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
                    return stop(_arg33);
                }

            }
          }, fail, new _List2.default(), $var102[4]);
          var parsed = Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([Token.Token[".ctor_3"]($var102[5]), $var102[1]])), patternInput[0]]));
          return parse(new State("LocalImd", []), stop, fail, $var102[3], new _List2.default(parsed, patternInput[1]));

        case 1:
          return null;
      }
    };

    var _Apply___ = __exports["|Apply|_|"] = function (state, stop, fail, _arg34_0, _arg34_1) {
      var _arg34 = [_arg34_0, _arg34_1];
      var $var105 = void 0;

      if (_arg34[0].tail != null) {
        if (_arg34[1].tail != null) {
          var activePatternResult588 = Token["|T|_|"](_arg34[1].head);

          if (activePatternResult588 != null) {
            if (activePatternResult588 === "(") {
              $var105 = [0, _arg34[0].head, _arg34[0].tail, _arg34[1].tail];
            } else {
              $var105 = [1];
            }
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
          var state_ = state.Case === "Global" ? new State("FunctionArgs", []) : new State("LocalImd", []);
          var patternInput = void 0;
          var matchValue = parse(state_, function (_arg35) {
            var $var106 = void 0;

            if (_arg35.tail != null) {
              var activePatternResult580 = Token["|T|_|"](_arg35.head);

              if (activePatternResult580 != null) {
                if (activePatternResult580 === ")") {
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
          }, function (_arg2) {
            return false;
          }, new _List2.default(), $var105[3]);
          var $var107 = void 0;
          var activePatternResult586 = Token["|X|"](matchValue[0]);

          if (activePatternResult586[0] === "sequence") {
            if (activePatternResult586[1].tail != null) {
              if (activePatternResult586[1].tail.tail == null) {
                if (matchValue[1].tail != null) {
                  var activePatternResult587 = Token["|T|_|"](matchValue[1].head);

                  if (activePatternResult587 != null) {
                    if (activePatternResult587 === ")") {
                      $var107 = [0, activePatternResult586[1].head, matchValue[1].tail];
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
            } else {
              $var107 = [1];
            }
          } else {
            $var107 = [1];
          }

          switch ($var107[0]) {
            case 0:
              patternInput = [$var107[1], $var107[2]];
              break;

            case 1:
              var $var108 = void 0;
              var activePatternResult584 = Token["|X|"](matchValue[0]);

              if (activePatternResult584[0] === "sequence") {
                if (activePatternResult584[1].tail == null) {
                  if (matchValue[1].tail != null) {
                    var activePatternResult585 = Token["|T|_|"](matchValue[1].head);

                    if (activePatternResult585 != null) {
                      if (activePatternResult585 === ")") {
                        $var108 = [0, matchValue[1].tail];
                      } else {
                        $var108 = [1];
                      }
                    } else {
                      $var108 = [1];
                    }
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
                  patternInput = [Token.Token[".ctor_2"]("_", new _List2.default()), $var108[1]];
                  break;

                case 1:
                  patternInput = (0, _String.fsFormat)("arguments were not formatted correctly %A")(function (x) {
                    throw new Error(x);
                  })(matchValue);
                  break;
              }

              break;
          }

          var parsed = Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var105[1], patternInput[0]]));
          return parse(new State("LocalImd", []), stop, fail, $var105[2], new _List2.default(parsed, patternInput[1]));

        case 1:
          return null;
      }
    };

    var _Index___ = __exports["|Index|_|"] = function (state, stop, fail, _arg36_0, _arg36_1) {
      var _arg36 = [_arg36_0, _arg36_1];
      var $var109 = void 0;

      if (_arg36[0].tail != null) {
        if (_arg36[1].tail != null) {
          var activePatternResult597 = Token["|T|_|"](_arg36[1].head);

          if (activePatternResult597 != null) {
            if (activePatternResult597 === "[") {
              $var109 = [0, _arg36[0].head, _arg36[0].tail, _arg36[1].tail];
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
          var patternInput = parse(state, function (_arg37) {
            var $var110 = void 0;

            if (_arg37.tail != null) {
              var activePatternResult593 = Token["|T|_|"](_arg37.head);

              if (activePatternResult593 != null) {
                if (activePatternResult593 === "]") {
                  $var110 = [0];
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
                return true;

              case 1:
                return false;
            }
          }, function (_arg3) {
            return false;
          }, new _List2.default(), $var109[3]);
          var $var111 = void 0;

          if (patternInput[1].tail != null) {
            var activePatternResult596 = Token["|T|_|"](patternInput[1].head);

            if (activePatternResult596 != null) {
              if (activePatternResult596 === "]") {
                $var111 = [0, patternInput[0], patternInput[1].tail];
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
              var parsed = Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([$var109[1], Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([$var111[1]]))]));
              return parse(state, stop, fail, $var109[2], new _List2.default(parsed, $var111[2]));

            case 1:
              throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 638, 10);
          }

        case 1:
          return null;
      }
    };

    var _Transfer___ = __exports["|Transfer|_|"] = function (state, stop, fail, _arg38_0, _arg38_1) {
      var _arg38 = [_arg38_0, _arg38_1];

      if (_arg38[1].tail != null) {
        var _x = _arg38[1].head;
        var restr = _arg38[1].tail;
        return parse(state, stop, fail, new _List2.default(_x, _arg38[0]), restr);
      } else {
        return null;
      }
    };

    var postProcess = __exports.postProcess = function (_arg1) {
      postProcess: while (true) {
        var $var112 = void 0;
        var activePatternResult613 = Token["|X|"](_arg1);

        if (activePatternResult613[0] === "declare function") {
          if (activePatternResult613[1].tail != null) {
            if (activePatternResult613[1].tail.tail != null) {
              if (activePatternResult613[1].tail.tail.tail != null) {
                if (activePatternResult613[1].tail.tail.tail.tail != null) {
                  if (activePatternResult613[1].tail.tail.tail.tail.tail == null) {
                    $var112 = [0, activePatternResult613[1].tail.tail.head, activePatternResult613[1].head, activePatternResult613[1].tail.head, activePatternResult613[1].tail.tail.tail.head];
                  } else {
                    $var112 = [5];
                  }
                } else {
                  $var112 = [5];
                }
              } else {
                $var112 = [5];
              }
            } else {
              $var112 = [5];
            }
          } else {
            $var112 = [5];
          }
        } else if (activePatternResult613[0] === "{}") {
          $var112 = [1, activePatternResult613[1]];
        } else if (activePatternResult613[0] === "sequence") {
          $var112 = [2, activePatternResult613[1]];
        } else if (activePatternResult613[0] === "==") {
          $var112 = [3, activePatternResult613[1]];
        } else if (activePatternResult613[0] === "apply") {
          if (activePatternResult613[1].tail != null) {
            var activePatternResult614 = Token["|T|_|"](activePatternResult613[1].head);

            if (activePatternResult614 != null) {
              if (activePatternResult614 === "printf") {
                if (activePatternResult613[1].tail.tail != null) {
                  var activePatternResult615 = Token["|X|"](activePatternResult613[1].tail.head);

                  if (activePatternResult615[0] === ",") {
                    if (activePatternResult615[1].tail != null) {
                      if (activePatternResult613[1].tail.tail.tail == null) {
                        $var112 = [4, activePatternResult615[1].tail, activePatternResult615[1].head, activePatternResult613[1].head];
                      } else {
                        $var112 = [5];
                      }
                    } else {
                      $var112 = [5];
                    }
                  } else {
                    $var112 = [5];
                  }
                } else {
                  $var112 = [5];
                }
              } else if (activePatternResult614 === "sprintf") {
                if (activePatternResult613[1].tail.tail != null) {
                  var activePatternResult616 = Token["|X|"](activePatternResult613[1].tail.head);

                  if (activePatternResult616[0] === ",") {
                    if (activePatternResult616[1].tail != null) {
                      if (activePatternResult613[1].tail.tail.tail == null) {
                        $var112 = [4, activePatternResult616[1].tail, activePatternResult616[1].head, activePatternResult613[1].head];
                      } else {
                        $var112 = [5];
                      }
                    } else {
                      $var112 = [5];
                    }
                  } else {
                    $var112 = [5];
                  }
                } else {
                  $var112 = [5];
                }
              } else if (activePatternResult614 === "scanf") {
                if (activePatternResult613[1].tail.tail != null) {
                  var activePatternResult617 = Token["|X|"](activePatternResult613[1].tail.head);

                  if (activePatternResult617[0] === ",") {
                    if (activePatternResult617[1].tail != null) {
                      if (activePatternResult613[1].tail.tail.tail == null) {
                        $var112 = [4, activePatternResult617[1].tail, activePatternResult617[1].head, activePatternResult613[1].head];
                      } else {
                        $var112 = [5];
                      }
                    } else {
                      $var112 = [5];
                    }
                  } else {
                    $var112 = [5];
                  }
                } else {
                  $var112 = [5];
                }
              } else {
                $var112 = [5];
              }
            } else {
              $var112 = [5];
            }
          } else {
            $var112 = [5];
          }
        } else {
          $var112 = [5];
        }

        switch ($var112[0]) {
          case 0:
            return Token.Token[".ctor_2"]("let", (0, _List.ofArray)([$var112[3], Token.Token[".ctor_2"]("fun", (0, _List.ofArray)([postProcess($var112[1]), postProcess($var112[4])]))]));

          case 1:
            _arg1 = Token.Token[".ctor_2"]("sequence", $var112[1]);
            continue postProcess;

          case 2:
            var xprs_ = (0, _List.filter)(function (_arg2) {
              var $var113 = void 0;
              var activePatternResult606 = Token["|T|_|"](_arg2);

              if (activePatternResult606 != null) {
                if (activePatternResult606 === ";") {
                  $var113 = [0];
                } else {
                  $var113 = [1];
                }
              } else {
                $var113 = [1];
              }

              switch ($var113[0]) {
                case 0:
                  return false;

                case 1:
                  return true;
              }
            }, $var112[1]);
            return Token.Token[".ctor_2"]("sequence", (0, _List.map)(function (_arg1_1) {
              return postProcess(_arg1_1);
            }, xprs_));

          case 3:
            return Token.Token[".ctor_2"]("=", (0, _List.map)(function (_arg1_2) {
              return postProcess(_arg1_2);
            }, $var112[1]));

          case 4:
            (0, _String.fsFormat)("%A")(function (x) {
              console.log(x);
            })($var112[1]);
            return function (e) {
              (0, _String.fsFormat)("AST: %s")(function (x) {
                console.log(x);
              })(e.ToStringExpr());
              return e;
            }((0, _Seq.fold)(function (acc, e_1) {
              (0, _String.fsFormat)("AST: %s")(function (x) {
                console.log(x);
              })(e_1.ToStringExpr());
              return Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([acc, e_1]));
            }, Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([$var112[3], $var112[2]])), $var112[1]));

          case 5:
            var activePatternResult612 = Token["|X|"](_arg1);
            return Token.Token[".ctor_2"](activePatternResult612[0], (0, _List.map)(function (_arg1_3) {
              return postProcess(_arg1_3);
            }, activePatternResult612[1]));
        }
      }
    };

    var parseSyntax = __exports.parseSyntax = function (e) {
      restoreDefault();
      return function (e_1) {
        (0, _String.fsFormat)("AST: %s")(function (x) {
          console.log(x);
        })(e_1.ToStringExpr());
        return e_1;
      }(String_Formatting.processStringFormatting(function (_arg2) {
        var activePatternResult623 = Token["|X|"](_arg2);

        if (activePatternResult623[0] === "sequence") {
          return Token.Token[".ctor_2"]("sequence", (0, _List.append)(activePatternResult623[1], (0, _List.ofArray)([Token.Token[".ctor_2"]("apply", (0, _List.ofArray)([Token.Token[".ctor_3"]("main"), Token.Token[".ctor_3"]("()")]))])));
        } else {
          throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 671, 8);
        }
      }(function (e_2) {
        (0, _String.fsFormat)("AST: %s")(function (x) {
          console.log(x);
        })(e_2.ToStringExpr());
        return e_2;
      }(postProcess(function (e_3) {
        (0, _String.fsFormat)("AST: %s")(function (x) {
          console.log(x);
        })(e_3.ToStringExpr());
        return e_3;
      }(parse(new State("Global", []), function (_arg1) {
        return _arg1.tail == null ? true : false;
      }, function (_arg1_1) {
        return false;
      }, new _List2.default(), preprocess(e))[0]))))));
    };

    return __exports;
  }({});

  var noObject = exports.noObject = new Map(new _List2.default());
  var objectTypes = exports.objectTypes = new Map();

  function compileObjectType(_arg1) {
    var $var114 = void 0;
    var activePatternResult643 = Token["|X|"](_arg1);

    if (activePatternResult643[0] === "struct") {
      if (activePatternResult643[1].tail != null) {
        var activePatternResult644 = Token["|T|_|"](activePatternResult643[1].head);

        if (activePatternResult644 != null) {
          if (activePatternResult643[1].tail.tail != null) {
            if (activePatternResult643[1].tail.tail.tail == null) {
              $var114 = [0, activePatternResult643[1].tail.head, activePatternResult644];
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
        (0, _String.fsFormat)("%A")(function (x) {
          console.log(x);
        })($var114[1]);
        var activePatternResult641 = Token["|X|"]($var114[1]);

        if (activePatternResult641[0] === "sequence") {
          var mapMembers = new Map((0, _List.mapIndexed)(function (i, e) {
            var $var115 = void 0;
            var activePatternResult633 = Token["|X|"](e);

            if (activePatternResult633[0] === "let") {
              if (activePatternResult633[1].tail != null) {
                var activePatternResult634 = Token["|T|_|"](activePatternResult633[1].head);

                if (activePatternResult634 != null) {
                  if (activePatternResult633[1].tail.tail != null) {
                    var activePatternResult635 = Token["|T|_|"](activePatternResult633[1].tail.head);

                    if (activePatternResult635 != null) {
                      if (activePatternResult635 === "nothing") {
                        if (activePatternResult633[1].tail.tail.tail == null) {
                          $var115 = [0, activePatternResult634];
                        } else {
                          $var115 = [1];
                        }
                      } else {
                        $var115 = [1];
                      }
                    } else {
                      $var115 = [1];
                    }
                  } else {
                    $var115 = [1];
                  }
                } else {
                  var activePatternResult636 = Token["|X|"](activePatternResult633[1].head);

                  if (activePatternResult636[0] === "declare") {
                    if (activePatternResult636[1].tail != null) {
                      if (activePatternResult636[1].tail.tail != null) {
                        var activePatternResult637 = Token["|T|_|"](activePatternResult636[1].tail.head);

                        if (activePatternResult637 != null) {
                          if (activePatternResult636[1].tail.tail.tail == null) {
                            if (activePatternResult633[1].tail.tail != null) {
                              var activePatternResult638 = Token["|T|_|"](activePatternResult633[1].tail.head);

                              if (activePatternResult638 != null) {
                                if (activePatternResult638 === "nothing") {
                                  if (activePatternResult633[1].tail.tail.tail == null) {
                                    $var115 = [0, activePatternResult637];
                                  } else {
                                    $var115 = [1];
                                  }
                                } else {
                                  $var115 = [1];
                                }
                              } else {
                                $var115 = [1];
                              }
                            } else {
                              $var115 = [1];
                            }
                          } else {
                            $var115 = [1];
                          }
                        } else {
                          $var115 = [1];
                        }
                      } else {
                        $var115 = [1];
                      }
                    } else {
                      $var115 = [1];
                    }
                  } else {
                    $var115 = [1];
                  }
                }
              } else {
                $var115 = [1];
              }
            } else {
              $var115 = [1];
            }

            switch ($var115[0]) {
              case 0:
                return [$var115[1], [i, null]];

              case 1:
                var $var116 = void 0;
                var activePatternResult629 = Token["|X|"](e);

                if (activePatternResult629[0] === "let") {
                  if (activePatternResult629[1].tail != null) {
                    var activePatternResult630 = Token["|T|_|"](activePatternResult629[1].head);

                    if (activePatternResult630 != null) {
                      if (activePatternResult629[1].tail.tail != null) {
                        if (activePatternResult629[1].tail.tail.tail == null) {
                          $var116 = [0, activePatternResult629[1].tail.head, activePatternResult630];
                        } else {
                          $var116 = [1];
                        }
                      } else {
                        $var116 = [1];
                      }
                    } else {
                      var activePatternResult631 = Token["|X|"](activePatternResult629[1].head);

                      if (activePatternResult631[0] === "declare") {
                        if (activePatternResult631[1].tail != null) {
                          if (activePatternResult631[1].tail.tail != null) {
                            var activePatternResult632 = Token["|T|_|"](activePatternResult631[1].tail.head);

                            if (activePatternResult632 != null) {
                              if (activePatternResult631[1].tail.tail.tail == null) {
                                if (activePatternResult629[1].tail.tail != null) {
                                  if (activePatternResult629[1].tail.tail.tail == null) {
                                    $var116 = [0, activePatternResult629[1].tail.head, activePatternResult632];
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
                        } else {
                          $var116 = [1];
                        }
                      } else {
                        $var116 = [1];
                      }
                    }
                  } else {
                    $var116 = [1];
                  }
                } else {
                  $var116 = [1];
                }

                switch ($var116[0]) {
                  case 0:
                    return [$var116[2], [i, $var116[1]]];

                  case 1:
                    throw new Error("wrong member format");
                }

            }
          }, activePatternResult641[1]));
          objectTypes.set($var114[2], mapMembers);
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
    var $var117 = void 0;
    var activePatternResult664 = Token["|X|"](_arg1);

    if (activePatternResult664[0] === "struct") {
      $var117 = [0, _arg1];
    } else if (activePatternResult664[0] === "dot") {
      if (activePatternResult664[1].tail != null) {
        var activePatternResult665 = Token["|T|_|"](activePatternResult664[1].head);

        if (activePatternResult665 != null) {
          if (activePatternResult664[1].tail.tail != null) {
            var activePatternResult666 = Token["|T|_|"](activePatternResult664[1].tail.head);

            if (activePatternResult666 != null) {
              if (activePatternResult664[1].tail.tail.tail == null) {
                $var117 = [1, activePatternResult665, activePatternResult666];
              } else {
                $var117 = [2];
              }
            } else {
              $var117 = [2];
            }
          } else {
            $var117 = [2];
          }
        } else {
          $var117 = [2];
        }
      } else {
        $var117 = [2];
      }
    } else {
      $var117 = [2];
    }

    switch ($var117[0]) {
      case 0:
        compileObjectType($var117[1]);
        return Token.Token[".ctor_3"]("()");

      case 1:
        return Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([Token.Token[".ctor_3"]($var117[1]), Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([Token.Token[".ctor_3"](String(varTypes.get($var117[1]).head.get($var117[2])[0]))]))]));

      case 2:
        var activePatternResult663 = Token["|X|"](_arg1);

        if (activePatternResult663[0] === "sequence") {
          var yld = function (e) {
            return Token.Token[".ctor_2"]("sequence", e);
          }((0, _List.collect)(function (_arg2) {
            var $var118 = void 0;
            var activePatternResult650 = Token["|X|"](_arg2);

            if (activePatternResult650[0] === "let") {
              if (activePatternResult650[1].tail != null) {
                var activePatternResult651 = Token["|X|"](activePatternResult650[1].head);

                if (activePatternResult651[0] === "declare") {
                  if (activePatternResult651[1].tail != null) {
                    var activePatternResult652 = Token["|T|_|"](activePatternResult651[1].head);

                    if (activePatternResult652 != null) {
                      if (activePatternResult651[1].tail.tail != null) {
                        var activePatternResult653 = Token["|T|_|"](activePatternResult651[1].tail.head);

                        if (activePatternResult653 != null) {
                          if (activePatternResult651[1].tail.tail.tail == null) {
                            if (activePatternResult650[1].tail.tail != null) {
                              if (activePatternResult650[1].tail.tail.tail == null) {
                                $var118 = [0, activePatternResult653, activePatternResult650[1].tail.head, _arg2, activePatternResult652];
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
                var mapping = objectTypes.has($var118[4]) ? objectTypes.get($var118[4]) : noObject;

                if (!varTypes.has($var118[1])) {
                  varTypes.set($var118[1], new _List2.default());
                }

                varTypes.set($var118[1], new _List2.default(mapping, varTypes.get($var118[1])));
                var matchValue = [(0, _Seq.count)(mapping), $var118[2]];
                var $var119 = void 0;
                var activePatternResult647 = Token["|T|_|"](matchValue[1]);

                if (activePatternResult647 != null) {
                  if (activePatternResult647 === "nothing") {
                    if (matchValue[0] > 0) {
                      $var119 = [0, matchValue[0]];
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
                    var allocate = Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_2"]("declare", (0, _List.ofArray)([Token.Token[".ctor_3"]($var118[4]), Token.Token[".ctor_3"]($var118[1])])), Token.Token[".ctor_2"]("array", (0, _List.ofArray)([Token.Token[".ctor_3"](String($var119[1]))]))]));
                    var initializeMembers = (0, _Seq.toList)((0, _Seq.collect)(function (_arg3) {
                      if (_arg3[1] == null) {
                        return new _List2.default();
                      } else {
                        var v = _arg3[1];
                        return (0, _List.ofArray)([Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([Token.Token[".ctor_3"]($var118[1]), Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([Token.Token[".ctor_3"](String(_arg3[0]))]))])), v]))]);
                      }
                    }, mapping.values()));
                    return new _List2.default(allocate, initializeMembers);

                  case 1:
                    return (0, _List.ofArray)([compileObjects($var118[3])]);
                }

              case 1:
                return (0, _List.ofArray)([compileObjects(_arg2)]);
            }
          }, activePatternResult663[1]));

          (0, _Seq.iterate)(function (_arg4) {
            var $var120 = void 0;
            var activePatternResult658 = Token["|X|"](_arg4);

            if (activePatternResult658[0] === "let") {
              if (activePatternResult658[1].tail != null) {
                var activePatternResult659 = Token["|X|"](activePatternResult658[1].head);

                if (activePatternResult659[0] === "declare") {
                  if (activePatternResult659[1].tail != null) {
                    if (activePatternResult659[1].tail.tail != null) {
                      var activePatternResult660 = Token["|T|_|"](activePatternResult659[1].tail.head);

                      if (activePatternResult660 != null) {
                        if (activePatternResult659[1].tail.tail.tail == null) {
                          if (activePatternResult658[1].tail.tail != null) {
                            if (activePatternResult658[1].tail.tail.tail == null) {
                              $var120 = [0, activePatternResult660, activePatternResult658[1].tail.head];
                            } else {
                              $var120 = [1];
                            }
                          } else {
                            $var120 = [1];
                          }
                        } else {
                          $var120 = [1];
                        }
                      } else {
                        $var120 = [1];
                      }
                    } else {
                      $var120 = [1];
                    }
                  } else {
                    $var120 = [1];
                  }
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
                varTypes.set($var120[1], varTypes.get($var120[1]).tail);
                break;

              case 1:
                break;
            }
          }, activePatternResult663[1]);
          return yld;
        } else {
          return Token.Token[".ctor_2"](activePatternResult663[0], (0, _List.map)(function (_arg1_1) {
            return compileObjects(_arg1_1);
          }, activePatternResult663[1]));
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
          type: "Xlvm.LabelledToken",
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

  (0, _Symbol2.setType)("Xlvm.LabelledToken", LabelledToken);

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
        var $var121 = void 0;
        var activePatternResult697 = Token["|T|_|"](_arg1);

        if (activePatternResult697 != null) {
          if (labels.has(activePatternResult697)) {
            $var121 = [0, activePatternResult697];
          } else {
            $var121 = [1];
          }
        } else {
          $var121 = [1];
        }

        switch ($var121[0]) {
          case 0:
            return new LabelledToken("LT", [$var121[1], new _List2.default(), labels.get($var121[1])]);

          case 1:
            var activePatternResult696 = Token["|X|"](_arg1);

            if (activePatternResult696[0] === "sequence") {
              var nodes_ = (0, _List.reverse)((0, _Seq.fold)(function (tupledArg, _arg2) {
                var $var122 = void 0;
                var activePatternResult689 = Token["|X|"](_arg2);

                if (activePatternResult689[0] === "let") {
                  if (activePatternResult689[1].tail != null) {
                    var activePatternResult690 = Token["|T|_|"](activePatternResult689[1].head);

                    if (activePatternResult690 != null) {
                      if (activePatternResult689[1].tail.tail != null) {
                        if (activePatternResult689[1].tail.tail.tail == null) {
                          $var122 = [0, activePatternResult689[1].head, activePatternResult689[1].tail.head, activePatternResult690];
                        } else {
                          $var122 = [1];
                        }
                      } else {
                        $var122 = [1];
                      }
                    } else {
                      var activePatternResult691 = Token["|X|"](activePatternResult689[1].head);

                      if (activePatternResult691[0] === "declare") {
                        if (activePatternResult691[1].tail != null) {
                          if (activePatternResult691[1].tail.tail != null) {
                            var activePatternResult692 = Token["|T|_|"](activePatternResult691[1].tail.head);

                            if (activePatternResult692 != null) {
                              if (activePatternResult691[1].tail.tail.tail == null) {
                                if (activePatternResult689[1].tail.tail != null) {
                                  if (activePatternResult689[1].tail.tail.tail == null) {
                                    $var122 = [0, activePatternResult689[1].head, activePatternResult689[1].tail.head, activePatternResult692];
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
                          } else {
                            $var122 = [1];
                          }
                        } else {
                          $var122 = [1];
                        }
                      } else {
                        $var122 = [1];
                      }
                    }
                  } else {
                    $var122 = [1];
                  }
                } else {
                  $var122 = [1];
                }

                switch ($var122[0]) {
                  case 0:
                    var x_1 = nxt(null);
                    var newLabels = (0, _Map.add)($var122[3], x_1, tupledArg[0]);
                    var newNode = new LabelledToken("LT", ["let", (0, _List.ofArray)([labelTokens(newLabels)($var122[1]), labelTokens(newLabels)($var122[2])]), -1]);
                    return [newLabels, new _List2.default(newNode, tupledArg[1])];

                  case 1:
                    return [tupledArg[0], new _List2.default(labelTokens(tupledArg[0])(_arg2), tupledArg[1])];
                }
              }, [labels, new _List2.default()], activePatternResult696[1])[1]);
              return new LabelledToken("LT", ["sequence", nodes_, -1]);
            } else {
              return new LabelledToken("LT", [activePatternResult696[0], (0, _List.map)(labelTokens(labels), activePatternResult696[1]), -1]);
            }

        }
      };
    };

    var k = labelTokens((0, _Map.create)(null, new _GenericComparer2.default(_Util.compare)))(x);
    var hasReference = Array.from((0, _Seq.replicate)(nxt(null), false));

    var findAllDerefs = function findAllDerefs(_arg3) {
      var $var123 = _arg3.Fields[0] === "apply" ? _arg3.Fields[1].tail != null ? _arg3.Fields[1].head.Fields[0] === "~&" ? _arg3.Fields[1].head.Fields[1].tail == null ? _arg3.Fields[1].tail.tail != null ? _arg3.Fields[1].tail.head.Fields[1].tail == null ? _arg3.Fields[1].tail.tail.tail == null ? function () {
        var s = _arg3.Fields[1].tail.head.Fields[0];
        var a = _arg3.Fields[1].tail.head.Fields[2];
        return a !== -1;
      }() ? [0, _arg3.Fields[1].tail.head.Fields[2], _arg3.Fields[1].tail.head.Fields[0]] : [1] : [1] : [1] : [1] : [1] : [1] : [1] : [1];

      switch ($var123[0]) {
        case 0:
          hasReference[$var123[1]] = true;
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
      var activePatternResult706 = _IsDeref___(_arg5);

      if (activePatternResult706 != null) {
        return Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([Token.Token[".ctor_3"](activePatternResult706), Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([Token.Token[".ctor_3"]("0")]))]));
      } else {
        var $var124 = void 0;

        if (_arg5.Fields[0] === "apply") {
          if (_arg5.Fields[1].tail != null) {
            if (_arg5.Fields[1].head.Fields[0] === "~&") {
              if (_arg5.Fields[1].head.Fields[1].tail == null) {
                if (_arg5.Fields[1].tail.tail != null) {
                  var activePatternResult705 = _IsDeref___(_arg5.Fields[1].tail.head);

                  if (activePatternResult705 != null) {
                    if (_arg5.Fields[1].tail.tail.tail == null) {
                      $var124 = [0, activePatternResult705];
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
        } else {
          $var124 = [1];
        }

        switch ($var124[0]) {
          case 0:
            return Token.Token[".ctor_3"]($var124[1]);

          case 1:
            if (_arg5.Fields[0] === "sequence") {
              var nodes__1 = (0, _List.collect)(function (_arg6) {
                var $var125 = void 0;

                if (_arg6.Fields[0] === "let") {
                  if (_arg6.Fields[1].tail != null) {
                    var activePatternResult702 = _IsDeref___(_arg6.Fields[1].head);

                    if (activePatternResult702 != null) {
                      if (_arg6.Fields[1].tail.tail != null) {
                        if (_arg6.Fields[1].tail.tail.tail == null) {
                          $var125 = [0, activePatternResult702, _arg6.Fields[1].tail.head];
                        } else {
                          $var125 = [1];
                        }
                      } else {
                        $var125 = [1];
                      }
                    } else if (_arg6.Fields[1].head.Fields[0] === "declare") {
                      if (_arg6.Fields[1].head.Fields[1].tail != null) {
                        if (_arg6.Fields[1].head.Fields[1].tail.tail != null) {
                          var activePatternResult703 = _IsDeref___(_arg6.Fields[1].head.Fields[1].tail.head);

                          if (activePatternResult703 != null) {
                            if (_arg6.Fields[1].head.Fields[1].tail.tail.tail == null) {
                              if (_arg6.Fields[1].tail.tail != null) {
                                if (_arg6.Fields[1].tail.tail.tail == null) {
                                  $var125 = [0, activePatternResult703, _arg6.Fields[1].tail.head];
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
                    return (0, _List.ofArray)([Token.Token[".ctor_2"]("let", (0, _List.ofArray)([Token.Token[".ctor_3"]($var125[1]), Token.Token[".ctor_2"]("array", (0, _List.ofArray)([Token.Token[".ctor_3"]("1")]))])), Token.Token[".ctor_2"]("assign", (0, _List.ofArray)([Token.Token[".ctor_2"]("dot", (0, _List.ofArray)([Token.Token[".ctor_3"]($var125[1]), Token.Token[".ctor_2"]("[]", (0, _List.ofArray)([Token.Token[".ctor_3"]("0")]))])), mapDerefs($var125[2])]))]);

                  case 1:
                    return (0, _List.ofArray)([mapDerefs(_arg6)]);
                }
              }, _arg5.Fields[1]);
              return Token.Token[".ctor_2"]("sequence", nodes__1);
            } else {
              return Token.Token[".ctor_2"](_arg5.Fields[0], (0, _List.map)(mapDerefs, _arg5.Fields[1]));
            }

        }
      }
    };

    var yld = mapDerefs(k);
    return yld;
  }

  function processDerefs(_arg1) {
    var $var126 = void 0;
    var activePatternResult711 = Token["|X|"](_arg1);

    if (activePatternResult711[0] === "apply") {
      if (activePatternResult711[1].tail != null) {
        var activePatternResult712 = Token["|T|_|"](activePatternResult711[1].head);

        if (activePatternResult712 != null) {
          if (activePatternResult712 === "~*") {
            if (activePatternResult711[1].tail.tail != null) {
              if (activePatternResult711[1].tail.tail.tail == null) {
                $var126 = [0, activePatternResult711[1].tail.head];
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
      } else {
        $var126 = [1];
      }
    } else {
      $var126 = [1];
    }

    switch ($var126[0]) {
      case 0:
        return Token.Token[".ctor_2"]("deref", (0, _List.ofArray)([$var126[1]]));

      case 1:
        var activePatternResult710 = Token["|X|"](_arg1);
        return Token.Token[".ctor_2"](activePatternResult710[0], (0, _List.map)(function (_arg1_1) {
          return processDerefs(_arg1_1);
        }, activePatternResult710[1]));
    }
  }

  var applyTypeSystem = exports.applyTypeSystem = function applyTypeSystem($var128) {
    return function (_arg1) {
      return processDerefs(_arg1);
    }(function ($var127) {
      return compilePointersToArrays(compileObjectsToArrays($var127));
    }($var128));
  };

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
          type: "Xlvm.AST",
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
            var $var129 = _arg1.Case === "Declare" ? [1, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "Define" ? [2, _arg1.Fields[0], _arg1.Fields[1], _arg1.Fields[2]] : _arg1.Case === "Value" ? [3, _arg1.Fields[0]] : _arg1.Case === "Const" ? [3, _arg1.Fields[0]] : _arg1.Case === "Apply" ? [4, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "If" ? [5, _arg1.Fields[0], _arg1.Fields[1], _arg1.Fields[2]] : _arg1.Case === "New" ? [6, _arg1.Fields[0]] : _arg1.Case === "Get" ? [7, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "Assign" ? [8, _arg1.Fields[0], _arg1.Fields[2], _arg1.Fields[1]] : _arg1.Case === "Return" ? _arg1.Fields[0] != null ? [10, _arg1.Fields[0]] : [9] : _arg1.Case === "Loop" ? [11, _arg1.Fields[0], _arg1.Fields[1]] : _arg1.Case === "Mutate" ? [12, _arg1.Fields[0], _arg1.Fields[1]] : [0, _arg1.Fields[0]];

            switch ($var129[0]) {
              case 0:
                return (0, _String.join)("\n", (0, _List.map)(str(indent), $var129[1]));

              case 1:
                return indent + (0, _String.fsFormat)("let %s =\n%s")(function (x) {
                  return x;
                })($var129[1])(str(indent + "  ")($var129[2]));

              case 2:
                return indent + (0, _String.fsFormat)("define %s(%s):\n%s")(function (x) {
                  return x;
                })($var129[1])((0, _String.join)(", ", $var129[2]))(str(indent + "  ")($var129[3]));

              case 3:
                return indent + $var129[1];

              case 4:
                return indent + (0, _String.fsFormat)("%s(%s)")(function (x) {
                  return x;
                })(str("")($var129[1]))((0, _String.join)(", ", (0, _List.map)(str(""), $var129[2])));

              case 5:
                return indent + (0, _String.fsFormat)("if %s then\n%s\n")(function (x) {
                  return x;
                })(str("")($var129[1]))(str(indent + "  ")($var129[2])) + indent + "else\n" + str(indent + "  ")($var129[3]);

              case 6:
                return indent + (0, _String.fsFormat)("alloc %s")(function (x) {
                  return x;
                })(str("")($var129[1]));

              case 7:
                return indent + (0, _String.fsFormat)("%s[%s]")(function (x) {
                  return x;
                })(str("")($var129[1]))(str("")($var129[2]));

              case 8:
                return indent + (0, _String.fsFormat)("%s[%s] <- %s")(function (x) {
                  return x;
                })(str("")($var129[1]))(str("")($var129[3]))(str("")($var129[2]));

              case 9:
                return indent + "return";

              case 10:
                return indent + (0, _String.fsFormat)("return\n%s")(function (x) {
                  return x;
                })(str(indent + "  ")($var129[1]));

              case 11:
                return indent + (0, _String.fsFormat)("while %s do\n%s\n")(function (x) {
                  return x;
                })(str("")($var129[1]))(str(indent + "  ")($var129[2]));

              case 12:
                return indent + (0, _String.fsFormat)("%s <-\n%s")(function (x) {
                  return x;
                })($var129[1])(str(indent + "  ")($var129[2]));
            }
          };
        };

        return str("")(this);
      }
    }]);

    return AST;
  }();

  (0, _Symbol2.setType)("Xlvm.AST", AST);

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
      var $var130 = void 0;
      var activePatternResult775 = Token["|X|"](_arg2);

      if (activePatternResult775[0] === "return") {
        $var130 = [0, activePatternResult775[1]];
      } else if (activePatternResult775[0] === ",") {
        var activePatternResult776 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult776.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult776.Fields[0]];
        } else if (activePatternResult776.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult776.Fields[0]];
        } else {
          $var130 = [3, activePatternResult775[1]];
        }
      } else if (activePatternResult775[0] === "apply") {
        var activePatternResult777 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult777.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult777.Fields[0]];
        } else if (activePatternResult777.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult777.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [4, activePatternResult775[1].head, activePatternResult775[1].tail.head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "fun") {
        var activePatternResult778 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult778.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult778.Fields[0]];
        } else if (activePatternResult778.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult778.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [5, activePatternResult775[1].tail.head, activePatternResult775[1].head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "declare") {
        var activePatternResult779 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult779.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult779.Fields[0]];
        } else if (activePatternResult779.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult779.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [6, activePatternResult775[1].tail.head, activePatternResult775[1].head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "let") {
        var activePatternResult780 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult780.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult780.Fields[0]];
        } else if (activePatternResult780.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult780.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [7, activePatternResult775[1].head, activePatternResult775[1].tail.head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "let rec") {
        var activePatternResult781 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult781.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult781.Fields[0]];
        } else if (activePatternResult781.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult781.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [8, activePatternResult775[1].head, activePatternResult775[1].tail.head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "array") {
        var activePatternResult782 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult782.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult782.Fields[0]];
        } else if (activePatternResult782.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult782.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail == null) {
            $var130 = [9, activePatternResult775[1].head];
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "dot") {
        var activePatternResult783 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult783.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult783.Fields[0]];
        } else if (activePatternResult783.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult783.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [10, activePatternResult775[1].head, activePatternResult775[1].tail.head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "assign") {
        var activePatternResult784 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult784.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult784.Fields[0]];
        } else if (activePatternResult784.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult784.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [11, activePatternResult775[1].head, activePatternResult775[1].tail.head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "if") {
        var activePatternResult785 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult785.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult785.Fields[0]];
        } else if (activePatternResult785.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult785.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail != null) {
              if (activePatternResult775[1].tail.tail.tail.tail == null) {
                $var130 = [12, activePatternResult775[1].tail.head, activePatternResult775[1].head, activePatternResult775[1].tail.tail.head];
              } else {
                $var130 = [18, _arg2];
              }
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "do") {
        var activePatternResult786 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult786.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult786.Fields[0]];
        } else if (activePatternResult786.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult786.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail == null) {
            $var130 = [13, activePatternResult775[1].head];
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "while") {
        var activePatternResult787 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult787.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult787.Fields[0]];
        } else if (activePatternResult787.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult787.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail == null) {
              $var130 = [14, activePatternResult775[1].tail.head, activePatternResult775[1].head];
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "for") {
        var activePatternResult788 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult788.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult788.Fields[0]];
        } else if (activePatternResult788.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult788.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail != null) {
            if (activePatternResult775[1].tail.tail.tail != null) {
              if (activePatternResult775[1].tail.tail.tail.tail == null) {
                $var130 = [15, activePatternResult775[1].tail.tail.head, activePatternResult775[1].tail.head, activePatternResult775[1].head];
              } else {
                $var130 = [18, _arg2];
              }
            } else {
              $var130 = [18, _arg2];
            }
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "deref") {
        var activePatternResult789 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult789.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult789.Fields[0]];
        } else if (activePatternResult789.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult789.Fields[0]];
        } else if (activePatternResult775[1].tail != null) {
          if (activePatternResult775[1].tail.tail == null) {
            $var130 = [16, activePatternResult775[1].head];
          } else {
            $var130 = [18, _arg2];
          }
        } else {
          $var130 = [18, _arg2];
        }
      } else if (activePatternResult775[0] === "sequence") {
        var activePatternResult790 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult790.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult790.Fields[0]];
        } else if (activePatternResult790.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult790.Fields[0]];
        } else {
          $var130 = [17, activePatternResult775[1]];
        }
      } else {
        var activePatternResult791 = Token["|Var|Cnst|Other|"](_arg2);

        if (activePatternResult791.Case === "Choice1Of3") {
          $var130 = [1, activePatternResult791.Fields[0]];
        } else if (activePatternResult791.Case === "Choice2Of3") {
          $var130 = [2, activePatternResult791.Fields[0]];
        } else {
          $var130 = [18, _arg2];
        }
      }

      switch ($var130[0]) {
        case 0:
          if ($var130[1].tail != null) {
            if ($var130[1].tail.tail == null) {
              return new AST("Return", [function (tupledArg) {
                return ASTCompile_(tupledArg[0], tupledArg[1]);
              }(_arg1)($var130[1].head)]);
            } else {
              throw new Error("cannot return more than one item");
            }
          } else {
            return new AST("Return", [null]);
          }

        case 1:
          if (_arg1[1].has($var130[1]) ? !_arg1[1].get($var130[1]).Equals(new _List2.default()) : false) {
            return new AST("Apply", [new AST("Value", [$var130[1]]), _arg1[1].get($var130[1])]);
          } else {
            return new AST("Value", [$var130[1]]);
          }

        case 2:
          return new AST("Const", [$var130[1]]);

        case 3:
          var name = "$tuple" + nxt(null);
          var allocate = (0, _List.ofArray)([new AST("Declare", [name, new AST("New", [new AST("Const", [String($var130[1].length)])])])]);
          var assignAll = (0, _List.mapIndexed)(function (i, e) {
            return new AST("Assign", [new AST("Value", [name]), new AST("Const", [String(i)]), function (tupledArg_1) {
              return ASTCompile_(tupledArg_1[0], tupledArg_1[1]);
            }(_arg1)(e)]);
          }, $var130[1]);
          var returnVal = (0, _List.ofArray)([new AST("Value", [name])]);
          return new AST("Sequence", [(0, _List.append)(allocate, (0, _List.append)(assignAll, returnVal))]);

        case 4:
          return new AST("Apply", [function (tupledArg_2) {
            return ASTCompile_(tupledArg_2[0], tupledArg_2[1]);
          }(_arg1)($var130[1]), (0, _List.ofArray)([function (tupledArg_3) {
            return ASTCompile_(tupledArg_3[0], tupledArg_3[1]);
          }(_arg1)($var130[2])])]);

        case 5:
          var unpack = function unpack(arg) {
            return function (_arg3) {
              var $var131 = void 0;
              var activePatternResult741 = Token["|X|"](_arg3);

              if (activePatternResult741[0] === "declare") {
                if (activePatternResult741[1].tail != null) {
                  if (activePatternResult741[1].tail.tail != null) {
                    var activePatternResult742 = Token["|T|_|"](activePatternResult741[1].tail.head);

                    if (activePatternResult742 != null) {
                      if (activePatternResult741[1].tail.tail.tail == null) {
                        $var131 = [0, activePatternResult742];
                      } else {
                        var activePatternResult743 = Token["|T|_|"](_arg3);

                        if (activePatternResult743 != null) {
                          $var131 = [0, activePatternResult743];
                        } else {
                          $var131 = [1];
                        }
                      }
                    } else {
                      var activePatternResult744 = Token["|T|_|"](_arg3);

                      if (activePatternResult744 != null) {
                        $var131 = [0, activePatternResult744];
                      } else {
                        $var131 = [1];
                      }
                    }
                  } else {
                    var activePatternResult745 = Token["|T|_|"](_arg3);

                    if (activePatternResult745 != null) {
                      $var131 = [0, activePatternResult745];
                    } else {
                      $var131 = [1];
                    }
                  }
                } else {
                  var activePatternResult746 = Token["|T|_|"](_arg3);

                  if (activePatternResult746 != null) {
                    $var131 = [0, activePatternResult746];
                  } else {
                    $var131 = [1];
                  }
                }
              } else {
                var activePatternResult747 = Token["|T|_|"](_arg3);

                if (activePatternResult747 != null) {
                  $var131 = [0, activePatternResult747];
                } else {
                  $var131 = [1];
                }
              }

              switch ($var131[0]) {
                case 0:
                  return [(0, _List.ofArray)([new AST("Declare", [$var131[1], arg])]), (0, _List.ofArray)([$var131[1]])];

                case 1:
                  var activePatternResult740 = Token["|X|"](_arg3);

                  if (activePatternResult740[0] === ",") {
                    var patternInput = (0, _List.unzip)((0, _List.mapIndexed)(function ($var132, $var133) {
                      return function (i_1) {
                        return unpack(new AST("Get", [arg, new AST("Const", [String(i_1)])]));
                      }($var132)($var133);
                    }, activePatternResult740[1]));
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
          var patternInput_1 = unpack(new AST("Value", [argName]))($var130[2]);
          var cptr_d = (0, _List.map)(function (arg0) {
            return new AST("Value", [arg0]);
          }, _arg1[0]);
          var cpt_ = [(0, _List.append)(patternInput_1[1], _arg1[0]), (0, _Map.add)("L", cptr_d, _arg1[1])];
          var functionBody = new AST("Sequence", [(0, _List.append)(patternInput_1[0], (0, _List.ofArray)([function (tupledArg_4) {
            return ASTCompile_(tupledArg_4[0], tupledArg_4[1]);
          }(cpt_)($var130[1])]))]);
          return new AST("Sequence", [(0, _Seq.toList)((0, _Seq.delay)(function () {
            return (0, _Seq.append)((0, _Seq.singleton)(new AST("Define", ["L", new _List2.default(argName, _arg1[0]), functionBody])), (0, _Seq.delay)(function () {
              var matchValue = function (tupledArg_5) {
                return ASTCompile_(tupledArg_5[0], tupledArg_5[1]);
              }(cpt_)(Token.Token[".ctor_2"]("L", new _List2.default()));

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
          }(_arg1)($var130[1]);

        case 7:
          var $var134 = void 0;
          var activePatternResult759 = Token["|X|"]($var130[1]);

          if (activePatternResult759[0] === "apply") {
            if (activePatternResult759[1].tail != null) {
              if (activePatternResult759[1].tail.tail != null) {
                if (activePatternResult759[1].tail.tail.tail == null) {
                  $var134 = [0, activePatternResult759[1].head, activePatternResult759[1].tail.head];
                } else {
                  var activePatternResult760 = Token["|T|_|"]($var130[1]);

                  if (activePatternResult760 != null) {
                    $var134 = [1, activePatternResult760];
                  } else {
                    $var134 = [2];
                  }
                }
              } else {
                var activePatternResult761 = Token["|T|_|"]($var130[1]);

                if (activePatternResult761 != null) {
                  $var134 = [1, activePatternResult761];
                } else {
                  $var134 = [2];
                }
              }
            } else {
              var activePatternResult762 = Token["|T|_|"]($var130[1]);

              if (activePatternResult762 != null) {
                $var134 = [1, activePatternResult762];
              } else {
                $var134 = [2];
              }
            }
          } else if (activePatternResult759[0] === "declare") {
            var activePatternResult763 = Token["|T|_|"]($var130[1]);

            if (activePatternResult763 != null) {
              $var134 = [1, activePatternResult763];
            } else if (activePatternResult759[1].tail != null) {
              if (activePatternResult759[1].tail.tail != null) {
                var activePatternResult764 = Token["|T|_|"](activePatternResult759[1].tail.head);

                if (activePatternResult764 != null) {
                  if (activePatternResult759[1].tail.tail.tail == null) {
                    $var134 = [1, activePatternResult764];
                  } else {
                    $var134 = [2];
                  }
                } else {
                  $var134 = [2];
                }
              } else {
                $var134 = [2];
              }
            } else {
              $var134 = [2];
            }
          } else {
            var activePatternResult765 = Token["|T|_|"]($var130[1]);

            if (activePatternResult765 != null) {
              $var134 = [1, activePatternResult765];
            } else {
              $var134 = [2];
            }
          }

          switch ($var134[0]) {
            case 0:
              return function (tupledArg_7) {
                return ASTCompile_(tupledArg_7[0], tupledArg_7[1]);
              }(_arg1)(Token.Token[".ctor_2"]("let", (0, _List.ofArray)([$var134[1], Token.Token[".ctor_2"]("fun", (0, _List.ofArray)([$var134[2], $var130[2]]))])));

            case 1:
              return new AST("Declare", [$var134[1], function (tupledArg_8) {
                return ASTCompile_(tupledArg_8[0], tupledArg_8[1]);
              }(_arg1)($var130[2])]);

            case 2:
              return (0, _String.fsFormat)("patterns in function arguments not supported yet: %A")(function (x) {
                throw new Error(x);
              })($var130[1]);
          }

        case 8:
          return function (tupledArg_9) {
            return ASTCompile_(tupledArg_9[0], tupledArg_9[1]);
          }(_arg1)(Token.Token[".ctor_2"]("let", (0, _List.ofArray)([$var130[1], $var130[2]])));

        case 9:
          return new AST("New", [function (tupledArg_10) {
            return ASTCompile_(tupledArg_10[0], tupledArg_10[1]);
          }(_arg1)($var130[1])]);

        case 10:
          var $var135 = void 0;
          var activePatternResult766 = Token["|X|"]($var130[2]);

          if (activePatternResult766[0] === "[]") {
            if (activePatternResult766[1].tail != null) {
              if (activePatternResult766[1].tail.tail == null) {
                $var135 = [0, activePatternResult766[1].head];
              } else {
                $var135 = [1];
              }
            } else {
              $var135 = [1];
            }
          } else {
            $var135 = [1];
          }

          switch ($var135[0]) {
            case 0:
              return new AST("Get", [function (tupledArg_11) {
                return ASTCompile_(tupledArg_11[0], tupledArg_11[1]);
              }(_arg1)($var130[1]), function (tupledArg_12) {
                return ASTCompile_(tupledArg_12[0], tupledArg_12[1]);
              }(_arg1)($var135[1])]);

            case 1:
              throw new Error("should never happen");
          }

        case 11:
          var $var136 = void 0;
          var activePatternResult767 = Token["|X|"]($var130[1]);

          if (activePatternResult767[1].tail != null) {
            if (activePatternResult767[1].tail.tail == null) {
              if (activePatternResult767[0] === "deref") {
                $var136 = [2];
              } else {
                $var136 = [3];
              }
            } else {
              var activePatternResult768 = Token["|X|"](activePatternResult767[1].tail.head);

              if (activePatternResult768[0] === "[]") {
                if (activePatternResult768[1].tail != null) {
                  if (activePatternResult768[1].tail.tail == null) {
                    if (activePatternResult767[1].tail.tail.tail == null) {
                      if (activePatternResult767[0] === "dot") {
                        $var136 = [1, activePatternResult767[1].head, activePatternResult768[1].head];
                      } else {
                        $var136 = [3];
                      }
                    } else {
                      $var136 = [3];
                    }
                  } else {
                    $var136 = [3];
                  }
                } else {
                  $var136 = [3];
                }
              } else {
                $var136 = [3];
              }
            }
          } else {
            $var136 = [0, activePatternResult767[0]];
          }

          switch ($var136[0]) {
            case 0:
              return new AST("Mutate", [$var136[1], function (tupledArg_13) {
                return ASTCompile_(tupledArg_13[0], tupledArg_13[1]);
              }(_arg1)($var130[2])]);

            case 1:
              return new AST("Assign", [function (tupledArg_14) {
                return ASTCompile_(tupledArg_14[0], tupledArg_14[1]);
              }(_arg1)($var136[1]), function (tupledArg_15) {
                return ASTCompile_(tupledArg_15[0], tupledArg_15[1]);
              }(_arg1)($var136[2]), function (tupledArg_16) {
                return ASTCompile_(tupledArg_16[0], tupledArg_16[1]);
              }(_arg1)($var130[2])]);

            case 2:
              return new AST("Assign", [function (tupledArg_17) {
                return ASTCompile_(tupledArg_17[0], tupledArg_17[1]);
              }(_arg1)($var130[1]), new AST("Const", ["0"]), function (tupledArg_18) {
                return ASTCompile_(tupledArg_18[0], tupledArg_18[1]);
              }(_arg1)($var130[2])]);

            case 3:
              throw new Error("todo: unpacking");
          }

        case 12:
          return new AST("If", [function (tupledArg_19) {
            return ASTCompile_(tupledArg_19[0], tupledArg_19[1]);
          }(_arg1)($var130[2]), function (tupledArg_20) {
            return ASTCompile_(tupledArg_20[0], tupledArg_20[1]);
          }(_arg1)($var130[1]), function (tupledArg_21) {
            return ASTCompile_(tupledArg_21[0], tupledArg_21[1]);
          }(_arg1)($var130[3])]);

        case 13:
          return new AST("Apply", [new AST("Value", ["ignore"]), (0, _List.ofArray)([function (tupledArg_22) {
            return ASTCompile_(tupledArg_22[0], tupledArg_22[1]);
          }(_arg1)($var130[1])])]);

        case 14:
          return new AST("Loop", [function (tupledArg_23) {
            return ASTCompile_(tupledArg_23[0], tupledArg_23[1]);
          }(_arg1)($var130[2]), function (tupledArg_24) {
            return ASTCompile_(tupledArg_24[0], tupledArg_24[1]);
          }(_arg1)($var130[1])]);

        case 15:
          var name_1 = void 0;
          var activePatternResult769 = Token["|X|"]($var130[3]);

          if (activePatternResult769[1].tail == null) {
            name_1 = activePatternResult769[0];
          } else {
            throw new Error("todo: unpacking");
          }

          var $var137 = void 0;
          var activePatternResult770 = Token["|X|"]($var130[2]);

          if (activePatternResult770[0] === "..") {
            if (activePatternResult770[1].tail != null) {
              if (activePatternResult770[1].tail.tail != null) {
                if (activePatternResult770[1].tail.tail.tail != null) {
                  if (activePatternResult770[1].tail.tail.tail.tail == null) {
                    $var137 = [0, activePatternResult770[1].head, activePatternResult770[1].tail.tail.head, activePatternResult770[1].tail.head];
                  } else {
                    $var137 = [1];
                  }
                } else {
                  $var137 = [1];
                }
              } else {
                $var137 = [1];
              }
            } else {
              $var137 = [1];
            }
          } else {
            $var137 = [1];
          }

          switch ($var137[0]) {
            case 0:
              return new AST("Sequence", [(0, _List.ofArray)([new AST("Declare", [name_1, function (tupledArg_25) {
                return ASTCompile_(tupledArg_25[0], tupledArg_25[1]);
              }(_arg1)($var137[1])]), new AST("Loop", [new AST("Apply", [new AST("Apply", [new AST("Value", ["<="]), (0, _List.ofArray)([new AST("Value", [name_1])])]), (0, _List.ofArray)([function (tupledArg_26) {
                return ASTCompile_(tupledArg_26[0], tupledArg_26[1]);
              }(_arg1)($var137[2])])]), new AST("Sequence", [(0, _List.ofArray)([function (tupledArg_27) {
                return ASTCompile_(tupledArg_27[0], tupledArg_27[1]);
              }(_arg1)($var130[1]), new AST("Mutate", [name_1, new AST("Apply", [new AST("Apply", [new AST("Value", ["+"]), (0, _List.ofArray)([new AST("Value", [name_1])])]), (0, _List.ofArray)([function (tupledArg_28) {
                return ASTCompile_(tupledArg_28[0], tupledArg_28[1]);
              }(_arg1)($var137[3])])])])])])])])]);

            case 1:
              throw new Error("iterable objects not supported yet");
          }

        case 16:
          return new AST("Get", [function (tupledArg_29) {
            return ASTCompile_(tupledArg_29[0], tupledArg_29[1]);
          }(_arg1)($var130[1]), new AST("Const", ["0"])]);

        case 17:
          return new AST("Sequence", [(0, _List.reverse)((0, _Seq.fold)(function (tupledArg_30, e_2) {
            var compiled = function (tupledArg_31) {
              return ASTCompile_(tupledArg_31[0], tupledArg_31[1]);
            }(tupledArg_30[1])(e_2);

            var cpt__1 = compiled.Case === "Declare" ? [new _List2.default(compiled.Fields[0], tupledArg_30[1][0]), tupledArg_30[1][1]] : compiled.Case === "Define" ? [new _List2.default(compiled.Fields[0], tupledArg_30[1][0]), (0, _Map.add)(compiled.Fields[0], (0, _List.map)(function (arg0_1) {
              return new AST("Value", [arg0_1]);
            }, tupledArg_30[1][0]), tupledArg_30[1][1])] : tupledArg_30[1];
            return [new _List2.default(compiled, tupledArg_30[0]), cpt__1];
          }, [new _List2.default(), _arg1], $var130[1])[0])]);

        case 18:
          return (0, _String.fsFormat)("unknown: %A")(function (x) {
            throw new Error(x);
          })($var130[1]);
      }
    };
  }

  exports.ASTCompile$27$ = ASTCompile_;

  function ASTCompile(e) {
    return ASTCompile_(new _List2.default(), (0, _Map.create)(null, new _GenericComparer2.default(_Util.compare)))(e);
  }

  var createAST = exports.createAST = function createAST($var141) {
    return function (e_3) {
      (0, _String.fsFormat)("%O")(function (x) {
        console.log(x);
      })(e_3);
      return e_3;
    }(function ($var140) {
      return function (e_2) {
        return ASTCompile(e_2);
      }(function ($var139) {
        return applyTypeSystem(function ($var138) {
          return function (e_1) {
            return e_1.Clean();
          }(function (e) {
            return C.parseSyntax(e);
          }($var138));
        }($var139));
      }($var140));
    }($var141));
  };

  var comb2 = exports.comb2 = function () {
    _createClass(comb2, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Xlvm.comb2",
          properties: {
            Name: "string",
            Symbol: "string"
          }
        };
      }
    }]);

    function comb2(name, symbol) {
      _classCallCheck(this, comb2);

      this.name = name;
      this.symbol = symbol;
    }

    _createClass(comb2, [{
      key: "ToStrPair",
      value: function () {
        return [this.name, ""];
      }
    }, {
      key: "ToString",
      value: function () {
        return this.name;
      }
    }, {
      key: "Name",
      get: function () {
        return this.name;
      }
    }, {
      key: "Symbol",
      get: function () {
        return this.symbol;
      }
    }]);

    return comb2;
  }();

  (0, _Symbol2.setType)("Xlvm.comb2", comb2);

  var PseudoAsm = exports.PseudoAsm = function () {
    function PseudoAsm(caseName, fields) {
      _classCallCheck(this, PseudoAsm);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(PseudoAsm, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Xlvm.PseudoAsm",
          interfaces: ["FSharpUnion", "System.IEquatable"],
          cases: {
            Call: [],
            Combinator_2: [comb2],
            GetHeap: [],
            GotoFwdShift: ["number"],
            GotoIfTrueFwdShift: ["number"],
            Input: ["string"],
            Load: ["string"],
            NewHeap: [],
            Output: ["string"],
            Pop: [],
            Popv: ["string"],
            Push: ["string"],
            PushFwdShift: ["number"],
            Return: [],
            Store: ["string"],
            WriteHeap: []
          }
        };
      }
    }, {
      key: "Equals",
      value: function (other) {
        return (0, _Util.equalsUnions)(this, other);
      }
    }]);

    return PseudoAsm;
  }();

  (0, _Symbol2.setType)("Xlvm.PseudoAsm", PseudoAsm);

  var C2_Add = exports.C2_Add = function (_comb) {
    _inherits(C2_Add, _comb);

    _createClass(C2_Add, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Add, {
          type: "Xlvm.C2_Add",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Add(name, symbol) {
      _classCallCheck(this, C2_Add);

      var _this2 = _possibleConstructorReturn(this, (C2_Add.__proto__ || Object.getPrototypeOf(C2_Add)).call(this, name, symbol));

      return _this2;
    }

    _createClass(C2_Add, [{
      key: "Interpret",
      value: function (a, b) {
        return String(Number.parseInt(a) + Number.parseInt(b));
      }
    }]);

    return C2_Add;
  }(comb2);

  (0, _Symbol2.setType)("Xlvm.C2_Add", C2_Add);

  var C2_Equals = exports.C2_Equals = function (_comb2) {
    _inherits(C2_Equals, _comb2);

    _createClass(C2_Equals, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Equals, {
          type: "Xlvm.C2_Equals",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Equals(name, symbol) {
      _classCallCheck(this, C2_Equals);

      var _this3 = _possibleConstructorReturn(this, (C2_Equals.__proto__ || Object.getPrototypeOf(C2_Equals)).call(this, name, symbol));

      return _this3;
    }

    _createClass(C2_Equals, [{
      key: "Interpret",
      value: function (a, b) {
        return String(a === b);
      }
    }]);

    return C2_Equals;
  }(comb2);

  (0, _Symbol2.setType)("Xlvm.C2_Equals", C2_Equals);

  function isInt(_arg1) {
    isInt: while (true) {
      if (_arg1 === "") {
        return false;
      } else if (_arg1[0] === "-") {
        _arg1 = _arg1.slice(1, _arg1.length);
        continue isInt;
      } else {
        return (0, _Seq.forAll)(function (e) {
          return "0" <= e ? e <= "9" : false;
        }, _arg1);
      }
    }
  }

  var C2_LEq = exports.C2_LEq = function (_comb3) {
    _inherits(C2_LEq, _comb3);

    _createClass(C2_LEq, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_LEq, {
          type: "Xlvm.C2_LEq",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_LEq(name, symbol) {
      _classCallCheck(this, C2_LEq);

      var _this4 = _possibleConstructorReturn(this, (C2_LEq.__proto__ || Object.getPrototypeOf(C2_LEq)).call(this, name, symbol));

      return _this4;
    }

    _createClass(C2_LEq, [{
      key: "Interpret",
      value: function (a, b) {
        if (isInt(a) ? isInt(b) : false) {
          return String(Number.parseInt(a) <= Number.parseInt(b));
        } else {
          return String(a <= b);
        }
      }
    }]);

    return C2_LEq;
  }(comb2);

  (0, _Symbol2.setType)("Xlvm.C2_LEq", C2_LEq);

  var C2_Greater = exports.C2_Greater = function (_comb4) {
    _inherits(C2_Greater, _comb4);

    _createClass(C2_Greater, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Greater, {
          type: "Xlvm.C2_Greater",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Greater(name, symbol) {
      _classCallCheck(this, C2_Greater);

      var _this5 = _possibleConstructorReturn(this, (C2_Greater.__proto__ || Object.getPrototypeOf(C2_Greater)).call(this, name, symbol));

      return _this5;
    }

    _createClass(C2_Greater, [{
      key: "Interpret",
      value: function (a, b) {
        if (isInt(a) ? isInt(b) : false) {
          return String(Number.parseInt(a) > Number.parseInt(b));
        } else {
          return String(a > b);
        }
      }
    }]);

    return C2_Greater;
  }(comb2);

  (0, _Symbol2.setType)("Xlvm.C2_Greater", C2_Greater);

  var C2_Mod = exports.C2_Mod = function (_comb5) {
    _inherits(C2_Mod, _comb5);

    _createClass(C2_Mod, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Mod, {
          type: "Xlvm.C2_Mod",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Mod(name, symbol) {
      _classCallCheck(this, C2_Mod);

      var _this6 = _possibleConstructorReturn(this, (C2_Mod.__proto__ || Object.getPrototypeOf(C2_Mod)).call(this, name, symbol));

      return _this6;
    }

    _createClass(C2_Mod, [{
      key: "Interpret",
      value: function (a, b) {
        return String(Number.parseInt(a) % Number.parseInt(b));
      }
    }]);

    return C2_Mod;
  }(comb2);

  (0, _Symbol2.setType)("Xlvm.C2_Mod", C2_Mod);
  var allCombinators = exports.allCombinators = (0, _List.map)(function (arg0) {
    return new PseudoAsm("Combinator_2", [arg0]);
  }, (0, _List.ofArray)([new C2_Add("add", "+"), new C2_Equals("equals", "="), new C2_LEq("leq", "<="), new C2_Greater("greater", ">"), new C2_Mod("mod", "%")]));
  var patternInput_1016 = allCombinators;

  var matchResultHolder_1016 = function () {
    var $var142 = patternInput_1016.tail != null ? patternInput_1016.tail.tail != null ? patternInput_1016.tail.tail.tail != null ? patternInput_1016.tail.tail.tail.tail != null ? patternInput_1016.tail.tail.tail.tail.tail != null ? patternInput_1016.tail.tail.tail.tail.tail.tail == null ? [0] : [1] : [1] : [1] : [1] : [1] : [1];

    switch ($var142[0]) {
      case 0:
        break;

      case 1:
        throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 1016, 4);
        break;
    }
  }();

  var Mod = exports.Mod = patternInput_1016.tail.tail.tail.tail.head;
  var LEq = exports.LEq = patternInput_1016.tail.tail.head;
  var Greater = exports.Greater = patternInput_1016.tail.tail.tail.head;
  var Equals = exports.Equals = patternInput_1016.tail.head;
  var Add = exports.Add = patternInput_1016.head;
  var x = exports.x = "F";

  function createComb2Section(cmd) {
    return (0, _List.ofArray)([cmd, new PseudoAsm("Return", []), new PseudoAsm("Store", [x]), new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("PushFwdShift", [-6]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Popv", [x]), new PseudoAsm("NewHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Popv", [x]), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Return", [])]);
  }

  var allComb2Sections = exports.allComb2Sections = (0, _List.collect)(function (cmd) {
    return createComb2Section(cmd);
  }, allCombinators);

  function getSectionAddress(i) {
    return 18 * i + 3;
  }

  function getSectionAddressFromCmd(cmd) {
    return getSectionAddress((0, _Seq.findIndex)(function (y) {
      return cmd.Equals(y);
    }, allCombinators));
  }

  function getSectionAddressFromInfix(nfx) {
    return getSectionAddressFromCmd((0, _Seq.find)(function (_arg1) {
      return _arg1.Case === "Combinator_2" ? _arg1.Fields[0].Symbol === nfx : false;
    }, allCombinators));
  }

  var allTypes = exports.allTypes = (0, _List.ofArray)(["%i", "%s"]);
  var patternInput_1034_1 = allTypes;

  var matchResultHolder_1034_1 = function () {
    var $var143 = patternInput_1034_1.tail != null ? patternInput_1034_1.tail.tail != null ? patternInput_1034_1.tail.tail.tail == null ? [0] : [1] : [1] : [1];

    switch ($var143[0]) {
      case 0:
        break;

      case 1:
        throw new Error("C:\\Users\\Thefak\\Desktop\\programming\\__random_folder\\requirejs\\xlvm.fsx", 1034, 4);
        break;
    }
  }();

  var type_string = exports.type_string = patternInput_1034_1.tail.head;
  var type_int32 = exports.type_int32 = patternInput_1034_1.head;
  var operationsPrefix = exports.operationsPrefix = (0, _List.append)(allComb2Sections, (0, _List.collect)(function (e) {
    return (0, _List.ofArray)([new PseudoAsm("Output", [e]), new PseudoAsm("Push", ["()"]), new PseudoAsm("Return", [])]);
  }, allTypes));

  function _PrintAddress(t) {
    var matchValue = (0, _Seq.tryFindIndex)(function (y) {
      return t === y;
    }, allTypes);

    if (matchValue == null) {
      return (0, _String.fsFormat)("can't output type: %A")(function (x) {
        throw new Error(x);
      })(t);
    } else {
      return allComb2Sections.length + 1 + 3 * matchValue;
    }
  }

  function _Inline___(_arg1) {
    var $var144 = _arg1.Case === "Value" ? (0, _Seq.exists)(function (_arg2) {
      return _arg2.Case === "Combinator_2" ? _arg2.Fields[0].Symbol === _arg1.Fields[0] : false;
    }, allCombinators) ? [0, _arg1.Fields[0]] : [1] : [1];

    switch ($var144[0]) {
      case 0:
        return (0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("Push", [String(getSectionAddressFromInfix($var144[1]))]), new PseudoAsm("WriteHeap", []), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Popv", [x])]);

      case 1:
        var $var145 = _arg1.Case === "Value" ? _arg1.Fields[0] === "ignore" ? [0] : _arg1.Fields[0] === "printf" ? [2] : _arg1.Fields[0] === "nothing" ? [4] : [5] : _arg1.Case === "Apply" ? _arg1.Fields[0].Case === "Value" ? _arg1.Fields[0].Fields[0] === "printf" ? _arg1.Fields[1].tail != null ? _arg1.Fields[1].head.Case === "Const" ? _arg1.Fields[1].head.Fields[0] === "%i" ? _arg1.Fields[1].tail.tail == null ? [1] : [5] : _arg1.Fields[1].head.Fields[0] === "%s" ? _arg1.Fields[1].tail.tail == null ? [2] : [5] : [5] : [5] : [5] : _arg1.Fields[0].Fields[0] === "scan" ? _arg1.Fields[1].tail != null ? _arg1.Fields[1].head.Case === "Const" ? _arg1.Fields[1].tail.tail == null ? [3, _arg1.Fields[1].head.Fields[0]] : [5] : _arg1.Fields[1].head.Case === "Value" ? _arg1.Fields[1].tail.tail == null ? [3, _arg1.Fields[1].head.Fields[0]] : [5] : [5] : [5] : [5] : [5] : [5];

        switch ($var145[0]) {
          case 0:
            return (0, _List.ofArray)([new PseudoAsm("Push", ["not implemented"])]);

          case 1:
            return (0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("Push", [String(_PrintAddress(type_int32))]), new PseudoAsm("WriteHeap", []), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Popv", [x])]);

          case 2:
            return (0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("Push", [String(_PrintAddress(type_string))]), new PseudoAsm("WriteHeap", []), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Popv", [x])]);

          case 3:
            return (0, _List.ofArray)([new PseudoAsm("Input", [$var145[1]])]);

          case 4:
            return (0, _List.ofArray)([new PseudoAsm("Push", ["nothing"])]);

          case 5:
            return null;
        }

    }
  }

  exports.$7C$Inline$7C$_$7C$ = _Inline___;

  function compile_(inScope, _arg3) {
    var activePatternResult894 = _Inline___(_arg3);

    if (activePatternResult894 != null) {
      return activePatternResult894;
    } else if (_arg3.Case === "Declare") {
      return (0, _List.append)(compile_(new _List2.default(_arg3.Fields[0], inScope), _arg3.Fields[1]), (0, _List.ofArray)([new PseudoAsm("Store", [_arg3.Fields[0]]), new PseudoAsm("Push", ["()"])]));
    } else if (_arg3.Case === "Define") {
      var functionBody = (0, _List.append)((0, _List.map)(function (arg0) {
        return new PseudoAsm("Store", [arg0]);
      }, (0, _List.reverse)(_arg3.Fields[1])), (0, _List.append)(compile_(_arg3.Fields[1], _arg3.Fields[2]), (0, _List.append)((0, _List.map)(function (arg0_1) {
        return new PseudoAsm("Popv", [arg0_1]);
      }, _arg3.Fields[1]), (0, _List.ofArray)([new PseudoAsm("Return", [])]))));
      var len = functionBody.length;
      return (0, _List.append)((0, _List.ofArray)([new PseudoAsm("GotoFwdShift", [len + 1])]), (0, _List.append)(functionBody, (0, _List.append)((0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [_arg3.Fields[0]]), new PseudoAsm("Load", [_arg3.Fields[0]]), new PseudoAsm("PushFwdShift", [-len - 3]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [_arg3.Fields[0]])]), (0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", [])]))));
    } else if (_arg3.Case === "Value") {
      return (0, _List.ofArray)([new PseudoAsm("Load", [_arg3.Fields[0]])]);
    } else if (_arg3.Case === "Const") {
      return (0, _List.ofArray)([new PseudoAsm("Push", [_arg3.Fields[0]])]);
    } else if (_arg3.Case === "Apply") {
      var functionArray = (0, _List.append)(compile_(inScope, _arg3.Fields[0]), (0, _List.ofArray)([new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x])]));
      var loop = (0, _List.ofArray)([new PseudoAsm("Push", ["1"]), Add, new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("GetHeap", []), new PseudoAsm("Push", ["endArr"]), Equals, new PseudoAsm("GotoIfTrueFwdShift", [9]), new PseudoAsm("Load", [x]), new PseudoAsm("GetHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Push", ["1"]), Add, new PseudoAsm("Popv", [x]), new PseudoAsm("Store", [x]), new PseudoAsm("GotoFwdShift", [-12]), new PseudoAsm("Popv", [x])]);
      var call = (0, _List.ofArray)([new PseudoAsm("Load", [x]), new PseudoAsm("GetHeap", []), new PseudoAsm("Call", []), new PseudoAsm("Popv", [x])]);
      return (0, _List.append)((0, _List.collect)(function (_arg3_1) {
        return compile_(inScope, _arg3_1);
      }, _arg3.Fields[1]), (0, _List.append)(functionArray, (0, _List.append)(loop, call)));
    } else if (_arg3.Case === "If") {
      var patternInput = [compile_(inScope, _arg3.Fields[0]), compile_(inScope, _arg3.Fields[1]), compile_(inScope, _arg3.Fields[2])];
      return (0, _List.append)(patternInput[0], (0, _List.append)((0, _List.ofArray)([new PseudoAsm("GotoIfTrueFwdShift", [patternInput[2].length + 2])]), (0, _List.append)(patternInput[2], (0, _List.append)((0, _List.ofArray)([new PseudoAsm("GotoFwdShift", [patternInput[1].length + 1])]), patternInput[1]))));
    } else if (_arg3.Case === "New") {
      var loop_1 = (0, _List.ofArray)([new PseudoAsm("Store", [x]), new PseudoAsm("NewHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Push", ["1"]), Equals, new PseudoAsm("GotoIfTrueFwdShift", [9]), new PseudoAsm("NewHeap", []), new PseudoAsm("Pop", []), new PseudoAsm("Load", [x]), new PseudoAsm("Push", ["-1"]), Add, new PseudoAsm("Popv", [x]), new PseudoAsm("Store", [x]), new PseudoAsm("GotoFwdShift", [-11]), new PseudoAsm("Popv", [x]), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", [])]);
      return (0, _List.append)(compile_(inScope, _arg3.Fields[0]), loop_1);
    } else if (_arg3.Case === "Get") {
      return (0, _List.append)(compile_(inScope, _arg3.Fields[0]), (0, _List.append)(compile_(inScope, _arg3.Fields[1]), (0, _List.ofArray)([Add, new PseudoAsm("GetHeap", [])])));
    } else if (_arg3.Case === "Assign") {
      return (0, _List.append)(compile_(inScope, _arg3.Fields[0]), (0, _List.append)(compile_(inScope, _arg3.Fields[1]), (0, _List.append)((0, _List.ofArray)([Add]), (0, _List.append)(compile_(inScope, _arg3.Fields[2]), (0, _List.ofArray)([new PseudoAsm("WriteHeap", []), new PseudoAsm("Push", ["()"])])))));
    } else if (_arg3.Case === "Return") {
      var exitScope = (0, _List.append)((0, _List.map)(function (arg0_2) {
        return new PseudoAsm("Popv", [arg0_2]);
      }, inScope), (0, _List.ofArray)([new PseudoAsm("Return", [])]));

      if (_arg3.Fields[0] != null) {
        return (0, _List.append)(compile_(inScope, _arg3.Fields[0]), exitScope);
      } else {
        return exitScope;
      }
    } else if (_arg3.Case === "Loop") {
      var patternInput_1 = [compile_(inScope, _arg3.Fields[0]), compile_(inScope, _arg3.Fields[1])];
      var a = (0, _List.append)(patternInput_1[0], (0, _List.append)((0, _List.ofArray)([new PseudoAsm("GotoIfTrueFwdShift", [2]), new PseudoAsm("GotoFwdShift", [patternInput_1[1].length + 3])]), (0, _List.append)(patternInput_1[1], (0, _List.ofArray)([new PseudoAsm("Pop", [])]))));
      return (0, _List.append)(a, (0, _List.ofArray)([new PseudoAsm("GotoFwdShift", [-a.length]), new PseudoAsm("Push", ["()"])]));
    } else if (_arg3.Case === "Mutate") {
      return (0, _List.append)(compile_(inScope, _arg3.Fields[1]), (0, _List.ofArray)([new PseudoAsm("Popv", [_arg3.Fields[0]]), new PseudoAsm("Store", [_arg3.Fields[0]]), new PseudoAsm("Push", ["()"])]));
    } else {
      var x_1 = (0, _List.collect)(function (e) {
        return new _List2.default(new PseudoAsm("Pop", []), (0, _List.reverse)(compile_(inScope, e)));
      }, (0, _List.reverse)(_arg3.Fields[0])).tail;
      return function (_arg4) {
        var $var146 = _arg4[0].tail != null ? _arg4[0].head.Case === "Pop" ? [0, _arg4[0].tail, _arg4[1]] : [1] : [1];

        switch ($var146[0]) {
          case 0:
            return (0, _List.append)((0, _List.reverse)($var146[1]), (0, _List.map)(function (arg0_3) {
              return new PseudoAsm("Popv", [arg0_3]);
            }, $var146[2]));

          case 1:
            throw new Error("this should never happen unless the sequence was empty");
        }
      }((0, _Seq.fold)(function (tupledArg, e_1) {
        var acc_ = (0, _List.append)(new _List2.default(new PseudoAsm("Pop", []), (0, _List.reverse)(compile_((0, _List.append)(tupledArg[1], inScope), e_1))), tupledArg[0]);

        if (e_1.Case === "Declare") {
          return [acc_, new _List2.default(e_1.Fields[0], tupledArg[1])];
        } else {
          return [acc_, tupledArg[1]];
        }
      }, [new _List2.default(), new _List2.default()], _arg3.Fields[0]));
    }
  }

  exports.compile$27$ = compile_;

  function compile(e) {
    return (0, _List.append)((0, _List.ofArray)([new PseudoAsm("GotoFwdShift", [operationsPrefix.length + 1])]), (0, _List.append)(operationsPrefix, compile_(new _List2.default(), e)));
  }

  var createASM = exports.createASM = function createASM($var148) {
    return function (e_1) {
      (0, _String.fsFormat)("%s")(function (x) {
        console.log(x);
      })((0, _String.join)(" ", (0, _List.map)((0, _String.fsFormat)("%A")(function (x) {
        return x;
      }), e_1)));
      return e_1;
    }(function ($var147) {
      return function (e) {
        return compile(e);
      }(createAST($var147));
    }($var148));
  };

  var Interpreters = exports.Interpreters = function (__exports) {
    var interpretPAsm = __exports.interpretPAsm = function (cmds, stdInput) {
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
      var x_1 = Array.from((0, _Set.create)(Array.from((0, _Seq.rangeChar)("A", "E")).map(function (value) {
        return value;
      }).concat(Array.from((0, _Seq.choose)(function (_arg1) {
        var $var149 = _arg1.Case === "Store" ? [0, _arg1.Fields[0]] : _arg1.Case === "Load" ? [0, _arg1.Fields[0]] : _arg1.Case === "Popv" ? [0, _arg1.Fields[0]] : [1];

        switch ($var149[0]) {
          case 0:
            return $var149[1];

          case 1:
            return null;
        }
      }, cmds))), new _GenericComparer2.default(_Util.compare)));
      stacks = new Map(Array.from((0, _Seq.zip)(x_1, Array.from((0, _Seq.initialize)(x_1.length, function (_arg1_1) {
        return {
          contents: new _List2.default()
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
      var heap = [];
      (0, _String.split)(stdInput, " ").slice().reverse().forEach(push(patternInput[3]));
      (0, _String.fsFormat)("input: %A")(function (x) {
        console.log(x);
      })(stacks.get(patternInput[3]).contents);

      var interpretCmd = function interpretCmd(i) {
        var fwd = function fwd($var150) {
          return String(i - 1 + $var150);
        };

        var fwd2 = function fwd2($var151) {
          return function (value_1) {
            return String(value_1);
          }(function (y) {
            return i + y;
          }($var151));
        };

        return function (_arg2) {
          if (_arg2.Case === "PushFwdShift") {
            push(patternInput[1])(fwd2(_arg2.Fields[0]));
          } else if (_arg2.Case === "Pop") {
            pop(patternInput[1]);
          } else if (_arg2.Case === "Store") {
            push(_arg2.Fields[0])(top(patternInput[1]));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Load") {
            push(patternInput[1])(top(_arg2.Fields[0]));
          } else if (_arg2.Case === "Popv") {
            pop(_arg2.Fields[0]);
          } else if (_arg2.Case === "GotoFwdShift") {
            pop(patternInput[0]);
            push(patternInput[0])(fwd(_arg2.Fields[0]));
          } else if (_arg2.Case === "GotoIfTrueFwdShift") {
            if (top(patternInput[1]).toLocaleLowerCase() === "true") {
              pop(patternInput[0]);
              push(patternInput[0])(fwd(_arg2.Fields[0]));
            }

            pop(patternInput[1]);
          } else if (_arg2.Case === "Call") {
            push(patternInput[0])(String(Number.parseInt(top(patternInput[1])) - 1));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Return") {
            pop(patternInput[0]);
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
            push(patternInput[1])(top(patternInput[3]));
            pop(patternInput[3]);
          } else if (_arg2.Case === "Output") {
            push(patternInput[4])(top(patternInput[1]));
            pop(patternInput[1]);
          } else if (_arg2.Case === "Combinator_2") {
            var a = top(patternInput[1]);
            pop(patternInput[1]);
            var b = top(patternInput[1]);
            pop(patternInput[1]);
            push(patternInput[1])(function (arg00) {
              return function (arg10) {
                return _arg2.Fields[0].Interpret(arg00, arg10);
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
        }

        var pt = Number.parseInt(top(patternInput[0]));
        pop(patternInput[0]);
        push(patternInput[0])(String(pt + 1));
      }

      (0, _String.fsFormat)("%A")(function (x) {
        console.log(x);
      })([stacks.get(patternInput[1]).contents, heap, (0, _List.reverse)(stacks.get(patternInput[4]).contents)]);
      return (0, _String.join)("", (0, _List.reverse)(stacks.get(patternInput[4]).contents));
    };

    return __exports;
  }({});

  var compileAndRun = exports.compileAndRun = function compileAndRun($var153) {
    return function (cmds) {
      return function (stdInput) {
        return Interpreters.interpretPAsm(cmds, stdInput);
      };
    }(function ($var152) {
      return function (list) {
        return Array.from(list);
      }(createASM($var152));
    }($var153));
  };
});