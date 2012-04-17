using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class Option : RuntimeObject
    {
        public new static Pattern pattern;
        
        public Name optionKey;
        public Expression optionValue;
        
        public Option(Source source, object optionKey, object optionValue)
            : base(source)
        {
            this.optionKey = (Name) optionKey;
            this.optionValue = (Expression) optionValue;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('option' l(optionKey 0) ?(s('=' o((dropPrecedence true) l(optionValue 1)))) ';')";
                Pattern[] patterns = {Name.pattern, Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "Option", parseGraph);
                pattern.SetType(typeof(Option));
            }
            
            module.SetName("Option", typeof(Option));
            grammar.PatternDefined(pattern);
        }
    }
}
