using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Katahdin.Compiler
{
    public class ConstructorBuilderProxy : IMethodBuilder
    {
        private ConstructorBuilder builder;
        
        public ConstructorBuilderProxy(ConstructorBuilder builder)
        {
            this.builder = builder;
        }
        
        public ILGenerator GetILGenerator()
        {
            return builder.GetILGenerator();
        }
    }
}
