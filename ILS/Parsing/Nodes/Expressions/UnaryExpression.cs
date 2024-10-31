using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class UnaryExpression : Expression
{
    public override NodeType type => NodeType.UNARY_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(operatorToken.span, right.span);
    public Token operatorToken;
    public Expression right;

    public UnaryExpression(Token operatorToken, Expression right)
    {
        this.operatorToken = operatorToken;
        this.right = right;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return operatorToken;
        yield return right;
    }
}