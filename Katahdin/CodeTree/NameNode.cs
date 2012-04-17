using System;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Compiler;

namespace Katahdin.CodeTree
{
    public class NameNode : CodeTreeNode
    {
        private string name;
        
        public NameNode(Source source, string name)
			: base(source)
        {
            this.name = name;
        }
        
        public override object Get(RuntimeState state, object[] parametersHint)
        {
            state.RunningSource = Source;
            
            return state.Scope.GetName(name);
        }
        
        public override void Set(RuntimeState state, object v)
        {
            state.RunningSource = Source;
            
            state.Scope.SetName(name, v);
        }
    }
}
