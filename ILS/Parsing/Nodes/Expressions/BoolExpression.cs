using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class BoolExpression : Expression
{
    public override NodeType type => NodeType.BOOL_EXPRESSION;
    public override TextSpan span => value.span;
    public Token value;

    public BoolExpression(Token value)
    {
        this.value = value;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return value;
    }
}