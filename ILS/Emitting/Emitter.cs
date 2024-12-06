using System;
using System.IO;
using ILS.Binding;
using ILS.Binding.Expressions;
using ILS.Binding.Members;
using ILS.Binding.Statements;
using ILS.Binding.Symbols;
using ILS.IO;
using ILS.Lexing;

namespace ILS.Emitting;

public sealed partial class Emitter
{
    private readonly TextWriter writer;
    private int labelCounter;
    private int extLabelCounter;
    private string currentBreakLabel;
    private string currentContinueLabel;

    public Emitter(TextWriter writer)
    {
        this.writer = writer;
        this.labelCounter = 1;
    }

    private string NextLabel()
    {
        return "%" + labelCounter++;
    }

    private string NextExtLabel()
    {
        return "EL" + extLabelCounter++;
    }

    public void EmitModule(BoundModule module)
    {
        foreach (BoundFunctionMember member in module.functions)
        {
            EmitFunctionMember(member);
        }
    }

    private void EmitFunctionMember(BoundFunctionMember member)
    {
        writer.Write("define dso_local ");
        writer.Write(member.symbol.returnType.llvmName);
        writer.Write(" ");
        writer.Write(member.symbol.llvmName);
        writer.Write("(");
        for (int i = 0; i < member.symbol.parameters.Length; i++)
        {
            if (i != 0)
            {
                writer.Write(", ");
            }
            VariableSymbol parameter = member.symbol.parameters[i];
            writer.Write(parameter.type.llvmName);
            writer.Write(" %");
            writer.Write(parameter.name);
        }
        writer.WriteLine(") {");

        foreach (VariableSymbol parameter in member.symbol.parameters)
        {
            writer.WriteIntend(parameter.llvmName);
            writer.Write(" = alloca ");
            writer.Write(parameter.type.llvmName);
            writer.Write(", align ");
            writer.WriteLine(parameter.type.align);
        }
        foreach (VariableSymbol parameter in member.symbol.parameters)
        {
            writer.WriteIntend("store ");
            writer.Write(parameter.type.llvmName);
            writer.Write(" %");
            writer.Write(parameter.name);
            writer.Write(", ptr ");
            writer.Write(parameter.llvmName);
            writer.Write(", align ");
            writer.WriteLine(parameter.type.align);
        }

        EmitStatement(member.body);
        writer.WriteLine("    ret void");
        writer.WriteLine("}");
    }

    private void EmitStatement(BoundStatement statement)
    {
		switch(statement.type) {
			case NodeType.BLOCK_STATEMENT:
				EmitBlockStatement((BoundBlockStatement)statement);
				break;
			case NodeType.VARIABLE_STATEMENT:
				EmitVariableStatement((BoundVariableStatement)statement);
				break;
			case NodeType.IF_STATEMENT:
				EmitIfStatement((BoundIfStatement)statement);
				break;
			case NodeType.WHILE_STATEMENT:
				EmitWhileStatement((BoundWhileStatement)statement);
				break;
			case NodeType.BREAK_STATEMENT:
				EmitBreakStatement((BoundBreakStatement)statement);
				break;
			case NodeType.CONTINUE_STATEMENT:
				EmitContinueStatement((BoundContinueStatement)statement);
				break;
			case NodeType.RETURN_STATEMENT:
				EmitReturnStatement((BoundReturnStatement)statement);
				break;
	    	case NodeType.EXPRESSION_STATEMENT:
				EmitExpressionStatement((BoundExpressionStatement)statement);
				break;
    	}
    }

    private void EmitBlockStatement(BoundBlockStatement statement)
    {
        foreach (BoundStatement sth in statement.statements)
        {
            EmitStatement(sth);
        }
    }

    private void EmitVariableStatement(BoundVariableStatement statement)
    {
        writer.WriteIntend(statement.variable.llvmName);
        writer.Write(" = alloca ");
        writer.Write(statement.variable.type.llvmName);
        writer.Write(", align ");
        writer.WriteLine(statement.variable.type.align);

        EmitExpression(new BoundAssignmentExpression(new BoundVariableExpression(statement.variable), statement.initializer));
    }

