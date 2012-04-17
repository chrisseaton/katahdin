using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class BlockCommentPrototype : RuntimeObject
    {
        public static AbstractPattern pattern;
        
        public BlockCommentPrototype(Source source)
            : base(source)
        {
        }
    }
    
    public class BlockComment : BlockCommentPrototype
    {
        public new static Pattern pattern;
        
        public BlockComment(Source source)
            : base(source)
        {
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                BlockCommentPrototype.pattern
                    = new AbstractPattern(null, "BlockCommentPrototype");    
                
                BlockCommentPrototype.pattern.SetType(typeof(BlockCommentPrototype));
                grammar.PatternDefined(BlockCommentPrototype.pattern);
                
                string expression = "s('/*' *(s(!('*/') a(0 any))) '*/')";
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression,
                    new Pattern[]{BlockCommentPrototype.pattern});
            
                pattern = new ConcretePattern(null, "BlockComment",
                    parseGraph);
                
                pattern.SetType(typeof(BlockComment));
                
                BlockCommentPrototype.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("BlockComment", typeof(BlockComment));
            grammar.PatternDefined(pattern);
        }
    }
}
