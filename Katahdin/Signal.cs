using System;
using System.Threading;

namespace Katahdin
{
    public class Signal
    {
        private Semaphore semaphore = new Semaphore(0, 1);
        
        public void WaitFor()
        {
            semaphore.WaitOne();
        }
        
        public void Send()
        {
            semaphore.Release(1);
        }
    }
}