    private void EmitIfStatement(BoundIfStatement statement)
    {
        bool elseExists = statement.elseBlock != null;
        string condition = EmitExpression(statement.condition);
        string thenBlock = NextExtLabel();
        string nextBlock = NextExtLabel();
        string endBlock = "?";

        writer.WriteIntend("br i1 ");
        writer.Write(condition);
        writer.Write(", label %");
        writer.Write(thenBlock);
        writer.Write(", label %");
        writer.WriteLine(nextBlock);

        writer.Write(thenBlock);
        writer.WriteLine(":");

        EmitStatement(statement.thenBlock);

        if (elseExists)
        {
            endBlock = NextExtLabel();
        }

        if (!EndsWithControlFlow(statement.thenBlock))
        {
            writer.WriteIntend("br label %");
            writer.WriteLine(elseExists ? endBlock : nextBlock);
        }

        writer.Write(nextBlock);
        writer.WriteLine(":");

        if (elseExists)
        {
            EmitStatement(statement.elseBlock);

            if (!EndsWithControlFlow(statement.elseBlock))
            {
                writer.WriteIntend("br label %");
                writer.WriteLine(endBlock);
            }

            writer.Write(endBlock);
            writer.WriteLine(":");
        }
    }

    private void EmitWhileStatement(BoundWhileStatement statement)
    {
        string startLabel = NextExtLabel();

        writer.WriteIntend("br label %");
        writer.WriteLine(startLabel);

        writer.Write(startLabel);
        writer.WriteLine(":");

        string condition = EmitExpression(statement.condition);
        string bodyLabel = NextExtLabel();
        string endLabel = NextExtLabel();

        writer.WriteIntend("br i1 ");
        writer.Write(condition);
        writer.Write(", label %");
        writer.Write(bodyLabel);
        writer.Write(", label %");
        writer.WriteLine(endLabel);

        writer.Write(bodyLabel);
        writer.WriteLine(":");

        string _currentBreakLabel = currentBreakLabel;
        string _currentContinueLabel = currentContinueLabel;
        currentBreakLabel = endLabel;
        currentContinueLabel = startLabel;
        EmitStatement(statement.body);
        currentBreakLabel = _currentBreakLabel;
        currentContinueLabel = _currentContinueLabel;

        writer.WriteIntend("br label %");
        writer.WriteLine(startLabel);

        writer.Write(endLabel);
        writer.WriteLine(":");
    }

    private void EmitBreakStatement(BoundBreakStatement statement)
    {
        writer.WriteIntend("br label %");
        writer.WriteLine(currentBreakLabel);
    }

    private void EmitContinueStatement(BoundContinueStatement statement)
    {
        writer.WriteIntend("br label %");
        writer.WriteLine(currentContinueLabel);
    }

	private void EmitReturnStatement(BoundReturnStatement statement) {
		if (statement.value == null) {
			writer.WriteIntend("ret void");
			writer.WriteLine();
			return;
		}		

		string label = EmitExpression(statement.value);

		writer.WriteIntend("ret ");
		writer.Write(statement.value.returnType.llvmName);
		writer.Write(" ");
		writer.WriteLine(label);
		
	}
	
    private void EmitExpressionStatement(BoundExpressionStatement statement)
    {
        EmitExpression(statement.expression);
    }

    private string EmitExpression(BoundExpression expression)
    {
		switch(expression.type) {
			case NodeType.INT_EXPRESSION:
				return EmitIntExpression((BoundIntExpression)expression);
			case NodeType.BOOL_EXPRESSION:
				return EmitBoolExpression((BoundBoolExpression)expression);
			case NodeType.UNARY_EXPRESSION:
				return EmitUnaryExpression((BoundUnaryExpression)expression);
			case NodeType.BINARY_EXPRESSION:
				return EmitBinaryExpression((BoundBinaryExpression)expression);
			case NodeType.ASSIGNMENT_EXPRESSION:
				return EmitAssignmentExpression((BoundAssignmentExpression)expression);
			case NodeType.VARIABLE_EXPRESSION:
				return EmitVariableExpression((BoundVariableExpression)expression);
            case NodeType.FUNCTION_EXPRESSION:
                return EmitFunctionExpression((BoundFunctionExpression)expression);
            case NodeType.VARIABLE_REFERENCE_EXPRESSION:
				return EmitVariableReferenceExpression((BoundVariableReferenceExpression)expression);
			case NodeType.CONVERSION_EXPRESSION:
				return EmitConversionExpression((BoundConversionExpression)expression);
			case NodeType.REINTERPRETATION_EXPRESSION:
				return EmitReinterpretationExpression((BoundReinterpretationExpression)expression);
			case NodeType.DEREFERENCE_EXPRESSION:
				return EmitDeReferenceExpression((BoundDeReferenceExpression)expression);
			case NodeType.TERNARY_EXPRESSION:
				return EmitTernaryExpression((BoundTernaryExpression)expression);
			default:
				throw new Exception("Unknown expression");
		}
    }

