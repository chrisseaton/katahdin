using System;
using System.Collections.Generic;

using Katahdin.Collections;

namespace Katahdin.Debugger.ObjectViewer
{
    public class SimpleObjectViewer : IObjectViewer
    {
        public bool Views(object obj)
        {
            return (obj is Boolean)
                || (obj is Byte)
                || (obj is UInt16)
                || (obj is Int16)
                || (obj is UInt32)
                || (obj is Int32)
                || (obj is UInt64)
                || (obj is Int64)
                || (obj is Single)
                || (obj is Double);
        }
        
        public ObjectViewHeader GetHeader(object obj)
        {
            string description = string.Format("({0}) {1}", TypeNames.GetName(obj.GetType()), obj);
            return new ObjectViewHeader(description, false, false);
        }
        
        public List<Tupple<string, object>> GetMembers(object obj)
        {
            return null;
        }
    }
}
