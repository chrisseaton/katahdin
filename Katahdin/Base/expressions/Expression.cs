using System;
using System.Reflection;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public abstract class Expression : RuntimeObject
    {
        public static AbstractPattern pattern;
        
        public Expression(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "Expression");
                pattern.SetType(typeof(Expression));
            }
            
            module.SetName("Expression", typeof(Expression));
            grammar.PatternDefined(pattern);
        }
        
        public virtual CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new ObjectNode(Source, this);
        }
        
        public object Get(RuntimeState state)
        {
            CodeTreeNode tree = BuildCodeTree(state);
            return tree.Get(state, null);
        }
        
        public void Set(RuntimeState state, object v)
        {
            CodeTreeNode tree = BuildCodeTree(state);
            tree.Set(state, v);
        }
    }
}
