using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class IncrementExpression : Expression
{
    public override NodeType type => NodeType.INCREMENT_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(left.span, plusPlusToken.span);

    public Expression left;
    public Token plusPlusToken;

    public IncrementExpression(Expression left, Token plusPlusToken)
    {
        this.left = left;
        this.plusPlusToken = plusPlusToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return left;
        yield return plusPlusToken;
    }
}