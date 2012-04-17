using System;
using System.Collections.Generic;

using Katahdin.Grammars.Precedences;

namespace Katahdin.Grammars.Alts
{
    public class PrecedenceAltGroup
    {
        private Precedence precedence;
        
        private List<IParseable> nodes;
        private IParseable parseGraph;
        
        private List<ParseGraphNode> leftRecursiveNodes;
        private ParseGraphNode leftRecursiveParseGraph;
        
        public PrecedenceAltGroup(Source source, Precedence precedence,
            List<IParseable> nodes)
        {
            this.nodes = nodes;
            this.precedence = precedence;
            
            if (nodes.Count == 1)
                parseGraph = nodes[0];
            else
                parseGraph = new LongestAlt(source, nodes);
                
            leftRecursiveNodes = new List<ParseGraphNode>();
            
            foreach (IParseable node in (IEnumerable<IParseable>) nodes)
            {
                PatternNode patternNode = node as PatternNode;
                
                if (patternNode != null)
                {
                    ConcretePattern pattern = patternNode.Pattern as ConcretePattern;
                    
                    if (pattern != null)
                    {
                        if (pattern.RecursionBehaviour == RecursionBehaviour.LeftRecursive)
                            leftRecursiveNodes.Add(new PatternNode(source, pattern, true));
                    }
                }
            }
            
            if (leftRecursiveNodes.Count == 0)
                leftRecursiveParseGraph = null;
            else if (leftRecursiveNodes.Count == 1)
                leftRecursiveParseGraph = leftRecursiveNodes[0];
            else
                leftRecursiveParseGraph = new AltNode(source, leftRecursiveNodes, true);
        }
        
        public Precedence Precedence
        {
            get
            {
                return precedence;
            }
        }
        
        public List<IParseable> Nodes
        {
            get
            {
                return nodes;
            }
        }
        
        public IParseable ParseGraph
        {
            get
            {
                return parseGraph;
            }
        }
        
        public List<ParseGraphNode> LeftRecursiveNodes
        {
            get
            {
                return leftRecursiveNodes;
            }
        }
        
        public ParseGraphNode LeftRecursiveParseGraph
        {
            get
            {
                return leftRecursiveParseGraph;
            }
        }
    }
}
