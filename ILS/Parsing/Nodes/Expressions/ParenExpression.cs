using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class ParenExpression : Expression
{
    public override NodeType type => NodeType.PAREN_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(lParen.span, rParen.span);
    public Token lParen;
    public Expression expression;
    public Token rParen;

    public ParenExpression(Token lParen, Expression expression, Token rParen)
    {
        this.lParen = lParen;
        this.expression = expression;
        this.rParen = rParen;
    }
    public override IEnumerable<Node> GetChildren()
    {
        yield return lParen;
        yield return expression;
        yield return rParen;
    }
}