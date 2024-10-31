using ILS.Lexing;

namespace ILS.Binding.Statements;

public sealed class BoundContinueStatement : BoundStatement
{
    public override NodeType type => NodeType.CONTINUE_STATEMENT;
}