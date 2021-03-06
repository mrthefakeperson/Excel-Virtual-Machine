define(["exports", "fable-core/umd/Seq", "fable-core/umd/Symbol", "fable-core/umd/List", "fable-core/umd/Util", "fable-core/umd/String"], function (exports, _Seq, _Symbol2, _List, _Util, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.createVariablesAndNumbers = exports.createStrings = exports.SpecialCases = exports.tokenizeRule = exports.CommonClassifiers = undefined;
  exports.makeRule = makeRule;
  exports.makeSymbolRule = makeSymbolRule;
  exports.makeDelimiterRule = makeDelimiterRule;
  exports.tokenize = tokenize;
  exports.createSingleLineComment = createSingleLineComment;
  exports.createDelimitedComment = createDelimitedComment;
  exports.createSymbol = createSymbol;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  var _List2 = _interopRequireDefault(_List);

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

  var CommonClassifiers = exports.CommonClassifiers = function (__exports) {
    var getOccurances = __exports.getOccurances = function (pred, str) {
      return Array.from((0, _Seq.collect)(function (e) {
        return pred(e) ? e : "";
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

    var entireString = __exports.entireString = function (predicate) {
      return function (str) {
        return (0, _Seq.forAll)(predicate, str);
      };
    };

    var firstCharOfString = __exports.firstCharOfString = function (isClassified, str) {
      return isClassified(str[0]);
    };

    var firstSuffixOfString = __exports.firstSuffixOfString = function (isClassified, str) {
      return entireString(isClassified)(str.slice(1, str.length));
    };

    var firstCharIsXAndSuffixIsY = __exports.firstCharIsXAndSuffixIsY = function (isClassified, pred, str) {
      if (firstCharOfString(isClassified, str)) {
        return pred(str.slice(1, str.length));
      } else {
        return false;
      }
    };

    var isNumeral = __exports.isNumeral = function (arg00) {
      return !isNaN(parseInt(arg00));
    };

    var isAlphabetic = __exports.isAlphabetic = function (arg00) {
      return (/^[a-zA-Z]$/.test(arg00)
      );
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
        return op_GreaterGreaterBarBar(function (arg00) {
          return isNumeral(arg00);
        }, function (arg00_1) {
          return isAlphabetic(arg00_1);
        }, e);
      };

      return function (e_1) {
        return op_GreaterGreaterBarBar(classifierA, isUndersc, e_1);
      };
    }();

    var isWhitespaceChar = __exports.isWhitespaceChar = function () {
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

    var isVariableSuffix = __exports.isVariableSuffix = entireString(isValidForVariables);

    var isInteger = __exports.isInteger = function (str) {
      return firstCharIsXAndSuffixIsY(function (arg00) {
        return isNumeral(arg00);
      }, isVariableSuffix, str);
    };

    var isVariable = __exports.isVariable = function () {
      var isClassified = void 0;

      var classifierB = function classifierB($var13) {
        return function (value) {
          return !value;
        }(function (arg00) {
          return isNumeral(arg00);
        }($var13));
      };

      isClassified = function isClassified(e) {
        return op_GreaterGreaterAmpAmp(isValidForVariables, classifierB, e);
      };

      return function (str) {
        return firstCharIsXAndSuffixIsY(isClassified, isVariableSuffix, str);
      };
    }();

    var isNumericSuffix = __exports.isNumericSuffix = entireString(function (e) {
      return op_GreaterGreaterBarBar(isDot, isValidForVariables, e);
    });

    var isNumeric = __exports.isNumeric = function (str) {
      return firstCharIsXAndSuffixIsY(function (arg00) {
        return isNumeral(arg00);
      }, isNumericSuffix, str);
    };

    var isFloat = __exports.isFloat = function () {
      var classifierB = function classifierB($var14) {
        return function (value) {
          return !value;
        }(isInteger($var14));
      };

      return function (e) {
        return op_GreaterGreaterAmpAmp(isNumeric, classifierB, e);
      };
    }();

    var isWhitespace = __exports.isWhitespace = entireString(isWhitespaceChar);
    var isAnything = __exports.isAnything = entireString(function (_arg1) {
      return true;
    });

    var failedToClassify = __exports.failedToClassify = function (message, _arg1) {
      throw new Error(message);
    };

    return __exports;
  }({});

  var tokenizeRule = exports.tokenizeRule = function () {
    function tokenizeRule(caseName, fields) {
      _classCallCheck(this, tokenizeRule);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(tokenizeRule, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Parser.Lexer.tokenizeRule",
          interfaces: ["FSharpUnion"],
          cases: {
            PriorityStickRule: ["function"],
            StickRule: ["function"],
            SymbolRule: ["string"]
          }
        };
      }
    }]);

    return tokenizeRule;
  }();

  (0, _Symbol2.setType)("Parser.Lexer.tokenizeRule", tokenizeRule);

  function makeRule(c1, c2) {
    return new tokenizeRule("StickRule", [function (s1) {
      return function (s2) {
        return c1(s1) ? c2(s2) : false;
      };
    }]);
  }

  function makeSymbolRule(symbol) {
    return new tokenizeRule("SymbolRule", [symbol]);
  }

  function makeDelimiterRule(symbol1, symbol2) {
    return new tokenizeRule("PriorityStickRule", [function (s1) {
      return function (s2) {
        return CommonClassifiers.isPrefix(symbol1, s1) ? !CommonClassifiers.isDelimitedString(symbol1, symbol2, s1) : false;
      };
    }]);
  }

  function tokenize(ruleset, txt) {
    var ruleset_1 = (0, _List.append)(ruleset, (0, _List.ofArray)([[makeRule(function () {
      var message = "could not classify";
      return function (arg10_) {
        return CommonClassifiers.failedToClassify(message, arg10_);
      };
    }(), CommonClassifiers.isAnything), false]]));
    var patternInput = (0, _Seq.fold)(function (tupledArg, _arg1) {
      if (_arg1[0].Case === "PriorityStickRule") {
        var r = _arg1[0].Fields[0];
        return [tupledArg[0], tupledArg[1], new _List2.default([r, _arg1[1]], tupledArg[2])];
      } else if (_arg1[0].Case === "SymbolRule") {
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
                var $var15 = _arg2[0].tail != null ? _arg2[1].tail != null ? [0, _arg2[0].head, _arg2[1].head, _arg2[0].tail, _arg2[1].tail] : [1] : [1];

                switch ($var15[0]) {
                  case 0:
                    var matchValue = tryGetPrefix([$var15[3], $var15[4]]);

                    if (matchValue != null) {
                      var s_1 = matchValue[1];
                      var p = matchValue[0];
                      return [new _List2.default($var15[2], p), s_1];
                    } else {
                      return null;
                    }

                  case 1:
                    throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/Parser.Lexer.fs", 74, 31);
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

        return [groupSymbols(tupledArg[0]), tupledArg[1], tupledArg[2]];
      } else {
        var r_1 = _arg1[0].Fields[0];
        return [tupledArg[0], new _List2.default([r_1, _arg1[1]], tupledArg[1]), tupledArg[2]];
      }
    }, [txt, new _List2.default(), new _List2.default()], ruleset_1);
    var stickRules = (0, _List.append)((0, _List.reverse)(patternInput[2]), (0, _List.reverse)(patternInput[1]));
    return (0, _List.reverse)((0, _Seq.fold)(function (acc, nextToken) {
      if (acc.tail != null) {
        var patternInput_1 = (0, _Seq.find)(function (tupledArg_1) {
          return tupledArg_1[0](acc.head)(nextToken);
        }, stickRules);

        if (patternInput_1[1]) {
          return new _List2.default(acc.head + nextToken, acc.tail);
        } else {
          return new _List2.default(nextToken, acc);
        }
      } else {
        return (0, _List.ofArray)([nextToken]);
      }
    }, new _List2.default(), patternInput[0]));
  }

  var SpecialCases = exports.SpecialCases = function (__exports) {
    var detectEscapeSequenceDoubleQuote = __exports.detectEscapeSequenceDoubleQuote = new tokenizeRule("StickRule", [function (s1) {
      return function (s2) {
        return CommonClassifiers.isPrefix("\"", s1) ? CommonClassifiers.isSuffix("\\\"", s1) : false;
      };
    }]);
    return __exports;
  }({});

  function createSingleLineComment(delimiter) {
    return (0, _List.ofArray)([[makeSymbolRule(delimiter), true], [makeDelimiterRule(delimiter, "\n"), true], [makeRule(function () {
      var delim2 = "\n";
      return function (str) {
        return CommonClassifiers.isDelimitedString(delimiter, delim2, str);
      };
    }(), CommonClassifiers.isAnything), false]]);
  }

  function createDelimitedComment(delim1, delim2) {
    return (0, _List.ofArray)([[makeSymbolRule(delim1), true], [makeSymbolRule(delim2), true], [makeDelimiterRule(delim1, delim2), true], [makeRule(function (str) {
      return CommonClassifiers.isDelimitedString(delim1, delim2, str);
    }, CommonClassifiers.isAnything), false]]);
  }

  function createSymbol(symbol) {
    return (0, _List.ofArray)([[makeSymbolRule(symbol), true], [makeRule(function (y) {
      return symbol === y;
    }, CommonClassifiers.isAnything), false], [makeRule(function (e) {
      return !CommonClassifiers.isPrefix(e, symbol);
    }, function (e_1) {
      return CommonClassifiers.isPrefix(e_1, symbol);
    }), false]]);
  }

  var createStrings = exports.createStrings = (0, _List.ofArray)([[SpecialCases.detectEscapeSequenceDoubleQuote, true], [makeDelimiterRule("\"", "\""), true], [makeRule(function () {
    var delim1 = "\"";
    var delim2 = "\"";
    return function (str) {
      return CommonClassifiers.isDelimitedString(delim1, delim2, str);
    };
  }(), CommonClassifiers.isAnything), false]]);
  var createVariablesAndNumbers = exports.createVariablesAndNumbers = (0, _List.ofArray)([[makeRule(CommonClassifiers.isNumeric, CommonClassifiers.isNumericSuffix), true], [makeRule(CommonClassifiers.isVariable, CommonClassifiers.isVariableSuffix), true], [makeRule(CommonClassifiers.isAnything, CommonClassifiers.isAnything), false]]);
});