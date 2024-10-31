using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Statements;

public sealed class BlockStatement : Statement
{
    public override NodeType type => NodeType.BLOCK_STATEMENT;
    public override TextSpan span => TextSpan.Merge(lBrace.span, rBrace.span);

    public Token lBrace;
    public List<Statement> statements;
    public Token rBrace;

    public BlockStatement(Token lBrace, List<Statement> statements, Token rBrace)
    {
        this.lBrace = lBrace;
        this.statements = statements;
        this.rBrace = rBrace;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return lBrace;
        foreach (Statement statement in statements)
        {
            yield return statement;
        }
        yield return rBrace;
    }
}