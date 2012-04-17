using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class JunctionExpression : Expression
    {
        public new static AbstractPattern pattern;
        
        public JunctionExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "JunctionExpression");
                pattern.SetType(typeof(JunctionExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("JunctionExpression", typeof(JunctionExpression));
            grammar.PatternDefined(pattern);
        }
    }
}
