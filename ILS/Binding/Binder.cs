using System;
using System.Collections.Generic;
using System.Linq;
using ILS.CFA;
using ILS.Binding.Expressions;
using ILS.Binding.Members;
using ILS.Binding.Operation;
using ILS.Binding.Statements;
using ILS.Binding.Symbols;
using ILS.Lexing;
using ILS.Parsing.Nodes;
using ILS.Parsing.Nodes.Expressions;
using ILS.Parsing.Nodes.Members;
using ILS.Parsing.Nodes.Statements;

namespace ILS.Binding;

public enum ConversionResult
{
    // Self explaining
    NONE,

    // Self explaining
    IDENTICAL,

    // A smaller type gets promoted to a larger type: i32 -> i64, i1 -> i32
    PROMOTION,

    // A larger type get casted to a smaller type: i32 -> i1, i64 -> i32
    CAST,

    // A type gets converted into another type with the same type: PTR<i32> -> i64 where (sizeof(PTR) == sizeof(i64))
    CONVERSION,
}

public sealed class Binder
{
    private DiagnosticBag diagnostics;
    private Scope scope;
    private int localVariableCounter;
    private int loopCounter;
	private TypeSymbol returnType;

    public Binder(Scope globalScope, TypeSymbol returnType = null)
    {
        diagnostics = new DiagnosticBag();
        scope = new Scope(globalScope);
		this.returnType = returnType;
        localVariableCounter = 1;
        loopCounter = 0;
    }

    public IEnumerable<Diagnostic> Diagnostics()
    {
        return diagnostics.diagnostics;
    }

    private BoundStatement BindStatement(Statement statement)
    {
        switch (statement.type)
        {
            case NodeType.BLOCK_STATEMENT:
                return BindBlockStatement((BlockStatement)statement);
            case NodeType.VARIABLE_STATEMENT:
                return BindVariableStatement((VariableStatement)statement);
            case NodeType.EXPRESSION_STATEMENT:
                return BindExpressionStatement((ExpressionStatement)statement);
            case NodeType.IF_STATEMENT:
                return BindIfStatement((IfStatement)statement);
            case NodeType.WHILE_STATEMENT:
                return BindWhileStatement((WhileStatement)statement);
            case NodeType.BREAK_STATEMENT:
                return BindBreakStatement((BreakStatement)statement);
            case NodeType.CONTINUE_STATEMENT:
                return BindContinueStatement((ContinueStatement)statement);
            case NodeType.RETURN_STATEMENT:
				return BindReturnStatement((ReturnStatement)statement);default:
                throw new Exception("Unexpected statement");
        }
    }

    private BoundBlockStatement BindBlockStatement(BlockStatement statement)
    {
        scope = new Scope(scope);

        List<BoundStatement> statements = new List<BoundStatement>();
        foreach (Statement sth in statement.statements)
        {
            statements.Add(BindStatement(sth));
        }

        scope = scope.parent;
        return new BoundBlockStatement(statements.ToArray());
    }

    private BoundStatement BindVariableStatement(VariableStatement statement)
    {
        VariableSymbol symbol = new VariableSymbol(
            statement.keywordToken.type == NodeType.CONST_KEYWORD,
            statement.identifierToken.text,
            BindTypeClause(statement.typeClause, false),
            localVariableCounter++
        );
        BoundExpression initializer = BindExpectedExpression(statement.initializer, symbol.type);
        if (!scope.TryDeclareVariable(symbol))
        {
            diagnostics.ReportSymbolAlreadyDefined(statement.identifierToken.span, symbol.name);
        }

        return new BoundVariableStatement(symbol, initializer);
    }

    private BoundStatement BindExpressionStatement(ExpressionStatement statement)
    {
        BoundExpression expression = BindExpression(statement.expression);
        if (expression.type != NodeType.ASSIGNMENT_EXPRESSION &&
            expression.type != NodeType.CALL_EXPRESSION)
        {
            diagnostics.ReportExpressionNotAllowed(statement.expression.span, statement.expression.type);
            expression = new BoundErrorExpression();
        }
        return new BoundExpressionStatement(expression);
    }

    private BoundStatement BindIfStatement(IfStatement statement)
    {
        BoundExpression condition = BindExpectedExpression(statement.condition, TypeSymbol.boolean);
        BoundStatement thenBody = BindStatement(statement.body);
        BoundStatement elseBody = null;
        if (statement.elseStatement != null)
        {
            elseBody = BindStatement(statement.elseStatement.body);
        }
        return new BoundIfStatement(condition, thenBody, elseBody);
    }

