using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class WhileStatement : Statement
    {
        public new static Pattern pattern;
        
        public Expression condition;
        public Statement body;
        
        public WhileStatement(Source source, object condition,
            object body)
                : base(source)
        {
            this.condition = (Expression) condition;
            this.body = (Statement) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('while' '(' l(condition 0) ')' l(body 1))";
                Pattern[] parameters = {Expression.pattern, Statement.pattern};
                
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "WhileStatement", parseGraph);
                pattern.SetType(typeof(WhileStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("WhileStatement", typeof(WhileStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new WhileNode(
                Source,
                condition.BuildCodeTree(state),
                body.BuildCodeTree(state));
        }
    }
}
