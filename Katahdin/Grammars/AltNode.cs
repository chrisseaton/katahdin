using System;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Grammars.Alts;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class AltNode : ParseGraphNode
    {
        private List<ParseGraphNode> alts;
        private bool simple;
        
        private IParseable implementation;
        
        public AltNode(Source source, List<ParseGraphNode> alts, bool simple)
            : base(source)
        {
            if (alts.Count == 1)
                throw new ArgumentException("Must be more than one choice in an alt");
            
            this.alts = alts;
            this.simple = simple;
            
            Implement();
        }
        
        public override bool Update(Pattern updated)
        {
            bool updates = false;
            
            foreach (ParseGraphNode alt in alts)
            {
                if (alt.Update(updated))
                    updates = true;
            }
            
            if (updates)
                Implement();
            
            return updates;
        }
        
        public override void CollectFields(OrderedSet<string> fields)
        {
            foreach (ParseGraphNode alt in alts)
                alt.CollectFields(fields);
        }
        
        public override bool GetShouldRemember()
        {
            foreach (ParseGraphNode alt in alts)
                if (!alt.GetShouldRemember())
                    return false;
            
            return true;
        }
        
        private void Implement()
        {
            implementation = Alt.Make(Source, alts, simple);
            Updated();
        }

        public override ParseTree Parse(Lexer lexer, ParserState state)
        {
            return implementation.Parse(lexer, state);
        }
        
        public override Block CompileNewState(Runtime runtime, StateForCompiler state)
        {
            return implementation.Compile(runtime, state);
        }
        
        public List<ParseGraphNode> Alts
        {
            get
            {
                return alts;
            }
        }
        
        public IParseable Implementation
        {
            get
            {
                return implementation;
            }
        }
    }
}
