using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class TypeExpression : Expression
    {
        public new static AbstractPattern pattern;
        
        public TypeExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "TypeExpression");
                pattern.SetType(typeof(TypeExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("TypeExpression", typeof(TypeExpression));
            grammar.PatternDefined(pattern);
        }
    }
}
