namespace ILS.Binding.Symbols;

public sealed class FunctionSymbol
{
    public readonly string name;
    public readonly string llvmName;
    public readonly TypeSymbol returnType;

    public FunctionSymbol(string name, TypeSymbol returnType)
    {
        this.name = name;
        this.llvmName = "@" + name;
        this.returnType = returnType;
    }

    public TypeSymbol AsTypeSymbol()
    {
        return TypeSymbol.Func(returnType);
    }
}