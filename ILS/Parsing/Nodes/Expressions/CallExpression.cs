using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class CallExpression : Expression
{
    public override NodeType type => NodeType.CALL_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(callee.span, rParen.span);

    public Expression callee;
    public Token lParen;
    public List<Expression> arguments;
    public Token rParen;

    public CallExpression(Expression callee, Token lParen, List<Expression> arguments, Token rParen)
    {
        this.callee = callee;
        this.lParen = lParen;
        this.arguments = arguments;
        this.rParen = rParen;
    }
    public override IEnumerable<Node> GetChildren()
    {
        yield return callee;
        yield return lParen;
        foreach (Expression argument in arguments)
        {
            yield return argument;
        }
        yield return rParen;
    }
}