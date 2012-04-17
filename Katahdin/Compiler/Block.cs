using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Katahdin.Compiler
{
    public class Block
    {
        private List<IBlockLine> lines = new List<IBlockLine>();
        
        private static Dictionary<object, int> resourceIndexes = new Dictionary<object, int>();
        public static List<object> resources = new List<object>();
        
        public void Comment(string comment)
        {
            Emit(new BlockInstruction(BlockOperation.Comment, comment));
        }
        
        public void LoadNull()
        {
            Emit(OpCodes.Ldnull);
        }
        
        public void Load(bool v)
        {
            Load(v ? 1 : 0);
        }
        
        public void Load(char character)
        {
            Load((int) character);
        }
        
        public void Load(int v)
        {
            Emit(OpCodes.Ldc_I4, v);
        }
        
        public void Load(string str)
        {
            if (str == null)
                LoadNull();
            else
                Emit(OpCodes.Ldstr, str);
        }
        
        public void Load(object o)
        {
            if (o == null)
            {
                LoadNull();
            }
            else if (o is string)
            {
                Load(o as string);
            }
            else
            {
                int id;
                
                if (!resourceIndexes.TryGetValue(o, out id))
                {
                    resources.Add(o);
                    id = resources.Count - 1;
                    
                    resourceIndexes[o] = id;
                }
                
                LoadField(typeof(Block).GetField("resources"));
                Load(id);
                GetProperty(resources.GetType().GetProperty("Item"));
            }
        }
        
        public void LoadArg(int n)
        {
            Emit(OpCodes.Ldarg, n);
        }
        
        public void LoadLocal(BlockLocal local)
        {
            Emit(OpCodes.Ldloc, local);
        }
        
        public void LoadLocalAddress(BlockLocal local)
        {
            Emit(OpCodes.Ldloca, local);
        }
        
        public void StoreLocal(BlockLocal local)
        {
            Emit(OpCodes.Stloc, local);
        }
        
        public void LoadField(FieldInfo field)
        {
            if (field.IsStatic)
                Emit(OpCodes.Ldsfld, field);
            else
                Emit(OpCodes.Ldfld, field);
        }
        
        public void StoreField(FieldInfo field)
        {
            Emit(OpCodes.Stfld, field);
        }
        
        public void StoreElementRef()
        {
            Emit(OpCodes.Stelem_Ref);
        }
        
        public void GetProperty(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();
            Call(getMethod);
        }

        public void SetProperty(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            Call(setMethod);
        }
        
        public void Branch(BlockLabel label)
        {
            Emit(OpCodes.Br, label);
        }
        
        public void BranchIfTrue(BlockLabel label)
        {
            Emit(OpCodes.Brtrue, label);
        }
        
        public void BranchIfFalse(BlockLabel label)
        {
            Emit(OpCodes.Brfalse, label);
        }
        
        public void CompareEqual()
        {
            Emit(OpCodes.Ceq);
        }

        public void BranchIfEqual(BlockLabel label)
        {
            Emit(OpCodes.Beq, label);
        }
        
        public void BranchIfNotEqual(BlockLabel label)
        {
            Emit(OpCodes.Bne_Un, label);
        }
        
        public void BranchIfLess(BlockLabel label)
        {
            Emit(OpCodes.Blt, label);
        }

        public void BranchIfLessOrEqual(BlockLabel label)
        {
            Emit(OpCodes.Ble, label);
        }

        public void BranchIfGreater(BlockLabel label)
        {
            Emit(OpCodes.Bgt, label);
        }
        
        public void BranchIfNull(BlockLabel label)
        {
            BranchIfFalse(label);
        }
        
        public void BranchIfNotNull(BlockLabel label)
        {
            BranchIfTrue(label);
        }
        
        public void Call(MethodInfo method)
        {
            Emit(OpCodes.Call, method);
        }

        public void CallVirt(MethodInfo method)
        {
            Emit(OpCodes.Callvirt, method);
        }
        
        public void Call(ConstructorInfo constructor)
        {
            Emit(OpCodes.Call, constructor);
        }
        
        public void New(Type type)
        {
            Emit(OpCodes.Newobj, type.GetConstructor(new Type[]{}));
        }
        
        public void New(ConstructorInfo constructor)
        {
            Emit(OpCodes.Newobj, constructor);
        }
        
        public void NewArray(Type type)
        {
            Emit(OpCodes.Newarr, type);
        }
        
        public void LoadStaticArray(Type type, int length)
        {
            Load(Array.CreateInstance(type, length));
        }
        
        public void Dup()
        {
            Emit(OpCodes.Dup);
        }
        
        public void Pop()
        {
            Emit(OpCodes.Pop);
        }
        
        public void Return()
        {
            Emit(OpCodes.Ret);
        }
        
        public void Add()
        {
            Emit(OpCodes.Add);
        }
        
        public void Sub()
        {
            Emit(OpCodes.Sub);
        }
        
        public void BitwiseNot()
        {
            Emit(OpCodes.Not);
        }
        
        public void LogicalNot()
        {
            Load(0);
            CompareEqual();
        }
        
        public void Increment()
        {
            Load(1);
            Add();
        }
        
        public void Print(string str)
        {
            Load(str);
            
            // TODO - load method only once
            
            Call(typeof(Console).GetMethod("WriteLine", new Type[]{typeof(string)}));
        }
        
        public void Emit(OpCode op)
        {
            Emit(op, null);
        }
        
        public void Emit(OpCode op, object parameter)
        {
            Emit(new Instruction(op, parameter));
        }

        public void DeclareLocal(BlockLocal local)
        {
            Emit(new BlockInstruction(BlockOperation.DeclareLocal, local));
        }
        
        public void MarkLabel(BlockLabel label)
        {
            Emit(new BlockInstruction(BlockOperation.MarkLabel, label));
        }
        
        public void BeginScope()
        {
            Emit(BlockInstruction.BeginScope);
        }
        
        public void EndScope()
        {
            Emit(BlockInstruction.EndScope);
        }
        
        public void Emit(IBlockLine line)
        {
            lines.Add(line);
        }
        
        public void Emit(Block block)
        {
            lines.AddRange(block.lines);
        }
        
        public void Dump()
        {
            System.CodeDom.Compiler.IndentedTextWriter indent = new System.CodeDom.Compiler.IndentedTextWriter(Console.Out);
            
            indent.WriteLine("--------");
            
            foreach (IBlockLine line in (IEnumerable<IBlockLine>) lines)
            {
                if ((line is BlockInstruction) && ((BlockInstruction) line).Operation == BlockOperation.BeginScope)
                    indent.Indent++;
                
                indent.WriteLine(line);
                
                if ((line is BlockInstruction) && ((BlockInstruction) line).Operation == BlockOperation.EndScope)
                    indent.Indent--;
            }
        }
        
        public void Build(IMethodBuilder builder)
        {
            //Dump();
            
            ILGenerator il = builder.GetILGenerator();
            
            Dictionary<BlockLabel, Label> labels
                = new Dictionary<BlockLabel, Label>();
            
            Dictionary<BlockLocal, LocalBuilder> locals
                = new Dictionary<BlockLocal, LocalBuilder>();
            
            foreach (IBlockLine line in (IEnumerable<IBlockLine>) lines)
            {
                BlockInstruction instruction = line as BlockInstruction;
                
                if ((instruction != null) && (instruction.Operation == BlockOperation.MarkLabel))
                    labels[(BlockLabel) instruction.Parameter] = il.DefineLabel();
            }
            
            foreach (IBlockLine line in (IEnumerable<IBlockLine>) lines)
            {
                if (line is Instruction)
                {
                    Instruction instruction = line as Instruction;
                    
                    if (instruction.Parameter == null)
                        il.Emit(instruction.Op);
                    else if (instruction.Parameter is int)
                        il.Emit(instruction.Op, (int) instruction.Parameter);
                    else if (instruction.Parameter is string)
                        il.Emit(instruction.Op, (string) instruction.Parameter);
                    else if (instruction.Parameter is Type)
                        il.Emit(instruction.Op, (Type) instruction.Parameter);
                    else if (instruction.Parameter is FieldInfo)
                        il.Emit(instruction.Op, (FieldInfo) instruction.Parameter);
                    else if (instruction.Parameter is MethodInfo)
                        il.Emit(instruction.Op, (MethodInfo) instruction.Parameter);
                    else if (instruction.Parameter is ConstructorInfo)
                        il.Emit(instruction.Op, (ConstructorInfo) instruction.Parameter);
                    else if (instruction.Parameter is BlockLabel)
                    {
                        BlockLabel blockLabel = (BlockLabel) instruction.Parameter;
                        
                        Label label;
                        
                        if (!labels.TryGetValue(blockLabel, out label))
                            throw new Exception("Label " + blockLabel.Name + " not marked");
                        
                        il.Emit(instruction.Op, label);
                    }
                    else if (instruction.Parameter is BlockLocal)
                        il.Emit(instruction.Op, locals[(BlockLocal) instruction.Parameter]);
                    else
                        throw new NotImplementedException(instruction.Op.Value + " " + TypeNames.GetName(instruction.Parameter.GetType()));
                }
                else if (line is BlockInstruction)
                {
                    BlockInstruction instruction = line as BlockInstruction;
                    
                    if (instruction.Operation == BlockOperation.Comment)
                        {}
                    else if (instruction.Operation == BlockOperation.MarkLabel)
                        il.MarkLabel(labels[(BlockLabel) instruction.Parameter]);
                    else if (instruction.Operation == BlockOperation.DeclareLocal)
                    {
                        BlockLocal local = (BlockLocal) instruction.Parameter;
                        locals[local] = il.DeclareLocal(local.Type);
                    }
                    else if (instruction.Operation == BlockOperation.BeginScope)
                        il.BeginScope();
                    else if (instruction.Operation == BlockOperation.EndScope)
                        il.EndScope();
                    else
                        throw new Exception();
                }        
                else
                    throw new Exception();
            }
        }
    }
}
