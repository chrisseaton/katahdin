using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class ComparisonExpression : Expression
    {
        public new static AbstractPattern pattern;
        
        public ComparisonExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "ComparisonExpression");
                pattern.SetType(typeof(ComparisonExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ComparisonExpression", typeof(ComparisonExpression));
            grammar.PatternDefined(pattern);
        }
    }
}
