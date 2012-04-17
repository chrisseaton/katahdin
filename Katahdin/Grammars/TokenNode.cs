using System;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class TokenNode : ParseGraphNode
    {
        private ParseGraphNode body;
        
        public TokenNode(Source source, ParseGraphNode body)
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
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), "token");
            
            int start = lexer.Position;
            
            ParseTree bodyTree = body.Parse(lexer, state);
            
            if (bodyTree == ParseTree.No)
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start));
                return ParseTree.No;
            }
            
            state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
            
            ParseTree tokenTree = new ParseTree(
                lexer.Text.Substring(start, lexer.Position - start));
            
            tokenTree = tokenTree.ExtendFields(bodyTree);
            
            return tokenTree;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of token --------------------");
            
            // todo enter
            
            block.BeginScope();
            
            BlockLocal start = block.SavePosition();
            
            block.Emit(body.Compile(runtime, state));
            
            BlockLocal bodyTree = new BlockLocal(typeof(ParseTree));
            block.DeclareLocal(bodyTree);
            block.Dup();
            block.StoreLocal(bodyTree);
            
            BlockLabel yes = new BlockLabel("yes");
            block.BranchIfNotNo(yes);
            
            // todo no
            
            block.LoadNo();
            
            BlockLabel returnLabel = new BlockLabel("return");
            block.Branch(returnLabel);
            
            block.MarkLabel(yes);
            
            // todo yes
            
            block.LoadLexer();
            block.GetProperty(typeof(Lexer).GetProperty("Text"));
            block.LoadLocal(start);
            block.LoadLexer();
            block.GetProperty(typeof(Lexer).GetProperty("Position"));
            block.LoadLocal(start);
            block.Sub();
            block.Call(typeof(string).GetMethod("Substring", new Type[]{typeof(int), typeof(int)}));
            block.New(typeof(ParseTree).GetConstructor(new Type[]{typeof(object)}));
            
            block.LoadLocal(bodyTree);
            block.Call(typeof(ParseTree).GetMethod("ExtendFields"));
            
            block.MarkLabel(returnLabel);
            
            block.EndScope();
            
            block.Comment("end of token --------------------");
            
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
