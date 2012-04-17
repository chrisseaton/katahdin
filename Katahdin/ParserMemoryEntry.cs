namespace Katahdin
{
    public class ParserMemoryEntry
    {
        private ParseTree tree;
        private int end;
        private object tag;
        
        public ParserMemoryEntry(ParseTree tree, int end, object tag)
        {
            this.tree = tree;
            this.end = end;
            this.tag = tag;
        }
        
        public ParseTree Tree
        {
            get
            {
                return tree;
            }
        }
        
        public int End
        {
            get
            {
                return end;
            }
        }
        
        public object Tag
        {
            get
            {
                return tag;
            }
        }
    }
}
