define(["exports", "fable-core/umd/List", "fable-core/umd/String", "fable-core/umd/Seq", "./CompileAndRun"], function (exports, _List, _String, _Seq, _CompileAndRun) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.consoleElement = undefined;
  exports.getById = getById;
  exports.makeTokenGenerator = makeTokenGenerator;
  exports.printToConsole = printToConsole;

  function getById(s) {
    return document.getElementById(s);
  }

  function makeTokenGenerator() {
    var stdinData = getById("stdinData").value;
    var tokenStream = {
      contents: (0, _List.ofArray)((0, _String.split)(stdinData, " "))
    };

    var popStream = function popStream() {
      try {
        return tokenStream.contents.head;
      } finally {
        tokenStream.contents = tokenStream.contents.tail;
      }
    };

    return function (_arg1) {
      switch (_arg1) {
        case "%i":
          tokenStream.contents = (0, _Seq.toList)((0, _Seq.skipWhile)(function ($var326) {
            return function (value) {
              return !value;
            }(function ($var325) {
              return function (tuple) {
                return tuple[0];
              }(function (arg00) {
                var $var324 = Number.parseInt(arg00, 10);
                return isNaN($var324) ? [false, 0] : [true, $var324];
              }($var325));
            }($var326));
          }, tokenStream.contents));
          return popStream(null);

        case "%s":
          return popStream(null);

        default:
          throw new Error("unrecognized or unsupported input format string");
      }
    };
  }

  var consoleElement = exports.consoleElement = getById("consoleContainer");

  function printToConsole(s) {
    consoleElement.textContent = consoleElement.textContent + s;
  }

  getById("compileAndRuttonButton").onclick = function (_arg1) {
    consoleElement.textContent = "";
    (function () {
      var getInput = makeTokenGenerator();
      return function (txt) {
        (0, _CompileAndRun.compileAndRun)(getInput, function (s) {
          printToConsole(s);
        }, txt);
      };
    })()(getById("codeContainer").value);
    return {};
  };
});