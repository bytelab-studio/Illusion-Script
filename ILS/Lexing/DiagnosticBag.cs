using System.Collections.Generic;
using ILS.Binding.Symbols;

namespace ILS.Lexing;

public sealed class DiagnosticBag
{
    public readonly List<Diagnostic> diagnostics;

    public DiagnosticBag()
    {
        this.diagnostics = new List<Diagnostic>();
    }

    private void Report(TextSpan span, string message)
    {
        Diagnostic diagnostic = new Diagnostic(span, message);
        this.diagnostics.Add(diagnostic);
    }

    //
    // Lexer
    //

    public void ReportUnexpectedChar(TextSpan span, char c)
    {
        Report(span, "ERROR: Bad character input: '" + c + "'");
    }

    //
    // Parser
    //

    public void ReportUnexpectedToken(TextSpan span, NodeType current, NodeType expected)
    {
        Report(span, "ERROR: Unexpected token '" + current + "', expected '" + expected + "'");
    }

    public void ReportUnexpectedToken(TextSpan span, NodeType current)
    {
        Report(span, "ERROR: Unexpected token '" + current + "'");
    }

    //
    // Binder
    //

    public void ReportUnknownUnaryOperation(TextSpan span, string operatorText, TypeSymbol rightType)
    {
        if (rightType == TypeSymbol.error)
        {
            return;
        }

        Report(span, "ERROR: Unary operator '" + operatorText + "' is not defined for type '" + rightType.fullName + "'");
    }
    public void ReportUnknownBinaryOperation(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
    {
        if (leftType == TypeSymbol.error || rightType == TypeSymbol.error)
        {
            return;
        }

        Report(span, "ERROR: Binary operator '" + operatorText + "' is not defined for the types '" + leftType.fullName + "' and '" +
                     rightType.fullName + "'");
    }

    public void ReportUnknownPostOperator(TextSpan span, string operatorText, TypeSymbol leftType)
    {
        if (leftType == TypeSymbol.error)
        {
            return;
        }

        Report(span, "ERROR: Post operator '" + operatorText + "' is not defined for type '" + leftType.fullName + "'");
    }

    public void ReportUnresolvedSymbol(TextSpan span, string name)
    {
        Report(span, "ERROR: Unresolved symbol '" + name + "'");
    }

    public void ReportImplicitConversion(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
    {
        if (fromType == TypeSymbol.error || toType == TypeSymbol.error)
        {
            return;
        }
        Report(span,
            "ERROR: Cannot convert '" + fromType.fullName + "' to '" + toType.fullName + "' implicitly (Are you missing a cast or a reinterpretation?)");
    }

    public void ReportConversion(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
    {
        if (fromType == TypeSymbol.error || toType == TypeSymbol.error)
        {
            return;
        }
        Report(span, "ERROR: Cannot convert '" + fromType.fullName + "' to '" + toType.fullName + "'");
    }
    public void ReportSymbolAlreadyDefined(TextSpan span, string name)
    {
        Report(span, "ERROR: Symbol '" + name + "' is already declared in this scope");
    }

    public void ReportExpressionNotAllowed(TextSpan span, NodeType type)
    {
        Report(span, "ERROR: The expression '" + type + "' is here not allowed");
    }

    public void ReportUnknownType(TextSpan span, string name)
    {
        Report(span, "ERROR: Unknown type '" + name + "'");
    }

    public void ReportNotAssignable(TextSpan span, NodeType type)
    {
        if (type == NodeType.ERROR_EXPRESSION)
        {
            return;
        }
        Report(span, "ERROR: Expression '" + type + "' is not assignable");
    }

    public void ReportVariableIsImmutable(TextSpan span, string name)
    {
        Report(span, "ERROR: Variable '" + name + "' is immutable");
    }

    public void ReportExpectGeneric(TextSpan span, string name)
    {
        Report(span, "ERROR: Type '" + name + "' expected a generic");
    }

    public void ReportExpectGenericCount(TextSpan span, string name, int expected, int got)
    {
        Report(span, "ERROR: Type '" + name + "' expected '" + expected + "' generics got '" + got + "'");
    }

    public void ReportDeReferenceRequiresPtr(TextSpan span)
    {
        Report(span, "ERROR: Dereference requires pointer operand");
    }

    public void ReportNotInLoop(TextSpan span, NodeType type)
    {
        Report(span, $"ERROR: '{type}' can only be used inside a loop");
    }

    public void ReportAmbiguousParameterName(TextSpan span, string name)
    {
        Report(span, "ERROR: A parameter with the name '" + name + "' is already declared");
    }

    public void ReportExpressionNotCallable(TextSpan span)
    {
        Report(span, "ERROR: The expression is not callable");
    }

    public void ReportUnexpectedArgumentCount(TextSpan span, int expected, int got)
    {
        Report(span, "ERROR: Function expected '" + expected + "' arguments, but '" + got + "' was provided");
    }
}