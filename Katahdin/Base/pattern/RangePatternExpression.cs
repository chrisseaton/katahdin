using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class RangePatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public String min;
        public String max;
        
        public RangePatternExpression(Source source, object min, object max)
            : base(source)
        {
            this.min = (String) min;
            this.max = (String) max;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s(l(min 0) '..' l(max 0))";
                Pattern[] patterns = {String.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "RangePatternExpression", parseGraph);
                pattern.SetType(typeof(RangePatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("RangePatternExpression", typeof(RangePatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            string min = (string) ((Base.String) this.min).text;
            string max = (string) ((Base.String) this.max).text;
            
            min = TextEscape.Unquote(min);
            max = TextEscape.Unquote(max);
            
            if ((min.Length != 1) || (max.Length != 1))
                throw new Exception();
            
            CharRange range = new CharRange(min[0], max[0]);
            
            return new CharNode(Source, range);
        }
    }
}
