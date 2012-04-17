using System;
using System.Collections.Generic;

using Katahdin.CodeTree;

namespace Katahdin
{
    public class Method : ICallable
    {
        private string id;
        private string name;
        private bool isInstance;
        private List<string> parameters;
        private CodeTreeNode codeTree;
        
        public Method(string id, string name, bool isInstance,
            List<string> parameters)
        {
            this.id = id;
            this.name = name;
            this.isInstance = isInstance;
            this.parameters = parameters;
        }
        
        public bool[] RefParams
        {
            get
            {
                return null;
            }
        }
        
        public object Call(RuntimeState state, object instance,
            object[] parameters, bool wantRefParams, out bool[] refParams)
        {
            if (!isInstance)
                throw new Exception();
            
            return CallInternal(state, instance, parameters, wantRefParams,
                out refParams);
        }
        
        public object Call(RuntimeState state, object[] parameters,
            bool wantRefParams, out bool[] refParams)
        {
            if (isInstance)
                throw new Exception();
            
            return CallInternal(state, null, parameters, wantRefParams,
                out refParams);
        }
        
        private object CallInternal(RuntimeState state, object instance,
            object[] parameterValues, bool wantRefParams,
            out bool[] refParams)
        {
            if (wantRefParams)
                refParams = RefParams;
            else
                refParams = null;
            
            if (parameterValues.Length != parameters.Count)
                throw new Exception("TODO Incorrect number of parameters, "
                    + "expecting " + parameters.Count + " got "
                    + parameterValues.Length);
            
            MethodScope methodScope = new MethodScope(
                state.Scope,
                instance);
            
            for (int n = 0; n < parameters.Count; n++)
                methodScope.SetName(parameters[n], parameterValues[n]);
            
            state.Scope = methodScope;
            object returned = codeTree.Run(state);
            state.Scope = methodScope.Parent;
            
            for (int n = 0; n < parameters.Count; n++)
                parameterValues[n] = methodScope.GetName(parameters[n]);
            
            if (state.Returning == methodScope)
                state.Returning = null;
            
            return returned;
        }
        
        public string Id
        {
            get
            {
                return id;
            }
        }
        
        public string Name
        {
            get
            {
                return name;
            }
        }
        
        public List<string> Parameters
        {
            get
            {
                return parameters;
            }
        }
        
        public CodeTreeNode CodeTree
        {
            get
            {
                if (codeTree == null)
                    throw new Exception();
                
                return codeTree;
            }
        }
        
        public bool IsInstance
        {
            get
            {
                return isInstance;
            }
        }
        
        public void SetCodeTree(CodeTreeNode codeTree)
        {
            if (this.codeTree != null)
                throw new Exception();
            
            this.codeTree = codeTree;
        }
    }
}
