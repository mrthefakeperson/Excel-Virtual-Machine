define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "../build-docs/Excel Language", "fable-core/umd/List", "fable-core/umd/Seq", "fable-core/umd/String"], function (exports, _Symbol2, _Util, _ExcelLanguage, _List, _Seq, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.compile$27$ = exports.$7C$Inline$7C$_$7C$ = exports.operationsPrefix = exports.type_int32 = exports.type_string = exports.allTypes = exports.allComb2Sections = exports.x = exports.Add = exports.Equals = exports.Greater = exports.LEq = exports.Mod = exports.allCombinators = exports.C2_Mod = exports.C2_Greater = exports.C2_LEq = exports.C2_Equals = exports.C2_Add = exports.PseudoAsm = exports.comb2 = undefined;
  exports.createComb2Section = createComb2Section;
  exports.getSectionAddress = getSectionAddress;
  exports.getSectionAddressFromCmd = getSectionAddressFromCmd;
  exports.getSectionAddressFromInfix = getSectionAddressFromInfix;
  exports._PrintAddress = _PrintAddress;
  exports.compile = compile;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

  var _List2 = _interopRequireDefault(_List);

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

  var comb2 = exports.comb2 = function () {
    _createClass(comb2, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "ASM_Compiler.comb2",
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

  (0, _Symbol2.setType)("ASM_Compiler.comb2", comb2);

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
          type: "ASM_Compiler.PseudoAsm",
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

  (0, _Symbol2.setType)("ASM_Compiler.PseudoAsm", PseudoAsm);

  var C2_Add = exports.C2_Add = function (_comb) {
    _inherits(C2_Add, _comb);

    _createClass(C2_Add, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Add, {
          type: "ASM_Compiler.C2_Add",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Add(name, symbol) {
      _classCallCheck(this, C2_Add);

      var _this = _possibleConstructorReturn(this, (C2_Add.__proto__ || Object.getPrototypeOf(C2_Add)).call(this, name, symbol));

      return _this;
    }

    _createClass(C2_Add, [{
      key: "Interpret",
      value: function (a, b) {
        return String(Number.parseInt(a) + Number.parseInt(b));
      }
    }, {
      key: "CreateFormula",
      value: function (a, b) {
        return (0, _ExcelLanguage.op_PlusDot)(a)(b);
      }
    }]);

    return C2_Add;
  }(comb2);

  (0, _Symbol2.setType)("ASM_Compiler.C2_Add", C2_Add);

  var C2_Equals = exports.C2_Equals = function (_comb2) {
    _inherits(C2_Equals, _comb2);

    _createClass(C2_Equals, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Equals, {
          type: "ASM_Compiler.C2_Equals",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Equals(name, symbol) {
      _classCallCheck(this, C2_Equals);

      var _this2 = _possibleConstructorReturn(this, (C2_Equals.__proto__ || Object.getPrototypeOf(C2_Equals)).call(this, name, symbol));

      return _this2;
    }

    _createClass(C2_Equals, [{
      key: "Interpret",
      value: function (a, b) {
        return String(a === b);
      }
    }, {
      key: "CreateFormula",
      value: function (a, b) {
        return (0, _ExcelLanguage.op_EqualsDot)(a)(b);
      }
    }]);

    return C2_Equals;
  }(comb2);

  (0, _Symbol2.setType)("ASM_Compiler.C2_Equals", C2_Equals);

  var C2_LEq = exports.C2_LEq = function (_comb3) {
    _inherits(C2_LEq, _comb3);

    _createClass(C2_LEq, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_LEq, {
          type: "ASM_Compiler.C2_LEq",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_LEq(name, symbol) {
      _classCallCheck(this, C2_LEq);

      var _this3 = _possibleConstructorReturn(this, (C2_LEq.__proto__ || Object.getPrototypeOf(C2_LEq)).call(this, name, symbol));

      return _this3;
    }

    _createClass(C2_LEq, [{
      key: "Interpret",
      value: function (a, b) {
        if (function () {
          var $var259 = Number.parseInt(a, 10);
          return isNaN($var259) ? [false, 0] : [true, $var259];
        }()[0] ? function () {
          var $var260 = Number.parseInt(b, 10);
          return isNaN($var260) ? [false, 0] : [true, $var260];
        }()[0] : false) {
          return String(Number.parseInt(a) <= Number.parseInt(b));
        } else {
          return String(a <= b);
        }
      }
    }, {
      key: "CreateFormula",
      value: function (a, b) {
        return (0, _ExcelLanguage.op_LessEqualsDot)(a)(b);
      }
    }]);

    return C2_LEq;
  }(comb2);

  (0, _Symbol2.setType)("ASM_Compiler.C2_LEq", C2_LEq);

  var C2_Greater = exports.C2_Greater = function (_comb4) {
    _inherits(C2_Greater, _comb4);

    _createClass(C2_Greater, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Greater, {
          type: "ASM_Compiler.C2_Greater",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Greater(name, symbol) {
      _classCallCheck(this, C2_Greater);

      var _this4 = _possibleConstructorReturn(this, (C2_Greater.__proto__ || Object.getPrototypeOf(C2_Greater)).call(this, name, symbol));

      return _this4;
    }

    _createClass(C2_Greater, [{
      key: "Interpret",
      value: function (a, b) {
        if (function () {
          var $var261 = Number.parseInt(a, 10);
          return isNaN($var261) ? [false, 0] : [true, $var261];
        }()[0] ? function () {
          var $var262 = Number.parseInt(b, 10);
          return isNaN($var262) ? [false, 0] : [true, $var262];
        }()[0] : false) {
          return String(Number.parseInt(a) > Number.parseInt(b));
        } else {
          return String(a > b);
        }
      }
    }, {
      key: "CreateFormula",
      value: function (a, b) {
        return (0, _ExcelLanguage.op_GreaterDot)(a)(b);
      }
    }]);

    return C2_Greater;
  }(comb2);

  (0, _Symbol2.setType)("ASM_Compiler.C2_Greater", C2_Greater);

  var C2_Mod = exports.C2_Mod = function (_comb5) {
    _inherits(C2_Mod, _comb5);

    _createClass(C2_Mod, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(C2_Mod, {
          type: "ASM_Compiler.C2_Mod",
          interfaces: [],
          properties: {}
        });
      }
    }]);

    function C2_Mod(name, symbol) {
      _classCallCheck(this, C2_Mod);

      var _this5 = _possibleConstructorReturn(this, (C2_Mod.__proto__ || Object.getPrototypeOf(C2_Mod)).call(this, name, symbol));

      return _this5;
    }

    _createClass(C2_Mod, [{
      key: "Interpret",
      value: function (a, b) {
        return String(Number.parseInt(a) % Number.parseInt(b));
      }
    }, {
      key: "CreateFormula",
      value: function (a, b) {
        return (0, _ExcelLanguage.op_PercentDot)(a)(b);
      }
    }]);

    return C2_Mod;
  }(comb2);

  (0, _Symbol2.setType)("ASM_Compiler.C2_Mod", C2_Mod);
  var allCombinators = exports.allCombinators = (0, _List.map)(function (arg0) {
    return new PseudoAsm("Combinator_2", [arg0]);
  }, (0, _List.ofArray)([new C2_Add("add", "+"), new C2_Equals("equals", "="), new C2_LEq("leq", "<="), new C2_Greater("greater", ">"), new C2_Mod("mod", "%")]));
  var patternInput_65_1 = allCombinators;

  var matchResultHolder_65 = function () {
    var $var263 = patternInput_65_1.tail != null ? patternInput_65_1.tail.tail != null ? patternInput_65_1.tail.tail.tail != null ? patternInput_65_1.tail.tail.tail.tail != null ? patternInput_65_1.tail.tail.tail.tail.tail != null ? patternInput_65_1.tail.tail.tail.tail.tail.tail == null ? [0] : [1] : [1] : [1] : [1] : [1] : [1];

    switch ($var263[0]) {
      case 0:
        break;

      case 1:
        throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/ASM Compiler.fs", 65, 4);
        break;
    }
  }();

  var Mod = exports.Mod = patternInput_65_1.tail.tail.tail.tail.head;
  var LEq = exports.LEq = patternInput_65_1.tail.tail.head;
  var Greater = exports.Greater = patternInput_65_1.tail.tail.tail.head;
  var Equals = exports.Equals = patternInput_65_1.tail.head;
  var Add = exports.Add = patternInput_65_1.head;
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
  var patternInput_85_2 = allTypes;

  var matchResultHolder_85_1 = function () {
    var $var264 = patternInput_85_2.tail != null ? patternInput_85_2.tail.tail != null ? patternInput_85_2.tail.tail.tail == null ? [0] : [1] : [1] : [1];

    switch ($var264[0]) {
      case 0:
        break;

      case 1:
        throw new Error("C:\\Users\\Thefak\\Documents\\Visual Studio 2013\\Projects\\Excel VM\\build-docs\\../Excel VM/ASM Compiler.fs", 85, 4);
        break;
    }
  }();

  var type_string = exports.type_string = patternInput_85_2.tail.head;
  var type_int32 = exports.type_int32 = patternInput_85_2.head;
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
    var $var265 = _arg1.Case === "Value" ? (0, _Seq.exists)(function (_arg2) {
      return _arg2.Case === "Combinator_2" ? _arg2.Fields[0].Symbol === _arg1.Fields[0] : false;
    }, allCombinators) ? [0, _arg1.Fields[0]] : [1] : [1];

    switch ($var265[0]) {
      case 0:
        return (0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("Push", [String(getSectionAddressFromInfix($var265[1]))]), new PseudoAsm("WriteHeap", []), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Popv", [x])]);

      case 1:
        var $var266 = _arg1.Case === "Value" ? _arg1.Fields[0] === "ignore" ? [0] : _arg1.Fields[0] === "printf" ? [2] : _arg1.Fields[0] === "nothing" ? [4] : [5] : _arg1.Case === "Apply" ? _arg1.Fields[0].Case === "Value" ? _arg1.Fields[0].Fields[0] === "printf" ? _arg1.Fields[1].tail != null ? _arg1.Fields[1].head.Case === "Const" ? _arg1.Fields[1].head.Fields[0] === "%i" ? _arg1.Fields[1].tail.tail == null ? [1] : [5] : _arg1.Fields[1].head.Fields[0] === "%s" ? _arg1.Fields[1].tail.tail == null ? [2] : [5] : [5] : [5] : [5] : _arg1.Fields[0].Fields[0] === "scan" ? _arg1.Fields[1].tail != null ? _arg1.Fields[1].head.Case === "Const" ? _arg1.Fields[1].tail.tail == null ? [3, _arg1.Fields[1].head.Fields[0]] : [5] : _arg1.Fields[1].head.Case === "Value" ? _arg1.Fields[1].tail.tail == null ? [3, _arg1.Fields[1].head.Fields[0]] : [5] : [5] : [5] : [5] : [5] : [5];

        switch ($var266[0]) {
          case 0:
            return (0, _List.ofArray)([new PseudoAsm("Push", ["not implemented"])]);

          case 1:
            return (0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("Push", [String(_PrintAddress(type_int32))]), new PseudoAsm("WriteHeap", []), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Popv", [x])]);

          case 2:
            return (0, _List.ofArray)([new PseudoAsm("NewHeap", []), new PseudoAsm("Store", [x]), new PseudoAsm("Load", [x]), new PseudoAsm("Push", [String(_PrintAddress(type_string))]), new PseudoAsm("WriteHeap", []), new PseudoAsm("NewHeap", []), new PseudoAsm("Push", ["endArr"]), new PseudoAsm("WriteHeap", []), new PseudoAsm("Load", [x]), new PseudoAsm("Popv", [x])]);

          case 3:
            return (0, _List.ofArray)([new PseudoAsm("Input", [$var266[1]])]);

          case 4:
            return (0, _List.ofArray)([new PseudoAsm("Push", ["nothing"])]);

          case 5:
            return null;
        }

    }
  }

  exports.$7C$Inline$7C$_$7C$ = _Inline___;

  function compile_(inScope, _arg3) {
    var activePatternResult982 = _Inline___(_arg3);

    if (activePatternResult982 != null) {
      return activePatternResult982;
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
        var $var267 = _arg4[0].tail != null ? _arg4[0].head.Case === "Pop" ? [0, _arg4[0].tail, _arg4[1]] : [1] : [1];

        switch ($var267[0]) {
          case 0:
            return (0, _List.append)((0, _List.reverse)($var267[1]), (0, _List.map)(function (arg0_3) {
              return new PseudoAsm("Popv", [arg0_3]);
            }, $var267[2]));

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
});