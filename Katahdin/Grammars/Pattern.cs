using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Collections;
using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;
using Katahdin.Base;

namespace Katahdin.Grammars
{
    public abstract class Pattern : ISourced
    {
        private Source source;
        
        private Type type;
        
        private Precedence precedence;
        private ParseGraphNode parseGraph;
        
        private OrderedSet<string> fields;
        private bool shouldRemember;
        
        private List<PatternTrampoline> trampolines = new List<PatternTrampoline>();
        
        private Dictionary<StateForCompiler, PatternTrampoline> trampolineMap
            = new Dictionary<StateForCompiler, PatternTrampoline>();
        
        public Pattern(Source source, string name)
        {
            this.source = source;
            precedence = new Precedence(name);
        }
        
        public virtual bool Update(Pattern updated)
        {
            bool updates = ParseGraph.Update(updated);
            
            if (updates)
                Updated();
            
            return updates;
        }
        
        public void Updated()
        {
            foreach (PatternTrampoline trampoline in trampolines)
                trampoline.Updated();
        }
        
        public abstract ParseTree Parse(Lexer lexer, ParserState state);
        public abstract Block CompileNew(Runtime runtime, StateForCompiler state);
        
        public ParseTree ParseUsingCompiler(Lexer lexer, ParserState state)
        {
            StateForCompiler stateForCompiler = new StateForCompiler();
            stateForCompiler.BuildTextNodes = state.BuildTextNodes;
            stateForCompiler.RecursionBehaviour = state.RecursionBehaviour;
            stateForCompiler.LeftRecursiveAlts = state.LeftRecursiveAlts;
            stateForCompiler.Whitespace = lexer.WhitespacePattern;
            
            PatternTrampoline trampoline = GetTrampoline(state.RuntimeState.Runtime, stateForCompiler);
            return trampoline.Implementation(lexer, state);
        }
        
        public Block CompileCall(Runtime runtime, StateForCompiler state)
        {
            return GetTrampoline(runtime, state).CallBlock;
        }
        
        public PatternTrampoline GetTrampoline(Runtime runtime, StateForCompiler state)
        {
            PatternTrampoline trampoline;
            
            if (!trampolineMap.TryGetValue(state, out trampoline))
            {
                state = state.Copy();
                
                trampoline = new PatternTrampoline(this, runtime, state);
                trampolines.Add(trampoline);
                trampolineMap[state] = trampoline;
            }
            
            return trampoline;
        }
        
        public ParseDelegate CompileImplementation(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Emit(CompileNew(runtime, state));
            block.Return();
        
            DynamicMethod implementation = new DynamicMethod(
                "parse" + type.Name,
                typeof(ParseTree), 
                new Type[]{typeof(Lexer), typeof(ParserState)},
                runtime.CompilerModule.ModuleBuilder);
    
            block.Build(new DynamicMethodProxy(implementation));
            
            return (ParseDelegate) implementation.CreateDelegate(typeof(ParseDelegate));
        }
        
        public Source Source
        {
            get
            {
                return source;
            }
        }
        
        public Type Type
        {
            get
            {
                if (type == null)
                    throw new Exception("Pattern's class has not been set");
                
                return type;
            }
        }
        
        public void SetType(Type type)
        {
            if (this.type != null)
                throw new Exception(type.ToString());
            
            this.type = type;
        }
        
        public static Pattern PatternForType(Type type)
        {
            // TODO - make virtual method to access pattern variable
            
            return (Pattern) type.GetField("pattern").GetValue(null);
        }
        
        public Precedence Precedence
        {
            get
            {
                return precedence;
            }
            
            set
            {
                this.precedence = value;
            }
        }
        
        public ParseGraphNode ParseGraph
        {
            get
            {
                return parseGraph;
            }
        }
        
        public OrderedSet<string> Fields
        {
            get
            {
                return fields;
            }
        }
        
        public bool ShouldRemember
        {
            get
            {
                return shouldRemember;
            }
        }
        
        protected void SetParseGraph(ParseGraphNode parseGraph)
        {
            this.parseGraph = parseGraph;
            fields = parseGraph.GetFields();
            shouldRemember = parseGraph.GetShouldRemember();
        }
    }
}
