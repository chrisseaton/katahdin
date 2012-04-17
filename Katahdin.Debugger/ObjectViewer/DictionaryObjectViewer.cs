using System;
using System.Collections;
using System.Collections.Generic;

using Katahdin.Collections;

namespace Katahdin.Debugger.ObjectViewer
{
    public class DictionaryObjectViewer : IObjectViewer
    {
        public bool Views(object obj)
        {
            return obj is IDictionary;
        }
        
        public ObjectViewHeader GetHeader(object obj)
        {
            IDictionary dictionary = (IDictionary) obj;
            
            return new ObjectViewHeader(TypeNames.GetName(obj.GetType()),
                                        dictionary.Count > 0,
                                        true);
        }
        
        public List<Tupple<string, object>> GetMembers(object obj)
        {
            IDictionary dictionary = (IDictionary) obj;
            
            List<Tupple<string, object>> members = new List<Tupple<string, object>>();
            
            foreach (object key in dictionary.Keys)
            {
                string keyDesc = ObjectViewerDirectory.GetDescription(key);
                object valueObj = dictionary[key];
                members.Add(new Tupple<string, object>(keyDesc, valueObj));
            }
            
            return members;
        }
    }
}
