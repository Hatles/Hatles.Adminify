using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Hatles.Adminify.Controllers
{
    public abstract class AdminifyControllerBase: AbpController
    {
        protected AdminifyControllerBase()
        {
            LocalizationSourceName = AdminifyConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
