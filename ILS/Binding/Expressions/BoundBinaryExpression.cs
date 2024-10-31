using ILS.Binding.Operation;
using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundBinaryExpression : BoundExpression
{
    public override NodeType type => NodeType.BINARY_EXPRESSION;
    public override TypeSymbol returnType => binaryOperator.resultType;
    public readonly BoundExpression left;
    public readonly BoundBinaryOperator binaryOperator;
    public readonly BoundExpression right;

    public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator binaryOperator, BoundExpression right)
    {
        this.left = left;
        this.binaryOperator = binaryOperator;
        this.right = right;
    }
}