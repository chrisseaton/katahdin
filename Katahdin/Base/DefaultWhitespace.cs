using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class DefaultWhitespace : RuntimeObject
    {
        public static Pattern pattern;
        
        public DefaultWhitespace(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "+(a(' ' '\\t' s('\\r' ?('\\n')) '\\n'))";
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, null);
            
                pattern = new ConcretePattern(null, "DefaultWhitespace", parseGraph);
                pattern.SetType(typeof(DefaultWhitespace));
            }
            
            module.SetName("DefaultWhitespace", typeof(DefaultWhitespace));
            grammar.PatternDefined(pattern);
        }
    }
}
