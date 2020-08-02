`fake build -t [target]`

`fake build` to list targets

target `All` builds all components
target `TestAll` runs tests

targets are divided into two categories:
  - libraries:
    - include datatypes and utility functions
    - contain a `Libname.fsproj`
    - build `Libname.dll`
  - compiler stages:
    - transform an input datatype to an output datatype
    - are composed to implement the compiler
    - contain a `Main.fs`, a `Main.fsproj`, and optionally a `Test.fsx`
    - build `Stagename.dll` (run with `dotnet Stagename.dll`)
    - executables contain basic tests
    - `fake run build_utils/build.fsx -t TestSamples`, then `dotnet build/TestSamples.dll` to run stages on code samples found in `Utils/TestUtils/Samples`
      - requires a file named `.env.fsx` containing a path to fsi, fuchu, and a bash command

ionide tips:
  - open this folder in VSCode
  - open a file from one of the targets
  - wait for the project to load (indicator at bottom, clicking it seems to help)
  - if the target depends on others, build the DLLs for those targets
  - `fsi build_utils/main.fsx -restore; fsi build_utils/main.fsx -clean; fake run build_utils/build.fsx -t All` seems to sometimes fix intellisense errors