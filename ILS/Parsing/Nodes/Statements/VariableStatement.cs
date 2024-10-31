using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Statements;

public sealed class VariableStatement : Statement
{
    public override NodeType type => NodeType.VARIABLE_STATEMENT;
    public override TextSpan span => TextSpan.Merge(keywordToken.span, semicolonToken.span);

    public Token keywordToken;
    public Token identifierToken;
    public TypeClause typeClause;
    public Token equalsToken;
    public Expression initializer;
    public Token semicolonToken;

    public VariableStatement(Token keywordToken, Token identifierToken, TypeClause typeClause, Token equalsToken, Expression initializer,
        Token semicolonToken)
    {
        this.keywordToken = keywordToken;
        this.identifierToken = identifierToken;
        this.typeClause = typeClause;
        this.equalsToken = equalsToken;
        this.initializer = initializer;
        this.semicolonToken = semicolonToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return keywordToken;
        yield return identifierToken;
        yield return equalsToken;
        yield return initializer;
        yield return semicolonToken;
    }
}