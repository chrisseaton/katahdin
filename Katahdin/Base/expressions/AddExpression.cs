using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class AddExpression : AdditiveExpression
    {
        public new static Pattern pattern;
        
        public Expression a;
        public Expression b;
        
        public AddExpression(Source source, object a, object b)
            : base(source)
        {
            this.a = (Expression) a;
            this.b = (Expression) b;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(a 0) '+' l(b 0)))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression,
                    parameters);
        
                pattern = new ConcretePattern(null, "AddExpression", parseGraph);
                pattern.SetType(typeof(AddExpression));
            
                AdditiveExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AddExpression", typeof(AddExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new OperationNode(
                Source,
                Operation.Add,
                a.BuildCodeTree(state),
                b.BuildCodeTree(state));
        }
    }
}
