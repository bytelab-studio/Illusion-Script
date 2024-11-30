using System;
using ILS.Binding;
using ILS.Binding.Expressions;
using ILS.Lexing;

namespace ILS.Emitting;

public sealed partial class Emitter
{
    public string EmitMetaExpression(BoundExpression expression)
    {
	 	switch(expression.type) {
			case NodeType.VARIABLE_EXPRESSION:
				return EmitMetaVariableExpression((BoundVariableExpression)expression);
			case NodeType.DEREFERENCE_EXPRESSION:
				return EmitMetaDeReferenceExpression((BoundDeReferenceExpression)expression);
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
}
