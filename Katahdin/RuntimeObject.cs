using System;
using System.Collections;

using Katahdin.Grammars;

namespace Katahdin
{
    public abstract class RuntimeObject
    {
        private Source source;
        
        public RuntimeObject(Source source)
        {
            this.source = source;
        }
        
        public Source Source
        {
            get
            {
                return source;
            }
        }
    }
}
