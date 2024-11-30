using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes;

public sealed class FunctionParameter : Node
{
    public override NodeType type => NodeType.PARAMETER;
    public override TextSpan span => TextSpan.Merge(identifierToken.span, clause.span);

    public Token identifierToken;
    public TypeClause clause;

    public FunctionParameter(Token identifierToken, TypeClause clause)
    {
        this.identifierToken = identifierToken;
        this.clause = clause;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return identifierToken;
        yield return clause;
    }
}