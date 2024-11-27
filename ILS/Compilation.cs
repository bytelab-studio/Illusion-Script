using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILS.Binding;
using ILS.Emitting;
using ILS.Lexing;
using ILS.Parsing.Nodes;

namespace ILS;

public sealed class Compilation
{
    public readonly SyntaxTree syntax;

    public Compilation(SyntaxTree syntax)
    {
        this.syntax = syntax;
    }

    public CompilationResult Emit(TextWriter writer)
    {
        if (syntax.diagnostics.diagnostics.Any())
        {
            return new CompilationResult(syntax.diagnostics.diagnostics);
        }

        BoundModule module = Binder.BindMembers(syntax.root);

        if (module.Diagnostics().Any())
        {
            return new CompilationResult(module.Diagnostics());
        }

        Emitter emitter = new Emitter(writer);
        emitter.EmitModule(module);

        return new CompilationResult(Enumerable.Empty<Diagnostic>());
    }
}

public sealed class CompilationResult
{
    private readonly IEnumerable<Diagnostic> diagnostics;
    public CompilationResult(IEnumerable<Diagnostic> diagnostics)
    {
        this.diagnostics = diagnostics;
    }

    public IEnumerable<Diagnostic> Diagnostics()
    {
        return diagnostics;
    }
}