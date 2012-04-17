using System;
using System.Collections.Generic;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class RepeatPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public PatternExpression body;
        public string reps;
        
        public RepeatPatternExpression(Source source, object body, object reps)
            : base(source)
        {
            this.body = (PatternExpression) body;
            this.reps = (string) reps;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(body 0) l(reps o((buildTextNodes true) a('?' '*' '+')))))";
                Pattern[] patterns = {PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "RepeatPatternExpression", parseGraph);
                pattern.SetType(typeof(RepeatPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("RepeatPatternExpression", typeof(RepeatPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            PatternExpression body = (PatternExpression) this.body;
            
            string reps = (string) this.reps;
            
            return new RepNode(
                Source,
                body.BuildParseGraph(state),
                Reps.ForName(reps));
        }
    }
}
