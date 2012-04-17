using Katahdin.Grammars;
using Katahdin.Compiler;

namespace Katahdin.Base
{
    public class Member : RuntimeObject
    {
        public static AbstractPattern pattern;
        
        public Member(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                pattern = new AbstractPattern(null, "Member");
                pattern.SetType(typeof(Member));
            }
            
            module.SetName("Member", typeof(Member));
            grammar.PatternDefined(pattern);
        }
        
        public virtual void Build(RuntimeState state, ClassBuilder builder)
        {
            // TODO call method defined by user when in a subclass
        }
    }
}
