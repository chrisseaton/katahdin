using System;
using System.Collections.Generic;

using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class AnyNode : ParseGraphNode
    {
        public AnyNode(Source source)
            : base(source)
        {
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            lexer.Whitespace(state.RuntimeState);
            
            int start = lexer.Position;
            
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), "[]");
            
            char character = lexer.Peek();
            
            if (character == '\0')
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start), "End of source");
                return ParseTree.No;
            }
            
            lexer.Read();
            
            state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start), TextEscape.Quote(character));
            
            if (state.BuildTextNodes)
                return new ParseTree(Convert.ToString(character));
            else
                return ParseTree.Yes;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of any --------------------");
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            // todo enter
            
            block.BeginScope();
            
            block.LoadLexer();
            block.Call(typeof(Lexer).GetMethod("Peek", new Type[]{}));
            
            BlockLabel failed = new BlockLabel("failed");
            block.BranchIfFalse(failed);
            
            block.LoadLexer();
            block.Call(typeof(Lexer).GetMethod("Read"));
            
            // todo yes
            
            if (state.BuildTextNodes)
            {
                block.Call(typeof(Convert).GetMethod("ToString", new Type[]{typeof(char)}));
                block.New(typeof(ParseTree).GetConstructor(new Type[]{typeof(object)}));
            }
            else
            {
                block.Pop();
                block.LoadYes();
            }
            
            block.Branch(returnLabel);
            
            block.MarkLabel(failed);
            
            // todo no
            
            block.LoadNo();
            
            block.MarkLabel(returnLabel);
            
            block.EndScope();
            
            block.Comment("end of any --------------------");
            
            return block;
        }
    }
}
