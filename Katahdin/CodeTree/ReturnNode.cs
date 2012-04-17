using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Katahdin.CodeTree
{
    public class ReturnNode : CodeTreeNode
    {
        private CodeTreeNode expression;
        
        public ReturnNode(Source source, CodeTreeNode expression)
			    : base(source)
        {
            this.expression = expression;
        }
        
        public override object Run(RuntimeState state)
        {
            object returned = expression.Get(state);
            
            state.Returning = state.Scope;
            return returned;
        }
    }
}
