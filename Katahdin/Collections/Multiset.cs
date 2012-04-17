using System;
using System.Collections;
using System.Collections.Generic;

namespace Katahdin.Collections
{
    public class Multiset<T> : IEnumerable<T>
    {
        private Dictionary<T, int> items = new Dictionary<T, int>();
        
		public Multiset()
		{
		}
		
		public Multiset(params T[] items)
		{
			foreach (T item in items)
				Add(item);
		}

        public void Add(T item)
        {
            if (Contains(item))
                items[item]++;
            else
                items[item] = 1;
        }
        
        public bool Contains(T item)
        {
            return items.ContainsKey(item);
        }
        
        public void Remove(T item)
        {
            if (!Contains(item))
                throw new Exception();
            
            if (--items[item] == 0)
                items.Remove(item);
        }
        
        public void Remove(IEnumerable<T> itemList)
        {
            foreach (T item in itemList)
                items.Remove(item);
        }
        
        public int Count
        {
            get
            {
                int count = 0;
                
                foreach (int n in items.Values)
                    count += n;
                
                return count;
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            foreach (KeyValuePair<T, int> entry in items)
            {
                for (int n = 0; n < entry.Value; n++)
                    yield return entry.Key;
            }
            
            /*List<T> enumerated = new List<T>();
            
            foreach (KeyValuePair<T, int> entry in items)
            {
                for (int n = 0; n < entry.Value; n++)
                    enumerated.Add(entry.Key);
            }
            
            return enumerated.GetEnumerator();*/
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
