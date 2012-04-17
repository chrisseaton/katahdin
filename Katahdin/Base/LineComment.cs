using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class LineComment : RuntimeObject
    {
        public static Pattern pattern;
        
        public LineComment(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('//' *(s(!(a( s('\r' ?('\n')) '\n' )) any)))";
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, null);
            
                pattern = new ConcretePattern(null, "LineComment", parseGraph);
                pattern.SetType(typeof(LineComment));
            }
            
            module.SetName("LineComment", typeof(LineComment));
            grammar.PatternDefined(pattern);
        }
    }
}
