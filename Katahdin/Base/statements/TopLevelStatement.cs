using System;
using System.Collections.Generic;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class TopLevelStatement : RuntimeObject
    {
        public new static Pattern pattern;
        
        public Statement statement;
        
        public TopLevelStatement(Source source,
            object statement)
                : base(source)
        {
            this.statement = (Statement) statement;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "l(statement 0)";
                Pattern[] parameters = {Statement.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, parameters);
            
                pattern = new ConcretePattern(null, "TopLevelStatement", parseGraph);
                pattern.SetType(typeof(TopLevelStatement));
            }
            
            module.SetName("TopLevelStatement", typeof(TopLevelStatement));
            grammar.PatternDefined(pattern);
        }

        public void Parsed(RuntimeState state)
        {
            statement.Run(state);
            
            if (state.Returning != null)
                throw new Exception("Top level statement returned");
        }
    }
}
