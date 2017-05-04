define(["exports", "fable-core/umd/List", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/Seq", "fable-core/umd/String", "fable-core/umd/Choice"], function (exports, _List, _Symbol2, _Util, _Seq, _String, _Choice) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.$7C$Var$7C$Cnst$7C$Other$7C$ = exports.$7C$Inner$7C$_$7C$ = exports.$7C$Pref$27$$7C$_$7C$ = exports.$7C$Pref$7C$_$7C$ = exports.$7C$X$7C$ = exports.$7C$A$7C$_$7C$ = exports.$7C$T$7C$_$7C$ = exports.Token = exports.Symbols = undefined;

  var _List2 = _interopRequireDefault(_List);

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  var _Choice2 = _interopRequireDefault(_Choice);

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

  var Symbols = exports.Symbols = function (__exports) {
    var infixOrder = __exports.infixOrder = [(0, _List.ofArray)(["*", "/", "%"]), (0, _List.ofArray)(["+", "-"]), (0, _List.append)((0, _List.ofArray)(["=", "<>", ">", ">=", "<", "<="]), (0, _List.ofArray)(["==", "!="])), (0, _List.ofArray)(["&&"]), (0, _List.ofArray)(["||"])];
    var prefixes = __exports.prefixes = (0, _List.ofArray)(["-", "!", "&", "*", "++", "--"]);
    return __exports;
  }({});

  var Token = exports.Token = function () {
    _createClass(Token, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Parser.Definition.Token",
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

  (0, _Symbol2.setType)("Parser.Definition.Token", Token);

  function _T___(x) {
    if (x.Single) {
      return x.Name;
    } else {
      return null;
    }
  }

  exports.$7C$T$7C$_$7C$ = _T___;

  function _A___(x) {
    if (x.CanApply) {
      return {};
    } else {
      return null;
    }
  }

  exports.$7C$A$7C$_$7C$ = _A___;

  function _X_(t) {
    return [t.Name, t.Dependants];
  }

  exports.$7C$X$7C$ = _X_;

  function _Pref___(_arg1) {
    var $var3 = void 0;

    var activePatternResult130 = _T___(_arg1);

    if (activePatternResult130 != null) {
      if ((0, _Seq.exists)(function (y) {
        return activePatternResult130 === y;
      }, Symbols.prefixes)) {
        $var3 = [0, activePatternResult130, _arg1];
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
  }

  exports.$7C$Pref$7C$_$7C$ = _Pref___;

  function _Pref____(_arg1) {
    var $var4 = void 0;

    var activePatternResult132 = _T___(_arg1);

    if (activePatternResult132 != null) {
      if ((activePatternResult132.length > 1 ? activePatternResult132[0] === "~" : false) ? (0, _Seq.exists)(function () {
        var x = activePatternResult132.slice(1, activePatternResult132.length);
        return function (y) {
          return x === y;
        };
      }(), Symbols.prefixes) : false) {
        $var4 = [0, activePatternResult132, _arg1];
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
  }

  exports.$7C$Pref$27$$7C$_$7C$ = _Pref____;

  function _Inner___(c, _arg1) {
    var $var5 = void 0;

    var activePatternResult136 = _T___(_arg1);

    if (activePatternResult136 != null) {
      if (activePatternResult136 === "\"\"") {
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

        var activePatternResult135 = _T___(_arg1);

        if (activePatternResult135 != null) {
          if ((activePatternResult135.length >= 2 ? activePatternResult135[0] === c : false) ? activePatternResult135[activePatternResult135.length - 1] === c : false) {
            $var6 = [0, activePatternResult135];
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
  }

  exports.$7C$Inner$7C$_$7C$ = _Inner___;

  function _Var_Cnst_Other_(_arg1) {
    var activePatternResult149 = function () {
      var c = "\"";
      return function (_arg1_1) {
        return _Inner___(c, _arg1_1);
      };
    }()(_arg1);

    if (activePatternResult149 != null) {
      return new _Choice2.default("Choice2Of3", [activePatternResult149]);
    } else {
      var activePatternResult147 = function () {
        var c_1 = "'";
        return function (_arg1_2) {
          return _Inner___(c_1, _arg1_2);
        };
      }()(_arg1);

      if (activePatternResult147 != null) {
        return new _Choice2.default("Choice2Of3", [String(activePatternResult147[0].charCodeAt(0))]);
      } else {
        var $var7 = void 0;

        var activePatternResult145 = _T___(_arg1);

        if (activePatternResult145 != null) {
          if (activePatternResult145 === "()") {
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

            var activePatternResult144 = _T___(_arg1);

            if (activePatternResult144 != null) {
              if (activePatternResult144 === "nothing") {
                $var8 = [0];
              } else {
                $var8 = [1];
              }
            } else {
              $var8 = [1];
            }

            switch ($var8[0]) {
              case 0:
                return new _Choice2.default("Choice2Of3", ["nothing"]);

              case 1:
                var $var9 = void 0;

                var activePatternResult143 = _T___(_arg1);

                if (activePatternResult143 != null) {
                  if (activePatternResult143 === "true") {
                    $var9 = [0];
                  } else {
                    $var9 = [1];
                  }
                } else {
                  $var9 = [1];
                }

                switch ($var9[0]) {
                  case 0:
                    return new _Choice2.default("Choice2Of3", ["1"]);

                  case 1:
                    var $var10 = void 0;

                    var activePatternResult142 = _T___(_arg1);

                    if (activePatternResult142 != null) {
                      if (activePatternResult142 === "false") {
                        $var10 = [0];
                      } else {
                        $var10 = [1];
                      }
                    } else {
                      $var10 = [1];
                    }

                    switch ($var10[0]) {
                      case 0:
                        return new _Choice2.default("Choice2Of3", ["0"]);

                      case 1:
                        var $var11 = void 0;

                        var activePatternResult141 = _T___(_arg1);

                        if (activePatternResult141 != null) {
                          if (activePatternResult141 === "true") {
                            $var11 = [0, activePatternResult141];
                          } else if (activePatternResult141 === "false") {
                            $var11 = [0, activePatternResult141];
                          } else {
                            $var11 = [1];
                          }
                        } else {
                          $var11 = [1];
                        }

                        switch ($var11[0]) {
                          case 0:
                            return new _Choice2.default("Choice2Of3", [$var11[1]]);

                          case 1:
                            var $var12 = void 0;

                            var activePatternResult140 = _T___(_arg1);

                            if (activePatternResult140 != null) {
                              if (activePatternResult140 === "return") {
                                $var12 = [0];
                              } else if (activePatternResult140 === "break") {
                                $var12 = [0];
                              } else {
                                $var12 = [1];
                              }
                            } else {
                              $var12 = [1];
                            }

                            switch ($var12[0]) {
                              case 0:
                                return new _Choice2.default("Choice3Of3", [null]);

                              case 1:
                                var activePatternResult139 = _T___(_arg1);

                                if (activePatternResult139 != null) {
                                  if ((activePatternResult139 !== "" ? "0" <= activePatternResult139[0] : false) ? activePatternResult139[0] <= "9" : false) {
                                    return new _Choice2.default("Choice2Of3", [activePatternResult139]);
                                  } else {
                                    return new _Choice2.default("Choice1Of3", [activePatternResult139]);
                                  }
                                } else {
                                  return new _Choice2.default("Choice3Of3", [null]);
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

  exports.$7C$Var$7C$Cnst$7C$Other$7C$ = _Var_Cnst_Other_;
});