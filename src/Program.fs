open Lexer.Main
open Parser.Main
open System.IO

let text = """
#include <stdio.h>

// Variable declaration:
extern int a, b;
extern int c;
extern float f;

int main () {

   /* variable definition: */
   int a, b;
   int c;
   float f;
 
   /* actual initialization */
   a = 10;
   b = 20;
  
   c = a + b;
   printf("value of c : %d \n", c);

   f = 70.0/3.0;
   printf("value of f : %f \n", f);
 
   retur 0;
}
"""

let text2 = """
#include <stdio.h>

// Variable declaration:
extern int a, b;
extern int c;
extern float f;

int main () {

   retur 0;
}
"""

open Parser.Combinators
[<EntryPoint>]
let main argv =
  //let tokens = tokenize_text text2
  //List.iter (printfn "%O") tokens
  //try
  //  let ast = parse_tokens_to_ast tokens
  //  printfn "\n\n\nparse:\n%A" ast
  //with ex -> printfn "%A" ex
  //System.Console.ReadLine() |> ignore

  Directory.GetFiles("../../../samples")
   |> Array.sortBy (fun fname -> int(fname.Replace(".c", "").Replace("../../../samples\\", "")))
   |> Seq.skip 17
   |> Seq.iter (fun fname ->
        let text = File.ReadAllText fname
        printfn "file: %s\n\ntext:\n%s\n\n\nlex:" fname text
        let tokens = tokenize_text text
        List.iter (printfn "%O") tokens
        let ast = parse_tokens_to_ast tokens
        printfn "\n\n\nparse:\n%A" ast
        System.Console.ReadLine() |> ignore
       )
  0
