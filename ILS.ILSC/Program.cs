using System;
using System.Linq;
using ILS.Lexing;
using ILS.Parsing;
using ILS.Parsing.Nodes;

namespace ILS.ILSC;

public static class Program
{
    public static void Main(string[] args)
    {
        string code = """
                      {
                        
                        let a: u16 = 20;
                        let b: u8 = 30;
                        while (b != 0) {
                            if (b == 2) {
                                break;
                            }
                            b = b - 1;
                        }
                        let c: u32 = a + b;
                      }
                      """;
        Console.WriteLine(code);
        Parser parser = new Parser(code);
        SyntaxTree syntaxTree = parser.Parse();
        CompilationResult result = new Compilation(syntaxTree).Emit(Console.Out);
        if (result.Diagnostics().Any())
        {
            foreach (Diagnostic diagnostic in result.Diagnostics())
            {
                Console.Write(diagnostic.span.lineStart);
                Console.Write(":");
                Console.Write(diagnostic.span.colStart);
                Console.Write(": ");
                Console.WriteLine(diagnostic.message);
            }
        }
    }
}