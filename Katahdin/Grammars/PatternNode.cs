using System;
using System.Collections.Generic;

using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class PatternNode : ParseGraphNode
    {
        private Pattern pattern;
        private bool simple;
        
        public PatternNode(Source source, Pattern pattern, bool simple)
            : base(source)
        {
            if (pattern == null)
                throw new ArgumentNullException();
            
            this.pattern = pattern;
            this.simple = simple;
        }
        
        public override bool Update(Pattern updated)
        {
            return pattern == updated;
        }
        
        public override bool GetShouldRemember()
        {
            return pattern.ShouldRemember;
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            if (!simple && (state.LeftHandSide != null))
            {
                Type leftType = state.LeftHandSide.Value.GetType();
                Type patternType = pattern.Type;
                
                if ((leftType == patternType) || leftType.IsSubclassOf(patternType))
                {
                    state.RuntimeState.Runtime.ParseTrace.Single("left hand side from recursion");
                    
                    lexer.Position = state.LeftHandSideEndPos;
                    return state.LeftHandSide;
                }
            }
            
            return pattern.Parse(lexer, state);
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock(runtime);
            
            block.Comment("start of pattern --------------------");
            
            if (simple)
            {
                block.Emit(pattern.CompileCall(runtime, state));
            }
            else
            {
                BlockLabel returnLabel = new BlockLabel("return");
            
                block.BeginScope();
            
                BlockLocal patternLocal = new BlockLocal(typeof(Pattern));
                block.DeclareLocal(patternLocal);
                block.Load(pattern);
                block.StoreLocal(patternLocal);
            
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
            
                BlockLabel parseNew = new BlockLabel("parseNew");
                block.BranchIfNull(parseNew);
            
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
                block.GetProperty(typeof(ParseTree).GetProperty("Value"));
                block.Call(typeof(object).GetMethod("GetType"));
                BlockLocal leftType = new BlockLocal(typeof(Type));
                block.DeclareLocal(leftType);
                block.Dup();
                block.StoreLocal(leftType);
            
                block.LoadLocal(patternLocal);
                block.GetProperty(typeof(Pattern).GetProperty("Type"));
                BlockLocal patternType = new BlockLocal(typeof(Type));
                block.DeclareLocal(patternType);
                block.Dup();
                block.StoreLocal(patternType);
            
                BlockLabel recurse = new BlockLabel("recurse");
                block.BranchIfEqual(recurse);
            
                block.LoadLocal(leftType);
                block.LoadLocal(patternType);
                block.Call(typeof(Type).GetMethod("IsSubclassOf"));
            
                block.BranchIfTrue(recurse);
            
                block.Branch(parseNew);
            
                block.MarkLabel(recurse);
            
                block.Single("left hand side from recursion");
            
                block.LoadLexer();
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("LeftHandSideEndPos"));
                block.SetProperty(typeof(Lexer).GetProperty("Position"));
                
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
                
                block.Branch(returnLabel);
                
                block.MarkLabel(parseNew);
                
                block.Emit(pattern.CompileCall(runtime, state));
                
                block.MarkLabel(returnLabel);
            
                block.EndScope();
            }
            
            block.Comment("end of pattern ---------------------");
            
            return block;
        }
        
        public override Precedence Precedence
        {
            get
            {
                return pattern.Precedence;
            }
        }
        
        public Pattern Pattern
        {
            get
            {
                return pattern;
            }
        }
    }
}
