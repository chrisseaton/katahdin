using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class CallExpression : Expression
    {
        public new static Pattern pattern;
        
        public Expression callable;
        public List<object> parameters;
        
        public CallExpression(Source source, object callable,
            object parameters)
                : base(source)
        {
            this.callable = (Expression) callable;
            this.parameters = (List<object>) parameters;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(callable 0) '(' o((dropPrecedence true) l(parameters ?(s(0 *(s(',' 0)))))) ')'))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "CallExpression", parseGraph);
                pattern.SetType(typeof(CallExpression));
            
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("CallExpression", typeof(CallExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            CodeTreeNode[] parameterNodes = new CodeTreeNode[parameters.Count];
            
            for (int n = 0; n < parameters.Count; n++)
                parameterNodes[n] =
                    ((Expression) parameters[n]).BuildCodeTree(state);
            
            return new CallNode(
				Source,
                callable.BuildCodeTree(state),
                parameterNodes);
        }
    }
}
