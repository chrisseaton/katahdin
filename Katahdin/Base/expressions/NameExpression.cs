using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class NameExpression : Expression
    {
        public new static Pattern pattern;
        
        public Name name;
        
        public NameExpression(Source source, object name)
            : base(source)
        {
            this.name = (Name) name;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "l(name 0)";
                Pattern[] parameters = {Name.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
        
                pattern = new ConcretePattern(null, "NameExpression", parseGraph);
                pattern.SetType(typeof(NameExpression));
            
                Expression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("NameExpression", typeof(NameExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            return new NameNode(
                Source,
                name.name);
        }
    }
}