    private BoundStatement BindWhileStatement(WhileStatement statement)
    {
        BoundExpression condition = BindExpectedExpression(statement.condition, TypeSymbol.boolean);
        loopCounter++;
        BoundStatement body = BindStatement(statement.body);
        loopCounter--;
        return new BoundWhileStatement(condition, body);
    }

    private BoundStatement BindBreakStatement(BreakStatement statement)
    {
        if (loopCounter == 0)
        {
            diagnostics.ReportNotInLoop(statement.span, statement.type);
        }

        return new BoundBreakStatement();
    }

    private BoundStatement BindContinueStatement(ContinueStatement statement)
    {
        if (loopCounter == 0)
        {
            diagnostics.ReportNotInLoop(statement.span, statement.type);
        }

        return new BoundContinueStatement();
    }

	private BoundStatement BindReturnStatement(ReturnStatement statement) {
		if (statement.value == null && returnType != TypeSymbol.voidType) {
			diagnostics.ReportReturnRequiresValue(statement.returnToken.span);
			return new BoundReturnStatement(null);
		}
		if (statement.value != null && returnType == TypeSymbol.voidType) {
			diagnostics.ReportReturnRequiresNoValue(statement.returnToken.span);
			return new BoundReturnStatement(null);
		}

		BoundExpression value = BindExpectedExpression(statement.value, returnType);
		return new BoundReturnStatement(value);
	}

    private ConversionResult ClassifyConversion(BoundExpression expression, TypeSymbol toType, bool isExplicit, bool allowReinterpretation)
    {
        TypeSymbol fromType = expression.returnType;

        // INT Expression can be implicit assigned to any integer type except PTR
        if (expression.type == NodeType.INT_EXPRESSION &&
            (toType.flags & TypeFlags.INTEGER) > 0 &&
            toType.name != TypeSymbol.PTR_NAME)
        {
            return ConversionResult.IDENTICAL;
        }

        // IDENTICAL
        if (fromType.Equals(toType))
        {
            return ConversionResult.IDENTICAL;
        }

        // iX TO uX PTR CONVERSION
        if (isExplicit &&
            allowReinterpretation &&
            (fromType.flags & TypeFlags.INTEGER) > 0 &&
            (toType.flags & (TypeFlags.INTEGER | TypeFlags.UNSIGNED)) > 0)
        {
            return ConversionResult.CONVERSION;
        }

        // uX TO iX PTR CONVERSION
        if (isExplicit &&
            allowReinterpretation &&
            (toType.flags & TypeFlags.INTEGER) > 0 &&
            (fromType.flags & (TypeFlags.INTEGER | TypeFlags.UNSIGNED)) > 0)
        {
            return ConversionResult.CONVERSION;
        }

        // PTR TO uX CONVERSION
        if (fromType.name == TypeSymbol.PTR_NAME &&
            toType.name != TypeSymbol.PTR_NAME &&
            (toType.flags & (TypeFlags.INTEGER | TypeFlags.UNSIGNED)) > 0 &&
            toType.size == TypeSymbol.PTR_SIZE)
        {
            return ConversionResult.CONVERSION;
        }

        // uX TO PTR CONVERSION
        if (isExplicit &&
            allowReinterpretation &&
            toType.name == TypeSymbol.PTR_NAME &&
            (fromType.flags & (TypeFlags.INTEGER | TypeFlags.UNSIGNED)) > 0 &&
            fromType.size == TypeSymbol.PTR_SIZE)
        {
            return ConversionResult.CONVERSION;
        }

        // PTR<void> TO PTR<?> CONVERSION
        if (isExplicit &&
            allowReinterpretation &&
            toType.name == TypeSymbol.PTR_NAME &&
            fromType.name == TypeSymbol.PTR_NAME &&
            fromType.generics[0].name == TypeSymbol.voidType.name)
        {
            return ConversionResult.CONVERSION;
        }

        // PTR<?> TO PTR<void> CONVERSION
        if (allowReinterpretation &&
            toType.name == TypeSymbol.PTR_NAME &&
            toType.generics[0].name == TypeSymbol.voidType.name &&
            fromType.name == TypeSymbol.PTR_NAME)
        {
            return ConversionResult.CONVERSION;
        }

        if (toType.name == TypeSymbol.PTR_NAME ||
            fromType.name == TypeSymbol.PTR_NAME)
        {
            return ConversionResult.NONE;
        }

        // PROMOTION
        if (fromType.flags == toType.flags &&
            fromType.size < toType.size)
        {
            return ConversionResult.PROMOTION;
        }

        // CAST
        if (isExplicit && fromType.flags == toType.flags &&
            fromType.size > toType.size)
        {
            return ConversionResult.CAST;
        }

        // BOOLEAN TO INTEGER PROMOTION
        if (fromType == TypeSymbol.boolean && (toType.flags & TypeFlags.INTEGER) > 0)
        {
            return ConversionResult.PROMOTION;
        }

        // CAST INTEGER TO BOOLEAN
        if (isExplicit && (fromType.flags & TypeFlags.INTEGER) > 0 && toType == TypeSymbol.boolean)
        {
            return ConversionResult.CAST;
        }

        return ConversionResult.NONE;
    }

