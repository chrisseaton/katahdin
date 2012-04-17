using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Collections;
using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public abstract class ParseGraphNode : IParseable, ISourced
    {
        private Source source;

        private Dictionary<StateForCompiler, Block> blockCache
            = new Dictionary<StateForCompiler, Block>();

        private Dictionary<StateForCompiler, DynamicMethod> methodCache
            = new Dictionary<StateForCompiler, DynamicMethod>();
        
        protected ParseGraphNode(Source source)
        {
            this.source = source;
        }
        
        public virtual bool Update(Pattern updated)
        {
            return false;
        }
        
        public void Updated()
        {
            blockCache.Clear();
            methodCache.Clear();
        }
        
        public OrderedSet<string> GetFields()
        {
            OrderedSet<string> fields = new OrderedSet<string>();
            CollectFields(fields);
            return fields;
        }
        
        public virtual void CollectFields(OrderedSet<string> fields)
        {
        }
        
        public virtual bool GetShouldRemember()
        {
            return true;
        }
        
        public abstract ParseTree Parse(Lexer lexer, ParserState state);
        public abstract Block CompileNewState(Runtime runtime, StateForCompiler state);
        
        public Block Compile(Runtime runtime, StateForCompiler state)
        {
            Block block;
            
            if (!blockCache.TryGetValue(state, out block))
            {
                block = CompileNewState(runtime, state);
                blockCache[state] = block;
                
                
                
                /*Console.WriteLine("testing " + GetType().Name);
                
                ParserBlock methodBlock = new ParserBlock();
            
                methodBlock.Emit(block);
                methodBlock.Return();
            
                DynamicMethod compiled = new DynamicMethod(
                    "test",
                    typeof(ParseTree), 
                    new Type[]{typeof(Lexer), typeof(ParserState)},
                    runtime.CompilerModule.ModuleBuilder);
        
                methodBlock.Build(new DynamicMethodProxy(compiled));
                
                try
                {
                    compiled.Invoke(null, new object[]{null, null});
                }
                catch (InvalidProgramException exception)
                {
                    throw exception;
                }
                catch
                {
                }
                
                Console.WriteLine("done testing " + GetType().Name);*/
                
                
                
                
                
            }
            
            return block;
        }

        public virtual Precedence Precedence
        {
            get
            {
                return null;
            }
        }
        
        public Source Source
        {
            get
            {
                return source;
            }
        }
    }
}
