using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.Grammars;
using Katahdin.Grammars.Precedences;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class SetPrecedenceStatement : Statement
    {
        public new static Pattern pattern;
        
        public Expression a;
        public string relation;
        public Expression b;
        
        private static MethodInfo defineMethod;
        
        public SetPrecedenceStatement(Source source, object a,
            object relation, object b)
                : base(source)
        {
            this.a = (Expression) a;
            this.relation = (string) relation;
            this.b = (Expression) b;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('precedence' l(a a(0 1)) l(relation o((buildTextNodes true) a('<' '=' '>'))) l(b a(0 1)) ';')";
                Pattern[] patterns = {MemberExpression.pattern, NameExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "SetPrecedenceStatement", parseGraph);
                pattern.SetType(typeof(SetPrecedenceStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("SetPrecedenceStatement", typeof(SetPrecedenceStatement));
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
            Pattern a = Pattern.PatternForType((Type) this.a.Get(state));
            Pattern b = Pattern.PatternForType((Type) this.b.Get(state));
            
            Precedence.SetPrecedence(a.Precedence, b.Precedence,
                RelationFromString(relation));
            
            state.Runtime.Grammar.PatternChanged(a);
        }
        
        public Relation RelationFromString(string relation)
        {
            switch (relation)
            {
                case "<":
                    return Relation.Lower;
                case "=":
                    return Relation.Equal;
                case ">":
                    return Relation.Higher;
            }
            
            throw new Exception();
        }
    }
}
