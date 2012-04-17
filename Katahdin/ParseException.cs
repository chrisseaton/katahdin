using System;
using System.Text;
using System.Collections.Generic;

using Katahdin.Collections;

namespace Katahdin
{
    public class ParseException : Exception
    {
        private Source source;
        
        public ParseException(Source source, Set<string> errorStrings, string got)
            : base(source + ": expecting " + FormatSet(errorStrings) + ", got " + got)
        {
            this.source = source;
        }
        
        private static string FormatSet(Set<string> errorStrings)
        {
            StringBuilder builder = new StringBuilder();
            
            List<string> sortedErrorStrings = new List<string>(errorStrings);
            sortedErrorStrings.Sort();
            
            foreach (string errorString in sortedErrorStrings)
            {
                if (builder.Length > 0)
                    builder.Append(", ");
                
                builder.Append(errorString);
            }
            
            return builder.ToString();
        }
        
        public Source CodeSource
        {
            get
            {
                return source;
            }
        }
    }
}