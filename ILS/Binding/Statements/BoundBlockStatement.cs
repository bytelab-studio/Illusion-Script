using ILS.Lexing;

namespace ILS.Binding.Statements;

public sealed class BoundBlockStatement : BoundStatement
{
    public override NodeType type => NodeType.BLOCK_STATEMENT;

    public readonly BoundStatement[] statements;

    public BoundBlockStatement(BoundStatement[] statements)
    {
        this.statements = statements;
    }
}