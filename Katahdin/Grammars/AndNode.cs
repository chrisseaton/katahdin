using System;

using Katahdin.Collections;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class AndNode : ParseGraphNode
    {
        private ParseGraphNode body;
        
        public AndNode(Source source, ParseGraphNode body)
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
            
            // todo traces
            
            ParseTree bodyTree = body.Parse(lexer, state);
            
            if (bodyTree == ParseTree.No)
                return ParseTree.No;
            
            lexer.Position = start;
            
            return ParseTree.Yes;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of and --------------------");
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            block.BeginScope();
            
            BlockLocal start = block.SavePosition();
            
            // todo enter
            
            block.Emit(body.Compile(runtime, state));
            
            BlockLabel yes = new BlockLabel("yes");
            block.BranchIfNotNo(yes);
            
            // todo no
            
            block.LoadNo();
            block.Branch(returnLabel);
            
            block.MarkLabel(yes);
            
            // todo yes
            
            block.RestorePosition(start);
            block.LoadYes();
            
            block.MarkLabel(returnLabel);
            
            block.EndScope();
            
            block.Comment("end of and --------------------");
            
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
