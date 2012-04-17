using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class TextPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public String text;
        
        public TextPatternExpression(Source source, object text)
            : base(source)
        {
            this.text = (String) text;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "l(text 0)";
                Pattern[] patterns = {String.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
            
                pattern = new ConcretePattern(null, "TextPatternExpression", parseGraph);
                pattern.SetType(typeof(TextPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("TextPatternExpression", typeof(TextPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            string text = (string) ((String) this.text).text;
            
            text = TextEscape.Unquote(text);
            
            if (text.Length == 1)
                return new CharNode(Source, text[0]);
            else
                return new TextNode(Source, text);
        }
    }
}
