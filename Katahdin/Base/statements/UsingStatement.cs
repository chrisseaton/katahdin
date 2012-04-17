using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class UsingStatement : Statement
    {
        public new static Pattern pattern;
        
        public Expression expression;
        
        private static MethodInfo defineMethod;
        
        public UsingStatement(Source source, object expression)
            : base(source)
        {
            this.expression = (Expression) expression;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('using' l(expression 0) ';')";
                Pattern[] parameters = {Expression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "UsingStatement", parseGraph);
                pattern.SetType(typeof(UsingStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("UsingStatement", typeof(UsingStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            if (defineMethod == null)
            {
                Type[] parameterTypes = new Type[]{typeof(RuntimeState)};
                defineMethod = GetType().GetMethod("Define", parameterTypes);
                
                if (defineMethod == null)
                    throw new Exception();
            }
            
            CodeTreeNode callable = new ValueNode(
                Source,
                new ClrObjectMethodBinding(
                    this,
                    defineMethod));
            
            return new GetToRunNode(
                Source,
                new CallNode(
                    Source,
                    callable,
                    null));
        }

        public void Define(RuntimeState state)
        {
            Type type = (Type) ((Expression) expression).Get(state);
            Pattern pattern = Pattern.PatternForType(type);
            
            state.Runtime.Grammar.RootPattern = pattern;
            state.Runtime.CurrentLexer.RootChanged();
        }
    }
}
