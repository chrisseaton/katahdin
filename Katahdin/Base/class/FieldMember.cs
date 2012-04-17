using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;
using Katahdin.Compiler;

namespace Katahdin.Base
{
    public class FieldMember : Member
    {
        public new static Pattern pattern;
        
        public Name name;
        
        public FieldMember(Source source, object name)
            : base(source)
        {
            this.name = (Name) name;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('field' l(name 0) ';')";
                Pattern[] patterns = {Name.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "FieldMember", parseGraph);
                pattern.SetType(typeof(FieldMember));
            
                Member.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("FieldMember", typeof(FieldMember));
            grammar.PatternDefined(pattern);
        }
        
        public override void Build(RuntimeState state, ClassBuilder builder)
        {
            builder.AddField(name.name);
        }
    }
}
