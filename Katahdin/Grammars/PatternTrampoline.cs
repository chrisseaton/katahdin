using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public class PatternTrampoline
    {
        private Pattern pattern;
        private Runtime runtime;
        private StateForCompiler state;
        
        private ParseDelegate implementation;
        private ParserBlock callBlock;
        
        public PatternTrampoline(Pattern pattern, Runtime runtime,
            StateForCompiler state)
        {
            this.pattern = pattern;
            this.runtime = runtime;
            this.state = state;
            
            callBlock = new ParserBlock();
            callBlock.Comment("start of call to " + pattern.Type.Name + " -------------");
            callBlock.Load(this);
            callBlock.GetProperty(typeof(PatternTrampoline).GetProperty("Implementation"));
            callBlock.LoadLexer();
            callBlock.LoadState();
            callBlock.Call(typeof(ParseDelegate).GetMethod("Invoke"));
            callBlock.Comment("end of call to " + pattern.Type.Name + " -------------");
        }
        
        public void Updated()
        {
            implementation = null;
        }
        
        public ParseDelegate Implementation
        {
            get
            {
                if (implementation == null)
                    implementation = pattern.CompileImplementation(runtime,
                        state);
                
                return implementation;
            }
        }
        
        public Block CallBlock
        {
            get
            {
                return callBlock;
            }
        }
    }
}
