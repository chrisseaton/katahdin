using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class ParenPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public PatternExpression body;
        
        public ParenPatternExpression(Source source, object body)
            : base(source)
        {
            this.body = (PatternExpression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((rightRecursive true) s('(' o((dropPrecedence true) l(body 0)) ')'))";
                Pattern[] patterns = {PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "ParenPatternExpression", parseGraph);
                pattern.SetType(typeof(ParenPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ParenPatternExpression", typeof(ParenPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            PatternExpression body = (PatternExpression) this.body;
            return body.BuildParseGraph(state);
        }
    }
}
