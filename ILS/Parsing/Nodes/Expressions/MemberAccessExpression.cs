using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class MemberAccessExpression : Expression
{
    public override NodeType type => NodeType.MEMBER_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(target.span, property.span);

    public readonly Expression target;
    public readonly Token dotToken;
    public readonly Token property;

    public MemberAccessExpression(Expression target, Token dotToken, Token property)
    {
        this.target = target;
        this.dotToken = dotToken;
        this.property = property;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return target;
        yield return dotToken;
        yield return property;
    }
}