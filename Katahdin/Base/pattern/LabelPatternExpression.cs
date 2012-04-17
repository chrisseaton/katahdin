using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class LabelPatternExpression : PatternExpression
    {
        public new static ConcretePattern pattern;
        
        public Name label;
        public PatternExpression body;
        
        public LabelPatternExpression(Source source, object label, object body)
            : base(source)
        {
            this.label = (Name) label;
            this.body = (PatternExpression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((recursive false) s(l(label 0) ':' l(body 1)))";
                Pattern[] patterns = {Name.pattern, PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "LabelPatternExpression", parseGraph);
                pattern.SetType(typeof(LabelPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("LabelPatternExpression", typeof(LabelPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            return new LabelNode(Source, label.name, body.BuildParseGraph(state));
        }
    }
}
