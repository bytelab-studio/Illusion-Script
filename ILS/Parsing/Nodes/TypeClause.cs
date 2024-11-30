using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes;

public sealed class TypeClause : Node
{
    public override NodeType type => NodeType.TYPE_CLAUSE;
    public override TextSpan span => TextSpan.Merge(identifierToken.span, rAngleToken != null ? rAngleToken.span : identifierToken.span);

    public Token identifierToken;
    public Token lAngleToken;
    public List<TypeClause> generics;
    public Token rAngleToken;
    public TypeClause(Token identifierToken, Token lAngleToken, List<TypeClause> generics, Token rAngleToken)
    {
        this.identifierToken = identifierToken;
        this.lAngleToken = lAngleToken;
        this.generics = generics;
        this.rAngleToken = rAngleToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        throw new System.NotImplementedException();
    }
}