using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes;

public sealed class SyntaxTree
{
    public List<Member> root;
    public Token eof;
    public DiagnosticBag diagnostics;

    public SyntaxTree(DiagnosticBag diagnostics, List<Member> root, Token eof)
    {
        this.root = root;
        this.eof = eof;
        this.diagnostics = diagnostics;
    }
}