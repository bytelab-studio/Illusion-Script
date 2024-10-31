using ILS.Lexing;

namespace ILS.Binding;

public abstract class BoundNode
{
    public abstract NodeType type { get; }
}