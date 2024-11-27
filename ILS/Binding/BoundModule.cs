using System.Collections.Generic;
using ILS.Binding.Members;
using ILS.Lexing;

namespace ILS.Binding;

public sealed class BoundModule
{
    public List<BoundFunctionMember> functions;
    public DiagnosticBag diagnostics;


    public BoundModule()
    {
        functions = new List<BoundFunctionMember>();
        diagnostics = new DiagnosticBag();
    }

    public IEnumerable<Diagnostic> Diagnostics()
    {
        return diagnostics.diagnostics;
    }
}