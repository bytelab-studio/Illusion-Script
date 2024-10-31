using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class NameExpression : Expression
{
    public override NodeType type => NodeType.NAME_EXPRESSION;
    public override TextSpan span => value.span;

    public Token value;

    public NameExpression(Token value)
    {
        this.value = value;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return value;
    }
}