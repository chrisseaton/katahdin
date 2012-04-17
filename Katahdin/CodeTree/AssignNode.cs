using System;


namespace Katahdin.CodeTree
{
    public class AssignNode : CodeTreeNode
    {
        private CodeTreeNode from;
        private CodeTreeNode to;
        
        public AssignNode(Source source, CodeTreeNode from, CodeTreeNode to)
			: base(source)
        {
            this.from = from;
            this.to = to;
        }
        
        public override object Get(RuntimeState state, object[] parametersHint)
        {
            object v = from.Get(state, null);
            to.Set(state, v);
            return v;
        }
    }
}
