using System.IO;

namespace ILS.IO;

public static class TextWriterExt
{
    private const string INDENT = "    ";

    public static void WriteIntend(this TextWriter writer, string value)
    {
        writer.Write(INDENT);
        writer.Write(value);
    }
}