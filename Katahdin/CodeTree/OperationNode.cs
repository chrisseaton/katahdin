using System;


namespace Katahdin.CodeTree
{
    public enum Operation
    {
        Is,
        Not,
        Add,
        Subtract,
        Multiply,
        Divide,
        CompareLess,
        CompareLessOrEqual,
        CompareEquality,
        CompareInequality,
        CompareGreater,
        CompareGreaterOrEqual,
        And
    }
    
    public class OperationNode : CodeTreeNode
    {
        private Operation operation;
        private CodeTreeNode[] operands;
        
        public OperationNode(Source source, Operation operation,
			params CodeTreeNode[] operands)
				: base(source)
        {
            this.operation = operation;
            this.operands = operands;
            
            int expectedOperands;
            
            switch (operation)
            {
                case Operation.Not:
                    expectedOperands = 1;
                    break;
                
                case Operation.Is:
                case Operation.Add:
                case Operation.Subtract:
                case Operation.Multiply:
                case Operation.Divide:
                case Operation.CompareLess:
                case Operation.CompareLessOrEqual:
                case Operation.CompareEquality:
                case Operation.CompareInequality:
                case Operation.CompareGreater:
                case Operation.CompareGreaterOrEqual:
                case Operation.And:
                    expectedOperands = 2;
                    break;

                default:
                    throw new NotImplementedException();
            }
            
            if (operands.Length != expectedOperands)
                throw new Exception("Wrong number of operands for operation " + operation);
        }
        
