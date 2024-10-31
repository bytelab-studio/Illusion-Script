using System.Collections.Generic;
using ILS.Binding.Symbols;

namespace ILS.Binding;

public sealed class Scope
{
    public Scope parent;
    private List<VariableSymbol> variables;

    public Scope(Scope parent)
    {
        this.parent = parent;
        this.variables = new List<VariableSymbol>();
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

    private bool CanDeclareSymbol(string name)
    {
        foreach (VariableSymbol variable in variables)
        {
            if (variable.name == name)
            {
                return false;
            }
        }

        return true;
    }
}