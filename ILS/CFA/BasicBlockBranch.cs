namespace ILS.CFA;

public class BasicBlockBranch {
	public readonly BasicBlock from;
	public readonly BasicBlock to;
	
	public BasicBlockBranch(BasicBlock from, BasicBlock to) {
		this.from  = from;
		this.to = to;
	}
}	
