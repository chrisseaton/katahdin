using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class ExpressionStatement : Statement
    {
        public new static Pattern pattern;
        
        public Expression expression;
        
        public ExpressionStatement(Source source,
            object expression)
                : base(source)
        {
            this.expression = (Expression) expression;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s(l(expression 0) ';')";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "ExpressionStatement", parseGraph);
                pattern.SetType(typeof(ExpressionStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ExpressionStatement", typeof(ExpressionStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new GetToRunNode(
                Source,
                expression.BuildCodeTree(state));
        }
    }
}
