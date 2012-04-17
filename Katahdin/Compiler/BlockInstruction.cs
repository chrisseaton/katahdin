using System;

namespace Katahdin.Compiler
{
    public class BlockInstruction : IBlockLine
    {
        private BlockOperation operation;
        private object parameter;
        
        public static readonly BlockInstruction BeginScope
            = new BlockInstruction(BlockOperation.BeginScope, null);

        public static readonly BlockInstruction EndScope
            = new BlockInstruction(BlockOperation.EndScope, null);
        
        public BlockInstruction(BlockOperation operation, object parameter)
        {
            this.operation = operation;
            this.parameter = parameter;
        }
        
        public override string ToString()
        {
            if (operation == BlockOperation.Comment)
                return "    // " + (string) parameter;
            else if (operation == BlockOperation.MarkLabel)
                return "  " + ((BlockLabel) parameter).Name + ":";
            else if (operation == BlockOperation.DeclareLocal)
                return "    local " + TypeNames.GetName(((BlockLocal) parameter).Type);
            else if (operation == BlockOperation.BeginScope)
                return "    {";
            else if (operation == BlockOperation.EndScope)
                return "    }";
            else
                throw new Exception();
        }
        
        public BlockOperation Operation
        {
            get
            {
                return operation;
            }
        }
        
        public object Parameter
        {
            get
            {
                return parameter;
            }
        }
    }
}
