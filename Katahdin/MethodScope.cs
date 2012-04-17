using System;
using System.Collections.Generic;
using System.Reflection;

using Katahdin.CodeTree;

namespace Katahdin
{
    public class MethodScope : IScope
    {
        private IScope parent;
        private object instance;
        
        private Dictionary<string, object> names = new Dictionary<string, object>();
        
        public MethodScope(IScope parent, object instance)
        {
            this.parent = parent;
            this.instance = instance;
        }
        
        public object GetName(string name)
        {
            // Look for special names
            
            switch (name)
            {
                case "this":
                    return instance;
                
                case "scope":
                    return this;
                
                case "names":
                    return names;
                
                case "globals":
                    return parent.GetName(name);
            }
            
            // Try for a local
            
            object o;
            
            if (names.TryGetValue(name, out o))
                return o;
            
            // Pass it up
            
            return parent.GetName(name);
        }
        
        public void SetName(string name, object val)
        {
            names[name] = val;
        }
        
        public Module GetModule()
        {
            return parent.GetModule();
        }

        public IScope Parent
        {
            get
            {
                return parent;
            }
        }
        
        public object Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
