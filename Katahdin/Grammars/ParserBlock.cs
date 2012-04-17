using System;

using Katahdin.Base;
using Katahdin.Grammars;

namespace Katahdin.Compiler
{
    public class ParserBlock : Block
    {
        private Runtime runtime;
        
        public ParserBlock()
        {
        }
        
        public ParserBlock(Runtime runtime)
        {
            this.runtime = runtime;
        }
        
        // TODO store type members in static fields
        
        public void LoadLexer()
        {
            LoadArg(0);
        }
        
        public void LoadState()
        {
            LoadArg(1);
        }
        
        public void LoadRuntimeState()
        {
            LoadState();
            GetProperty(typeof(ParserState).GetProperty("RuntimeState"));
        }

        public void LoadRuntime()
        {
            LoadRuntimeState();
            GetProperty(typeof(RuntimeState).GetProperty("Runtime"));
        }

        public void LoadParseTrace()
        {
            LoadRuntime();
            GetProperty(typeof(Runtime).GetProperty("ParseTrace"));
        }

        public void LoadYes()
        {
            GetProperty(typeof(ParseTree).GetProperty("Yes"));
        }
    
        public void LoadNo()
        {
            GetProperty(typeof(ParseTree).GetProperty("No"));
        }
        
        public BlockLocal SavePosition()
        {
            BlockLocal local = new BlockLocal(typeof(int));
            DeclareLocal(local);
            
            LoadLexer();
            GetProperty(typeof(Lexer).GetProperty("Position"));
            StoreLocal(local);
            
            return local;
        }
        
        public void RestorePosition(BlockLocal local)
        {
            LoadLexer();
            LoadLocal(local);
            SetProperty(typeof(Lexer).GetProperty("Position"));
        }
        
        public void BranchIfNo(BlockLabel label)
        {
            GetProperty(typeof(ParseTree).GetProperty("No"));
            BranchIfEqual(label);
        }
        
        public void BranchIfNotNo(BlockLabel label)
        {
            LoadNo();
            BranchIfNotEqual(label);
        }
        
        public void NewParseTree()
        {
            New(typeof(ParseTree));
        }
        
        public void Whitespace(Runtime runtime, StateForCompiler state)
        {
            if (state.Whitespace == null)
                return;
            
            if (state.Whitespace == Katahdin.Base.Whitespace.pattern)
            {
                LoadLexer();
                Call(typeof(Lexer).GetMethod("QuickWhitespace"));
                return;
            }
            
            if (state.Whitespace == DefaultWhitespace.pattern)
            {
                LoadLexer();
                Call(typeof(Lexer).GetMethod("QuickDefaultWhitespace"));
                return;
            }
            
            Pattern oldWhitespace = state.Whitespace;
            state.Whitespace = null;
            
            Emit(oldWhitespace.CompileCall(runtime, state));
            Pop();
            
            state.Whitespace = oldWhitespace;
        }
        
        public void Single(string label)
        {
            if (!runtime.TraceParser)
                return;
            
            LoadParseTrace();
            LoadLexer();
            Call(typeof(Lexer).GetMethod("CurrentSource"));
            Load(label);
            Call(typeof(ParseTrace).GetMethod("Single", new Type[]{typeof(Source), typeof(string)}));
        }

        public void Single(BlockLocal start, string label)
        {
            if (!runtime.TraceParser)
                return;

            LoadParseTrace();
            LoadLexer();
            LoadLocal(start);
            Call(typeof(Lexer).GetMethod("SourceFrom"));
            Load(label);
            Call(typeof(ParseTrace).GetMethod("Single", new Type[]{typeof(Source), typeof(string)}));
        }
    
        public void Enter(object marker, string label)
        {
            if (!runtime.TraceParser)
                return;
            
            LoadParseTrace();
            Load(marker);
            LoadLexer();
            Call(typeof(Lexer).GetMethod("CurrentSource"));
            Load(label);
            Call(typeof(ParseTrace).GetMethod("Enter"));
        }
        
        public void Yes(object marker, BlockLocal start)
        {
            if (!runtime.TraceParser)
                return;
            
            LoadParseTrace();
            Load(marker);
            LoadLexer();
            LoadLocal(start);
            Call(typeof(Lexer).GetMethod("SourceFrom"));
            Call(typeof(ParseTrace).GetMethod("Yes", new Type[]{typeof(object), typeof(Source)}));
        }
        
        public void Yes(object marker, BlockLocal start, string label)
        {
            if (!runtime.TraceParser)
                return;
            
            LoadParseTrace();
            Load(marker);
            LoadLexer();
            LoadLocal(start);
            Call(typeof(Lexer).GetMethod("SourceFrom"));
            Load(label);
            Call(typeof(ParseTrace).GetMethod("Yes", new Type[]{typeof(object), typeof(Source), typeof(string)}));
        }

        public void No(object marker)
        {
            if (!runtime.TraceParser)
                return;

            LoadParseTrace();
            Load(marker);
            LoadLexer();
            Call(typeof(Lexer).GetMethod("CurrentSource"));
            Call(typeof(ParseTrace).GetMethod("No", new Type[]{typeof(object), typeof(Source)}));
        }
        
        public void No(object marker, BlockLocal start)
        {
            if (!runtime.TraceParser)
                return;
            
            LoadParseTrace();
            Load(marker);
            LoadLexer();
            LoadLocal(start);
            Call(typeof(Lexer).GetMethod("SourceFrom"));
            Call(typeof(ParseTrace).GetMethod("No", new Type[]{typeof(object), typeof(Source)}));
        }
        
        public void No(object marker, BlockLocal start, string label)
        {
            if (!runtime.TraceParser)
                return;
            
            LoadParseTrace();
            Load(marker);
            LoadLexer();
            LoadLocal(start);
            Call(typeof(Lexer).GetMethod("SourceFrom"));
            Load(label);
            Call(typeof(ParseTrace).GetMethod("No", new Type[]{typeof(object), typeof(Source), typeof(string)}));
        }
        
        public void No(object marker, string label)
        {
            if (!runtime.TraceParser)
                return;
            
            LoadParseTrace();
            Load(marker);
            LoadLexer();
            Call(typeof(Lexer).GetMethod("CurrentSource"));
            Load(label);
            Call(typeof(ParseTrace).GetMethod("No", new Type[]{typeof(object), typeof(Source), typeof(string)}));
        }
    }
}
