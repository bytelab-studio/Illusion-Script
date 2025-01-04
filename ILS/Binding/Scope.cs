using System.Collections.Generic;
using ILS.Binding.Symbols;

namespace ILS.Binding;

public sealed class Scope
{
    public Scope parent;
    private List<VariableSymbol> variables;
    private List<FunctionSymbol> functions;
    private List<TypeSymbol> types;

    public Scope(Scope parent)
    {
        this.parent = parent;
        this.variables = new List<VariableSymbol>();
        this.functions = new List<FunctionSymbol>();
        this.types = new List<TypeSymbol>();
    }

    public bool TryDeclareVariable(VariableSymbol variable)
    {
        if (!CanDeclareSymbol(variable.name))
        {
            return false;
        }

        variables.Add(variable);
        return true;
    }

    public VariableSymbol TryLookupVariable(string name)
    {
        foreach (VariableSymbol variable in variables)
        {
            if (variable.name == name)
            {
                return variable;
            }
        }

        if (parent != null)
        {
            return parent.TryLookupVariable(name);
        }

        return null;
    }

    public bool TryDeclareFunction(FunctionSymbol function)
    {
        if (!CanDeclareSymbol(function.name))
        {
            return false;
        }

        functions.Add(function);
        return true;
    }

    public FunctionSymbol TryLookupFunction(string name)
    {
        foreach (FunctionSymbol function in functions)
        {
            if (function.name == name)
            {
                return function;
            }
        }

        if (parent != null)
        {
            return parent.TryLookupFunction(name);
        }

        return null;
    }

    public bool TryDeclareType(TypeSymbol type)
    {
        if (!CanDeclareSymbol(type.name))
        {
            return false;
        }

        types.Add(type);
        return true;
    }

    public TypeSymbol TryLookupType(string name)
    {
        foreach (TypeSymbol type in types)
        {
            if (type.name == name)
            {
                return type;
            }
        }

        if (parent != null)
        {
            return parent.TryLookupType(name);
        }

        return null;
    }

    public object TryLookupSymbol(string name)
    {
        foreach (VariableSymbol variable in variables)
        {
            if (variable.name == name)
            {
                return variable;
            }
        }
        foreach (FunctionSymbol function in functions)
        {
            if (function.name == name)
            {
                return function;
            }
        }
        foreach (TypeSymbol type in types)
        {
            if (type.name == name)
            {
                return type;
            }
        }

        if (parent != null)
        {
            return parent.TryLookupSymbol(name);
        }

        return null;
    }

    private bool CanDeclareSymbol(string name)
    {
        foreach (VariableSymbol variable in variables)
        {
            if (variable.name == name)
            {
                return false;
            }
        }
        foreach (FunctionSymbol function in functions)
        {
            if (function.name == name)
            {
                return false;
            }
        }

        return true;
    }
}