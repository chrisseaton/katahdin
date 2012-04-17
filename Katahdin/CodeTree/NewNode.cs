using System;
using System.Reflection;


namespace Katahdin.CodeTree
{
    public class NewNode : CodeTreeNode
    {
        private CodeTreeNode type;
        private CodeTreeNode[] parameters;
        
        private static readonly CodeTreeNode[] zeroParameters = new CodeTreeNode[0];
        
        public NewNode(Source source, CodeTreeNode type,
			CodeTreeNode[] parameters)
				: base(source)
        {
            this.type = type;
            
            if (parameters == null)
                parameters = zeroParameters;
            
            this.parameters = parameters;
        }
        
        public override object Get(RuntimeState state, object[] parametersHint)
        {
            Type type = (Type) this.type.Get(state, null);
            
            object[] parameterValues = new object[parameters.Length];

            for (int n = 0; n < parameters.Length; n++)
                parameterValues[n] = parameters[n].Get(state, null);
            
            state.RunningSource = Source;
            
            return New(state, type, parameterValues);
        }
        
        public static object New(RuntimeState state, Type type,
            object[] parameterValues)
        {
            if (parameterValues == null)
                parameterValues = new object[]{};
            
            ConstructorInfo constructor = (ConstructorInfo) MemberNode.ChooseMethod(
                type.GetConstructors(),
                null,
                parameterValues);
            
            ParameterInfo[] parameterInfo = constructor.GetParameters();
            
            bool passingState = false;
            
            if (parameterInfo.Length > 0)
                passingState = parameterInfo[0].GetType() == typeof(RuntimeState);
            else
                passingState = false;
            
            // Convert parameters to the right type
            
            object[] passedParameters = new object[parameterInfo.Length];
            
            if (passingState)
                passedParameters[0] = state;
            
            int offset;
            
            if (passingState)
                offset = 1;
            else
                offset = 0;
            
            for (int n = 0; n < parameterValues.Length; n++)
                passedParameters[n + offset] = parameterValues[n];
            
            // Call
            
            return constructor.Invoke(passedParameters);
        }
    }
}
