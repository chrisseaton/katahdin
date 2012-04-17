using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class UnaryExpression : Expression
    {
        public new static AbstractPattern pattern;
        
        public UnaryExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "UnaryExpression");
                pattern.SetType(typeof(UnaryExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("UnaryExpression", typeof(UnaryExpression));
            grammar.PatternDefined(pattern);
        }
    }
}