    private BoundExpression ClassifyConversion(TextSpan span, BoundExpression expression, TypeSymbol toType, bool isExplicit, bool allowReinterpretation)
    {
        ConversionResult result = ClassifyConversion(expression, toType, isExplicit, allowReinterpretation);

        if (result == ConversionResult.IDENTICAL)
        {
            return expression;
        }

        if (result == ConversionResult.CONVERSION)
        {
            return new BoundReinterpretationExpression(expression, toType);
        }
        if (result == ConversionResult.CAST || result == ConversionResult.PROMOTION)
        {
            return new BoundConversionExpression(expression, toType, result == ConversionResult.PROMOTION);
        }

        if (expression.returnType == TypeSymbol.error || toType == TypeSymbol.error)
        {
            return new BoundErrorExpression();
        }

        if (!isExplicit)
        {
            diagnostics.ReportImplicitConversion(span, expression.returnType, toType);
        }
        else
        {
            diagnostics.ReportConversion(span, expression.returnType, toType);
        }
        return new BoundErrorExpression();
    }

    private BoundExpression BindExpectedExpression(Expression expression, TypeSymbol expectedType)
    {
        BoundExpression boundExpression = BindExpression(expression);
        if (boundExpression.returnType != expectedType)
        {
            return ClassifyConversion(expression.span, boundExpression, expectedType, false, false);
        }

        return boundExpression;
    }

    private BoundExpression BindExpression(Expression expression)
    {
        switch (expression.type)
        {
            case NodeType.INT_EXPRESSION:
                return BindIntExpression((IntExpression)expression);
            case NodeType.BOOL_EXPRESSION:
                return BindBoolExpression((BoolExpression)expression);
            case NodeType.UNARY_EXPRESSION:
                return BindUnaryExpression((UnaryExpression)expression);
            case NodeType.BINARY_EXPRESSION:
                return BindBinaryExpression((BinaryExpression)expression);
            case NodeType.PAREN_EXPRESSION:
                return BindExpression(((ParenExpression)expression).expression);
            case NodeType.ASSIGNMENT_EXPRESSION:
                return BindAssignmentExpression((AssignmentExpression)expression);
            case NodeType.NAME_EXPRESSION:
                return BindNameExpression((NameExpression)expression);
            case NodeType.CONVERSION_EXPRESSION:
                return BindConversionExpression((ConversionExpression)expression);
            case NodeType.REINTERPRETATION_EXPRESSION:
                return BindReinterpretationExpression((ReinterpretationExpression)expression);
            case NodeType.INCREMENT_EXPRESSION:
                return BindIncrementExpression((IncrementExpression)expression);
            case NodeType.DECREMENT_EXPRESSION:
                return BindDecrementExpression((DecrementExpression)expression);
            case NodeType.TERNARY_EXPRESSION:
                return BindTernaryExpression((TernaryExpression)expression);
            case NodeType.CALL_EXPRESSION:
                return BindCallExpression((CallExpression)expression);
            default:
                throw new Exception("Unexptected expression");
        }
    }

    private BoundExpression BindIntExpression(IntExpression expression)
    {
        return new BoundIntExpression(expression.value.text);
    }

    private BoundExpression BindBoolExpression(BoolExpression expression)
    {
        return new BoundBoolExpression(expression.value.type);
    }

