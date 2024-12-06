using System;
using System.Linq;
using ILS.Lexing;
using ILS.Binding;
using ILS.Binding.Statements;

namespace ILS.CFA;

public class CFAScanner {
	private CFAScanner() {

	}

	public static bool AllPathsReturn(BoundBlockStatement statement) {
		BasicBlockBuilder builder = new BasicBlockBuilder();
		CFAGraph graph = builder.Build(statement);		
		BasicBlock end = graph.end;
		BoundStatement lastStatement = end.statements.LastOrDefault();
		if (lastStatement == null || lastStatement.type != NodeType.RETURN_STATEMENT) {
			return false;
		}

		return true;
	}
}
