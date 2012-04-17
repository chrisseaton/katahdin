using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class ThrowStatement : Statement
    {
        public new static Pattern pattern;
        
        public Expression exception;
        
        public ThrowStatement(Source source, object exception)
            : base(source)
        {
            this.exception = (Expression) exception;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('throw' l(exception 0) ';')";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "ThrowStatement", parseGraph);
                pattern.SetType(typeof(ThrowStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ThrowStatement", typeof(ThrowStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new ThrowNode(
                Source,
                exception.BuildCodeTree(state));
        }
    }
}
