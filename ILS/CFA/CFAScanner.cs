using System;
using ILS.Lexing;
using ILS.Binding;
using ILS.Binding.Statements;

namespace ILS.CFA;

public class CFAScanner {
	public CFAScanner() {

	}

	public bool AllPathsReturn(BoundBlockStatement statement) {
		foreach(BoundStatement sth in statement.statements) {
			WalkStatement(sth);
		}
		return false;
	} 

	public void WalkStatement(BoundStatement statement) {	
//		Console.WriteLine(statement.type);
		switch(statement.type) {
			case NodeType.BLOCK_STATEMENT: {	
				foreach(BoundStatement sth in ((BoundBlockStatement) statement).statements) {
					WalkStatement(sth);
				}
				break;
			}
			case NodeType.IF_STATEMENT: {
				BoundIfStatement sth = (BoundIfStatement)statement;
				WalkStatement(sth.thenBlock);
				if (sth.elseBlock != null) {
					WalkStatement(sth.elseBlock);
				}
				break;
			}
		}
	}
}
