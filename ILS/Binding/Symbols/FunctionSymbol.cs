using System;
using System.Linq;

namespace ILS.Binding.Symbols;

public sealed class FunctionSymbol
{
    public readonly string name;
    public readonly VariableSymbol[] parameters;
    public readonly string llvmName;
    public readonly TypeSymbol returnType;

    public FunctionSymbol(string name, VariableSymbol[] parameters, TypeSymbol returnType)
    {
        this.name = name;
        this.parameters = parameters;
        this.llvmName = "@" + name;
        this.returnType = returnType;
    }

    public TypeSymbol AsTypeSymbol()
    {
        return TypeSymbol.Func(parameters.Select(parameter => parameter.type).Concat(new []{returnType}).ToArray());
    }
}