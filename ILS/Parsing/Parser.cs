using System.Collections.Generic;
using ILS.Lexing;
using ILS.Parsing.Nodes;
using ILS.Parsing.Nodes.Expressions;
using ILS.Parsing.Nodes.Members;
using ILS.Parsing.Nodes.Statements;
using Expression = ILS.Parsing.Nodes.Expression;

namespace ILS.Parsing;

public sealed class Parser
{
    private readonly List<Token> tokens;
    private int position;
    private DiagnosticBag diagnostics;

    public Parser(string text)
    {
        tokens = new List<Token>();
        position = 0;
        diagnostics = new DiagnosticBag();
        Lexer lexer = new Lexer(text);
        Token token;
        do
        {
            token = lexer.NextToken();
            if (token.type != NodeType.WHITESPACE_TOKEN && token.type != NodeType.ERROR_TOKEN)
            {
                tokens.Add(token);
            }
        } while (token.type != NodeType.EOF_TOKEN);
        diagnostics.diagnostics.AddRange(lexer.Diagnostics());
    }

    public SyntaxTree Parse()
    {
        List<Member> members = ParseMembers();
        Token eof = Match(NodeType.EOF_TOKEN);
        return new SyntaxTree(diagnostics, members, eof);
    }


    private Token Peek(int offset)
    {
        if (position + offset >= tokens.Count)
        {
            return tokens[tokens.Count - 1];
        }
        return tokens[position + offset];
    }

    private Token Current() => Peek(0);

    private Token NextToken()
    {
        Token current = Current();
        position++;
        return current;
    }

    private Token Match(NodeType type)
    {
        if (Current().type == type)
        {
            return NextToken();
        }

        diagnostics.ReportUnexpectedToken(Current().span, Current().type, type);
        return new Token(type, Current().span, null);
    }

    private List<Member> ParseMembers()
    {
        List<Member> members = new List<Member>();

        while (Current().type != NodeType.EOF_TOKEN)
        {
            Member member = ParseMember();
            if (member != null)
            {
                members.Add(member);
            }
            while (Current().type == NodeType.SEMI_TOKEN)
            {
                NextToken();
            }
        }

        return members;
    }

    private Member ParseMember()
    {
        if (Current().type == NodeType.FUNCTION_KEYWORD)
        {
            return ParseFunctionMember();
        }
        Token current = NextToken();
        diagnostics.ReportUnexpectedToken(current.span, current.type);
        return null;
    }

    private Member ParseFunctionMember()
    {
        Token functionKeyword = Match(NodeType.FUNCTION_KEYWORD);
        Token identifierToken = Match(NodeType.IDENTIFIER_TOKEN);
        Token lParenToken = Match(NodeType.LPAREN_TOKEN);

        List<FunctionParameter> parameters = new List<FunctionParameter>();

        while (Current().type != NodeType.RPAREN_TOKEN && Current().type != NodeType.EOF_TOKEN)
        {
            Token parameterIdentifierToken = Match(NodeType.IDENTIFIER_TOKEN);
            TypeClause clause = ParseTypeClause();
            parameters.Add(new FunctionParameter(parameterIdentifierToken, clause));
            if (Current().type != NodeType.RPAREN_TOKEN)
            {
                Match(NodeType.COMMA_TOKEN);
            }
        }

        Token rParenToken = Match(NodeType.RPAREN_TOKEN);
        TypeClause returnClause = ParseTypeClause();
        BlockStatement body = ParseBlockStatement();

        return new FunctionMember(functionKeyword, identifierToken, lParenToken, parameters, rParenToken, returnClause, body);
    }

    private Statement ParseStatement()
    {
        Statement statement = ParseStatementInternal();
        while (Current().type == NodeType.SEMI_TOKEN)
        {
            NextToken();
        }
        return statement;
    }

    private Statement ParseStatementInternal()
    {
        if (Current().type == NodeType.LBRACE_TOKEN)
        {
            return ParseBlockStatement();
        }
        if (Current().type == NodeType.LET_KEYWORD || Current().type == NodeType.CONST_KEYWORD)
        {
            return ParseVariableStatement();
        }
        if (Current().type == NodeType.IF_KEYWORD)
        {
            return ParseIfStatement();
        }
        if (Current().type == NodeType.WHILE_KEYWORD)
        {
            return ParseWhileKeyword();
        }
        if (Current().type == NodeType.BREAK_KEYWORD)
        {
            return ParseBreakStatement();
        }
        if (Current().type == NodeType.CONTINUE_KEYWORD)
        {
            return ParseContinueStatement();
        }

        return ParseExpressionStatement();
    }

