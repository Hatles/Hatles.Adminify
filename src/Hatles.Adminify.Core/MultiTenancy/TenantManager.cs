using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Hatles.Adminify.Authorization.Users;
using Hatles.Adminify.Editions;

namespace Hatles.Adminify.MultiTenancy
{
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository, 
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository, 
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore) 
            : base(
                tenantRepository, 
                tenantFeatureRepository, 
                editionManager,
                featureValueStore)
        {
        }
    }
}
