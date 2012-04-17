using System;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Grammars;
using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;

using Grammars = Katahdin.Grammars;

namespace Katahdin.Grammars
{
    public class OptionsNode : ParseGraphNode
    {
        private Optional<bool> buildTextNodes = new Optional<bool>();
        private Optional<Grammars.RecursionBehaviour> recursionBehaviour
            = new Optional<Grammars.RecursionBehaviour>(Grammars.RecursionBehaviour.Recursive);
        private Optional<bool> dropPrecedence = new Optional<bool>();
        private Optional<Pattern> whitespace = new Optional<Pattern>();
        private Optional<Pattern> exclude = new Optional<Pattern>();
        
        private ParseGraphNode body;
        
        public OptionsNode(Source source, ParseGraphNode body)
            : base(source)
        {
            this.body = body;
        }
        
        public override bool Update(Pattern updated)
        {
            bool updates = body.Update(updated);
            
            if (updates)
                Updated();
            
            return updates;
        }
        
        public override void CollectFields(OrderedSet<string> fields)
        {
            body.CollectFields(fields);
        }
        
        public override bool GetShouldRemember()
        {
            return body.GetShouldRemember();
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            int start = lexer.Position;
            
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), "options");
            
            bool oldBuildTextNodes = state.BuildTextNodes;
            Pattern oldWhitespace = lexer.WhitespacePattern;
            Precedence oldCurrentPrecedence = state.CurrentPrecedence;
            bool oldPrecedenceCanEqualCurrent = state.PrecedenceCanEqualCurrent;
            
            if (buildTextNodes.HasValue)
                state.BuildTextNodes = buildTextNodes.Value;
            
            if (whitespace.HasValue)
            {
                lexer.Whitespace(state.RuntimeState);
                lexer.WhitespacePattern = whitespace.Value;
            }
            
            if (exclude.HasValue)
                state.Excluded.Add(exclude.Value);
            
            if (dropPrecedence.HasValue && dropPrecedence.Value)
            {
                state.CurrentPrecedence = null;
                state.PrecedenceCanEqualCurrent = false;
            }
            
            ParseTree tree = body.Parse(lexer, state);
            
            state.BuildTextNodes = oldBuildTextNodes;
            lexer.WhitespacePattern = oldWhitespace;
        
            if (exclude.HasValue)
                state.Excluded.Remove(exclude.Value);
            
            state.CurrentPrecedence = oldCurrentPrecedence;
            state.PrecedenceCanEqualCurrent = oldPrecedenceCanEqualCurrent;
            
