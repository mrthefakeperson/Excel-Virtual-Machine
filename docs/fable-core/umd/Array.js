(function (factory) {
    if (typeof module === "object" && typeof module.exports === "object") {
        var v = factory(require, exports);
        if (v !== undefined) module.exports = v;
    }
    else if (typeof define === "function" && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    function addRangeInPlace(range, xs) {
        var iter = range[Symbol.iterator]();
        var cur = iter.next();
        while (!cur.done) {
            xs.push(cur.value);
            cur = iter.next();
        }
    }
    exports.addRangeInPlace = addRangeInPlace;
    function copyTo(source, sourceIndex, target, targetIndex, count) {
        while (count--)
            target[targetIndex++] = source[sourceIndex++];
    }
    exports.copyTo = copyTo;
    function partition(f, xs) {
        var ys = [], zs = [], j = 0, k = 0;
        for (var i = 0; i < xs.length; i++)
            if (f(xs[i]))
                ys[j++] = xs[i];
            else
                zs[k++] = xs[i];
        return [ys, zs];
    }
    exports.partition = partition;
    function permute(f, xs) {
        var ys = xs.map(function () { return null; });
        var checkFlags = new Array(xs.length);
        for (var i = 0; i < xs.length; i++) {
            var j = f(i);
            if (j < 0 || j >= xs.length)
                throw new Error("Not a valid permutation");
            ys[j] = xs[i];
            checkFlags[j] = 1;
        }
        for (var i = 0; i < xs.length; i++)
            if (checkFlags[i] != 1)
                throw new Error("Not a valid permutation");
        return ys;
    }
    exports.permute = permute;
    function removeInPlace(item, xs) {
        var i = xs.indexOf(item);
        if (i > -1) {
            xs.splice(i, 1);
            return true;
        }
        return false;
    }
    exports.removeInPlace = removeInPlace;
    function setSlice(target, lower, upper, source) {
        var length = (upper || target.length - 1) - lower;
        if (ArrayBuffer.isView(target) && source.length <= length)
            target.set(source, lower);
        else
            for (var i = lower | 0, j = 0; j <= length; i++, j++)
                target[i] = source[j];
    }
    exports.setSlice = setSlice;
    function sortInPlaceBy(f, xs, dir) {
        if (dir === void 0) { dir = 1; }
        return xs.sort(function (x, y) {
            x = f(x);
            y = f(y);
            return (x < y ? -1 : x == y ? 0 : 1) * dir;
        });
    }
    exports.sortInPlaceBy = sortInPlaceBy;
    function unzip(xs) {
        var bs = new Array(xs.length), cs = new Array(xs.length);
        for (var i = 0; i < xs.length; i++) {
            bs[i] = xs[i][0];
            cs[i] = xs[i][1];
        }
        return [bs, cs];
    }
    exports.unzip = unzip;
    function unzip3(xs) {
        var bs = new Array(xs.length), cs = new Array(xs.length), ds = new Array(xs.length);
        for (var i = 0; i < xs.length; i++) {
            bs[i] = xs[i][0];
            cs[i] = xs[i][1];
            ds[i] = xs[i][2];
        }
        return [bs, cs, ds];
    }
    exports.unzip3 = unzip3;
    function getSubArray(xs, startIndex, count) {
        return xs.slice(startIndex, startIndex + count);
    }
    exports.getSubArray = getSubArray;
    function fill(target, targetIndex, count, value) {
        target.fill(value, targetIndex, targetIndex + count);
    }
    exports.fill = fill;
});
