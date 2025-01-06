using System.Collections.Generic;
using ILS.Binding.Members;
using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding;

public sealed class BoundModule
{
    public Scope scope;
    public List<FunctionSymbol> externFunctions;
    public List<BoundFunctionMember> functions;
    public List<TypeSymbol> structs;
    public DiagnosticBag diagnostics;


    public BoundModule()
    {
        scope = new Scope(null);
        externFunctions = new List<FunctionSymbol>();
        functions = new List<BoundFunctionMember>();
        structs = new List<TypeSymbol>();
        diagnostics = new DiagnosticBag();
    }

    public IEnumerable<Diagnostic> Diagnostics()
    {
        return diagnostics.diagnostics;
    }
}