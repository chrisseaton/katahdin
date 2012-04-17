using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class MemberExpression : Expression
    {
        public new static Pattern pattern;
        
        public Expression obj;
        public Name member;
        
        public MemberExpression(Source source, object obj,
            object member)
                : base(source)
        {
            this.obj = (Expression) obj;
            this.member = (Name) member;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((leftRecursive true) s(l(obj 0) '.' l(member 1)))";
                Pattern[] parameters = {Expression.pattern, Name.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "MemberExpression", parseGraph);
                pattern.SetType(typeof(MemberExpression));
            
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("MemberExpression", typeof(MemberExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            CodeTreeNode node = obj.BuildCodeTree(state);
            
            string name = member.name;
            node = new MemberNode(Source, node, name);
            
            return node;
        }
    }
}
