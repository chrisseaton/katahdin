using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class String : RuntimeObject
    {
        public static Pattern pattern;
        
        public string text;
        
        public String(Source source, object text)
            : base(source)
        {
            this.text = (string) text;
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "l(text o((whitespace null) t( s( '\\\"' *( a( s( !(a('\\\\' '\\\"')) any ) s('\\\\' a('\\\\' 'r' 'n' 't' '\\\"' '\\\'' '0') ) ) ) '\\\"' ) ) ) )";
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, null);
        
                pattern = new ConcretePattern(null, "String", parseGraph);
                pattern.SetType(typeof(String));
            }
            
            module.SetName("String", typeof(String));
            grammar.PatternDefined(pattern);
        }
        
        public string Text
        {
            get
            {
                return TextEscape.Unquote(text);
            }
        }
    }
}
