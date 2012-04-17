using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class ReferencePatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public Expression name;
        
        public ReferencePatternExpression(Source source, object name)
            : base(source)
        {
            this.name = (Expression) name;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "l(name a(0 1))";
                Pattern[] patterns = {MemberExpression.pattern, NameExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "ReferencePatternExpression", parseGraph);
                pattern.SetType(typeof(ReferencePatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ReferencePatternExpression", typeof(ReferencePatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            Type type = (Type) name.Get(state);
            Pattern pattern = Pattern.PatternForType(type);
            
            if (pattern != null)
                return new PatternNode(Source, pattern, false);
            else
                return new UserDefinedNode(Source, type);
        }
    }
}
