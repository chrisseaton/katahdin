using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Katahdin.CodeTree
{
    public class PrintNode : CodeTreeNode
    {
        private CodeTreeNode expression;
        
        private static MethodInfo writeLine;
        
        static PrintNode()
        {
            Type[] parameterTypes = new Type[]{typeof(string)};
            writeLine = typeof(Console).GetMethod("WriteLine", parameterTypes);
            
            if (writeLine == null)
                throw new Exception();
        }
        
        public PrintNode(Source source, CodeTreeNode expression)
			: base(source)
        {
            this.expression = new ConvertNode(
                source,
                expression,
                new ValueNode(
                    source,
                    typeof(string)));
        }
        
        public override object Run(RuntimeState state)
        {
            Console.WriteLine((string) expression.Get(state));
            return null;
        }
    }
}
