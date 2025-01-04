namespace ILS.Binding.Symbols;

public sealed class StructItemSymbol
{
    public readonly string name;
    public readonly TypeSymbol type;

    public StructItemSymbol(string name, TypeSymbol type)
    {
        this.name = name;
        this.type = type;
    }
}