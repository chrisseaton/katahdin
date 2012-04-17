using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.Grammars;
using Katahdin.CodeTree;
using Katahdin.Compiler;

namespace Katahdin.Base
{
    public class ClassStatement : Statement
    {
        public new static Pattern pattern;
        
        public Name name;
        public Expression baseType;
        public List<object> members;
        
        private static MethodInfo defineMethod;
        
        public ClassStatement(Source source, object name, object baseType,
            object members)
                : base(source)
        {
            this.name = (Name) name;
            this.baseType = (Expression) baseType;
            this.members = (List<object>) members;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('class' l(name 0) ?(s(':' l(baseType a(2 3)))) a(';' s('{' l(members *(1)) '}')))";
                Pattern[] patterns = {Name.pattern, Member.pattern, MemberExpression.pattern, NameExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "ClassStatement", parseGraph);
                pattern.SetType(typeof(ClassStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ClassStatement", typeof(ClassStatement));
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
            AbstractPattern basePattern;
            
            Type baseType;

            if (this.baseType != null)
            {
                baseType = (Type) this.baseType.Get(state);
                basePattern = (AbstractPattern) Pattern.PatternForType(baseType);
            }
            else
            {
                baseType = typeof(RuntimeObject);
                basePattern = null;
            }

            ClassBuilder classBuilder = new ClassBuilder(
                state.Runtime.CompilerModule,
                state.Scope.GetModule(),
                name.name,
                baseType);

            // Go through members
            
            if (members != null)
            {
                foreach (object member in members)
                    ((Member) member).Build(state, classBuilder);
            }
            
            UserDefinedNode userDefined = null;;
            
            if (classBuilder.Pattern == null)
            {
                bool parseMethod = false;;
                
                foreach (Method method in classBuilder.Methods)
                    if (method.Name == "Parse")
                    {
                        parseMethod = true;
                        break;
                    }
                
                if (parseMethod)
                {
                    userDefined = new UserDefinedNode(Source,
                        null);
                    
                    classBuilder.Pattern = new ConcretePattern(Source,
                        name.name, userDefined);
                }
                else
                    classBuilder.Pattern = new AbstractPattern(Source,
                        classBuilder.TypeBuilder.Name);
            }

            // Create the class, which closes the pattern, and define the pattern

            Type type = classBuilder.CreateType();
            state.Runtime.Grammar.PatternDefined(classBuilder.Pattern);

            state.Scope.SetName(name.name, type);
            
            if (userDefined != null)
                userDefined.Type = type;

            // Add a concrete class as an alt to its abstract

            if (basePattern != null)
            {
                basePattern.AddAltPattern(classBuilder.Pattern);
                basePattern.Updated();
            }
        }
    }
}
