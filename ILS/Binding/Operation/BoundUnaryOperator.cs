using System.Collections.Generic;
using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Operation;

public sealed class BoundUnaryOperator
{
    public readonly NodeType type;
    public readonly TypeSymbol rightType;
    public readonly TypeSymbol resultType;

    private BoundUnaryOperator(NodeType type, TypeSymbol rightType, TypeSymbol resultType)
    {
        this.type = type;
        this.rightType = rightType;
        this.resultType = resultType;
    }

    private BoundUnaryOperator(NodeType type, TypeSymbol rightType)
        : this(type, rightType, rightType)
    {
    }

    private static BoundUnaryOperator[] operators =
    {
        new(NodeType.BANG_TOKEN, TypeSymbol.boolean),
        new BoundUnaryOperator(NodeType.MINUS_TOKEN, TypeSymbol.i32),
        new BoundUnaryOperator(NodeType.PLUS_TOKEN, TypeSymbol.i32),
    };

    public static IEnumerable<BoundUnaryOperator> Bind(NodeType type)
    {
        foreach (BoundUnaryOperator unaryOperator in operators)
        {
            if (unaryOperator.type == type)
            {
                yield return unaryOperator;
            }
        }
    }
}