using System;
using System.Collections.Generic;

namespace Katahdin
{
    public class ParseTree
    {
        private static ParseTree yes = new ParseTree();
        private static ParseTree no = new ParseTree();
        
        private bool single;
        
        private List<object> nodes = null;
        private Dictionary<string, object> fields = null;
        
        public ParseTree()
        {
        }
        
        public ParseTree(object node)
        {
            single = true;
            
            nodes = new List<object>();
            nodes.Add(node);
        }
        
        public ParseTree Extend(ParseTree other)
        {
            return Extend(other, false);
        }
        
        public ParseTree ExtendFields(ParseTree other)
        {
            return Extend(other, true);
        }
        
        private ParseTree Extend(ParseTree other, bool justFields)
        {
            if (other == No)
                throw new NotSupportedException("Should not extend from ParseTree.No");
            
            if (this == No)
                throw new NotSupportedException("ParseTree.No is read only");
            
            ParseTree tree = this;
            
            if (!justFields)
            {
                single = false;
            
                if ((other.nodes != null) && (other.nodes.Count > 0))
                {
                    if (tree == ParseTree.Yes)
                        tree = new ParseTree();
                    
                    if (tree.nodes == null)
                        tree.nodes = new List<object>();
            
                    tree.nodes.AddRange(other.nodes);
                }
            }
            
            if ((other.fields != null) && (other.fields.Count > 0))
            {
                if (tree == ParseTree.Yes)
                    tree = new ParseTree();
                
                if (tree.fields == null)
                    tree.fields = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> field in other.fields)
                    tree.fields[field.Key] = field.Value;
            }
            
            return tree;
        }
        
        public static ParseTree Yes
        {
            get
            {
                return yes;
            }
        }
        
        public static ParseTree No
        {
            get
            {
                return no;
            }
        }
        
        public object Value
        {
            get
            {
                if (single)
                    return nodes[0];
                else
                    return Nodes;
            }
        }
        
        public List<object> Nodes
        {
            get
            {
                if (nodes == null)
                    nodes = new List<object>();
                
                return nodes;
            }
        }
        
        public Dictionary<string, object> Fields
        {
            get
            {
                if (fields == null)
                    fields = new Dictionary<string, object>();
                
                return fields;
            }
        }
        
        public bool Single
        {
            get
            {
                return single;
            }
            
            set
            {
                single = value;
            }
        }
    }
}
