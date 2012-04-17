using System;
using System.Collections;
using System.Collections.Generic;

using Katahdin.Collections;

namespace Katahdin.Debugger.ObjectViewer
{
    public class ListObjectViewer : IObjectViewer
    {
        public bool Views(object obj)
        {
            return obj is IList;
        }
        
        public ObjectViewHeader GetHeader(object obj)
        {
            IList list = (IList) obj;
            
            return new ObjectViewHeader(TypeNames.GetName(obj.GetType()),
                                        list.Count > 0,
                                        true);
        }
        
        public List<Tupple<string, object>> GetMembers(object obj)
        {
            IList list = (IList) obj;
            
            List<Tupple<string, object>> members = new List<Tupple<string, object>>();
            
            for (int n = 0; n < list.Count; n++)
                members.Add(new Tupple<string, object>("[" + n.ToString() + "]", list[n]));
            
            return members;
        }
    }
}
