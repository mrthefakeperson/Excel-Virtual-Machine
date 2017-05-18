define(["exports", "fable-core/umd/List", "fable-core/umd/String", "fable-core/umd/Seq", "./CompileAndRun"], function (exports, _List, _String, _Seq, _CompileAndRun) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.spreadsheetPanel = exports.samplesPanel = exports.spreadsheetCell = exports.compileSpreadsheet = exports.compileConsole = exports.consoleElement = exports.stdinElement = undefined;
  exports.getById = getById;
  exports.makeTokenGenerator = makeTokenGenerator;
  exports.printToConsole = printToConsole;
  exports.printToSheet = printToSheet;
  exports.updateSheet = updateSheet;

  function getById(s) {
    return document.getElementById(s);
  }

  var stdinElement = exports.stdinElement = getById("stdinData");
  var consoleElement = exports.consoleElement = getById("consoleContainer");
  var compileConsole = exports.compileConsole = getById("compileAndRunButton");
  var compileSpreadsheet = exports.compileSpreadsheet = getById("compileToSpreadsheetButton");

  var spreadsheetCell = exports.spreadsheetCell = function spreadsheetCell(s) {
    return getById(s);
  };

  var samplesPanel = exports.samplesPanel = getById("samples");
  var spreadsheetPanel = exports.spreadsheetPanel = getById("ExcelSheet");

  function makeTokenGenerator() {
    var stdinData = stdinElement.value;
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
          tokenStream.contents = (0, _Seq.toList)((0, _Seq.skipWhile)(function ($var327) {
            return function (value) {
              return !value;
            }(function ($var326) {
              return function (tuple) {
                return tuple[0];
              }(function (arg00) {
                var $var325 = Number.parseInt(arg00, 10);
                return isNaN($var325) ? [false, 0] : [true, $var325];
              }($var326));
            }($var327));
          }, tokenStream.contents));
          return popStream(null);

        case "%s":
          return popStream(null);

        default:
          throw new Error("unrecognized or unsupported input format string");
      }
    };
  }

  function printToConsole(s) {
    consoleElement.value = consoleElement.value + s;
  }

  compileConsole.onclick = function (_arg4) {
    consoleElement.value = "> Excel_VM code.c\n> code\n";
    (function () {
      var act = function act(_arg3) {
        return function (_arg2) {
          return function (_arg1) {};
        };
      };

      var getInput = makeTokenGenerator();
      return function (txt) {
        (0, _CompileAndRun.compileAndRun)(act, getInput, function (s) {
          printToConsole(s);
        }, txt);
      };
    })()(getById("codeContainer").value);
    return {};
  };

  function printToSheet(s) {
    spreadsheetCell("B2").textContent = spreadsheetCell("B2").textContent + s;
  }

  function updateSheet(stacks, heap, cmd) {
    spreadsheetCell("A2").textContent = stdinElement.value;
    var instr = stacks.get("A").contents.head;
    spreadsheetCell("A3").textContent = String((Number.parseInt(instr) - 1) * 2);
    spreadsheetCell("A4").textContent = String(Number.parseInt(instr) * 2);
    var patternInput = cmd.CommandInfo.StringPair;
    spreadsheetCell("A5").textContent = patternInput[0];
    spreadsheetCell("A6").textContent = patternInput[1];
    var matchValue = stacks.get("B").contents;

    if (matchValue.tail == null) {
      spreadsheetCell("C3").textContent = "";
    } else {
      spreadsheetCell("C3").textContent = matchValue.head;
    }

    spreadsheetCell("C4").textContent = String(stacks.get("B").contents.length + 1);
    (0, _Seq.iterateIndexed)(function (i, e) {
      if (5 + i < 20) {
        spreadsheetCell("C" + String(5 + i)).textContent = e;
      }
    }, stacks.get("B").contents);

    if (heap.length === 0) {
      spreadsheetCell("B3").textContent = "";
    } else {
      spreadsheetCell("B3").textContent = heap[heap.length - 1];
    }

    spreadsheetCell("B4").textContent = String(heap.length + 1);
    (0, _Seq.iterateIndexed)(function (i_1, e_1) {
      if (5 + i_1 < 20) {
        spreadsheetCell("B" + String(5 + i_1)).textContent = e_1;
      }
    }, heap);

    var delay = function delay(_arg1) {
      if (_arg1 === 0) {} else {
        delay(_arg1 - 1);
      }
    };

    delay(0);
  }

  compileSpreadsheet.onclick = function (_arg1_1) {
    samplesPanel.style.display = "none";
    spreadsheetPanel.style.display = "block";

    for (var e = 1; e <= 19; e++) {
      spreadsheetCell("A" + String(e)).textContent = "";
      spreadsheetCell("B" + String(e)).textContent = "";
      spreadsheetCell("C" + String(e)).textContent = "";
    }

    spreadsheetCell("B2").textContent = "stdout:\n";
    consoleElement.value = "> Excel_VM code.c -outputExcelFile\n";
    (function () {
      var act_1 = function act_1(stacks) {
        return function (heap) {
          return function (cmd) {
            updateSheet(stacks, heap, cmd);
          };
        };
      };

      var getInput_1 = makeTokenGenerator();
      return function (txt_1) {
        (0, _CompileAndRun.compileAndRun)(act_1, getInput_1, function (s_1) {
          printToSheet(s_1);
        }, txt_1);
      };
    })()(getById("codeContainer").value);
    return {};
  };
});