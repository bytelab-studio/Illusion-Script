using System;
using System.Linq;

namespace ILS.Binding.Symbols;

public sealed class TypeSymbol
{
    public const int STRUCT_ALIGN = 8;

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
    public readonly StructItemSymbol[] items;
    public readonly TypeFlags flags;

    public TypeSymbol(bool primitive, string name, string llvmName, int size, int align, TypeSymbol[] generics, StructItemSymbol[] items, TypeFlags flags)
    {
        this.primitive = primitive;
        this.fullName = name + "<" + string.Join(", ", generics.Select(generic => generic.fullName)) + ">";
        this.name = name;
        this.llvmName = llvmName;
        this.size = size;
        this.align = align;
        this.generics = generics;
        this.items = items;
        this.flags = flags;
    }

    public TypeSymbol(string name, string llvmName, int size, int align, TypeFlags flags) : this(
        true,
        name,
        llvmName,
        size,
        align,
        Array.Empty<TypeSymbol>(),
        Array.Empty<StructItemSymbol>(),
        flags)
    {
    }

    public bool Equals(TypeSymbol otherType)
    {
        return fullName == otherType.fullName;
    }


    public static readonly TypeSymbol u8 = new TypeSymbol("u8", "i8", 1, 1, TypeFlags.INTEGER | TypeFlags.UNSIGNED);
    public static readonly TypeSymbol u16 = new TypeSymbol("u16", "i16", 2, 2, TypeFlags.INTEGER | TypeFlags.UNSIGNED);
    public static readonly TypeSymbol u32 = new TypeSymbol("u32", "i32", 4, 4, TypeFlags.INTEGER | TypeFlags.UNSIGNED);
    public static readonly TypeSymbol u64 = new TypeSymbol("u64", "i64", 8, 8, TypeFlags.INTEGER | TypeFlags.UNSIGNED);

    public static readonly TypeSymbol i8 = new TypeSymbol("i8", "i8", 1, 1, TypeFlags.INTEGER);
    public static readonly TypeSymbol i16 = new TypeSymbol("i16", "i16", 2, 2, TypeFlags.INTEGER);
    public static readonly TypeSymbol i32 = new TypeSymbol("i32", "i32", 4, 4, TypeFlags.INTEGER);
    public static readonly TypeSymbol i64 = new TypeSymbol("i64", "i64", 8, 8, TypeFlags.INTEGER);

    public static readonly TypeSymbol voidType = new TypeSymbol("void", "void", 0, 0, 0);
    public static readonly TypeSymbol boolean = new TypeSymbol("boolean", "i1", 1, 1, 0);
    public static readonly TypeSymbol error = new TypeSymbol("?", "?", 0, 0, 0);

    public static TypeSymbol Reference(TypeSymbol baseType)
    {
        return new TypeSymbol(true, "PTR", "ptr", PTR_SIZE, PTR_ALIGN, new[]
        {
            baseType
        }, Array.Empty<StructItemSymbol>(), TypeFlags.INTEGER | TypeFlags.UNSIGNED);
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
        return new TypeSymbol(true, FUNC_NAME, "ptr", FUNC_SIZE, FUNC_ALIGN, generics, Array.Empty<StructItemSymbol>(), TypeFlags.NONE);
    }

    public static TypeSymbol Struct(string name, StructItemSymbol[] items)
    {
        int size = items.Select(item => item.type.size).Sum();
        return new TypeSymbol(true, name, "%" + name, size, STRUCT_ALIGN, Array.Empty<TypeSymbol>(), items, TypeFlags.STRUCT);
    }
}

[Flags]
public enum TypeFlags
{
    NONE,
    UNSIGNED,
    INTEGER,
    FLOAT,
    STRUCT
}