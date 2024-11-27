using ILS.Binding.Statements;
using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Members;

public sealed class BoundFunctionMember : BoundMember
{
    public override NodeType type => NodeType.FUNCTION_MEMBER;

    public readonly FunctionSymbol symbol;
    public readonly BoundBlockStatement body;

    public BoundFunctionMember(FunctionSymbol symbol, BoundBlockStatement body)
    {
        this.symbol = symbol;
        this.body = body;
    }
}