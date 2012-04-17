using System;
using System.Collections.Generic;

using Katahdin.Grammars.Precedences;
using Katahdin.Compiler;

namespace Katahdin.Grammars.Alts
{
    public abstract class Alt : IParseable, ISourced
    {
        private Source source;
        
        private Dictionary<StateForCompiler, Block> blockCache
            = new Dictionary<StateForCompiler, Block>();
    
        protected Alt(Source source)
        {
            this.source = source;
        }
        
        public static IParseable Make(Source source, List<ParseGraphNode> nodes, bool simple)
        {
            if (nodes.Count == 0)
                throw new Exception();
            
            if (simple)
            {
                List<IParseable> abstracted = new List<IParseable>(nodes.Count);
                
                foreach (IParseable node in nodes)
                    abstracted.Add(node);
                
                return new LongestAlt(source, abstracted);
            }
            
            Group group = null;
            
            foreach (ParseGraphNode node in nodes)
            {
                if (node.Precedence != null)
                {
                    if (node.Precedence.Group != null)
                    {
                        group = node.Precedence.Group;
                        break;
                    }
                }
            }
            
            foreach (ParseGraphNode node in nodes)
            {
                if (group == null)
                {
                    if (node.Precedence != null)
                    {
                        if (node.Precedence.Group != null)
                            throw new Exception(node.Precedence.Name);
                    }
                }
                else
                {
                    if ((node.Precedence == null) && (node.Precedence != null)
                        && (node.Precedence.Group != group))
                            throw new Exception(nodes[0].Precedence.Name);
                }
            }
            
            List<PrecedenceAltGroup> groups = new List<PrecedenceAltGroup>();
            
            if (group != null)
            {
                foreach (List<Precedence> entry in group.Order)
                {
                    List<IParseable> precedenceGroup = new List<IParseable>();
                
                    foreach (ParseGraphNode node in nodes)
                    {
                        if (entry.Contains(node.Precedence))
                            precedenceGroup.Add(node);
                    }
                
                    if (precedenceGroup.Count > 0)
                        groups.Add(new PrecedenceAltGroup(source, entry[0],
                            precedenceGroup));
                }
            }

            List<IParseable> nullGroup = new List<IParseable>();

            foreach (ParseGraphNode node in nodes)
            {
                if ((node.Precedence == null) || (group == null)
                    || (node.Precedence.Group != group))
                        nullGroup.Add(node);
            }

            if (nullGroup.Count > 0)
                groups.Add(new PrecedenceAltGroup(source, null, nullGroup));
            
            return new PrecedenceAlt(source, groups);
        }
    
        public abstract ParseTree Parse(Lexer lexer, ParserState state);
        
        public virtual Block Compile(Runtime runtime, StateForCompiler state)
        {
            Block block;
            
            if (!blockCache.TryGetValue(state, out block))
            {
                block = CompileNewState(runtime, state);
                blockCache[state] = block;
            }
            
            return block;
        }
        
        public abstract Block CompileNewState(Runtime runtime, StateForCompiler state);
        /*{
            Block block = new Block();
            
            block.Comment("begin alt wrapper -----------");
            
            //block.Print("using original parse method for " + GetType());
            
            block.Load(this);
            block.LoadArg(0);
            block.LoadArg(1);
            block.Call(GetType().GetMethod("Parse"));
            
            block.Comment("end alt wrapper -----------");
            
            return block;
        }*/
        
        public Source Source
        {
            get
            {
                return source;
            }
        }
    }
}
