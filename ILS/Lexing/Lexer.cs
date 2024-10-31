using System.Collections.Generic;

namespace ILS.Lexing;

public sealed class Lexer
{
    private readonly string text;
    private int position;
    private int col;
    private int line;
    private DiagnosticBag diagnostics;

    public Lexer(string text)
    {
        this.text = text;
        this.position = 0;
        this.col = 1;
        this.line = 1;
        this.diagnostics = new DiagnosticBag();
    }

    public IEnumerable<Diagnostic> Diagnostics()
    {
        return diagnostics.diagnostics;
    }

    public Token NextToken()
    {
        int start = position;
        TextSpan span = new TextSpan();
        span.start = start;
        span.colStart = col;
        span.lineStart = line;

        if (position >= text.Length)
        {
            return new Token(NodeType.EOF_TOKEN, FinishTextSpan(span), "\0");
        }

        if (char.IsDigit(Current()))
        {
            while (char.IsDigit(Current()) && Current() != '\0')
            {
                Next();
            }

            int length = position - start;
            string text = this.text.Substring(start, length);
            return new Token(NodeType.INT_TOKEN, FinishTextSpan(span), text);
        }
        if (char.IsWhiteSpace(Current()))
        {
            while (char.IsWhiteSpace(Current()) && Current() != '\0')
            {
                Next();
            }

            int length = position - start;
            string text = this.text.Substring(start, length);
            return new Token(NodeType.WHITESPACE_TOKEN, FinishTextSpan(span), text);
        }
        if (char.IsLetter(Current()))
        {
            while ((char.IsLetter(Current()) || char.IsDigit(Current()) || Current() == '_') && Current() != '\0')
            {
                Next();
            }

            int length = position - start;
            string text = this.text.Substring(start, length);
            NodeType type = GetKeywordType(text);
            return new Token(type, FinishTextSpan(span), text);
        }
        if (Current() == ';')
        {
            position++;
            return new Token(NodeType.SEMI_TOKEN, FinishTextSpan(span), ";");
        }
        if (Current() == ':')
        {
            position++;
            return new Token(NodeType.COLON_TOKEN, FinishTextSpan(span), ":");
        }
        if (Current() == '+')
        {
            position++;
            return new Token(NodeType.PLUS_TOKEN, FinishTextSpan(span), "+");
        }
        if (Current() == '-')
        {
            position++;
            return new Token(NodeType.MINUS_TOKEN, FinishTextSpan(span), "-");
        }
        if (Current() == '*')
        {
            position++;
            return new Token(NodeType.STAR_TOKEN, FinishTextSpan(span), "*");
        }
        if (Current() == '/')
        {
            position++;
            return new Token(NodeType.SLASH_TOKEN, FinishTextSpan(span), "/");
        }
        if (Current() == '(')
        {
            position++;
            return new Token(NodeType.LPAREN_TOKEN, FinishTextSpan(span), "/");
        }
        if (Current() == ')')
        {
            position++;
            return new Token(NodeType.RPAREN_TOKEN, FinishTextSpan(span), "/");
        }
        if (Current() == '{')
        {
            position++;
            return new Token(NodeType.LBRACE_TOKEN, FinishTextSpan(span), "{");
        }
        if (Current() == '}')
        {
            position++;
            return new Token(NodeType.RBRACE_TOKEN, FinishTextSpan(span), "}");
        }
        if (Current() == '<')
        {
            position++;
            return new Token(NodeType.LANGLE_TOKEN, FinishTextSpan(span), "<");
        }
        if (Current() == '>')
        {
            position++;
            return new Token(NodeType.RANGLE_TOKEN, FinishTextSpan(span), ">");
        }
        if (Current() == '!')
        {
            if (Peek(1) == '=')
            {
                position += 2;
                return new Token(NodeType.BANG_EQUALS_TOKEN, FinishTextSpan(span), "!=");
            }
            position++;
            return new Token(NodeType.BANG_TOKEN, FinishTextSpan(span), "!");
        }
        if (Current() == '&')
        {
            if (Peek(1) == '&')
            {
                position += 2;
                return new Token(NodeType.AND_AND_TOKEN, FinishTextSpan(span), "&&");
            }
            position++;
            return new Token(NodeType.AND_TOKEN, FinishTextSpan(span), "&");
        }
        if (Current() == '|')
        {
            if (Peek(1) == '|')
            {
                position += 2;
                return new Token(NodeType.PIPE_PIPE_TOKEN, FinishTextSpan(span), "||");
            }
        }
        if (Current() == '=')
        {
            if (Peek(1) == '=')
            {
                position += 2;
                return new Token(NodeType.EQUALS_EQUALS_TOKEN, FinishTextSpan(span), "==");
            }

            position++;
            return new Token(NodeType.EQUALS_TOKEN, FinishTextSpan(span), "=");
        }

        position++;
        diagnostics.ReportUnexpectedChar(FinishTextSpan(span), Current());
        return new Token(NodeType.ERROR_TOKEN, FinishTextSpan(span), text.Substring(position - 1, 1));
    }

    private char Peek(int offset)
    {
        if (position + offset >= text.Length)
        {
            return '\0';
        }

        return text[position + offset];
    }

    private char Current() => Peek(0);

    private void Next()
    {
        if (Current() == '\n')
        {
            line++;
            col = 1;
        }
        else
        {
            col++;
        }
        position++;
    }

    private TextSpan FinishTextSpan(TextSpan span)
    {
        span.end = position;
        span.length = span.end - span.start;
        span.colEnd = col;
        span.lineEnd = line;

        return span;
    }

    private NodeType GetKeywordType(string keyword)
    {
        if (keyword == "true")
        {
            return NodeType.TRUE_KEYWORD;
        }
        if (keyword == "false")
        {
            return NodeType.FALSE_KEYWORD;
        }
        if (keyword == "let")
        {
            return NodeType.LET_KEYWORD;
        }
        if (keyword == "const")
        {
            return NodeType.CONST_KEYWORD;
        }
        if (keyword == "if")
        {
            return NodeType.IF_KEYWORD;
        }
        if (keyword == "else")
        {
            return NodeType.ELSE_KEYWORD;
        }
        if (keyword == "as")
        {
            return NodeType.AS_KEYWORD;
        }
        if (keyword == "while")
        {
            return NodeType.WHILE_KEYWORD;
        }
        if (keyword == "break")
        {
            return NodeType.BREAK_KEYWORD;
        }
        if (keyword == "continue")
        {
            return NodeType.CONTINUE_KEYWORD;
        }

        return NodeType.IDENTIFIER_TOKEN;
    }
}