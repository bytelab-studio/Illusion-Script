using System.Collections.Generic;
using ILS.Binding.Symbols;
using ILS.Lexing;

namespace ILS.Binding.Operation;

public sealed class BoundBinaryOperator
{
    public readonly NodeType type;
    public readonly TypeSymbol leftType;
    public readonly TypeSymbol rightType;
    public readonly TypeSymbol resultType;

    private BoundBinaryOperator(NodeType type, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
    {
        this.type = type;
        this.leftType = leftType;
        this.rightType = rightType;
        this.resultType = resultType;
    }

    private BoundBinaryOperator(NodeType type, TypeSymbol baseType)
        : this(type, baseType, baseType, baseType)
    {
    }

    private BoundBinaryOperator(NodeType type, TypeSymbol baseType, TypeSymbol resultType)
        : this(type, baseType, baseType, resultType)
    {
    }

    private static BoundBinaryOperator[] operators;

    public static IEnumerable<BoundBinaryOperator> Bind(NodeType type)
    {
        foreach (BoundBinaryOperator binaryOperator in operators)
        {
            if (binaryOperator.type == type)
            {
                yield return binaryOperator;
            }
        }
    }

    private static IEnumerable<BoundBinaryOperator> CreateMathematicalOperator(TypeSymbol baseType)
    {
        yield return new BoundBinaryOperator(NodeType.PLUS_TOKEN, baseType);
        yield return new BoundBinaryOperator(NodeType.MINUS_TOKEN, baseType);
        yield return new BoundBinaryOperator(NodeType.STAR_TOKEN, baseType);
        yield return new BoundBinaryOperator(NodeType.SLASH_TOKEN, baseType);
        foreach (BoundBinaryOperator binaryOperator in CreateComparisionOperator(baseType))
        {
            yield return binaryOperator;
        }
    }

    private static IEnumerable<BoundBinaryOperator> CreateComparisionOperator(TypeSymbol baseType)
    {
        yield return new BoundBinaryOperator(NodeType.EQUALS_EQUALS_TOKEN, baseType, TypeSymbol.boolean);
        yield return new BoundBinaryOperator(NodeType.BANG_EQUALS_TOKEN, baseType, TypeSymbol.boolean);
    }

    static BoundBinaryOperator()
    {
        List<BoundBinaryOperator> operators = new List<BoundBinaryOperator>();
        // Unsigned ints
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.u64));
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.u32));
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.u16));
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.u8));

        // Signed ints
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.i64));
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.i32));
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.i16));
        operators.AddRange(CreateMathematicalOperator(TypeSymbol.i8));

        // Booleans
        operators.Add(new(NodeType.AND_AND_TOKEN, TypeSymbol.boolean));
        operators.Add(new(NodeType.PIPE_PIPE_TOKEN, TypeSymbol.boolean));
        operators.AddRange(CreateComparisionOperator(TypeSymbol.boolean));

        BoundBinaryOperator.operators = operators.ToArray();
    }
}