using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Statements;

public sealed class IfStatement : Statement
{
    public override NodeType type => NodeType.IF_STATEMENT;
    public override TextSpan span => TextSpan.Merge(ifKeyword.span, elseStatement != null ? elseStatement.span : body.span);

    public Token ifKeyword;
    public Token lParen;
    public Expression condition;
    public Token rParen;
    public Statement body;
    public ElseStatement elseStatement;

    public IfStatement(Token ifKeyword, Token lParen, Expression condition, Token rParen, Statement body, ElseStatement elseStatement)
    {
        this.ifKeyword = ifKeyword;
        this.lParen = lParen;
        this.condition = condition;
        this.rParen = rParen;
        this.body = body;
        this.elseStatement = elseStatement;
    }

    public override IEnumerable<Node> GetChildren()
    {
        throw new System.NotImplementedException();
    }
}