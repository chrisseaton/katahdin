using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class TryStatement : Statement
    {
        public new static Pattern pattern;
        
        public Statement tryBody;
        public Expression catchVariable;
        public Statement catchBody;
        
        public TryStatement(Source source, object tryBody,
            object catchVariable, object catchBody)
                : base(source)
        {
            this.tryBody = (Statement) tryBody;
            this.catchVariable = (Expression) catchVariable;
            this.catchBody = (Statement) catchBody;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('try' l(tryBody 0) 'catch' ?(s('(' l(catchVariable 1) ')')) l(catchBody 0))";
                Pattern[] parameters = {Statement.pattern, Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "TryStatement", parseGraph);
                pattern.SetType(typeof(TryStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("TryStatement", typeof(TryStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new TryNode(
                Source,
                tryBody.BuildCodeTree(state),
                catchVariable != null ?
                    catchVariable.BuildCodeTree(state)
                  : null,
                catchBody.BuildCodeTree(state));
        }
    }
}
