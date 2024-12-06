using ILS.Lexing;
using ILS.Binding;

namespace ILS.Binding.Statements;

public class BoundReturnStatement : BoundStatement {
	public override NodeType type => NodeType.RETURN_STATEMENT;

	public readonly BoundExpression value;

	public BoundReturnStatement(BoundExpression value) {
		this.value = value;
	}
}
