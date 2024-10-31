using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundAssignmentExpression : BoundExpression
{
    public override NodeType type => NodeType.ASSIGNMENT_EXPRESSION;
    public override TypeSymbol returnType => expression.returnType;
    public readonly BoundExpression fieldExpression;
    public readonly BoundExpression expression;

    public BoundAssignmentExpression(BoundExpression fieldExpression, BoundExpression expression)
    {
        this.fieldExpression = fieldExpression;
        this.expression = expression;
    }
}