using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;
using Katahdin.Compiler;

namespace Katahdin.Base
{
    public class ConstructorMember : Member
    {
        public new static Pattern pattern;
        
        public List<object> parameters;
        public Statement body;
        
        public ConstructorMember(Source source, object parameters, object body)
            : base(source)
        {    
            this.parameters = (List<object>) parameters;
            this.body = (Statement) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string parameters = "'(' l(parameters ?(s(0 *(s(',' 0))))) ')'";
                string expression = "s('new' " + parameters + " l(body 1))";
                Pattern[] patterns = {Name.pattern, Statement.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "ConstructorMember", parseGraph);
                pattern.SetType(typeof(ConstructorMember));
            
                Member.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ConstructorMember", typeof(ConstructorMember));
            grammar.PatternDefined(pattern);
        }
        
        public override void Build(RuntimeState state, ClassBuilder builder)
        {
            List<string> parameters = new List<string>();
            
            foreach (Name parameter in this.parameters)
                parameters.Add(parameter.name);
            
            Method method = new Method(
                builder.TypeBuilder.Name + ".constructor",
                "constructor for " + builder.TypeBuilder.Name,
                true,
                parameters);
            
            method.SetCodeTree(body.BuildCodeTree(state));
            
            builder.AddConstructor(method);
        }
    }
}
