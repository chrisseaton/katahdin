using System;
using System.Collections;

namespace Katahdin.CodeTree
{
    public class ConvertNode : CodeTreeNode
    {
        private CodeTreeNode child;
        private CodeTreeNode type;
        
        public ConvertNode(Source source, CodeTreeNode child,
            CodeTreeNode type)
			    : base(source)
        {
            this.child = child;
            this.type = type;
        }
        
        public override object Get(RuntimeState state, object[] parametersHint)
        {
            object child = this.child.Get(state, null);
            Type type = (Type) this.type.Get(state, null);
            
            state.RunningSource = Source;
            
            return Convert(
                child,
                type);
        }

        public static object Convert(object o, Type type)
        {
            if ((o != null) && type.IsAssignableFrom(o.GetType()))
                return o;
            else if (type == typeof(bool))
                return ToBool(o);
            else if (type == typeof(double))
                return ToDouble(o);
            else if (type == typeof(int))
                return ToInt(o);
            else if (type == typeof(string))
                return ToString(o);
            else if (type.IsArray)
                return ToArray(o, type);
            else
                throw new NotImplementedException(
                    "No conversion to " + TypeNames.GetName(type) + " from "
                    + TypeNames.GetName(o.GetType()));
        }

        public static bool ToBool(object o)
        {
            if (o == null)
                return false;
            else if (o is bool)
                return (bool) o;
            else
                throw new NotImplementedException(
                    "No conversion to bool from " + TypeNames.GetName(o.GetType()));
        }

        public static int ToInt(object o)
        {
            if (o == null)
                return 0;
            else if (o is int)
                return (int) o;
            else if (o is double)
                return (int) (double) o;
            else if (o is string)
                return System.Convert.ToInt32((string) o);
            else
                throw new NotImplementedException(
                    "No conversion to int from " + TypeNames.GetName(o.GetType()));
        }

        public static double ToDouble(object o)
        {
            if (o == null)
                return 0;
            else if (o is int)
                return (double) (int) o;
            else if (o is double)
                return (double) o;
            else if (o is string)
                return System.Convert.ToDouble((string) o);
            else
                throw new NotImplementedException(
                    "No conversion to double from " + TypeNames.GetName(o.GetType()));
        }

        public static string ToString(object o)
        {
            if (o == null)
                return "null";
            else
                return o.ToString();
        }
        
        public static Array ToArray(object o, Type type)
        {
            int length;
            
            if (o is Array)
                length = ((Array) o).Length;
            else if (o is IList)
                length = ((IList) o).Count;
            else
                throw new Exception();
            
            Array array = Array.CreateInstance(type.GetElementType(), length);
            
            for (int n = 0; n < length; n++)
            {
                object element = null;
                
                if (o is Array)
                    element = ((Array) o).GetValue(n);
                else if (o is IList)
                    element = ((IList) o)[n];
                
                array.SetValue(element, n);
            }
            
            return array;
        }
    }
}
