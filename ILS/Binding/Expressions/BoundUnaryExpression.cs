using ILS.Binding.Operation;
using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundUnaryExpression : BoundExpression
{
    public override NodeType type => NodeType.UNARY_EXPRESSION;
    public override TypeSymbol returnType => unaryOperator.resultType;

    public readonly BoundUnaryOperator unaryOperator;
    public readonly BoundExpression right;

    public BoundUnaryExpression(BoundUnaryOperator unaryOperator, BoundExpression right)
    {
        this.unaryOperator = unaryOperator;
        this.right = right;
    }
}