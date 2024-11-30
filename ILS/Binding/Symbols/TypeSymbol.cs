using System;
using System.Linq;

namespace ILS.Binding.Symbols;

public sealed class TypeSymbol
{
    public const string PTR_NAME = "PTR";
    public const int PTR_SIZE = 8;
    public const int PTR_ALIGN = 8;

    public const string FUNC_NAME = "FUNC";
    public const int FUNC_SIZE = 8;
    public const int FUNC_ALIGN = 8;

    public readonly bool primitive;
    public readonly string fullName;
    public readonly string name;
    public readonly string llvmName;
    public readonly int size;
    public readonly int align;
    public readonly TypeSymbol[] generics;
    public readonly TypeFlags flags;

    public TypeSymbol(bool primitive, string name, string llvmName, int size, int align, TypeSymbol[] generics, TypeFlags flags)
    {
        this.primitive = primitive;
        this.fullName = name + "<" + string.Join(", ", generics.Select(generic => generic.fullName)) + ">";
        this.name = name;
        this.llvmName = llvmName;
        this.size = size;
        this.align = align;
        this.generics = generics;
        this.flags = flags;
    }

    public bool Equals(TypeSymbol otherType)
    {
        return fullName == otherType.fullName;
    }


    public static readonly TypeSymbol u8 = new TypeSymbol(true, "u8", "i8", 1, 1, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER | TypeFlags.UNSIGNED);
    public static readonly TypeSymbol u16 = new TypeSymbol(true, "u16", "i16", 2, 2, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER | TypeFlags.UNSIGNED);
    public static readonly TypeSymbol u32 = new TypeSymbol(true, "u32", "i32", 4, 4, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER | TypeFlags.UNSIGNED);
    public static readonly TypeSymbol u64 = new TypeSymbol(true, "u64", "i64", 8, 8, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER | TypeFlags.UNSIGNED);

    public static readonly TypeSymbol i8 = new TypeSymbol(true, "i8", "i8", 1, 1, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER);
    public static readonly TypeSymbol i16 = new TypeSymbol(true, "i16", "i16", 2, 2, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER);
    public static readonly TypeSymbol i32 = new TypeSymbol(true, "i32", "i32", 4, 4, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER);
    public static readonly TypeSymbol i64 = new TypeSymbol(true, "i64", "i64", 8, 8, Array.Empty<TypeSymbol>(), TypeFlags.INTEGER);

    public static readonly TypeSymbol voidType = new TypeSymbol(true, "void", "void", 0, 0, Array.Empty<TypeSymbol>(), 0);
    public static readonly TypeSymbol boolean = new TypeSymbol(true, "boolean", "i1", 1, 1, Array.Empty<TypeSymbol>(), 0);
    public static readonly TypeSymbol error = new TypeSymbol(true, "?", "?", 0, 0, Array.Empty<TypeSymbol>(), 0);

    public static TypeSymbol Reference(TypeSymbol baseType)
    {
        return new TypeSymbol(true, "PTR", "ptr", PTR_SIZE, PTR_ALIGN, new[]
        {
            baseType
        }, TypeFlags.INTEGER | TypeFlags.UNSIGNED);
    }

    public static TypeSymbol DeReference(TypeSymbol ptrType)
    {
        if (ptrType.name != PTR_NAME || ptrType.generics.Length != 1)
        {
            throw new Exception("Type is not a ptr");
        }

        return ptrType.generics[0];
    }

    public static TypeSymbol Func(TypeSymbol[] generics)
    {
        return new TypeSymbol(true, FUNC_NAME, "ptr", FUNC_SIZE, FUNC_ALIGN, generics, TypeFlags.NONE);
    }
}

[Flags]
public enum TypeFlags
{
    NONE,
    UNSIGNED,
    INTEGER,
    FLOAT,
}