            if (tree == ParseTree.No)
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start));
                return ParseTree.No;
            }
            
            state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
            
            return tree;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of options --------------------");
            
            // todo enter
            
            block.BeginScope();
            
            block.Comment("save");
            
            // todo can remove this when no longer mixing jit and aot
            BlockLocal oldBuildTextNodes = null;
            
            BlockLocal oldWhitespace = null;
            BlockLocal excludeLocal = null;
            BlockLocal oldCurrentPrecedence = null;
            BlockLocal oldPrecedenceCanEqualCurrent = null;
            
            if (buildTextNodes.HasValue)
            {
                oldBuildTextNodes = new BlockLocal(typeof(bool));
                block.DeclareLocal(oldBuildTextNodes);
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("BuildTextNodes"));
                block.StoreLocal(oldBuildTextNodes);
                
                block.LoadState();
                block.Load(buildTextNodes.Value);
                block.SetProperty(typeof(ParserState).GetProperty("BuildTextNodes"));
            }
            
            if (whitespace.HasValue)
            {
                block.Whitespace(runtime, state);
                
                oldWhitespace = new BlockLocal(typeof(Pattern));
                block.DeclareLocal(oldWhitespace);
                
                block.LoadLexer();
                block.GetProperty(typeof(Lexer).GetProperty("WhitespacePattern"));
                block.StoreLocal(oldWhitespace);
                
                block.LoadLexer();
                block.Load(whitespace.Value);
                block.SetProperty(typeof(Lexer).GetProperty("WhitespacePattern"));
            }
            
            if (exclude.HasValue)
            {
                if (exclude.Value == null)
                    throw new Exception();
                
                excludeLocal = new BlockLocal(typeof(Pattern));
                block.DeclareLocal(excludeLocal);
                block.Load(exclude.Value);
                block.StoreLocal(excludeLocal);
            
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
                block.LoadLocal(excludeLocal);
                block.Call(typeof(Multiset<Pattern>).GetMethod("Add"));
            }
            
            if (dropPrecedence.HasValue && dropPrecedence.Value)
            {
                oldCurrentPrecedence = new BlockLocal(typeof(Precedence));
                block.DeclareLocal(oldCurrentPrecedence);
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
                block.StoreLocal(oldCurrentPrecedence);
                
                block.LoadState();
                block.LoadNull();
                block.SetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
                
                oldPrecedenceCanEqualCurrent = new BlockLocal(typeof(bool));
                block.DeclareLocal(oldPrecedenceCanEqualCurrent);
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
                block.StoreLocal(oldPrecedenceCanEqualCurrent);
                
                block.LoadState();
                block.Load(false);
                block.SetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
            }
            
            bool oldBuildTextNodesState = state.BuildTextNodes;
            Pattern oldWhitespaceState = state.Whitespace;
            
            if (buildTextNodes.HasValue)
                state.BuildTextNodes = buildTextNodes.Value;
            
            if (whitespace.HasValue)
                state.Whitespace = whitespace.Value;
            
            block.Emit(body.Compile(runtime, state)); 
            
            state.BuildTextNodes = oldBuildTextNodesState;
            state.Whitespace = oldWhitespaceState;
            
            block.Comment("restore");   
        
            if (buildTextNodes.HasValue)
            {
                block.LoadState();
                block.LoadLocal(oldBuildTextNodes);
                block.SetProperty(typeof(ParserState).GetProperty("BuildTextNodes"));
            }     
        
            if (whitespace.HasValue)
            {
                block.LoadLexer();
                block.LoadLocal(oldWhitespace);
                block.SetProperty(typeof(Lexer).GetProperty("WhitespacePattern"));
            }
        
            if (exclude.HasValue)
            {
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
                block.LoadLocal(excludeLocal);
                block.Call(typeof(Multiset<Pattern>).GetMethod("Remove", new Type[]{typeof(Pattern)}));
            }
        
            if (dropPrecedence.HasValue && dropPrecedence.Value)
            {
                block.LoadState();
                block.LoadLocal(oldCurrentPrecedence);
                block.SetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
                
                block.LoadState();
                block.LoadLocal(oldPrecedenceCanEqualCurrent);
                block.SetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
            }
            
            // todo yes or no
            
            block.Comment("end of options --------------------");
            
            block.EndScope();
            
            return block;
        }

        public static void Add(Multiset<Pattern> excluded, Pattern add)
        {
            excluded.Add(add);
        }
        
        public static void Remove(Multiset<Pattern> excluded, Pattern remove)
        {
            excluded.Remove(remove);
        }
        
        public Optional<bool> BuildTextNodes
        {
            get
            {
                return buildTextNodes;
            }
        }
        
        public Optional<RecursionBehaviour> RecursionBehaviour
        {
            get
            {
                return recursionBehaviour;
            }
        }
        
        public Optional<bool> DropPrecedence
        {
            get
            {
                return dropPrecedence;
            }
        }
        
        public Optional<Pattern> Whitespace
        {
            get
            {
                return whitespace;
            }
        }

        public Optional<Pattern> Exclude
        {
            get
            {
                return exclude;
            }
        }
        
        public ParseGraphNode Body
        {
            get
            {
                return body;
            }
        }
    }
}
