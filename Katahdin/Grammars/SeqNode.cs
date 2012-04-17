using System;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class SeqNode : ParseGraphNode
    {
        private List<ParseGraphNode> nodes;
        
        public SeqNode(Source source, List<ParseGraphNode> nodes)
            : base(source)
        {
            if (nodes.Count == 1)
                throw new ArgumentException("Must be more than one node in a sequence");
            
            foreach (ParseGraphNode node in nodes)
            {
                if (node is SeqNode)
                    throw new ArgumentException("A sequence should not contain other sequences");
            }
            
            this.nodes = nodes;
        }
        
        public override bool Update(Pattern updated)
        {
            bool updates = false;
            
            foreach (ParseGraphNode node in nodes)
            {
                if (node.Update(updated))
                    updates = true;
            }
            
            if (updates)
                Updated();
            
            return updates;
        }
        
        public override void CollectFields(OrderedSet<string> fields)
        {
            foreach (ParseGraphNode node in nodes)
                node.CollectFields(fields);
        }
        
        public override bool GetShouldRemember()
        {
            foreach (ParseGraphNode node in nodes)
                if (!node.GetShouldRemember())
                    return false;
            
            return true;
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            int start = lexer.Position;
            
            ParseTree oldLHS = state.LeftHandSide;
            bool oldPrecedenceCanEqualCurrent = state.PrecedenceCanEqualCurrent;
            ConcretePattern oldRecursionExclude = null;
            
            ParseTree tree = ParseTree.Yes;
            
            for (int n = 0; n < nodes.Count; n++)
            {
                ParseGraphNode node = nodes[n];
                
                ParseTree nodeTree = node.Parse(lexer, state);
                
                if (nodeTree == ParseTree.No)
                {
                    state.PrecedenceCanEqualCurrent = oldPrecedenceCanEqualCurrent;
                    state.LeftHandSide = oldLHS;
                    state.RecursionExclude = oldRecursionExclude;
                    
                    if (oldRecursionExclude != null)
                        state.Excluded.Add(oldRecursionExclude);
                    
                    lexer.Position = start;
                    return ParseTree.No;
                }
                
                tree = tree.Extend(nodeTree);
                
                if (n == 0)
                {
                    if (state.RecursionExclude != null)
                    {
                        oldRecursionExclude = state.RecursionExclude;
                        state.Excluded.Remove(state.RecursionExclude);
                        state.RecursionExclude = null;
                    }
                    
                    if (state.RecursionBehaviour == RecursionBehaviour.RightRecursive)
                        state.PrecedenceCanEqualCurrent = true;
                    else if (state.RecursionBehaviour == RecursionBehaviour.LeftRecursive)
                        state.LeftHandSide = null;
                }
            }
            
            state.PrecedenceCanEqualCurrent = oldPrecedenceCanEqualCurrent;
            state.LeftHandSide = oldLHS;
            state.RecursionExclude = oldRecursionExclude;
            
            if (oldRecursionExclude != null)
                state.Excluded.Add(oldRecursionExclude);
            
            return tree;
        }
        
        public override Block CompileNewState(Runtime runtime,
            StateForCompiler state)
        {
            ParserBlock block = new ParserBlock();
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            block.Comment("start of seq --------------------");
            
            block.BeginScope();
            
            // Save
            
            BlockLocal start = block.SavePosition();
            
            BlockLocal oldLeftHandSide = new BlockLocal(typeof(RuntimeObject));
            BlockLocal oldPrecedenceCanEqualCurrent = new BlockLocal(typeof(bool));
            BlockLocal oldRecursionExclude = new BlockLocal(typeof(Pattern));
            
            block.DeclareLocal(oldRecursionExclude);
            
            if (state.RecursionBehaviour == RecursionBehaviour.RightRecursive)
            {
                block.DeclareLocal(oldPrecedenceCanEqualCurrent);
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
                block.StoreLocal(oldPrecedenceCanEqualCurrent);
            }
            
            if (state.RecursionBehaviour == RecursionBehaviour.LeftRecursive)
            {
                block.DeclareLocal(oldLeftHandSide);
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
                block.StoreLocal(oldLeftHandSide);
            }
            
            block.LoadYes();
            
            for (int n = 0; n < nodes.Count; n++)
            {
                ParseGraphNode node = nodes[n];
                
                // Parse the node
                
                block.Emit(node.Compile(runtime, state));
                
                // If no
                
                BlockLabel yes = new BlockLabel("yes");
                block.Dup();    // dup the body tree
                block.BranchIfNotNo(yes);
                
                block.Pop();    // pop body tree
                block.Pop();    // pop new tree
                
                Restore(state, block, oldLeftHandSide,
                    oldPrecedenceCanEqualCurrent, oldRecursionExclude);
                
                block.RestorePosition(start);
                
                block.LoadNo();
                block.Branch(returnLabel);
                
                block.MarkLabel(yes);
                
                // Extend
                
                block.Call(typeof(ParseTree).GetMethod("Extend"));
                
                if (n == 0)
                {
                    block.LoadState();
                    block.GetProperty(typeof(ParserState).GetProperty("RecursionExclude"));
                    block.LoadNull();

                    BlockLabel recursionExcludeNull = new BlockLabel("recursionExcludeNull");
                    block.BranchIfEqual(recursionExcludeNull);

                    block.LoadState();
                    block.GetProperty(typeof(ParserState).GetProperty("RecursionExclude"));
                    block.StoreLocal(oldRecursionExclude);

                    block.LoadState();
                    block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
                    block.LoadState();
                    block.GetProperty(typeof(ParserState).GetProperty("RecursionExclude"));
                    block.Call(typeof(Multiset<Pattern>).GetMethod("Remove", new Type[]{typeof(Pattern)}));

                    block.LoadState();
                    block.LoadNull();
                    block.SetProperty(typeof(ParserState).GetProperty("RecursionExclude"));

                    block.MarkLabel(recursionExcludeNull);
                    
                    if (state.RecursionBehaviour == RecursionBehaviour.RightRecursive)
                    {
                        block.Comment("right recursion");

                        block.LoadState();
                        block.Load(1);
                        block.SetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
                    }
                    else if (state.RecursionBehaviour == RecursionBehaviour.LeftRecursive)
                    {
                        block.Comment("left recursion");
                        
                        block.LoadState();
                        block.LoadNull();
                        block.SetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
                    }
                }
            }
            
            Restore(state, block, oldLeftHandSide,
                oldPrecedenceCanEqualCurrent, oldRecursionExclude);
            
            block.EndScope();
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end of seq --------------------");
            
            return block;
        }
        
        private void Restore(StateForCompiler state, ParserBlock block,
            BlockLocal oldLeftHandSide,
            BlockLocal oldPrecedenceCanEqualCurrent,
            BlockLocal oldRecursionExclude)
        {
            block.LoadState();
            block.LoadLocal(oldRecursionExclude);
            block.SetProperty(typeof(ParserState).GetProperty("RecursionExclude"));
        
            BlockLabel nullLabel = new BlockLabel("null");
        
            block.LoadLocal(oldRecursionExclude);
            block.LoadNull();
            block.BranchIfEqual(nullLabel);
        
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
            block.LoadLocal(oldRecursionExclude);
            block.Call(typeof(Multiset<Pattern>).GetMethod("Add"));
        
            block.MarkLabel(nullLabel);
            
            if (state.RecursionBehaviour == RecursionBehaviour.RightRecursive)
            {
                block.LoadState();
                block.LoadLocal(oldPrecedenceCanEqualCurrent);
                block.SetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
            }
            else if (state.RecursionBehaviour == RecursionBehaviour.LeftRecursive)
            {
                block.LoadState();
                block.LoadLocal(oldLeftHandSide);
                block.SetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
            }
        }
        
        public List<ParseGraphNode> Nodes
        {
            get
            {
                return nodes;
            }
        }
    }
}
