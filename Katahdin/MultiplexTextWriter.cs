using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Katahdin
{
    public class MultiplexTextWriter : TextWriter
    {
        private List<TextWriter> outputs = new List<TextWriter>();
    
        public override void Write(string s)
        {
            foreach (TextWriter output in outputs)
                output.Write(s);
        }
    
        public override void WriteLine()
        {
            Write("\n");
        }
        
        public List<TextWriter> Outputs
        {
            get
            {
                return outputs;
            }
        }

        public override Encoding Encoding
        {
            get
            {
                return outputs[0].Encoding;
            }
    	}
    }
}
