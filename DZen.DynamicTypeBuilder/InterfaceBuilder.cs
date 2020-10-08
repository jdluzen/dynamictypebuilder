using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace DZen.DynamicTypeBuilder
{
    public class InterfaceBuilder : IInterfaceBuilder
    {
        internal static AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"Dynamic.{Guid.NewGuid()}"), AssemblyBuilderAccess.Run/*RunAndSave net48*/);
        //ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule", "dynamic.dll");//net48 version
        private static ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        private static Type CreateType<IInterface>(string name = default)
        {
            Type interfaceType = typeof(IInterface);
            TypeBuilder tb = GetTypeBuilder(interfaceType, interfaceType.Namespace, name ?? interfaceType.Name.Substring(1));
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (var field in interfaceType.GetProperties().Select(p => (p.Name, p.PropertyType)))
                CreateProperty(tb, interfaceType, field.Name, field.PropertyType);

            Type objectType = tb.CreateTypeInfo();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder(Type interfaceType, string @namespace, string typeName)
        {
            string typeSignature = @namespace + '.' + typeName;
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            tb.AddInterfaceImplementation(interfaceType);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, Type interfaceType, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField($"<{propertyName}>k__BackingField", propertyType, FieldAttributes.Private);
            AddCustomAttributes(fieldBuilder);

            PropertyBuilder propBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, CallingConventions.HasThis, propertyType, null);
            MethodBuilder getBuilder = tb.DefineMethod("get_" + propertyName,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig |
                MethodAttributes.Virtual |
                MethodAttributes.NewSlot |
                MethodAttributes.Final, propertyType, Type.EmptyTypes);
            AddCustomAttributes(getBuilder);
            ILGenerator getIl = getBuilder.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setBuilder =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig |
                  MethodAttributes.Virtual |
                  MethodAttributes.NewSlot |
                  MethodAttributes.Final,
                  null, new[] { propertyType });
            AddCustomAttributes(setBuilder);

            ILGenerator setIl = setBuilder.GetILGenerator();
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);
            setIl.Emit(OpCodes.Ret);

            //tb.DefineMethodOverride(getBuilder, interfaceType.GetProperty(propertyName).GetGetMethod());
            propBuilder.SetGetMethod(getBuilder);
            //tb.DefineMethodOverride(setBuilder, interfaceType.GetProperty(propertyName).GetSetMethod());
            propBuilder.SetSetMethod(setBuilder);
        }

        private static void AddCustomAttributes(FieldBuilder fieldBuilder)
        {
            fieldBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute)
                .GetConstructor(new Type[0]), new object[0]));
            fieldBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute)
                .GetConstructor(new Type[] { typeof(DebuggerBrowsableState) }), new object[] { DebuggerBrowsableState.Never }));
        }

        private static void AddCustomAttributes(MethodBuilder methodBuilder)
        {
            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute)
                .GetConstructor(new Type[0]), new object[0]));
        }

        public Type Implement<T>(string name = default)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException($"Type {typeof(T)} must be an interface");
            return CreateType<T>(name);
        }
    }
}