    private string EmitIntExpression(BoundIntExpression expression)
    {
        return expression.value;
    }

    private string EmitBoolExpression(BoundBoolExpression expression)
    {
        return expression.value == NodeType.TRUE_KEYWORD ? "1" : "0";
    }

    private string EmitUnaryExpression(BoundUnaryExpression expression)
    {
        NodeType operatorType = expression.unaryOperator.type;
        string right = EmitExpression(expression.right);

        if (operatorType == NodeType.PLUS_TOKEN)
        {
            return right;
        }
        if (operatorType == NodeType.MINUS_TOKEN)
        {
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = ");
            writer.Write("sub nsw ");
            writer.Write(expression.right.returnType.llvmName);
            writer.Write(" 0, ");
            writer.WriteLine(right);
            return result;
        }
        if (operatorType == NodeType.BANG_TOKEN)
        {
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = xor ");
            writer.Write(expression.right.returnType.llvmName);
            writer.Write(" ");
            writer.Write(right);
            writer.WriteLine(", 1");
            return result;
        }

        throw new Exception("Unexpected unary operation");
    }

    private string EmitBinaryExpression(BoundBinaryExpression expression)
    {
        NodeType operatorType = expression.binaryOperator.type;

        if (operatorType == NodeType.PLUS_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string right = EmitExpression(expression.right);
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = ");
            writer.Write("add ");
            if ((expression.binaryOperator.leftType.flags & TypeFlags.UNSIGNED) == 0)
            {
                writer.Write("nsw ");
            }
            writer.Write(expression.returnType.llvmName);
            writer.Write(" ");
            writer.Write(left);
            writer.Write(", ");
            writer.WriteLine(right);

            return result;
        }
        if (operatorType == NodeType.MINUS_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string right = EmitExpression(expression.right);
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = ");
            writer.Write("sub ");
            if ((expression.binaryOperator.leftType.flags & TypeFlags.UNSIGNED) == 0)
            {
                writer.Write("nsw ");
            }
            writer.Write(expression.returnType.llvmName);
            writer.Write(" ");
            writer.Write(left);
            writer.Write(", ");
            writer.WriteLine(right);

            return result;
        }
        if (operatorType == NodeType.STAR_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string right = EmitExpression(expression.right);
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = ");
            writer.Write("mul ");
            if ((expression.binaryOperator.leftType.flags & TypeFlags.UNSIGNED) == 0)
            {
                writer.Write("nsw ");
            }
            writer.Write(expression.returnType.llvmName);
            writer.Write(" ");
            writer.Write(left);
            writer.Write(", ");
            writer.WriteLine(right);

            return result;
        }
        if (operatorType == NodeType.SLASH_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string right = EmitExpression(expression.right);
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = ");
            if ((expression.binaryOperator.leftType.flags & TypeFlags.UNSIGNED) == 0)
            {
                writer.Write("sdiv ");
            }
            else
            {
                writer.Write("udiv ");
            }
            writer.Write(expression.returnType.llvmName);
            writer.Write(" ");
            writer.Write(left);
            writer.Write(", ");
            writer.WriteLine(right);

            return result;
        }
        if (operatorType == NodeType.AND_AND_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string startLabel = NextExtLabel();
            string ifLabel = NextExtLabel();
            string endLabel = NextExtLabel();

            writer.WriteIntend("br label %");
            writer.WriteLine(startLabel);

            writer.Write(startLabel);
            writer.WriteLine(":");

            writer.WriteIntend("br i1 ");
            writer.Write(left);
            writer.Write(", label %");
            writer.Write(ifLabel);
            writer.Write(", label %");
            writer.WriteLine(endLabel);

            writer.Write(ifLabel);
            writer.WriteLine(":");

            string right = EmitExpression(expression.right);
            writer.WriteIntend("br label %");
            writer.WriteLine(endLabel);

            writer.Write(endLabel);
            writer.WriteLine(":");

            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = phi ");
            writer.Write(TypeSymbol.boolean.llvmName);
            writer.Write("[ false, %");
            writer.Write(startLabel);
            writer.Write(" ], [ ");
            writer.Write(right);
            writer.Write(", %");
            writer.Write(ifLabel);
            writer.WriteLine(" ]");

            return result;
        }
        if (operatorType == NodeType.PIPE_PIPE_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string startLabel = NextExtLabel();
            string ifLabel = NextExtLabel();
            string endLabel = NextExtLabel();

            writer.WriteIntend("br label %");
            writer.WriteLine(startLabel);

            writer.Write(startLabel);
            writer.WriteLine(":");

            writer.WriteIntend("br i1 ");
            writer.Write(left);
            writer.Write(", label %");
            writer.Write(endLabel);
            writer.Write(", label %");
            writer.WriteLine(ifLabel);

            writer.Write(ifLabel);
            writer.WriteLine(":");

            string right = EmitExpression(expression.right);
            writer.WriteIntend("br label %");
            writer.WriteLine(endLabel);

            writer.Write(endLabel);
            writer.WriteLine(":");

            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = phi ");
            writer.Write(TypeSymbol.boolean.llvmName);
            writer.Write("[ true, %");
            writer.Write(startLabel);
            writer.Write(" ], [ ");
            writer.Write(right);
            writer.Write(", %");
            writer.Write(ifLabel);
            writer.WriteLine(" ]");

            return result;
        }
        if (operatorType == NodeType.EQUALS_EQUALS_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string right = EmitExpression(expression.right);
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = ");
            writer.Write("icmp eq ");
            writer.Write(expression.binaryOperator.leftType.llvmName);
            writer.Write(" ");
            writer.Write(left);
            writer.Write(", ");
            writer.WriteLine(right);

            return result;
        }
        if (operatorType == NodeType.BANG_EQUALS_TOKEN)
        {
            string left = EmitExpression(expression.left);
            string right = EmitExpression(expression.right);
            string result = NextLabel();
            writer.WriteIntend(result);
            writer.Write(" = ");
            writer.Write("icmp ne ");
            writer.Write(expression.binaryOperator.leftType.llvmName);
            writer.Write(" ");
            writer.Write(left);
            writer.Write(", ");
            writer.WriteLine(right);

            return result;
        }

        throw new Exception("Unexpected binary operation");
    }

