using System;
using ILS.Binding;
using ILS.Binding.Expressions;
using ILS.IO;
using ILS.Lexing;

namespace ILS.Emitting;

public sealed partial class Emitter
{
    public string EmitMetaExpression(BoundExpression expression)
    {
        switch (expression.type)
        {
            case NodeType.VARIABLE_EXPRESSION:
                return EmitMetaVariableExpression((BoundVariableExpression)expression);
            case NodeType.DEREFERENCE_EXPRESSION:
                return EmitMetaDeReferenceExpression((BoundDeReferenceExpression)expression);
            case NodeType.MEMBER_EXPRESSION:
                return EmitMetaMemberAccessExpression((BoundMemberAccessExpression)expression);
            default:
                throw new Exception("Unknown meta expression");
        }
    }

    private string EmitMetaVariableExpression(BoundVariableExpression expression)
    {
        return expression.variable.llvmName;
    }

    private string EmitMetaDeReferenceExpression(BoundDeReferenceExpression expression)
    {
        return EmitExpression(expression.expression);
    }

    private string EmitMetaMemberAccessExpression(BoundMemberAccessExpression expression)
    {
        string target = EmitMetaExpression(expression.target);
        string property = NextLabel();
        writer.WriteIntend(property);
        writer.Write(" = getelementptr inbounds ");
        writer.Write(expression.target.returnType.llvmName);
        writer.Write(", ptr ");
        writer.Write(target);
        writer.Write(", i32 0, i32 ");
        writer.WriteLine(expression.offset);

        return property;
    }
}