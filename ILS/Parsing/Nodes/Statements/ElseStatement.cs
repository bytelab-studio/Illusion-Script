using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Statements;

public sealed class ElseStatement : Statement
{
    public override NodeType type => NodeType.ELSE_STATEMENT;
    public override TextSpan span => TextSpan.Merge(elseKeyword.span, body.span);

    public Token elseKeyword;
    public Statement body;
    public ElseStatement(Token elseKeyword, Statement body)
    {
        this.elseKeyword = elseKeyword;
        this.body = body;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return elseKeyword;
        yield return body;
    }
}