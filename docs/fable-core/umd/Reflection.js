(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports", "./Util", "./List", "./Symbol"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Util_1 = require("./Util");
    var List_1 = require("./List");
    var Symbol_1 = require("./Symbol");
    var MemberInfo = (function () {
        function MemberInfo(name, index, declaringType, propertyType, unionFields) {
            this.name = name;
            this.index = index;
            this.declaringType = declaringType;
            this.propertyType = propertyType;
            this.unionFields = unionFields;
        }
        MemberInfo.prototype.getUnionFields = function () {
            var _this = this;
            return this.unionFields.map(function (fi, i) { return new MemberInfo("unknown", i, _this.declaringType, fi); });
        };
        return MemberInfo;
    }());
    exports.MemberInfo = MemberInfo;
    function resolveGeneric(idx, enclosing) {
        try {
            var t = enclosing.head;
            if (t.generics == null) {
                return resolveGeneric(idx, enclosing.tail);
            }
            else {
                var name_1 = typeof idx === "string"
                    ? idx : Object.getOwnPropertyNames(t.generics)[idx];
                var resolved = t.generics[name_1];
                if (resolved == null) {
                    return resolveGeneric(idx, enclosing.tail);
                }
                else if (resolved instanceof Util_1.NonDeclaredType && resolved.kind === "GenericParam") {
                    return resolveGeneric(resolved.definition, enclosing.tail);
                }
                else {
                    return new List_1.default(resolved, enclosing);
                }
            }
        }
        catch (err) {
            throw new Error("Cannot resolve generic argument " + idx + ": " + err);
        }
    }
    exports.resolveGeneric = resolveGeneric;
    function getType(obj) {
        var t = typeof obj;
        switch (t) {
            case "boolean":
            case "number":
            case "string":
            case "function":
                return t;
            default:
                return Object.getPrototypeOf(obj).constructor;
        }
    }
    exports.getType = getType;
    function getTypeFullName(typ, option) {
        function trim(fullName, option) {
            if (typeof fullName !== "string") {
                return "unknown";
            }
            if (option === "name") {
                var i = fullName.lastIndexOf('.');
                return fullName.substr(i + 1);
            }
            if (option === "namespace") {
                var i = fullName.lastIndexOf('.');
                return i > -1 ? fullName.substr(0, i) : "";
            }
            return fullName;
        }
        if (typeof typ === "string") {
            return typ;
        }
        else if (typ instanceof Util_1.NonDeclaredType) {
            switch (typ.kind) {
                case "Unit":
                    return "unit";
                case "Option":
                    return getTypeFullName(typ.generics, option) + " option";
                case "Array":
                    return getTypeFullName(typ.generics, option) + "[]";
                case "Tuple":
                    return typ.generics.map(function (x) { return getTypeFullName(x, option); }).join(" * ");
                case "GenericParam":
                case "Interface":
                    return typ.definition;
                case "GenericType":
                    return getTypeFullName(typ.definition, option);
                case "Any":
                default:
                    return "unknown";
            }
        }
        else {
            var proto = typ.prototype;
            return trim(typeof proto[Symbol_1.default.reflection] === "function"
                ? proto[Symbol_1.default.reflection]().type : null, option);
        }
    }
    exports.getTypeFullName = getTypeFullName;
    function getName(x) {
        if (x instanceof MemberInfo) {
            return x.name;
        }
        return getTypeFullName(x, "name");
    }
    exports.getName = getName;
    function getPrototypeOfType(typ) {
        if (typeof typ === "string") {
            return null;
        }
        else if (typ instanceof Util_1.NonDeclaredType) {
            return typ.kind === "GenericType" ? typ.definition.prototype : null;
        }
        else {
            return typ.prototype;
        }
    }
    exports.getPrototypeOfType = getPrototypeOfType;
    function getProperties(typ) {
        var proto = getPrototypeOfType(typ);
        if (proto != null && typeof proto[Symbol_1.default.reflection] === "function") {
            var info_1 = proto[Symbol_1.default.reflection]();
            if (info_1.properties) {
                return Object.getOwnPropertyNames(info_1.properties)
                    .map(function (k, i) { return new MemberInfo(k, i, typ, info_1.properties[k]); });
            }
        }
        throw new Error("Type " + getTypeFullName(typ) + " doesn't contain property info.");
    }
    exports.getProperties = getProperties;
    function getUnionCases(typ) {
        var proto = getPrototypeOfType(typ);
        if (proto != null && typeof proto[Symbol_1.default.reflection] === "function") {
            var info_2 = proto[Symbol_1.default.reflection]();
            if (info_2.cases) {
                return Object.getOwnPropertyNames(info_2.cases)
                    .map(function (k, i) { return new MemberInfo(k, i, typ, null, info_2.cases[k]); });
            }
        }
        throw new Error("Type " + getTypeFullName(typ) + " doesn't contain union case info.");
    }
    exports.getUnionCases = getUnionCases;
    function getPropertyValues(obj) {
        return Util_1.getPropertyNames(obj).map(function (k) { return obj[k]; });
    }
    exports.getPropertyValues = getPropertyValues;
    function getUnionFields(obj, typ) {
        if (obj != null && typeof obj[Symbol_1.default.reflection] === "function") {
            var info = obj[Symbol_1.default.reflection]();
            if (info.cases) {
                var uci = null, cases = Object.getOwnPropertyNames(info.cases);
                for (var i = 0; i < cases.length; i++) {
                    if (cases[i] === obj.Case) {
                        uci = new MemberInfo(cases[i], i, typ, null, info.cases[cases[i]]);
                        break;
                    }
                }
                if (uci != null) {
                    return [uci, obj.Fields];
                }
            }
        }
        throw new Error("Not an F# union type.");
    }
    exports.getUnionFields = getUnionFields;
    function makeUnion(caseInfo, args) {
        var Cons = Util_1.getDefinition(caseInfo.declaringType);
        return new (Cons.bind.apply(Cons, [void 0, caseInfo.name].concat(args)))();
    }
    exports.makeUnion = makeUnion;
    function getTupleElements(typ) {
        if (typ instanceof Util_1.NonDeclaredType && typ.kind === "Tuple") {
            return typ.generics;
        }
        throw new Error("Type " + getTypeFullName(typ) + " is not a tuple type.");
    }
    exports.getTupleElements = getTupleElements;
    function isTupleType(typ) {
        if (typ instanceof Util_1.NonDeclaredType) {
            return typ.kind === "Tuple";
        }
        return false;
    }
    exports.isTupleType = isTupleType;
});
