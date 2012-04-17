using Katahdin.Grammars;
using Katahdin.Compiler;

namespace Katahdin.Base
{
    public class PatternMember : Member
    {
        public new static Pattern pattern;
        
        public PatternExpression body;
        
        public PatternMember(Source source, object body)
            : base(source)
        {
            this.body = (PatternExpression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('pattern' l(body 0))";
                Pattern[] patterns = {BlockPatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "PatternMember", parseGraph);
                pattern.SetType(typeof(PatternMember));
            
                Member.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("PatternMember", typeof(PatternMember));
            grammar.PatternDefined(pattern);
        }
        
        public override void Build(RuntimeState state, ClassBuilder builder)
        {
            builder.Pattern = new ConcretePattern(Source,
                builder.TypeBuilder.Name, body.BuildParseGraph(state));
        }
    }
}
