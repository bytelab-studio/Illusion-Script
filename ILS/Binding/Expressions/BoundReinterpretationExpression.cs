using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundReinterpretationExpression : BoundExpression
{
    public override NodeType type => NodeType.REINTERPRETATION_EXPRESSION;
    public override TypeSymbol returnType => targetType;

    public readonly BoundExpression expression;
    public readonly TypeSymbol targetType;

    public BoundReinterpretationExpression(BoundExpression expression, TypeSymbol targetType)
    {
        this.expression = expression;
        this.targetType = targetType;
    }
}