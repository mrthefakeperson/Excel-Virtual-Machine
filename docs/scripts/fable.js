define(["exports", "./xlvm"], function (exports, _xlvm) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.getById = getById;

  function getById(s) {
    return document.getElementById(s);
  }

  getById("compile").onclick = function (_arg1) {
    var inputCode = getById("input_code").value;
    var stdInput = getById("stdin").value;
    var stdOutput = getById("stdout");
    var output = (0, _xlvm.compileAndRun)(inputCode)(stdInput);
    stdOutput.textContent = output;
    return {};
  };
});