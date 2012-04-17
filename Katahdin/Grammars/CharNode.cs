using System;
using System.Collections.Generic;

using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class CharNode : ParseGraphNode
    {
        private CharRange range;
        
        public CharNode(Source source, char character)
            : base(source)
        {
            this.range = new CharRange(character);
        }
        
        public CharNode(Source source, CharRange range)
            : base(source)
        {
            this.range = range;
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            lexer.Whitespace(state.RuntimeState);
            
            int start = lexer.Position;
            
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), range.ToString());
            
            char character = lexer.Peek();
            
            if (!range.Contains(character))
            {
                lexer.ErrorStrings[start].Add(range.ToString());
                
                state.RuntimeState.Runtime.ParseTrace.No(this,
                    lexer.SourceFrom(start), TextEscape.Quote(character));
                
                return ParseTree.No;
            }
            
            lexer.Read();
            
            state.RuntimeState.Runtime.ParseTrace.Yes(this,
                lexer.SourceFrom(start), TextEscape.Quote(character));
            
            if (state.BuildTextNodes)
                return new ParseTree(Convert.ToString(character));
            else
                return ParseTree.Yes;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock(runtime);
            
            block.Comment("start of char --------------------");
            
            block.BeginScope();
            
            block.Whitespace(runtime, state);
            
            BlockLocal start = block.SavePosition();
            
            block.Enter(this, range.ToString());
            
            block.LoadLexer();
            block.Call(typeof(Lexer).GetMethod("Peek", new Type[]{}));
            
            BlockLabel failed = new BlockLabel("failed");
            
            BlockLocal character = null;
            
            if (range.Min == range.Max)
            {
                if (runtime.TraceParser)
                {
                    block.Dup();
                
                    character = new BlockLocal(typeof(char));
                    block.DeclareLocal(character);
                    block.StoreLocal(character);
                }
            
                block.Load(range.Min);
                block.BranchIfNotEqual(failed);
            }
            else
            {
                character = new BlockLocal(typeof(char));
                block.DeclareLocal(character);
                block.StoreLocal(character);
                
                block.LoadLocal(character);
                block.Load(range.Min);
                block.BranchIfLess(failed);
                
                block.LoadLocal(character);
                block.Load(range.Max);
                block.BranchIfGreater(failed);
            }
            
            block.LoadLexer();
            block.Call(typeof(Lexer).GetMethod("Read"));
            
            if (runtime.TraceParser)
            {
                block.LoadParseTrace();
                block.Load(this);
                block.LoadLexer();
                block.LoadLocal(start);
                block.Call(typeof(Lexer).GetMethod("SourceFrom"));
                block.LoadLocal(character);
                block.Call(typeof(TextEscape).GetMethod("Quote", new Type[]{typeof(char)}));
                block.Call(typeof(ParseTrace).GetMethod("Yes", new Type[]{typeof(object), typeof(Source), typeof(string)}));
            }
            
            if (state.BuildTextNodes)
            {
                if (range.Min == range.Max)
                {
                    block.Pop();
                    block.Load(Convert.ToString(range.Min));
                }
                else
                {
                    block.Call(typeof(Convert).GetMethod("ToString", new Type[]{typeof(char)}));
                }
                
                block.New(typeof(ParseTree).GetConstructor(new Type[]{typeof(object)}));
            }
            else
            {
                block.Pop();
                block.LoadYes();
            }
            
            BlockLabel returnLabel = new BlockLabel("return");
            block.Branch(returnLabel);
            
            block.MarkLabel(failed);
            
            if (runtime.TraceParser)
            {
                block.LoadParseTrace();
                block.Load(this);
                block.LoadLexer();
                block.LoadLocal(start);
                block.Call(typeof(Lexer).GetMethod("SourceFrom"));
                block.LoadLocal(character);
                block.Call(typeof(TextEscape).GetMethod("Quote", new Type[]{typeof(char)}));
                block.Call(typeof(ParseTrace).GetMethod("No", new Type[]{typeof(object), typeof(Source), typeof(string)}));
            }
            
            // todo error string
            
            block.LoadNo();
            
            block.EndScope();
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end of char --------------------");
            
            return block;
        }

        public CharRange Range
        {
            get
            {
                return range;
            }
        }
    }
}