    private BoundExpression BindUnaryExpression(UnaryExpression expression)
    {
        if (expression.operatorToken.type == NodeType.AND_TOKEN)
        {
            return BindReferenceExpression(expression);
        }

        BoundExpression right = BindExpression(expression.right);
        if (expression.operatorToken.type == NodeType.STAR_TOKEN)
        {
            if (right.returnType.name != TypeSymbol.PTR_NAME)
            {
                diagnostics.ReportDeReferenceRequiresPtr(expression.operatorToken.span);
                return new BoundErrorExpression();
            }
            return new BoundDeReferenceExpression(right);
        }

        BoundUnaryOperator unaryOperator = null;
        ConversionResult result = ConversionResult.NONE;
        foreach (BoundUnaryOperator boundUnaryOperator in BoundUnaryOperator.Bind(expression.operatorToken.type))
        {
            result = ClassifyConversion(right, boundUnaryOperator.rightType, false, false);
            if (result == ConversionResult.IDENTICAL)
            {
                unaryOperator = boundUnaryOperator;
                break;
            }
            if (result != ConversionResult.NONE)
            {
                unaryOperator = boundUnaryOperator;
            }
        }

        if (unaryOperator == null)
        {
            diagnostics.ReportUnknownUnaryOperation(expression.operatorToken.span, expression.operatorToken.text, right.returnType);
            return new BoundErrorExpression();
        }

        if (result != ConversionResult.IDENTICAL)
        {
            return new BoundUnaryExpression(unaryOperator, ClassifyConversion(expression.operatorToken.span, right, unaryOperator.rightType, false, false));
        }
        return new BoundUnaryExpression(unaryOperator, right);
    }

    private BoundExpression BindReferenceExpression(UnaryExpression expression)
    {
        if (expression.right.type == NodeType.NAME_EXPRESSION)
        {
            BoundExpression right = BindNameExpression((NameExpression)expression.right);
            if (right.type == NodeType.VARIABLE_EXPRESSION)
            {
                return new BoundVariableReferenceExpression(((BoundVariableExpression)right).variable);
            }
        }

        return new BoundErrorExpression();
    }

    private BoundExpression BindBinaryExpression(BinaryExpression expression)
    {
        BoundExpression left = BindExpression(expression.left);
        BoundExpression right = BindExpression(expression.right);

        BoundBinaryOperator binaryOperator = null;
        ConversionResult leftResult = ConversionResult.NONE;
        ConversionResult rightResult = ConversionResult.NONE;

        foreach (BoundBinaryOperator boundBinaryOperator in BoundBinaryOperator.Bind(expression.operatorToken.type))
        {
            ConversionResult _leftResult = ClassifyConversion(left, boundBinaryOperator.leftType, false, false);
            ConversionResult _rightResult = ClassifyConversion(right, boundBinaryOperator.rightType, false, false);

            if (_leftResult == ConversionResult.IDENTICAL && _rightResult == ConversionResult.IDENTICAL)
            {
                leftResult = _leftResult;
                rightResult = _rightResult;
                binaryOperator = boundBinaryOperator;
                break;
            }
            if (_leftResult != ConversionResult.NONE && _rightResult != ConversionResult.NONE)
            {
                leftResult = _leftResult;
                rightResult = _rightResult;
                binaryOperator = boundBinaryOperator;
            }
        }

        if (binaryOperator == null)
        {
            diagnostics.ReportUnknownBinaryOperation(expression.operatorToken.span, expression.operatorToken.text, left.returnType,
                right.returnType);
            return new BoundErrorExpression();
        }

        if (leftResult != ConversionResult.IDENTICAL)
        {
            left = ClassifyConversion(expression.operatorToken.span, left, binaryOperator.leftType, false, false);
        }
        if (rightResult != ConversionResult.IDENTICAL)
        {
            right = ClassifyConversion(expression.operatorToken.span, right, binaryOperator.rightType, false, false);
        }

        return new BoundBinaryExpression(left, binaryOperator, right);
    }

    private BoundExpression BindAssignmentExpression(AssignmentExpression expression)
    {
        BoundExpression fieldExpression = BindExpression(expression.fieldExpression);
        if (fieldExpression.type != NodeType.VARIABLE_EXPRESSION && fieldExpression.type != NodeType.DEREFERENCE_EXPRESSION)
        {
            diagnostics.ReportNotAssignable(expression.fieldExpression.span, expression.fieldExpression.type);
        }

        if (fieldExpression.type == NodeType.VARIABLE_EXPRESSION)
        {
            BoundVariableExpression variableExpression = (BoundVariableExpression)fieldExpression;
            if (variableExpression.variable.isReadonly)
            {
                diagnostics.ReportVariableIsImmutable(expression.fieldExpression.span, variableExpression.variable.name);
            }
        }
        BoundExpression boundExpression = BindExpectedExpression(expression.expression, fieldExpression.returnType);
        return new BoundAssignmentExpression(fieldExpression, boundExpression);
    }

