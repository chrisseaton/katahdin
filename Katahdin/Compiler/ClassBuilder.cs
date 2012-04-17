using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Collections;
using Katahdin.Grammars;

namespace Katahdin.Compiler
{
    public class ClassBuilder
    {
        private TypeBuilder typeBuilder;
        private Type type;
        
        private Pattern pattern;
        
        private List<Method> methods = new List<Method>();
        private List<Method> constructors = new List<Method>();
        private Set<string> instanceNames = new Set<string>();
        
        public ClassBuilder(CompilerModule compilerModule, Module module, string name, Type baseType)
        {
            if ((module != null) && (module.Parent != null))
                name = module.Namespace + "." + name;
            
            if ((baseType != typeof(RuntimeObject)) && !baseType.IsSubclassOf(typeof(RuntimeObject)))
                throw new Exception();
            
            while (compilerModule.ModuleBuilder.GetType(name) != null)
                name = "_" + name;
            
            typeBuilder = compilerModule.ModuleBuilder.DefineType(
                name,
                TypeAttributes.Public,
                baseType);
            
            typeBuilder.DefineField("pattern", typeof(Pattern),
                FieldAttributes.Public | FieldAttributes.Static);
        }
        
        public void AddField(string name)
        {
            instanceNames.Add(name);
        }
        
        public void AddMethod(Method method)
        {
            methods.Add(method);
        }
        
        public void AddConstructor(Method constructor)
        {
            constructors.Add(constructor);
        }
        
        public Type CreateType()
        {
            foreach (string name in instanceNames)
            {
                typeBuilder.DefineField(
                    name,
                    typeof(object),
                    FieldAttributes.Public);
            }
            
            foreach (Method method in methods)
            {
                typeBuilder.DefineField(
                    method.Name,
                    typeof(Method),
                    FieldAttributes.Public);
            }
            
            DefineParserConstructor();
            
            foreach (Method constructor in constructors)
                DefineConstructor(constructor);
            
            if (constructors.Count == 0)
                DefineDefaultConstructor();
            
            type = typeBuilder.CreateType();
            
            type.GetField("pattern").SetValue(null, pattern);
            
            pattern.SetType(type);
            
            return type;
        }
        
        private void DefineParserConstructor()
        {
            OrderedSet<string> fields = pattern.Fields;
            
            Type[] parameterTypes = new Type[1 + fields.Count];
            
            parameterTypes[0] = typeof(Source);
            
            for (int n = 1; n < parameterTypes.Length; n++)
                parameterTypes[n] = typeof(object);

            ConstructorBuilder builder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes);
            
            Block generator = new Block();
            
            // Call the base constructor
            
            ConstructorInfo baseConstructor
                = typeBuilder.BaseType.GetConstructor(new Type[]{typeof(Source)});
            
            if (baseConstructor == null)
                throw new Exception();
            
            generator.LoadArg(0);
            generator.LoadArg(1);
            generator.Call(baseConstructor);
            
            SetMethodsToInitialValues(generator);
            
            // Save the fields
            
            for (int n = 0; n < fields.Count; n++)
            {
                generator.LoadArg(0);
                generator.LoadArg(n + 2);
                generator.StoreField(typeBuilder.GetField(fields[n]));
            }
            
            generator.Return();
            
            generator.Build(new ConstructorBuilderProxy(builder));
        }
        
        private void DefineDefaultConstructor()
        {
            ConstructorBuilder builder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[0]);
            
            Block generator = new Block();
            
            // Call the base constructor
            
            ConstructorInfo baseConstructor
                = typeBuilder.BaseType.GetConstructor(new Type[0]);
            
            if (baseConstructor != null)
            {
                generator.LoadArg(0);
                generator.Call(baseConstructor);
            }
            
            SetMethodsToInitialValues(generator);
            
            generator.Return();
            
            generator.Build(new ConstructorBuilderProxy(builder));
        }
        
        private void DefineConstructor(Method constructor)
        {
            Type[] parameters = new Type[1 + constructor.Parameters.Count];
            
            parameters[0] = typeof(RuntimeState);
            
            for (int n = 1; n < parameters.Length; n++)
                parameters[n] = typeof(object);
            
            ConstructorBuilder builder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameters);
            
            Block generator = new Block();
            
            SetMethodsToInitialValues(generator);
            
            if (!constructor.IsInstance)
                throw new Exception();
            
            MethodInfo callMethod = typeof(Method).GetMethod("Call", new Type[]{typeof(RuntimeState), typeof(object), typeof(object[])});
            
            if (callMethod == null)
                throw new Exception();
            
            generator.Load(constructor);
            generator.LoadArg(1);
            generator.LoadArg(0);
            
            generator.Load(constructor.Parameters.Count);
            generator.NewArray(typeof(object));
            
            for (int n = 0; n < constructor.Parameters.Count; n++)
            {
                generator.Dup();
                generator.Load(n);
                generator.LoadArg(n + 1);
                generator.StoreElementRef();
            }
            
            generator.Call(callMethod);
            generator.Pop();
            
            generator.Return();
            
            generator.Build(new ConstructorBuilderProxy(builder));
        }
        
        private void SetMethodsToInitialValues(Block generator)
        {
            foreach (Method method in methods)
            {
                generator.LoadArg(0);
                generator.Load(method);
                generator.StoreField(typeBuilder.GetField(method.Name));
            }
        }
        
        public TypeBuilder TypeBuilder
        {
            get
            {
                return typeBuilder;
            }
        }
        
        public Pattern Pattern
        {
            get
            {
                return pattern;
            }
            
            set
            {
                if (pattern != null)
                    throw new Exception();
                
                pattern = value;
                
                foreach (string field in pattern.Fields)
                    instanceNames.Add(field);
            }
        }
        
        public List<Method> Methods
        {
            get
            {
                return methods;
            }
        }
    }
}