    private BlockStatement ParseBlockStatement()
    {
        Token lBrace = Match(NodeType.LBRACE_TOKEN);
        List<Statement> statements = new List<Statement>();
        while (Current().type != NodeType.RBRACE_TOKEN && Current().type != NodeType.EOF_TOKEN)
        {
            statements.Add(ParseStatement());
        }
        Token rBrace = Match(NodeType.RBRACE_TOKEN);
        return new BlockStatement(lBrace, statements, rBrace);
    }

    private VariableStatement ParseVariableStatement()
    {
        Token keywordToken;
        if (Current().type != NodeType.LET_KEYWORD && Current().type != NodeType.CONST_KEYWORD)
        {
            keywordToken = Match(NodeType.LET_KEYWORD);
        }
        else
        {
            keywordToken = NextToken();
        }
        Token identifierToken = Match(NodeType.IDENTIFIER_TOKEN);
        TypeClause typeClause = ParseTypeClause();
        Token equalsToken = Match(NodeType.EQUALS_TOKEN);
        Expression initializer = ParseExpression();
        Token semicolonToken = Match(NodeType.SEMI_TOKEN);

        return new VariableStatement(keywordToken, identifierToken, typeClause, equalsToken, initializer, semicolonToken);
    }

    private IfStatement ParseIfStatement()
    {
        Token ifKeyword = Match(NodeType.IF_KEYWORD);
        Token lParen = Match(NodeType.LPAREN_TOKEN);
        Expression condition = ParseExpression();
        Token rParen = Match(NodeType.RPAREN_TOKEN);
        Statement body = ParseStatement();
        ElseStatement elseStatement = null;
        if (Current().type == NodeType.ELSE_KEYWORD)
        {
            elseStatement = ParseElseStatement();
        }

        return new IfStatement(ifKeyword, lParen, condition, rParen, body, elseStatement);
    }

    private ElseStatement ParseElseStatement()
    {
        Token elseKeyword = Match(NodeType.ELSE_KEYWORD);
        Statement body = ParseStatement();
        return new ElseStatement(elseKeyword, body);
    }

    private WhileStatement ParseWhileKeyword()
    {
        Token whileKeyword = Match(NodeType.WHILE_KEYWORD);
        Token lParen = Match(NodeType.LPAREN_TOKEN);
        Expression condition = ParseExpression();
        Token rParen = Match(NodeType.RPAREN_TOKEN);
        Statement body = ParseStatement();

        return new WhileStatement(whileKeyword, lParen, condition, rParen, body);
    }

    private BreakStatement ParseBreakStatement()
    {
        Token breakKeyword = Match(NodeType.BREAK_KEYWORD);
        Token semicolonToken = Match(NodeType.SEMI_TOKEN);

        return new BreakStatement(breakKeyword, semicolonToken);
    }

    private ContinueStatement ParseContinueStatement()
    {
        Token breakKeyword = Match(NodeType.CONTINUE_KEYWORD);
        Token semicolonToken = Match(NodeType.SEMI_TOKEN);

        return new ContinueStatement(breakKeyword, semicolonToken);
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        Expression expression = ParseExpression();
        Token semicolonToken = Match(NodeType.SEMI_TOKEN);
        return new ExpressionStatement(expression, semicolonToken);
    }

    private Expression ParseExpression()
    {
        return ParseAssignmentExpression();
    }


    private Expression ParseAssignmentExpression()
    {
        Expression left = ParsePostOperationExpression();
        while (Current().type == NodeType.EQUALS_TOKEN)
        {
            Token equalsToken = NextToken();
            Expression expression = ParseExpression();
            left = new AssignmentExpression(left, equalsToken, expression);
        }

        return left;
    }

    private Expression ParsePostOperationExpression()
    {
        Expression left = ParseBinaryExpression(0);
        if (Current().type == NodeType.AS_KEYWORD)
        {
            Token asKeyword = NextToken();
            TypeClause clause = ParseTypeSignature();
            return new ReinterpretationExpression(left, asKeyword, clause);
        }
        if (Current().type == NodeType.PLUS_PLUS_TOKEN)
        {
            Token plusPlusToken = NextToken();
            return new IncrementExpression(left, plusPlusToken);
        }
        if (Current().type == NodeType.MINUS_MINUS_TOKEN)
        {
            Token minusMinusToken = NextToken();
            return new DecrementExpression(left, minusMinusToken);
        }
        if (Current().type == NodeType.QUESTION_TOKEN)
        {
            Token questionToken = NextToken();
            Expression thenExpression = ParseExpression();
            Token colonToken = Match(NodeType.COLON_TOKEN);
            Expression elseExpression = ParseExpression();
            return new TernaryExpression(left, questionToken, thenExpression, colonToken, elseExpression);
        }

        return left;
    }

