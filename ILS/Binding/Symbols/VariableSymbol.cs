namespace ILS.Binding.Symbols;

public sealed class VariableSymbol
{
    public readonly string llvmName;
    public readonly bool isReadonly;
    public readonly string name;
    public readonly TypeSymbol type;

    public VariableSymbol(bool isReadonly, string name, TypeSymbol type, int llvmNumber)
    {
        this.llvmName = "%v" + llvmNumber;
        this.isReadonly = isReadonly;
        this.name = name;
        this.type = type;
    }
}