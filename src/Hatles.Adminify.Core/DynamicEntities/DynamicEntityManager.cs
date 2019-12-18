using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Dependency;

namespace Hatles.Adminify.DynamicEntities
{
    public class DynamicEntityManager : ISingletonDependency
    {
        private List<DynamicEntityInfo> _dynamicEntityInfos;
        public Type DynamicType;
        public Type DynamicDtoType;

        public DynamicEntityManager()
        {
            _dynamicEntityInfos = DynamicEntityRegistrar.GetDynamicEntityInfos();
            DynamicType = DynamicEntityTypeBuilder.CompileEntityType("MyDynamicAssembly.MyDynamicType", "MyDynamicAssembly", "MyDynamicModule",
                new List<DynamicEntityField>()
                {
                    new DynamicEntityField("MyField", typeof(string))
                });
            DynamicDtoType = DynamicEntityTypeBuilder.CompileType<EntityDto>("MyDynamicDtoAssembly.MyDynamicDtoType", "MyDynamicDtoAssembly", "MyDynamicDtoModule",
                new List<DynamicEntityField>()
                {
                    new DynamicEntityField("MyField", typeof(string))
                });
            _dynamicEntityInfos.Add(new DynamicEntityInfo(DynamicType, null));
        }

        public List<DynamicEntityInfo> GetDynamicEntityInfos()
        {
            return _dynamicEntityInfos;
        }
    }
}