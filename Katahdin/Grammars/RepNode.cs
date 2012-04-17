using System;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class RepNode : ParseGraphNode
    {
        private ParseGraphNode body;
        private Reps reps;
        
        public RepNode(Source source, ParseGraphNode body, Reps reps)
            : base(source)
        {
            this.body = body;
            this.reps = reps;
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
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), Reps.Name);
            
            int start = lexer.Position;
            bool matched = false;
            
            ParseTree tree = ParseTree.Yes;
            
            while (true)
            {
                ParseTree repTree = body.Parse(lexer, state);
                
                if (repTree == ParseTree.No)
                    break;
                
                tree = tree.Extend(repTree);
                
                if (reps.MaximumOfOne)
                    break;
                
                matched = true;
            }
            
            if ((reps.Minimum > 0) && !matched)
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start), "Not enough reps");
                
                lexer.Position = start;
                return ParseTree.No;
            }
            
            state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
            
            return tree;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of rep " + reps.Name + " --------------------");
            
            // todo enter
            
            block.BeginScope();
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            BlockLocal start = block.SavePosition();
            
            block.NewParseTree();
            
            BlockLabel reiterate = null;
            BlockLocal matched = null;
            
            if (!reps.MaximumOfOne)
            {
                reiterate = new BlockLabel("reiterate");
                block.MarkLabel(reiterate);
                
                matched = new BlockLocal(typeof(bool));
                block.DeclareLocal(matched);
            }
            
            BlockLabel breakLabel = new BlockLabel("break");
            
            block.Emit(body.Compile(runtime, state));
            
            BlockLabel yes = new BlockLabel("yes");
            block.Dup();
            block.BranchIfNotNo(yes);
            
            block.Pop();
            
            block.Branch(breakLabel);
            
            block.MarkLabel(yes);
            
            block.Call(typeof(ParseTree).GetMethod("Extend"));
            
            if (!reps.MaximumOfOne)
            {
                block.Load(true);
                block.StoreLocal(matched);
                
                block.Branch(reiterate);
            }
            
            block.MarkLabel(breakLabel);
            
            if (reps.Minimum > 0)
            {
                BlockLabel enough = new BlockLabel("enough");
                block.LoadLocal(matched);
                block.BranchIfTrue(enough);
                
                // todo no
                
                block.RestorePosition(start);
                
                block.Pop();
                block.LoadNo();
                
                block.Branch(returnLabel);
                
                block.MarkLabel(enough);
            }

            // todo yes
            
            block.MarkLabel(returnLabel);
            
            block.EndScope();
            
            block.Comment("end of rep --------------------");
            
            return block;
        }
        
        public ParseGraphNode Body
        {
            get
            {
                return body;
            }
        }
        
        public Reps Reps
        {
            get
            {
                return reps;
            }
        }
    }
}
