using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class ModuleStatement : Statement
    {
        public new static Pattern pattern;
        
        public Name name;
        public Statement body;
        
        private static MethodInfo defineMethod;
        
        public ModuleStatement(Source source, object name,
            object body)
                : base(source)
        {
            this.name = (Name) name;
            this.body = (Statement) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('module' l(name 0) l(body 1))";
                Pattern[] patterns = {Name.pattern, Statement.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "ModuleStatement", parseGraph);
                pattern.SetType(typeof(ModuleStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ModuleStatement", typeof(ModuleStatement));
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
            Module parent = (Module) state.Scope;
            
            Module module = new Module(parent, name.name);
            state.Scope.SetName(name.name, module);
            
            state.Scope = module;
            body.Run(state);
            state.Scope = parent;
            
            if (state.Returning != null)
                throw new Exception("Statement within a module block returned");
        }
    }
}
