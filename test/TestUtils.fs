module TestUtils
open System.IO

let approval_test fname obj =
  let serialized = sprintf "%A" obj
  let fpath = Path.Combine(__SOURCE_DIRECTORY__, "approval", fname)
  let record = try File.ReadAllText fpath with :? FileNotFoundException -> ""
  if serialized.Split('\n') <> record.Split('\n') then
    printf "Approval test failed on %s:\nExisting:\n%s\n-----\n\nNew:\n%s\n-----"
     fname record serialized
    let rec confirm() =
      printf "\nOverwrite? (y/n) "
      match System.Console.ReadLine() with
      |"y" -> true
      |"n" -> false
      |_ -> confirm()
    if confirm()
     then File.WriteAllText(fpath, serialized)
     else failwith "Approval test failed"
