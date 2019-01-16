open Lexer.Main
open Parser.Main
open System.IO

let text = """
#include <stdio.h>

int main () {

   int n[ 10 ]; /* n is an array of 10 integers */
   int i,j;

   /* initialize elements of array n to 0 */
   for ( i = 0; i < 10; i++ ) {
      n[ i ] = i + 100; /* set element at location i to i + 100 */
   }

   /* output each array element's value */
   for (j = 0; j < 10; j++ ) {
      printf("Element[%d] = %d\n", j, n[j] );
   }

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
  //let tokens = tokenize_text text
  //List.iter (printfn "%O") tokens
  //try
  //  let ast = parse_tokens_to_ast tokens
  //  printfn "\n\n\nparse:\n%A" ast
  //with ex -> printfn "%A" ex
  //System.Console.ReadLine() |> ignore

  Directory.GetFiles("../../../samples")
   |> Array.sortBy (fun fname -> int(fname.Replace(".c", "").Replace("../../../samples\\", "")))
   |> Seq.skip 0  // 17
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
