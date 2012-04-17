using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class MultiplicativeExpression : Expression
    {
        public new static AbstractPattern pattern;
        
        public MultiplicativeExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "MultiplicativeExpression");
                pattern.SetType(typeof(MultiplicativeExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("MultiplicativeExpression", typeof(MultiplicativeExpression));
            grammar.PatternDefined(pattern);
        }
    }
}
