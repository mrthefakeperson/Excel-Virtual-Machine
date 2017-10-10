build\fslex.exe lexer.fsl -o build\lex.fs --unicode
fsc build\lex.fs -r build\Lexing.dll --target:library