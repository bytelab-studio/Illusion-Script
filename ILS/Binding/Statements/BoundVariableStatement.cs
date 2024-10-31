using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Statements;

public sealed class BoundVariableStatement : BoundStatement
{
    public override NodeType type => NodeType.VARIABLE_STATEMENT;

    public readonly VariableSymbol variable;
    public readonly BoundExpression initializer;

    public BoundVariableStatement(VariableSymbol variable, BoundExpression initializer)
    {
        this.variable = variable;
        this.initializer = initializer;
    }
}