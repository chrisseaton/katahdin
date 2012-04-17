using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Katahdin.CodeTree
{
    public class TryNode : CodeTreeNode
    {
        private CodeTreeNode tryBody;
        private CodeTreeNode catchVariable;
        private CodeTreeNode catchBody;
        
        public TryNode(Source source, CodeTreeNode tryBody,
            CodeTreeNode catchVariable, CodeTreeNode catchBody)
			    : base(source)
        {
            if (tryBody == null)
                throw new Exception();
            
            this.tryBody = tryBody;
            this.catchVariable = catchVariable;
            this.catchBody = catchBody;
        }
        
        public override object Run(RuntimeState state)
        {
            try
            {
                return tryBody.Run(state);
            }
            catch (Exception e)
            {
                state.RunningSource = Source;
                
                /*
                    If you use reflection to invoke a method, any exceptions
                    thrown inside the invoke will be wrapped in a
                    TargetInvocationException. This is a problem because then
                    you might filter for certain types of exception and miss
                    the wrapped instances. Also, the user doesn't care and
                    shouldn't know that reflection was used to invoke a
                    method. Might cause problems with expected behaviour if
                    a user explicitly uses invoke or throws a
                    TargetInvocationException.
                    
                    Unwrap TargetInvocationException instances in all user
                    type blocks.
                */
                
                while (true)
                {
                    TargetInvocationException wrapper
                        = e as TargetInvocationException;

                    if (wrapper == null)
                        break;

                    e = wrapper.InnerException;
                }
                
                if (catchVariable != null)
                   catchVariable.Set(state, e);
                
                return catchBody.Run(state);
            }
        }
    }
}
