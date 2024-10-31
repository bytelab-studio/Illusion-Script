using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Statements;

public sealed class ExpressionStatement : Statement
{
    public override NodeType type => NodeType.EXPRESSION_STATEMENT;
    public override TextSpan span => TextSpan.Merge(expression.span, semicolonToken.span);

    public Expression expression;
    public Token semicolonToken;

    public ExpressionStatement(Expression expression, Token semicolonToken)
    {
        this.expression = expression;
        this.semicolonToken = semicolonToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return expression;
    }
}