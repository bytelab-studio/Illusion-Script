using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Expressions;

public sealed class BoundFunctionExpression : BoundExpression
{
    public override NodeType type => NodeType.FUNCTION_EXPRESSION;
    public override TypeSymbol returnType => function.AsTypeSymbol();

    public readonly FunctionSymbol function;

    public BoundFunctionExpression(FunctionSymbol function)
    {
        this.function = function;
    }
}