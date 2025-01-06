using System.Collections.Generic;
using ILS.Lexing;

namespace ILS.Parsing.Nodes.Members;

public sealed class ExternFunctionMember : Member
{
    public override NodeType type => NodeType.EXTERN_FUNCTION_MEMBER;
    public override TextSpan span => TextSpan.Merge(externKeyword.span, semiToken.span);

    public Token externKeyword;
    public Token functionKeyword;
    public Token identifierKeyword;
    public Token lParen;
    public List<FunctionParameter> parameters;
    public Token rParen;
    public TypeClause returnType;
    public Token semiToken;
    public ExternFunctionMember(
        Token externKeyword,
        Token functionKeyword,
        Token identifierKeyword,
        Token lParen,
        List<FunctionParameter> parameters,
        Token rParen,
        TypeClause returnType,
        Token semiToken)
    {
        this.externKeyword = externKeyword;
        this.functionKeyword = functionKeyword;
        this.identifierKeyword = identifierKeyword;
        this.lParen = lParen;
        this.parameters = parameters;
        this.rParen = rParen;
        this.returnType = returnType;
        this.semiToken = semiToken;
    }

    public override IEnumerable<Node> GetChildren()
    {
        yield return externKeyword;
        yield return functionKeyword;
        yield return identifierKeyword;
        yield return lParen;
        foreach (FunctionParameter parameter in parameters)
        {
            yield return parameter;
        }
        yield return rParen;
        yield return returnType;
        yield return semiToken;
    }
}