using Katahdin.Grammars;

namespace Katahdin
{
    public class ParserMemoryKey
    {
        private ConcretePattern pattern;
        private int position;
        
        public ParserMemoryKey(ConcretePattern pattern, int position)
        {
            this.pattern = pattern;
            this.position = position;
        }
        
        public ConcretePattern Pattern
        {
            get
            {
                return pattern;
            }
        }
        
        public int Position
        {
            get
            {
                return position;
            }
        }
        
        public override int GetHashCode()
        {
            return pattern.GetHashCode() ^ position;
        }
        
        public override bool Equals(object other)
        {
            ParserMemoryKey key = other as ParserMemoryKey;
            
            if (key != null)
                return Equals(key);
            
            return false;
        }
        
        public bool Equals(ParserMemoryKey other)
        {
            return (pattern == other.pattern) && (position == other.position);
        }
    }
}
