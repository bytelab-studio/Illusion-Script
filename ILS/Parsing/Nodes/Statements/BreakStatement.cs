using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class BreakStatement : Statement
{
    public override NodeType type => NodeType.BREAK_STATEMENT;
    public override TextSpan span => TextSpan.Merge(breakKeyword.span, semicolonToken.span);

    public Token breakKeyword;
    public Token semicolonToken;

    public BreakStatement(Token breakKeyword, Token semicolonToken)
    {
        this.breakKeyword = breakKeyword;
        this.semicolonToken = semicolonToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return breakKeyword;
        yield return semicolonToken;
    }
}