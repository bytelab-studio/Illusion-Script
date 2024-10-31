using System.Collections.Generic;
using System.Linq;
using ILS.Parsing.Nodes;

namespace ILS.Lexing;

public sealed class Token : Node
{
    public override NodeType type { get; }
    public string text;
    public override TextSpan span { get; }

    public Token(NodeType type, TextSpan span, string text)
    {
        this.type = type;
        this.text = text;
        this.span = span;
    }
    public override IEnumerable<Node> GetChildren()
    {
        return Enumerable.Empty<Node>();
    }
}