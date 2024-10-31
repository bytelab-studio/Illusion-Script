using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes;

public abstract class Node
{
    public abstract NodeType type { get; }
    public abstract TextSpan span { get; }

    public abstract IEnumerable<Node> GetChildren();
}