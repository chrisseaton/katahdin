using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Katahdin.Compiler
{
    public class DynamicMethodProxy : IMethodBuilder
    {
        private DynamicMethod builder;
        
        public DynamicMethodProxy(DynamicMethod builder)
        {
            this.builder = builder;
        }
        
        public ILGenerator GetILGenerator()
        {
            return builder.GetILGenerator();
        }
    }
}
