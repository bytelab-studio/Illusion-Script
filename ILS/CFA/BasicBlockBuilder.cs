using System.Collections.Generic;
using ILS.Binding;
using ILS.Binding.Statements;
using ILS.Lexing;

namespace ILS.CFA;

public class BasicBlockBuilder {
	private BasicBlock currentBlock;

	public CFAGraph Build(BoundBlockStatement statement) {
		BasicBlock start = new BasicBlock(true);
		currentBlock = start;
		WalkStatement(statement);
		return new CFAGraph(start, currentBlock);
	}

	private void WalkStatement(BoundStatement statement) {
		switch(statement.type) {
			case NodeType.BLOCK_STATEMENT:
				WalkBlockStatement((BoundBlockStatement)statement);
				break;
			case NodeType.IF_STATEMENT:
				WalkIfStatement((BoundIfStatement)statement);
				break;
			case NodeType.WHILE_STATEMENT:
				WalkWhileStatement((BoundWhileStatement)statement);
				break;
			default:
				currentBlock.statements.Add(statement);
				break;
		}	
	}

	private void WalkBlockStatement(BoundBlockStatement statement) {
		foreach (BoundStatement sth in statement.statements) {
			WalkStatement(sth);
		}
	}

	private void WalkIfStatement(BoundIfStatement statement) {
		BasicBlock thenBlock = new BasicBlock(false);
		BasicBlock elseBlock = null;
		CreateBranch(currentBlock, thenBlock);
		BasicBlock incomingBlock = currentBlock;
		currentBlock = thenBlock;
		WalkStatement(statement.thenBlock);
	
		if (statement.elseBlock != null) {
			elseBlock = new BasicBlock(false);
			CreateBranch(incomingBlock, elseBlock);
			currentBlock = elseBlock;
			WalkStatement(statement.elseBlock);	
		}

		BasicBlock endBlock = new BasicBlock(false);
		CreateBranch(thenBlock, endBlock);
		if (elseBlock != null) {
			CreateBranch(elseBlock, endBlock);
		} else {
			CreateBranch(incomingBlock, endBlock);
		}

		currentBlock = endBlock;
	}

	private void WalkWhileStatement(BoundWhileStatement statement) {
		BasicBlock body = new BasicBlock(false);
		CreateBranch(currentBlock, body);
		currentBlock = body;
		CreateBranch(body, body);
		WalkStatement(statement.body);
	}

	private void CreateBranch(BasicBlock from, BasicBlock to) {
		BasicBlockBranch branch = new BasicBlockBranch(from, to);
		from.outgoing.Add(branch);
		to.incoming.Add(branch);
	}
}
