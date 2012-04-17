using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Katahdin.Compiler
{
    public class MethodBuilderProxy : IMethodBuilder
    {
        private MethodBuilder builder;
        
        public MethodBuilderProxy(MethodBuilder builder)
        {
            this.builder = builder;
        }
        
        public ILGenerator GetILGenerator()
        {
            return builder.GetILGenerator();
        }
    }
}
