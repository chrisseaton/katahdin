using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Katahdin.CodeTree
{
    public class IfNode : CodeTreeNode
    {
        private CodeTreeNode condition;
        private CodeTreeNode trueBody;
        private CodeTreeNode falseBody;
        
        public IfNode(Source source, CodeTreeNode condition,
			CodeTreeNode trueBody, CodeTreeNode falseBody)
				: base(source)
        {
            this.condition = new ConvertNode(
                source,
                condition,
                new ValueNode(
                    source,
                    typeof(bool)));
            
            this.trueBody = trueBody;
            this.falseBody = falseBody;
        }
        
        public override object Run(RuntimeState state)
        {
            if ((bool) condition.Get(state, null))
                return trueBody.Run(state);
            else if (falseBody != null)
                return falseBody.Run(state);
            else
                return null;
        }
    }
}