        public override object Get(RuntimeState state, object[] parametersHint)
        {
            // Short-circuit operators
            
            switch (operation)
            {
                case Operation.And:
                {
                    if (!ConvertNode.ToBool(operands[0].Get(state)))
                        return false;
                    
                    return ConvertNode.ToBool(operands[1].Get(state));
                }
            }
            
            // Normal operators
            
            object[] operandValues = new object[operands.Length];
            
            for (int n = 0; n < operands.Length; n++)
                operandValues[n] = operands[n].Get(state);
            
            state.RunningSource = Source;
            
            switch (operation)
            {
                case Operation.Is:
                    return Is(
                        operandValues[0],
                        (Type) operandValues[1]);
                
                case Operation.Not:
                    return Not(operandValues[0]);
                
                case Operation.Add:
                    return Add(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.Subtract:
                    return Subtract(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.Multiply:
                    return Multiply(
                        operandValues[0],
                        operandValues[1]);
                        
                case Operation.Divide:
                    return Divide(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.CompareLess:
                    return CompareLess(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.CompareLessOrEqual:
                    return CompareLessOrEqual(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.CompareEquality:
                    return CompareEquality(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.CompareInequality:
                    return CompareInequality(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.CompareGreater:
                    return CompareGreater(
                        operandValues[0],
                        operandValues[1]);
                
                case Operation.CompareGreaterOrEqual:
                    return CompareGreaterOrEqual(
                        operandValues[0],
                        operandValues[1]);
                
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool Is(object obj, Type type)
        {
            if (obj == null)
                return false;
            
            return type.IsAssignableFrom(obj.GetType());
        }
        
        public static object Not(object a)
        {
            if (a is bool)
            {
                return !((bool) a);
            }
            else
            {
                throw new NotImplementedException(
                    "No not operator for "
                    + TypeNames.GetName(a.GetType()));
            }
        }
        
        public static object Add(object a, object b)
        {
            if (a is int)
            {
                return (int) a + ConvertNode.ToInt(b);
            }
            else if (a is string)
            {
                return (string) a + ConvertNode.ToString(b);
            }
            else
            {
                throw new NotImplementedException(
                    "No add operator for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static object Subtract(object a, object b)
        {
            if (a is int)
            {
                return (int) a - ConvertNode.ToInt(b);
            }
            else
            {
                throw new NotImplementedException(
                    "No subtract operator for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static object Multiply(object a, object b)
        {
            if (a is int)
            {
                return (int) a * ConvertNode.ToInt(b);
            }
            else
            {
                throw new NotImplementedException(
                    "No multiply operator for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }

        public static object Divide(object a, object b)
        {
            if (a is double || b is double)
                return ConvertNode.ToDouble(a) / ConvertNode.ToDouble(b);
            
            if (a is int)
                return (int) a / ConvertNode.ToInt(b);
            else if (a is double)
                return (double) a / ConvertNode.ToDouble(b);
            else
            {
                throw new NotImplementedException(
                    "No divide operator for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static bool CompareLess(object a, object b)
        {
            if (a is double || b is double)
                return ConvertNode.ToDouble(a) < ConvertNode.ToDouble(b);
            
            if (a is int)
                return (int) a < ConvertNode.ToInt(b);
            else if (a is double)
                return (double) a < ConvertNode.ToDouble(b);
            else
            {
                throw new NotImplementedException(
                    "No less comparision for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static bool CompareLessOrEqual(object a, object b)
        {
            if (a is double || b is double)
                return ConvertNode.ToDouble(a) <= ConvertNode.ToDouble(b);
            
            if (a is int)
                return (int) a <= ConvertNode.ToInt(b);
            else
            {
                throw new NotImplementedException(
                    "No less or equal comparision for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static bool CompareEquality(object a, object b)
        {
            if (a is double || b is double)
                return ConvertNode.ToDouble(a) == ConvertNode.ToDouble(b);
            
            if (a == null)
                return b == null;
            else if (b == null)
                return false;
            else if (a is bool)
                return (bool) a == ConvertNode.ToBool(b);
            else if (a is int)
                return (int) a == ConvertNode.ToInt(b);
            else if (a is double)
                return (double) a == ConvertNode.ToDouble(b);
            else if (a is char)
                return a.ToString() == ConvertNode.ToString(b);
            else if (a is string)
                return (string) a == ConvertNode.ToString(b);
            else if (a is char)
                return a.ToString() == ConvertNode.ToString(b);
            else
                return a == b;
        }
        
        public static bool CompareInequality(object a, object b)
        {
            if (a is double || b is double)
                return ConvertNode.ToDouble(a) != ConvertNode.ToDouble(b);
            
            if (a == null)
                return b != null;
            else if (b == null)
                return true;
            else if (a is int)
                return (int) a != ConvertNode.ToInt(b);
            else if (a is double)
                return (double) a != ConvertNode.ToDouble(b);
            else if (a is string)
                return (string) a != ConvertNode.ToString(b);
            else if (a is char)
                return a.ToString() != ConvertNode.ToString(b);
            else
            {
                throw new NotImplementedException(
                    "No inequality comparision for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static bool CompareGreater(object a, object b)
        {
            if (a is double || b is double)
                return ConvertNode.ToDouble(a) > ConvertNode.ToDouble(b);
            
            if (a is int)
                return (int) a > ConvertNode.ToInt(b);
            else if (a is double)
                return (double) a > ConvertNode.ToDouble(b);
            else
            {
                throw new NotImplementedException(
                    "No greater comparision for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static bool CompareGreaterOrEqual(object a, object b)
        {
            if (a is double || b is double)
                return ConvertNode.ToDouble(a) >= ConvertNode.ToDouble(b);
            
            if (a is int)
                return (int) a >= ConvertNode.ToInt(b);
            else if (a is double)
                return (double) a >= ConvertNode.ToDouble(b);
            else
            {
                throw new NotImplementedException(
                    "No greater or equal comparision for "
                    + TypeNames.GetName(a.GetType()) + " and "
                    + (b == null ? "null" : TypeNames.GetName(b.GetType())));
            }
        }
        
        public static bool And(object a, object b)
        {
            return ConvertNode.ToBool(a) && ConvertNode.ToBool(b);
        }
    }
}