    private BoundExpression BindNameExpression(NameExpression expression)
    {
        object symbol = scope.TryLookupSymbol(expression.value.text);
        if (symbol is VariableSymbol variable)
        {
            return new BoundVariableExpression(variable);
        }
        if (symbol is FunctionSymbol function)
        {
            return new BoundFunctionExpression(function);
        }

        diagnostics.ReportUnresolvedSymbol(expression.value.span, expression.value.text);
        return new BoundErrorExpression();
    }

    private BoundExpression BindConversionExpression(ConversionExpression expression)
    {
        BoundExpression boundExpression = BindExpression(expression.expression);
        TypeSymbol targetType = BindTypeClause(expression.clause, false);

        return ClassifyConversion(expression.clause.span, boundExpression, targetType, true, false);
    }

    private BoundExpression BindReinterpretationExpression(ReinterpretationExpression expression)
    {
        BoundExpression boundExpression = BindExpression(expression.expression);
        TypeSymbol targetType = BindTypeClause(expression.clause, false);

        return ClassifyConversion(expression.clause.span, boundExpression, targetType, true, true);
    }

    private BoundExpression BindIncrementExpression(IncrementExpression expression)
    {
        AssignmentExpression assignment = new AssignmentExpression(
            expression.left,
            new Token(NodeType.EQUALS_TOKEN, expression.plusPlusToken.span, "="),
            new BinaryExpression(
                expression.left,
                new Token(NodeType.PLUS_TOKEN, expression.plusPlusToken.span, "+"),
                new IntExpression(
                    new Token(NodeType.INT_TOKEN, expression.plusPlusToken.span, "1")
                )
            )
        );

        return BindAssignmentExpression(assignment);
    }

    private BoundExpression BindDecrementExpression(DecrementExpression expression)
    {
        AssignmentExpression assignment = new AssignmentExpression(
            expression.left,
            new Token(NodeType.EQUALS_TOKEN, expression.minusMinusToken.span, "="),
            new BinaryExpression(
                expression.left,
                new Token(NodeType.MINUS_TOKEN, expression.minusMinusToken.span, "-"),
                new IntExpression(
                    new Token(NodeType.INT_TOKEN, expression.minusMinusToken.span, "1")
                )
            )
        );

        return BindAssignmentExpression(assignment);
    }

    private BoundExpression BindTernaryExpression(TernaryExpression expression)
    {
        BoundExpression condition = BindExpectedExpression(expression.condition, TypeSymbol.boolean);
        BoundExpression thenExpression = BindExpression(expression.thenExpression);
        BoundExpression elseExpression = BindExpectedExpression(expression.elseExpression, thenExpression.returnType);
        return new BoundTernaryExpression(condition, thenExpression, elseExpression);
    }

    private BoundExpression BindCallExpression(CallExpression expression)
    {
        BoundExpression callee = BindExpression(expression.callee);
        if (callee.returnType.name != TypeSymbol.FUNC_NAME)
        {
            diagnostics.ReportExpressionNotCallable(expression.callee.span);
            return new BoundErrorExpression();
        }
        TypeSymbol functionType = callee.returnType;
        if (expression.arguments.Count != functionType.generics.Length - 1)
        {
            diagnostics.ReportUnexpectedArgumentCount(expression.callee.span, functionType.generics.Length - 1, expression.arguments.Count);
            return new BoundErrorExpression();
        }
        List<BoundExpression> arguments = new List<BoundExpression>();
        for (int i = 0; i < expression.arguments.Count; i++)
        {
            Expression argument = expression.arguments[i];
            arguments.Add(BindExpectedExpression(argument, functionType.generics[i]));
        }

        return new BoundCallExpression(callee, arguments.ToArray());
    }

