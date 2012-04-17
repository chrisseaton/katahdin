using System;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class LabelNode : ParseGraphNode
    {
        private string label;
        private ParseGraphNode body;
        
        public LabelNode(Source source, string label, ParseGraphNode body)
            : base(source)
        {
            this.label = label;
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
            fields.Add(label);
            body.CollectFields(fields);
        }
        
        public override bool GetShouldRemember()
        {
            return body.GetShouldRemember();
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            ParseTree bodyTree = body.Parse(lexer, state);
            
            if (bodyTree == ParseTree.No)
                return ParseTree.No;
            
            ParseTree labelledTree = new ParseTree();
            labelledTree = labelledTree.ExtendFields(bodyTree);
            labelledTree.Fields[label] = bodyTree.Value;
            
            return labelledTree;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            block.Comment("start of label --------------------");
            
            BlockLabel returnLabel = new BlockLabel("returnLabel");
            
            block.Emit(body.Compile(runtime, state));
            
            block.Dup();
            block.BranchIfNo(returnLabel);
            
            block.BeginScope();
            
            BlockLocal bodyTree = new BlockLocal(typeof(ParseTree));
            block.DeclareLocal(bodyTree);
            block.StoreLocal(bodyTree);
            
            block.NewParseTree();
            
            block.LoadLocal(bodyTree);
            block.Call(typeof(ParseTree).GetMethod("ExtendFields"));
            
            block.Dup();
            block.GetProperty(typeof(ParseTree).GetProperty("Fields"));
            
            block.Load(label);
            
            block.LoadLocal(bodyTree);
            block.GetProperty(typeof(ParseTree).GetProperty("Value"));
            
            block.SetProperty(typeof(Dictionary<string, object>).GetProperty("Item"));
            
            block.EndScope();
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end of label --------------------");
            
            return block;
        }
        
        public string Label
        {
            get
            {
                return label;
            }
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
