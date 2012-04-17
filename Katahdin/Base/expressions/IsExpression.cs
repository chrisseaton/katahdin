using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class IsExpression : TypeExpression
    {
        public new static Pattern pattern;
        
        public Expression obj;
        public Expression type;
        
        public IsExpression(Source source, object obj,
            object type)
                : base(source)
        {
            this.obj = (Expression) obj;
            this.type = (Expression) type;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(obj 0) 'is' l(type 0)))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "IsExpression", parseGraph);
                pattern.SetType(typeof(IsExpression));
            
                TypeExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("IsExpression", typeof(IsExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new OperationNode(
                Source,
                Operation.Is,
                obj.BuildCodeTree(state),
                type.BuildCodeTree(state));
        }
    }
}
