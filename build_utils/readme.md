build scripts for FAKE

directory structure:
  - project_root
    - project.fsproject
    - fsbuild_generator.fsx
    - build.fsx (references script)
    - src
      - module1
        - module1.fsbuild
        - module1.fsproj (generated)
        - source1.fs
        - source2.fs
        - main.fs
        - data.txt
      - module2
        - ...
    - test
      - module1
        - module1.fsbuild
        - test.fsx
      - module2
        - module2.fsbuild
        - test.fsx
      - added_test_module
        - added_test_module.fsbuild
        - added_test_module.fsproj (generated)
        - test.fsx

## *.fsbuild files

 - for compiling modules and creating targets for scripts
 - generates *.fsproj
 - file paths must be relative (to directory containing *.fsbuild)
 - two space indent, each item must be on its own line

syntax:
```
output:
  exe
files:
  source:
    source1.fs
    source2.fs
  main:
    main.fs
  data:
    data.txt
  test:
    ../test/module1
```
 - builds modules in a linear order
 - project dependencies to all previous modules added automatically
 - external references defined at top level are added automatically
 - output supports [exe | lib | script]
   - main must be defined if exe is specified, and can be defined if script is specified
 - source files are compiled in order (main added to end of source list)
 - scripts are run as part of build target (script outputs require only a main)
 - data is copied to the same directory as compiled assembly
 - if a directory is specified under test, a target will be generated to build that directory
    while loading the compiled module (a dependency will be established for the test module)

## *.fsproject files

 - for linking modules
 - generates build.fsx, associated FAKE commands

syntax:
```
framework:
  netcoreapp2.2
env:
  variable1:
    value1
  variable2:
    value2
modules:
  src/module1
  src/module2
  test/added_test_module
packages:
  // packages here (name : version)
script_packages:
  // paths to *.dll files
fsi:
  // command line for fsi (required when running scripts)
```

 - defines environment variables used in build.fsx
 - builds modules specified in targets (in order)
 - creates additional test targets for modules
 - adds package dependencies for all modules

## main.fsx

 - generates *.fsproj files, interprets data from *.fsproject

## running builds

 - `fake run path/to/main.fsx -t [target_name]`
   - each module `M` specified under targets in build.fsconfig will be a target
   - so will `TestM`
   - so will `All` and `TestAll`
   - so will added test modules
 - run the script directly for persistent builds
 - delete main.fsx.lock to refresh dependencies
 - tested with FAKE 5.20.0