using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes;

public sealed class StructItem : Node
{
    public override NodeType type => NodeType.STRUCT_ITEM;
    public override TextSpan span => TextSpan.Merge(identifierToken.span, typeClause.span);

    public Token identifierToken;
    public TypeClause typeClause;
    public Token semiToken;

    public StructItem(Token identifierToken, TypeClause typeClause, Token semiToken)
    {
        this.identifierToken = identifierToken;
        this.typeClause = typeClause;
        this.semiToken = semiToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return identifierToken;
        yield return typeClause;
        yield return semiToken;
    }
}