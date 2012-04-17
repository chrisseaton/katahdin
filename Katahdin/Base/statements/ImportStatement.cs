using System;
using System.Reflection;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class ImportStatement : Statement
    {
        public new static Pattern pattern;
        
        public String fileName;
        
        private static MethodInfo defineMethod;
        
        public ImportStatement(Source source, object fileName)
            : base(source)
        {
            this.fileName = (String) fileName;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('import' l(fileName 0) ';')";
                Pattern[] parameters = {String.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "ImportStatement", parseGraph);
                pattern.SetType(typeof(ImportStatement));
            
                Statement.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("ImportStatement", typeof(ImportStatement));
            grammar.PatternDefined(pattern);
        }
        
        public override CodeTreeNode BuildCodeTree(RuntimeState state)
        {
            if (defineMethod == null)
            {
                Type[] parameterTypes = new Type[]{typeof(RuntimeState)};
                defineMethod = GetType().GetMethod("Define", parameterTypes);
                
                if (defineMethod == null)
                    throw new Exception();
            }
            
            CodeTreeNode callable = new ValueNode(
                Source,
                new ClrObjectMethodBinding(
                    this,
                    defineMethod));
            
            return new GetToRunNode(
                Source,
                new CallNode(
                    Source,
                    callable,
                    null));
        }
        
        public void Define(RuntimeState state)
        {
            state.Runtime.Import(fileName.Text);
        }
    }
}