    private Expression ParseBinaryExpression(int parentPrecedence)
    {
        Expression left;
        int unaryPrecedence = GetUnaryOperatorPrecedence(Current().type);
        if (unaryPrecedence != 0 && unaryPrecedence >= parentPrecedence)
        {
            Token operatorToken = NextToken();
            Expression right = ParseBinaryExpression(unaryPrecedence);
            left = new UnaryExpression(operatorToken, right);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while (true)
        {
            int precedence = GetBinaryOperatorPrecedence(Current().type);
            if (precedence == 0 || precedence <= parentPrecedence)
            {
                break;
            }
            Token operatorToken = NextToken();
            Expression right = ParseBinaryExpression(precedence);
            left = new BinaryExpression(left, operatorToken, right);
        }

        return left;
    }

    private Expression ParsePrimaryExpression()
    {
        if (Current().type == NodeType.LPAREN_TOKEN)
        {
            Token lParen = Match(NodeType.LPAREN_TOKEN);
            Expression expression = ParseExpression();
            Token rParen = Match(NodeType.RPAREN_TOKEN);
            return new ParenExpression(lParen, expression, rParen);
        }
        if (Current().type == NodeType.LANGLE_TOKEN)
        {
            Token lAngle = Match(NodeType.LANGLE_TOKEN);
            TypeClause typeClause = ParseTypeSignature();
            Token rAngle = Match(NodeType.RANGLE_TOKEN);
            Expression expression = ParseExpression();
            return new ConversionExpression(lAngle, typeClause, rAngle, expression);
        }
        if (Current().type == NodeType.TRUE_KEYWORD || Current().type == NodeType.FALSE_KEYWORD)
        {
            return new BoolExpression(NextToken());
        }
        if (Current().type == NodeType.IDENTIFIER_TOKEN)
        {
            return new NameExpression(NextToken());
        }
        return new IntExpression(Match(NodeType.INT_TOKEN));
    }

    private TypeClause ParseTypeClause()
    {
        Match(NodeType.COLON_TOKEN);
        return ParseTypeSignature();
    }

    private TypeClause ParseTypeSignature()
    {
        Token identifierToken = Match(NodeType.IDENTIFIER_TOKEN);
        Token lAngleToken = null;
        Token rAngleToken = null;
        List<TypeClause> generics = new List<TypeClause>();
        if (Current().type == NodeType.LANGLE_TOKEN)
        {
            lAngleToken = NextToken();
            do
            {
                generics.Add(ParseTypeSignature());
                if (Current().type == NodeType.COMMA_TOKEN)
                {
                    Match(NodeType.COMMA_TOKEN);
                }
            } while (Current().type != NodeType.RANGLE_TOKEN && Current().type != NodeType.EOF_TOKEN);
            rAngleToken = Match(NodeType.RANGLE_TOKEN);
        }
        return new TypeClause(identifierToken, lAngleToken, generics, rAngleToken);
    }

    private static int GetUnaryOperatorPrecedence(NodeType type)
    {
        if (type == NodeType.PLUS_TOKEN ||
            type == NodeType.MINUS_TOKEN ||
            type == NodeType.BANG_TOKEN ||
            type == NodeType.STAR_TOKEN ||
            type == NodeType.AND_TOKEN)
        {
            return 6;
        }

        return 0;
    }

    private static int GetBinaryOperatorPrecedence(NodeType type)
    {
        if (type == NodeType.STAR_TOKEN ||
            type == NodeType.SLASH_TOKEN)
        {
            return 5;
        }
        if (type == NodeType.PLUS_TOKEN ||
            type == NodeType.MINUS_TOKEN)
        {
            return 4;
        }
        if (type == NodeType.EQUALS_EQUALS_TOKEN ||
            type == NodeType.BANG_EQUALS_TOKEN)
        {
            return 3;
        }
        if (type == NodeType.AND_AND_TOKEN)
        {
            return 2;
        }
        if (type == NodeType.PIPE_PIPE_TOKEN)
        {
            return 1;
        }

        return 0;
    }
}