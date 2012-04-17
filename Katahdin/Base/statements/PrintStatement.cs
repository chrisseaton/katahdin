using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class PrintStatement : Statement
    {
        public new static Pattern pattern;
        
        public List<object> expressions;
        
        public PrintStatement(Source source,
            object expressions)
                : base(source)
        {
            this.expressions = (List<object>) expressions;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('print' l(expressions s(0 *(s(',' 0)))) ';')";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "PrintStatement", parseGraph);
                pattern.SetType(typeof(PrintStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("PrintStatement", typeof(PrintStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            Expression firstExpression = (Expression) expressions[0];
            
            CodeTreeNode printed = firstExpression.BuildCodeTree(state);
            
            printed = new ConvertNode(
                Source,
                printed,
                new ValueNode(
                    Source,
                    typeof(string)));
            
            for (int n = 1; n < expressions.Count; n++)
            {
                Expression expression = (Expression) expressions[n];
                CodeTreeNode append = expression.BuildCodeTree(state);
                
                CodeTreeNode sep = new ValueNode(
                    Source,
                    " ");
                
                append = new OperationNode(
                    Source,
                    Operation.Add,
                    sep,
                    append);
                
                printed = new OperationNode(
                    Source,
                    Operation.Add,
                    printed,
                    append);
            }
            
            return new PrintNode(
                Source,
                printed);
        }
    }
}
