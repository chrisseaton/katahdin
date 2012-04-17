using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Compiler;

namespace Katahdin.Grammars.Alts
{
    public class LongestAlt : Alt
    {
        private List<IParseable> alts;
        
        public LongestAlt(Source source, List<IParseable> alts)
            : base(source)
        {
            this.alts = alts;
        }
    
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            state.RuntimeState.Runtime.ParseTrace.Enter(this,
                lexer.CurrentSource(), "longest alt");

            int start = lexer.Position;
            
            ParseTree longestTree = null;
            int longestTreeEnd = -1;
            
			foreach (IParseable alt in alts)
			{
				ParseTree tree = alt.Parse(lexer, state);
				
				if (tree != ParseTree.No)
				{
					if (lexer.Position > longestTreeEnd)
					{
					    longestTree = tree;
					    longestTreeEnd = lexer.Position;
					}
				}
				
				lexer.Position = start;
			}
            
            if (longestTree == null)
            {
			    state.RuntimeState.Runtime.ParseTrace.No(this,
			        lexer.CurrentSource(), "No alts matched");
			    
			    return ParseTree.No;
			}
			
			lexer.Position = longestTreeEnd;
			
			state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
			return longestTree;
        }
        
        public override Block CompileNewState(Runtime runtime, StateForCompiler state)
        {
            ParserBlock block = new ParserBlock(runtime);
            
            block.Comment("begin longest alt -----------");
            
            block.Enter(this, "longest alt");
            
            block.BeginScope();
            
            BlockLocal start = block.SavePosition();
            
            BlockLocal longestTree = new BlockLocal(typeof(ParseTree));
            block.DeclareLocal(longestTree);
            block.LoadNull();
            block.StoreLocal(longestTree);
            
            BlockLocal longestTreeEnd = new BlockLocal(typeof(int));
            block.DeclareLocal(longestTreeEnd);
            block.Load(-1);
            block.StoreLocal(longestTreeEnd);
            
            foreach (IParseable alt in alts)
            {
                block.Emit(alt.Compile(runtime, state));
                
                BlockLabel no = new BlockLabel("no");
                block.Dup();
                block.BranchIfNo(no);
                
                block.LoadLexer();
                block.GetProperty(typeof(Lexer).GetProperty("Position"));
                block.LoadLocal(longestTreeEnd);
                
                block.BranchIfLessOrEqual(no);
                
                block.StoreLocal(longestTree);

                block.LoadLexer();
                block.GetProperty(typeof(Lexer).GetProperty("Position"));
                block.StoreLocal(longestTreeEnd);
                
                BlockLabel continueLabel = new BlockLabel("continue");
                block.Branch(continueLabel);
                
                block.MarkLabel(no);
                block.Pop();
                
                block.MarkLabel(continueLabel);
                
                block.RestorePosition(start);
            }
            
            BlockLabel yes = new BlockLabel("null");
            block.LoadLocal(longestTree);
            block.BranchIfNotNull(yes);
            
            block.Comment("no");
            
            block.No(this, start, "No alts matched");
            
            block.LoadNo();
            
            BlockLabel returnLabel = new BlockLabel("return");
            block.Branch(returnLabel);
            
            block.Comment("yes");
            
            block.MarkLabel(yes);
            
            block.Yes(this, start);
            
            block.RestorePosition(longestTreeEnd);
            
            block.LoadLocal(longestTree);
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end longest alt -----------");
            
            return block;
        }
    
        public List<IParseable> Alts
        {
            get
            {
                return alts;
            }
        }
    }
}