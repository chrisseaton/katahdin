using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.Collections;

namespace Katahdin.Debugger.ObjectViewer
{
    public class StandardObjectViewer : IObjectViewer
    {
        public bool Views(object obj)
        {
            return true;
        }
        
        public ObjectViewHeader GetHeader(object obj)
        {
            if (obj == null)
                return new ObjectViewHeader("null", false, false);
            
            Type type = obj.GetType();
            
            string description = string.Format("({0}) 0x{1:x}", TypeNames.GetName(type), obj.GetHashCode());
            bool hasContents = GetFields(type).Length > 0;
            
            return new ObjectViewHeader(description, hasContents, false);
        }
        
        public List<Tupple<string, object>> GetMembers(object obj)
        {
            Type type = obj.GetType();
            
            List<Tupple<string, object>> members = new List<Tupple<string, object>>();
        
            foreach (FieldInfo field in GetFields(type))
                members.Add(new Tupple<string, object>(field.Name, field.GetValue(obj)));
        
            return members;
        }
        
        private FieldInfo[] GetFields(Type type)
        {
            return type.GetFields(BindingFlags.Public
                | BindingFlags.NonPublic | BindingFlags.Instance
                | BindingFlags.FlattenHierarchy);
        }
    }
}
