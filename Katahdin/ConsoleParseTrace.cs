using System;

namespace Katahdin
{
    public class ConsoleParseTrace
    {
        private Runtime runtime;
        private int indent;
        
        public ConsoleParseTrace(Runtime runtime)
        {
            this.runtime = runtime;
            runtime.ParseTrace.EnterEvent += OnEnter;
            runtime.ParseTrace.LeaveEvent += OnLeave;
        }
        
        ~ConsoleParseTrace()
        {
            runtime.ParseTrace.EnterEvent -= OnEnter;
            runtime.ParseTrace.LeaveEvent -= OnLeave;
        }
        
        private void OnEnter(Source source, string label, object tag, object link)
        {
            for (int n = 0; n < indent; n++)
                Console.Write("    ");
            
            Console.WriteLine(label);
            
            indent++;
        }
        
        private void OnLeave()
        {
            indent--;
        }
    }
}
