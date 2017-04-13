import { NonDeclaredType, getPropertyNames, getDefinition } from "./Util";
import List from "./List";
import FSymbol from "./Symbol";
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
export { MemberInfo };
export function resolveGeneric(idx, enclosing) {
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
            else if (resolved instanceof NonDeclaredType && resolved.kind === "GenericParam") {
                return resolveGeneric(resolved.definition, enclosing.tail);
            }
            else {
                return new List(resolved, enclosing);
            }
        }
    }
    catch (err) {
        throw new Error("Cannot resolve generic argument " + idx + ": " + err);
    }
}
export function getType(obj) {
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
export function getTypeFullName(typ, option) {
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
    else if (typ instanceof NonDeclaredType) {
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
        return trim(typeof proto[FSymbol.reflection] === "function"
            ? proto[FSymbol.reflection]().type : null, option);
    }
}
export function getName(x) {
    if (x instanceof MemberInfo) {
        return x.name;
    }
    return getTypeFullName(x, "name");
}
export function getPrototypeOfType(typ) {
    if (typeof typ === "string") {
        return null;
    }
    else if (typ instanceof NonDeclaredType) {
        return typ.kind === "GenericType" ? typ.definition.prototype : null;
    }
    else {
        return typ.prototype;
    }
}
export function getProperties(typ) {
    var proto = getPrototypeOfType(typ);
    if (proto != null && typeof proto[FSymbol.reflection] === "function") {
        var info_1 = proto[FSymbol.reflection]();
        if (info_1.properties) {
            return Object.getOwnPropertyNames(info_1.properties)
                .map(function (k, i) { return new MemberInfo(k, i, typ, info_1.properties[k]); });
        }
    }
    throw new Error("Type " + getTypeFullName(typ) + " doesn't contain property info.");
}
export function getUnionCases(typ) {
    var proto = getPrototypeOfType(typ);
    if (proto != null && typeof proto[FSymbol.reflection] === "function") {
        var info_2 = proto[FSymbol.reflection]();
        if (info_2.cases) {
            return Object.getOwnPropertyNames(info_2.cases)
                .map(function (k, i) { return new MemberInfo(k, i, typ, null, info_2.cases[k]); });
        }
    }
    throw new Error("Type " + getTypeFullName(typ) + " doesn't contain union case info.");
}
export function getPropertyValues(obj) {
    return getPropertyNames(obj).map(function (k) { return obj[k]; });
}
export function getUnionFields(obj, typ) {
    if (obj != null && typeof obj[FSymbol.reflection] === "function") {
        var info = obj[FSymbol.reflection]();
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
export function makeUnion(caseInfo, args) {
    var Cons = getDefinition(caseInfo.declaringType);
    return new (Cons.bind.apply(Cons, [void 0, caseInfo.name].concat(args)))();
}
export function getTupleElements(typ) {
    if (typ instanceof NonDeclaredType && typ.kind === "Tuple") {
        return typ.generics;
    }
    throw new Error("Type " + getTypeFullName(typ) + " is not a tuple type.");
}
export function isTupleType(typ) {
    if (typ instanceof NonDeclaredType) {
        return typ.kind === "Tuple";
    }
    return false;
}
