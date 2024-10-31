using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class AssignmentExpression : Expression
{
    public override NodeType type => NodeType.ASSIGNMENT_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(fieldExpression.span, expression.span);
    public Expression fieldExpression;
    public Token equalsToken;
    public Expression expression;

    public AssignmentExpression(Expression fieldExpression, Token equalsToken, Expression expression)
    {
        this.fieldExpression = fieldExpression;
        this.equalsToken = equalsToken;
        this.expression = expression;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return fieldExpression;
        yield return equalsToken;
        yield return expression;
    }
}