#load "./parse_fsbuild.fsx"

// module Schema
open Parse_fsbuild.Schema

let fsbuild : fsbuild_schema =
  NamedList(
    required = [
      "output", Value One
      "files", NamedList(
        required = [],
        optional = [
          "source", Value Many
          "main", Value One
          "data", Value Many
          "test", Value One
         ]
       )
     ],
    optional = []
   )

// used to parse targets specified as tests under other modules
let test_fsbuild : fsbuild_schema =
  NamedList(
    required = [
      "output", Value One
      "files", NamedList(
        required = [],
        optional = [
          "main", Value One
          "data", Value Many
         ]
       )
     ],
    optional = []
   )

let fsproject : fsbuild_schema =
  NamedList(
    required = [
      "framework", Value One
      "modules", Value Many
     ],
    optional = [
      "env", Named(Many, Value One)
      "packages", Named(Many, Value One)
      "script_packages", Value Many
      "fsi", Value One
     ]
   )