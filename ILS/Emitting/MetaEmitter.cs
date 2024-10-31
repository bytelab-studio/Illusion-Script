using System;
using ILS.Binding;
using ILS.Binding.Expressions;
using ILS.Lexing;

namespace ILS.Emitting;

public sealed partial class Emitter
{
    public string EmitMetaExpression(BoundExpression expression)
    {
        if (expression.type == NodeType.VARIABLE_EXPRESSION)
        {
            return EmitMetaVariableExpression((BoundVariableExpression)expression);
        }
        if (expression.type == NodeType.DEREFERENCE_EXPRESSION)
        {
            return EmitMetaDeReferenceExpression((BoundDeReferenceExpression)expression);
        }

        throw new Exception("Unknown meta expression");
    }

    private string EmitMetaVariableExpression(BoundVariableExpression expression)
    {
        return expression.variable.llvmName;
    }

    private string EmitMetaDeReferenceExpression(BoundDeReferenceExpression expression)
    {
        return EmitExpression(expression.expression);
    }
}