    private string EmitAssignmentExpression(BoundAssignmentExpression expression)
    {
        string field = EmitMetaExpression(expression.fieldExpression);
        string right = EmitExpression(expression.expression);

        writer.WriteIntend("store ");
        writer.Write(expression.fieldExpression.returnType.llvmName);
        writer.Write(" ");
        writer.Write(right);
        writer.Write(", ptr ");
        writer.Write(field);
        writer.Write(", align ");
        writer.WriteLine(expression.fieldExpression.returnType.align);

        return right;
    }

    private string EmitVariableExpression(BoundVariableExpression expression)
    {
        string result = NextLabel();

        writer.WriteIntend(result);
        writer.Write(" = load ");
        writer.Write(expression.variable.type.llvmName);
        writer.Write(", ptr ");
        writer.Write(expression.variable.llvmName);
        writer.Write(", align ");
        writer.WriteLine(expression.variable.type.align);

        return result;
    }

    private string EmitFunctionExpression(BoundFunctionExpression expression)
    {
        return expression.function.llvmName;
    }

    private string EmitVariableReferenceExpression(BoundVariableReferenceExpression expression)
    {
        return expression.variable.llvmName;
    }

    private string EmitConversionExpression(BoundConversionExpression expression)
    {
        string right = EmitExpression(expression.expression);
        string result = NextLabel();
        writer.WriteIntend(result);
        writer.Write(" = ");

        if (expression.expression.returnType == TypeSymbol.i32 && expression.returnType == TypeSymbol.boolean)
        {
            writer.Write("icmp ne ");
            writer.Write(expression.expression.returnType.llvmName);
            writer.Write(" ");
            writer.Write(right);
            writer.WriteLine(", 0");
            return result;
        }

        if (expression.isPromotion)
        {
            writer.Write("sext ");
            writer.Write(expression.expression.returnType.llvmName);
            writer.Write(" ");
            writer.Write(right);
            writer.Write(" to ");
            writer.WriteLine(expression.returnType.llvmName);
        }
        else
        {
            writer.Write("trunc ");
            writer.Write(expression.expression.returnType.llvmName);
            writer.Write(" ");
            writer.Write(right);
            writer.Write(" to ");
            writer.WriteLine(expression.returnType.llvmName);
        }

        return result;
    }

