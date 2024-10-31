using ILS.Lexing;

namespace ILS.Binding.Statements;

public sealed class BoundExpressionStatement : BoundStatement
{
    public override NodeType type => NodeType.EXPRESSION_STATEMENT;

    public readonly BoundExpression expression;

    public BoundExpressionStatement(BoundExpression expression)
    {
        this.expression = expression;
    }
}