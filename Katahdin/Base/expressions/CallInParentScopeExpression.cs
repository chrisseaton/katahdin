using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class CallInParentScopeExpression : Expression
    {
        public new static Pattern pattern;
        
        public Expression callable;
        public List<object> parameters;
        
        public CallInParentScopeExpression(Source source, object callable,
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
                string expression = "o((leftRecursive true) s(l(callable 0) '...' '(' o((dropPrecedence true) l(parameters ?(s(0 *(s(',' 0)))))) ')'))";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "CallInParentScopeExpression", parseGraph);
                pattern.SetType(typeof(CallInParentScopeExpression));
            
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("CallInParentScopeExpression",
                typeof(CallInParentScopeExpression));
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
                true,
                parameterNodes);
        }
    }
}
