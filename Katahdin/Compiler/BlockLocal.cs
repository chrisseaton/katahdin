using System;

namespace Katahdin.Compiler
{
    public class BlockLocal
    {
        private Type type;
        
        public BlockLocal(Type type)
        {
            this.type = type;
        }
        
        public Type Type
        {
            get
            {
                return type;
            }
        }
    }
}
