using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class NotPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public PatternExpression body;
        
        public NotPatternExpression(Source source, object body)
            : base(source)
        {
            this.body = (PatternExpression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((rightRecursive true) s('!' l(body 0)))";
                Pattern[] patterns = {PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "NotPatternExpression", parseGraph);
                pattern.SetType(typeof(NotPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("NotPatternExpression", typeof(NotPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            return new NotNode(
                Source,
                body.BuildParseGraph(state));
        }
    }
}