    private TypeSymbol BindTypeClause(TypeClause clause, bool allowVoid)
    {
        if (allowVoid && clause.identifierToken.text == TypeSymbol.voidType.name)
        {
            return TypeSymbol.voidType;
        }
        if (clause.identifierToken.text == TypeSymbol.u64.name)
        {
            return TypeSymbol.u64;
        }
        if (clause.identifierToken.text == TypeSymbol.u32.name)
        {
            return TypeSymbol.u32;
        }
        if (clause.identifierToken.text == TypeSymbol.u16.name)
        {
            return TypeSymbol.u16;
        }
        if (clause.identifierToken.text == TypeSymbol.u8.name)
        {
            return TypeSymbol.u8;
        }
        if (clause.identifierToken.text == TypeSymbol.i64.name)
        {
            return TypeSymbol.i64;
        }
        if (clause.identifierToken.text == TypeSymbol.i32.name)
        {
            return TypeSymbol.i32;
        }
        if (clause.identifierToken.text == TypeSymbol.i16.name)
        {
            return TypeSymbol.i16;
        }
        if (clause.identifierToken.text == TypeSymbol.i8.name)
        {
            return TypeSymbol.i8;
        }
        if (clause.identifierToken.text == TypeSymbol.boolean.name)
        {
            return TypeSymbol.boolean;
        }
        if (clause.identifierToken.text == TypeSymbol.PTR_NAME)
        {
            if (clause.generics.Count != 1)
            {
                diagnostics.ReportExpectGenericCount(clause.identifierToken.span, clause.identifierToken.text, 1, clause.generics.Count);
                return TypeSymbol.error;
            }

            TypeSymbol baseType = BindTypeClause(clause.generics[0], true);
            return TypeSymbol.Reference(baseType);
        }
        if (clause.identifierToken.text == TypeSymbol.FUNC_NAME)
        {
            if (clause.generics.Count == 0)
            {
                diagnostics.ReportExpectGeneric(clause.identifierToken.span, clause.identifierToken.text);
                return TypeSymbol.error;
            }

            List<TypeSymbol> generics = new List<TypeSymbol>();

            for (int i = 0; i < clause.generics.Count; i++)
            {
                generics.Add(BindTypeClause(clause.generics[i], i == clause.generics.Count - 1));
            }

            return TypeSymbol.Func(generics.ToArray());
        }

        diagnostics.ReportUnknownType(clause.span, clause.identifierToken.text);
        return TypeSymbol.error;
    }

    private static IEnumerable<T> GetMembers<T>(IEnumerable<Member> members, NodeType type) where T : Member =>
        members.Where(member => member.type == type).Cast<T>();

    private static BoundFunctionMember BindFunctionMember(BoundModule module, FunctionMember member)
    {
        Binder binder = new Binder(module.scope);
        TypeSymbol returnType = binder.BindTypeClause(member.returnClause, true);

		binder = new Binder(module.scope, returnType);
        BoundBlockStatement body = binder.BindBlockStatement(member.body);
        if (returnType != TypeSymbol.voidType && !CFAScanner.AllPathsReturn(body)) {
            module.diagnostics.ReportNotAllPathsReturn(member.identifierToken.span, member.identifierToken.text);
        }

        List<VariableSymbol> parameters = new List<VariableSymbol>();
        HashSet<string> seenNames = new HashSet<string>();

        foreach (FunctionParameter parameter in member.parameters)
        {
            if (!seenNames.Add(parameter.identifierToken.text))
            {
                module.diagnostics.ReportAmbiguousParameterName(parameter.identifierToken.span, parameter.identifierToken.text);
            }
            TypeSymbol type = binder.BindTypeClause(parameter.clause, false);
            VariableSymbol variableSymbol = new VariableSymbol(false, parameter.identifierToken.text, type, binder.localVariableCounter++);
            parameters.Add(variableSymbol);
            if (!module.scope.TryDeclareVariable(variableSymbol))
            {
                module.diagnostics.ReportSymbolAlreadyDefined(parameter.identifierToken.span, variableSymbol.name);
            }
        }
        FunctionSymbol symbol = new FunctionSymbol(member.identifierToken.text, parameters.ToArray(), returnType);

        module.diagnostics.diagnostics.AddRange(binder.diagnostics.diagnostics);
        if (!module.scope.TryDeclareFunction(symbol))
        {
            module.diagnostics.ReportSymbolAlreadyDefined(member.identifierToken.span, symbol.name);
        }
        return new BoundFunctionMember(symbol, body);
    }

    public static BoundModule BindMembers(List<Member> members)
    {
        BoundModule module = new BoundModule();
        foreach (FunctionMember member in GetMembers<FunctionMember>(members, NodeType.FUNCTION_MEMBER))
        {
            module.functions.Add(BindFunctionMember(module, member));
        }

        return module;
    }
}