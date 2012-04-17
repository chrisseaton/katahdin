using System;
using System.Reflection;

namespace Katahdin
{
    public class ObjectMethodBinding : ICallable
    {
        private object instance;
        private Method method;
        
        public ObjectMethodBinding(object instance, Method method)
        {
            this.instance = instance;
            this.method = method;
        }
        
        public bool[] RefParams
        {
            get
            {
                return method.RefParams;
            }
        }
        
        public object Call(RuntimeState state, object[] parameterValues,
            bool wantRefParams, out bool[] refParams)
        {
            return method.Call(state, instance, parameterValues,
                wantRefParams, out refParams);
        }
        
        public Object Instance
        {
            get
            {
                return instance;
            }
        }
        
        public Method Method
        {
            get
            {
                return method;
            }
        }
    }
}
