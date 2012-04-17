using System;
using System.Collections.Generic;

using Katahdin.Collections;
using Katahdin.Grammars;
using Katahdin.Grammars.Precedences;

namespace Katahdin
{
    public class ParserState
    {
        private RuntimeState runtimeState;
        
        private bool buildTextNodes = false;
        private RecursionBehaviour recursionBehaviour = RecursionBehaviour.Recursive;
        
        private Precedence currentPrecedence;
        private bool precedenceCanEqualCurrent;
        
        private Dictionary<ParserMemoryKey, ParserMemoryEntry> memory
                = new Dictionary<ParserMemoryKey, ParserMemoryEntry>(1000);
        
        // TODO - should this be ConcretePattern
        private Multiset<Pattern> excluded = new Multiset<Pattern>();
        
        /*
            leftHandSide is the object that we previously parsed and will use
            when satisfying the left hand side of the rule that we recurse
            back up into.
            
            Set by ConcretePattern, used by PatternNode, cleared by SeqNode.
        */
        
        private ParseTree leftHandSide;
        
        /*
            leftHandSideEndPos is the position where the leftHandSide
            finished parsing, and so to skip to when using leftHandSide.
            
            Set by ConcretePattern, used by PatternNode.
        */
        
        private int leftHandSideEndPos;
        
        /*
            leftRecursiveAlts is alt of possible rules to left recurse up
            into. It is all the patterns in the current precedence group that
            the rule you started in was a member of. If you go to the Add rule
            from an Additive abstract rule, the leftRecursiveAlts is
            everything in Additive, so Add and Sub.
        */
        
        private ParseGraphNode leftRecursiveAlts;
        
        /*
            skipMemoryForLeftRecursion tells ConcretePattern not to look in
            memory when performing a left recursion. If you were in Add,
            left recursed and tried to match Add you would of course find the
            first Add already there. Set and cleared in ConcretePattern.
        */
        
        private bool skipMemoryForLeftRecursion;
        
        /*
            recursionExclude is the pattern currently being matched when
            excluded to prevent recursion. Stored here and not just in
            excluded because when we right recurse it is removed.
        */
        
        private ConcretePattern recursionExclude;
        
        public ParserState(RuntimeState runtimeState)
        {
            this.runtimeState = runtimeState;
        }
        
        public RuntimeState RuntimeState
        {
            get
            {
                return runtimeState;
            }
        }
        
        public bool BuildTextNodes
        {
            get
            {
                return buildTextNodes;
            }
            
            set
            {
                buildTextNodes = value;
            }
        }

        public RecursionBehaviour RecursionBehaviour
        {
            get
            {
                return recursionBehaviour;
            }

            set
            {
                recursionBehaviour = value;
            }
        }
        
        public Precedence CurrentPrecedence
        {
            get
            {
                return currentPrecedence;
            }
            
            set
            {
                currentPrecedence = value;
            }
        }
        
        public bool PrecedenceCanEqualCurrent
        {
            get
            {
                return precedenceCanEqualCurrent;
            }
            
            set
            {
                precedenceCanEqualCurrent = value;
            }
        }
        
        public Dictionary<ParserMemoryKey, ParserMemoryEntry> Memory
        {
            get
            {
                return memory;
            }
        }
        
        public Multiset<Pattern> Excluded
        {
            get
            {
                return excluded;
            }
        }
        
        public ParseTree LeftHandSide
        {
            get
            {
                return leftHandSide;
            }
            
            set
            {
                leftHandSide = value;
            }
        }
        
        public int LeftHandSideEndPos
        {
            get
            {
                return leftHandSideEndPos;
            }
            
            set
            {
                leftHandSideEndPos = value;
            }
        }
        
        public ParseGraphNode LeftRecursiveAlts
        {
            get
            {
                return leftRecursiveAlts;
            }
            
            set
            {
                leftRecursiveAlts = value;
            }
        }
        
        public bool SkipMemoryForLeftRecursion
        {
            get
            {
                return skipMemoryForLeftRecursion;
            }
            
            set
            {
                skipMemoryForLeftRecursion = value;
            }
        }
        
        public ConcretePattern RecursionExclude
        {
            get
            {
                return recursionExclude;
            }
            
            set
            {
                recursionExclude = value;
            }
        }
    }
}
