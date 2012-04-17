using System;


namespace Katahdin.CodeTree
{
    public class GetToRunNode : CodeTreeNode
    {
        private CodeTreeNode child;
        
        public GetToRunNode(Source source, CodeTreeNode child)
			: base(source)
        {
            this.child = child;
        }
        
        public override object Run(RuntimeState state)
        {
            return child.Get(state, null);
        }
    }
}
