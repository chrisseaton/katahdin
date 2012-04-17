using System;

using Katahdin.CodeTree;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class UserDefinedNode : ParseGraphNode
    {
        private Type type;
        
        public UserDefinedNode(Source source, Type type)
            : base(source)
        {
            this.type = type;
        }
        
        public override bool GetShouldRemember()
        {
            return false;
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            int start = lexer.Position;
            
            if (type == null)
                throw new Exception();
            
            if (state.RuntimeState.Runtime.TraceParser)
                state.RuntimeState.Runtime.ParseTrace.Enter(this,
                    lexer.CurrentSource(),
                    "User defined node " + TypeNames.GetName(type));
            
            object instance = NewNode.New(state.RuntimeState, type, null);
            object parseMethod = MemberNode.GetMember(instance, "Parse", true);
            object result = CallNode.Call(state.RuntimeState, parseMethod,
                new object[]{lexer});
            
            if (result == null)
                result = ParseTree.Yes;
            else if (result is bool)
            {
                if ((bool) result)
                    result = ParseTree.Yes;
                else
                    result = ParseTree.No;
            }
            
            if (state.RuntimeState.Runtime.TraceParser)
            {
                if (result == ParseTree.No)
                    state.RuntimeState.Runtime.ParseTrace.No(this,
                        lexer.SourceFrom(start));
                else
                    state.RuntimeState.Runtime.ParseTrace.Yes(this,
                        lexer.SourceFrom(start));
            }
            
            return (ParseTree) result;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Load(this);
            block.LoadLexer();
            block.LoadState();
            block.Call(typeof(UserDefinedNode).GetMethod("Parse"));
            
            return block;
        }
        
        public Type Type
        {
            get
            {
                return type;
            }
            
            set
            {
                type = value;
            }
        }
    }
}
