using System;

namespace Hatles.Adminify.DynamicEntities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoAppServiceTypesAttribute : Attribute
    {

        public Type AppServiceTypesImplementation { get; }

        public Type AppServiceTypesImplementationWithPrimaryKey { get; }

        public bool WithDefaultAppServiceTypesInterfaces { get; set; }

        public AutoAppServiceTypesAttribute(
            Type appServiceTypesImplementation,
            Type appServiceTypesImplementationWithPrimaryKey)
        {
            AppServiceTypesImplementation = appServiceTypesImplementation;
            AppServiceTypesImplementationWithPrimaryKey = appServiceTypesImplementationWithPrimaryKey;
        }
    }
}