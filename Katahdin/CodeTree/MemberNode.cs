using System;
using System.Reflection;
using System.Collections.Generic;


namespace Katahdin.CodeTree
{
    public class MemberNode : CodeTreeNode
    {
        private CodeTreeNode obj;
        private string member;
        
        public MemberNode(Source source, CodeTreeNode obj,
            string member)
			    : base(source)
        {
            this.obj = obj;
            this.member = member;
        }
        
        public override object Get(RuntimeState state,
            object[] parametersHint)
        {
            object obj = this.obj.Get(state, null);
            
            state.RunningSource = Source;
            
            return GetMember(
                obj,
                member,
                parametersHint,
                true);
        }
        
        public override void Set(RuntimeState state, object v)
        {
            object obj = this.obj.Get(state, null);
            
            state.RunningSource = Source;
            
            SetMember(
                obj,
                member,
                v,
                true);
        }
        
        public static object GetMember(object obj, string member,
            bool throwIfUndefined)
        {
            return GetMember(obj, member, null, throwIfUndefined);
        }
        
        public static object GetMember(object obj, string member,
            object[] parametersHint, bool throwIfUndefined)
        {
            if (obj is IScope)
            {
                // FIXME - shouldn't be using exceptions for this
                
                try
                {
                    return ((IScope) obj).GetName(member);
                }
                catch
                {
                }
            }

            MemberInfo memberInfo = FindMember(obj, member,
                parametersHint);
            
            if (memberInfo == null)
            {
                if (throwIfUndefined)
                    throw new Exception("can't find getable member "
                        + TextEscape.Quote(member) + " in "
                        + TypeNames.GetName(obj.GetType()));
                else
                    return null;
            }
            
            if (memberInfo is MethodInfo)
            {
                return new ClrObjectMethodBinding(obj, (MethodInfo) memberInfo);
            }
            else if (memberInfo is PropertyInfo)
            {
                return ((PropertyInfo) memberInfo).GetValue(obj, null);
            }
            else if (memberInfo is FieldInfo)
            {
                object fieldValue = ((FieldInfo) memberInfo).GetValue(obj);
                
                if (fieldValue is Method)
                    fieldValue = new ObjectMethodBinding(obj,
                        (Method) fieldValue);

                return fieldValue;
            }
            else
            {
                throw new Exception("can't get "
                    + TypeNames.GetName(memberInfo.GetType()));
            }
        }
        
        public static bool SetMember(object obj, string member, object v,
            bool throwIfUndefined)
        {
            if (obj is IScope)
            {
                // FIXME - shouldn't be using exceptions for this
                
                try
                {
                    ((IScope) obj).SetName(member, v);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            MemberInfo memberInfo = FindMember(obj, member, null);
            
            if (memberInfo == null)
            {
                if (throwIfUndefined)
                    throw new Exception("can't find setable " + member
                        + " in " + obj.GetType());
                else
                    return false;
            }
            
            if (memberInfo is PropertyInfo)
            {
                ((PropertyInfo) memberInfo).SetValue(obj, v, null);
            }
            else if (memberInfo is FieldInfo)
            {
                ((FieldInfo) memberInfo).SetValue(obj, v);
            }
            else
            {
                throw new Exception("can't set "
                    + TypeNames.GetName(memberInfo.GetType()));
            }
            
            return true;
        }
        
        private static MemberInfo FindMember(object obj, string member,
            object[] parametersHint)
        {
            // If it's a type, search for static members

            if (obj is Type)
            {
                for (Type n = (Type) obj; n != null; n = n.BaseType)
                {
                    // Properties

                    PropertyInfo property = n.GetProperty(member,
                        BindingFlags.Static | BindingFlags.Public
                        | BindingFlags.DeclaredOnly);

                    if (property != null)
                        return property;

                    // Fields

                    FieldInfo field = n.GetField(member,
                        BindingFlags.Static | BindingFlags.Public
                        | BindingFlags.DeclaredOnly);

                    if (field != null)
                        return field;
                        
                    // Methods

                    MethodInfo[] methods = n.GetMethods(
                        BindingFlags.Static | BindingFlags.Public
                        | BindingFlags.DeclaredOnly);

                    if (methods.Length > 0)
                    {
                        MethodInfo method = (MethodInfo) ChooseMethod(
                            methods, member, parametersHint);

                        if (method != null)
                            return method;
                    }
                }
            }

            // Search for instance members

            for (Type n = obj.GetType(); n != null; n = n.BaseType)
            {
                // Properties

                PropertyInfo property = n.GetProperty(member,
                    BindingFlags.Instance | BindingFlags.Public
                    | BindingFlags.DeclaredOnly);
                
                // fixme
                
                if ((property == null) && (member == "Count"))
                    property = n.GetProperty("Length",
                        BindingFlags.Instance | BindingFlags.Public
                        | BindingFlags.DeclaredOnly);
                
                if (property != null)
                    return property;

                // Fields

                FieldInfo field = n.GetField(member,
                    BindingFlags.Instance | BindingFlags.Public
                    | BindingFlags.DeclaredOnly);
                
                if (field != null)
                    return field;
                
                // Methods

                MethodInfo[] methods = n.GetMethods(
                    BindingFlags.Instance | BindingFlags.Public
                    | BindingFlags.DeclaredOnly);

                if (methods.Length > 0)
                {
                    MethodInfo method = (MethodInfo) ChooseMethod(
                        methods, member, parametersHint);

                    if (method != null)
                        return method;
                }
            }
            
            return null;
        }
        
        public static MethodBase ChooseMethod(MethodBase[] methods,
            string name, object[] parameterValues)
        {
            List<MethodBase> options = new List<MethodBase>();
            List<MethodBase> weakOptions = new List<MethodBase>();
            
            foreach (MethodBase method in methods)
            {
                if (name != null)
                {
                    if (method.Name != name)
                        continue;
                }
                
                bool add = false;
                bool addWeak = false;
                
                ParameterInfo[] parameterInfo = method.GetParameters();
                
                if (parameterValues == null)
                {
                    add = true;
                }
                else if (parameterInfo.Length == parameterValues.Length)
                {
                    add = true;
                }
                else if (parameterInfo.Length == parameterValues.Length + 1)
                {
                    if (parameterInfo[0].ParameterType
                        == typeof(RuntimeState))
                            add = true;
                }
                
                // TODO - check if can convert to rather than can assign from
                // TODO - work for calls prefixed with state
                
                //Console.WriteLine(parameterValues == null);
                
                if ((parameterValues != null) && (parameterInfo.Length == parameterValues.Length))
                {
                    for (int n = 0; n < parameterValues.Length; n++)
                    {
                        Type expected = parameterInfo[n].ParameterType;
                        
                        if (expected.IsByRef)
                            expected = expected.GetElementType();
                        
                        if (parameterValues[n] == null)
                            continue;
                        
                        Type have = parameterValues[n].GetType();
                        
                        if (!expected.IsAssignableFrom(have))
                        {
                            add = false;
                            break;
                        }
                        
                        if (expected == typeof(object))
                            addWeak = true;
                    }
                }
                
                if (add)
                {
                    if (addWeak)
                        weakOptions.Add(method);
                    else
                        options.Add(method);
                }
            }
            
            if (options.Count == 0)
                options = weakOptions;
            
            if (options.Count == 0)
                return null;
            
            if (options.Count > 1)
            {
                foreach (MethodBase option in options)
                    Console.WriteLine(option);
                
                throw new Exception("can't choose between " + options.Count
                    + " methods for " + name);
            }
            
            return options[0];
        }
    }
}
