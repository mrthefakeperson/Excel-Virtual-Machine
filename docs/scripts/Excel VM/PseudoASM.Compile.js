define(["exports", "./PseudoASM.Definition", "fable-core/umd/List", "./AST.Definition", "fable-core/umd/Seq", "fable-core/umd/Set", "fable-core/umd/GenericComparer", "fable-core/umd/Util", "fable-core/umd/String"], function (exports, _PseudoASM, _List, _AST, _Seq, _Set, _GenericComparer, _Util, _String) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.pushAgain = exports.ldTemp = exports.ldTemp2 = exports.stTemp = exports.stTemp2 = undefined;
  exports.getLocalVariables = getLocalVariables;
  exports.compileASM = compileASM;
  exports.predefined = predefined;
  exports.defineBinaryOperator = defineBinaryOperator;
  exports.definePrint = definePrint;
  exports.CompileToASM = CompileToASM;

  var _List2 = _interopRequireDefault(_List);

  var _GenericComparer2 = _interopRequireDefault(_GenericComparer);

  function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
      default: obj
    };
  }

  var patternInput_5_3 = [new _PseudoASM.PseudoASM("Store", ["!temp"]), new _PseudoASM.PseudoASM("Load", ["!temp"]), new _PseudoASM.PseudoASM("Store", ["!temp2"]), new _PseudoASM.PseudoASM("Load", ["!temp2"])];
  var stTemp2 = exports.stTemp2 = patternInput_5_3[2];
  var stTemp = exports.stTemp = patternInput_5_3[0];
  var ldTemp2 = exports.ldTemp2 = patternInput_5_3[3];
  var ldTemp = exports.ldTemp = patternInput_5_3[1];
  var pushAgain = exports.pushAgain = (0, _List.ofArray)([stTemp, ldTemp, ldTemp]);

  function getLocalVariables(_arg1) {
    if (_arg1.Case === "Define") {
      var yld = {
        contents: new _List2.default()
      };

      var getLocalVariables_1 = function getLocalVariables_1(_arg2) {
        var $var294 = _arg2.Case === "Define" ? _arg2.Fields[0] === _arg1.Fields[0] ? [0, _arg2.Fields[2], _arg2.Fields[1], _arg2.Fields[0]] : [1] : [1];

        switch ($var294[0]) {
          case 0:
            yld.contents = (0, _List.append)($var294[2], yld.contents);
            getLocalVariables_1($var294[1]);
            break;

          case 1:
            var $var295 = _arg2.Case === "Define" ? [0] : _arg2.Case === "Declare" ? [1, _arg2.Fields[1], _arg2.Fields[0]] : _arg2.Case === "Mutate" ? [1, _arg2.Fields[1], _arg2.Fields[0]] : [2];

            switch ($var295[0]) {
              case 0:
                break;

              case 1:
                yld.contents = new _List2.default($var295[2], yld.contents);
                getLocalVariables_1($var295[1]);
                break;

              case 2:
                var activePatternResult989 = (0, _AST.$7C$Children$7C$)(_arg2);
                (0, _Seq.iterate)(getLocalVariables_1, activePatternResult989);
                break;
            }

            break;
        }
      };

      getLocalVariables_1(_arg1);
      return (0, _Seq.toList)((0, _Set.create)(yld.contents, new _GenericComparer2.default(_Util.compare)));
    } else {
      throw new Error("local variables are for functions only");
    }
  }

  function compileASM(redef) {
    var _RedefDetected___ = function _RedefDetected___(construct) {
      return redef(construct);
    };

    return function (_arg1) {
      var activePatternResult1005 = _RedefDetected___(_arg1);

      if (activePatternResult1005 != null) {
        return activePatternResult1005;
      } else {
        var $var296 = _arg1.Case === "Assign" ? [1] : _arg1.Case === "Const" ? [2] : _arg1.Case === "Declare" ? [3] : _arg1.Case === "Define" ? [4] : _arg1.Case === "Get" ? [5] : _arg1.Case === "If" ? [6] : _arg1.Case === "Loop" ? [7] : _arg1.Case === "Mutate" ? [8] : _arg1.Case === "New" ? [9] : _arg1.Case === "Return" ? [10] : _arg1.Case === "Break" ? [10] : _arg1.Case === "Continue" ? [10] : _arg1.Case === "Sequence" ? [11] : _arg1.Case === "Value" ? [12] : [0];

        switch ($var296[0]) {
          case 0:
            var pushSuffixArgs = void 0;
            var loopBody = (0, _List.ofArray)([stTemp, ldTemp, new _PseudoASM.PseudoASM("GetHeap", []), ldTemp, new _PseudoASM.PseudoASM("Push", ["1"]), _PseudoASM.Add]);
            var loopCond = (0, _List.append)(pushAgain, (0, _List.ofArray)([new _PseudoASM.PseudoASM("GetHeap", []), new _PseudoASM.PseudoASM("Push", ["endArr"]), _PseudoASM.Equals]));
            var skipBody = loopBody.length + 2;
            var loopAgain = -(loopCond.length + 1 + loopBody.length);
            pushSuffixArgs = (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("Push", ["1"]), _PseudoASM.Add]), (0, _List.append)(loopCond, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoIfTrueFwdShift", [skipBody])]), (0, _List.append)(loopBody, (0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [loopAgain]), new _PseudoASM.PseudoASM("Pop", [])])))));
            return (0, _List.append)((0, _List.collect)(function (x) {
              return compileASM(redef)(x);
            }, _arg1.Fields[1]), (0, _List.append)(function (x_1) {
              return compileASM(redef)(x_1);
            }(_arg1.Fields[0]), (0, _List.append)((0, _List.ofArray)([stTemp2, ldTemp2]), (0, _List.append)(pushSuffixArgs, (0, _List.ofArray)([ldTemp2, new _PseudoASM.PseudoASM("GetHeap", []), new _PseudoASM.PseudoASM("Call", [])])))));

          case 1:
            return (0, _List.append)(function (x_2) {
              return compileASM(redef)(x_2);
            }(_arg1.Fields[0]), (0, _List.append)(function (x_3) {
              return compileASM(redef)(x_3);
            }(_arg1.Fields[1]), (0, _List.append)((0, _List.ofArray)([_PseudoASM.Add]), (0, _List.append)(function (x_4) {
              return compileASM(redef)(x_4);
            }(_arg1.Fields[2]), (0, _List.append)(pushAgain, (0, _List.ofArray)([stTemp, new _PseudoASM.PseudoASM("WriteHeap", []), ldTemp]))))));

          case 2:
            return (0, _List.ofArray)([new _PseudoASM.PseudoASM("Push", [_arg1.Fields[0]])]);

          case 3:
            return (0, _List.append)(function (x_5) {
              return compileASM(redef)(x_5);
            }(_arg1.Fields[1]), (0, _List.ofArray)([new _PseudoASM.PseudoASM("Store", [_arg1.Fields[0]]), new _PseudoASM.PseudoASM("Load", [_arg1.Fields[0]])]));

          case 4:
            var patternInput = [(0, _List.ofArray)([new _PseudoASM.PseudoASM("Store", ["*call_addr"])]), (0, _List.ofArray)([new _PseudoASM.PseudoASM("Load", ["*call_addr"])])];
            var getArgumentsFromStack = (0, _List.map)(function (arg0) {
              return new _PseudoASM.PseudoASM("Store", [arg0]);
            }, (0, _List.reverse)(_arg1.Fields[1]));
            var patternInput_1 = [(0, _List.ofArray)([new _PseudoASM.PseudoASM("Store", ["!yield"])]), (0, _List.ofArray)([new _PseudoASM.PseudoASM("Load", ["!yield"])])];
            var xs = getLocalVariables(_arg1);
            var getAllLocalsFromStack = (0, _List.map)(function (arg0_1) {
              return new _PseudoASM.PseudoASM("Store", [arg0_1]);
            }, (0, _List.reverse)(xs));
            var storeArgumentsLocally = (0, _List.concat)((0, _List.mapIndexed)(function (i, e) {
              return (0, _List.ofArray)([new _PseudoASM.PseudoASM("Load", [e]), new _PseudoASM.PseudoASM("Store", [String(i) + "-arg"])]);
            }, xs));
            var storeArgumentsOnStackFromLocal = (0, _List.map)(function (i_1) {
              return new _PseudoASM.PseudoASM("Load", [String(i_1) + "-arg"]);
            }, (0, _Seq.toList)((0, _Seq.range)(0, xs.length - 1)));
            var restorePrevArgsAndReturnTopstackValue = (0, _List.append)(patternInput_1[0], (0, _List.append)(getAllLocalsFromStack, (0, _List.append)(patternInput[0], (0, _List.append)(patternInput_1[1], (0, _List.append)(patternInput[1], (0, _List.ofArray)([new _PseudoASM.PseudoASM("Return", [])]))))));

            var redefWithEarlyReturn = function redefWithEarlyReturn(_arg2) {
              if (_arg2.Case === "Return") {
                if (_arg2.Fields[0].Case === "Apply") {
                  var pushB_sThenA = (0, _List.append)((0, _List.collect)(compileASM(redef), _arg2.Fields[0].Fields[1]), compileASM(redef)(_arg2.Fields[0].Fields[0]));
                  var storeB_sAndA = (0, _List.map)(function (i_2) {
                    return new _PseudoASM.PseudoASM("Store", [String(i_2) + "-arg"]);
                  }, (0, _Seq.toList)((0, _Seq.rangeStep)(_arg2.Fields[0].Fields[1].length, -1, 0)));
                  var loadB_sAndA = (0, _List.map)(function (i_3) {
                    return new _PseudoASM.PseudoASM("Load", [String(i_3) + "-arg"]);
                  }, (0, _Seq.toList)((0, _Seq.range)(0, _arg2.Fields[0].Fields[1].length)));
                  var handleCall = (0, _List.append)(pushB_sThenA, (0, _List.append)(storeB_sAndA, (0, _List.append)(getAllLocalsFromStack, (0, _List.append)(patternInput[0], loadB_sAndA))));
                  var unboxA = void 0;
                  var pushSuffixArgs_1 = void 0;
                  var loopBody_1 = (0, _List.ofArray)([stTemp, ldTemp, new _PseudoASM.PseudoASM("GetHeap", []), ldTemp, new _PseudoASM.PseudoASM("Push", ["1"]), _PseudoASM.Add]);
                  var loopCond_1 = (0, _List.append)(pushAgain, (0, _List.ofArray)([new _PseudoASM.PseudoASM("GetHeap", []), new _PseudoASM.PseudoASM("Push", ["endArr"]), _PseudoASM.Equals]));
                  var skipBody_1 = loopBody_1.length + 2;
                  var loopAgain_1 = -(loopCond_1.length + 1 + loopBody_1.length);
                  pushSuffixArgs_1 = (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("Push", ["1"]), _PseudoASM.Add]), (0, _List.append)(loopCond_1, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoIfTrueFwdShift", [skipBody_1])]), (0, _List.append)(loopBody_1, (0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [loopAgain_1]), new _PseudoASM.PseudoASM("Pop", [])])))));
                  unboxA = (0, _List.append)((0, _List.ofArray)([stTemp2, ldTemp2]), (0, _List.append)(pushSuffixArgs_1, (0, _List.ofArray)([ldTemp2, new _PseudoASM.PseudoASM("GetHeap", [])])));
                  return (0, _List.append)(handleCall, (0, _List.append)(unboxA, (0, _List.append)((0, _List.ofArray)([stTemp]), (0, _List.append)(patternInput[1], (0, _List.ofArray)([ldTemp, new _PseudoASM.PseudoASM("Return", [])])))));
                } else {
                  return (0, _List.append)(compileASM(redefWithEarlyReturn)(_arg2.Fields[0]), restorePrevArgsAndReturnTopstackValue);
                }
              } else {
                return redef(_arg2);
              }
            };

            var functionBlock = (0, _List.append)(patternInput[0], (0, _List.append)(storeArgumentsLocally, (0, _List.append)(getArgumentsFromStack, (0, _List.append)(patternInput[1], (0, _List.append)(storeArgumentsOnStackFromLocal, (0, _List.append)(compileASM(redefWithEarlyReturn)(_arg1.Fields[2]), restorePrevArgsAndReturnTopstackValue))))));
            var patternInput_2 = [functionBlock.length + 1, -functionBlock.length];
            var assignFunction = (0, _List.ofArray)([stTemp, new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Store", [_arg1.Fields[0]]), new _PseudoASM.PseudoASM("Load", [_arg1.Fields[0]]), ldTemp, new _PseudoASM.PseudoASM("WriteHeap", []), new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Push", ["endArr"]), new _PseudoASM.PseudoASM("WriteHeap", []), new _PseudoASM.PseudoASM("Load", [_arg1.Fields[0]])]);
            return (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [patternInput_2[0]])]), (0, _List.append)(functionBlock, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("PushFwdShift", [patternInput_2[1]])]), assignFunction)));

          case 5:
            return (0, _List.append)(function (x_6) {
              return compileASM(redef)(x_6);
            }(_arg1.Fields[0]), (0, _List.append)(function (x_7) {
              return compileASM(redef)(x_7);
            }(_arg1.Fields[1]), (0, _List.ofArray)([_PseudoASM.Add, new _PseudoASM.PseudoASM("GetHeap", [])])));

          case 6:
            var patternInput_3 = [function (x_8) {
              return compileASM(redef)(x_8);
            }(_arg1.Fields[1]), function (x_9) {
              return compileASM(redef)(x_9);
            }(_arg1.Fields[2])];
            var patternInput_4 = [patternInput_3[0].length + 1, patternInput_3[1].length + 2];
            return (0, _List.append)(function (x_10) {
              return compileASM(redef)(x_10);
            }(_arg1.Fields[0]), (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoIfTrueFwdShift", [patternInput_4[1]])]), (0, _List.append)(patternInput_3[1], (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [patternInput_4[0]])]), patternInput_3[0]))));

          case 7:
            var compiledCond = function (x_11) {
              return compileASM(redef)(x_11);
            }(_arg1.Fields[0]);

            var pushBranchAddr = (0, _List.ofArray)([new _PseudoASM.PseudoASM("PushFwdShift", [compiledCond.length + 1])]);

            var redefWithBreakCont = function redefWithBreakCont(_arg3) {
              if (_arg3.Case === "Break") {
                return (0, _List.ofArray)([stTemp, ldTemp, new _PseudoASM.PseudoASM("Push", ["False"]), ldTemp, new _PseudoASM.PseudoASM("Return", [])]);
              } else if (_arg3.Case === "Continue") {
                return (0, _List.ofArray)([stTemp, ldTemp, new _PseudoASM.PseudoASM("Push", ["True"]), ldTemp, new _PseudoASM.PseudoASM("Return", [])]);
              } else if (_arg3.Case === "Return") {
                return new _List2.default(new _PseudoASM.PseudoASM("Pop", []), function (x_12) {
                  return compileASM(redef)(x_12);
                }(new _AST.AST("Return", [_arg3.Fields[0]])));
              } else {
                return redef(_arg3);
              }
            };

            var compiledBody = (0, _List.append)(compileASM(redefWithBreakCont)(_arg1.Fields[1]), (0, _List.ofArray)([new _PseudoASM.PseudoASM("Pop", [])]));
            var skipBody_2 = compiledBody.length + 2;
            var loopAgain_2 = -(compiledCond.length + 2 + compiledBody.length);
            return (0, _List.append)(pushBranchAddr, (0, _List.append)(compiledCond, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoIfTrueFwdShift", [2]), new _PseudoASM.PseudoASM("GotoFwdShift", [skipBody_2])]), (0, _List.append)(compiledBody, (0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [loopAgain_2]), new _PseudoASM.PseudoASM("Pop", []), new _PseudoASM.PseudoASM("Push", ["()"])])))));

          case 8:
            return function (x_13) {
              return compileASM(redef)(x_13);
            }(new _AST.AST("Declare", [_arg1.Fields[0], _arg1.Fields[1]]));

          case 9:
            var allocInALoop = void 0;
            var cond = (0, _List.append)(pushAgain, (0, _List.ofArray)([new _PseudoASM.PseudoASM("Push", ["1"]), _PseudoASM.LEq]));
            var body = (0, _List.ofArray)([new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Pop", []), new _PseudoASM.PseudoASM("Push", ["-1"]), _PseudoASM.Add]);
            var skipBody_3 = body.length + 2;
            var loopAgain_3 = -(cond.length + 2 + body.length);
            allocInALoop = (0, _List.append)(cond, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoIfTrueFwdShift", [2]), new _PseudoASM.PseudoASM("GotoFwdShift", [skipBody_3])]), (0, _List.append)(body, (0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [loopAgain_3]), new _PseudoASM.PseudoASM("Pop", [])]))));
            return (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("NewHeap", [])]), (0, _List.append)(pushAgain, (0, _List.append)(function (x_14) {
              return compileASM(redef)(x_14);
            }(_arg1.Fields[0]), (0, _List.append)(pushAgain, (0, _List.append)(allocInALoop, (0, _List.ofArray)([_PseudoASM.Add, new _PseudoASM.PseudoASM("Push", ["endArr"]), new _PseudoASM.PseudoASM("WriteHeap", [])]))))));

          case 10:
            throw new Error("invalid: found return/break/continue in a wrong place");

          case 11:
            var matchValue = (0, _List.map)(function (x_15) {
              return compileASM(redef)(x_15);
            }, _arg1.Fields[0]);

            if (matchValue.tail != null) {
              return (0, _List.concat)(new _List2.default(matchValue.head, (0, _List.map)(function (e_1) {
                return new _List2.default(new _PseudoASM.PseudoASM("Pop", []), e_1);
              }, matchValue.tail)));
            } else {
              return (0, _List.ofArray)([new _PseudoASM.PseudoASM("Push", ["()"])]);
            }

          case 12:
            return (0, _List.ofArray)([new _PseudoASM.PseudoASM("Load", [_arg1.Fields[0]])]);
        }
      }
    };
  }

  function predefined(_arg1) {
    var $var297 = _arg1.Case === "Apply" ? _arg1.Fields[0].Case === "Value" ? _arg1.Fields[0].Fields[0] === "printf" ? _arg1.Fields[1].tail != null ? _arg1.Fields[1].head.Case === "Const" ? _arg1.Fields[1].tail.tail == null ? function () {
      var fmt = _arg1.Fields[1].head.Fields[0];
      return (0, _Seq.exists)(function (y) {
        return fmt === y;
      }, _PseudoASM.allFormatSymbols);
    }() ? [0, _arg1.Fields[1].head.Fields[0]] : [1] : [1] : [1] : [1] : [1] : [1] : [1];

    switch ($var297[0]) {
      case 0:
        return compileASM(function (_arg1_1) {
          return null;
        })(new _AST.AST("Value", ["printf " + $var297[1]]));

      case 1:
        var $var298 = _arg1.Case === "Value" ? _arg1.Fields[0] === "printf" ? [0] : _arg1.Fields[0] === "nothing" ? [2] : [3] : _arg1.Case === "Apply" ? _arg1.Fields[0].Case === "Value" ? _arg1.Fields[0].Fields[0] === "scan" ? _arg1.Fields[1].tail != null ? _arg1.Fields[1].head.Case === "Const" ? _arg1.Fields[1].tail.tail == null ? [1, _arg1.Fields[1].head.Fields[0]] : [3] : _arg1.Fields[1].head.Case === "Value" ? _arg1.Fields[1].tail.tail == null ? [1, _arg1.Fields[1].head.Fields[0]] : [3] : [3] : [3] : [3] : [3] : [3];

        switch ($var298[0]) {
          case 0:
            return compileASM(function (_arg2) {
              return null;
            })(new _AST.AST("Value", ["printf %s"]));

          case 1:
            return (0, _List.ofArray)([new _PseudoASM.PseudoASM("Input", [$var298[1]])]);

          case 2:
            return (0, _List.ofArray)([new _PseudoASM.PseudoASM("Push", ["nothing"])]);

          case 3:
            return null;
        }

    }
  }

  function defineBinaryOperator(op) {
    var functionBlock2Args = (0, _List.ofArray)([stTemp, new _PseudoASM.PseudoASM("Combinator_2", [op.Name, op.Symbol]), ldTemp, new _PseudoASM.PseudoASM("Return", [])]);
    var patternInput = [functionBlock2Args.length + 1, -functionBlock2Args.length - 6];
    var functionBlock1Arg = (0, _List.ofArray)([stTemp, new _PseudoASM.PseudoASM("Store", [op.Symbol + "-1"]), new _PseudoASM.PseudoASM("NewHeap", []), stTemp2, ldTemp2, new _PseudoASM.PseudoASM("PushFwdShift", [patternInput[1]]), new _PseudoASM.PseudoASM("WriteHeap", []), new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Load", [op.Symbol + "-1"]), new _PseudoASM.PseudoASM("WriteHeap", []), new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Push", ["endArr"]), new _PseudoASM.PseudoASM("WriteHeap", []), ldTemp2, ldTemp, new _PseudoASM.PseudoASM("Return", [])]);
    var patternInput_1 = [functionBlock1Arg.length + 1, -functionBlock1Arg.length];
    var assignFunction = void 0;
    var f = op.Symbol;
    assignFunction = (0, _List.ofArray)([stTemp, new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Store", [f]), new _PseudoASM.PseudoASM("Load", [f]), ldTemp, new _PseudoASM.PseudoASM("WriteHeap", []), new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Push", ["endArr"]), new _PseudoASM.PseudoASM("WriteHeap", [])]);
    return (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [patternInput[0]])]), (0, _List.append)(functionBlock2Args, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [patternInput_1[0]])]), (0, _List.append)(functionBlock1Arg, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("PushFwdShift", [patternInput_1[1]])]), assignFunction)))));
  }

  function definePrint(formattedName) {
    var functionBlock = (0, _List.ofArray)([stTemp, new _PseudoASM.PseudoASM("Output", [(0, _String.split)(formattedName, " ")[1]]), new _PseudoASM.PseudoASM("Push", ["()"]), ldTemp, new _PseudoASM.PseudoASM("Return", [])]);
    var patternInput = [functionBlock.length + 1, -functionBlock.length];
    var assignFunction = (0, _List.ofArray)([stTemp, new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Store", [formattedName]), new _PseudoASM.PseudoASM("Load", [formattedName]), ldTemp, new _PseudoASM.PseudoASM("WriteHeap", []), new _PseudoASM.PseudoASM("NewHeap", []), new _PseudoASM.PseudoASM("Push", ["endArr"]), new _PseudoASM.PseudoASM("WriteHeap", [])]);
    return (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("GotoFwdShift", [patternInput[0]])]), (0, _List.append)(functionBlock, (0, _List.append)((0, _List.ofArray)([new _PseudoASM.PseudoASM("PushFwdShift", [patternInput[1]])]), assignFunction)));
  }

  function CompileToASM(ast) {
    return (0, _Seq.ofList)((0, _List.append)((0, _List.collect)(function (c2) {
      return defineBinaryOperator(c2.CommandInfo);
    }, _PseudoASM.allCombinators), (0, _List.append)((0, _List.collect)(function ($var299) {
      return definePrint("printf " + $var299);
    }, _PseudoASM.allFormatSymbols), compileASM(function (_arg1) {
      return predefined(_arg1);
    })(ast))));
  }
});