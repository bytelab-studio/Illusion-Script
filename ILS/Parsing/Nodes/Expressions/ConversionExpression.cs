using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class ConversionExpression : Expression
{
    public override NodeType type => NodeType.CONVERSION_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(lAngle.span, expression.span);

    public Token lAngle;
    public TypeClause clause;
    public Token rAngle;
    public Expression expression;

    public ConversionExpression(Token lAngle, TypeClause clause, Token rAngle, Expression expression)
    {
        this.lAngle = lAngle;
        this.clause = clause;
        this.rAngle = rAngle;
        this.expression = expression;
    }
    public override IEnumerable<Node> GetChildren()
    {
        yield return lAngle;
        yield return clause;
        yield return rAngle;
        yield return expression;
    }
}