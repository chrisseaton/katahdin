using System;
using System.Collections.Generic;

namespace Katahdin
{
    public class Module : IScope
    {
        private Module parent;
        private string name;
        
        private IDictionary<string, object> names = new Dictionary<string, object>();
		
		public Module(Module parent, string name)
		{
		    this.parent = parent;
		    this.name = name;
		}
		
		public IScope Parent
		{
		    get
		    {
		        return parent;
		    }
		}
		
		public string Name
		{
		    get
		    {
		        return name;
		    }
		}
		
		public string Namespace
	    {
	        get
	        {
	            if (parent == null)
	            {
	                return name;
                }
	            else
	            {
	                string parentNamespace = parent.Namespace;
	                
	                if (parentNamespace == null)
	                    return name;
	                else
	                    return parentNamespace + Type.Delimiter + name;
                }
	        }
	    }
	    
	    public Module GetModule()
	    {
	        return this;
	    }
	    
	    public bool IsDefined(string name)
	    {
	        return names.ContainsKey(name);
	    }
		
		public object GetName(string name)
		{
            // Look for special names
            
            switch (name)
            {
                case "scope":
                    return this;
                
                case "names":
                    return names;
                
                case "globals":
    		        if (parent == null)
    		            return this;
    		        else
    		            return parent.GetName(name);
            }
		    
            // Module names
            
		    if (names.ContainsKey(name))
		        return names[name];
		    
		    // Check for root
		    
		    if (parent == null)
		        throw new Exception("name " + TextEscape.Quote(name) + " not defined");
		    
		    // Pass it up
		    
		    return parent.GetName(name);
		}

		public void SetName(string name, object obj)
		{
		    names[name] = obj;
		}
    }
}
