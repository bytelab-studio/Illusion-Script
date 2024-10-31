using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class IntExpression : Expression
{
    public override NodeType type => NodeType.INT_EXPRESSION;
    public override TextSpan span => value.span;

    public Token value;

    public IntExpression(Token value)
    {
        this.value = value;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return value;
    }
}