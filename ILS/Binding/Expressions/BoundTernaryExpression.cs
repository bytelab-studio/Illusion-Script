using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundTernaryExpression : BoundExpression
{
    public override NodeType type => NodeType.TERNARY_EXPRESSION;
    public override TypeSymbol returnType => thenExpression.returnType;

    public readonly BoundExpression condition;
    public readonly BoundExpression thenExpression;
    public readonly BoundExpression elseExpression;

    public BoundTernaryExpression(BoundExpression condition, BoundExpression thenExpression, BoundExpression elseExpression)
    {
        this.condition = condition;
        this.thenExpression = thenExpression;
        this.elseExpression = elseExpression;
    }
}