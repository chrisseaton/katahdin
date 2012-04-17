using System;
using System.Collections;
using System.Collections.Generic;

namespace Katahdin.Collections
{
    public class Stack<T> : IEnumerable<T>
    {
        private List<T> stack = new List<T>();
        
        public int Count
        {
            get
            {
                return stack.Count;
            }
        }
        
        public T this[int n]
        {
            get
            {
                return stack[n];
            }
        }
        
        public void Push(T item)
        {
            stack.Add(item);
        }
        
        public T Pop()
        {
            T item = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);
            return item;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return stack.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
