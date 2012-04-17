using System;
using System.Collections.Generic;

using Katahdin.Compiler;

namespace Katahdin.CodeTree
{
    public class SeqNode : CodeTreeNode
    {
        private List<CodeTreeNode> children;
        
        public SeqNode(Source source, List<CodeTreeNode> children)
			    : base(source)
        {
            this.children = children;
        }
        
        public override object Run(RuntimeState state)
        {
            foreach (CodeTreeNode child in children)
            {
                object returned = child.Run(state);
                
                if (state.Returning != null)
                    return returned;
            }
            
            return null;
        }
    }
}
