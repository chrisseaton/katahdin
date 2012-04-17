using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Katahdin.CodeTree
{
    public class WhileNode : CodeTreeNode
    {
        private CodeTreeNode condition;
        private CodeTreeNode body;
        
        public WhileNode(Source source, CodeTreeNode condition,
			CodeTreeNode body)
				: base(source)
        {
            this.condition = new ConvertNode(
                source,
                condition,
                new ValueNode(
                    source,
                    typeof(bool)));
            
            this.body = body;;
        }
        
        public override object Run(RuntimeState state)
        {
            while ((bool) condition.Get(state))
            {
                object returned = body.Run(state);
                
                if (state.Returning != null)
                    return returned;
            }
            
            return null;
        }
    }
}
