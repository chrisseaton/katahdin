using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public abstract class PatternExpression : RuntimeObject
    {
        public static AbstractPattern pattern;
        
        public PatternExpression(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "PatternExpression");
                pattern.SetType(typeof(PatternExpression));
            }
            
            module.SetName("PatternExpression", typeof(PatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public abstract ParseGraphNode BuildParseGraph(RuntimeState state);
    }
}
