using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class ParenExpression : Expression
    {
        public new static Pattern pattern;
        
        public Expression body;
        
        public ParenExpression(Source source, object body)
            : base(source)
        {
            this.body = (Expression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('(' o((dropPrecedence true) l(body 0)) ')')";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "ParenExpression", parseGraph);
                pattern.SetType(typeof(ParenExpression));
                
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ParenExpression", typeof(ParenExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return body.BuildCodeTree(state);
        }
    }
}
