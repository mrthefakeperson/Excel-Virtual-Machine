#load "parse_fsbuild.fsx"
#load "schema.fsx"

// module Generate
open System.IO
open Parse_fsbuild.Accessors

let get_module_name dir_path =
  match Path.GetFileName dir_path with
  | "" -> Path.GetFileName (Path.GetDirectoryName dir_path)
  | name -> name

let read_build_file dir_path extension schema =
  let dir_name = get_module_name dir_path
  let build_file = Path.Combine(dir_path, dir_name + "." + extension)
  try Some (Parse_fsbuild.parse build_file schema)
  with :? FileNotFoundException -> None

type path = string
type module_name = string

let generate_fsproj
  (build_dir : path)
  (data_dir : path)
  (framework : string)
  (packages : Map<string, string>)
  (dependencies : module_name list)
  (module_path : path)
  (fsbuild : Parse_fsbuild.fsbuild)
  (output_exe : bool)
  : path
  =
  let module_name = get_module_name module_path
  let files = access "files" fsbuild
  let data = try_access "data" files |> Option.map get_values |> Option.defaultValue []
  List.iter (fun data_file ->
    let data_path = Path.Combine(module_path, data_file)
    let dest = Path.Combine(data_dir, Path.GetFileName data_path)
    if File.Exists dest then
      failwith "fsbuild error: data file already exists (from another target?)"
    File.Copy(data_path, dest, false)
   ) data
  let source_files = try_access "source" files |> Option.map get_values |> Option.defaultValue []
  let source_files =
    match try_access "main" files with
    | Some main when output_exe -> source_files @ [get_value main]
    | Some _ -> failwith "fsbuild error: main cannot be specified for lib"
    | None when not output_exe -> source_files
    | None -> failwith "fsbuild error: main must be specified for exe"
  if source_files = [] then
    failwith "fsbuild error: no sources to build"
  let output_type_attr = if output_exe then "<OutputType>Exe</OutputType>" else ""
  let target_framework_attr = "<TargetFramework>" + framework + "</TargetFramework>"
  let property_group_attr =
    sprintf """
  <PropertyGroup>
    %s
    %s
  </PropertyGroup>
     """
     output_type_attr
     target_framework_attr
  let source_file_attrs = List.map (sprintf "<Compile Include=%A />") source_files
  let source_item_group_attr =
    sprintf """
  <ItemGroup>
    %s
  </ItemGroup>
     """
     (String.concat "\n    " source_file_attrs)
  let package_attrs =
    List.map (fun (package, version) ->
      sprintf "<PackageReference Include=%A Version=%A />" package version
     ) (Map.toList packages)
  let make_reference_attr dependency =
    // let dependency_dll = Path.Combine(build_dir, dependency + ".dll")
    // sprintf """
    // <Reference Include=%A>
    //   <HintPath>%s</HintPath>
    // </Reference>
    //  """
    //  dependency
    //  dependency_dll
    sprintf """
    <ProjectReference Include=%A />
     """
     (Path.Combine(dependency, get_module_name dependency + ".fsproj"))

  let dependency_attrs = List.map make_reference_attr dependencies
  let dependency_item_group_attr =
    sprintf """
  <ItemGroup>
    %s
%s
  </ItemGroup>
     """
     (String.concat "\n      " package_attrs)
     (String.concat "\n" dependency_attrs)
  let project_attr =
    sprintf """
<Project Sdk="Microsoft.NET.Sdk">
%s
%s
%s
</Project>
     """
     property_group_attr
     source_item_group_attr
     dependency_item_group_attr
  let clean (xml : string) =
    let lines = xml.Trim().Split '\n'
    Seq.filter (fun (line : string) -> line.Trim() <> "") lines
     |> String.concat "\n"
  let fsproj_path = Path.Combine(module_path, module_name + ".fsproj")
  File.WriteAllText(fsproj_path, clean project_attr)
  fsproj_path

type Target_path =
  | Script of main : path
  | Module of fsproj : path

type Target =
  { name : module_name
  ; dependencies : module_name list
  ; target_path : Target_path
  ; test_target_path : Target_path option
  }

let rec make_target
  (build_dir : path)
  (data_dir : path)
  (framework : string)
  (packages : Map<string, string>)
  (dependencies : module_name list)
  (module_path : path)
  (fsbuild : Parse_fsbuild.fsbuild)
  : Target
  =
  let name = get_module_name module_path
  let files = access "files" fsbuild
  let target_path =
    match get_value (access "output" fsbuild) with
    | "script" ->
      let main = Path.Combine(module_path, get_value (access "main" files))
      Script main
    | "exe" | "lib" as output_type ->
      let fsproj_path =
        generate_fsproj
         build_dir data_dir framework packages dependencies module_path fsbuild
         (output_type = "exe")
      Module fsproj_path
    | _ -> failwith "fsbuild error: output type must be exe, lib, or script"

  let dependencies = List.map get_module_name dependencies
  let test_target_path =
    try_access "test" files
     |> Option.map (fun test_path ->
          let test_path = Path.Combine(module_path, get_value test_path)
          let test_fsbuild =
            match read_build_file test_path "fsbuild" Schema.test_fsbuild with
            | Some fsbuild -> fsbuild
            | None -> failwithf "test module %s not found" test_path
          printfn "\ntest module %s:\n%A" test_path test_fsbuild
          let test_target =
            make_target build_dir data_dir framework packages dependencies test_path test_fsbuild
          test_target.target_path
         )
  { name = name
  ; dependencies = dependencies
  ; target_path = target_path
  ; test_target_path = test_target_path }

type Project =
  { build_dir : path
  ; bin_dir : path
  ; data_dir : path
  ; framework : string
  ; script_packages : string list
  ; fsi_command : string
  ; targets : Target list
  ; env : Map<string, string>
  }

let generate_project_targets (root_path : path) : Project =
  let build_dir = Path.Combine(root_path, "build")
  let data_dir = Path.Combine(root_path, "data")
  let project =
    match read_build_file root_path "fsproject" Schema.fsproject with
    | Some project -> project
    | None -> failwith "project file not found"
  printfn "project:\n%A" project
  let framework = get_value (access "framework" project)
  let packages =
    try_access "packages" project |> Option.map get_value_map |> Option.defaultValue Map.empty
  let modules =
    get_values (access "modules" project)
     |> List.map (fun module_path ->
          let full_module_path = Path.Combine(root_path, module_path)
          match read_build_file full_module_path "fsbuild" Schema.fsbuild with
          | Some fsbuild -> full_module_path, fsbuild
          | None -> failwithf "module %s not found" module_path
         )
  try Directory.Delete(data_dir, true)
  with :? DirectoryNotFoundException -> ()
  ignore (Directory.CreateDirectory data_dir)
  let targets, _ =
    List.mapFold (fun deps (module_path, fsbuild) ->
      printfn "\nmodule %s:\n%A" module_path fsbuild
      let target = make_target build_dir data_dir framework packages deps module_path fsbuild
      let deps =
        match target.target_path with
        | Script _ -> deps
        // | Module _ -> get_module_name module_path :: deps
        | Module _ -> module_path :: deps
      target, deps
     ) [] modules
  { build_dir = build_dir
  ; bin_dir = Path.Combine(root_path, "bin")
  ; data_dir = data_dir
  ; framework = framework
  ; script_packages =
      try_access "script_packages" project |> Option.map get_values |> Option.defaultValue []
  ; fsi_command = try_access "fsi" project |> Option.map get_value |> Option.defaultValue "fsi"
  ; targets = targets
  ; env = try_access "env" project |> Option.map get_value_map |> Option.defaultValue Map.empty
  }