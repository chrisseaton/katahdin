using System;
using System.Collections.Generic;

using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class TextNode : ParseGraphNode
    {
        private string text;
        
        public TextNode(Source source, string text)
            : base(source)
        {
            if (text.Length == 0)
                throw new ArgumentException("Can't create TextNode with an empty string");
            
            if (text.Length == 1)
                throw new ArgumentException("Use CharNode for single characters");
            
            this.text = text;
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            lexer.Whitespace(state.RuntimeState);
            
            int start = lexer.Position;
            
            state.RuntimeState.Runtime.ParseTrace.Enter(
                this,
                lexer.CurrentSource(),
                TextEscape.Quote(text));
            
            for (int n = 0; n < text.Length; n++)
            {
                char character = lexer.Peek(n);
                
                if (character != text[n])
                {
                    string stoppedString;
                    
                    if (character == '\0')
                        stoppedString = "end";
                    else
                        stoppedString = TextEscape.Quote(character);
                    
                    lexer.ErrorStrings[start + n].Add(TextEscape.Quote(text));
                    
                    if (n > 0)
                        state.RuntimeState.Runtime.ParseTrace.No(
                            this,
                            lexer.SourceFrom(start),
                            TextEscape.Quote(lexer.Text.Substring(start, n))
                                + "..." + stoppedString);
                    else
                        state.RuntimeState.Runtime.ParseTrace.No(
                            this,
                            lexer.SourceFrom(start),
                            stoppedString);
                    
                    return ParseTree.No;
                }
            }
            
            lexer.Skip(text.Length);
            
            state.RuntimeState.Runtime.ParseTrace.Yes(this,
                lexer.SourceFrom(start));
            
            if (state.BuildTextNodes)
                return new ParseTree(text);
            else
                return ParseTree.Yes;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock(runtime);
            
            block.Comment("start of text --------------------");
            
            block.Enter(this, TextEscape.Quote(text));
            
            block.BeginScope();
            
            block.Whitespace(runtime, state);
            BlockLocal start = block.SavePosition();
            
            BlockLocal n = new BlockLocal(typeof(int));
            block.DeclareLocal(n);
            
            BlockLabel reitterate = new BlockLabel("reitterate");
            block.MarkLabel(reitterate);
            
            block.Comment("itteration");
            
            BlockLocal character = new BlockLocal(typeof(char));
            block.DeclareLocal(character);
            
            block.LoadLexer();
            block.LoadLocal(n);
            block.Call(typeof(Lexer).GetMethod("Peek",
                new Type[]{typeof(int)}));
            
            block.Dup();
            block.StoreLocal(character);
            
            block.Load(text);
            block.LoadLocal(n);
            block.GetProperty(typeof(string).GetProperty("Chars"));
            
            BlockLabel matched = new BlockLabel("matched");
            block.BranchIfEqual(matched);
            
            block.Comment("handle the failure");
            
            // todo error string
            
            // todo specifics
            block.No(this, start);
            
            block.LoadNo();
            
            BlockLabel returnLabel = new BlockLabel("return");
            block.Branch(returnLabel);
            
            block.MarkLabel(matched);
            
            block.Comment("increment and check the loop variable");
            
            block.LoadLocal(n);
            block.Increment();
            block.Dup();
            
            block.StoreLocal(n);
            
            block.Load(text.Length);
            block.BranchIfLess(reitterate);
            
            block.Comment("skip over the text");
            
            block.LoadLexer();
            block.Load(text.Length);
            block.Call(typeof(Lexer).GetMethod("Skip"));
            
            // todo specifics
            block.Yes(this, start);
            
            block.Comment("create the parse tree");
            
            if (state.BuildTextNodes)
            {
                block.Load(text);
                block.New(typeof(ParseTree).GetConstructor(
                    new Type[]{typeof(object)}));
            }
            else
            {
                block.LoadYes();
            }
            
            block.EndScope();
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end of text --------------------");
            
            return block;
        }
        
        public string Text
        {
            get
            {
                return text;
            }
        }
    }
}
