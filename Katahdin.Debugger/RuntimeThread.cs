using System;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;

namespace Katahdin.Debugger
{
    public class RuntimeThread
    {
        public event Handler RunningEvent;
        public event Handler FinishedEvent;
        
        public delegate void RuntimeErrorHandler(Exception exception);
        public event RuntimeErrorHandler RuntimeErrorEvent;
        
        private Runtime runtime;
        private Thread thread;
        
        private Signal signal = new Signal();
        
        private bool importStandard;
        private List<string> files;
        private List<string> args;
        
        public RuntimeThread(Runtime runtime)
        {
            this.runtime = runtime;
            
            thread = new Thread(new ThreadStart(Entry));
            thread.Start();
        }
        
        public void Run(List<string> files)
        {
            this.files = files;
            
            signal.Send();
        }
        
        public void Shutdown()
        {
            thread.Abort();
        }
        
        private void Entry()
        {
            bool hasSetUp = false;
            
            while (true)
            {
                signal.WaitFor();
                
                if (RunningEvent != null)
                    RunningEvent();
                
                try
                {
                    if (!hasSetUp)
                    {
                        runtime.SetUp(args);
                    
                        if (importStandard)
                            runtime.ImportStandard();
                    
                        hasSetUp = true;
                    }
                
                    foreach (string file in (IEnumerable<string>) files)
                        runtime.Import(file);
                }
                catch (Exception e)
                {
                    while (true)
                    {
                        TargetInvocationException wrapper
                            = e as TargetInvocationException;

                        if (wrapper == null)
                            break;

                        e = wrapper.InnerException;
                    }

                    if (e is ThreadAbortException)
                        break;
                    
                    if (RuntimeErrorEvent != null)
                        RuntimeErrorEvent(e);
                }
                
                if (FinishedEvent != null)
                    FinishedEvent();
            }
            
            if (FinishedEvent != null)
                FinishedEvent();
        }
        
        public Runtime Runtime
        {
            get
            {
                return runtime;
            }
        }
        
        public bool ImportStandard
        {
            get
            {
                return importStandard;
            }
            
            set
            {
                importStandard = value;
            }
        }
    }
}
