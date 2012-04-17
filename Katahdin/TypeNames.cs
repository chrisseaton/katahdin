using System;

namespace Katahdin
{
    public abstract class TypeNames
    {
        public static string GetName(Type type)
        {
            // FullName has culture and other stuff nobody cares about, so we use Name
            
            string name = type.Name;
            
            // Add the namespace if there is one
            
            if (type.Namespace != null)
                name = type.Namespace + Type.Delimiter + name;
            
            // Remove the number of parameters - who needs to know that?
            
            int tick = name.IndexOf('`');
            
            if (tick > -1)
                name = name.Substring(0, tick);
            
            // Add the types of the parameters - much more useful
            
            if (type.IsGenericType || type.IsGenericTypeDefinition)
            {
                name += "<";
                
                Type[] parameters = type.GetGenericArguments();
                
                for (int n = 0; n < parameters.Length; n++)
                {
                    if (n > 0)
                        name += ", ";
                    
                    name += GetName(parameters[n]);
                }
                
                name += ">";
            }
            
            return name;
        }
    }
}