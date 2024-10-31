using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundIntExpression : BoundExpression
{
    public override NodeType type => NodeType.INT_EXPRESSION;
    public override TypeSymbol returnType => TypeSymbol.i32;
    public readonly string value;

    public BoundIntExpression(string value)
    {
        this.value = value;
    }
}