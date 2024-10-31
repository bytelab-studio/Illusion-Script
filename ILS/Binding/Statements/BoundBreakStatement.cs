using ILS.Lexing;

namespace ILS.Binding.Statements;

public sealed class BoundBreakStatement : BoundStatement
{
    public override NodeType type => NodeType.BREAK_STATEMENT;
}