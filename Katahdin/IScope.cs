using System;

namespace Katahdin
{
    public interface IScope
    {
        object GetName(string name);
        void SetName(string name, object val);
        
        Module GetModule();
        
        IScope Parent
        {
            get;
        }
    }
}
