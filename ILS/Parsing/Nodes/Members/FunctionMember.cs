using System.Collections.Generic;
using ILS.Lexing;
using ILS.Parsing.Nodes.Statements;

namespace ILS.Parsing.Nodes.Members;

public sealed class FunctionMember : Member
{
    public override NodeType type => NodeType.FUNCTION_MEMBER;
    public override TextSpan span => TextSpan.Merge(functionKeyword.span, body.span);

    public Token functionKeyword;
    public Token identifierToken;
    public Token lParenToken;
    public Token rParenToken;
    public TypeClause returnClause;
    public BlockStatement body;

    public FunctionMember(Token functionKeyword, Token identifierToken, Token lParenToken, Token rParenToken, TypeClause returnClause, BlockStatement body)
    {
        this.functionKeyword = functionKeyword;
        this.identifierToken = identifierToken;
        this.lParenToken = lParenToken;
        this.rParenToken = rParenToken;
        this.returnClause = returnClause;
        this.body = body;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return functionKeyword;
        yield return identifierToken;
        yield return lParenToken;
        yield return rParenToken;
        yield return returnClause;
        yield return body;
    }
}