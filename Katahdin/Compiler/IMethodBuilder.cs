using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Katahdin.Compiler
{
    public interface IMethodBuilder
    {
        ILGenerator GetILGenerator();
    }
}
