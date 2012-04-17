using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class AdditiveExpression : Expression
    {
        public new static AbstractPattern pattern;
        
        public AdditiveExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "AdditiveExpression");
                pattern.SetType(typeof(AdditiveExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AdditiveExpression", typeof(AdditiveExpression));
            grammar.PatternDefined(pattern);
        }
    }
}
