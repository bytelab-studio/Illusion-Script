using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundConversionExpression : BoundExpression
{
    public override NodeType type => NodeType.CONVERSION_EXPRESSION;
    public override TypeSymbol returnType => targetType;

    public readonly BoundExpression expression;
    public readonly TypeSymbol targetType;
    public readonly bool isPromotion;

    public BoundConversionExpression(BoundExpression expression, TypeSymbol targetType, bool isPromotion)
    {
        this.expression = expression;
        this.targetType = targetType;
        this.isPromotion = isPromotion;
    }
}