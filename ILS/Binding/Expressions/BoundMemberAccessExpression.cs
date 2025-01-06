using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundMemberAccessExpression : BoundExpression
{
    public override NodeType type => NodeType.MEMBER_EXPRESSION;
    public override TypeSymbol returnType => item.type;

    public readonly BoundExpression target;
    public readonly StructItemSymbol item;
    public readonly int offset;

    public BoundMemberAccessExpression(BoundExpression target, StructItemSymbol item, int offset)
    {
        this.target = target;
        this.item = item;
        this.offset = offset;
    }
}