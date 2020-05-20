// module Parse_fsbuild
open System.IO

module Schema =
  type value_schema =
    | One
    | Many
    | Fixed_value of string

  type fsbuild_schema =
    | Value of value_schema
    | Named of value_schema * fsbuild_schema
    | NamedList of
        required : (string * fsbuild_schema) list
         * optional : (string * fsbuild_schema) list

type fsbuild =
  | Value of string
  | Named of string * fsbuild
  | List of fsbuild list
  | NamedList of (string * fsbuild) list

type parsed_line =
  { value : string
  ; indent : int
  }

let extract_label line =
  if (line.value + " ").[line.value.Length - 1] = ':'
   then Some line.value.[..line.value.Length - 2]
   else None

let all_same_indent lines =
  match lines with
  | [] -> true
  | hd :: tl -> List.forall (fun line -> line.indent = hd.indent) tl

let group_by_indent lines =
  let rec take_while_indented = function
    | line :: rest ->
      let is_indented inner_line = line.indent < inner_line.indent
      let indented = Seq.takeWhile is_indented rest |> List.ofSeq
      let not_indented = Seq.skipWhile is_indented rest |> List.ofSeq
      (line :: indented) :: take_while_indented not_indented
    | [] -> []
  let items = take_while_indented lines
  let header_lines = List.map List.head items
  if all_same_indent header_lines
   then Some items
   else None

module S = Schema

let rec parse_lines
  (expected_structure : Schema.fsbuild_schema)
  (lines : parsed_line list)
  : fsbuild =

  let error message =
    let lines_string = String.concat "\n" (List.map (fun line -> line.value) lines)
    failwithf "fsbuild error: %s\nexpected structure: %A\nlines:\n%s"
     message expected_structure lines_string

  match expected_structure with
  | S.Value S.One ->
    match lines with
    | [ line ] -> Value line.value
    | _ -> error "expected one value, got zero or many"
  | S.Value S.Many ->
    if not (all_same_indent lines) then
      error "wrong indentation in value list"
    let values = List.map (fun line -> Value line.value) lines
    List values
  | S.Value (S.Fixed_value constant) ->
    match lines with
    | [ line ] when line.value = constant -> Value constant
    | [ _ ] -> error "expected fixed value, got a different value"
    | _ -> error "expected one value, got zero or many"
  | S.Named(S.One | S.Fixed_value _ as name_schema, substructure) ->
    match lines with
    | line :: rest ->
      let name =
        match extract_label line with
        | Some name -> name
        | None -> error "expected a named line (ending with ':')"
      match name_schema with
      | S.Fixed_value constant when name <> constant ->
        error (sprintf "expected fixed name %s, got a different name" constant)
      | _ -> ()
      let all_indented =
        List.forall (fun inner_line -> line.indent < inner_line.indent) rest
      if not all_indented then
        error "in named field, expected all items to be indented"
      let parsed_substructure = parse_lines substructure rest
      Named(name, parsed_substructure)
    | [] -> error "expected a named line, got nothing"
  | S.Named(S.Many, substructure) ->
    let groups =
      match group_by_indent lines with
      | Some groups -> groups
      | None -> error "wrong indentation in list"
    let fields =
      List.map (fun lines ->
        parse_lines (S.Named(S.One, substructure)) lines
       ) groups
    List fields
  | S.NamedList(required, optional) ->
    let groups =
      match group_by_indent lines with
      | Some groups -> groups
      | None -> error "wrong indentation in list"
    let parse_map =
      List.fold (fun acc_map (name, substructure) ->
        let structure = S.Named(S.Fixed_value name, substructure)
        Map.add name structure acc_map
       ) Map.empty (required @ optional)
    let required_fields = List.map fst required
    let parsed_fields, parsed =
      List.map (fun lines ->
        let name =
          match extract_label (List.head lines) with
          | Some name -> name
          | None -> error "expected a named line (ending with ':')"
        let parsed =
          match Map.tryFind name parse_map with
          | Some substructure -> parse_lines substructure lines
          | None -> error (sprintf "unrecognized field in list: %s" name)
        name, parsed
       ) groups
       |> List.unzip
    let parsed_field_set = Set.ofList parsed_fields
    if List.length parsed_fields <> Set.count parsed_field_set then
      error "duplicate field in list"
    let missing_required_field =
      List.tryFind (fun field -> not (Set.contains field parsed_field_set)) required_fields
    match missing_required_field with
    | Some field -> error (sprintf "required field missing: %s" field)
    | None -> ()
    let parsed =
      List.map (function
        | Named(name, substructure) -> (name, substructure)
        | _ -> failwith "should never be reached"
       ) parsed
    NamedList parsed

let parse filepath (structure : S.fsbuild_schema) : fsbuild =
  let lines = File.ReadAllLines filepath |> Array.toList
  let lines =
    List.choose (fun (line : string) ->
      let value = line.TrimStart()
      let indent = line.Length - value.Length
      let value = value.Trim()
      if value <> "" && (value + "  ").[..1] <> "//"
       then Some { value = value; indent = indent }
       else None
     ) lines
  parse_lines structure lines

module Accessors =
  let try_access field (fsbuild : fsbuild) : fsbuild option =
    match fsbuild with
    | Value _ -> None
    | Named(name, substructure) when name = field -> Some substructure
    | Named(_, _) -> None
    | List _ -> None
    | NamedList pairs ->
      List.tryPick (fun (name, substructure) ->
        if name = field then Some substructure else None
       ) pairs

  let access field (fsbuild : fsbuild) : fsbuild =
    match try_access field fsbuild with
    | Some result -> result
    | None ->
      failwithf "fsbuild error: field %s does not exist\nstructure: %A" field fsbuild

  let get_value (fsbuild : fsbuild) : string =
    match fsbuild with
    | Value x -> x
    | _ -> failwith "fsbuild error: not a value"

  let get_values (fsbuild : fsbuild) : string list =
    match fsbuild with
    | List values -> List.map get_value values
    | _ -> failwith "fsbuild error: not a value list"

  let get_value_map (fsbuild : fsbuild) : Map<string, string> =
    match fsbuild with
    | List pairs ->
      List.map (function
        | Named(key, Value value) -> key, value
        | _ -> failwith "fsbuild error: item in list is not a key-value pair"
       ) pairs
       |> Map.ofList
    | _ -> failwith "fsbuild error: not a key-value list"