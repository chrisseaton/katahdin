using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class TokenPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public BlockPatternExpression body;
        
        public TokenPatternExpression(Source source, object body)
            : base(source)
        {
            this.body = (BlockPatternExpression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('token' l(body 0))";
                Pattern[] patterns = {BlockPatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "TokenPatternExpression", parseGraph);
                pattern.SetType(typeof(TokenPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("TokenPatternExpression", typeof(TokenPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            BlockPatternExpression body = (BlockPatternExpression) this.body;
            
            return new TokenNode(Source, body.BuildParseGraph(state));
        }
    }
}
