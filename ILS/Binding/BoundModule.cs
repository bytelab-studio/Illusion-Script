using System.Collections.Generic;
using ILS.Binding.Members;
using ILS.Lexing;

namespace ILS.Binding;

public sealed class BoundModule
{
    public Scope scope;
    public List<BoundFunctionMember> functions;
    public DiagnosticBag diagnostics;


    public BoundModule()
    {
        scope = new Scope(null);
        functions = new List<BoundFunctionMember>();
        diagnostics = new DiagnosticBag();
    }

    public IEnumerable<Diagnostic> Diagnostics()
    {
        return diagnostics.diagnostics;
    }
}