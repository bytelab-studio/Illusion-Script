namespace ILS.Lexing;

public sealed class Diagnostic
{
    public readonly TextSpan span;
    public readonly string message;

    public Diagnostic(TextSpan span, string message)
    {
        this.span = span;
        this.message = message;
    }
}