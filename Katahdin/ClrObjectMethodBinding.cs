using System;
using System.Reflection;

namespace Katahdin
{
    public class ClrObjectMethodBinding : ICallable
    {
        private object obj;
        private MethodInfo method;
        private bool passState;
        private bool[] refParams;
        
        public ClrObjectMethodBinding(object obj, MethodInfo method)
        {
            this.obj = obj;
            this.method = method;
            
            ParameterInfo[] parameters = method.GetParameters();
            
            passState = ((parameters.Length > 0)
                && (parameters[0].ParameterType == typeof(RuntimeState)));
        }
        
        public bool[] RefParams
        {
            get
            {
                if (refParams == null)
                    refParams = GetRefParams(method);
                
                return refParams;
            }
        }
        
        public object Call(RuntimeState state, object[] parameterValues,
            bool wantRefParams, out bool[] refParams)
        {
            int extra;
            
            if (passState)
                extra = 1;
            else
                extra = 0;
            
            object[] actualParameterValues =
                new object[extra + parameterValues.Length];
            
            if (passState)
                actualParameterValues[0] = state;
            
            for (int n = 0; n < parameterValues.Length; n++)
                actualParameterValues[extra + n] = parameterValues[n];
            
            if (wantRefParams)
            {
                refParams = GetRefParams(method);
                
                if ((refParams != null) && passState)
                {
                    bool[] newRefParams = new bool[refParams.Length - 1];
                    
                    for (int n = 0; n < newRefParams.Length; n++)
                        newRefParams[n] = refParams[n + 1];
                    
                    refParams = newRefParams;
                }
            }
            else
            {
                refParams = null;
            }
            
            object returnValue = method.Invoke(obj, actualParameterValues);
            
            for (int n = 0; n < parameterValues.Length; n++)
                parameterValues[n] = actualParameterValues[extra + n];
            
            return returnValue;
        }
        
        public static bool[] GetRefParams(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            
            if (parameters.Length == 0)
                return null;
            
            bool anyRefs = false;
            
            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.ParameterType.IsByRef)
                {
                    anyRefs = true;
                    break;
                }
            }
            
            if (!anyRefs)
                return null;
            
            bool[] refParams = new bool[parameters.Length];
            
            for (int n = 0; n < parameters.Length; n++)
                refParams[n] = parameters[n].ParameterType.IsByRef;
            
            return refParams;
        }

        public MethodInfo Method
        {
            get
            {
                return method;
            }
        }
    }
}
