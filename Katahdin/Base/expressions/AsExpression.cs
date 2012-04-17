using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class AsExpression : TypeExpression
    {
        public new static Pattern pattern;
        
        public Expression obj;
        public Expression type;
        
        public AsExpression(Source source, object obj,
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
                string expression = "o((leftRecursive true) s(l(obj 0) 'as' l(type 0)))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "AsExpression", parseGraph);
                pattern.SetType(typeof(AsExpression));
            
                TypeExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AsExpression", typeof(AsExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new ConvertNode(
                Source,
                obj.BuildCodeTree(state),
                type.BuildCodeTree(state));
        }
    }
}
