using System.Collections.Generic;
using ILS.Binding.Symbols;

namespace ILS.Binding;

public sealed class Scope
{
    public Scope parent;
    private List<VariableSymbol> variables;
    private List<FunctionSymbol> functions;

    public Scope(Scope parent)
    {
        this.parent = parent;
        this.variables = new List<VariableSymbol>();
        this.functions = new List<FunctionSymbol>();
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