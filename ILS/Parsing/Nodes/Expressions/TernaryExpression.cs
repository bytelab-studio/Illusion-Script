using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class TernaryExpression : Expression
{
    public override NodeType type => NodeType.TERNARY_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(condition.span, elseExpression.span);

    public Expression condition;
    public Token questionToken;
    public Expression thenExpression;
    public Token colonToken;
    public Expression elseExpression;

    public TernaryExpression(Expression condition, Token questionToken, Expression thenExpression, Token colonToken, Expression elseExpression)
    {
        this.condition = condition;
        this.questionToken = questionToken;
        this.thenExpression = thenExpression;
        this.colonToken = colonToken;
        this.elseExpression = elseExpression;
    }
    public override IEnumerable<Node> GetChildren()
    {
        yield return condition;
        yield return questionToken;
        yield return thenExpression;
        yield return colonToken;
        yield return elseExpression;
    }
}