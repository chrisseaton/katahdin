using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class AnyPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public AnyPatternExpression(Source source)
            : base(source)
        {
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                ParseGraphNode parseGraph = BootstrapParser.Parse("'.'", null);
        
                pattern = new ConcretePattern(null, "AnyPatternExpression", parseGraph);
                pattern.SetType(typeof(AnyPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AnyPatternExpression", typeof(AnyPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            return new AnyNode(Source);
        }
    }
}
