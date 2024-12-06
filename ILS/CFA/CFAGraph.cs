namespace ILS.CFA;

public struct CFAGraph {
	public BasicBlock start;
	public BasicBlock end;

	public CFAGraph(BasicBlock start, BasicBlock end) {
		this.start = start;
		this.end = end;
	}

}
