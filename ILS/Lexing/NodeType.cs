﻿namespace ILS.Lexing;

public enum NodeType
{
    EOF_TOKEN,
    ERROR_TOKEN,
    INT_TOKEN,
    WHITESPACE_TOKEN,
    PLUS_PLUS_TOKEN,
    PLUS_TOKEN,
    MINUS_MINUS_TOKEN,
    MINUS_TOKEN,
    STAR_TOKEN,
    SLASH_TOKEN,
    LPAREN_TOKEN,
    RPAREN_TOKEN,
    LBRACE_TOKEN,
    RBRACE_TOKEN,
    LANGLE_TOKEN,
    RANGLE_TOKEN,
    BANG_TOKEN,
    AND_TOKEN,
    AND_AND_TOKEN,
    PIPE_PIPE_TOKEN,
    PIPE_TOKEN,
    EQUALS_EQUALS_TOKEN,
    BANG_EQUALS_TOKEN,
    EQUALS_TOKEN,
    QUESTION_TOKEN,
    SEMI_TOKEN,
    COLON_TOKEN,
    COMMA_TOKEN,
    DOT_TOKEN,

    IDENTIFIER_TOKEN,
    TRUE_KEYWORD,
    FALSE_KEYWORD,
    LET_KEYWORD,
    CONST_KEYWORD,
    IF_KEYWORD,
    ELSE_KEYWORD,
    AS_KEYWORD,
    WHILE_KEYWORD,
    BREAK_KEYWORD,
    CONTINUE_KEYWORD,
    FUNCTION_KEYWORD,
	RETURN_KEYWORD,
	STRUCT_KEYWORD,
	EXTERN_KEYWORD,

    TYPE_CLAUSE,
    PARAMETER,
    STRUCT_ITEM,

    ERROR_EXPRESSION,
    INT_EXPRESSION,
    BOOL_EXPRESSION,
    BINARY_EXPRESSION,
    UNARY_EXPRESSION,
    PAREN_EXPRESSION,
    NAME_EXPRESSION,
    ASSIGNMENT_EXPRESSION,
    VARIABLE_EXPRESSION,
    FUNCTION_EXPRESSION,
    VARIABLE_REFERENCE_EXPRESSION,
    CONVERSION_EXPRESSION,
    REINTERPRETATION_EXPRESSION,
    DEREFERENCE_EXPRESSION,
    INCREMENT_EXPRESSION,
    DECREMENT_EXPRESSION,
    TERNARY_EXPRESSION,
    CALL_EXPRESSION,
    MEMBER_EXPRESSION,

    BLOCK_STATEMENT,
    VARIABLE_STATEMENT,
    IF_STATEMENT,
    ELSE_STATEMENT,
    WHILE_STATEMENT,
    BREAK_STATEMENT,
    CONTINUE_STATEMENT,
	RETURN_STATEMENT,
    EXPRESSION_STATEMENT,

    EXTERN_FUNCTION_MEMBER,
    FUNCTION_MEMBER,
    STRUCT_MEMBER
}