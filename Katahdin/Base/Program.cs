using System.Collections.Generic;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class Program : RuntimeObject
    {
        public static Pattern pattern;
        
        public List<object> statements;
        
        public Program(Source source, object statements)
            : base(source)
        {
            this.statements = (List<object>) statements;
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "o((whitespace 0) l(statements *(1)))";
                Pattern[] patterns = {Whitespace.pattern, TopLevelStatement.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
            
                pattern = new ConcretePattern(null, "Program", parseGraph);
                pattern.SetType(typeof(Program));
            }
            
            module.SetName("Program", typeof(Program));
            grammar.PatternDefined(pattern);
        }
    }
}
