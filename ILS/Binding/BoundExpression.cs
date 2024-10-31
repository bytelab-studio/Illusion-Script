using ILS.Binding.Symbols;

namespace ILS.Binding;

public abstract class BoundExpression : BoundNode
{
    public abstract TypeSymbol returnType { get; }
}