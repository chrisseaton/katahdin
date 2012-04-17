using System;
using System.Collections.Generic;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class SequencePatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public List<object> nodes;
        
        public SequencePatternExpression(Source source, object nodes)
            : base(source)
        {
            this.nodes = (List<object>) nodes;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((recursive false) l(nodes s(0 +(0))))";
                Pattern[] patterns = {PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "SequencePatternExpression", parseGraph);
                pattern.SetType(typeof(SequencePatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("SequencePatternExpression", typeof(SequencePatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            List<ParseGraphNode> parseNodes = new List<ParseGraphNode>();
            
            foreach (PatternExpression node in nodes)
            {
                ParseGraphNode nodeGraph = node.BuildParseGraph(state);
                
                if (nodeGraph is SeqNode)
                    parseNodes.AddRange(((SeqNode) nodeGraph).Nodes);
                else
                    parseNodes.Add(nodeGraph);
            }
            
            return new SeqNode(Source, parseNodes);
        }
    }
}
