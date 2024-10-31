using ILS.Lexing;

namespace ILS.Binding.Statements;

public sealed class BoundIfStatement : BoundStatement
{
    public override NodeType type => NodeType.IF_STATEMENT;

    public readonly BoundExpression condition;
    public readonly BoundStatement thenBlock;
    public readonly BoundStatement elseBlock;

    public BoundIfStatement(BoundExpression condition, BoundStatement thenBlock, BoundStatement elseBlock)
    {
        this.condition = condition;
        this.thenBlock = thenBlock;
        this.elseBlock = elseBlock;
    }
}