using System;
using System.IO;
using System.Text;

using Gtk;

using Katahdin;

namespace Katahdin.Debugger
{
    public class ConsoleWidget : ScrolledWindow
    {
        private class ConsoleWidgetWriter : TextWriter
        {
            private TextView view;
        
            public ConsoleWidgetWriter(TextView view)
            {
                this.view = view;
            }
            
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.Unicode;
                }
    		}
            
            public override void Write(string s)
            {
                Application.Invoke(delegate
                {
                    TextIter iter = view.Buffer.EndIter;
                    view.Buffer.Insert(ref iter, s);
                    view.ScrollToIter(iter, 0, false, 0, 0);
                });
            }
            
            public override void WriteLine()
            {
                Write("\n");
            }
        }
    
        private TextView view;
        
        public ConsoleWidget()
        {
            view = new TextView();
            view.Editable = false;
            view.CursorVisible = false;
            view.LeftMargin = 5;
            view.RightMargin = 5;
            Add(view);
            
            ConsoleWidgetWriter consoleWriter = new ConsoleWidgetWriter(view);
            
            MultiplexTextWriter consoleOutput = new MultiplexTextWriter();
            consoleOutput.Outputs.Add(Console.Out);
            consoleOutput.Outputs.Add(consoleWriter);
            Console.SetOut(consoleOutput);
            
            MultiplexTextWriter consoleError = new MultiplexTextWriter();
            consoleError.Outputs.Add(Console.Error);
            consoleError.Outputs.Add(consoleWriter);
            Console.SetError(consoleError);
        }
    }
}
