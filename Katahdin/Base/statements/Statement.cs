using System;
using System.Reflection;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public abstract class Statement : RuntimeObject
    {
        public static AbstractPattern pattern;
        
        public Statement(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "Statement");
                pattern.SetType(typeof(Statement));
            }
            
            module.SetName("Statement", typeof(Statement));
            grammar.PatternDefined(pattern);
        }
        
        public virtual CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new ObjectNode(Source, this);
        }
        
        public object Run(RuntimeState state)
        {
            CodeTreeNode tree = BuildCodeTree(state);
            return tree.Run(state);
        }
    }
}
