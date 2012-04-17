
namespace Katahdin
{
    public class RuntimeState
    {
        private Runtime runtime;
        private IScope scope;
        
        private IScope returning;
        
        private Source runningSource;
        
        public RuntimeState(Runtime runtime, IScope scope)
        {
            this.runtime = runtime;
            this.scope = scope;
        }
        
        public Runtime Runtime
        {
            get
            {
                return runtime;
            }
        }
        
        public IScope Scope
        {
            get
            {
                return scope;
            }
            
            set
            {
                scope = value;
            }
        }
        
        public IScope Returning
        {
            get
            {
                return returning;
            }
            
            set
            {
                returning = value;
            }
        }
        
        public Source RunningSource
        {
            get
            {
                return runningSource;
            }
            
            set
            {
                runningSource = value;
            }
        }
    }
}