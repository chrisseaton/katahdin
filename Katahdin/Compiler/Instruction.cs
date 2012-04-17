using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Katahdin.Compiler
{
    public class Instruction : IBlockLine
    {
        private OpCode op;
        private object parameter;
    
        public Instruction(OpCode op, object parameter)
        {
            this.op = op;
            this.parameter = parameter;
        }
        
        public OpCode Op
        {
            get
            {
                return op;
            }
        }
        
        public object Parameter
        {
            get
            {
                return parameter;
            }
        }
        
        public override string ToString()
        {
            if (parameter == null)
                return "    " + op.ToString();
            else if (parameter is string)
                return "    " + op + " " + TextEscape.Quote((string) parameter);
            else if (parameter is Type)
                return "    " + op + " " + TypeNames.GetName((Type) parameter);
            else if (parameter is FieldInfo)
            {
                FieldInfo field = (FieldInfo) parameter;
                return "    " + op + " " + TypeNames.GetName(field.DeclaringType) + "::" + field.Name;
            }
            else if (parameter is MethodInfo)
            {
                MethodInfo method = (MethodInfo) parameter;
                return "    " + op + " " + TypeNames.GetName(method.DeclaringType) + "::" + method.Name;
            }
            else if (parameter is ConstructorInfo)
            {
                ConstructorInfo constructor = (ConstructorInfo) parameter;
                return "    " + op + " " + TypeNames.GetName(constructor.DeclaringType);
            }    
            else if (parameter is BlockLocal)
                return "    " + op + " " + TypeNames.GetName(((BlockLocal) parameter).Type);
            else
                return "    " + op + " " + parameter;
        }
    }
}