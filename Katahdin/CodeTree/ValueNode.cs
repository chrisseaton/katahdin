using System;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Compiler;

namespace Katahdin.CodeTree
{
    public class ValueNode : CodeTreeNode
    {
        private object o;
        
        public ValueNode(Source source, object o)
			: base(source)
        {
            this.o = o;
        }
        
        public override object Get(RuntimeState state, object[] parametersHint)
        {
            return o;
        }
        
        public override void EmitGet(Block generator)
        {
            generator.Load(o);
        }
    }
}
