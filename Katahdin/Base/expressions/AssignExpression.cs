using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class AssignExpression : AssignmentExpression
    {
        public new static Pattern pattern;
        
        public Expression from;
        public Expression to;
        
        public AssignExpression(Source source, object to,
            object from)
                : base(source)
        {
            this.from = (Expression) from;
            this.to = (Expression) to;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(to 0) '=' l(from 0)))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "AssignExpression", parseGraph);
                pattern.SetType(typeof(AssignExpression));
            
                AssignmentExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("AssignExpression", typeof(AssignExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new AssignNode(
                Source,
                from.BuildCodeTree(state),
                to.BuildCodeTree(state));
        }
    }
}
