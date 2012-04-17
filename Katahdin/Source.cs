using System;

namespace Katahdin
{
    public class Source
    {
        private string fileName;
        private int line;
        
        private int position;
        private int? extent;
    
        public Source(string fileName, int line, int position)
        {
            this.fileName = fileName;
            this.line = line;
            this.position = position;
        }
        
        public Source(string fileName, int line, int position, int extent)
        {
            this.fileName = fileName;
            this.line = line;
            this.position = position;
            this.extent = extent;
        }
        
        public override string ToString()
        {
            return fileName + ", line " + line + ", char " + position;
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
        }
        
        public int Line
        {
            get
            {
                return line;
            }
        }

        public int Position
        {
            get
            {
                return position;
            }
        }
        
        public bool HasExtent
        {
            get
            {
                return extent.HasValue;
            }
        }
        
        public int Extent
        {
            get
            {
                return extent.Value;
            }
        }
    }
}
