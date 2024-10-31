using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundDeReferenceExpression : BoundExpression
{
    public override NodeType type => NodeType.DEREFERENCE_EXPRESSION;
    public override TypeSymbol returnType => TypeSymbol.DeReference(expression.returnType);

    public readonly BoundExpression expression;

    public BoundDeReferenceExpression(BoundExpression expression)
    {
        this.expression = expression;
    }
}