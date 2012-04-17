using System;
using System.Collections.Generic;

using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class FailNode : ParseGraphNode
    {
        private string message;
        
        public FailNode(Source source, string message)
            : base(source)
        {
            this.message = message;
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            state.RuntimeState.Runtime.ParseTrace.Single(lexer.CurrentSource(), message);
            
            return ParseTree.No;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of fail --------------------");
            
            // todo no
            
            block.LoadNo();
            
            block.Comment("end of fail --------------------");
            
            return block;
        }
        
        public string Message
        {
            get
            {
                return message;
            }
        }
    }
}
