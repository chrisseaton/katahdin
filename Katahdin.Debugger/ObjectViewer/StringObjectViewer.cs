using System;
using System.Collections.Generic;

using Katahdin.Collections;

namespace Katahdin.Debugger.ObjectViewer
{
    public class StringObjectViewer : IObjectViewer
    {
        public bool Views(object obj)
        {
            return obj is string;
        }
        
        public ObjectViewHeader GetHeader(object obj)
        {
            return new ObjectViewHeader(TextEscape.Quote((string) obj), false, false);
        }
        
        public List<Tupple<string, object>> GetMembers(object obj)
        {
            return null;
        }
    }
}
