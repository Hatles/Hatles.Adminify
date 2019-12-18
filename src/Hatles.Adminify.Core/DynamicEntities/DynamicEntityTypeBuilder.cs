using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Abp.Domain.Entities;
using JetBrains.Annotations;

namespace Hatles.Adminify.DynamicEntities
{
    public static class DynamicEntityTypeBuilder
    {
        public static object CreateNewObject(string typeSignature, string assemblyName, string moduleName, List<DynamicEntityField> dynamicEntityFields)
        {
            var myType = CompileType(typeSignature, assemblyName, moduleName, dynamicEntityFields);
            return Activator.CreateInstance(myType);
        }
        
        public static object CreateNewInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
        
        public static TType CreateNewInstance<TType>()
        {
            return (TType) CreateNewInstance(typeof(TType));
        }

        public static Type CompileType(string typeSignature, string assemblyName, string moduleName,
            List<DynamicEntityField> dynamicEntityFields)
        {
            return CompileType(typeSignature, assemblyName, moduleName, dynamicEntityFields, null, new List<Type>());
        }
        
        public static Type CompileType<TEntity>(string typeSignature, string assemblyName, string moduleName,
            List<DynamicEntityField> dynamicEntityFields)
        {
            return CompileType(typeSignature, assemblyName, moduleName, dynamicEntityFields, typeof(TEntity));
        }
        
        public static Type CompileEntityType<TPrimaryKey>(string typeSignature, string assemblyName, string moduleName,
            List<DynamicEntityField> dynamicEntityFields)
        {
            return CompileType<Entity<TPrimaryKey>>(typeSignature, assemblyName, moduleName, dynamicEntityFields);
        }
        
        public static Type CompileEntityType(string typeSignature, string assemblyName, string moduleName,
            List<DynamicEntityField> dynamicEntityFields)
        {
            return CompileType<Entity>(typeSignature, assemblyName, moduleName, dynamicEntityFields);
        }
        
        public static Type CompileType(string typeSignature, string assemblyName, string moduleName,
            List<DynamicEntityField> dynamicEntityFields, Type inheritedTypes)
        {
            return CompileType(typeSignature, assemblyName, moduleName, dynamicEntityFields, inheritedTypes, new List<Type>());
        }

        public static Type CompileType(string typeSignature, string assemblyName, string moduleName, List<DynamicEntityField> dynamicEntityFields, [CanBeNull] Type inheritedTypes, List<Type> inheritedInterfaces)
        {
            TypeBuilder tb = GetTypeBuilder(typeSignature, assemblyName, moduleName, inheritedTypes);
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            
            foreach (var inheritedInterface in inheritedInterfaces)
                tb.AddInterfaceImplementation(inheritedInterface);
            
            // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
            foreach (var field in dynamicEntityFields)
                CreateProperty(tb, field.Name, field.Type);

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder(string typeSignature, string assemblyName, string moduleName, [CanBeNull] Type inheritedTypes)
        {
            var assemblyNameDef = new AssemblyName(assemblyName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyNameDef, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    inheritedTypes);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
