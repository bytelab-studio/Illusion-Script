using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundErrorExpression : BoundExpression
{
    public override NodeType type => NodeType.ERROR_EXPRESSION;
    public override TypeSymbol returnType => TypeSymbol.error;
}