using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Katahdin.CodeTree
{
    public class ThrowNode : CodeTreeNode
    {
        private CodeTreeNode exception;
        
        public ThrowNode(Source source, CodeTreeNode exception)
			: base(source)
        {
            this.exception = exception;
        }
        
        public override object Run(RuntimeState state)
        {
            object exception = this.exception.Get(state);
            
            state.RunningSource = Source;
            
            if (exception is Exception)
                throw (Exception) exception;
            
            throw new Exception(exception.ToString());
        }
    }
}
