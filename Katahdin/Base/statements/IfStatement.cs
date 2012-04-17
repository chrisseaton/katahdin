using System;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class IfStatement : Statement
    {
        public new static Pattern pattern;
        
        public Expression condition;
        public Statement trueBody;
        public Statement falseBody;
        
        public IfStatement(Source source, object condition,
            object trueBody, object falseBody)
                : base(source)
        {
            this.condition = (Expression) condition;
            this.trueBody = (Statement) trueBody;
            this.falseBody = (Statement) falseBody;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('if' '(' l(condition 0) ')' l(trueBody 1) ?(s('else' l(falseBody 1))))";
                Pattern[] parameters = {Expression.pattern, Statement.pattern};
                
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "IfStatement", parseGraph);
                pattern.SetType(typeof(IfStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("IfStatement", typeof(IfStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new IfNode(
                Source,
                condition.BuildCodeTree(state),
                trueBody.BuildCodeTree(state),
                falseBody == null ?
                    null
                  : falseBody.BuildCodeTree(state));
        }
    }
}
