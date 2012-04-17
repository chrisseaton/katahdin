using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class AssignmentExpression : Expression
    {
        public new static AbstractPattern pattern;
        
        public AssignmentExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "AssignmentExpression");
                pattern.SetType(typeof(AssignmentExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AssignmentExpression", typeof(AssignmentExpression));
            grammar.PatternDefined(pattern);
        }
    }
}
