using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Members;

public sealed class StructMember : Member
{
    public override NodeType type => NodeType.STRUCT_MEMBER;
    public override TextSpan span { get; }

    public Token structKeyword;
    public Token identifierToken;
    public Token lBraceToken;
    public List<StructItem> items;
    public Token rBraceToken;

    public StructMember(Token structKeyword, Token identifierToken, Token lBraceToken, List<StructItem> items, Token rBraceToken)
    {
        this.structKeyword = structKeyword;
        this.identifierToken = identifierToken;
        this.lBraceToken = lBraceToken;
        this.items = items;
        this.rBraceToken = rBraceToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return structKeyword;
        yield return identifierToken;
        yield return lBraceToken;
        foreach (StructItem member in items)
        {
            yield return member;
        }
        yield return rBraceToken;
    }
}