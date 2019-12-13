using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.MultiTenancy;
using Abp.Zero.EntityFrameworkCore;

namespace Hatles.Adminify.EntityFrameworkCore.DynamicEntities
{
    public class AdminifyDynamicDbMigrator : AbpZeroDbMigrator<AdminifyDbContext>
    {
        public AdminifyDynamicDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver)
            : base(
                unitOfWorkManager,
                connectionStringResolver,
                dbContextResolver)
        {
        }
    }
}
