define(["exports", "fable-core/umd/Symbol", "fable-core/umd/Util"], function (exports, _Symbol2, _Util) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.op_AmpAmpDot = exports.op_BarBarDot = exports.op_DivideDot = exports.op_EqualsDot = exports.op_GreaterDot = exports.op_LessEqualsDot = exports.op_LessGreaterDot = exports.op_MinusDot = exports.op_MultiplyDot = exports.op_PercentDot = exports.op_PlusDot = exports.Formula = undefined;

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

  var Formula = exports.Formula = function () {
    function Formula(caseName, fields) {
      _classCallCheck(this, Formula);

      this.Case = caseName;
      this.Fields = fields;
    }

    _createClass(Formula, [{
      key: _Symbol3.default.reflection,
      value: function () {
        return {
          type: "Excel_Language.Definitions.Formula",
          interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
          cases: {
            F: []
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

    return Formula;
  }();

  (0, _Symbol2.setType)("Excel_Language.Definitions.Formula", Formula);

  var patternInput_72 = function () {
    var f = function f(_arg2) {
      return function (_arg1) {
        return new Formula("F", []);
      };
    };

    return [f, f, f, f, f, f, f, f, f, f, f];
  }();

  var op_PlusDot = exports.op_PlusDot = patternInput_72[0];
  var op_PercentDot = exports.op_PercentDot = patternInput_72[4];
  var op_MultiplyDot = exports.op_MultiplyDot = patternInput_72[2];
  var op_MinusDot = exports.op_MinusDot = patternInput_72[1];
  var op_LessGreaterDot = exports.op_LessGreaterDot = patternInput_72[7];
  var op_LessEqualsDot = exports.op_LessEqualsDot = patternInput_72[5];
  var op_GreaterDot = exports.op_GreaterDot = patternInput_72[8];
  var op_EqualsDot = exports.op_EqualsDot = patternInput_72[6];
  var op_DivideDot = exports.op_DivideDot = patternInput_72[3];
  var op_BarBarDot = exports.op_BarBarDot = patternInput_72[10];
  var op_AmpAmpDot = exports.op_AmpAmpDot = patternInput_72[9];
});