using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class BinaryExpression : Expression
{
    public override NodeType type => NodeType.BINARY_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(left.span, right.span);
    public Expression left;
    public Token operatorToken;
    public Expression right;
    public BinaryExpression(Expression left, Token operatorToken, Expression right)
    {
        this.left = left;
        this.operatorToken = operatorToken;
        this.right = right;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return left;
        yield return operatorToken;
        yield return right;
    }
}