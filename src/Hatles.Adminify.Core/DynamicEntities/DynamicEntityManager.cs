using System;
using System.Collections.Generic;
using Abp.Dependency;

namespace Hatles.Adminify.DynamicEntities
{
    public class DynamicEntityManager : ISingletonDependency
    {
        private List<DynamicEntityInfo> _dynamicEntityInfos;

        public DynamicEntityManager()
        {
            _dynamicEntityInfos = DynamicEntityRegistrar.GetDynamicEntityInfos();
        }

        public List<DynamicEntityInfo> GetDynamicEntityInfos()
        {
            return _dynamicEntityInfos;
        }
    }
}