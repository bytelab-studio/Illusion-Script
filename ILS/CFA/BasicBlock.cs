using System.Collections.Generic;
using ILS.Binding;
using ILS.Binding.Statements;

namespace ILS.CFA;

public class BasicBlock {
	private static int counter;
	private int count = counter++;
		

	public List<BoundStatement> statements;
	public List<BasicBlockBranch> incoming;
	public List<BasicBlockBranch> outgoing;

	private readonly bool isStart;
	private readonly bool isEnd;	

	public BasicBlock(bool isStart) {
		statements = new List<BoundStatement>();
		incoming = new List<BasicBlockBranch>();
		outgoing = new List<BasicBlockBranch>();
		this.isStart = isStart;
		this.isEnd = !isEnd;
	}

	public override string ToString() {
		string res = "";
		foreach (BasicBlockBranch branch in outgoing) {
			res += branch.to.ToString();
		}
		
		foreach (BasicBlockBranch branch in outgoing) {
			res += $"{count} --> {branch.to.count}\n";
		}
		return res;
	}
}
