using System;

namespace Katahdin.Grammars
{
    public class GrammarTrace
    {
        public delegate void PatternDefinedHandler(Pattern pattern);
        public event PatternDefinedHandler PatternDefinedEvent;
        
        public delegate void PatternChangedHandler(Pattern pattern);
        public event PatternChangedHandler PatternChangedEvent;
        
        public void PatternDefined(Pattern pattern)
        {
            if (PatternDefinedEvent != null)
                PatternDefinedEvent(pattern);
        }
        
        public void PatternChanged(Pattern pattern)
        {
            if (PatternChangedEvent != null)
                PatternChangedEvent(pattern);
        }
    }
}
