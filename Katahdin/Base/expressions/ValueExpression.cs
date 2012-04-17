using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class ValueExpression : Expression
    {
        public new static Pattern pattern;
        
        public object v;
        
        public ValueExpression(Source source, object v)
            : base(source)
        {
            this.v = v;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "l(v a(0 1))";
                Pattern[] patterns = {Number.pattern, String.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
            
                pattern = new ConcretePattern(null, "ValueExpression", parseGraph);
                pattern.SetType(typeof(ValueExpression));
            
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ValueExpression", typeof(ValueExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new ValueNode(
                Source,
                Value);
        }
        
        public object Value
        {
            get
            {
                if (v is Number)
                    return ((Number) v).Value;
                else if (v is String)
                    return ((String) v).Text;
                else
                    throw new NotImplementedException();
            }
        }
    }
}
