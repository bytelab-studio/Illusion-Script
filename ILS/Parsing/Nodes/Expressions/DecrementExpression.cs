using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class DecrementExpression : Expression
{
    public override NodeType type => NodeType.DECREMENT_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(left.span, minusMinusToken.span);

    public Expression left;
    public Token minusMinusToken;

    public DecrementExpression(Expression left, Token minusMinusToken)
    {
        this.left = left;
        this.minusMinusToken = minusMinusToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return left;
        yield return minusMinusToken;
    }
}