    private string EmitReinterpretationExpression(BoundReinterpretationExpression expression)
    {
        string right = EmitExpression(expression.expression);

        if (expression.expression.returnType.name == TypeSymbol.PTR_NAME && expression.returnType.name == TypeSymbol.PTR_NAME)
        {
            return right;
        }
        if (expression.expression.returnType.llvmName == expression.returnType.llvmName)
        {
            return right;
        }

        string result = NextLabel();
        writer.WriteIntend(result);
        writer.Write(" = ");

        if (expression.expression.returnType.name == TypeSymbol.PTR_NAME && (expression.returnType.flags & TypeFlags.INTEGER) > 0 &&
            expression.returnType.size == TypeSymbol.PTR_SIZE)
        {
            writer.Write("ptrtoint ptr ");
            writer.Write(right);
            writer.Write(" to ");
            writer.WriteLine(expression.returnType.llvmName);
        }
        else if (expression.returnType.name == TypeSymbol.PTR_NAME && (expression.expression.returnType.flags & TypeFlags.INTEGER) > 0 &&
                 expression.expression.returnType.size == TypeSymbol.PTR_SIZE)
        {
            writer.Write("inttoptr ");
            writer.Write(expression.expression.returnType.name);
            writer.Write(" ");
            writer.Write(right);
            writer.WriteLine(" to ptr");
        }
        else
        {
            throw new Exception("Conversion not implemented");
        }


        return result;
    }

    private string EmitDeReferenceExpression(BoundDeReferenceExpression expression)
    {
        string right = EmitExpression(expression.expression);
        string result = NextLabel();

        writer.WriteIntend(result);
        writer.Write(" = load ");
        writer.Write(expression.returnType.llvmName);
        writer.Write(", ptr ");
        writer.Write(right);
        writer.Write(", align ");
        writer.WriteLine(expression.returnType.align);

        return result;
    }

    private string EmitTernaryExpression(BoundTernaryExpression expression)
    {
        string startLabel = NextExtLabel();

        writer.WriteIntend("br label %");
        writer.WriteLine(startLabel);

        writer.Write(startLabel);
        writer.WriteLine(":");

        string condition = EmitExpression(expression.condition);
        string trueLabel = NextExtLabel();
        string falseLabel = NextExtLabel();
        string endLabel = NextExtLabel();

        writer.WriteIntend("br i1 ");
        writer.Write(condition);
        writer.Write(", label %");
        writer.Write(trueLabel);
        writer.Write(", label %");
        writer.WriteLine(falseLabel);

        writer.Write(trueLabel);
        writer.WriteLine(":");

        string thenValue = EmitExpression(expression.thenExpression);
        writer.WriteIntend("br label %");
        writer.WriteLine(endLabel);

        writer.Write(falseLabel);
        writer.WriteLine(":");

        string elseValue = EmitExpression(expression.elseExpression);
        writer.WriteIntend("br label %");
        writer.WriteLine(endLabel);

        writer.Write(endLabel);
        writer.WriteLine(":");

        string result = NextLabel();

        writer.WriteIntend(result);
        writer.Write(" = phi ");
        writer.Write(expression.returnType.llvmName);
        writer.Write(" [ ");
        writer.Write(thenValue);
        writer.Write(", %");
        writer.Write(trueLabel);
        writer.Write(" ], [ ");
        writer.Write(elseValue);
        writer.Write(", %");
        writer.Write(falseLabel);
        writer.WriteLine(" ]");

        return result;
    }

    private bool EndsWithControlFlow(BoundStatement statement)
    {
        if (statement.type == NodeType.BLOCK_STATEMENT)
        {
            BoundBlockStatement blockStatement = (BoundBlockStatement)statement;
            if (blockStatement.statements.Length > 0)
            {
                return EndsWithControlFlow(blockStatement.statements[blockStatement.statements.Length - 1]);
            }
        }
        if (statement.type == NodeType.BREAK_STATEMENT ||
            statement.type == NodeType.CONTINUE_STATEMENT)
        {
            return true;
        }

        return false;
    }
}
