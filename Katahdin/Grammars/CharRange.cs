using System;


namespace Katahdin.Grammars
{
    public class CharRange
    {
        private char min;
        private char max;
        
        public CharRange(char character)
        {
            min = character;
            max = character;
        }
        
        public CharRange(char min, char max)
        {
            this.min = min;
            this.max = max;
        }
        
        public bool Contains(char character)
        {
            return (character >= min) && (character <= max);
        }
        
        public char Min
        {
            get
            {
                return min;
            }
        }
        
        public char Max
        {
            get
            {
                return max;
            }
        }
        
        public override string ToString()
        {
            if (min == max)
                return TextEscape.Quote(min);
            else
                return TextEscape.Quote(min) + ".." + TextEscape.Quote(max);
        }
    }
}
