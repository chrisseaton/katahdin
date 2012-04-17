using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class CompoundStatement : Statement
    {
        public new static Pattern pattern;
        
        public List<object> body;
        
        public CompoundStatement(Source source, object body)
            : base(source)
        {
            this.body = (List<object>) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('{' l(body *(0)) '}')";
                Pattern[] patterns = {Statement.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "CompoundStatement", parseGraph);
                pattern.SetType(typeof(CompoundStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("CompoundStatement", typeof(CompoundStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            List<CodeTreeNode> children = new List<CodeTreeNode>();
            
            foreach (Statement statement in body)
            {
                CodeTreeNode tree = statement.BuildCodeTree(state);
                children.Add(tree);
            }
            
            return new CodeTree.SeqNode(
                Source,
                children);
        }
    }
}
