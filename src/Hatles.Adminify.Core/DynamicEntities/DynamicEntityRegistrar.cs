using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Hatles.Adminify.DynamicEntities
{
    public static class DynamicEntityRegistrar
    {
        public static List<Type> GetDynamicEntityConfigurationTypes()
        {
            var types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain
                .GetAssemblies())
            {
                var configTypes = assembly
                    .GetTypes()
                    .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDynamicEntityConfiguration<>)));

                types.AddRange(configTypes);
            }

            return types;
        }
        
        public static List<DynamicEntityInfo> GetDynamicEntityInfos()
        {
            return GetDynamicEntityConfigurationTypes().Select(GetDynamicEntityInfo).ToList();
        }
        
        public static DynamicEntityInfo GetDynamicEntityInfo(Type configurationType)
        {
            return new DynamicEntityInfo(GetDynamicEntityType(configurationType), configurationType);
        }
        
        public static Type GetDynamicEntityType(Type configurationType)
        {
            var interfaceType = configurationType.GetInterface(typeof(IEntityTypeConfiguration<>).Name);
            return interfaceType.GetGenericArguments().Single();
        }
    }
}