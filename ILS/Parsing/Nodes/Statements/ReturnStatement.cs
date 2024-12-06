using System.Collections.Generic;
using ILS.Lexing;
using ILS.Parsing.Nodes;

namespace ILS.Parsing.Nodes.Statements;

public class ReturnStatement : Statement {	
    public override NodeType type => NodeType.RETURN_STATEMENT;
    public override TextSpan span => TextSpan.Merge(returnToken.span, semicolonToken.span);

	public Token returnToken;
	public Expression value;
	public Token semicolonToken;

	public ReturnStatement(Token returnToken, Expression value, Token semicolonToken) {
		this.returnToken = returnToken;
		this.value = value;
		this.semicolonToken = semicolonToken;
	}

    public override IEnumerable<Node> GetChildren() {
		yield return returnToken;
		if (value != null) {
			yield return value;
		}
		yield return semicolonToken;
	}
}
