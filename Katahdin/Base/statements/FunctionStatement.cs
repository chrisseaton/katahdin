using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.CodeTree;
using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class FunctionStatement : Statement
    {
        public new static Pattern pattern;
        
        public Name name;
        public List<object> parameters;
        public Statement body;
        
        private static MethodInfo defineMethod;
        
        public FunctionStatement(Source source, object name,
            object parameters, object body)
                : base(source)
        {
            this.name = (Name) name;
            this.parameters = (List<object>) parameters;
            this.body = (Statement) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string parameters = "'(' l(parameters ?(s(1 *(s(',' 0))))) ')'";
                string expression = "s('function' l(name 0) " + parameters + " l(body 2))";
                Pattern[] patterns = {Name.pattern, Name.pattern, Statement.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "FunctionStatement", parseGraph);
                pattern.SetType(typeof(FunctionStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("FunctionStatement", typeof(FunctionStatement));
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
            List<string> parameters = new List<string>();
            
            foreach (Name parameter in this.parameters)
                parameters.Add(parameter.name);
            
            Method function = new Method(
                name.name,
                name.name,
                false,
                parameters);
            
            function.SetCodeTree(body.BuildCodeTree(state));
            
            state.Scope.SetName(name.name, function);
        }
    }
}
