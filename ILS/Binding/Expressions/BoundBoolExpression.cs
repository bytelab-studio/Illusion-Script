using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundBoolExpression : BoundExpression
{
    public override NodeType type => NodeType.BOOL_EXPRESSION;
    public override TypeSymbol returnType => TypeSymbol.boolean;
    public readonly NodeType value;

    public BoundBoolExpression(NodeType value)
    {
        this.value = value;
    }
}