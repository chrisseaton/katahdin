using System;

using Katahdin.Collections;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class NotNode : ParseGraphNode
    {
        private ParseGraphNode body;
        
        public NotNode(Source source, ParseGraphNode body)
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
            // todo traces
            
            int start = lexer.Position;
            
            ParseTree bodyTree = body.Parse(lexer, state);
            
            if (bodyTree == ParseTree.No)
                return ParseTree.Yes;
            
            lexer.Position = start;
            
            return ParseTree.No;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of not --------------------");
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            block.BeginScope();
            
            BlockLocal start = block.SavePosition();
            
            // todo enter
            
            block.Emit(body.Compile(runtime, state));
            
            BlockLabel yes = new BlockLabel("yes");
            block.BranchIfNotNo(yes);
            
            // todo yes
            
            block.LoadYes();
            block.Branch(returnLabel);
            
            block.MarkLabel(yes);
            
            // todo no
            
            block.RestorePosition(start);
            block.LoadNo();
            
            block.MarkLabel(returnLabel);
            
            block.EndScope();
            
            block.Comment("end of not --------------------");
            
            return block;
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
