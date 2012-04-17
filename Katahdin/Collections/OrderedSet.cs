using System;
using System.Collections;
using System.Collections.Generic;

namespace Katahdin.Collections
{
    public class OrderedSet<T> : IEnumerable<T>
    {
        private Set<T> items = new Set<T>();
        private List<T> order = new List<T>();
        
        public void Add(T item)
        {
            if (items.Add(item))
                order.Add(item);
        }
        
        public T this[int index]
        {
            get
            {
                return order[index];
            }
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
            return order.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
