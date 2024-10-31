using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class ReinterpretationExpression : Expression
{
    public override NodeType type => NodeType.REINTERPRETATION_EXPRESSION;
    public override TextSpan span => TextSpan.Merge(expression.span, clause.span);

    public Expression expression;
    public Token asKeyword;
    public TypeClause clause;
    public ReinterpretationExpression(Expression expression, Token asKeyword, TypeClause clause)
    {
        this.expression = expression;
        this.asKeyword = asKeyword;
        this.clause = clause;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return expression;
        yield return asKeyword;
        yield return clause;
    }
}