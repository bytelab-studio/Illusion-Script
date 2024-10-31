using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Statements;

public sealed class WhileStatement : Statement
{
    public override NodeType type => NodeType.WHILE_STATEMENT;
    public override TextSpan span => TextSpan.Merge(whileKeyword.span, body.span);

    public Token whileKeyword;
    public Token lParen;
    public Expression condition;
    public Token rParen;
    public Statement body;


    public WhileStatement(Token whileKeyword, Token lParen, Expression condition, Token rParen, Statement body)
    {
        this.whileKeyword = whileKeyword;
        this.lParen = lParen;
        this.condition = condition;
        this.rParen = rParen;
        this.body = body;
    }
    public override IEnumerable<Node> GetChildren()
    {
        yield return whileKeyword;
        yield return lParen;
        yield return condition;
        yield return rParen;
        yield return body;
    }
}