using System;
using System.Collections.Generic;

using Katahdin.Collections;

namespace Katahdin.Debugger.ObjectViewer
{
    public class ArrayObjectViewer : IObjectViewer
    {
        public bool Views(object obj)
        {
            return obj is Array;
        }
        
        public ObjectViewHeader GetHeader(object obj)
        {
            Array array = (Array) obj;
            
            return new ObjectViewHeader(TypeNames.GetName(obj.GetType()),
                                        array.Length > 0,
                                        false);
        }
        
        public List<Tupple<string, object>> GetMembers(object obj)
        {
            Array array = (Array) obj;
            
            List<Tupple<string, object>> members = new List<Tupple<string, object>>();
            
            for (int n = 0; n < array.Length; n++)
                members.Add(new Tupple<string, object>("[" + n + "]", array.GetValue(n)));
            
            return members;
        }
    }
}
