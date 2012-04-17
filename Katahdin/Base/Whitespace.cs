using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class Whitespace : RuntimeObject
    {
        public static Pattern pattern;
        
        public Whitespace(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "+(a(0 1 2))";
                Pattern[] patterns = {LineComment.pattern, BlockComment.pattern, DefaultWhitespace.pattern};
                
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
            
                pattern = new ConcretePattern(null, "Whitespace", parseGraph);
                pattern.SetType(typeof(Whitespace));
            }
            
            module.SetName("Whitespace", typeof(Whitespace));
            grammar.PatternDefined(pattern);
        }
    }
}
