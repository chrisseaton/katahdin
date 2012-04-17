using System;

namespace Katahdin.Grammars
{
    public class Reps
    {
        public static readonly Reps Opt = new Reps("?");
        public static readonly Reps ZeroPlus = new Reps("*");
        public static readonly Reps OnePlus = new Reps("+");
        
        private string name;
        
        public static Reps ForName(string name)
        {
            if (name.Length != 1)
                throw new ArgumentException();
            
            return ForName(name[0]);
        }
        
        public static Reps ForName(char name)
        {
            switch (name)
            {
                case '?': return Opt;
                case '*': return ZeroPlus;
                case '+': return OnePlus;
            }
            
            throw new ArgumentException();
        }
        
        private Reps(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
        
        public int Minimum
        {
            get
            {
                if (this == OnePlus)
                    return 1;
                else
                    return 0;
            }
        }
        
        public bool MaximumOfOne
        {
            get
            {
                return this == Opt;
            }
        }
    }
}
