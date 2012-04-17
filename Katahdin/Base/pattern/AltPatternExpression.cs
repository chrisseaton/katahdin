using System;
using System.Collections.Generic;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class AltPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public PatternExpression a;
        public PatternExpression b;
        
        public AltPatternExpression(Source source, object a, object b)
            : base(source)
        {
            this.a = (PatternExpression) a;
            this.b = (PatternExpression) b;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(a 0) '|' l(b 0)))";
                Pattern[] patterns = {PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "AltPatternExpression", parseGraph);
                pattern.SetType(typeof(AltPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AltPatternExpression", typeof(AltPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            List<ParseGraphNode> nodes = new List<ParseGraphNode>();
            
            foreach (PatternExpression node in new PatternExpression[]{a, b})
            {
                ParseGraphNode nodeGraph = node.BuildParseGraph(state);
                
                if (nodeGraph is AltNode)
                    nodes.AddRange(((AltNode) nodeGraph).Alts);
                else
                    nodes.Add(nodeGraph);
            }
            
            return new AltNode(Source, nodes, false);
        }
    }
}
