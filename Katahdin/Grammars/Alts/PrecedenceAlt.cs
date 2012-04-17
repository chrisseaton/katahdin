using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;

namespace Katahdin.Grammars.Alts
{
    public class PrecedenceAlt : Alt
    {
        private List<PrecedenceAltGroup> groups;
        
        public PrecedenceAlt(Source source, List<PrecedenceAltGroup> groups)
            : base(source)
        {
            this.groups = groups;
        }
    
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(),
                "precedence alt, current = " + state.CurrentPrecedence
                + ", can equal current = " + state.PrecedenceCanEqualCurrent);

            int start = lexer.Position;
            
            ParseGraphNode oldLeftRecursiveAlts = state.LeftRecursiveAlts;
            
			foreach (PrecedenceAltGroup group in groups)
			{
			    if (state.CurrentPrecedence != null)
			    {
			        if (group.Precedence != null)
			        {
    			        if (state.CurrentPrecedence.Group == group.Precedence.Group)
    			        {
    			            if (group.Precedence.IsLowerThan(state.CurrentPrecedence,
    			                !state.PrecedenceCanEqualCurrent))
    			                    continue;
    			        }
		            }
			    }
			    
			    state.LeftRecursiveAlts = group.LeftRecursiveParseGraph;
			    
				ParseTree tree = group.ParseGraph.Parse(lexer, state);

				if (tree != ParseTree.No)
				{
			        state.LeftRecursiveAlts = oldLeftRecursiveAlts;
                    
				    state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
					return tree;
				}
			}
			
	        state.LeftRecursiveAlts = oldLeftRecursiveAlts;

			state.RuntimeState.Runtime.ParseTrace.No(this, lexer.CurrentSource(), "No alts matched");
            
			return ParseTree.No;
        }
        
        public override Block CompileNewState(Runtime runtime, StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("begin precedence alt -----------");
            
            // todo enter
            
            block.BeginScope();
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            BlockLocal oldLeftRecursiveAlts = new BlockLocal(typeof(IParseable));
            block.DeclareLocal(oldLeftRecursiveAlts);
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("LeftRecursiveAlts"));
            block.StoreLocal(oldLeftRecursiveAlts);
            
            ParseGraphNode oldLeftRecursiveAltsForCompiler = state.LeftRecursiveAlts;
            
			foreach (PrecedenceAltGroup group in groups)
			{
			    block.BeginScope();
			    
			    BlockLocal groupLocal = new BlockLocal(typeof(PrecedenceAltGroup));
			    block.DeclareLocal(groupLocal);
			    block.Load(group);
			    block.StoreLocal(groupLocal);
			    
                BlockLabel apply = new BlockLabel("apply");
                BlockLabel continueLabel = new BlockLabel("continue");

                BlockLocal currentPrecedence = new BlockLocal(typeof(Precedence));
                block.DeclareLocal(currentPrecedence);
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
                block.Dup();
                block.StoreLocal(currentPrecedence);
                block.BranchIfNull(apply);

                BlockLocal groupPrecedence = new BlockLocal(typeof(Precedence));
                block.DeclareLocal(groupPrecedence);
                block.LoadLocal(groupLocal);
                block.GetProperty(groupLocal.Type.GetProperty("Precedence"));
                block.Dup();
                block.StoreLocal(groupPrecedence);
                block.BranchIfNull(apply);

                block.LoadLocal(currentPrecedence);
                block.GetProperty(currentPrecedence.Type.GetProperty("Group"));
                block.LoadLocal(groupPrecedence);
                block.GetProperty(groupPrecedence.Type.GetProperty("Group"));
                block.BranchIfNotEqual(apply);

                block.LoadLocal(groupPrecedence);
                block.LoadLocal(currentPrecedence);
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
                block.LogicalNot();
                block.Call(groupPrecedence.Type.GetMethod("IsLowerThan"));
                block.BranchIfTrue(continueLabel);

                block.MarkLabel(apply);
                
                block.LoadState();
                block.LoadLocal(groupLocal);
                block.GetProperty(groupLocal.Type.GetProperty("LeftRecursiveParseGraph"));
                block.SetProperty(typeof(ParserState).GetProperty("LeftRecursiveAlts"));
                
                state.LeftRecursiveAlts = group.LeftRecursiveParseGraph;
                
                block.Emit(group.ParseGraph.Compile(runtime, state));
                
                block.Dup();
                
                BlockLabel no = new BlockLabel("no");
                block.BranchIfNo(no);
                
                block.LoadState();
                block.LoadLocal(oldLeftRecursiveAlts);
                block.SetProperty(typeof(ParserState).GetProperty("LeftRecursiveAlts"));
                
                // todo yes
                
                block.Branch(returnLabel);
                
                block.MarkLabel(no);
                
                block.Pop();
                
                block.MarkLabel(continueLabel);
                
                block.EndScope();
			}
			
            block.LoadState();
            block.LoadLocal(oldLeftRecursiveAlts);
            block.SetProperty(typeof(ParserState).GetProperty("LeftRecursiveAlts"));
            
            state.LeftRecursiveAlts = oldLeftRecursiveAltsForCompiler;
            
            block.EndScope();
            
            // todo no
            
            block.LoadNo();
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end precedence alt -----------");
            
            return block;
        }
        
        public List<PrecedenceAltGroup> Groups
        {
            get
            {
                return groups;
            }
        }
    }
}
