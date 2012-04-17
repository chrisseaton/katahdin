using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class LessOrEqualExpression : ComparisonExpression
    {
        public new static Pattern pattern;
        
        public Expression a;
        public Expression b;
        
        public LessOrEqualExpression(Source source, object a,
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
                string expression = "o((leftRecursive true) s(l(a 0) '<=' l(b 0)))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "LessOrEqualExpression", parseGraph);
                pattern.SetType(typeof(LessOrEqualExpression));
            
                ComparisonExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("LessOrEqualExpression", typeof(LessOrEqualExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new OperationNode(
                Source,
                Operation.CompareLessOrEqual,
                a.BuildCodeTree(state),
                b.BuildCodeTree(state));
        }
    }
}
