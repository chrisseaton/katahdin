using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class AndPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public PatternExpression body;
        
        public AndPatternExpression(Source source, object body)
            : base(source)
        {
            this.body = (PatternExpression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((rightRecursive true) s('&' l(body 0)))";
                Pattern[] patterns = {PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "AndPatternExpression", parseGraph);
                pattern.SetType(typeof(AndPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AndPatternExpression", typeof(AndPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            return new AndNode(
                Source,
                body.BuildParseGraph(state));
        }
    }
}
