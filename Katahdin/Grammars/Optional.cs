using System;

/*
    This class differs from Nullable in that T can be nullable. This allows
    you to have an optional class value that can be set to null, for example.
    Also, this is a class instead of a struct, which is nicer to deal with.
*/

namespace Katahdin.Grammars
{
    public class Optional<T>
    {
        private bool hasValue;
        private T optionValue;
        
        public Optional()
        {
        }
        
        public Optional(T optionValue)
        {
            Value = optionValue;
        }
        
        public bool HasValue
        {
            get
            {
                return hasValue;
            }
        }
        
        public T Value
        {
            get
            {
                if (!hasValue)
                    throw new Exception();
                
                return optionValue;
            }
            
            set
            {
                hasValue = true;
                optionValue = value;
            }
        }
    }
}
