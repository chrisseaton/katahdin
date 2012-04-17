using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.Collections;
using Katahdin.CodeTree;
using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class ConcretePattern : Pattern
    {
        private RecursionBehaviour recursionBehaviour;
        private PatternNode recurseNode;
        
        public ConcretePattern(Source source, string name,
            ParseGraphNode parseGraph)
                : base(source, name)
        {
            SetParseGraph(parseGraph);
            
            recursionBehaviour = RecursionBehaviour.Recursive;
            
            OptionsNode options = ParseGraph as OptionsNode;
            
            while (options != null)
            {
                if (options.RecursionBehaviour.HasValue)
                {
                    recursionBehaviour = options.RecursionBehaviour.Value;
                    break;
                }
                
                options = options.Body as OptionsNode;
            }
            
            recurseNode = new PatternNode(source, this, true);
        }
        
        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            int start = lexer.Position;
            
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), "Pattern " + Type.Name);
            
            /*state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.CurrentSource(), "Excluded:");
            
            foreach (Pattern excluded in state.Excluded)
                state.RuntimeState.Runtime.ParseTrace.Single(excluded.Type.Name);
            
            state.RuntimeState.Runtime.ParseTrace.Leave(this);*/
            
            if (state.Excluded.Contains(this))
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.CurrentSource(), "Excluded");
                return ParseTree.No;
            }
            
            ParserMemoryKey key = new ParserMemoryKey(this, start);
            
            bool oldSkipMemoryForLeftRecursion = state.SkipMemoryForLeftRecursion;
            
            if (state.SkipMemoryForLeftRecursion)
            {
                state.SkipMemoryForLeftRecursion = false;
            }
            else
            {
                ParserMemoryEntry entry;
                
                if (state.Memory.TryGetValue(key, out entry))
                {
                    state.RuntimeState.Runtime.ParseTrace.LinkNext(entry.Tag);

                    if (entry.Tree == ParseTree.No)
                    {
                        state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start), "From memory");
                    }
                    else
                    {
                        state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start), "From memory");
                        lexer.Position = entry.End;
                    }

                    return entry.Tree;
                }
            }
            
            ConcretePattern oldRecursionExclude = state.RecursionExclude;
            
            if (RecursionBehaviour != RecursionBehaviour.Recursive)
            {
                if (state.RecursionExclude != null)
                    state.Excluded.Remove(state.RecursionExclude);
                
                state.RecursionExclude = this;
                
                state.Excluded.Add(this);
            }
            
            RecursionBehaviour oldRecursionBehaviour = state.RecursionBehaviour;
            state.RecursionBehaviour = RecursionBehaviour;
            
            Precedence oldCurrentPrecedence = state.CurrentPrecedence;
            
            if (Precedence.Overwrites(state.CurrentPrecedence))
                state.CurrentPrecedence = Precedence;

            bool oldPrecedenceCanEqualCurrent = state.PrecedenceCanEqualCurrent;
            state.PrecedenceCanEqualCurrent = RecursionBehaviour == RecursionBehaviour.Recursive;
            
            ParseTree tree = ParseGraph.Parse(lexer, state);
            
            state.CurrentPrecedence = oldCurrentPrecedence;
            state.PrecedenceCanEqualCurrent = oldPrecedenceCanEqualCurrent;
            state.RecursionBehaviour = oldRecursionBehaviour;
            state.RecursionExclude = oldRecursionExclude;
            state.SkipMemoryForLeftRecursion = oldSkipMemoryForLeftRecursion;
            
            if (RecursionBehaviour != RecursionBehaviour.Recursive)
            {
                if (oldRecursionExclude != null)
                    state.Excluded.Add(oldRecursionExclude);
                
                state.Excluded.Remove(this);
            }
            
            if (tree != ParseTree.No)
            {
                lexer.Whitespace(state.RuntimeState);
                tree = Instantiate(lexer, state, start, tree);
            }
            
            object nextTag = new object();
        
            state.RuntimeState.Runtime.ParseTrace.TagNext(nextTag);
            
            if (tree == ParseTree.No)
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start));
            }
            else
            {
                state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
                
                if (RecursionBehaviour == RecursionBehaviour.LeftRecursive)
                    tree = LeftRecurse(lexer, state, start, tree);
            }
            
            // TODO - can remove some other code if not remembering
            
            if (ShouldRemember)
                state.Memory[key] = new ParserMemoryEntry(tree, lexer.Position, nextTag);
            
            return tree;
        }
        
        public ParseTree Instantiate(Lexer lexer, ParserState state,
            int start, ParseTree tree)
        {
            Type[] parameterTypes = new Type[1 + Fields.Count];
            
            parameterTypes[0] = typeof(Source);
        
            for (int n = 1; n < parameterTypes.Length; n++)
                parameterTypes[n] = typeof(object);

            ConstructorInfo constructor = Type.GetConstructor(parameterTypes);

            if (constructor == null)
                throw new Exception("Couldn't find constructor for "
                    + Type.FullName);
        
            object[] parameterValues = new object[parameterTypes.Length];
        
            parameterValues[0] = lexer.SourceFrom(start);
    
            for (int n = 0; n < Fields.Count; n++)
            {
                object fieldValue = null;
                tree.Fields.TryGetValue(Fields[n], out fieldValue);
                    
                parameterValues[n + 1] = fieldValue;
            }
        
            RuntimeObject instance =
                (RuntimeObject) constructor.Invoke(parameterValues);
        
            object[] parsedParameters = {};
            
            object parsedMethod = MemberNode.GetMember(instance, "Parsed",
                parsedParameters, false);
            
            if (parsedMethod != null)
            {
                CallNode.Call(state.RuntimeState, parsedMethod,
                    parsedParameters);
            }
            
            return new ParseTree(instance);
        }
        
        public ParseTree LeftRecurse(Lexer lexer, ParserState state,
            int start, ParseTree leftHandSide)
        {
            state.RuntimeState.Runtime.ParseTrace.Enter(this, lexer.SourceFrom(start), "Left recursion");
            
            int finished = lexer.Position;
            
            state.LeftHandSide = leftHandSide;
            state.LeftHandSideEndPos = finished;
            state.SkipMemoryForLeftRecursion = true;
            
            lexer.Position = start;
            
            ParseGraphNode node;
            
            if (state.LeftRecursiveAlts != null)
                node = state.LeftRecursiveAlts;
            else
                node = recurseNode;
            
            ParseTree recursedTree = node.Parse(lexer, state);
            
            state.SkipMemoryForLeftRecursion = false;
            state.LeftHandSide = null;

            if (recursedTree == ParseTree.No)
            {
                state.RuntimeState.Runtime.ParseTrace.No(this, lexer.SourceFrom(start));
                lexer.Position = finished;
                
                return leftHandSide;
            }
            
            state.RuntimeState.Runtime.ParseTrace.Yes(this, lexer.SourceFrom(start));
            
            return recursedTree;
        }
        
        public override Block CompileNew(Runtime runtime, StateForCompiler state)
        {
            ParserBlock block = new ParserBlock(runtime);
            
            block.Comment("start of concrete pattern -------------");
            
            BlockLabel returnLabel = new BlockLabel("return");
            
            block.BeginScope();
            
            block.Enter(this, Type.Name);
            
            BlockLocal start = block.SavePosition();
            
            BlockLabel notExcluded = new BlockLabel("notExcluded");
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
            block.Load(this);
            block.Call(typeof(Multiset<Pattern>).GetMethod("Contains"));
            block.BranchIfFalse(notExcluded);
            
            block.No(this, "Excluded");
            
            block.LoadNo();
            block.Branch(returnLabel);
            
            block.MarkLabel(notExcluded);
            
            BlockLocal key = new BlockLocal(typeof(ParserMemoryKey));
            block.DeclareLocal(key);
            block.Load(this);
            block.LoadLocal(start);
            block.New(typeof(ParserMemoryKey).GetConstructor(new Type[]{typeof(ConcretePattern), typeof(int)}));
            block.StoreLocal(key);
            
            BlockLocal oldSkipMemoryForLeftRecursion = new BlockLocal(typeof(bool));
            block.DeclareLocal(oldSkipMemoryForLeftRecursion);
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("SkipMemoryForLeftRecursion"));
            block.Dup();
            block.StoreLocal(oldSkipMemoryForLeftRecursion);
            
            BlockLabel checkMemory = new BlockLabel("checkMemory");
            block.BranchIfFalse(checkMemory);
            
            block.LoadState();
            block.Load(false);
            block.SetProperty(typeof(ParserState).GetProperty("SkipMemoryForLeftRecursion"));
            
            BlockLabel setRecursion = new BlockLabel("setRecursion");
            block.Branch(setRecursion);
            
            block.MarkLabel(checkMemory);
            
            block.BeginScope();
            
            BlockLocal entry = new BlockLocal(typeof(ParserMemoryEntry));
            block.DeclareLocal(entry);
            
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("Memory"));
            block.LoadLocal(key);
            block.LoadLocalAddress(entry);
            block.Call(typeof(Dictionary<ParserMemoryKey, ParserMemoryEntry>).GetMethod("TryGetValue"));
            block.BranchIfFalse(setRecursion);
            
            // todo link next
            
            BlockLabel memoryYes = new BlockLabel("memoryYes");
            block.LoadLocal(entry);
            block.GetProperty(typeof(ParserMemoryEntry).GetProperty("Tree"));
            
            BlockLabel returnTree = new BlockLabel("returnTree");
            block.BranchIfNotNo(memoryYes);
            
            block.No(this, start, "from memory");
            
            block.Branch(returnTree);
            
            block.MarkLabel(memoryYes);
            
            block.Yes(this, start, "from memory");
            
            block.LoadLexer();
            block.LoadLocal(entry);
            block.GetProperty(typeof(ParserMemoryEntry).GetProperty("End"));
            block.SetProperty(typeof(Lexer).GetProperty("Position"));
            
            block.MarkLabel(returnTree);
            
            block.LoadLocal(entry);
            block.GetProperty(typeof(ParserMemoryEntry).GetProperty("Tree"));
            
            block.Branch(returnLabel);
            
            block.MarkLabel(setRecursion);
            
            BlockLocal oldRecursionExclude = new BlockLocal(typeof(ConcretePattern));
            block.DeclareLocal(oldRecursionExclude);
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("RecursionExclude"));
            block.StoreLocal(oldRecursionExclude);
            
            if (RecursionBehaviour != RecursionBehaviour.Recursive)
            {
                BlockLabel recursionExcludeSaveNull = new BlockLabel("recursionExcludeSaveNull");
                block.LoadLocal(oldRecursionExclude);
                block.BranchIfNull(recursionExcludeSaveNull);
                
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
                block.LoadLocal(oldRecursionExclude);
                block.Call(typeof(Multiset<Pattern>).GetMethod("Remove", new Type[]{typeof(Pattern)}));
                
                block.MarkLabel(recursionExcludeSaveNull);
                
                block.LoadState();
                block.Load(this);
                block.SetProperty(typeof(ParserState).GetProperty("RecursionExclude"));
                
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
                block.Load(this);
                block.Call(typeof(Multiset<Pattern>).GetMethod("Add", new Type[]{typeof(Pattern)}));
            }
            
            BlockLocal oldRecursionBehaviour = new BlockLocal(typeof(RecursionBehaviour));
            block.DeclareLocal(oldRecursionBehaviour);
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("RecursionBehaviour"));
            block.StoreLocal(oldRecursionBehaviour);
            
            block.LoadState();
            block.Load(Convert.ToInt32(RecursionBehaviour));
            block.SetProperty(typeof(ParserState).GetProperty("RecursionBehaviour"));
            
            RecursionBehaviour oldRecursionBehaviourState = state.RecursionBehaviour;
            state.RecursionBehaviour = RecursionBehaviour;
            
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
            
            BlockLocal oldPrecedenceCanEqualCurrent = new BlockLocal(typeof(bool));
            block.DeclareLocal(oldPrecedenceCanEqualCurrent);
            block.LoadState();
            block.GetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
            block.StoreLocal(oldPrecedenceCanEqualCurrent);
            
            block.LoadState();
            block.Load(RecursionBehaviour == RecursionBehaviour.Recursive);
            block.SetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
            
            BlockLocal tree = new BlockLocal(typeof(ParseTree));
            block.DeclareLocal(tree);
            block.Emit(ParseGraph.Compile(runtime, state));
            block.StoreLocal(tree);
            
            block.LoadState();
            block.LoadLocal(oldCurrentPrecedence);
            block.SetProperty(typeof(ParserState).GetProperty("CurrentPrecedence"));
            
            block.LoadState();
            block.LoadLocal(oldPrecedenceCanEqualCurrent);
            block.SetProperty(typeof(ParserState).GetProperty("PrecedenceCanEqualCurrent"));
            
            block.LoadState();
            block.LoadLocal(oldRecursionBehaviour);
            block.SetProperty(typeof(ParserState).GetProperty("RecursionBehaviour"));
            
            state.RecursionBehaviour = oldRecursionBehaviourState;
            
            block.LoadState();
            block.LoadLocal(oldRecursionExclude);
            block.SetProperty(typeof(ParserState).GetProperty("RecursionExclude"));
            
            block.LoadState();
            block.LoadLocal(oldSkipMemoryForLeftRecursion);
            block.SetProperty(typeof(ParserState).GetProperty("SkipMemoryForLeftRecursion"));
            
            if (RecursionBehaviour != RecursionBehaviour.Recursive)
            {
                BlockLabel recursionExcludeRestoreNull = new BlockLabel("recursionExcludeRestoreNull");
                block.LoadLocal(oldRecursionExclude);
                block.BranchIfNull(recursionExcludeRestoreNull);
                
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
                block.LoadLocal(oldRecursionExclude);
                block.Call(typeof(Multiset<Pattern>).GetMethod("Add", new Type[]{typeof(Pattern)}));
                
                block.MarkLabel(recursionExcludeRestoreNull);
                
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("Excluded"));
                block.Load(this);
                block.Call(typeof(Multiset<Pattern>).GetMethod("Remove", new Type[]{typeof(Pattern)}));
            }
            
            BlockLabel no = new BlockLabel("yes1");
            block.LoadLocal(tree);
            block.BranchIfNo(no);
            
            block.Whitespace(runtime, state);
            
            CompileInstantiate(block, start, tree);
            
            block.MarkLabel(no);
            
            BlockLocal nextTag = new BlockLocal(typeof(object));
            block.DeclareLocal(nextTag);
            block.New(typeof(object));
            block.StoreLocal(nextTag);
            
            // todo tag next
            
            BlockLabel yes = new BlockLabel("yes");
            block.LoadLocal(tree);
            block.BranchIfNotNo(yes);
            
            block.No(this, start, "a");
            
            BlockLabel finish = new BlockLabel("finish");
            block.Branch(finish);
            
            block.MarkLabel(yes);
            
            block.Yes(this, start);
            
            if (RecursionBehaviour == RecursionBehaviour.LeftRecursive)
                CompileLeftRecurse(runtime, state, block, start, tree);
            
            block.MarkLabel(finish);
            
            if (ShouldRemember)
            {
                // TODO - can remove some other code if not remembering
                
                block.LoadState();
                block.GetProperty(typeof(ParserState).GetProperty("Memory"));
                block.LoadLocal(key);
            
                block.LoadLocal(tree);
                block.LoadLexer();
                block.GetProperty(typeof(Lexer).GetProperty("Position"));
                block.LoadLocal(nextTag);
                block.New(typeof(ParserMemoryEntry).GetConstructor(new Type[]{typeof(ParseTree), typeof(int), typeof(object)}));
            
                block.SetProperty(typeof(Dictionary<ParserMemoryKey, ParserMemoryEntry>).GetProperty("Item"));
            }
            
            block.LoadLocal(tree);
             
            block.EndScope();
            
            block.MarkLabel(returnLabel);
            
            block.Comment("end of concrete pattern -------------");
            
            return block;
        }
        
        private void CompileInstantiate(ParserBlock block, BlockLocal start, BlockLocal tree)
        {
            block.BeginScope();
            
            Type[] parameterTypes = new Type[1 + Fields.Count];
            
            parameterTypes[0] = typeof(Source);
        
            for (int n = 1; n < parameterTypes.Length; n++)
                parameterTypes[n] = typeof(object);

            ConstructorInfo constructor = Type.GetConstructor(parameterTypes);

            if (constructor == null)
                throw new Exception("Couldn't find constructor for " + Type.FullName);
            
            block.LoadLexer();
            block.LoadLocal(start);
            block.Call(typeof(Lexer).GetMethod("SourceFrom"));
            
            BlockLocal fields = new BlockLocal(typeof(Dictionary<string, object>));
            block.DeclareLocal(fields);
            block.LoadLocal(tree);
            block.GetProperty(typeof(ParseTree).GetProperty("Fields"));
            block.StoreLocal(fields);
                        
            for (int n = 0; n < Fields.Count; n++)
            {
                block.BeginScope();
                
                BlockLocal fieldValue = new BlockLocal(typeof(object));
                block.DeclareLocal(fieldValue);
                
                block.LoadLocal(fields);
                block.Load(Fields[n]);
                block.LoadLocalAddress(fieldValue);
                block.Call(fields.Type.GetMethod("TryGetValue"));
                block.Pop();
                
                block.LoadLocal(fieldValue);
                
                block.EndScope();
            }
            
            block.New(constructor);
            
            BlockLocal instance = new BlockLocal(typeof(RuntimeObject));
            block.DeclareLocal(instance);
            block.StoreLocal(instance);
            
            // Call Parsed
            
            block.Load(0);
            block.NewArray(typeof(object));
            
            block.BeginScope();
            
            BlockLocal parsedParameters = new BlockLocal(typeof(object[]));
            block.DeclareLocal(parsedParameters);
            block.StoreLocal(parsedParameters);
            
            block.LoadLocal(instance);
            block.Load("Parsed");
            block.LoadLocal(parsedParameters);
            block.Load(false);
            
            block.Call(typeof(MemberNode).GetMethod("GetMember", new Type[]{
                typeof(object), typeof(string), typeof(object[]),
                typeof(bool)}));
            
            BlockLabel noParsedMethod = new BlockLabel("noParsedMethod");
            
            block.Dup();
            block.BranchIfNull(noParsedMethod);
            
            BlockLocal parsedMethod = new BlockLocal(typeof(object));
            block.DeclareLocal(parsedMethod);
            block.StoreLocal(parsedMethod);
            
            block.LoadRuntimeState();
            block.LoadLocal(parsedMethod);
            block.LoadLocal(parsedParameters);
            block.Call(typeof(CallNode).GetMethod("Call", new Type[]{
                typeof(RuntimeState), typeof(object), typeof(object[])}));
            block.Pop();
            
            BlockLabel finishedParsedCall = new BlockLabel("finishedParsedCall");
            block.Branch(finishedParsedCall);
            
            block.MarkLabel(noParsedMethod);
            
            block.Pop();
            
            block.MarkLabel(finishedParsedCall);
            
            block.EndScope();
            
            block.LoadLocal(instance);
            block.New(typeof(ParseTree).GetConstructor(new Type[]{typeof(object)}));
            
            block.StoreLocal(tree);
            
            block.EndScope();
        }

        private void CompileLeftRecurse(Runtime runtime,
            StateForCompiler state, ParserBlock block, BlockLocal start,
            BlockLocal tree)
        {
            block.BeginScope();
            
            block.Enter(this, "left recurse");
            
            BlockLocal finished = block.SavePosition();
            
            block.LoadState();
            block.LoadLocal(tree);
            block.SetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
            
            block.LoadState();
            block.LoadLocal(finished);
            block.SetProperty(typeof(ParserState).GetProperty("LeftHandSideEndPos"));
            
            block.LoadState();
            block.Load(true);
            block.SetProperty(typeof(ParserState).GetProperty("SkipMemoryForLeftRecursion"));
            
            block.LoadLexer();
            block.LoadLocal(start);
            block.SetProperty(typeof(Lexer).GetProperty("Position"));
            
            ParseGraphNode node;
            
            if (state.LeftRecursiveAlts != null)
                node = state.LeftRecursiveAlts;
            else
                node = recurseNode;
            
            block.Emit(node.Compile(runtime, state));
            
            block.LoadState();
            block.Load(false);
            block.SetProperty(typeof(ParserState).GetProperty("SkipMemoryForLeftRecursion"));
            
            block.LoadState();
            block.LoadNull();
            block.SetProperty(typeof(ParserState).GetProperty("LeftHandSide"));
            
            BlockLabel yes = new BlockLabel("yes");
            block.Dup();
            block.BranchIfNotNo(yes);
            
            block.No(this, start);
            
            block.Pop();
            block.RestorePosition(finished);
            
            BlockLabel returnLabel = new BlockLabel("return");
            block.Branch(returnLabel);
            
            block.MarkLabel(yes);
            
            block.Yes(this, start);
            
            block.StoreLocal(tree);
            
            block.MarkLabel(returnLabel);
            
            block.EndScope();
        }
        
        public RecursionBehaviour RecursionBehaviour
        {
            get
            {
                return recursionBehaviour;
            }
        }
    }
}
