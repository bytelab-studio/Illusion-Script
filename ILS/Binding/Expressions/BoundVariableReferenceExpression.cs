using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundVariableReferenceExpression : BoundExpression
{
    public override NodeType type => NodeType.VARIABLE_REFERENCE_EXPRESSION;
    public override TypeSymbol returnType => TypeSymbol.Reference(variable.type);

    public readonly VariableSymbol variable;
    public BoundVariableReferenceExpression(VariableSymbol variable)
    {
        this.variable = variable;
    }
}