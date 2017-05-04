define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util", "fable-core/umd/String", "fable-core/umd/List"], function (exports, _Symbol2, _Util, _String, _List) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.stringFormat = exports.allFormatSymbols = exports.allCombinators = exports.And = exports.Or = exports.Equals = exports.GEq = exports.Greater = exports.LEq = exports.Less = exports.NotEq = exports.Add = exports.Div = exports.Mod = exports.Mul = exports.Sub = exports.PseudoASM = exports.Comb2 = exports.Cmd = undefined;

  var _Symbol3 = _interopRequireDefault(_Symbol2);

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

  function _defineProperty(obj, key, value) {
    if (key in obj) {
      Object.defineProperty(obj, key, {
        value: value,
        enumerable: true,
        configurable: true,
        writable: true
      });
    } else {
      obj[key] = value;
    }

    return obj;
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

  var Cmd = exports.Cmd = function () {
    _createClass(Cmd, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "PseudoASM.Definition.Cmd",
          properties: _defineProperty({
            Name: "string",
            StringPair: (0, _Util.Tuple)(["string", "string"])
          }, "StringPair", (0, _Util.Tuple)(["string", "string"]))
        };
      }
    }]);

    function Cmd(name, arg) {
      _classCallCheck(this, Cmd);

      this.name = name;
      this.arg = arg;
    }

    _createClass(Cmd, [{
      key: "ToString",
      value: function () {
        return (0, _String.fsFormat)("%O ( %O )")(function (x) {
          return x;
        })(this.name)(this.arg);
      }
    }, {
      key: "Name",
      get: function () {
        return this.name;
      }
    }, {
      key: "StringPair",
      get: function () {
        var $var292 = this.arg != null ? typeof this.arg === "string" ? [0, this.arg] : [1] : [1];

        switch ($var292[0]) {
          case 0:
            return [this.name, $var292[1]];

          case 1:
            var $var293 = this.arg != null ? typeof this.arg === "number" ? [0, this.arg] : [1] : [1];

            switch ($var293[0]) {
              case 0:
                return [this.name, String($var293[1])];

              case 1:
                if (this.arg != null) {
                  return (0, _String.fsFormat)("argument type not recognized: %O")(function (x) {
                    throw new Error(x);
                  })(this.arg);
                } else {
                  return [this.name, ""];
                }

            }

        }
      }
    }]);

    return Cmd;
  }();

  (0, _Symbol2.setType)("PseudoASM.Definition.Cmd", Cmd);

  var Comb2 = exports.Comb2 = function (_Cmd) {
    _inherits(Comb2, _Cmd);

    _createClass(Comb2, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return (0, _Util.extendInfo)(Comb2, {
          type: "PseudoASM.Definition.Comb2",
          interfaces: [],
          properties: {
            StringPair: (0, _Util.Tuple)(["string", "string"]),
            Symbol: "string"
          }
        });
      }
    }]);

    function Comb2(name, symbol) {
      _classCallCheck(this, Comb2);

      var _this = _possibleConstructorReturn(this, (Comb2.__proto__ || Object.getPrototypeOf(Comb2)).call(this, name, ""));

      _this.name = name;
      _this.symbol = symbol;
      return _this;
    }

    _createClass(Comb2, [{
      key: "StringPair",
      get: function () {
        return [this.name, ""];
      }
    }, {
      key: "Symbol",
      get: function () {
        return this.symbol;
      }
    }]);

    return Comb2;
  }(Cmd);

  (0, _Symbol2.setType)("PseudoASM.Definition.Comb2", Comb2);

  var PseudoASM = exports.PseudoASM = function () {
    function PseudoASM(caseName, fields) {
      _classCallCheck(this, PseudoASM);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(PseudoASM, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "PseudoASM.Definition.PseudoASM",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            Call: [],
            Combinator_2: ["string", "string"],
            GetHeap: [],
            GotoFwdShift: ["number"],
            GotoIfTrueFwdShift: ["number"],
            Input: ["string"],
            Load: ["string"],
            NewHeap: [],
            Output: ["string"],
            Pop: [],
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
    }, {
      key: "CompareTo",
      value: function (other) {
        return (0, _Util.compareUnions)(this, other);
      }
    }, {
      key: "CommandInfo",
      get: function () {
        if (this.Case === "PushFwdShift") {
          return new Cmd("PushFwdShift", this.Fields[0]);
        } else if (this.Case === "Pop") {
          return new Cmd("Pop");
        } else if (this.Case === "Store") {
          return new Cmd("Store", this.Fields[0]);
        } else if (this.Case === "Load") {
          return new Cmd("Load", this.Fields[0]);
        } else if (this.Case === "GotoFwdShift") {
          return new Cmd("GotoFwdShift", this.Fields[0]);
        } else if (this.Case === "GotoIfTrueFwdShift") {
          return new Cmd("GotoIfTrueFwdShift", this.Fields[0]);
        } else if (this.Case === "Call") {
          return new Cmd("Call");
        } else if (this.Case === "Return") {
          return new Cmd("Return");
        } else if (this.Case === "NewHeap") {
          return new Cmd("NewHeap");
        } else if (this.Case === "GetHeap") {
          return new Cmd("GetHeap");
        } else if (this.Case === "WriteHeap") {
          return new Cmd("WriteHeap");
        } else if (this.Case === "Input") {
          return new Cmd("Input", this.Fields[0]);
        } else if (this.Case === "Output") {
          return new Cmd("Output", this.Fields[0]);
        } else if (this.Case === "Combinator_2") {
          return new Comb2(this.Fields[0], this.Fields[1]);
        } else {
          return new Cmd("Push", this.Fields[0]);
        }
      }
    }]);

    return PseudoASM;
  }();

  (0, _Symbol2.setType)("PseudoASM.Definition.PseudoASM", PseudoASM);

  function c2(arg0, arg1) {
    return new PseudoASM("Combinator_2", [arg0, arg1]);
  }

  var patternInput_55 = [c2("Add", "+"), c2("Sub", "-"), c2("Mul", "*"), c2("Div", "/"), c2("Mod", "%")];
  var Sub = exports.Sub = patternInput_55[1];
  var Mul = exports.Mul = patternInput_55[2];
  var Mod = exports.Mod = patternInput_55[4];
  var Div = exports.Div = patternInput_55[3];
  var Add = exports.Add = patternInput_55[0];
  var patternInput_57_1 = [c2("Equals", "="), c2("Greater", ">"), c2("Less", "<"), c2("NotEq", "<>"), c2("GEq", ">="), c2("LEq", "<=")];
  var NotEq = exports.NotEq = patternInput_57_1[3];
  var Less = exports.Less = patternInput_57_1[2];
  var LEq = exports.LEq = patternInput_57_1[5];
  var Greater = exports.Greater = patternInput_57_1[1];
  var GEq = exports.GEq = patternInput_57_1[4];
  var Equals = exports.Equals = patternInput_57_1[0];
  var patternInput_60_2 = [c2("And", "&&"), c2("Or", "||")];
  var Or = exports.Or = patternInput_60_2[1];
  var And = exports.And = patternInput_60_2[0];
  var allCombinators = exports.allCombinators = (0, _List.ofArray)([Add, Sub, Mul, Div, Mod, Less, LEq, Equals, NotEq, Greater, GEq, And, Or]);
  var allFormatSymbols = exports.allFormatSymbols = (0, _List.ofArray)(["%i", "%s"]);
  var stringFormat = exports.stringFormat = "%s";
});