using ILS.Lexing;

namespace ILS.Binding.Statements;

public sealed class BoundWhileStatement : BoundStatement
{
    public override NodeType type => NodeType.WHILE_STATEMENT;

    public readonly BoundExpression condition;
    public readonly BoundStatement body;

    public BoundWhileStatement(BoundExpression condition, BoundStatement body)
    {
        this.condition = condition;
        this.body = body;
    }
}