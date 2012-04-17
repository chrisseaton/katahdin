using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class NewExpression : Expression
    {
        public new static Pattern pattern;
        
        public Expression type;
        public List<object> parameters;
        
        public NewExpression(Source source, object type,
            object parameters)
                : base(source)
        {
            this.type = (Expression) type;
            this.parameters = (List<object>) parameters;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('new' l(type o((exclude 1) 0)) '(' o((dropPrecedence true) l(parameters ?(s(0 *(s(',' 0)))))) ')')";
                Pattern[] parameters = {Expression.pattern, CallExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "NewExpression", parseGraph);
                pattern.SetType(typeof(NewExpression));
            
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("NewExpression", typeof(NewExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            CodeTreeNode[] parameterNodes = new CodeTreeNode[parameters.Count];
            
            for (int n = 0; n < parameters.Count; n++)
                parameterNodes[n] =
                    ((Expression) parameters[n]).BuildCodeTree(state);
        
            return new NewNode(
                Source,
                type.BuildCodeTree(state),
                parameterNodes);
        }
    }
}
