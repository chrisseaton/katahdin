using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;
using Katahdin.Compiler;

namespace Katahdin.Base
{
    public class MethodMember : Member
    {
        public new static Pattern pattern;
        
        public Name name;
        public List<object> parameters;
        public Statement body;
        
        public MethodMember(Source source, object name, object parameters, object body)
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
                string parameters = "'(' l(parameters ?(s(0 *(s(',' 0))))) ')'";
                string expression = "s('method' l(name 0) " + parameters + " l(body 1))";
                Pattern[] patterns = {Name.pattern, Statement.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "MethodMember", parseGraph);
                pattern.SetType(typeof(MethodMember));
            
                Member.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("MethodMember", typeof(MethodMember));
            grammar.PatternDefined(pattern);
        }
        
        public override void Build(RuntimeState state, ClassBuilder builder)
        {
            List<string> parameters = new List<string>();
            
            foreach (Name parameter in this.parameters)
                parameters.Add(parameter.name);
            
            Method method = new Method(
                builder.TypeBuilder.Name + "." + name.name,
                name.name,
                true,
                parameters);
            
            method.SetCodeTree(body.BuildCodeTree(state));
            
            builder.AddMethod(method);
        }
    }
}
