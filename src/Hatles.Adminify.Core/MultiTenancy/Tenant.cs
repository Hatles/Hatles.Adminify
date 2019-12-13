using Abp.MultiTenancy;
using Hatles.Adminify.Authorization.Users;

namespace Hatles.Adminify.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
