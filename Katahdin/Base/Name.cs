using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class Name : RuntimeObject
    {
        public static Pattern pattern;
        
        public string name;
        
        public Name(Source source, object name)
            : base(source)
        {
            this.name = (string) name;
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string startChars = "a(r('a' 'z') r('A' 'Z') '_')";
                string continueChars = "a(r('a' 'z') r('A' 'Z') r('0' '9') '_')";
                string basic = "s(" + startChars + " *(" + continueChars + "))";
                string token = "o((whitespace null) t(" + basic + ")))";
                string expression = "l(name " + token + ")";
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, null);
            
                pattern = new ConcretePattern(null, "Name", parseGraph);
                pattern.SetType(typeof(Name));
            }
            
            module.SetName("Name", typeof(Name));
            grammar.PatternDefined(pattern);
        }
    }
}
