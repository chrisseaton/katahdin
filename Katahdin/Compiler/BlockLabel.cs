namespace Katahdin.Compiler
{
    public class BlockLabel
    {
        private string name;
        
        private static int unique = 0;
        
        public BlockLabel(string name)
        {
            this.name = name + unique;
            unique++;
        }
        
        public override string ToString()
        {
            return name;
        }
        
        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
