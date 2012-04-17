using System;
using System.Collections;
using System.Collections.Generic;

namespace Katahdin.Collections
{
    public class Set<T> : IEnumerable<T>
    {
        private Dictionary<T, object> items = new Dictionary<T, object>();
        
		public Set()
		{
		}
		
		public Set(params T[] items)
		{
			foreach (T item in items)
				Add(item);
		}

        public bool Add(T item)
        {
            if (Contains(item))
                return false;
            
            items[item] = null;
            return true;
        }
        
        public bool Contains(T item)
        {
            return items.ContainsKey(item);
        }
        
        public void Remove(T item)
        {
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
                return items.Count;
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return items.Keys.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
