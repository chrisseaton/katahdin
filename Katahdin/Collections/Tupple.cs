using System;

namespace Katahdin.Collections
{
    public class Tupple<TA, TB>
    {
        private TA a;
        private TB b;
        
        public Tupple(TA a, TB b)
        {
            this.a = a;
            this.b = b;
        }
        
        public TA A
        {
            get
            {
                return a;
            }
        }
        
        public TB B
        {
            get
            {
                return b;
            }
        }
        
        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode();
        }
        
        public bool Equals(Tupple<TA, TB> other)
        {
            return a.Equals(other.a) && b.Equals(other.b);
        }
        
        public override bool Equals(object other)
        {
            if (other is Tupple<TA, TB>)
                return Equals((Tupple<TA, TB>) other);
            else
                return false;
        }
    }
    
    public class Tupple3<TA, TB, TC>
    {
        private TA a;
        private TB b;
        private TC c;
        
        public Tupple3(TA a, TB b, TC c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        
        public TA A
        {
            get
            {
                return a;
            }
        }
        
        public TB B
        {
            get
            {
                return b;
            }
        }
        
        public TC C
        {
            get
            {
                return c;
            }
        }
        
        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode() ^ c.GetHashCode();
        }
        
        public bool Equals(Tupple3<TA, TB, TC> other)
        {
            return a.Equals(other.a) && b.Equals(other.b) && c.Equals(other.c);
        }
        
        public override bool Equals(object other)
        {
            if (other is Tupple3<TA, TB, TC>)
                return Equals((Tupple3<TA, TB, TC>) other);
            else
                return false;
        }
    }
    
    public class ListTupple
    {
        private object[] items;
        
        public ListTupple(params object[] items)
        {
            this.items = items;
        }
        
        public object this[int index]
        {
            get
            {
                return items[index];
            }
        }
        
        public override int GetHashCode()
        {
            int hash = 0;
            
            foreach (object item in items)
                hash ^= item.GetHashCode();
            
            return hash;
        }
        
        public bool Equals(ListTupple other)
        {
            if (items.Length != other.items.Length)
                return false;
            
            for (int n = 0; n < items.Length; n++)
            {
                if (items[n] != other.items[n])
                    return false;
            }
            
            return true;
        }
        
        public override bool Equals(object other)
        {
            if (other is ListTupple)
                return Equals((ListTupple) other);
            else
                return false;
        }
    }
}
