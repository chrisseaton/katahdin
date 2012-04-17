using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class MultiplyExpression : MultiplicativeExpression
    {
        public new static Pattern pattern;
        
        public Expression a;
        public Expression b;
        
        public MultiplyExpression(Source source, object a,
            object b)
                : base(source)
        {
            this.a = (Expression) a;
            this.b = (Expression) b;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(a 0) '*' l(b 0)))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "MultiplyExpression", parseGraph);
                pattern.SetType(typeof(MultiplyExpression));
            
                MultiplicativeExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("MultiplyExpression", typeof(MultiplyExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new OperationNode(
                Source,
                Operation.Multiply,
                a.BuildCodeTree(state),
                b.BuildCodeTree(state));
        }
    }
}
