define(["exports", "../build-docs/unused modules/UnusedModule2", "./Parser.CParser", "./Parser.TypeValidation"], function (exports, _UnusedModule, _Parser, _Parser2) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.fromStringRunUntilParsed = fromStringRunUntilParsed;
  exports.fromString = fromString;

  function fromStringRunUntilParsed(txt, args) {
    var languageParser = void 0;
    var matchValue = args.get("language");

    switch (matchValue) {
      case "F#":
        languageParser = function languageParser(s) {
          return (0, _UnusedModule.parseSyntax)(s);
        };

        break;

      case "C":
        languageParser = function languageParser(e) {
          return (0, _Parser.parseSyntax)(e);
        };

        break;

      default:
        throw new Error("error: defined language does not exist");
    }

    return languageParser(txt).Clean();
  }

  function fromString(txt, args) {
    return function (e) {
      return [e.Clean(), args];
    }((0, _Parser2.validateTypes)(fromStringRunUntilParsed(txt, args)));
  }
});