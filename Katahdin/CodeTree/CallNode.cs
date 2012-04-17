using System;
using System.Reflection;


namespace Katahdin.CodeTree
{
    public class CallNode : CodeTreeNode
    {
        private CodeTreeNode callable;
        private bool parentScope;
        private CodeTreeNode[] parameters;
        
        private static readonly CodeTreeNode[] zeroParameters = new CodeTreeNode[0];
        
        public CallNode(Source source, CodeTreeNode callable,
			bool parentScope, CodeTreeNode[] parameters)
				: base(source)
        {
            this.callable = callable;
            
            this.parentScope = parentScope;
            
            if (parameters == null)
                parameters = zeroParameters;
            
            this.parameters = parameters;
        }
        
        public CallNode(Source source, CodeTreeNode callable,
			CodeTreeNode[] parameters)
			    : this(source, callable, false, parameters)
		{
		}
        
        public override object Get(RuntimeState state,
            object[] parametersHint)
        {
            object[] parameterValues = new object[parameters.Length];
            
            for (int n = 0; n < parameters.Length; n++)
                parameterValues[n] = parameters[n].Get(state, null);
            
            object callable = this.callable.Get(state, parameterValues);
            
            state.RunningSource = Source;
            
            // For call in parent scope, move into the parent scope
            // now - reset in finally
            
            IScope currentScope = state.Scope;
            
            if (parentScope)
                state.Scope = state.Scope.Parent;
            
            object returnValue;
            bool[] refParams = null;
            
            try
            {
                returnValue = Call(
                    state,
                    callable,
                    parameterValues,
                    true,
                    out refParams);
            }
            finally
            {
                // If we didn't return, don't move back into the child scope
                
                if (state.Returning == null)
                    state.Scope = currentScope;
            }
            
            if (refParams != null)
            {
                for (int n = 0; n < parameters.Length; n++)
                {
                    if (refParams[n])
                        parameters[n].Set(state, parameterValues[n]);
                }
            }
            
            return returnValue;
        }

        public static object Call(RuntimeState state, object callable,
            object[] parameters)
        {
            bool[] refParams;
            
            return Call(state, callable, parameters, false, out refParams);
        }

        public static object Call(RuntimeState state, object callable,
            object[] parameters, bool wantRefParams, out bool[] refParams)
        {
            if (parameters == null)
                parameters = new object[]{};

            if (callable == null)
                throw new NotImplementedException(
                    "No call operation for null");
            else if (callable is ICallable)
                return ((ICallable) callable).Call(state, parameters,
                    wantRefParams, out refParams);
            else if (callable is MethodInfo)
            {
                ClrObjectMethodBinding binding
                    = new ClrObjectMethodBinding(null, (MethodInfo) callable);
                
                return binding.Call(state, parameters, wantRefParams,
                    out refParams);
            }
            else
            {
                object callMethod = MemberNode.GetMember(callable, "Call",
                    false);
                
                if (callMethod == null)
                    throw new NotImplementedException(
                        "No call operation for "
                        + TypeNames.GetName(callable.GetType()));
                
                if (wantRefParams)
                {
                    object getRefParamsMethod = MemberNode.GetMember(callable,
                        "GetRefParams", false);
                
                    if (getRefParamsMethod != null)
                    {
                        object refParamsObject = CallNode.Call(state,
                            getRefParamsMethod, null);
                        
                        refParams = (bool[]) ConvertNode.Convert(
                            refParamsObject, typeof(bool[]));
                    }
                    else
                    {
                        refParams = null;
                    }
                }
                else
                {
                    refParams = null;
                }
                
                return Call(state, callMethod, new object[]{parameters});
            }
        }
    }
}
