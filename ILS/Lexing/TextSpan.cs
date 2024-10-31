namespace ILS.Lexing;

public struct TextSpan
{
    public int start;
    public int length;
    public int end;

    public int colStart;
    public int lineStart;

    public int colEnd;
    public int lineEnd;

    public TextSpan()
    {
    }

    public static TextSpan Merge(TextSpan start, TextSpan end)
    {
        TextSpan span = new TextSpan();
        span.start = start.start;
        span.end = end.end;
        span.length = span.end - span.start;

        span.colStart = start.colStart;
        span.lineStart = start.lineStart;

        span.colEnd = end.colEnd;
        span.lineEnd = end.lineEnd;
        return span;
    }
}