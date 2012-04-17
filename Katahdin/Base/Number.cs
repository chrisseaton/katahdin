using System;

using Katahdin.Grammars;

namespace Katahdin.Base
{
    public class Number : RuntimeObject
    {
        public static Pattern pattern;
        
        public string number;
        
        public Number(Source source, object number)
            : base(source)
        {
            this.number = (string) number;
        }
        
        public static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "l(number o((whitespace null) t(s(+(r('0' '9')) ?(s('.' +(r('0' '9'))))))))";
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, null);
        
                pattern = new ConcretePattern(null, "Number", parseGraph);
                pattern.SetType(typeof(Number));
            }
            
            module.SetName("Number", typeof(Number));
            grammar.PatternDefined(pattern);
        }
        
        public object Value
        {
            get
            {
                if (number.IndexOf('.') > -1)
                    return Convert.ToDouble(number);
                else
                    return Convert.ToInt32(number);
            }
        }
    }
}
