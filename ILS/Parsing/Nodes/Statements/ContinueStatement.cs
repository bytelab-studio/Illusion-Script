using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Expressions;

public sealed class ContinueStatement : Statement
{
    public override NodeType type => NodeType.CONTINUE_STATEMENT;
    public override TextSpan span => TextSpan.Merge(continueKeyword.span, semicolonToken.span);

    public Token continueKeyword;
    public Token semicolonToken;

    public ContinueStatement(Token continueKeyword, Token semicolonToken)
    {
        this.continueKeyword = continueKeyword;
        this.semicolonToken = semicolonToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return continueKeyword;
        yield return semicolonToken;
    }
}