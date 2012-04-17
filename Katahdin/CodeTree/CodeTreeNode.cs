using System;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Compiler;

namespace Katahdin.CodeTree
{
    public abstract class CodeTreeNode
    {
		private Source source;
		
		public CodeTreeNode(Source source)
		{
			this.source = source;
		}
	
        public virtual object Run(RuntimeState state)
        {
            throw new NotImplementedException(TypeNames.GetName(GetType())
                + " (do you need a GetToRunNode?)");
        }
        
        public object Get(RuntimeState state)
        {
            return Get(state, null);
        }
        
        public virtual object Get(RuntimeState state, object[] parametersHint)
        {
            throw new NotImplementedException(TypeNames.GetName(GetType()));
        }
        
        public virtual void Set(RuntimeState state, object v)
        {
            throw new NotImplementedException(TypeNames.GetName(GetType()));
        }
        
        public virtual void EmitRun(Block generator)
        {
            throw new NotImplementedException(TypeNames.GetName(GetType()));
        }
        
        public virtual void EmitGet(Block generator)
        {
            throw new NotImplementedException(TypeNames.GetName(GetType()));
        }
        
        public virtual void EmitSet(Block generator)
        {
            throw new NotImplementedException(TypeNames.GetName(GetType()));
        }

		public Source Source
		{
			get
			{
				return source;
			}
		}
    }
}
