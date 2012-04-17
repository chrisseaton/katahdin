using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class SubtractExpression : AdditiveExpression
    {
        public new static Pattern pattern;
        
        public Expression a;
        public Expression b;
        
        public SubtractExpression(Source source, object a,
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
                string expression = "o((leftRecursive true) s(l(a 0) '-' l(b 0)))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "SubtractExpression", parseGraph);
                pattern.SetType(typeof(SubtractExpression));
            
                AdditiveExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("SubtractExpression", typeof(SubtractExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new OperationNode(
                Source,
                Operation.Subtract,
                a.BuildCodeTree(state),
                b.BuildCodeTree(state));
        }
    }
}
