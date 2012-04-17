using System;
using System.Diagnostics;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class AbstractPattern : Pattern
    {
        private List<Pattern> altPatterns = new List<Pattern>();
        
        public AbstractPattern(Source source, string name)
            : base(source, name)
        {
            SetParseGraph(new FailNode(source, "No concrete patterns"));
        }
        
        public void AddAltPattern(Pattern pattern)
        {
            // FIXME - quick fix for keeping keywords from looking like ids to ExpressionStatement
            
            altPatterns.Insert(0, pattern);
            
            if (altPatterns.Count == 1)
            {
                SetParseGraph(new PatternNode(Source, pattern, false));
            }
            else
            {
                // Create a new parse graph as if it were an alt between the concrete patterns
            
                List<ParseGraphNode> alts = new List<ParseGraphNode>();
            
                foreach (Pattern alt in altPatterns)
                    alts.Add(new PatternNode(Source, alt, false));
            
                SetParseGraph(new AltNode(Source, alts, false));
            }
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), "Pattern " + Type.Name);
            
            int start = lexer.Position;
            
            if (state.Excluded.Contains(this))
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.CurrentSource(), "Excluded");
                return ParseTree.No;
            }
            
		    Precedence oldCurrentPrecedence = state.CurrentPrecedence;
		    
		    if (Precedence.Overwrites(state.CurrentPrecedence))
		        state.CurrentPrecedence = Precedence;

            ParseTree tree = ParseGraph.Parse(lexer, state);
            
            state.CurrentPrecedence = oldCurrentPrecedence;

            if (tree == ParseTree.No)
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start));
                return ParseTree.No;
            }

            state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
            
            return tree;
        }
        
        public override Block CompileNew(Runtime runtime, StateForCompiler state)
        {
            ParserBlock block = new ParserBlock(runtime);
            
            block.Comment("start of abstract pattern -------------");
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            block.Enter(this, Type.Name);
            
            block.BeginScope();
            
            BlockLocal start = block.SavePosition();
            
            BlockLabel notExcluded = new BlockLabel("notExcluded");
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
            block.Load(this);
            block.Call(typeof(Multiset<Pattern>).GetMethod("Contains"));
            block.BranchIfFalse(notExcluded);
            
            block.No(this);
            
            block.LoadNo();
            block.Branch(returnLabel);
            
            block.MarkLabel(notExcluded);
            
            BlockLocal oldCurrentPrecedence = new BlockLocal(typeof(Precedence));
            block.DeclareLocal(oldCurrentPrecedence);
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
            block.StoreLocal(oldCurrentPrecedence);
            
            BlockLabel doesntOverwrite = new BlockLabel("doesntOverwrite");
            block.Load(Precedence);
            block.LoadLocal(oldCurrentPrecedence);
            block.Call(typeof(Precedence).GetMethod("Overwrites"));
            block.BranchIfFalse(doesntOverwrite);
            
            block.LoadState();
            block.Load(Precedence);
            block.SetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
            
            block.MarkLabel(doesntOverwrite);
            
            block.Emit(ParseGraph.Compile(runtime, state));
            
            block.LoadState();
            block.LoadLocal(oldCurrentPrecedence);
            block.SetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
            
            block.Dup();
            
            BlockLabel yes = new BlockLabel("yes");
            block.BranchIfNotNo(yes);
            
            block.No(this, start);
            
            block.Branch(returnLabel);
            
            block.MarkLabel(yes);
            
            block.Yes(this, start);
            
            block.EndScope();
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end of abstract pattern -------------");
            
            return block;
        }
        
        public List<Pattern> AltPatterns
        {
            get
            {
                return altPatterns;
            }
        }
    }
}
