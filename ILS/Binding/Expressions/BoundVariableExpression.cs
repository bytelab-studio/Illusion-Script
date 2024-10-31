using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundVariableExpression : BoundExpression
{
    public override NodeType type => NodeType.VARIABLE_EXPRESSION;
    public override TypeSymbol returnType => variable.type;

    public readonly VariableSymbol variable;
    
    public BoundVariableExpression(VariableSymbol variable)
    {
        this.variable = variable;
    }
}