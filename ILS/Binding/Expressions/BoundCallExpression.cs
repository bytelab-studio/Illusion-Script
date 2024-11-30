using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundCallExpression : BoundExpression
{
    public override NodeType type => NodeType.CALL_EXPRESSION;
    public override TypeSymbol returnType => callee.returnType.generics[callee.returnType.generics.Length - 1];

    public readonly BoundExpression callee;
    public readonly BoundExpression[] arguments;
    public BoundCallExpression(BoundExpression callee, BoundExpression[] arguments)
    {
        this.callee = callee;
        this.arguments = arguments;
    }
}