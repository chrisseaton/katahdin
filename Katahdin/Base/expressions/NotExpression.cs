using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class NotExpression : UnaryExpression
    {
        public new static Pattern pattern;
        
        public Expression a;
        
        public NotExpression(Source source, object a)
            : base(source)
        {
            this.a = (Expression) a;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((rightRecursive true) s('!' l(a 0))))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "NotExpression", parseGraph);
                pattern.SetType(typeof(NotExpression));
            
                UnaryExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("NotExpression", typeof(NotExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new OperationNode(
                Source,
                Operation.Not,
                a.BuildCodeTree(state));
        }
    }
}
