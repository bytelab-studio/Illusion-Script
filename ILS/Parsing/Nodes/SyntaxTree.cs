using ILS.Lexing;

namespace ILS.Parsing.Nodes;

public sealed class SyntaxTree
{
    public Statement root;
    public Token eof;
    public DiagnosticBag diagnostics;

    public SyntaxTree(DiagnosticBag diagnostics, Statement root, Token eof)
    {
        this.root = root;
        this.eof = eof;
        this.diagnostics = diagnostics;
    }
}