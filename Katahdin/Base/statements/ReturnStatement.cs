using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class ReturnStatement : Statement
    {
        public new static Pattern pattern;
        
        public Expression expression;
        
        public ReturnStatement(Source source,
            object expression)
                : base(source)
        {
            this.expression = (Expression) expression;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('return' l(expression 0) ';')";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "ReturnStatement", parseGraph);
                pattern.SetType(typeof(ReturnStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ReturnStatement", typeof(ReturnStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new ReturnNode(
                Source,
                expression.BuildCodeTree(state));
        }
    }
}
