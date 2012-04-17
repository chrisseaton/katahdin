using System;

using Katahdin.Grammars;

namespace Katahdin
{
    public class StateForCompiler
    {
        private bool buildTextNodes;
        private RecursionBehaviour recursionBehaviour;
        private ParseGraphNode leftRecursiveAlts;
        private Pattern whitespace;
        
        public StateForCompiler Copy()
        {
            StateForCompiler copy = new StateForCompiler();
            copy.buildTextNodes = buildTextNodes;
            copy.recursionBehaviour = recursionBehaviour;
            copy.leftRecursiveAlts = leftRecursiveAlts;
            copy.whitespace = whitespace;
            
            return copy;
        }
        
        public override int GetHashCode()
        {
            return (buildTextNodes ? 1 : 0) ^ recursionBehaviour.GetHashCode()
                ^ (leftRecursiveAlts == null?
                    90463 : leftRecursiveAlts.GetHashCode())
                ^ (whitespace == null?
                    77403 : whitespace.GetHashCode());
        }
        
        public override bool Equals(object other)
        {
            StateForCompiler otherState = other as StateForCompiler;
            
            if (otherState == null)
                return false;
            
            return Equals(otherState);
        }
        
        public bool Equals(StateForCompiler other)
        {
            return (buildTextNodes == other.buildTextNodes)
                && (recursionBehaviour == other.recursionBehaviour)
                && (leftRecursiveAlts == other.leftRecursiveAlts)
                && (whitespace == other.whitespace);
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
        
        public Pattern Whitespace
        {
            get
            {
                return whitespace;
            }
            
            set
            {
                whitespace = value;
            }
        }
    }
}
